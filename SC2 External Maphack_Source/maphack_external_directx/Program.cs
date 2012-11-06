namespace maphack_external_directx
{
	using System;
	using System.IO;
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
		public static string ApplicationVersion = "?.?.?.?";

		private static void OnUnhandledException(Object sender, UnhandledExceptionEventArgs e)
		{
			WT.ReportCrash((Exception)e.ExceptionObject, "oops!");
		}

		private static void OnFormsUnhandledException(Object sender, ThreadExceptionEventArgs e)
		{
			WT.ReportCrash(e.Exception, "oops!");
		}

		[STAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);
			Application.ThreadException += new ThreadExceptionEventHandler(OnFormsUnhandledException);

			try
			{
				if (!DirectX_HUDs.AeroEnabled)
					MessageBox.Show("Desktop Window Manager (DWM) is not enabled. Without DWM, the DirectX overlays used by this program will have a significant performance penalty and may not work properly.\n\nDWM is only available on Windows Vista and later, and requires the Aero theme.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				runApplication();
			}
			catch (ThreadAbortException)
			{
			}
			catch (Exception exception)
			{
				WT.ReportCrash(exception, ApplicationTitle + " " + ApplicationVersion);
			}
		}

		public static void MessageOk(string text, MessageBoxIcon icon)
		{
			MessageBox.Show(text, ApplicationTitle, MessageBoxButtons.OK, icon);
		}

		private static void runApplication()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			_mainWindow = new maphack_external_directx.MainWindow();
			Application.Run(MainWindow);
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

