namespace Data
{
	using System;

	public enum TargetFilter : ulong
	{
		Radar =					0x0000080000000000,
		Detector =				0x0000040000000000,
		Passive =				0x0000020000000000,
		Benign =				0x0000010000000000,
		HasShields =			0x0000008000000000,
		HasEnergy =				0x0000004000000000,
		Invulnerable =			0x0000002000000000,
		Hallucination =			0x0000001000000000,
		Hidden =				0x0000000800000000,
		Revivable =				0x0000000400000000,
		Dead =					0x0000000200000000,
		UnderConstruction =		0x0000000100000000,
		Stasis =				0x0000000080000000,
		Unknown40000000 =		0x0000000040000000,
		Cloaked =				0x0000000020000000,
		Buried =				0x0000000010000000,
		PreventReveal =			0x0000000008000000,
		PreventDefeat =			0x0000000004000000,
		CanHaveShields =		0x0000000002000000,
		CanHaveEnergy =			0x0000000001000000,
		Uncommandable =			0x0000000000800000,
		Item =					0x0000000000400000,
		Destructable =			0x0000000000200000,
		Missile =				0x0000000000100000,
		ResourcesHarvestable =	0x0000000000080000,
		ResourcesRaw =			0x0000000000040000,
		Worker =				0x0000000000020000,
		Heroic =				0x0000000000010000,
		Hover =					0x0000000000008000,
		Structure =				0x0000000000004000,
		Massive =				0x0000000000002000,
		Psionic =				0x0000000000001000,
		Mechanical =			0x0000000000000800,
		Robotic =				0x0000000000000400,
		Biological =			0x0000000000000200,
		Armored =				0x0000000000000100,
		Light =					0x0000000000000080,
		Ground =				0x0000000000000040,
		Air =					0x0000000000000020,
		Unknown10 =				0x0000000000000010,
		Unknown08 =				0x0000000000000008,
		Unknown04 =				0x0000000000000004,
		Unknown02 =				0x0000000000000002,
		Unknown01 =				0x0000000000000001
	}

	public static class TFToString
	{
		public static string AsList(TargetFilter tf)
		{
			string ReturnVal = string.Empty;
			ulong flag = 0;
			for(int i = 0; i < 64; i++)
			{
				flag = 1ul << i;
				if (((ulong)tf & flag) != 0)
				{
					if (ReturnVal.Length > 0)
						ReturnVal += ", ";
					ReturnVal += Enum.GetName(typeof(TargetFilter), (TargetFilter)flag);
				}
			}
			return ReturnVal;
		}
	}
}

