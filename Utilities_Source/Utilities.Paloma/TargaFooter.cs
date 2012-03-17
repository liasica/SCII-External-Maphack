namespace Utilities.Paloma
{
	using System;

	public class TargaFooter
	{
		private int intDeveloperDirectoryOffset;
		private int intExtensionAreaOffset;
		private string strReservedCharacter = string.Empty;
		private string strSignature = string.Empty;

		protected internal void SetDeveloperDirectoryOffset(int intDeveloperDirectoryOffset)
		{
			this.intDeveloperDirectoryOffset = intDeveloperDirectoryOffset;
		}

		protected internal void SetExtensionAreaOffset(int intExtensionAreaOffset)
		{
			this.intExtensionAreaOffset = intExtensionAreaOffset;
		}

		protected internal void SetReservedCharacter(string strReservedCharacter)
		{
			this.strReservedCharacter = strReservedCharacter;
		}

		protected internal void SetSignature(string strSignature)
		{
			this.strSignature = strSignature;
		}

		public int DeveloperDirectoryOffset
		{
			get
			{
				return this.intDeveloperDirectoryOffset;
			}
		}

		public int ExtensionAreaOffset
		{
			get
			{
				return this.intExtensionAreaOffset;
			}
		}

		public string ReservedCharacter
		{
			get
			{
				return this.strReservedCharacter;
			}
		}

		public string Signature
		{
			get
			{
				return this.strSignature;
			}
		}
	}
}

