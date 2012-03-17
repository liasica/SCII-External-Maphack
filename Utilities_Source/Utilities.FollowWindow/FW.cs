namespace Utilities.FollowWindow
{
	using System;
	using System.Drawing;
	using System.Threading;
	using Utilities.WinControl;

	public class FW
	{
		private IntPtr ExternalHwnd;
		public Thread Follow;
		public bool Following;
		public bool followsParent;
		private int offsetX;
		private int offsetY;
		private IntPtr ParentHwnd;
		public bool QuickStop;
		public WindowHandlingLocation winHanLocation;

		public FW(IntPtr ParentHwnd, IntPtr ExternalHwnd, bool followsParrent)
		{
			this.ParentHwnd = ParentHwnd;
			this.ExternalHwnd = ExternalHwnd;
			this.followsParent = followsParrent;
		}

		public FW(IntPtr ParentHwnd, IntPtr ExternalHwnd, bool followsParrent, WindowHandlingLocation winHanLocation)
		{
			this.ParentHwnd = ParentHwnd;
			this.ExternalHwnd = ExternalHwnd;
			this.followsParent = followsParrent;
			this.winHanLocation = winHanLocation;
		}

		public void Follower()
		{
			EnumWindowsItem childWindow = WC.GetChildWindow("Map", "");
			if (childWindow != null)
			{
				goto Label_0253;
			}
			return;
		Label_0249:
			Thread.Sleep(0x14d);
		Label_0253:
			if (WC.Hwnd2ID(childWindow.Handle) != 0)
			{
				try
				{
					int x = WC.winGetPosition(this.ParentHwnd).X;
					int y = WC.winGetPosition(this.ParentHwnd).Y;
					int width = WC.winGetSize(this.ParentHwnd).Width;
					int height = WC.winGetSize(this.ParentHwnd).Height;
					int num5 = WC.winGetPosition(this.ExternalHwnd).X;
					int num6 = WC.winGetPosition(this.ExternalHwnd).Y;
					int num7 = WC.winGetSize(this.ExternalHwnd).Width;
					int num8 = WC.winGetSize(this.ExternalHwnd).Height;
					if (this.followsParent)
					{
						switch (this.winHanLocation)
						{
							case WindowHandlingLocation.TopLeft2mid:
								WC.winSetPosition(this.ExternalHwnd, x + this.offsetX, y + this.offsetY);
								goto Label_0249;

							case WindowHandlingLocation.TopRight2mid:
								WC.winSetPosition(this.ExternalHwnd, ((x + width) - num7) - this.offsetX, y + this.offsetY);
								goto Label_0249;

							case WindowHandlingLocation.BottomLeft2mid:
								WC.winSetPosition(this.ExternalHwnd, x + this.offsetX, ((y + height) - num8) - this.offsetY);
								goto Label_0249;

							case WindowHandlingLocation.BottomRight2mid:
								WC.winSetPosition(this.ExternalHwnd, ((x + width) - num7) - this.offsetX, ((y + height) - num8) - this.offsetY);
								goto Label_0249;
						}
					}
					else
					{
						switch (this.winHanLocation)
						{
							case WindowHandlingLocation.TopLeft2mid:
								WC.winSetPosition(this.ParentHwnd, num5 + this.offsetX, num6 + this.offsetY);
								goto Label_0249;

							case WindowHandlingLocation.TopRight2mid:
								WC.winSetPosition(this.ParentHwnd, ((num5 + num7) - width) - this.offsetX, num6 + this.offsetY);
								goto Label_0249;

							case WindowHandlingLocation.BottomLeft2mid:
								WC.winSetPosition(this.ParentHwnd, num5 + this.offsetX, ((num6 + num8) - height) - this.offsetY);
								goto Label_0249;

							case WindowHandlingLocation.BottomRight2mid:
								WC.winSetPosition(this.ParentHwnd, ((num5 + num7) - width) - this.offsetX, ((num6 + num8) - height) - this.offsetY);
								goto Label_0249;
						}
					}
				}
				catch
				{
					this.Toggle(false);
				}
				goto Label_0249;
			}
		}

		public void Offset(Point p)
		{
			this.offsetX = p.X;
			this.offsetY = p.Y;
		}

		public void Offset(int x, int y)
		{
			this.offsetX = x;
			this.offsetY = y;
		}

		public void Start()
		{
			if (this.Follow == null)
			{
				this.Follow = new Thread(new ThreadStart(this.Follower));
				this.Follow.Name = "Follow Thread1";
				this.Follow.Start();
			}
			else if (this.Follow.ThreadState == ThreadState.Stopped)
			{
				this.Follow = new Thread(new ThreadStart(this.Follower));
				this.Follow.Name = "Follow Thread2";
				this.Follow.Start();
			}
			this.Following = true;
			this.QuickStop = false;
		}

		public void Toggle(bool On)
		{
			if (On)
			{
				this.Start();
			}
			else
			{
				this.Follow.Abort();
				this.Following = false;
			}
		}
	}
}

