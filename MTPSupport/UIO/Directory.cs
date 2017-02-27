using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpdInterop;

namespace UIO
{
    public class Directory
    {
        public static bool Exists(string path)
        {
            return Path.Exists(path, Path.HeadType.Directory);
        }

        public static void CreateDirectory(string path)
        {
            if (Path.IsLocal(path))
            {
                System.IO.Directory.CreateDirectory(path);
                return;
            }
            string[] mtpSegments;
            if (Path.IsMtp(path, out mtpSegments))
            {
                var pair = Path.GetDriveByMtpPathSegments(mtpSegments);
                using (/*device*/ pair.Item1)
                using (var drive = pair.Item2)
                {
                    var pathSegments = mtpSegments[3].Split(new []{System.IO.Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);
                    var disposables = new List<IDisposable>();
                    bool creationMode = false;
                    WpdFilesystemItem child = drive, existingDir = null;
                    foreach (var segm in pathSegments)
                    {
                        if (!creationMode)
                        {
                            var children = child.GetChildren();
                            disposables.AddRange(children);
                            existingDir = children.FirstOrDefault(c =>
                                c.IsFolder &&
                                c.Name.Equals(segm, StringComparison.OrdinalIgnoreCase));
                        }
                        if (existingDir != null)
                        {
                            child = existingDir;
                        }
                        else
                        {
                            creationMode = true;
                            child = child.CreateChildFolder(segm);
                            disposables.Add(child);
                        }
                    }
                    disposables.ForEach(d => d.Dispose());
                }
            }
            else
            {
                throw new System.IO.IOException("Path is not identified as IO.Path or MTP.Path");
            }
        }

        public static string GetFileSystemOpenPath(string uioPath)
        {
            if (Path.IsLocal(uioPath))
            {
                return uioPath;
            }

            string[] mtpSegments;
            if (!Path.IsMtp(uioPath, out mtpSegments))
                throw new System.IO.IOException("Path is not identified as IO.Path or MTP.Path");

            var finalPath = new StringBuilder(@"::{20D04FE0-3AEA-1069-A2D8-08002B30309D}\");
            var pair = Path.GetDriveByMtpPathSegments(mtpSegments);
            using (/*device*/ pair.Item1)
            using (var drive = pair.Item2)
            {
                finalPath.Append(pair.Item1.GetId())
                         .Append('\\')
                         .Append(pair.Item2.Name)
                         .Append('\\');
                var pathSegments = mtpSegments[3].Split(new[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                var disposables = new List<IDisposable>();
                WpdFilesystemItem child = drive;
                foreach (var segm in pathSegments)
                {
                    var children = child.GetChildren();
                    disposables.AddRange(children);
                    child = children.Single(c => c.IsFolder &&
                                                 c.Name.Equals(segm, StringComparison.OrdinalIgnoreCase));
                    finalPath.Append(child.Name).Append('\\');
                }
                disposables.ForEach(d => d.Dispose());
            }
            return finalPath.ToString();
        }

        public static string[] GetFiles(string uioPath)
        {
            return Path.IsLocal(uioPath)
                ? System.IO.Directory.GetFiles(uioPath)
                : GetDirectoryItems(uioPath, false);
        }

        public static string[] GetDirectories(string uioPath)
        {
            return Path.IsLocal(uioPath)
                ? System.IO.Directory.GetDirectories(uioPath)
                : GetDirectoryItems(uioPath, true);
        }

        private static string[] GetDirectoryItems(string uioPath, bool directories)
        {
            uioPath = uioPath.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            string[] mtpSegments;
            if (!Path.IsMtp(uioPath, out mtpSegments))
                throw new System.IO.IOException("Path is not identified as IO.Path or MTP.Path");

            var pair = Path.GetDriveByMtpPathSegments(mtpSegments);
            using (/*device*/ pair.Item1)
            using (var drive = pair.Item2)
            {
                var pathSegments = mtpSegments[3].Split(new[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                var disposables = new List<IDisposable>();
                var result = new List<string>();
                WpdFilesystemItem child = drive;
                for (int i = 0; i <= pathSegments.Length; i++)
                {
                    var children = child.GetChildren();
                    disposables.AddRange(children);
                    if (i < pathSegments.Length)
                    {
                        child = children.Single(c => c.IsFolder &&
                                                     c.Name.Equals(pathSegments[i],
                                                                   StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        result.AddRange(children.Where(c => c.IsFolder == directories)
                                                .Select(c => uioPath + System.IO.Path.DirectorySeparatorChar + c.Name));
                    }
                }
                disposables.ForEach(d => d.Dispose());
                return result.ToArray();
            }
        }

        public static void Delete(string uioPath, bool recursive)
        {
            Path.Delete(uioPath, true, recursive);
        }
    }
}
