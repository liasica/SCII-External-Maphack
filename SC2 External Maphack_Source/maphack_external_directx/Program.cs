namespace maphack_external_directx
{
	using System;
	using System.Diagnostics;
	using System.Security.Principal;
	using System.Threading;
	using System.Windows.Forms;
	using Utilities.WebTools;
	using System.Reflection;

	

	internal static class Program
	{
		private static bool? _IsAdministrator = null;
		private static maphack_external_directx.MainWindow _mainWindow;
		public static string ApplicationTitle = "SC2 External Maphack";
		public static string ApplicationVersion = "1.14.4";

		[STAThread]
		private static void Main()
		{
			if (/*Debugger.IsAttached*/false)
			{
				runApplication();
			}
			else
			{
				try
				{
					runApplication();
				}
				catch (ThreadAbortException)
				{
				}
				catch (Exception exception)
				{
					WT.ReportCrash(exception, ApplicationTitle + " " + ApplicationVersion, null, null, true);
				}
			}
		}

		public static void MessageOk(string text, MessageBoxIcon icon)
		{
			MessageBox.Show(text, ApplicationTitle, MessageBoxButtons.OK, icon);
		}

		private static void runApplication()
		{
			if (!IsAdministrator)
			{
				MessageOk("This program requires to be ran as Administrator.", MessageBoxIcon.Exclamation);
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				_mainWindow = new maphack_external_directx.MainWindow();
				Application.Run(MainWindow);
			}
		}

		public static bool IsAdministrator
		{
			get
			{
				if (!_IsAdministrator.HasValue)
				{
					WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
					_IsAdministrator = new bool?(principal.IsInRole(WindowsBuiltInRole.Administrator));
				}
				return _IsAdministrator.Value;
			}
		}

		public static maphack_external_directx.MainWindow MainWindow
		{
			get
			{
				return _mainWindow;
			}
		}
	}
}

