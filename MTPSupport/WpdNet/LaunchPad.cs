using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpdInterop;
using DriveInfo = UIO.DriveInfo;

namespace WpdNet
{
    class LaunchPad
    {
        static void Main()
        {
            UioTest();
            Console.WriteLine("Done, kick Anykey to be done!");
            Console.ReadKey();
        }

        private static void UioTest()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}",
                    drive.Name, 
                    drive.IsReady, 
                    drive.IsReady ? drive.VolumeLabel :"<n/a>",
                    drive.IsReady ? drive.TotalSize.ToString() :"<n/a>",
                    drive.IsReady ? drive.AvailableFreeSpace.ToString() :"<n/a>"
                    );
            }
            Console.WriteLine();
            var path = @"Mike's MX5::s10001:\tmp\poster.tmp";
            Console.WriteLine("Path <{0}> exists: {1}", path, UIO.Directory.Exists(path));  
        }

        private static void InteropTest()
        {
            //const string path = @"\Mike's Lumia\Phone\Documents\New Text Document.txt";
            const string path = @"\Mike's MX5\Память телефона\tmp\New Folder";
            var segments = path.Split(new[] {Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);
            using (var managert = new Manager())
            {
                var devices = managert.GetDevices();
                var device = devices.Single(d => d.Name == segments[0]);
                var drives = device.GetStorage();
                WpdFilesystemItem target = drives.Single(d => d.Name == segments[1]);
                var disposables = new List<WpdFilesystemItem>();
                var children = target.GetChildren();
                target = children.Single(c => c.Name == segments[2]);
                disposables.AddRange(children);

                const string content = "This is a text file!";
                var binary = System.Text.Encoding.UTF8.GetBytes(content);
                var fileProxy = target.CreateChildFile("TestFile from C#.txt", (ulong) binary.Length);
                fileProxy.WriteContent(binary, (uint) binary.Length);
                var item = fileProxy.CommitAndDispose();
                item.Dispose();

                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }
                foreach (var drive1 in drives)
                {
                    drive1.Dispose();
                }
                foreach (var device1 in devices)
                {
                    device1.Dispose();
                }
            }
        }
    }
}
