namespace Utilities.Paloma
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	public class TargaExtensionArea
	{
		private List<Color> cColorCorrectionTable = new List<Color>();
		private Color cKeyColor = Color.Empty;
		private DateTime dtDateTimeStamp = DateTime.Now;
		private TimeSpan dtJobTime = TimeSpan.Zero;
		private int intAttributesType;
		private int intColorCorrectionOffset;
		private int intExtensionSize;
		private int intGammaDenominator;
		private int intGammaNumerator;
		private int intPixelAspectRatioDenominator;
		private int intPixelAspectRatioNumerator;
		private int intPostageStampOffset;
		private int intScanLineOffset;
		private List<int> intScanLineTable = new List<int>();
		private string strAuthorComments = string.Empty;
		private string strAuthorName = string.Empty;
		private string strJobName = string.Empty;
		private string strSoftwareID = string.Empty;
		private string strSoftwareVersion = string.Empty;

		protected internal void SetAttributesType(int intAttributesType)
		{
			this.intAttributesType = intAttributesType;
		}

		protected internal void SetAuthorComments(string strAuthorComments)
		{
			this.strAuthorComments = strAuthorComments;
		}

		protected internal void SetAuthorName(string strAuthorName)
		{
			this.strAuthorName = strAuthorName;
		}

		protected internal void SetColorCorrectionOffset(int intColorCorrectionOffset)
		{
			this.intColorCorrectionOffset = intColorCorrectionOffset;
		}

		protected internal void SetDateTimeStamp(DateTime dtDateTimeStamp)
		{
			this.dtDateTimeStamp = dtDateTimeStamp;
		}

		protected internal void SetExtensionSize(int intExtensionSize)
		{
			this.intExtensionSize = intExtensionSize;
		}

		protected internal void SetGammaDenominator(int intGammaDenominator)
		{
			this.intGammaDenominator = intGammaDenominator;
		}

		protected internal void SetGammaNumerator(int intGammaNumerator)
		{
			this.intGammaNumerator = intGammaNumerator;
		}

		protected internal void SetJobName(string strJobName)
		{
			this.strJobName = strJobName;
		}

		protected internal void SetJobTime(TimeSpan dtJobTime)
		{
			this.dtJobTime = dtJobTime;
		}

		protected internal void SetKeyColor(Color cKeyColor)
		{
			this.cKeyColor = cKeyColor;
		}

		protected internal void SetPixelAspectRatioDenominator(int intPixelAspectRatioDenominator)
		{
			this.intPixelAspectRatioDenominator = intPixelAspectRatioDenominator;
		}

		protected internal void SetPixelAspectRatioNumerator(int intPixelAspectRatioNumerator)
		{
			this.intPixelAspectRatioNumerator = intPixelAspectRatioNumerator;
		}

		protected internal void SetPostageStampOffset(int intPostageStampOffset)
		{
			this.intPostageStampOffset = intPostageStampOffset;
		}

		protected internal void SetScanLineOffset(int intScanLineOffset)
		{
			this.intScanLineOffset = intScanLineOffset;
		}

		protected internal void SetSoftwareID(string strSoftwareID)
		{
			this.strSoftwareID = strSoftwareID;
		}

		protected internal void SetSoftwareVersion(string strSoftwareVersion)
		{
			this.strSoftwareVersion = strSoftwareVersion;
		}

		public int AttributesType
		{
			get
			{
				return this.intAttributesType;
			}
		}

		public string AuthorComments
		{
			get
			{
				return this.strAuthorComments;
			}
		}

		public string AuthorName
		{
			get
			{
				return this.strAuthorName;
			}
		}

		public int ColorCorrectionOffset
		{
			get
			{
				return this.intColorCorrectionOffset;
			}
		}

		public List<Color> ColorCorrectionTable
		{
			get
			{
				return this.cColorCorrectionTable;
			}
		}

		public DateTime DateTimeStamp
		{
			get
			{
				return this.dtDateTimeStamp;
			}
		}

		public int ExtensionSize
		{
			get
			{
				return this.intExtensionSize;
			}
		}

		public int GammaDenominator
		{
			get
			{
				return this.intGammaDenominator;
			}
		}

		public int GammaNumerator
		{
			get
			{
				return this.intGammaNumerator;
			}
		}

		public float GammaRatio
		{
			get
			{
				if (this.intGammaDenominator > 0)
				{
					float num = ((float) this.intGammaNumerator) / ((float) this.intGammaDenominator);
					return (float) Math.Round((double) num, 1);
				}
				return 1f;
			}
		}

		public string JobName
		{
			get
			{
				return this.strJobName;
			}
		}

		public TimeSpan JobTime
		{
			get
			{
				return this.dtJobTime;
			}
		}

		public Color KeyColor
		{
			get
			{
				return this.cKeyColor;
			}
		}

		public float PixelAspectRatio
		{
			get
			{
				if (this.intPixelAspectRatioDenominator > 0)
				{
					return (((float) this.intPixelAspectRatioNumerator) / ((float) this.intPixelAspectRatioDenominator));
				}
				return 0f;
			}
		}

		public int PixelAspectRatioDenominator
		{
			get
			{
				return this.intPixelAspectRatioDenominator;
			}
		}

		public int PixelAspectRatioNumerator
		{
			get
			{
				return this.intPixelAspectRatioNumerator;
			}
		}

		public int PostageStampOffset
		{
			get
			{
				return this.intPostageStampOffset;
			}
		}

		public int ScanLineOffset
		{
			get
			{
				return this.intScanLineOffset;
			}
		}

		public List<int> ScanLineTable
		{
			get
			{
				return this.intScanLineTable;
			}
		}

		public string SoftwareID
		{
			get
			{
				return this.strSoftwareID;
			}
		}

		public string SoftwareVersion
		{
			get
			{
				return this.strSoftwareVersion;
			}
		}
	}
}

