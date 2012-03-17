namespace Utilities.Paloma
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	public class TargaHeader
	{
		private byte bAttributeBits;
		private byte bColorMapEntrySize;
		private byte bImageDescriptor;
		private byte bImageIDLength;
		private byte bPixelDepth;
		private List<Color> cColorMap = new List<Color>();
		private ColorMapType eColorMapType;
		private HorizontalTransferOrder eHorizontalTransferOrder = HorizontalTransferOrder.UNKNOWN;
		private ImageType eImageType;
		private VerticalTransferOrder eVerticalTransferOrder = VerticalTransferOrder.UNKNOWN;
		private short sColorMapFirstEntryIndex;
		private short sColorMapLength;
		private short sHeight;
		private string strImageIDValue = string.Empty;
		private short sWidth;
		private short sXOrigin;
		private short sYOrigin;

		protected internal void SetAttributeBits(byte bAttributeBits)
		{
			this.bAttributeBits = bAttributeBits;
		}

		protected internal void SetColorMapEntrySize(byte bColorMapEntrySize)
		{
			this.bColorMapEntrySize = bColorMapEntrySize;
		}

		protected internal void SetColorMapFirstEntryIndex(short sColorMapFirstEntryIndex)
		{
			this.sColorMapFirstEntryIndex = sColorMapFirstEntryIndex;
		}

		protected internal void SetColorMapLength(short sColorMapLength)
		{
			this.sColorMapLength = sColorMapLength;
		}

		protected internal void SetColorMapType(ColorMapType eColorMapType)
		{
			this.eColorMapType = eColorMapType;
		}

		protected internal void SetHeight(short sHeight)
		{
			this.sHeight = sHeight;
		}

		protected internal void SetHorizontalTransferOrder(HorizontalTransferOrder eHorizontalTransferOrder)
		{
			this.eHorizontalTransferOrder = eHorizontalTransferOrder;
		}

		protected internal void SetImageIDLength(byte bImageIDLength)
		{
			this.bImageIDLength = bImageIDLength;
		}

		protected internal void SetImageIDValue(string strImageIDValue)
		{
			this.strImageIDValue = strImageIDValue;
		}

		protected internal void SetImageType(ImageType eImageType)
		{
			this.eImageType = eImageType;
		}

		protected internal void SetPixelDepth(byte bPixelDepth)
		{
			this.bPixelDepth = bPixelDepth;
		}

		protected internal void SetVerticalTransferOrder(VerticalTransferOrder eVerticalTransferOrder)
		{
			this.eVerticalTransferOrder = eVerticalTransferOrder;
		}

		protected internal void SetWidth(short sWidth)
		{
			this.sWidth = sWidth;
		}

		protected internal void SetXOrigin(short sXOrigin)
		{
			this.sXOrigin = sXOrigin;
		}

		protected internal void SetYOrigin(short sYOrigin)
		{
			this.sYOrigin = sYOrigin;
		}

		public byte AttributeBits
		{
			get
			{
				return this.bAttributeBits;
			}
		}

		public int BytesPerPixel
		{
			get
			{
				return (this.bPixelDepth / 8);
			}
		}

		public List<Color> ColorMap
		{
			get
			{
				return this.cColorMap;
			}
		}

		public byte ColorMapEntrySize
		{
			get
			{
				return this.bColorMapEntrySize;
			}
		}

		public short ColorMapFirstEntryIndex
		{
			get
			{
				return this.sColorMapFirstEntryIndex;
			}
		}

		public short ColorMapLength
		{
			get
			{
				return this.sColorMapLength;
			}
		}

		public ColorMapType ColorMapType
		{
			get
			{
				return this.eColorMapType;
			}
		}

		public FirstPixelDestination FirstPixelDestination
		{
			get
			{
				if ((this.eVerticalTransferOrder == VerticalTransferOrder.UNKNOWN) || (this.eHorizontalTransferOrder == HorizontalTransferOrder.UNKNOWN))
				{
					return FirstPixelDestination.UNKNOWN;
				}
				if ((this.eVerticalTransferOrder == VerticalTransferOrder.BOTTOM) && (this.eHorizontalTransferOrder == HorizontalTransferOrder.LEFT))
				{
					return FirstPixelDestination.BOTTOM_LEFT;
				}
				if ((this.eVerticalTransferOrder == VerticalTransferOrder.BOTTOM) && (this.eHorizontalTransferOrder == HorizontalTransferOrder.RIGHT))
				{
					return FirstPixelDestination.BOTTOM_RIGHT;
				}
				if ((this.eVerticalTransferOrder == VerticalTransferOrder.TOP) && (this.eHorizontalTransferOrder == HorizontalTransferOrder.LEFT))
				{
					return FirstPixelDestination.TOP_LEFT;
				}
				return FirstPixelDestination.TOP_RIGHT;
			}
		}

		public short Height
		{
			get
			{
				return this.sHeight;
			}
		}

		public HorizontalTransferOrder HorizontalTransferOrder
		{
			get
			{
				return this.eHorizontalTransferOrder;
			}
		}

		public int ImageDataOffset
		{
			get
			{
				int num = 0x12;
				num += this.bImageIDLength;
				int num2 = 0;
				switch (this.bColorMapEntrySize)
				{
					case 15:
						num2 = 2;
						break;

					case 0x10:
						num2 = 2;
						break;

					case 0x18:
						num2 = 3;
						break;

					case 0x20:
						num2 = 4;
						break;
				}
				return (num + (this.sColorMapLength * num2));
			}
		}

		protected internal byte ImageDescriptor
		{
			get
			{
				return this.bImageDescriptor;
			}
			set
			{
				this.bImageDescriptor = value;
			}
		}

		public byte ImageIDLength
		{
			get
			{
				return this.bImageIDLength;
			}
		}

		public string ImageIDValue
		{
			get
			{
				return this.strImageIDValue;
			}
		}

		public ImageType ImageType
		{
			get
			{
				return this.eImageType;
			}
		}

		public byte PixelDepth
		{
			get
			{
				return this.bPixelDepth;
			}
		}

		public VerticalTransferOrder VerticalTransferOrder
		{
			get
			{
				return this.eVerticalTransferOrder;
			}
		}

		public short Width
		{
			get
			{
				return this.sWidth;
			}
		}

		public short XOrigin
		{
			get
			{
				return this.sXOrigin;
			}
		}

		public short YOrigin
		{
			get
			{
				return this.sYOrigin;
			}
		}
	}
}

