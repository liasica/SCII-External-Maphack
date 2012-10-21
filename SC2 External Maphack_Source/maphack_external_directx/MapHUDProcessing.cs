using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;

namespace maphack_external_directx
{
	public enum TypeIndex : int
	{
		Normal,
		Detector,
		Cloaked,
		CloakedDetector
	}

	public class UnitInfo
	{
		public double[] PlayerWeights = new double[16];
		public double[] TypeWeights = new double[4];
		public int Owner = -1;
		public fixed32 X = -1;
		public fixed32 Y = -1;
		public fixed32 Z = -1;
		public fixed32 Radius = -1;
		public TargetFilter TargetFlags = 0;
		public TypeIndex Type = TypeIndex.Normal;
		public bool Cloaked
		{ get { return (TargetFlags & TargetFilter.Cloaked) != 0; } }
		public bool Detector
		{ get { return (TargetFlags & TargetFilter.Detector) != 0; } }

		public float FinalDepth = float.NaN;
		public bool Show = true;
	}

	public static class MapHUDProcessing
	{
		static List<UnitInfo> AllUnits;
		public static List<UnitInfo> Process(List<Unit> Units)
		{
			AllUnits = new List<UnitInfo>();

			foreach (Unit unit in Units)
			{
				UnitInfo Info = new UnitInfo();
				Info.X = unit.locationX;
				Info.Y = unit.locationY;
				Info.Z = unit.locationZ;
				Info.TargetFlags = unit.targetFilterFlags;
				if ((Info.TargetFlags & TargetFilter.Dead) != 0 || (Info.TargetFlags & TargetFilter.Missile) != 0)
					continue;

				Info.Radius = unit.minimapRadius;

				if (Info.Radius < 0 || Info.Radius > 100)
					continue;

				Info.Owner = (int)unit.playerNumber;
				if(Info.Owner >= 16)
					Info.Owner = 0;

				if (Info.Cloaked && Info.Detector)
					Info.Type = TypeIndex.CloakedDetector;
				else if (Info.Cloaked)
					Info.Type = TypeIndex.Cloaked;
				else if (Info.Detector)
					Info.Type = TypeIndex.Detector;

				AllUnits.Add(Info);
			}

			for (int i = 0; i < AllUnits.Count; i++)
			{
				UnitInfo A = AllUnits[i];
				double AArea = A.Radius * A.Radius;

				for (int j = i + 1; j < AllUnits.Count; j++)
				{
					UnitInfo B = AllUnits[j];
					double BArea = B.Radius * B.Radius;

					double Distance = (B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y);
					Distance *= Distance;
					if (Distance <= 0.005) //we need a minimum distance so really close units don't mess it all up.
						continue;

					A.PlayerWeights[B.Owner] += BArea / Distance;
					B.PlayerWeights[A.Owner] += AArea / Distance;

					A.TypeWeights[(int)B.Type] += BArea / Distance;
					B.TypeWeights[(int)A.Type] += AArea / Distance;
				}

				List<KeyValuePair<int, double>> SortedWeights = new List<KeyValuePair<int, double>>();
				for (int p = 0; p < A.PlayerWeights.Length; p++)
					if (A.PlayerWeights[p] != 0)
						SortedWeights.Add(new KeyValuePair<int, double>(p, A.PlayerWeights[p]));
				SortedWeights.Sort((x, y) => x.Value.CompareTo(y.Value));

				float BaseDepth = 0;
				for (int p = 0; p < SortedWeights.Count; p++)
				{
					if (SortedWeights[p].Key == A.Owner)
					{
						BaseDepth = p;
						break;
					}
				}

				SortedWeights = new List<KeyValuePair<int, double>>();
				for (int p = 0; p < A.TypeWeights.Length; p++)
					if (A.TypeWeights[p] != 0)
						SortedWeights.Add(new KeyValuePair<int, double>(p, A.TypeWeights[p]  + (A.TypeWeights[p] * 0.2 * p))); //giving higher priority types a slightly higher weight.
				SortedWeights.Sort((x, y) => x.Value.CompareTo(y.Value));

				float TypeDepth = 0;
				for (int p = 0; p < SortedWeights.Count; p++)
				{
					if (SortedWeights[p].Key == (int)A.Type)
					{
						TypeDepth = p * 16;
						break;
					}
				}

				A.FinalDepth = (BaseDepth + TypeDepth);
			}

			return AllUnits;
		}
	}
}
