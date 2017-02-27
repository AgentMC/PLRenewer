using System;
using System.Linq;
using WpdInterop;

namespace UIO
{
    public class DriveInfo
    {
        private DriveInfo(System.IO.DriveInfo di)
        {
            Name = di.Name;
            DriveType = di.DriveType;
            IsReady = di.IsReady;
            if (IsReady)
            {
                DriveFormat = di.DriveFormat;
                AvailableFreeSpace = di.AvailableFreeSpace;
                TotalFreeSpace = di.TotalFreeSpace;
                TotalSize = di.TotalSize;
                RootDirectory = di.RootDirectory;
                VolumeLabel = di.VolumeLabel;
            }
        }
        
        private DriveInfo(Device d, WpdDriveInfo di)
        {
            Name = d.Name + "::" + di.GetId() + ":\\";
            DriveType = System.IO.DriveType.Removable;
            IsReady = true;
            DriveFormat = "MTP";
            AvailableFreeSpace = (long) di.FreeSpace;
            TotalFreeSpace = AvailableFreeSpace;
            TotalSize = (long) di.Capacity;
            VolumeLabel = di.Name;
        }

        public static DriveInfo[] GetDrives()
        {
            var list = System.IO.DriveInfo.GetDrives()
                                          .Select(driveInfo => new DriveInfo(driveInfo))
                                          .ToList();
            using (var manager = new Manager())
            {
                foreach (var device in manager.GetDevices())
                {
                    foreach (var driveInfo in device.GetStorage())
                    {
                        if (!Path.IsLocal(driveInfo.GetId()))
                        {
                            list.Add(new DriveInfo(device, driveInfo));
                        }
                        driveInfo.Dispose();
                    }
                    device.Dispose();
                }

            }
            return list.ToArray();
        }

        public String Name { get; private set; }

        public System.IO.DriveType DriveType { get; private set; }

        public String DriveFormat { get; private set; }

        public bool IsReady { get; private set; }

        public long AvailableFreeSpace { get; private set; }

        public long TotalFreeSpace { get; private set; }

        public long TotalSize { get; private set; }

        public System.IO.DirectoryInfo RootDirectory { get; private set; }

        public String VolumeLabel { get; private set; }
    }
}
