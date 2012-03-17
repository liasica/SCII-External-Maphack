namespace Utilities.Paloma
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Text;

	public class TargaImage : IDisposable
	{
		private Bitmap bmpImageThumbnail;
		private Bitmap bmpTargaImage;
		private bool disposed;
		private TGAFormat eTGAFormat;
		private GCHandle ImageByteHandle;
		private int intPadding;
		private int intStride;
		private TargaExtensionArea objTargaExtensionArea;
		private TargaFooter objTargaFooter;
		private TargaHeader objTargaHeader;
		private List<byte> row;
		private List<List<byte>> rows;
		private string strFileName;
		private GCHandle ThumbnailByteHandle;

		public TargaImage()
		{
			this.strFileName = string.Empty;
			this.rows = new List<List<byte>>();
			this.row = new List<byte>();
			this.objTargaFooter = new TargaFooter();
			this.objTargaHeader = new TargaHeader();
			this.objTargaExtensionArea = new TargaExtensionArea();
			this.bmpTargaImage = null;
			this.bmpImageThumbnail = null;
		}

		public TargaImage(string strFileName) : this()
		{
			if (!(Path.GetExtension(strFileName).ToLower() == ".tga"))
			{
				throw new Exception("Error loading file, file '" + strFileName + "' must have an extension of '.tga'.");
			}
			if (!File.Exists(strFileName))
			{
				throw new Exception("Error loading file, could not find file '" + strFileName + "' on disk.");
			}
			this.strFileName = strFileName;
			MemoryStream input = null;
			BinaryReader binReader = null;
			byte[] buffer = null;
			buffer = File.ReadAllBytes(this.strFileName);
			if ((buffer != null) && (buffer.Length > 0))
			{
				using (input = new MemoryStream(buffer))
				{
					if (((input != null) && (input.Length > 0L)) && input.CanSeek)
					{
						using (binReader = new BinaryReader(input))
						{
							this.LoadTGAFooterInfo(binReader);
							this.LoadTGAHeaderInfo(binReader);
							this.LoadTGAExtensionArea(binReader);
							this.LoadTGAImage(binReader);
							return;
						}
					}
					throw new Exception("Error loading file, could not read file from disk.");
				}
			}
			throw new Exception("Error loading file, could not read file from disk.");
		}

		private void ClearAll()
		{
			if (this.bmpTargaImage != null)
			{
				this.bmpTargaImage.Dispose();
				this.bmpTargaImage = null;
			}
			if (this.ImageByteHandle.IsAllocated)
			{
				this.ImageByteHandle.Free();
			}
			if (this.ThumbnailByteHandle.IsAllocated)
			{
				this.ThumbnailByteHandle.Free();
			}
			this.objTargaHeader = new TargaHeader();
			this.objTargaExtensionArea = new TargaExtensionArea();
			this.objTargaFooter = new TargaFooter();
			this.eTGAFormat = TGAFormat.UNKNOWN;
			this.intStride = 0;
			this.intPadding = 0;
			this.rows.Clear();
			this.row.Clear();
			this.strFileName = string.Empty;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				if (this.bmpTargaImage != null)
				{
					this.bmpTargaImage.Dispose();
				}
				if (this.bmpImageThumbnail != null)
				{
					this.bmpImageThumbnail.Dispose();
				}
				if (this.ImageByteHandle.IsAllocated)
				{
					this.ImageByteHandle.Free();
				}
				if (this.ThumbnailByteHandle.IsAllocated)
				{
					this.ThumbnailByteHandle.Free();
				}
			}
			this.disposed = true;
		}

		~TargaImage()
		{
			this.Dispose(false);
		}

		private PixelFormat GetPixelFormat()
		{
			PixelFormat undefined = PixelFormat.Undefined;
			byte pixelDepth = this.objTargaHeader.PixelDepth;
			if (pixelDepth <= 0x10)
			{
				switch (pixelDepth)
				{
					case 8:
						return PixelFormat.Format8bppIndexed;

					case 0x10:
						if (this.Format == TGAFormat.NEW_TGA)
						{
							switch (this.objTargaExtensionArea.AttributesType)
							{
								case 0:
								case 1:
								case 2:
									return PixelFormat.Format16bppRgb555;

								case 3:
									return PixelFormat.Format16bppArgb1555;
							}
							return undefined;
						}
						return PixelFormat.Format16bppRgb555;
				}
				return undefined;
			}
			switch (pixelDepth)
			{
				case 0x18:
					return PixelFormat.Format24bppRgb;

				case 0x20:
					if (this.Format == TGAFormat.NEW_TGA)
					{
						switch (this.objTargaExtensionArea.AttributesType)
						{
							case 0:
							case 3:
								return PixelFormat.Format32bppArgb;

							case 1:
							case 2:
								return PixelFormat.Format32bppRgb;

							case 4:
								return PixelFormat.Format32bppPArgb;
						}
						return undefined;
					}
					return PixelFormat.Format32bppRgb;
			}
			return undefined;
		}

		private byte[] LoadImageBytes(BinaryReader binReader)
		{
			if (((binReader == null) || (binReader.BaseStream == null)) || ((binReader.BaseStream.Length <= 0L) || !binReader.BaseStream.CanSeek))
			{
				this.ClearAll();
				throw new Exception("Error loading file, could not read file from disk.");
			}
			if (this.objTargaHeader.ImageDataOffset > 0)
			{
				byte[] buffer = new byte[this.intPadding];
				MemoryStream stream = null;
				binReader.BaseStream.Seek((long) this.objTargaHeader.ImageDataOffset, SeekOrigin.Begin);
				int num = this.objTargaHeader.Width * this.objTargaHeader.BytesPerPixel;
				int num2 = num * this.objTargaHeader.Height;
				if (((this.objTargaHeader.ImageType != ImageType.RUN_LENGTH_ENCODED_BLACK_AND_WHITE) && (this.objTargaHeader.ImageType != ImageType.RUN_LENGTH_ENCODED_COLOR_MAPPED)) && (this.objTargaHeader.ImageType != ImageType.RUN_LENGTH_ENCODED_TRUE_COLOR))
				{
					for (int i = 0; i < this.objTargaHeader.Height; i++)
					{
						for (int j = 0; j < num; j++)
						{
							this.row.Add(binReader.ReadByte());
						}
						this.rows.Add(this.row);
						this.row = new List<byte>();
					}
				}
				else
				{
					byte b = 0;
					int num4 = -1;
					int num5 = 0;
					byte[] buffer3 = null;
					int num6 = 0;
					int num7 = 0;
					while (num6 < num2)
					{
						b = binReader.ReadByte();
						num4 = Utilities.GetBits(b, 7, 1);
						num5 = Utilities.GetBits(b, 0, 7) + 1;
						switch (num4)
						{
							case 1:
							{
								buffer3 = binReader.ReadBytes(this.objTargaHeader.BytesPerPixel);
								for (int k = 0; k < num5; k++)
								{
									foreach (byte num9 in buffer3)
									{
										this.row.Add(num9);
									}
									num7 += buffer3.Length;
									num6 += buffer3.Length;
									if (num7 == num)
									{
										this.rows.Add(this.row);
										this.row = new List<byte>();
										num7 = 0;
									}
								}
								continue;
							}
							case 0:
							{
								int num10 = num5 * this.objTargaHeader.BytesPerPixel;
								for (int m = 0; m < num10; m++)
								{
									this.row.Add(binReader.ReadByte());
									num6++;
									num7++;
									if (num7 == num)
									{
										this.rows.Add(this.row);
										this.row = new List<byte>();
										num7 = 0;
									}
								}
								break;
							}
						}
					}
				}
				bool flag = false;
				bool flag2 = false;
				switch (this.objTargaHeader.FirstPixelDestination)
				{
					case FirstPixelDestination.UNKNOWN:
					case FirstPixelDestination.BOTTOM_RIGHT:
						flag = true;
						flag2 = false;
						break;

					case FirstPixelDestination.TOP_LEFT:
						flag = false;
						flag2 = true;
						break;

					case FirstPixelDestination.TOP_RIGHT:
						flag = false;
						flag2 = false;
						break;

					case FirstPixelDestination.BOTTOM_LEFT:
						flag = true;
						flag2 = true;
						break;
				}
				using (stream = new MemoryStream())
				{
					if (flag)
					{
						this.rows.Reverse();
					}
					for (int n = 0; n < this.rows.Count; n++)
					{
						if (flag2)
						{
							this.rows[n].Reverse();
						}
						byte[] buffer4 = this.rows[n].ToArray();
						stream.Write(buffer4, 0, buffer4.Length);
						stream.Write(buffer, 0, buffer.Length);
					}
					return stream.ToArray();
				}
			}
			this.ClearAll();
			throw new Exception("Error loading file, No image data in file.");
		}

		public static Bitmap LoadTargaImage(string sFileName)
		{
			using (TargaImage image = new TargaImage(sFileName))
			{
				return new Bitmap(image.Image);
			}
		}

		private void LoadTGAExtensionArea(BinaryReader binReader)
		{
			if (((binReader != null) && (binReader.BaseStream != null)) && ((binReader.BaseStream.Length > 0L) && binReader.BaseStream.CanSeek))
			{
				if (this.objTargaFooter.ExtensionAreaOffset <= 0)
				{
					return;
				}
				try
				{
					DateTime time;
					binReader.BaseStream.Seek((long) this.objTargaFooter.ExtensionAreaOffset, SeekOrigin.Begin);
					this.objTargaExtensionArea.SetExtensionSize(binReader.ReadInt16());
					this.objTargaExtensionArea.SetAuthorName(Encoding.ASCII.GetString(binReader.ReadBytes(0x29)).TrimEnd(new char[1]));
					this.objTargaExtensionArea.SetAuthorComments(Encoding.ASCII.GetString(binReader.ReadBytes(0x144)).TrimEnd(new char[1]));
					short num = binReader.ReadInt16();
					short num2 = binReader.ReadInt16();
					short num3 = binReader.ReadInt16();
					short hours = binReader.ReadInt16();
					short minutes = binReader.ReadInt16();
					short seconds = binReader.ReadInt16();
					string str3 = num.ToString() + "/" + num2.ToString() + "/" + num3.ToString() + " ";
					if (DateTime.TryParse(str3 + hours.ToString() + ":" + minutes.ToString() + ":" + seconds.ToString(), out time))
					{
						this.objTargaExtensionArea.SetDateTimeStamp(time);
					}
					this.objTargaExtensionArea.SetJobName(Encoding.ASCII.GetString(binReader.ReadBytes(0x29)).TrimEnd(new char[1]));
					hours = binReader.ReadInt16();
					minutes = binReader.ReadInt16();
					seconds = binReader.ReadInt16();
					TimeSpan dtJobTime = new TimeSpan(hours, minutes, seconds);
					this.objTargaExtensionArea.SetJobTime(dtJobTime);
					this.objTargaExtensionArea.SetSoftwareID(Encoding.ASCII.GetString(binReader.ReadBytes(0x29)).TrimEnd(new char[1]));
					this.objTargaExtensionArea.SetSoftwareID(((((float) binReader.ReadInt16()) / 100f)).ToString("F2") + Encoding.ASCII.GetString(binReader.ReadBytes(1)).TrimEnd(new char[1]));
					int alpha = binReader.ReadByte();
					int red = binReader.ReadByte();
					int blue = binReader.ReadByte();
					int green = binReader.ReadByte();
					this.objTargaExtensionArea.SetKeyColor(Color.FromArgb(alpha, red, green, blue));
					this.objTargaExtensionArea.SetPixelAspectRatioNumerator(binReader.ReadInt16());
					this.objTargaExtensionArea.SetPixelAspectRatioDenominator(binReader.ReadInt16());
					this.objTargaExtensionArea.SetGammaNumerator(binReader.ReadInt16());
					this.objTargaExtensionArea.SetGammaDenominator(binReader.ReadInt16());
					this.objTargaExtensionArea.SetColorCorrectionOffset(binReader.ReadInt32());
					this.objTargaExtensionArea.SetPostageStampOffset(binReader.ReadInt32());
					this.objTargaExtensionArea.SetScanLineOffset(binReader.ReadInt32());
					this.objTargaExtensionArea.SetAttributesType(binReader.ReadByte());
					if (this.objTargaExtensionArea.ScanLineOffset > 0)
					{
						binReader.BaseStream.Seek((long) this.objTargaExtensionArea.ScanLineOffset, SeekOrigin.Begin);
						for (int i = 0; i < this.objTargaHeader.Height; i++)
						{
							this.objTargaExtensionArea.ScanLineTable.Add(binReader.ReadInt32());
						}
					}
					if (this.objTargaExtensionArea.ColorCorrectionOffset > 0)
					{
						binReader.BaseStream.Seek((long) this.objTargaExtensionArea.ColorCorrectionOffset, SeekOrigin.Begin);
						for (int j = 0; j < 0x100; j++)
						{
							alpha = binReader.ReadInt16();
							red = binReader.ReadInt16();
							blue = binReader.ReadInt16();
							green = binReader.ReadInt16();
							this.objTargaExtensionArea.ColorCorrectionTable.Add(Color.FromArgb(alpha, red, green, blue));
						}
					}
					return;
				}
				catch (Exception exception)
				{
					this.ClearAll();
					throw exception;
				}
			}
			this.ClearAll();
			throw new Exception("Error loading file, could not read file from disk.");
		}

		private void LoadTGAFooterInfo(BinaryReader binReader)
		{
			if (((binReader != null) && (binReader.BaseStream != null)) && ((binReader.BaseStream.Length > 0L) && binReader.BaseStream.CanSeek))
			{
				try
				{
					binReader.BaseStream.Seek(-18L, SeekOrigin.End);
					string strA = Encoding.ASCII.GetString(binReader.ReadBytes(0x10)).TrimEnd(new char[1]);
					if (string.Compare(strA, "TRUEVISION-XFILE") == 0)
					{
						this.eTGAFormat = TGAFormat.NEW_TGA;
						binReader.BaseStream.Seek(-26L, SeekOrigin.End);
						int intExtensionAreaOffset = binReader.ReadInt32();
						int intDeveloperDirectoryOffset = binReader.ReadInt32();
						binReader.ReadBytes(0x10);
						string strReservedCharacter = Encoding.ASCII.GetString(binReader.ReadBytes(1)).TrimEnd(new char[1]);
						this.objTargaFooter.SetExtensionAreaOffset(intExtensionAreaOffset);
						this.objTargaFooter.SetDeveloperDirectoryOffset(intDeveloperDirectoryOffset);
						this.objTargaFooter.SetSignature(strA);
						this.objTargaFooter.SetReservedCharacter(strReservedCharacter);
						return;
					}
					this.eTGAFormat = TGAFormat.ORIGINAL_TGA;
				}
				catch (Exception exception)
				{
					this.ClearAll();
					throw exception;
				}
			}
			this.ClearAll();
			throw new Exception("Error loading file, could not read file from disk.");
		}

		private void LoadTGAHeaderInfo(BinaryReader binReader)
		{
			if (((binReader != null) && (binReader.BaseStream != null)) && ((binReader.BaseStream.Length > 0L) && binReader.BaseStream.CanSeek))
			{
				try
				{
					binReader.BaseStream.Seek(0L, SeekOrigin.Begin);
					this.objTargaHeader.SetImageIDLength(binReader.ReadByte());
					this.objTargaHeader.SetColorMapType((ColorMapType) binReader.ReadByte());
					this.objTargaHeader.SetImageType((ImageType) binReader.ReadByte());
					this.objTargaHeader.SetColorMapFirstEntryIndex(binReader.ReadInt16());
					this.objTargaHeader.SetColorMapLength(binReader.ReadInt16());
					this.objTargaHeader.SetColorMapEntrySize(binReader.ReadByte());
					this.objTargaHeader.SetXOrigin(binReader.ReadInt16());
					this.objTargaHeader.SetYOrigin(binReader.ReadInt16());
					this.objTargaHeader.SetWidth(binReader.ReadInt16());
					this.objTargaHeader.SetHeight(binReader.ReadInt16());
					byte bPixelDepth = binReader.ReadByte();
					switch (bPixelDepth)
					{
						case 0x18:
						case 0x20:
						case 8:
						case 0x10:
							this.objTargaHeader.SetPixelDepth(bPixelDepth);
							break;

						default:
							this.ClearAll();
							throw new Exception("Targa Image only supports 8, 16, 24, or 32 bit pixel depths.");
					}
					byte b = binReader.ReadByte();
					this.objTargaHeader.SetAttributeBits((byte) Utilities.GetBits(b, 0, 4));
					this.objTargaHeader.SetVerticalTransferOrder((VerticalTransferOrder) Utilities.GetBits(b, 5, 1));
					this.objTargaHeader.SetHorizontalTransferOrder((HorizontalTransferOrder) Utilities.GetBits(b, 4, 1));
					if (this.objTargaHeader.ImageIDLength > 0)
					{
						byte[] bytes = binReader.ReadBytes(this.objTargaHeader.ImageIDLength);
						this.objTargaHeader.SetImageIDValue(Encoding.ASCII.GetString(bytes).TrimEnd(new char[1]));
					}
				}
				catch (Exception exception)
				{
					this.ClearAll();
					throw exception;
				}
				if (this.objTargaHeader.ColorMapType != ColorMapType.COLOR_MAP_INCLUDED)
				{
					if ((this.objTargaHeader.ImageType == ImageType.UNCOMPRESSED_COLOR_MAPPED) || (this.objTargaHeader.ImageType == ImageType.RUN_LENGTH_ENCODED_COLOR_MAPPED))
					{
						this.ClearAll();
						throw new Exception("Image Type requires a Color Map and there was not a Color Map included in the file.");
					}
				}
				else if ((this.objTargaHeader.ImageType == ImageType.UNCOMPRESSED_COLOR_MAPPED) || (this.objTargaHeader.ImageType == ImageType.RUN_LENGTH_ENCODED_COLOR_MAPPED))
				{
					if (this.objTargaHeader.ColorMapLength > 0)
					{
						try
						{
							for (int i = 0; i < this.objTargaHeader.ColorMapLength; i++)
							{
								int alpha = 0;
								int red = 0;
								int green = 0;
								int blue = 0;
								switch (this.objTargaHeader.ColorMapEntrySize)
								{
									case 15:
									{
										byte[] buffer2 = binReader.ReadBytes(2);
										this.objTargaHeader.ColorMap.Add(Utilities.GetColorFrom2Bytes(buffer2[1], buffer2[0]));
										break;
									}
									case 0x10:
									{
										byte[] buffer3 = binReader.ReadBytes(2);
										this.objTargaHeader.ColorMap.Add(Utilities.GetColorFrom2Bytes(buffer3[1], buffer3[0]));
										break;
									}
									case 0x18:
										blue = Convert.ToInt32(binReader.ReadByte());
										green = Convert.ToInt32(binReader.ReadByte());
										red = Convert.ToInt32(binReader.ReadByte());
										this.objTargaHeader.ColorMap.Add(Color.FromArgb(red, green, blue));
										break;

									case 0x20:
										alpha = Convert.ToInt32(binReader.ReadByte());
										blue = Convert.ToInt32(binReader.ReadByte());
										green = Convert.ToInt32(binReader.ReadByte());
										red = Convert.ToInt32(binReader.ReadByte());
										this.objTargaHeader.ColorMap.Add(Color.FromArgb(alpha, red, green, blue));
										break;

									default:
										this.ClearAll();
										throw new Exception("TargaImage only supports ColorMap Entry Sizes of 15, 16, 24 or 32 bits.");
								}
							}
							return;
						}
						catch (Exception exception2)
						{
							this.ClearAll();
							throw exception2;
						}
					}
					this.ClearAll();
					throw new Exception("Image Type requires a Color Map and Color Map Length is zero.");
				}
			}
			else
			{
				this.ClearAll();
				throw new Exception("Error loading file, could not read file from disk.");
			}
		}

		private void LoadTGAImage(BinaryReader binReader)
		{
			this.intStride = (((this.objTargaHeader.Width * this.objTargaHeader.PixelDepth) + 0x1f) & -32) >> 3;
			this.intPadding = this.intStride - (((this.objTargaHeader.Width * this.objTargaHeader.PixelDepth) + 7) / 8);
			byte[] buffer = this.LoadImageBytes(binReader);
			this.ImageByteHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			if (this.bmpTargaImage != null)
			{
				this.bmpTargaImage.Dispose();
			}
			if (this.bmpImageThumbnail != null)
			{
				this.bmpImageThumbnail.Dispose();
			}
			PixelFormat pixelFormat = this.GetPixelFormat();
			this.bmpTargaImage = new Bitmap(this.objTargaHeader.Width, this.objTargaHeader.Height, this.intStride, pixelFormat, this.ImageByteHandle.AddrOfPinnedObject());
			this.LoadThumbnail(binReader, pixelFormat);
			if (this.objTargaHeader.ColorMap.Count > 0)
			{
				ColorPalette palette = this.bmpTargaImage.Palette;
				for (int i = 0; i < this.objTargaHeader.ColorMap.Count; i++)
				{
					if ((this.objTargaExtensionArea.AttributesType == 0) || (this.objTargaExtensionArea.AttributesType == 1))
					{
						Color color = this.objTargaHeader.ColorMap[i];
						Color color2 = this.objTargaHeader.ColorMap[i];
						Color color3 = this.objTargaHeader.ColorMap[i];
						palette.Entries[i] = Color.FromArgb(0xff, color.R, color2.G, color3.B);
					}
					else
					{
						palette.Entries[i] = this.objTargaHeader.ColorMap[i];
					}
				}
				this.bmpTargaImage.Palette = palette;
				if (this.bmpImageThumbnail != null)
				{
					this.bmpImageThumbnail.Palette = palette;
				}
			}
			else if ((this.objTargaHeader.PixelDepth == 8) && ((this.objTargaHeader.ImageType == ImageType.UNCOMPRESSED_BLACK_AND_WHITE) || (this.objTargaHeader.ImageType == ImageType.RUN_LENGTH_ENCODED_BLACK_AND_WHITE)))
			{
				ColorPalette palette2 = this.bmpTargaImage.Palette;
				for (int j = 0; j < 0x100; j++)
				{
					palette2.Entries[j] = Color.FromArgb(j, j, j);
				}
				this.bmpTargaImage.Palette = palette2;
				if (this.bmpImageThumbnail != null)
				{
					this.bmpImageThumbnail.Palette = palette2;
				}
			}
		}

		private void LoadThumbnail(BinaryReader binReader, PixelFormat pfPixelFormat)
		{
			byte[] buffer = null;
			if (((binReader != null) && (binReader.BaseStream != null)) && ((binReader.BaseStream.Length > 0L) && binReader.BaseStream.CanSeek))
			{
				if (this.ExtensionArea.PostageStampOffset <= 0)
				{
					if (this.bmpImageThumbnail != null)
					{
						this.bmpImageThumbnail.Dispose();
						this.bmpImageThumbnail = null;
					}
				}
				else
				{
					binReader.BaseStream.Seek((long) this.ExtensionArea.PostageStampOffset, SeekOrigin.Begin);
					int width = binReader.ReadByte();
					int height = binReader.ReadByte();
					int stride = (((width * this.objTargaHeader.PixelDepth) + 0x1f) & -32) >> 3;
					int num4 = stride - (((width * this.objTargaHeader.PixelDepth) + 7) / 8);
					List<List<byte>> list = new List<List<byte>>();
					List<byte> item = new List<byte>();
					byte[] buffer2 = new byte[num4];
					MemoryStream stream = null;
					bool flag = false;
					bool flag2 = false;
					using (stream = new MemoryStream())
					{
						int num5 = width * (this.objTargaHeader.PixelDepth / 8);
						for (int i = 0; i < height; i++)
						{
							for (int k = 0; k < num5; k++)
							{
								item.Add(binReader.ReadByte());
							}
							list.Add(item);
							item = new List<byte>();
						}
						switch (this.objTargaHeader.FirstPixelDestination)
						{
							case FirstPixelDestination.UNKNOWN:
							case FirstPixelDestination.BOTTOM_RIGHT:
								flag2 = true;
								flag = false;
								break;

							case FirstPixelDestination.TOP_RIGHT:
								flag2 = false;
								flag = false;
								break;
						}
						if (flag2)
						{
							list.Reverse();
						}
						for (int j = 0; j < list.Count; j++)
						{
							if (flag)
							{
								list[j].Reverse();
							}
							byte[] buffer3 = list[j].ToArray();
							stream.Write(buffer3, 0, buffer3.Length);
							stream.Write(buffer2, 0, buffer2.Length);
						}
						buffer = stream.ToArray();
					}
					if ((buffer != null) && (buffer.Length > 0))
					{
						this.ThumbnailByteHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
						this.bmpImageThumbnail = new Bitmap(width, height, stride, pfPixelFormat, this.ThumbnailByteHandle.AddrOfPinnedObject());
					}
				}
			}
			else if (this.bmpImageThumbnail != null)
			{
				this.bmpImageThumbnail.Dispose();
				this.bmpImageThumbnail = null;
			}
		}

		public TargaExtensionArea ExtensionArea
		{
			get
			{
				return this.objTargaExtensionArea;
			}
		}

		public string FileName
		{
			get
			{
				return this.strFileName;
			}
		}

		public TargaFooter Footer
		{
			get
			{
				return this.objTargaFooter;
			}
		}

		public TGAFormat Format
		{
			get
			{
				return this.eTGAFormat;
			}
		}

		public TargaHeader Header
		{
			get
			{
				return this.objTargaHeader;
			}
		}

		public Bitmap Image
		{
			get
			{
				return this.bmpTargaImage;
			}
		}

		public int Padding
		{
			get
			{
				return this.intPadding;
			}
		}

		public int Stride
		{
			get
			{
				return this.intStride;
			}
		}

		public Bitmap Thumbnail
		{
			get
			{
				return this.bmpImageThumbnail;
			}
		}
	}
}

