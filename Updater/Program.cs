using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Security.Cryptography;

namespace Updater
{
	/// <summary>
	/// Contains information about an available update.
	/// </summary>
	public struct UpdateInfo
	{
		public string LocalFile;
		public string URL;
		public string Operations;
		public int CurrentSize;
		public int NewSize;

		public UpdateInfo(string localfile, string url, string operations, int currentsize, int newsize)
		{
			LocalFile = localfile;
			URL = url;
			Operations = operations;
			CurrentSize = currentsize;
			NewSize = newsize;
		}
	}
	/// <summary>
	/// Can be used by SCIIEMH to check for updates, before starting the updater.
	/// This is also used within Updater to check which files have updates available.
	/// </summary>
	public static class UpdateChecker
	{
		private static int LineComparer(string x, string y)
		{
			string[] SplitX = x.Split('|');
			string[] SplitY = y.Split('|');

			bool XIsLinked = SplitX[1].Contains('L');
			bool YIsLinked = SplitY[1].Contains('L');

			if (XIsLinked == YIsLinked)
				return 0;
			else
				return XIsLinked ? 1 : -1;
		}

		/// <summary>
		/// Checks for available updates and returns an array of filenames that have updates avaiable.
		/// </summary>
		/// <returns>An array of filenames that have updates available</returns>
		public static UpdateInfo[] UpdatesAvailable()
		{
			List<UpdateInfo> ReturnVal = new List<UpdateInfo>();

			Downloader dl = new Downloader();
			byte[] IndexFile = dl.Download("https://raw.github.com/MrNukealizer/SCII-External-Maphack/Main/UpdaterFileList.txt");
			if (IndexFile == null || IndexFile.Length == 0)
				return ReturnVal.ToArray();

			List<string> FileList = Encoding.UTF8.GetString(IndexFile).Replace("\r", "").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			string CurrentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + '\\';

			FileList.Sort(LineComparer);
			foreach (string s in FileList)
			{
				string[] file = s.Split('|');
				if (file.Length < 5)
					continue;

				bool exists = File.Exists(CurrentFolder + file[0]);
				if (file[1].Contains('D'))
				{
					if (exists)
					{
						int NewSize;
						if (!int.TryParse(file[4], out NewSize))
							NewSize = 0;
						ReturnVal.Add(new UpdateInfo(file[0], file[3], "D", -1, NewSize));
					}
					continue;
				}

				if (!exists)
				{
					int NewSize;
					if (!int.TryParse(file[4], out NewSize))
						NewSize = -1;
					ReturnVal.Add(new UpdateInfo(file[0], file[3], "C", -1, NewSize));
				}
				else
				{
					byte[] buffer;
					using (FileStream fs = new FileStream(CurrentFolder + file[0], FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						buffer = new byte[fs.Length];
						fs.Read(buffer, 0, (int)fs.Length);
					}

					if(file[1].Contains('V'))
					{
						FileVersionInfo version = FileVersionInfo.GetVersionInfo(CurrentFolder + file[0]);
						if (version.FileVersion != file[2])
						{
							int NewSize;
							if (!int.TryParse(file[4], out NewSize))
								NewSize = -1;
							ReturnVal.Add(new UpdateInfo(file[0], file[3], "R", buffer.Length, NewSize));
						}
						continue;
					}

					if (file[1].Contains('L'))
					{
						if (ReturnVal.Exists(x => x.LocalFile.ToLower() == file[2].ToLower()))
						{
							int NewSize;
							if (!int.TryParse(file[4], out NewSize))
								NewSize = -1;
							ReturnVal.Add(new UpdateInfo(file[0], file[3], "R", buffer.Length, NewSize));
						}
						continue;
					}

					MD5 md5 = MD5.Create();
					byte[] md5Result = md5.ComputeHash(buffer);
					string md5ResultAsString = string.Empty;
					foreach (byte b in md5Result)
						md5ResultAsString += b.ToString("x2");

					if (md5ResultAsString != file[2].ToLower())
					{
						int NewSize;
						if (!int.TryParse(file[4], out NewSize))
							NewSize = -1;
						ReturnVal.Add(new UpdateInfo(file[0], file[3], "R", buffer.Length, NewSize));
					}

				}
			}

			return ReturnVal.ToArray();
		}
	}

	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			bool Done = false;
			foreach (string s in args)
			{
				string arg = s.ToLower();
				if (arg.StartsWith("--us--"))
				{
					UpdateSelfPart2();
					return;
				}
				else if (arg.StartsWith("--dus--"))
				{
					Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\UpdaterUpdater.exe");
					Done = true;
				}
				
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm(Done));
		}

