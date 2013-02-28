using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Utilities
{
	public partial class CrashReportWindow : Form
	{
		protected string FinalMessage = null;

		public static string Show(string Title, string Message, Exception ex)
		{
			string ReturnVal = string.Empty;
			CrashReportWindow Window = new CrashReportWindow(Title, Message, ex);
			try
			{
				if (Window.ShowDialog() != DialogResult.OK)
					return null;

				return Window.FinalMessage;
			}
			finally
			{
				Window.Dispose();
			}
		}

		protected CrashReportWindow(string Title, string Message, Exception ex)
		{
			InitializeComponent();
			this.Text = Title;
			labelMessage.Text = Message;
			textBoxError.Text = ex.ToString();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			FinalMessage = textBoxError.Text;
			if(!string.IsNullOrWhiteSpace(textBoxAdditionalInfo.Text))
				FinalMessage += "\n\nAditional Info:\n\"" + textBoxAdditionalInfo.Text + "\"";
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}
	}
}
