using System;
using System.Collections.Generic;
using System.Linq;
using WpdInterop;

namespace UIO
{
    public class FileStream : IDisposable
    {
        private readonly bool _isMtp;
        private readonly System.IO.FileStream _baseFileStream;
        private readonly WpdFileCreationProxy _baseProxy;
        private readonly Device _device;

        public FileStream(string path, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share, ulong fileSizeForMtpWrite = 0)
        {
            string[] mtpSegments;
            if (Path.IsLocal(path))
            {
                _isMtp = false;
                _baseFileStream = new System.IO.FileStream(path, mode, access, share);
            }
            else if(Path.IsMtp(path, out mtpSegments))
            {
                if (mode != System.IO.FileMode.Create && mode != System.IO.FileMode.CreateNew)
                    throw new System.IO.IOException("Reading MTP is not supported yet");
                if(fileSizeForMtpWrite == 0)
                    throw new System.IO.IOException("Writing MTP requires projected file size");
                
                var dName = Path.GetDirectoryName(path);
                if (Directory.Exists(dName))
                {
                    _isMtp = true;
                    var pair = Path.GetDriveByMtpPathSegments(mtpSegments);
                    _device = pair.Item1;
                    using (var drive = pair.Item2)
                    {
                        var pathSegments = mtpSegments[3].Split(new[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                        var disposables = new List<IDisposable>();
                        WpdFilesystemItem child = drive;
                        for (int i = 0; i < pathSegments.Length - 1; i++)
                        {
                            var children = child.GetChildren();
                            disposables.AddRange(children);
                            child = children.Single(c =>
                                                        c.IsFolder &&
                                                        c.Name.Equals(pathSegments[i],StringComparison.OrdinalIgnoreCase));
                        }
                        _baseProxy = child.CreateChildFile(Path.GetFileName(path), fileSizeForMtpWrite);
                        disposables.ForEach(d => d.Dispose());
                    }
                }
                else
                {
                    throw new System.IO.IOException("Part of the directory path <" + dName + "> is not found");
                }
            }
            else
            {
                throw new System.IO.IOException("The path <" + path + "> is not recognized as IO.Path or WPD.Path");
            }
        }

        public void Dispose()
        {
            if (_isMtp)
            {
                _baseProxy.Dispose();
                _device.Dispose();
            }
            else
            {
                _baseFileStream.Dispose();
            }
        }

        public long Length
        {
            get { return _isMtp ? (long) _baseProxy.GetLength() : _baseFileStream.Length; }
        }

        public long Position
        {
            get
            {
                { return _isMtp ? (long)_baseProxy.GetPosition() : _baseFileStream.Position; }
            }
            set
            {
                if (_isMtp)
                {
                    _baseProxy.SetPosition((ulong) value);
                }
                else
                {
                    _baseFileStream.Position = value;
                }
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (_isMtp) throw new NotImplementedException();
            return _baseFileStream.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (_isMtp)
            {
                if(offset + count > buffer.Length) throw new IndexOutOfRangeException();
                if (offset > 0)
                {
                    var newArr = new byte[count];
                    Array.ConstrainedCopy(buffer, offset, newArr, 0, count);
                    buffer = newArr;
                }
                _baseProxy.WriteContent(buffer, (uint) count);
            }
            else
            {
                _baseFileStream.Write(buffer, offset, count);
            }
        }
    }
}
