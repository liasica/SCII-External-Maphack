namespace ScreenAPI
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Threading;
	using System.Windows.Forms;
	using Utilities.WinControl;

	public class SmartClicker : Utility
	{
		private dPoint bot_left;
		private double bot_length;
		private dPoint bot_right;
		private dPoint camera;
		public static Utilities.WinControl.RECT ClientRect;
		private double height;
		public static SmartClicker instance = null;
		private double left_slope;
		private double right_slope;
		private List<KeyValuePair<Msg, object>> run_list;
		private bool running;
		private List<Sequence> sequences;
		private dPoint top_left;
		private double top_length;
		private dPoint top_right;

		private SmartClicker(IntPtr hwnd, ScreenSize size) : base(hwnd, size)
		{
			this.sequences = new List<Sequence>();
			this.run_list = new List<KeyValuePair<Msg, object>>();
		}

		public void AddSequence(List<KeyValuePair<Msg, object>> sequence)
		{
			Sequence item = new Sequence(sequence);
			int num = 0;
			foreach (KeyValuePair<Msg, object> pair in sequence)
			{
				Msg key = pair.Key;
				object obj2 = pair.Value;
				switch (key)
				{
					case Msg.Left:
					case Msg.LeftDbl:
					case Msg.Right:
					case Msg.RightDbl:
						if (item.areas.Count == 0)
						{
							item.areas.Add(new Area((dPoint) obj2));
							item.change_cam.Add(num);
						}
						else
						{
							Area area;
							if (this.CheckArea(item.areas[item.areas.Count - 1], (dPoint) obj2, out area))
							{
								item.areas.Add(new Area((dPoint) obj2));
								item.change_cam.Add(num);
							}
							else
							{
								item.areas[item.areas.Count - 1] = area;
							}
						}
						break;
				}
				num++;
			}
			if (item.areas.Count == 0)
			{
				this.run_list.AddRange(sequence);
			}
			else
			{
				this.sequences.Add(item);
			}
		}

		public override bool ChangeSize(ScreenSize size)
		{
			if (this.sequences.Count != 0)
			{
				return false;
			}
			if (!base.ChangeSize(size))
			{
				return false;
			}
			double num4 = (this.margins.right - this.camera_center.X) / base.Cood2PixelRatio;
			double x = (this.camera_center.Y - this.margins.top) / base.Cood2PixelRatio;
			double a = Math.Atan(base.camera_dist / x);
			this.top_right = new dPoint();
			this.top_right.Y = (x / Math.Sin((a + base.camera_angle) - 1.5707963267948966)) * Math.Sin(3.1415926535897931 - a);
			double num2 = Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(base.camera_dist, 2.0));
			double num3 = (x / Math.Sin((a + base.camera_angle) - 1.5707963267948966)) * Math.Sin(1.5707963267948966 - base.camera_angle);
			this.top_right.X = (num4 / num2) * (num2 + num3);
			num4 = (this.margins.left - this.camera_center.X) / base.Cood2PixelRatio;
			x = (this.camera_center.Y - this.margins.top) / base.Cood2PixelRatio;
			a = Math.Atan(base.camera_dist / x);
			this.top_left = new dPoint();
			this.top_left.Y = (x / Math.Sin((a + base.camera_angle) - 1.5707963267948966)) * Math.Sin(3.1415926535897931 - a);
			num2 = Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(base.camera_dist, 2.0));
			num3 = (x / Math.Sin((a + base.camera_angle) - 1.5707963267948966)) * Math.Sin(1.5707963267948966 - base.camera_angle);
			this.top_left.X = (num4 / num2) * (num2 + num3);
			num4 = (this.margins.right - this.camera_center.X) / base.Cood2PixelRatio;
			x = (this.margins.bot - this.camera_center.Y) / base.Cood2PixelRatio;
			a = Math.Atan(base.camera_dist / x);
			this.bot_right = new dPoint();
			this.bot_right.Y = (-x / Math.Sin((1.5707963267948966 + base.camera_angle) - a)) * Math.Sin(a);
			num2 = Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(base.camera_dist, 2.0));
			num3 = (x / Math.Sin((1.5707963267948966 + base.camera_angle) - a)) * Math.Sin(1.5707963267948966 - base.camera_angle);
			this.bot_right.X = (num4 / num2) * (num2 - num3);
			num4 = (this.margins.left - this.camera_center.X) / base.Cood2PixelRatio;
			x = (this.margins.bot - this.camera_center.Y) / base.Cood2PixelRatio;
			a = Math.Atan(base.camera_dist / x);
			this.bot_left = new dPoint();
			this.bot_left.Y = (-x / Math.Sin((1.5707963267948966 + base.camera_angle) - a)) * Math.Sin(a);
			num2 = Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(base.camera_dist, 2.0));
			num3 = (x / Math.Sin((1.5707963267948966 + base.camera_angle) - a)) * Math.Sin(1.5707963267948966 - base.camera_angle);
			this.bot_left.X = (num4 / num2) * (num2 - num3);
			this.right_slope = (this.top_right.Y - this.bot_right.Y) / (this.top_right.X - this.bot_right.X);
			this.left_slope = (this.top_left.Y - this.bot_left.Y) / (this.top_left.X - this.bot_left.X);
			this.top_length = this.top_right.X - this.top_left.X;
			this.bot_length = this.bot_right.X - this.bot_left.X;
			this.height = this.top_right.Y - this.bot_right.Y;
			return true;
		}

		private bool CheckArea(Area a, dPoint p, out Area result)
		{
			double num;
			if (((a.top_right.Y >= p.Y) && (p.Y >= (a.top_right.Y - this.height))) && ((p.X >= ((a.top_right.X - this.top_length) + ((p.Y - a.top_right.Y) / this.left_slope))) && (p.X <= (a.top_right.X + ((p.Y - a.top_right.Y) / this.right_slope)))))
			{
				if (p.Y < a.bot_right.Y)
				{
					num = p.Y - a.bot_right.Y;
					a.bot_right.X += num / this.right_slope;
					a.bot_left.X += num / this.left_slope;
					a.bot_right.Y = p.Y;
					a.bot_left.Y = p.Y;
				}
				if (p.X < (a.top_left.X + ((p.Y - a.top_left.Y) / this.left_slope)))
				{
					a.top_left.X = ((a.top_left.Y - p.Y) / this.left_slope) + p.X;
					a.bot_left.X = ((a.bot_left.Y - p.Y) / this.left_slope) + p.X;
				}
				result = a;
				return false;
			}
			if (((a.top_left.Y >= p.Y) && (p.Y >= (a.top_left.Y - this.height))) && ((p.X >= (a.top_left.X + ((p.Y - a.top_left.Y) / this.left_slope))) && (p.X <= ((a.top_left.X + this.top_length) + ((p.Y - a.top_left.Y) / this.right_slope)))))
			{
				if (p.Y < a.bot_left.Y)
				{
					num = p.Y - a.bot_left.Y;
					a.bot_right.X += num / this.right_slope;
					a.bot_left.X += num / this.left_slope;
					a.bot_right.Y = p.Y;
					a.bot_left.Y = p.Y;
				}
				if (p.X > (a.top_right.X + ((p.Y - a.top_right.Y) / this.right_slope)))
				{
					a.top_right.X = ((a.top_right.Y - p.Y) / this.right_slope) + p.X;
					a.bot_right.X = ((a.bot_right.Y - p.Y) / this.right_slope) + p.X;
				}
				result = a;
				return false;
			}
			if ((((a.bot_right.Y + this.height) >= p.Y) && (p.Y >= a.bot_right.Y)) && ((p.X >= ((a.bot_right.X - this.top_length) + ((p.Y - a.bot_right.Y) / this.left_slope))) && (p.X <= (a.bot_right.X + ((p.Y - a.bot_right.Y) / this.right_slope)))))
			{
				if (p.Y > a.top_right.Y)
				{
					num = p.Y - a.top_right.Y;
					a.top_right.X += num / this.right_slope;
					a.top_left.X += num / this.left_slope;
					a.top_right.Y = p.Y;
					a.top_left.Y = p.Y;
				}
				if (p.X < (a.bot_left.X + ((p.Y - a.bot_left.Y) / this.left_slope)))
				{
					a.top_left.X = ((a.top_left.Y - p.Y) / this.left_slope) + p.X;
					a.bot_left.X = ((a.bot_left.Y - p.Y) / this.left_slope) + p.X;
				}
				result = a;
				return false;
			}
			if ((((a.bot_left.Y + this.height) >= p.Y) && (p.Y <= a.bot_left.Y)) && ((p.X >= (a.bot_left.X + ((p.Y - a.bot_left.Y) / this.left_slope))) && (p.X <= ((a.bot_left.X + this.top_length) + ((p.Y - a.bot_left.Y) / this.right_slope)))))
			{
				if (p.Y > a.top_left.Y)
				{
					num = p.Y - a.top_left.Y;
					a.top_right.X += num / this.right_slope;
					a.top_left.X += num / this.left_slope;
					a.top_right.Y = p.Y;
					a.top_left.Y = p.Y;
				}
				if (p.X > (a.bot_right.X + ((p.Y - a.bot_right.Y) / this.right_slope)))
				{
					a.top_right.X = ((a.top_right.Y - p.Y) / this.right_slope) + p.X;
					a.bot_right.X = ((a.bot_right.Y - p.Y) / this.right_slope) + p.X;
				}
				result = a;
				return false;
			}
			result = a;
			return true;
		}

		private bool CompareArea(Area a, Area b, out Area result)
		{
			Area area = new Area();
			if (((a.bot_left == a.bot_right) && (a.bot_left == a.top_right)) && (a.bot_left == a.top_left))
			{
				this.CheckArea(b, a.bot_left, out area);
			}
			else if (((b.bot_left == b.bot_right) && (b.bot_left == b.top_right)) && (b.bot_left == b.top_left))
			{
				this.CheckArea(a, b.bot_left, out area);
			}
			else
			{
				area.top_left.Y = (a.top_left.Y > b.top_left.Y) ? a.top_left.Y : b.top_left.Y;
				area.top_right.Y = area.top_left.Y;
				area.bot_left.Y = (a.bot_left.Y < b.bot_left.Y) ? a.bot_left.Y : b.bot_left.Y;
				area.bot_right.Y = area.bot_left.Y;
				area.top_left.X = ((area.top_left.Y - ((a.top_left.X < b.top_left.X) ? a.top_left.Y : b.top_left.Y)) / this.left_slope) + ((a.top_left.X < b.top_left.X) ? a.top_left.X : b.top_left.X);
				area.bot_left.X = ((area.bot_left.Y - area.top_left.Y) / this.left_slope) + area.top_left.X;
				area.top_right.X = ((area.top_right.Y - ((a.top_right.X > b.top_right.X) ? a.top_left.Y : b.top_left.Y)) / this.right_slope) + ((a.top_right.X < b.top_right.X) ? a.top_right.X : b.top_right.X);
				area.bot_right.X = ((area.bot_right.Y - area.top_right.Y) / this.right_slope) + area.top_left.X;
			}
			result = area;
			return ((((area.top_right.X - area.top_left.X) <= this.top_length) && (((area.top_right.X - (this.height / this.right_slope)) - (area.top_left.X - (this.height / this.left_slope))) <= this.bot_length)) && ((area.top_right.Y - area.bot_right.Y) <= this.height));
		}

		public static ScreenSize CurrentSize(IntPtr hwnd)
		{
			WC.GetClientRect(hwnd, out ClientRect);
			try
			{
				return (ScreenSize) Enum.Parse(typeof(ScreenSize), string.Concat(new object[] { "Size", ClientRect.Width, "x", ClientRect.Height }));
			}
			catch
			{
				return ScreenSize.SizeUnknown;
			}
		}

		public static SmartClicker GetInstance(IntPtr hwnd)
		{
			WC.GetClientRect(hwnd, out ClientRect);
			if (!WC.IsFullScreen(hwnd))
			{
				return null;
			}
			ScreenSize size = (ScreenSize) Enum.Parse(typeof(ScreenSize), string.Concat(new object[] { "Size", ClientRect.Width, "x", ClientRect.Height }));
			return GetInstance(hwnd, size);
		}

		public static SmartClicker GetInstance(IntPtr hwnd, ScreenSize size)
		{
			if ((hwnd == IntPtr.Zero) || !Utility.supported.Contains(size))
			{
				return null;
			}
			return (instance ?? new SmartClicker(hwnd, size));
		}

		public static bool IsSupported(IntPtr hwnd)
		{
			WC.GetClientRect(hwnd, out ClientRect);
			try
			{
				ScreenSize item = (ScreenSize) Enum.Parse(typeof(ScreenSize), string.Concat(new object[] { "Size", ClientRect.Width, "x", ClientRect.Height }));
				return Utility.supported.Contains(item);
			}
			catch
			{
				return false;
			}
		}

		public override void MoveCamera(dPoint p)
		{
			if (this.running)
			{
				base.MoveCamera(this.camera);
			}
			else
			{
				base.MoveCamera(p);
			}
		}

		private void ParseAndRun(KeyValuePair<Msg, object> pair)
		{
			Msg key = pair.Key;
			object obj2 = pair.Value;
			switch (key)
			{
				case Msg.Left:
					base.GameLeftClick((dPoint) obj2, false);
					return;

				case Msg.Right:
					base.GameRightClick((dPoint) obj2, false);
					return;

				case Msg.LeftDbl:
					base.GameLeftClick((dPoint) obj2, true);
					return;

				case Msg.RightDbl:
					base.GameRightClick((dPoint) obj2, true);
					return;

				case Msg.Move:
					this.MoveCamera((dPoint) obj2);
					return;

				case Msg.Key:
					base.SendKey((Keys) obj2);
					return;

				case Msg.Sleep:
					Thread.Sleep((int) obj2);
					return;
			}
		}

		public void Run()
		{
			this.running = true;
			int index = 0;
			int num2 = index;
			int num3 = index;
			int num4 = index + 1;
			while ((num2 < this.sequences.Count) && (num4 < this.sequences.Count))
			{
				Sequence sequence;
				Area area;
				if (this.CompareArea(this.sequences[num3].areas[this.sequences[num3].areas.Count - 1], this.sequences[num4].areas[this.sequences[num4].areas.Count - 1], out area))
				{
					this.sequences[num3].areas[this.sequences[num3].areas.Count - 1] = area;
					if (this.sequences[num4].change_cam.Count != 1)
					{
						num3 = num2 + 1;
					}
					this.sequences[num4].change_cam[0] = -1;
					sequence = this.sequences[num4];
					this.sequences.RemoveAt(num4);
					this.sequences.Insert(++num2, sequence);
				}
				else if (this.CompareArea(this.sequences[num4].areas[this.sequences[num4].areas.Count - 1], this.sequences[index].areas[this.sequences[index].areas.Count - 1], out area))
				{
					this.sequences[num4].areas[this.sequences[num4].areas.Count - 1] = area;
					this.sequences[index].change_cam[0] = -1;
					sequence = this.sequences[num4];
					this.sequences.RemoveAt(num4);
					this.sequences.Insert(index, sequence);
					num2++;
				}
				num4++;
				if (num4 == this.sequences.Count)
				{
					num2++;
					index = num2;
					num3 = num2;
					num4 = num2 + 1;
				}
			}
			for (num4 = 0; num4 < this.sequences.Count; num4++)
			{
				int num5 = -1;
				num3 = 0;
				while ((num5 == -1) && (num3 < this.sequences[num4].change_cam.Count))
				{
					num5 = this.sequences[num4].change_cam[num3++];
				}
				for (int i = 0; i < this.sequences[num4].sequence.Count; i++)
				{
					if (i == num5)
					{
						Area area2 = this.sequences[num4].areas[num3 - 1];
						this.camera = new dPoint(area2.bot_left.X + (((area2.bot_right.X - area2.bot_left.X) * this.bot_right.X) / this.bot_length), area2.bot_left.Y + (((area2.top_left.Y - area2.bot_left.Y) * Math.Abs(this.bot_left.Y)) / this.height));
						for (num5 = -1; (num5 == -1) && (num3 < this.sequences[num4].change_cam.Count); num5 = this.sequences[num4].change_cam[num3++])
						{
						}
					}
					this.ParseAndRun(this.sequences[num4].sequence[i]);
				}
			}
			for (num4 = 0; num4 < this.run_list.Count; num4++)
			{
				this.ParseAndRun(this.run_list[num4]);
			}
			this.sequences.Clear();
			this.run_list.Clear();
			this.running = false;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct Area
		{
			public dPoint top_right;
			public dPoint top_left;
			public dPoint bot_right;
			public dPoint bot_left;
			public Area(dPoint p)
			{
				this.top_right = p;
				this.top_left = p;
				this.bot_right = p;
				this.bot_left = p;
			}

			public static bool operator ==(SmartClicker.Area a, SmartClicker.Area b)
			{
				return ((((a.top_right == b.top_right) && (a.top_left == b.top_left)) && (a.bot_right == b.bot_right)) && (a.bot_left == b.bot_left));
			}

			public static bool operator !=(SmartClicker.Area a, SmartClicker.Area b)
			{
				return !(a == b);
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct Sequence
		{
			public List<KeyValuePair<Msg, object>> sequence;
			public List<SmartClicker.Area> areas;
			public List<int> change_cam;
			public Sequence(List<KeyValuePair<Msg, object>> s)
			{
				this.sequence = s;
				this.areas = new List<SmartClicker.Area>();
				this.change_cam = new List<int>();
			}
		}
	}
}

