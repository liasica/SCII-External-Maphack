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

		public List<UnitInfo> UnitsWithinRadius = new List<UnitInfo>();
		public List<UnitInfo> UnitsWithinTheirRadius = new List<UnitInfo>();
		public CellInfo ContainingCell = new CellInfo();

		public float FinalDepth = float.NaN;
		public bool Show = true;
	}

	public class CellInfo
	{
		public CellInfo()
		{ }
		public CellInfo(int x, int y)
		{
			X = x;
			Y = y;
		}
		public int X = -1;
		public int Y = -1;
		public int[] Players = new int[16];
		public int Detectors = -1;
		public int Cloaked = -1;
		public int CloakedDetectors = -1;
		public List<UnitInfo> Units = new List<UnitInfo>();
		public List<UnitInfo> UnitsWithinTheirRadius = new List<UnitInfo>();
	}

	public static class MapHUDProcessing
	{
		static CellInfo[,] Grid;
		static List<UnitInfo> AllUnits;
		static int MapLeft = -1;
		static int MapRight = -1;
		static int MapTop = -1;
		static int MapBottom = -1;
		static int MapWidth { get { return MapRight - MapLeft; } }
		static int MapHeight { get { return MapTop - MapBottom; } }

		static int Width = -1;
		static int Height = -1;
		static float Scale = -1;

		public static List<UnitInfo> Process(List<Unit> Units)
		{
			MapLeft = (int)Math.Round(MainWindow.playable_map_left);
			MapRight = (int)Math.Round(MainWindow.playable_map_right);
			MapTop = (int)Math.Round(MainWindow.playable_map_top);
			MapBottom = (int)Math.Round(MainWindow.playable_map_bottom);
			Scale = MainWindow.minimap_scale;
			Width = (int)Math.Round(MapWidth * Scale);
			Height = (int)Math.Round(MapHeight * Scale);

			Grid = new CellInfo[Width, Height];
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					Grid[x, y] = new CellInfo(x, y);

			AllUnits = new List<UnitInfo>();

			foreach (Unit unit in Units)
			{
				UnitInfo Info = new UnitInfo();
				Info.X = unit.locationX;
				Info.Y = unit.locationY;
				Info.Z = unit.locationZ;
				int CellX = (int)((Info.X - MapLeft) * Scale);
				int CellY = (int)((Info.Y - MapBottom) * Scale);
				if (CellX < 0 || CellX >= Width || CellY < 0 || CellY >= Height)
					continue;

				Info.ContainingCell = Grid[CellX, CellY];
				Info.TargetFlags = unit.targetFilterFlags;
				if ((Info.TargetFlags & TargetFilter.Dead) != 0 || (Info.TargetFlags & TargetFilter.Missile) != 0)
					continue;

				Info.Radius = unit.minimapRadius;
				if (Info.Radius < 0 || Info.Radius > 100)
					continue;

				Info.Owner = (int)unit.playerNumber;
				if(Info.Owner >= 16)
					Info.Owner = 0;
				Info.ContainingCell.Units.Add(Info);
				if ((Info.TargetFlags & TargetFilter.Cloaked) != 0 && (Info.TargetFlags & TargetFilter.Detector) != 0)
				{
					Info.Type = TypeIndex.CloakedDetector;
					Info.ContainingCell.CloakedDetectors++;
				}
				else if (Info.Cloaked)
				{
					Info.Type = TypeIndex.Cloaked;
					Info.ContainingCell.Cloaked++;
				}
				else if (Info.Detector)
				{
					Info.Type = TypeIndex.Detector;
					Info.ContainingCell.Detectors++;
				}

				AllUnits.Add(Info);
			}

			int lol = 0;
			for (int i = 0; i < AllUnits.Count; i++)
			{
				UnitInfo A = AllUnits[i];
				double AArea = A.Radius * A.Radius;
				if (AArea == 0)
					continue;

				for (int j = i + 1; j < AllUnits.Count; j++)
				{
					UnitInfo B = AllUnits[j];
					double BArea = B.Radius * B.Radius;
					if (BArea == 0)
						continue;

					double Distance = (B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y);
					Distance *= Distance;
					if (Distance == 0)
						continue;

					A.PlayerWeights[B.Owner] += BArea / Distance;
					B.PlayerWeights[A.Owner] += AArea / Distance;

					A.TypeWeights[(int)B.Type] += BArea / Distance;
					B.TypeWeights[(int)A.Type] += AArea / Distance;
				}

				if (A.Detector)
				{ }

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
						SortedWeights.Add(new KeyValuePair<int, double>(p, A.TypeWeights[p]));
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
