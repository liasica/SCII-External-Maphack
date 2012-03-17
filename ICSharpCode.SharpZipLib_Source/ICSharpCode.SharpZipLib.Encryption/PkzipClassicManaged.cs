namespace ICSharpCode.SharpZipLib.Encryption
{
    using System;
    using System.Security.Cryptography;

    public sealed class PkzipClassicManaged : PkzipClassic
    {
        private byte[] key;

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new PkzipClassicDecryptCryptoTransform(rgbKey);
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new PkzipClassicEncryptCryptoTransform(rgbKey);
        }

        public override void GenerateIV()
        {
        }

        public override void GenerateKey()
        {
            this.key = new byte[12];
            new Random().NextBytes(this.key);
        }

        public override int BlockSize
        {
            get
            {
                return 8;
            }
            set
            {
                if (value != 8)
                {
                    throw new CryptographicException();
                }
            }
        }

        public override byte[] Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }

        public override KeySizes[] LegalBlockSizes
        {
            get
            {
                return new KeySizes[] { new KeySizes(8, 8, 0) };
            }
        }

        public override KeySizes[] LegalKeySizes
        {
            get
            {
                return new KeySizes[] { new KeySizes(0x60, 0x60, 0) };
            }
        }
    }
}

