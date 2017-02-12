using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Archiver;

namespace Plu
{
	static class Program
	{
		const string SCRIPT_FILE_NAME = "script.txt";
		static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Usage: PLU <source file> <destination file> [<process name to monitor>]\n");
				Console.WriteLine("=== or new style ===\n");
				Console.WriteLine("       PLU /hdz <update hdz> /path <launch file> [/exe <process to monitor>]\n");
				return;
			}
			try
			{
				var dest = args[0] != "/hdz" ? OldImpl(args) : NewImpl(args);
				Console.WriteLine("Starting {0}...", dest);
				Process.Start(dest);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Fatal exception:\n" + ex.Message);
				Console.ReadKey(true);
			}
		}

		private static string NewImpl(string[] args)
		{
			string prcName = "", deployPath = "", deployBin = "", srcHdz = "", srcPath = "";
			for (var i = 0; i < args.Length; i++)
			{
				switch(args[i])
				{
					case "/hdz":
						srcHdz = args[++i];
						srcPath = srcHdz.Substring(0, srcHdz.LastIndexOf('\\') + 1);
						break;
					case "/path":
						deployBin = args[++i];
						deployPath = deployBin.Substring(0, deployBin.LastIndexOf('\\') + 1);
						prcName = deployBin.Substring(deployPath.Length);
						break;
					case "/exe":
						prcName = args[++i];
						break;
					default:
						throw new ArgumentException("Invalid command line!");
				}
			}
			prcName = prcName.Trim('"');
			deployPath = deployPath.Trim('"');
			deployBin = deployBin.Trim('"');
			srcHdz = srcHdz.Trim('"');
			srcPath = srcPath.Trim('"');
			WaitForExit(prcName);
			var hdz = new HdzArchive(srcHdz);
			if (!hdz.HdzItems.ContainsKey(SCRIPT_FILE_NAME))
				throw new ArgumentException("HDZ doesn't contain " + SCRIPT_FILE_NAME);
			hdz.ExtractItemsFromHdz();
			foreach (var fn in hdz.HdzItems.Keys)
			{
				if (fn == SCRIPT_FILE_NAME)
					continue;
				EnsureFileMoved(srcPath + fn, deployPath + fn);
			}
			using (var stream = new StreamReader(srcPath + SCRIPT_FILE_NAME, System.Text.Encoding.Default))
			{
				var empty = false;
				do
				{
					string line = stream.ReadLine();
					if (string.IsNullOrEmpty(line))
					{
						empty = true;
					}
					else
					{ 
						var command = line.Substring(0, 2);
						string arg1, arg2 = "";
						var idx = line.IndexOf("::");
						if (idx > 0)
						{
							arg1 = deployPath + line.Substring(3, idx - 3);
							arg2 = deployPath + line.Substring(idx + 2);
						}
						else
						{
							arg1 = deployPath + line.Substring(3);
						}
						switch(command)
						{
							case "mf":
								EnsureFileMoved(arg1, arg2);
								break;
							case "md":
								EnsureFSOperation(arg1, arg2, "{0} is moved to {1} successfully.", FileOps.Md);
								break;
							case "cd":
								EnsureFSOperation(arg1, null, "{0} is created successfully.", FileOps.Cd);
								break;
							case "df":
								EnsureFSOperation(arg1, null, "{0} is deleted successfully.", FileOps.Df);
								break;
							case "dd":
								EnsureFSOperation(arg1, null, "{0} is deleted successfully.", FileOps.Dd);
								break;
							default:
								throw new Exception(string.Format("Invalid command «{0}» in script file!", command));
						}
					}
				} while (!empty);
			}
			EnsureFSOperation(srcPath + SCRIPT_FILE_NAME, null, "{0} is cleaned up successfully.", FileOps.Df);
			return deployBin;
		}

		private static string OldImpl(string[] args)
		{
			var prcName = args.Length > 2 ? args[2] : args[1].Substring(args[1].LastIndexOf('\\') + 1);
			prcName = prcName.Substring(0, prcName.LastIndexOf('.'));
			var dest = args[1].Trim('"');
			var src = args[0].Trim('"');
			WaitForExit(prcName);
			EnsureFileMoved(src, dest);
			return dest;
		}

		private static void EnsureFileMoved(string src, string dest)
		{
			EnsureFSOperation(src, dest, "{0} is moved to {1} successfully.", FileOps.Mf);
		}

		private static void EnsureFSOperation(string src, string dest, string message, FileOps operation)
		{
			var ok = false;
			while (!ok)
			{
				try
				{
					switch (operation)
					{
						case FileOps.Mf:
							if (File.Exists(src))
							{
								File.Copy(src, dest, true);
								File.Delete(src);
							}
							break;
						case FileOps.Md:
							if (Directory.Exists(src))
								Directory.Move(src, dest);
							break;
						case FileOps.Cd:
							if (!Directory.Exists(src))
								Directory.CreateDirectory(src);
							break;
						case FileOps.Dd:
							if (Directory.Exists(src))
								Directory.Delete(src, true);
							break;
						case FileOps.Df:
							if (File.Exists(src))
								File.Delete(src);
							break;
					}
					ok = true;
					Console.WriteLine(string.Format(message, src, dest));
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					Thread.Sleep(1000);
				}
			}
		}

		private enum FileOps
		{Mf, Md, Cd, Df, Dd}

		private static void WaitForExit(string prcName)
		{
			var worx = false;
			do
			{
				if (worx)
				{
					Thread.Sleep(1000);
					worx = false;
				}
				foreach (var prc in Process.GetProcesses())
				{
					if (prc.ProcessName.ToLower() != prcName.ToLower()) 
						continue;
					Console.WriteLine("Process {0} is active...", prcName);
					worx = true;
					break;
				}
			} while (worx);
			Console.WriteLine("Process {0} not found...", prcName);
		}
	}
}
