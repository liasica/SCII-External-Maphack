namespace Utilities.WebTools
{
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Mail;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;
	using Utilities.PixelPower;
	using Utilities.ScreenShot;

	public static class WT
	{
		private static Attachment _attachment = null;
		private static string _body = "";
		private static string _subject = "";
		private static string[] blacklistedHosts = new string[] { "qazzy-desktop" };

		public static void CheckForSourceForgeUpdates(string sourceForgeProjectName, string currentVersion)
		{
			string str2 = FetchPage("https://sourceforge.net/projects/" + sourceForgeProjectName + "/files/");
			if (!string.IsNullOrEmpty(str2))
			{
				int startIndex = str2.IndexOf("<title>") + 7;
				int index = str2.IndexOf("</title>", startIndex);
				string oldValue = str2.Substring(startIndex, index - startIndex);
				oldValue = oldValue.Remove(oldValue.IndexOf(" - "));
				int num3 = str2.IndexOf("<table id=\"files_list\"");
				int num4 = str2.IndexOf("<tbody>", num3);
				int num5 = str2.IndexOf("title=\"", num4) + 7;
				int num6 = str2.IndexOf("\"", num5);
				string input = str2.Substring(num5, num6 - num5).Replace(oldValue, "").Replace("beta", "").Replace(" ", "").Replace(".zip", "").Replace(".msi", "");
				input = input.Remove(input.LastIndexOf('.'));
				Version version = Version.Parse(currentVersion);
				Version version2 = Version.Parse(input);
				if ((version < version2) && (MessageBox.Show("There is a new version availiable. Would you like to open the download page?", "New Version", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1) == DialogResult.Yes))
				{
					int num7 = str2.IndexOf("href=\"", num4) + 6;
					int num8 = str2.IndexOf("\"", num7);
					Process.Start(str2.Substring(num7, num8 - num7));
				}
			}
		}

		public static string FetchPage(string httpUrl)
		{
			StringBuilder builder = new StringBuilder();
			byte[] buffer = new byte[0x2000];
			Stream stream = FetchPageStream(httpUrl);
			if (stream == null)
			{
				return null;
			}
			string str = null;
			int count = 0;
			do
			{
				try
				{
					count = stream.Read(buffer, 0, buffer.Length);
				}
				catch
				{
					return null;
				}
				if (count != 0)
				{
					str = Encoding.ASCII.GetString(buffer, 0, count);
					builder.Append(str);
				}
			}
			while (count > 0);
			return builder.ToString();
		}

		public static Stream FetchPageStream(string httpUrl)
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(httpUrl);
				HttpWebResponse response = (HttpWebResponse) request.GetResponse();
				return response.GetResponseStream();
			}
			catch
			{
				return null;
			}
		}

		public static DialogResult InputBox(string title, string promptText, ref string value)
		{
			Form form = new Form();
			Label label = new Label();
			TextBox box = new TextBox();
			Button button = new Button();
			Button button2 = new Button();
			form.Text = title;
			label.Text = promptText;
			box.Text = value;
			button.Text = "OK";
			button2.Text = "Cancel";
			button.DialogResult = DialogResult.OK;
			button2.DialogResult = DialogResult.Cancel;
			label.SetBounds(9, 20, 0x174, 13);
			box.SetBounds(12, 0x24, 0x174, 20);
			button.SetBounds(0xe4, 0x48, 0x4b, 0x17);
			button2.SetBounds(0x135, 0x48, 0x4b, 0x17);
			label.AutoSize = true;
			box.Anchor |= AnchorStyles.Right;
			button.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
			button2.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
			form.ClientSize = new Size(0x18c, 0x6b);
			form.Controls.AddRange(new Control[] { label, box, button, button2 });
			form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.StartPosition = FormStartPosition.CenterScreen;
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.AcceptButton = button;
			form.CancelButton = button2;
			DialogResult result = form.ShowDialog();
			value = box.Text;
			return result;
		}

		public static void ReportCrash(Exception ex, string appplicationTitle, IntPtr? handleToScreenShot = new IntPtr?(), Size? screenShotSize = new Size?(), bool messageBox = true)
		{
			throw ex;

			Func<string, bool> predicate = null;
			Attachment attachment = null;
			if (handleToScreenShot.HasValue)
			{
				Bitmap windowImage = CaptureScreen.GetWindowImage(handleToScreenShot.Value);
				Bitmap bitmap2 = PixelTools.FixedSize(windowImage, screenShotSize.Value.Width, screenShotSize.Value.Height);
				MemoryStream stream = new MemoryStream();
				bitmap2.Save(stream, ImageFormat.Jpeg);
				stream.Position = 0L;
				bitmap2.Dispose();
				windowImage.Dispose();
				attachment = new Attachment(stream, "snapshot.jpg");
			}
			string dns = Dns.GetHostName();
			string body = ((("Computer: " + dns) + "\n\n" + ex.Message) + "\n\n" + "Main Exception") + "\n" + ex.StackTrace;
			Exception innerException = ex.InnerException;
			for (int i = 1; innerException != null; i++)
			{
				body = ((body + "\n\n") + "Inner Exception #" + i) + "\n" + innerException.StackTrace;
				innerException = innerException.InnerException;
			}
			if (messageBox)
			{
				DialogResult result = MessageBox.Show("An unexpected error has occursed. This problem has been reported. Would you like to help debug this problem?", "Unexpected Crash", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
				if (predicate == null)
				{
					predicate = f => f.ToLower() == dns.ToLower();
				}
				if (blacklistedHosts.FirstOrDefault<string>(predicate) != null)
				{
					return;
				}
				if (result == DialogResult.Yes)
				{
					string str2 = "";
					if (InputBox(appplicationTitle + ": Crash Report", "Enter an email where you can be contacted at.", ref str2) == DialogResult.OK)
					{
						body = "Contact Email: " + str2 + "\n\n" + body;
					}
				}
			}
			SendEmail(appplicationTitle + ": Crash Report", body, attachment);
		}

		public static void SendEmail(string subject, string body)
		{
			if ((subject == "") || (body == ""))
			{
				throw new Exception("Email needs a subject and body!");
			}
			_subject = subject;
			_body = body;
			new Thread(new ThreadStart(WT.sendEmailThread)) { Name = "Send Email Thread" }.Start();
		}

		public static void SendEmail(string subject, string body, Attachment attachment)
		{
			_attachment = attachment;
			SendEmail(subject, body);
		}

		private static void sendEmailThread()
		{
			try
			{
				MailMessage message = new MailMessage();
				SmtpClient client = new SmtpClient("smtp.gmail.com");
				message.From = new MailAddress("noreply@2csUG.com");
				message.To.Add("2csUG.SC2@gmail.com");
				message.Subject = _subject;
				message.Body = _body;
				if (_attachment != null)
				{
					message.Attachments.Add(_attachment);
				}
				client.Port = 0x24b;
				client.Credentials = new NetworkCredential("2csUG.SC2", "2csUGftw");
				client.EnableSsl = true;
				client.Send(message);
				if (_attachment != null)
				{
					_attachment.Dispose();
				}
			}
			catch
			{
			}
		}
	}
}

