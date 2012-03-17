namespace Utilities.KeyboardHook
{
	using System;

	[Flags]
	public enum ModifierKeys : uint
	{
		Alt = 1,
		Control = 2,
		Shift = 4,
		Win = 8
	}
}

