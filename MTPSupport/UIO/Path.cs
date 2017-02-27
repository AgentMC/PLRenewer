using System;
using System.Collections.Generic;
using System.Linq;
using WpdInterop;

namespace UIO
{
    public static class Path
    {
        internal enum HeadType
        {
            Any,
            File,
            Directory
        }

        internal static bool IsLocal(string path)
        {
            return (path.StartsWith(@"\\") || //network
                    (path.Length > 2 && path[1] == ':' && path[2] == '\\')); //local
        }

        internal static bool IsMtp(string path, out string[] name_Empty_DriveId_Path)
        {
            if (path.Contains("::"))
            {
                //device name, empty, itemId, path
                var majorSegments = path.Split(new[] {':'});
                if (majorSegments.Length == 4 && majorSegments[1] == string.Empty)
                {
                    name_Empty_DriveId_Path = majorSegments;
                    return true;
                }
            }
            name_Empty_DriveId_Path = null;
            return false;
        }

        internal static Tuple<Device, WpdDriveInfo> GetDriveByMtpPathSegments(string[] mtpPathSegments)
        {
            Tuple<Device, WpdDriveInfo> result = null;
            using (var manager = new Manager())
            {
                foreach (var device in manager.GetDevices())
                {
                    if (result == null && device.Name.Equals(mtpPathSegments[0], StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var driveInfo in device.GetStorage())
                        {
                            if (result == null &&
                                driveInfo.GetId().Equals(mtpPathSegments[2], StringComparison.OrdinalIgnoreCase))
                            {
                                result = Tuple.Create(device, driveInfo);
                            }
                            else
                            {
                                driveInfo.Dispose();
                            }
                        }
                    }
                    if (result == null || device != result.Item1)
                    {
                        device.Dispose();
                    }
                }
            }
            return result;
        }

        internal static bool Exists(string path, HeadType expectedObjectType)
        {
            if (IsLocal(path))
            {
                switch (expectedObjectType)
                {
                    case HeadType.Directory:
                        return System.IO.Directory.Exists(path);
                    case HeadType.File:
                        return System.IO.File.Exists(path);
                    default:
                        throw new Exception("The path <" + path +
                                            "> is local/UNC but no type switch (dir/file) provided.");
                }
            }
            bool found = false;
            string[] majorSegments;

            if (IsMtp(path, out majorSegments))
            {
                var pair = GetDriveByMtpPathSegments(majorSegments);
                using ( /*device*/ pair.Item1)
                using (var drive = pair.Item2)
                {
                    var pathSegments =
                        majorSegments[3].Split(new[] {System.IO.Path.DirectorySeparatorChar},
                                               StringSplitOptions.RemoveEmptyEntries);
                    WpdFilesystemItem target = drive;
                    var disposables = new List<IDisposable>();
                    for (int i = 0; i < pathSegments.Length; i++)
                    {
                        var children = target.GetChildren();
                        disposables.AddRange(children);
                        target = children.FirstOrDefault(
                            c => c.Name.Equals(pathSegments[i]) &&
                                 (expectedObjectType == HeadType.Directory
                                     ? c.IsFolder //all segments point to folders, even the last one
                                     : (expectedObjectType == HeadType.File
                                         ? (c.IsFolder == (i != pathSegments.Length - 1))
                                         //all folders except the last one, which is file
                                         : (c.IsFolder || (i == pathSegments.Length - 1)))));
                            //all folders, excet the last one, which can be both 
                        if (target == null) break;
                    }
                    found = target != null;
                    disposables.ForEach(d => d.Dispose());
                }
            }
            return found;
        }

        public static string GetDirectoryName(string path)
        {
            return IsLocal(path)
                ? System.IO.Path.GetDirectoryName(path)
                : path.Substring(0, path.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
        }

        public static string GetFileName(string path)
        {
            path = path.TrimEnd(System.IO.Path.DirectorySeparatorChar);
            return IsLocal(path)
                ? System.IO.Path.GetFileName(path)
                : path.Substring(path.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1);
        }

        internal static void Delete(string path, bool dir, bool recursive4Dirs = true)
        {
            if (IsLocal(path))
            {
                if (dir)
                    System.IO.Directory.Delete(path, recursive4Dirs);
                else
                    System.IO.File.Delete(path);
            }

            string[] segments;
            if (!IsMtp(path, out segments))
                throw new System.IO.IOException("Path is not identified as IO.Path or MTP.Path");

            var pair = GetDriveByMtpPathSegments(segments);
            if (pair == null) return; //no such device or disk available anymore

            using ( /*device*/ pair.Item1)
            using (var drive = pair.Item2)
            {
                var pathSegments = segments[3].Split(new[] {System.IO.Path.DirectorySeparatorChar},
                                                     StringSplitOptions.RemoveEmptyEntries);
                WpdFilesystemItem target = drive;
                var disposables = new List<IDisposable>();
                for (int i = 0; i < pathSegments.Length; i++)
                {
                    var children = target.GetChildren();
                    disposables.AddRange(children);
                    target = children.FirstOrDefault(
                        c => c.Name.Equals(pathSegments[i]) &&
                             c.IsFolder
                             || (!dir && i == pathSegments.Length - 1));
                        //all folders, excet the last one, which depends on the switch
                    if (target == null) break;
                }
                Action disposeAll = () => disposables.ForEach(d => d.Dispose());
                if (target != null)
                {
                    if (target == drive)
                    {
                        disposeAll();
                        throw new System.IO.IOException("Cannot delete a drive on MTP device");
                    }
                    if (dir && !recursive4Dirs)
                    {
                        var temp = target.GetChildren();
                        if (temp.Length > 0)
                        {
                            disposables.AddRange(temp);
                            disposeAll();
                            throw new System.IO.IOException("The target directory contains child entries!");
                        }
                    }
                    target.Delete();
                }
                disposeAll();
            }
        }
    }
}
