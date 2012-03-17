namespace Utilities.KeyboardHook
{
	using System;
	using System.Windows.Forms;

	public class KeyPressedEventArgs : EventArgs
	{
		private Keys _key;
		private ModifierKeys _modifier;

		internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
		{
			this._modifier = modifier;
			this._key = key;
		}

		public Keys Key
		{
			get
			{
				return this._key;
			}
		}

		public ModifierKeys Modifier
		{
			get
			{
				return this._modifier;
			}
		}
	}
}