		public static void StartSCIIEMH()
		{
			string CurrentFile = Assembly.GetExecutingAssembly().Location;
			string SCIIEMHFile = Path.GetDirectoryName(CurrentFile) + "\\SC2 External Maphack.exe";

			ProcessStartInfo info = new ProcessStartInfo(SCIIEMHFile);
			Process p = Process.Start(info);
		}


		public static void Delete(string file)
		{
			bool SucessfullyDeleted = false;
			DateTime StartTime = DateTime.Now;
			while (!SucessfullyDeleted && DateTime.Now.Subtract(TimeSpan.FromSeconds(30)) < StartTime)
			{
				try
				{
					File.Delete(file);
					SucessfullyDeleted = true;
				}
				catch (IOException)
				{
					Thread.Sleep(100);
				}
				catch (UnauthorizedAccessException)
				{
					MessageBox.Show("Failed to delete the following file because access is denied:\n" + file, "Error");
					return;
				}
			}

			if (!SucessfullyDeleted)
			{
				MessageBox.Show("Failed to delete the following file within 30 seconds:\n" + file, "Error");
			}
		}

		/// <summary>
		/// Updates a file. This does not work on Updater itself.
		/// </summary>
		/// <param name="file">The name of the file to update</param>
		public static void Update(string file, string URL, Downloader downloader = null)
		{
			bool SucessfullyDeleted = false;
			DateTime StartTime = DateTime.Now;
			while (!SucessfullyDeleted && DateTime.Now.Subtract(TimeSpan.FromSeconds(30)) < StartTime)
			{
				try
				{
					File.Delete(file);
					SucessfullyDeleted = true;
				}
				catch (IOException)
				{
					Thread.Sleep(100);
				}
				catch (UnauthorizedAccessException)
				{
					MessageBox.Show("Failed to delete the following file because access is denied:\n" + file, "Error");
					return;
				}
			}

			if (!SucessfullyDeleted)
			{
				MessageBox.Show("Failed to delete the following file within 30 seconds:\n" + file, "Error");
			}

			//Not finished. This is just for debugging.
			if (downloader == null)
				downloader = new Downloader();


			string CurrentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";

			downloader.DownloadToFile(URL, CurrentFolder + file);
		}

		public static void UpdateSelfPart2()
		{
			string CurrentFile = Assembly.GetExecutingAssembly().Location;
			string TempFile = Path.GetDirectoryName(CurrentFile) + "\\Updater.exe";

			File.Copy(CurrentFile, TempFile, true);

			ProcessStartInfo info = new ProcessStartInfo(TempFile, "--DUS--");
			info.UseShellExecute = true;

			Process p = Process.Start(info);

			Application.Exit();
		}

		public static void UpdateSelf(string URL, Downloader downloader = null)
		{
			string CurrentFile = Assembly.GetExecutingAssembly().Location;
			string TempFile = Path.GetDirectoryName(CurrentFile) + "\\UpdaterUpdater.exe";
			Update("UpdaterUpdater.exe", URL, downloader);

			ProcessStartInfo info = new ProcessStartInfo(TempFile, "--US--");
			info.UseShellExecute = true;

			Process p = Process.Start(info);

			Application.Exit();
		}
	}
}
