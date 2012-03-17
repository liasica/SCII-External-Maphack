namespace _2cs_API
{
	using System;
	using System.Runtime.InteropServices;
	using Utilities.WinControl;
	using Utilities.WinControl.Mouse;

	public static class clsKeyboard
	{
		public static void CleanUp()
		{
			KeyUp(160);
			KeyUp(0xa2);
			KeyUp(0xa4);
		}

		public static void KeyDown(ushort vkKey)
		{
			M.INPUT pInputs = new M.INPUT {
				type = M.SendInputEventType.InputKeyboard
			};
			pInputs.mkhi.ki.wVk = vkKey;
			pInputs.mkhi.ki.wScan = 0;
			pInputs.mkhi.ki.dwFlags = 0;
			pInputs.mkhi.ki.time = 0;
			pInputs.mkhi.ki.dwExtraInfo = IntPtr.Zero;
			M.SendInput(1, ref pInputs, Marshal.SizeOf(pInputs));
		}

		public static void KeyUp(ushort vkKey)
		{
			M.INPUT pInputs = new M.INPUT {
				type = M.SendInputEventType.InputKeyboard
			};
			pInputs.mkhi.ki.wVk = vkKey;
			pInputs.mkhi.ki.wScan = 0;
			pInputs.mkhi.ki.dwFlags = 2;
			pInputs.mkhi.ki.time = 0;
			pInputs.mkhi.ki.dwExtraInfo = IntPtr.Zero;
			M.SendInput(1, ref pInputs, Marshal.SizeOf(pInputs));
		}

		public static void SendKey(ushort vkKey)
		{
			KeyDown(vkKey);
			KeyUp(vkKey);
		}

		public static void SendKeys(string characters)
		{
			if (!string.IsNullOrEmpty(characters))
			{
				for (int i = 0; i < characters.Length; i++)
				{
					if (characters[i] != '{')
					{
						goto Label_00FC;
					}
					int num2 = characters.IndexOf('}', i) - 1;
					if (num2 == -1)
					{
						throw new Exception("MissingEndBracket");
					}
					string str = characters.Substring(i + 1, num2 - i).ToLower();
					ushort vkKey = 0;
					switch (str)
					{
						case "shiftdown":
							KeyDown(0x10);
							goto Label_00ED;

						case "shiftup":
							KeyUp(0x10);
							goto Label_00ED;

						case "enter":
							vkKey = 13;
							goto Label_00ED;

						case "esc":
						case "escape":
							vkKey = 0x1b;
							goto Label_00ED;

						case "f1":
							vkKey = 0x70;
							goto Label_00ED;

						case "f10":
							vkKey = 0x79;
							break;

						case "bs":
						case "backspace":
							vkKey = 8;
							break;
					}
				Label_00ED:
					if (vkKey != 0)
					{
						SendKey(vkKey);
					}
					i = num2 + 1;
					goto Label_010E;
				Label_00FC:
					SendKey((ushort) WC.VkKeyScan(characters[i]));
				Label_010E:;
				}
			}
		}
	}
}

