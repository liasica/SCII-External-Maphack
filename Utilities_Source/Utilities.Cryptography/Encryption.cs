namespace Utilities.Cryptography
{
	using System;
	using System.IO;
	using System.Security.Cryptography;
	using System.Text;

	public class Encryption
	{
		public static byte[] Decrypt(byte[] outputData, byte[] pwd, byte[] IV)
		{
			MemoryStream stream = new MemoryStream();
			Rijndael rijndael = Rijndael.Create();
			rijndael.Key = pwd;
			rijndael.IV = IV;
			CryptoStream stream2 = new CryptoStream(stream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
			stream2.Write(outputData, 0, outputData.Length);
			stream2.Close();
			return stream.ToArray();
		}

		public static string DecryptString(string Message, string Passphrase)
		{
			byte[] buffer;
			UTF8Encoding encoding = new UTF8Encoding();
			MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
			byte[] buffer2 = provider.ComputeHash(encoding.GetBytes(Passphrase));
			TripleDESCryptoServiceProvider provider2 = new TripleDESCryptoServiceProvider {
				Key = buffer2,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			};
			byte[] inputBuffer = Convert.FromBase64String(Message);
			try
			{
				buffer = provider2.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
			}
			finally
			{
				provider2.Clear();
				provider.Clear();
			}
			return encoding.GetString(buffer);
		}

		public static byte[] Encrypt(byte[] inputData, byte[] pwd, byte[] IV)
		{
			MemoryStream stream = new MemoryStream();
			Rijndael rijndael = Rijndael.Create();
			rijndael.Key = pwd;
			rijndael.IV = IV;
			CryptoStream stream2 = new CryptoStream(stream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
			stream2.Write(inputData, 0, inputData.Length);
			stream2.Close();
			return stream.ToArray();
		}

		public static string EncryptString(string Message, string Passphrase)
		{
			byte[] buffer;
			UTF8Encoding encoding = new UTF8Encoding();
			MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
			byte[] buffer2 = provider.ComputeHash(encoding.GetBytes(Passphrase));
			TripleDESCryptoServiceProvider provider2 = new TripleDESCryptoServiceProvider {
				Key = buffer2,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			};
			byte[] bytes = encoding.GetBytes(Message);
			try
			{
				buffer = provider2.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
			}
			finally
			{
				provider2.Clear();
				provider.Clear();
			}
			return Convert.ToBase64String(buffer);
		}
	}
}

