namespace _2cs_API
{
	using Data;
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	public static class _2csAPI
	{
		private static string _lastButtonHotkey;
		private static ButtonGameHotkeys _lastButtonHotkeyName = ButtonGameHotkeys.None;
		private static string _lastUIHotkey;
		private static UIGameHotkeys _lastUIHotkeyName = UIGameHotkeys.None;
		private static List<KeyValuePair<ButtonGameHotkeys, string>> gameHotkeys = new List<KeyValuePair<ButtonGameHotkeys, string>>();
		private static List<KeyValuePair<UIGameHotkeys, string>> uiGameHotkeys = new List<KeyValuePair<UIGameHotkeys, string>>();

		public static UnitType BuildingOfTrainingUnit(UnitType trainingUnit)
		{
			if (Player.LocalPlayer.race == Race.Zerg)
			{
				return UnitType.Hatchery;
			}
			switch (trainingUnit)
			{
				case UnitType.SCV:
					return UnitType.CommandCenter;

				case UnitType.Marine:
					return UnitType.Barracks;

				case UnitType.Stalker:
					return UnitType.Gateway;

				case UnitType.VoidRay:
					return UnitType.Stargate;

				case UnitType.Probe:
					return UnitType.Nexus;
			}
			throw new NotImplementedException(trainingUnit.ToString() + " is not supported.");
		}

		public static double DistanceBetweenUnits(Unit unit1, Unit unit2)
		{
			float num = unit2.locationX - unit1.locationX;
			float num2 = unit2.locationY - unit1.locationY;
			return Math.Sqrt(Math.Pow((double) num, 2.0) + Math.Pow((double) num2, 2.0));
		}

		public static string EnglishMapName(string filePath)
		{
			MpqManager manager = new MpqManager(filePath);
			string str = manager.readFile(MpqManager.MapGameStringsFile);
			manager.Close();
			while (str.Contains("="))
			{
				int index = str.IndexOf("=");
				int num2 = -1;
				for (int i = 1; ((i < str.Length) && (str.IndexOf("/", i) != -1)) && (str.IndexOf("/", i) < index); i++)
				{
					num2 = str.IndexOf("/", i);
				}
				if (str.Substring(num2 + 1, (index - num2) - 1) == "Name")
				{
					return str.Substring(index + 1, (str.IndexOf("\r") - index) - 1);
				}
				str = str.Substring(str.IndexOf("\r") + 2);
			}
			throw new Exception("Name not found in file.");
		}

		public static List<Unit> FilterPlayerUnitsKeepPlayer(List<Unit> units, uint playerNumber)
		{
			List<Unit> list = new List<Unit>();
			foreach (Unit unit in units)
			{
				if (unit.playerNumber == playerNumber)
				{
					list.Add(unit);
				}
			}
			return list;
		}

		public static List<Unit> FilterPlayerUnitsRemovePlayer(List<Unit> units, uint playerNumber)
		{
			List<Unit> list = new List<Unit>();
			foreach (Unit unit in units)
			{
				if (unit.playerNumber != playerNumber)
				{
					list.Add(unit);
				}
			}
			return list;
		}

		public static List<Unit> FindBuildings(uint playerNumber)
		{
			return GameData.getUnitData().FindAll(delegate (Unit u) {
				if (u.playerNumber != playerNumber)
				{
					return false;
				}
				if (((u.state != UnitStateOld.BuildingDefault) && (u.state != UnitStateOld.BuildingMorphing)) && (u.state != UnitStateOld.Pylon))
				{
					return u.state == UnitStateOld.UnitDefaultBuildingAir;
				}
				return true;
			});
		}

		public static List<Unit> FindMainBuildings()
		{
			return FindMainBuildings(Player.LocalPlayerNumber);
		}

		public static List<Unit> FindMainBuildings(uint playerNumber)
		{
			return GameData.getUnitData().FindAll(delegate (Unit u) {
				if (u.playerNumber != playerNumber)
				{
					return false;
				}
				if ((((u.unitType != (ushort)UnitType.Hatchery) && (u.unitType != (ushort)(ushort)UnitType.Hive)) && ((u.unitType != (ushort)UnitType.Lair) && (u.unitType != (ushort)UnitType.CommandCenter))) && (((u.unitType != (ushort)UnitType.CommandCenterFlying) && (u.unitType != (ushort)UnitType.OrbitalCommand)) && ((u.unitType != (ushort)UnitType.OrbitalCommandFlying) && (u.unitType != (ushort)UnitType.PlanetaryFortress))))
				{
					return u.unitType == (ushort)UnitType.Nexus;
				}
				return true;
			});
		}

		public static Unit FindRandomBuilding(uint playerNumber)
		{
			List<Unit> list = FindBuildings(playerNumber);
			if (list.Count == 0)
			{
				return new Unit(0);
			}
			Random random = new Random();
			return list[random.Next(0, list.Count - 1)];
		}

		public static Unit FindUnit(UnitType unitType)
		{
			return FindUnit(unitType, Player.LocalPlayerNumber);
		}

		public static Unit FindUnit(int unitID)
		{
			return GameData.getUnitData().Find(u => u.ID == unitID);
		}

		public static Unit FindUnit(UnitType unitType, uint playerNumber)
		{
			return GameData.getUnitData().Find(u => (u.unitType == (ushort)unitType) && (u.playerNumber == playerNumber));
		}

		public static List<Unit> FindUnits(UnitType unitType)
		{
			return FindUnits(unitType, Player.LocalPlayerNumber);
		}

		public static List<Unit> FindUnits(UnitType[] unitTypes)
		{
			return FindUnits(unitTypes, Player.LocalPlayerNumber);
		}

		public static List<Unit> FindUnits(UnitType unitType, uint playerNumber)
		{
			return GameData.getUnitData().FindAll(u => (u.unitType == (ushort)unitType) && (u.playerNumber == playerNumber));
		}

		public static List<Unit> FindUnits(UnitType[] unitTypes, uint playerNumber)
		{
			List<Unit> list = GameData.getUnitData();
			List<Unit> list2 = new List<Unit>();
			Predicate<Unit> match = null;
			for (int i = 0; i < unitTypes.Length; i++)
			{
				if (match == null)
				{
					match = u => (u.unitType == (ushort)unitTypes[i]) && (u.playerNumber == playerNumber);
				}
				List<Unit> collection = list.FindAll(match);
				list2.AddRange(collection);
			}
			return list2;
		}

		public static Unit FindUnitUnderconstruction(UnitType unitType, uint playerNumber)
		{
			return GameData.getUnitData().Find(u => ((u.unitType == (ushort)unitType) && (u.healthDamage != 0)) && (u.playerNumber == playerNumber));
		}

		public static List<Unit> FindWorkers()
		{
			return FindWorkers(Player.LocalPlayerNumber);
		}

		public static List<Unit> FindWorkers(uint playerNumber)
		{
			List<Unit> list = GameData.getUnitData();
			List<Unit> list2 = new List<Unit>();
			List<Unit> collection = list.FindAll(u => IsWorker((UnitType)u.unitType) && (u.playerNumber == playerNumber));
			list2.AddRange(collection);
			return list2;
		}

		public static string GetGameHotkey(ButtonGameHotkeys gameHotkey)
		{
			if (gameHotkey == _lastButtonHotkeyName)
			{
				return _lastButtonHotkey;
			}
			string str = gameHotkeys.Find(p => ((ButtonGameHotkeys) p.Key) == gameHotkey).Value;
			if (str != null)
			{
				return str;
			}
			MpqManager manager = new MpqManager(MpqManager.PatchArchive);
			string str2 = manager.readFile(MpqManager.ButtonGameHotkeysFile);
			manager.Close();
			while (str2.Contains("="))
			{
				str2 = str2.Substring(str2.IndexOf("/") + 1);
				int index = str2.IndexOf("=");
				int num2 = str2.IndexOf("/");
				if (str2.Substring(num2 + 1, (index - num2) - 1) == gameHotkey.ToString())
				{
					_lastButtonHotkey = str2.Substring(index + 1, 1);
					gameHotkeys.Add(new KeyValuePair<ButtonGameHotkeys, string>(gameHotkey, _lastButtonHotkey));
					return _lastButtonHotkey;
				}
				str2 = str2.Substring(index + 1);
			}
			return null;
		}

		public static string GetGameHotkey(UIGameHotkeys gameHotkey)
		{
			if (gameHotkey == _lastUIHotkeyName)
			{
				return _lastUIHotkey;
			}
			string str = uiGameHotkeys.Find(p => ((UIGameHotkeys) p.Key) == gameHotkey).Value;
			if (str != null)
			{
				return str;
			}
			MpqManager manager = new MpqManager(MpqManager.PatchArchive);
			string str2 = manager.readFile(MpqManager.UIGameHotkeysFile);
			manager.Close();
			while (str2.Contains("="))
			{
				int index = str2.IndexOf("=");
				int num2 = -1;
				for (int i = 1; ((i < str2.Length) && (str2.IndexOf("/", i) != -1)) && (str2.IndexOf("/", i) < index); i++)
				{
					num2 = str2.IndexOf("/", i);
				}
				if (str2.Substring(num2 + 1, (index - num2) - 1) == gameHotkey.ToString())
				{
					_lastUIHotkey = str2.Substring(index + 1, (str2.IndexOf("\r") - index) - 1);
					uiGameHotkeys.Add(new KeyValuePair<UIGameHotkeys, string>(gameHotkey, _lastUIHotkey));
					return _lastUIHotkey;
				}
				str2 = str2.Substring(str2.IndexOf("\r") + 2);
			}
			throw new Exception("Game hotkey not found in file.");
		}

		public static List<Unit> GetMineralFields()
		{
			List<Unit> mainBuildings = FindMainBuildings();
			List<Unit> list = new List<Unit>();
			Predicate<Unit> match = null;
			for (int i = 0; i < mainBuildings.Count; i++)
			{
				if (match == null)
				{
					match = p => (p.unitType == (ushort)UnitType.MineralField) && (DistanceBetweenUnits(p, mainBuildings[i]) <= 10.0);
				}
				list.AddRange(GameData.getUnitData().FindAll(match));
			}
			return list;
		}

		public static int GetPlayerStartLocation()
		{
			return GetPlayerStartLocation(Player.LocalPlayerNumber);
		}

		public static int GetPlayerStartLocation(uint playerNumber)
		{
			byte[] buffer;
			GameData.mem.ReadMemory((uint) (GameData.Offsets.StartLocations + (4 * (playerNumber - 1))), 4, out buffer);
			return (BitConverter.ToInt32(buffer, 0) + 1);
		}

		public static Unit GetRandomMineralField()
		{
			List<Unit> mineralFields = GetMineralFields();
			if (mineralFields.Count == 0)
			{
				return new Unit(0);
			}
			Random random = new Random();
			return mineralFields[random.Next(0, mineralFields.Count - 1)];
		}

		public static List<Unit> GetResources()
		{
			List<Unit> mineralFields = GetMineralFields();
			mineralFields.AddRange(GetVespenes());
			return mineralFields;
		}

		public static List<Unit> GetVespenes()
		{
			List<Unit> mainBuildings = FindMainBuildings();
			List<Unit> list = new List<Unit>();
			Predicate<Unit> match = null;
			for (int i = 0; i < mainBuildings.Count; i++)
			{
				if (match == null)
				{
					match = delegate (Unit p) {
						if (((p.unitType != (ushort)UnitType.VespeneGeyser) && (p.unitType != (ushort)UnitType.SpacePlatformGeyser)) && ((p.unitType != (ushort)UnitType.TempleGeyser) && (p.unitType != (ushort)UnitType.RichVespeneGeyser)))
						{
							return false;
						}
						return DistanceBetweenUnits(p, mainBuildings[i]) <= 10.0;
					};
				}
				list.AddRange(GameData.getUnitData().FindAll(match));
			}
			return list;
		}

		public static List<Unit> GetWorkers()
		{
			UnitType workerType = WorkerType;
			uint localPlayerNumber = Player.LocalPlayerNumber;
			GameData.GetPlayerSelections((int)localPlayerNumber);
			return GameData.getUnitData().FindAll(p => ((p.playerNumber == localPlayerNumber) && (p.unitType == (ushort)workerType)) && (p.state != UnitStateOld.WorkerBuilding));
		}

		public static bool InGame()
		{
			return GameData.SecondsElapsed > 0;
		}

		public static bool IsWorker(UnitType unitType)
		{
			if (((unitType != UnitType.Drone) && (unitType != UnitType.Probe)) && (unitType != UnitType.SCV))
			{
				return false;
			}
			return true;
		}

		public static List<Unit> PlayerUnits(uint playerNumber)
		{
			List<Unit> list = GameData.getUnitData();
			if (Player.Count == 0)
			{
				return null;
			}
			return list.FindAll(u => u.playerNumber == playerNumber);
		}

		public static bool TerranIsAFaggot(uint playerNumber)
		{
			return (GameData.getUnitData().Find(delegate (Unit u) {
				if (u.playerNumber != playerNumber)
				{
					return false;
				}
				if (u.unitType != (ushort)UnitType.CommandCenterFlying)
				{
					return u.unitType == (ushort)UnitType.BarracksFlying;
				}
				return true;
			}).ID != 0);
		}

		public static int UnitCount(UnitType unitType)
		{
			return GameData.getUnitData().FindAll(u => u.unitType == (ushort)unitType).Count;
		}

		public static int UnitCount(uint playerNumber)
		{
			List<Unit> list = GameData.getUnitData();
			if (Player.Count == 0)
			{
				return 0;
			}
			return list.FindAll(u => u.playerNumber == playerNumber).Count;
		}

		public static int UnitCount(UnitType unitType, uint playerNumber)
		{
			return GameData.getUnitData().FindAll(u => (u.unitType == (ushort)unitType) && (u.playerNumber == playerNumber)).Count;
		}

		public static bool UnitExists(UnitType unitType)
		{
			return UnitExists(unitType, 1, Player.LocalPlayerNumber);
		}

		public static bool UnitExists(UnitType unitType, uint playerNumber)
		{
			return UnitExists(unitType, 1, playerNumber);
		}

		public static bool UnitExists(int quantity, UnitType unitType)
		{
			return UnitExists(unitType, quantity, Player.LocalPlayerNumber);
		}

		public static bool UnitExists(UnitType unitType, int quantity, uint playerNumber)
		{
			return (GameData.getUnitData().FindAll(u => (u.unitType == (ushort)unitType) && (u.playerNumber == playerNumber)).Count >= quantity);
		}

		public static string UnitHotkey(UnitType unit)
		{
			ButtonGameHotkeys gameHotkey = (ButtonGameHotkeys) Enum.Parse(typeof(ButtonGameHotkeys), unit.ToString());
			return GetGameHotkey(gameHotkey);
		}

		public static bool UnitOutOfTown(int unitID)
		{
			double num = 25.0;
			Unit unit = FindUnit(unitID);
			List<Unit> list = FindMainBuildings(unit.playerNumber);
			for (int i = 0; i < list.Count; i++)
			{
				if (DistanceBetweenUnits(unit, list[i]) >= num)
				{
					return true;
				}
			}
			return false;
		}

		public static bool UnitOutOfTown(int unitID, uint playersTown)
		{
			double num = 25.0;
			Unit unit = FindUnit(unitID);
			List<Unit> list = FindMainBuildings(playersTown);
			for (int i = 0; i < list.Count; i++)
			{
				if (DistanceBetweenUnits(unit, list[i]) >= num)
				{
					return true;
				}
			}
			return false;
		}

		public static Unit SelectedUnit
		{
			get
			{
				try
				{
					ControlGroup currentSelection = GameData.GetPlayerSelections(Player.LocalPlayer).currentSelection;
					if (currentSelection.selected_unit_ids.Count == 0)
					{
						return new Unit(0);
					}
					return FindUnit(currentSelection.selected_unit_ids[0]);
				}
				catch
				{
					return new Unit(0);
				}
			}
		}

		public static UnitType SelectedUnitType
		{
			get
			{
				return (UnitType)SelectedUnit.unitType;
			}
		}

		public static UnitType SupplyType
		{
			get
			{
				UnitType none;
				try
				{
					switch (Player.LocalPlayer.race)
					{
						case Race.Zerg:
							return UnitType.Overlord;

						case Race.Protoss:
							return UnitType.Pylon;

						case Race.Terran:
							return UnitType.SupplyDepot;
					}
					throw new Exception("Not a valid player to get supply for.");
				}
				catch
				{
					none = UnitType.None;
				}
				return none;
			}
		}

		public static UnitType VespeneType
		{
			get
			{
				UnitType none;
				try
				{
					switch (Player.LocalPlayer.race)
					{
						case Race.Zerg:
							return UnitType.Extractor;

						case Race.Protoss:
							return UnitType.Assimilator;

						case Race.Terran:
							return UnitType.Refinery;
					}
					throw new Exception("Not a valid player to get vespene building for.");
				}
				catch
				{
					none = UnitType.None;
				}
				return none;
			}
		}

		public static UnitType WorkerType
		{
			get
			{
				UnitType none;
				try
				{
					switch (Player.LocalPlayer.race)
					{
						case Race.Zerg:
							return UnitType.Drone;

						case Race.Protoss:
							return UnitType.Probe;

						case Race.Terran:
							return UnitType.SCV;
					}
					throw new Exception("Not a valid player to get worker for.");
				}
				catch
				{
					none = UnitType.None;
				}
				return none;
			}
		}

		public enum Ability
		{
			InjectLarvae
		}

		public enum ButtonGameHotkeys
		{
			None,
			_250mmStrikeCannons,
			_250mmStrikeCannons_NRS,
			_250mmStrikeCannons_USD,
			_250mmStrikeCannons_USDL,
			AWrp,
			AWrp_NRS,
			AWrp_USD,
			AWrp_USDL,
			AcquireMove,
			AcquireMove_NRS,
			AcquireMove_USD,
			AcquireMove_USDL,
			Archon,
			ArchonHallucination,
			ArchonHallucination_SC1,
			ArchonHallucination_USD,
			ArchonHallucination_USDL,
			ArchonWarpTarget,
			ArchonWarpTarget_USD,
			ArchonWarpTarget_USDL,
			Archon_USD,
			Archon_USDL,
			Armory,
			Armory_NRS,
			Armory_USD,
			Armory_USDL,
			AssaultMode,
			AssaultMode_NRS,
			AssaultMode_USD,
			AssaultMode_USDL,
			Assimilator,
			Assimilator_NRS,
			Assimilator_USD,
			Assimilator_USDL,
			Attack,
			AttackBuilding,
			AttackBuilding_NRS,
			AttackBuilding_USD,
			AttackBuilding_USDL,
			AttackWarpPrism,
			AttackWarpPrism_NRS,
			AttackWarpPrism_USD,
			AttackWarpPrism_USDL,
			Attack_NRS,
			Attack_USD,
			Attack_USDL,
			AutoTurret,
			AutoTurret_NRS,
			AutoTurret_USD,
			AutoTurret_USDL,
			Baneling,
			BanelingFromZerglingBurrowed,
			BanelingFromZerglingBurrowed_NRS,
			BanelingFromZerglingBurrowed_SC1,
			BanelingFromZerglingBurrowed_USD,
			BanelingFromZerglingBurrowed_USDL,
			BanelingNest,
			BanelingNest_USD,
			BanelingNest_USDL,
			Baneling_NRS,
			Baneling_SC1,
			Baneling_USD,
			Baneling_USDL,
			Banshee,
			Banshee_NRS,
			Banshee_USD,
			Banshee_USDL,
			Barracks,
			Barracks_USD,
			Barracks_USDL,
			Battlecruiser,
			Battlecruiser_USD,
			Battlecruiser_USDL,
			Blink,
			Blink_USD,
			Blink_USDL,
			BroodLord,
			BroodLord_SC1,
			BroodLord_USD,
			BroodLord_USDL,
			BuildCreepTumor,
			BuildCreepTumor_NRS,
			BuildCreepTumor_USD,
			BuildCreepTumor_USDL,
			BuildTechLabBarracks,
			BuildTechLabBarracks_NRS,
			BuildTechLabBarracks_SC1,
			BuildTechLabBarracks_USD,
			BuildTechLabBarracks_USDL,
			BuildTechLabFactory,
			BuildTechLabFactory_NRS,
			BuildTechLabFactory_SC1,
			BuildTechLabFactory_USD,
			BuildTechLabFactory_USDL,
			BuildTechLabStarport,
			BuildTechLabStarport_NRS,
			BuildTechLabStarport_SC1,
			BuildTechLabStarport_USD,
			BuildTechLabStarport_USDL,
			Bunker,
			BunkerLoad,
			BunkerLoad_NRS,
			BunkerLoad_USD,
			BunkerLoad_USDL,
			BunkerUnloadAll,
			BunkerUnloadAll_SC1,
			BunkerUnloadAll_USD,
			BunkerUnloadAll_USDL,
			Bunker_USD,
			Bunker_USDL,
			BurrowDown,
			BurrowDown_NRS,
			BurrowDown_SC1,
			BurrowDown_USD,
			BurrowDown_USDL,
			BurrowUp,
			BurrowUp_NRS,
			BurrowUp_SC1,
			BurrowUp_USD,
			BurrowUp_USDL,
			CalldownMULE,
			CalldownMULE_NRS,
			CalldownMULE_USD,
			CalldownMULE_USDL,
			Cancel,
			CancelBuilding,
			CancelBuilding_GLS,
			CancelBuilding_GRS,
			CancelBuilding_NRS,
			CancelBuilding_USD,
			CancelBuilding_USDL,
			CancelSlot,
			CancelSlot_GLS,
			CancelSlot_GRS,
			CancelSlot_NRS,
			CancelSlot_USD,
			CancelSlot_USDL,
			Cancel_GLS,
			Cancel_GRS,
			Cancel_NRS,
			Cancel_USD,
			Cancel_USDL,
			Carrier,
			Carrier_NRS,
			Carrier_USD,
			Carrier_USDL,
			Charge,
			Charge_NRS,
			Charge_USD,
			Charge_USDL,
			CloakOff,
			CloakOff_NRS,
			CloakOff_SC1,
			CloakOff_USD,
			CloakOff_USDL,
			CloakOnBanshee,
			CloakOnBanshee_NRS,
			CloakOnBanshee_USD,
			CloakOnBanshee_USDL,
			CloakOnGhost,
			CloakOnGhost_NRS,
			CloakOnGhost_USD,
			CloakOnGhost_USDL,
			CloakOn_NRS,
			CloakOn_USD,
			CloakOn_USDL,
			CloakingField,
			CloakingField_USD,
			CloakingField_USDL,
			Colossus,
			ColossusHallucination,
			ColossusHallucination_NRS,
			ColossusHallucination_SC1,
			ColossusHallucination_USD,
			ColossusHallucination_USDL,
			Colossus_NRS,
			Colossus_SC1,
			Colossus_USD,
			Colossus_USDL,
			CommandCenter,
			CommandCenterLoad,
			CommandCenterLoad_NRS,
			CommandCenterLoad_USD,
			CommandCenterLoad_USDL,
			CommandCenterUnloadAll,
			CommandCenterUnloadAll_USD,
			CommandCenterUnloadAll_USDL,
			CommandCenter_USD,
			CommandCenter_USDL,
			Contaminate,
			Contaminate_NRS,
			Contaminate_USD,
			Contaminate_USDL,
			CorruptionAbility,
			CorruptionAbility_NRS,
			CorruptionAbility_USD,
			CorruptionAbility_USDL,
			Corruptor,
			Corruptor_NRS,
			Corruptor_USD,
			Corruptor_USDL,
			CreepTumor,
			CreepTumor_NRS,
			CreepTumor_USD,
			CreepTumor_USDL,
			CyberneticsCore,
			CyberneticsCore_USD,
			CyberneticsCore_USDL,
			DarkShrine,
			DarkShrine_NRS,
			DarkShrine_SC1,
			DarkShrine_USD,
			DarkShrine_USDL,
			DarkTemplar,
			DarkTemplar_NRS,
			DarkTemplar_SC1,
			DarkTemplar_USD,
			DarkTemplar_USDL,
			Disguise,
			Disguise_USD,
			Disguise_USDL,
			Drone,
			Drone_NRS,
			Drone_USD,
			Drone_USDL,
			EMP,
			EMP_NRS,
			EMP_USD,
			EMP_USDL,
			EngineeringBay,
			EngineeringBay_NRS,
			EngineeringBay_USD,
			EngineeringBay_USDL,
			EvolutionChamber,
			EvolutionChamber_USD,
			EvolutionChamber_USDL,
			EvolveAnabolicSynthesis2,
			EvolveAnabolicSynthesis2_USD,
			EvolveAnabolicSynthesis2_USDL,
			EvolveCentrificalHooks,
			EvolveCentrificalHooks_USD,
			EvolveCentrificalHooks_USDL,
			EvolveChitinousPlating,
			EvolveChitinousPlating_USD,
			EvolveChitinousPlating_USDL,
			EvolveGlialRegeneration,
			EvolveGlialRegeneration_USD,
			EvolveGlialRegeneration_USDL,
			EvolveInfestorEnergyUpgrade,
			EvolveInfestorEnergyUpgrade_SC1,
			EvolveInfestorEnergyUpgrade_USD,
			EvolveInfestorEnergyUpgrade_USDL,
			EvolveOrganicCarapace,
			EvolveOrganicCarapace_NRS,
			EvolveOrganicCarapace_USD,
			EvolveOrganicCarapace_USDL,
			EvolvePeristalsis,
			EvolvePeristalsis_USD,
			EvolvePeristalsis_USDL,
			EvolveTunnelingClaws,
			EvolveTunnelingClaws_USD,
			EvolveTunnelingClaws_USDL,
			EvolveVentralSacks,
			EvolveVentralSacks_NRS,
			EvolveVentralSacks_SC1,
			EvolveVentralSacks_USD,
			EvolveVentralSacks_USDL,
			Explode,
			Explode_NRS,
			Explode_USD,
			Explode_USDL,
			Extractor,
			Extractor_NRS,
			Extractor_USD,
			Extractor_USDL,
			Factory,
			Factory_NRS,
			Factory_USD,
			Factory_USDL,
			Feedback,
			Feedback_NRS,
			Feedback_USD,
			Feedback_USDL,
			FighterMode,
			FighterMode_NRS,
			FighterMode_USD,
			FighterMode_USDL,
			FleetBeacon,
			FleetBeacon_NRS,
			FleetBeacon_USD,
			FleetBeacon_USDL,
			ForceField,
			ForceField_NRS,
			ForceField_USD,
			ForceField_USDL,
			Forge,
			Forge_NRS,
			Forge_USD,
			Forge_USDL,
			Frenzy,
			Frenzy_NRS,
			Frenzy_USD,
			Frenzy_USDL,
			FungalGrowth,
			FungalGrowth_NRS,
			FungalGrowth_USD,
			FungalGrowth_USDL,
			FusionCore,
			FusionCore_NRS,
			FusionCore_USD,
			FusionCore_USDL,
			Gateway,
			Gateway_USD,
			Gateway_USDL,
			Gather,
			GatherMULE,
			GatherMULE_USD,
			GatherMULE_USDL,
			GatherProt,
			GatherProt_USD,
			GatherProt_USDL,
			GatherTerr,
			GatherTerr_USD,
			GatherTerr_USDL,
			GatherZerg,
			GatherZerg_USD,
			GatherZerg_USDL,
			Gather_USD,
			Gather_USDL,
			GenerateCreep,
			GenerateCreep_USD,
			GenerateCreep_USDL,
			Ghost,
			GhostAcademy,
			GhostAcademy_USD,
			GhostAcademy_USDL,
			GhostHoldFire,
			GhostHoldFire_USD,
			GhostHoldFire_USDL,
			Ghost_USD,
			Ghost_USDL,
			GravitonBeam,
			GravitonBeam_NRS,
			GravitonBeam_USD,
			GravitonBeam_USDL,
			GreaterSpire,
			GreaterSpire_USD,
			GreaterSpire_USDL,
			GuardianShield,
			GuardianShield_USD,
			GuardianShield_USDL,
			Hallucination,
			Hallucination_NRS,
			Hallucination_SC1,
			Hallucination_USD,
			Hallucination_USDL,
			Halt,
			Halt_USD,
			Halt_USDL,
			HardenedShield,
			HardenedShield_USD,
			HardenedShield_USDL,
			Hatchery,
			Hatchery_USD,
			Hatchery_USDL,
			Heal,
			Heal_NRS,
			Heal_USD,
			Heal_USDL,
			Hellion,
			Hellion_NRS,
			Hellion_SC1,
			Hellion_USD,
			Hellion_USDL,
			HighTemplar,
			HighTemplarHallucination,
			HighTemplarHallucination_NRS,
			HighTemplarHallucination_USD,
			HighTemplarHallucination_USDL,
			HighTemplar_NRS,
			HighTemplar_USD,
			HighTemplar_USDL,
			Hive,
			Hive_USD,
			Hive_USDL,
			HoldFire,
			HoldFire_USD,
			HoldFire_USDL,
			HunterSeekerMissile,
			HunterSeekerMissile_NRS,
			HunterSeekerMissile_USD,
			HunterSeekerMissile_USDL,
			Hydralisk,
			HydraliskDen,
			HydraliskDen_SC1,
			HydraliskDen_USD,
			HydraliskDen_USDL,
			Hydralisk_USD,
			Hydralisk_USDL,
			Immortal,
			ImmortalHallucination,
			ImmortalHallucination_USD,
			ImmortalHallucination_USDL,
			Immortal_USD,
			Immortal_USDL,
			Infestation,
			InfestationPit,
			InfestationPit_USD,
			InfestationPit_USDL,
			Infestation_USD,
			Infestation_USDL,
			InfestedTerrans,
			InfestedTerrans_NRS,
			InfestedTerrans_SC1,
			InfestedTerrans_USD,
			InfestedTerrans_USDL,
			Infestor,
			Infestor_NRS,
			Infestor_USD,
			Infestor_USDL,
			Interceptor,
			Interceptor_USD,
			Interceptor_USDL,
			Lair,
			Lair_NRS,
			Lair_USD,
			Lair_USDL,
			Land,
			Land_NRS,
			Land_USD,
			Land_USDL,
			Larva,
			Larva_NRS,
			Larva_USD,
			Larva_USDL,
			Leech,
			Leech_USD,
			Leech_USDL,
			Lift,
			Lift_NRS,
			Lift_USD,
			Lift_USDL,
			Load,
			Load_NRS,
			Load_USD,
			Load_USDL,
			Lower,
			Lower_NRS,
			Lower_USD,
			Lower_USDL,
			Marauder,
			Marauder_NRS,
			Marauder_SC1,
			Marauder_USD,
			Marauder_USDL,
			Marine,
			Marine_NRS,
			Marine_SC1,
			Marine_USD,
			Marine_USDL,
			MassRecall,
			MassRecall_NRS,
			MassRecall_USD,
			MassRecall_USDL,
			Medivac,
			MedivacLoad,
			MedivacLoad_NRS,
			MedivacLoad_USD,
			MedivacLoad_USDL,
			MedivacUnloadAll,
			MedivacUnloadAll_SC1,
			MedivacUnloadAll_USD,
			MedivacUnloadAll_USDL,
			Medivac_NRS,
			Medivac_USD,
			Medivac_USDL,
			MissileTurret,
			MissileTurret_NRS,
			MissileTurret_USD,
			MissileTurret_USDL,
			MorphBackToGateway,
			MorphBackToGateway_NRS,
			MorphBackToGateway_USD,
			MorphBackToGateway_USDL,
			MorphMorphalisk,
			MorphMorphalisk_NRS,
			MorphMorphalisk_USD,
			MorphMorphalisk_USDL,
			MorphToOverseer,
			MorphToOverseer_USD,
			MorphToOverseer_USDL,
			Mothership,
			Mothership_USD,
			Mothership_USDL,
			Move,
			MoveHoldPosition,
			MoveHoldPosition_USD,
			MoveHoldPosition_USDL,
			MovePatrol,
			MovePatrol_USD,
			MovePatrol_USDL,
			Move_USD,
			Move_USDL,
			Mutalisk,
			Mutalisk_NRS,
			Mutalisk_SC1,
			Mutalisk_USD,
			Mutalisk_USDL,
			NeuralParasite,
			NeuralParasite_NRS,
			NeuralParasite_USD,
			NeuralParasite_USDL,
			Nexus,
			Nexus_USD,
			Nexus_USDL,
			NukeArm,
			NukeArm_USD,
			NukeArm_USDL,
			NukeCalldown,
			NukeCalldown_USD,
			NukeCalldown_USDL,
			NydusCanal,
			NydusCanalLoad,
			NydusCanalLoad_NRS,
			NydusCanalLoad_USD,
			NydusCanalLoad_USDL,
			NydusCanalUnloadAll,
			NydusCanalUnloadAll_SC1,
			NydusCanalUnloadAll_USD,
			NydusCanalUnloadAll_USDL,
			NydusCanal_SC1,
			NydusCanal_USD,
			NydusCanal_USDL,
			NydusNetwork,
			NydusNetwork_USD,
			NydusNetwork_USDL,
			Observer,
			Observer_NRS,
			Observer_SC1,
			Observer_USD,
			Observer_USDL,
			ObverseIncubation,
			ObverseIncubation_USD,
			ObverseIncubation_USDL,
			OrbitalCommand,
			OrbitalCommand_NRS,
			OrbitalCommand_SC1,
			OrbitalCommand_USD,
			OrbitalCommand_USDL,
			Overlord,
			OverlordTransportLoad,
			OverlordTransportLoad_NRS,
			OverlordTransportLoad_USD,
			OverlordTransportLoad_USDL,
			OverlordTransportUnload,
			OverlordTransportUnload_SC1,
			OverlordTransportUnload_USD,
			OverlordTransportUnload_USDL,
			Overlord_NRS,
			Overlord_SC1,
			Overlord_USD,
			Overlord_USDL,
			OverseerMut,
			OverseerMut_NRS,
			OverseerMut_SC1,
			OverseerMut_USD,
			OverseerMut_USDL,
			PhaseShift,
			PhaseShift_USD,
			PhaseShift_USDL,
			PhasingMode,
			PhasingMode_NRS,
			PhasingMode_USD,
			PhasingMode_USDL,
			Phoenix,
			PhoenixHallucination,
			PhoenixHallucination_NRS,
			PhoenixHallucination_SC1,
			PhoenixHallucination_USD,
			PhoenixHallucination_USDL,
			Phoenix_NRS,
			Phoenix_SC1,
			Phoenix_USD,
			Phoenix_USDL,
			PhotonCannon,
			PhotonCannon_NRS,
			PhotonCannon_USD,
			PhotonCannon_USDL,
			PlanetaryFortress,
			PlanetaryFortressLoad,
			PlanetaryFortressLoad_NRS,
			PlanetaryFortressLoad_USD,
			PlanetaryFortressLoad_USDL,
			PlanetaryFortress_USD,
			PlanetaryFortress_USDL,
			PointDefenseDrone,
			PointDefenseDrone_NRS,
			PointDefenseDrone_USD,
			PointDefenseDrone_USDL,
			Probe,
			ProbeHallucination,
			ProbeHallucination_NRS,
			ProbeHallucination_SC1,
			ProbeHallucination_USD,
			ProbeHallucination_USDL,
			Probe_NRS,
			Probe_SC1,
			Probe_USD,
			Probe_USDL,
			ProtossAirArmorLevel1,
			ProtossAirArmorLevel1_NRS,
			ProtossAirArmorLevel1_USD,
			ProtossAirArmorLevel1_USDL,
			ProtossAirArmorLevel2,
			ProtossAirArmorLevel2_NRS,
			ProtossAirArmorLevel2_USD,
			ProtossAirArmorLevel2_USDL,
			ProtossAirArmorLevel3,
			ProtossAirArmorLevel3_NRS,
			ProtossAirArmorLevel3_USD,
			ProtossAirArmorLevel3_USDL,
			ProtossAirWeaponsLevel1,
			ProtossAirWeaponsLevel1_NRS,
			ProtossAirWeaponsLevel1_SC1,
			ProtossAirWeaponsLevel1_USD,
			ProtossAirWeaponsLevel1_USDL,
			ProtossAirWeaponsLevel2,
			ProtossAirWeaponsLevel2_NRS,
			ProtossAirWeaponsLevel2_SC1,
			ProtossAirWeaponsLevel2_USD,
			ProtossAirWeaponsLevel2_USDL,
			ProtossAirWeaponsLevel3,
			ProtossAirWeaponsLevel3_NRS,
			ProtossAirWeaponsLevel3_SC1,
			ProtossAirWeaponsLevel3_USD,
			ProtossAirWeaponsLevel3_USDL,
			ProtossBuild,
			ProtossBuildAdvanced,
			ProtossBuildAdvanced_NRS,
			ProtossBuildAdvanced_USD,
			ProtossBuildAdvanced_USDL,
			ProtossBuild_USD,
			ProtossBuild_USDL,
			ProtossGroundArmorLevel1,
			ProtossGroundArmorLevel1_USD,
			ProtossGroundArmorLevel1_USDL,
			ProtossGroundArmorLevel2,
			ProtossGroundArmorLevel2_USD,
			ProtossGroundArmorLevel2_USDL,
			ProtossGroundArmorLevel3,
			ProtossGroundArmorLevel3_USD,
			ProtossGroundArmorLevel3_USDL,
			ProtossGroundWeaponsLevel1,
			ProtossGroundWeaponsLevel1_SC1,
			ProtossGroundWeaponsLevel1_USD,
			ProtossGroundWeaponsLevel1_USDL,
			ProtossGroundWeaponsLevel2,
			ProtossGroundWeaponsLevel2_SC1,
			ProtossGroundWeaponsLevel2_USD,
			ProtossGroundWeaponsLevel2_USDL,
			ProtossGroundWeaponsLevel3,
			ProtossGroundWeaponsLevel3_SC1,
			ProtossGroundWeaponsLevel3_USD,
			ProtossGroundWeaponsLevel3_USDL,
			ProtossShieldsLevel1,
			ProtossShieldsLevel1_USD,
			ProtossShieldsLevel1_USDL,
			ProtossShieldsLevel2,
			ProtossShieldsLevel2_USD,
			ProtossShieldsLevel2_USDL,
			ProtossShieldsLevel3,
			ProtossShieldsLevel3_USD,
			ProtossShieldsLevel3_USDL,
			PsiStorm,
			PsiStorm_NRS,
			PsiStorm_USD,
			PsiStorm_USDL,
			Pylon,
			Pylon_NRS,
			Pylon_SC1,
			Pylon_USD,
			Pylon_USDL,
			Queen,
			Queen_NRS,
			Queen_SC1,
			Queen_USD,
			Queen_USDL,
			Raise,
			Raise_NRS,
			Raise_USD,
			Raise_USDL,
			Rally,
			RallyEgg,
			RallyEgg_SC1,
			RallyEgg_USD,
			RallyEgg_USDL,
			Rally_SC1,
			Rally_USD,
			Rally_USDL,
			Raven,
			Raven_NRS,
			Raven_SC1,
			Raven_USD,
			Raven_USDL,
			Reactor,
			Reactor_NRS,
			Reactor_SC1,
			Reactor_USD,
			Reactor_USDL,
			Reaper,
			ReaperSpeed,
			ReaperSpeed_USD,
			ReaperSpeed_USDL,
			Reaper_NRS,
			Reaper_SC1,
			Reaper_USD,
			Reaper_USDL,
			Refinery,
			Refinery_NRS,
			Refinery_USD,
			Refinery_USDL,
			Repair,
			Repair_NRS,
			Repair_USD,
			Repair_USDL,
			ResearchBansheeCloak,
			ResearchBansheeCloak_NRS,
			ResearchBansheeCloak_USD,
			ResearchBansheeCloak_USDL,
			ResearchBattlecruiserEnergyUpgrade,
			ResearchBattlecruiserEnergyUpgrade_SC1,
			ResearchBattlecruiserEnergyUpgrade_USD,
			ResearchBattlecruiserEnergyUpgrade_USDL,
			ResearchBattlecruiserSpecializations,
			ResearchBattlecruiserSpecializations_SC1,
			ResearchBattlecruiserSpecializations_USD,
			ResearchBattlecruiserSpecializations_USDL,
			ResearchBurrow,
			ResearchBurrow_USD,
			ResearchBurrow_USDL,
			ResearchCharge,
			ResearchCharge_SC1,
			ResearchCharge_USD,
			ResearchCharge_USDL,
			ResearchDurableMaterials,
			ResearchDurableMaterials_SC1,
			ResearchDurableMaterials_USD,
			ResearchDurableMaterials_USDL,
			ResearchExtendedThermalLance,
			ResearchExtendedThermalLance_NRS,
			ResearchExtendedThermalLance_SC1,
			ResearchExtendedThermalLance_USD,
			ResearchExtendedThermalLance_USDL,
			ResearchGhostEnergyUpgrade,
			ResearchGhostEnergyUpgrade_USD,
			ResearchGhostEnergyUpgrade_USDL,
			ResearchGraviticBooster,
			ResearchGraviticBooster_SC1,
			ResearchGraviticBooster_USD,
			ResearchGraviticBooster_USDL,
			ResearchGraviticDrive,
			ResearchGraviticDrive_SC1,
			ResearchGraviticDrive_USD,
			ResearchGraviticDrive_USDL,
			ResearchHallucination,
			ResearchHallucination_USD,
			ResearchHallucination_USDL,
			ResearchHiSecAutoTracking,
			ResearchHiSecAutoTracking_USD,
			ResearchHiSecAutoTracking_USDL,
			ResearchHighCapacityBarrels,
			ResearchHighCapacityBarrels_USD,
			ResearchHighCapacityBarrels_USDL,
			ResearchHighTemplarEnergyUpgrade,
			ResearchHighTemplarEnergyUpgrade_USD,
			ResearchHighTemplarEnergyUpgrade_USDL,
			ResearchIncreasedRange,
			ResearchIncreasedRange_USD,
			ResearchIncreasedRange_USDL,
			ResearchInterceptorLaunchSpeedUpgrade,
			ResearchInterceptorLaunchSpeedUpgrade_SC1,
			ResearchInterceptorLaunchSpeedUpgrade_USD,
			ResearchInterceptorLaunchSpeedUpgrade_USDL,
			ResearchJotunBoosters,
			ResearchJotunBoosters_USD,
			ResearchJotunBoosters_USDL,
			ResearchMedivacEnergyUpgrade,
			ResearchMedivacEnergyUpgrade_SC1,
			ResearchMedivacEnergyUpgrade_USD,
			ResearchMedivacEnergyUpgrade_USDL,
			ResearchNeosteelFrame,
			ResearchNeosteelFrame_USD,
			ResearchNeosteelFrame_USDL,
			ResearchNeuralParasite,
			ResearchNeuralParasite_NRS,
			ResearchNeuralParasite_USD,
			ResearchNeuralParasite_USDL,
			ResearchPersonalCloaking,
			ResearchPersonalCloaking_NRS,
			ResearchPersonalCloaking_USD,
			ResearchPersonalCloaking_USDL,
			ResearchPsiStorm,
			ResearchPsiStorm_NRS,
			ResearchPsiStorm_SC1,
			ResearchPsiStorm_USD,
			ResearchPsiStorm_USDL,
			ResearchPunisherGrenades,
			ResearchPunisherGrenades_USD,
			ResearchPunisherGrenades_USDL,
			ResearchRavenEnergyUpgrade,
			ResearchRavenEnergyUpgrade_USD,
			ResearchRavenEnergyUpgrade_USDL,
			ResearchSeekerMissile,
			ResearchSeekerMissile_NRS,
			ResearchSeekerMissile_USD,
			ResearchSeekerMissile_USDL,
			ResearchShieldWall,
			ResearchShieldWall_USD,
			ResearchShieldWall_USDL,
			ResearchSiegeTech,
			ResearchSiegeTech_USD,
			ResearchSiegeTech_USDL,
			ResearchStalkerTeleport,
			ResearchStalkerTeleport_USD,
			ResearchStalkerTeleport_USDL,
			ResearchStrikeCannons,
			ResearchStrikeCannons_USD,
			ResearchStrikeCannons_USDL,
			ResearchVoidRaySpeedUpgrade,
			ResearchVoidRaySpeedUpgrade_SC1,
			ResearchVoidRaySpeedUpgrade_USD,
			ResearchVoidRaySpeedUpgrade_USDL,
			ResearchWarpGate,
			ResearchWarpGate_USD,
			ResearchWarpGate_USDL,
			ReturnCargo,
			ReturnCargo_USD,
			ReturnCargo_USDL,
			Roach,
			RoachWarren,
			RoachWarren_NRS,
			RoachWarren_USD,
			RoachWarren_USDL,
			Roach_NRS,
			Roach_USD,
			Roach_USDL,
			RoboticsBay,
			RoboticsBay_USD,
			RoboticsBay_USDL,
			RoboticsFacility,
			RoboticsFacility_NRS,
			RoboticsFacility_USD,
			RoboticsFacility_USDL,
			SCV,
			SCV_NRS,
			SCV_USD,
			SCV_USDL,
			Salvage,
			Salvage_USD,
			Salvage_USDL,
			SapStructure,
			SapStructure_NRS,
			SapStructure_USD,
			SapStructure_USDL,
			Scan,
			Scan_NRS,
			Scan_USD,
			Scan_USDL,
			SelectBuilder,
			SelectBuilder_NRS,
			SelectBuilder_SC1,
			SelectBuilder_USD,
			SelectBuilder_USDL,
			SensorTower,
			SensorTower_NRS,
			SensorTower_USD,
			SensorTower_USDL,
			Sentry,
			Sentry_NRS,
			Sentry_USD,
			Sentry_USDL,
			SetBunkerRallyPoint,
			SetBunkerRallyPoint_SC1,
			SetBunkerRallyPoint_USD,
			SetBunkerRallyPoint_USDL,
			SetRallyPoint,
			SetRallyPoint2,
			SetRallyPoint2_SC1,
			SetRallyPoint2_USD,
			SetRallyPoint2_USDL,
			SetRallyPoint_SC1,
			SetRallyPoint_USD,
			SetRallyPoint_USDL,
			SiegeMode,
			SiegeMode_NRS,
			SiegeMode_SC1,
			SiegeMode_USD,
			SiegeMode_USDL,
			SiegeTank,
			SiegeTank_NRS,
			SiegeTank_SC1,
			SiegeTank_USD,
			SiegeTank_USDL,
			Siphon,
			Siphon_USD,
			Siphon_USDL,
			Snipe,
			Snipe_NRS,
			Snipe_USD,
			Snipe_USDL,
			SpawnChangeling,
			SpawnChangeling_NRS,
			SpawnChangeling_USD,
			SpawnChangeling_USDL,
			SpawningPool,
			SpawningPool_NRS,
			SpawningPool_USD,
			SpawningPool_USDL,
			SpineCrawler,
			SpineCrawlerRoot,
			SpineCrawlerRoot_NRS,
			SpineCrawlerRoot_SC1,
			SpineCrawlerRoot_USD,
			SpineCrawlerRoot_USDL,
			SpineCrawlerUproot,
			SpineCrawlerUproot_NRS,
			SpineCrawlerUproot_SC1,
			SpineCrawlerUproot_USD,
			SpineCrawlerUproot_USDL,
			SpineCrawler_NRS,
			SpineCrawler_USD,
			SpineCrawler_USDL,
			Spire,
			Spire_NRS,
			Spire_USD,
			Spire_USDL,
			SporeCrawler,
			SporeCrawlerRoot,
			SporeCrawlerRoot_NRS,
			SporeCrawlerRoot_SC1,
			SporeCrawlerRoot_USD,
			SporeCrawlerRoot_USDL,
			SporeCrawlerUproot,
			SporeCrawlerUproot_NRS,
			SporeCrawlerUproot_SC1,
			SporeCrawlerUproot_USD,
			SporeCrawlerUproot_USDL,
			SporeCrawler_NRS,
			SporeCrawler_SC1,
			SporeCrawler_USD,
			SporeCrawler_USDL,
			Stalker,
			StalkerHallucination,
			StalkerHallucination_NRS,
			StalkerHallucination_SC1,
			StalkerHallucination_USD,
			StalkerHallucination_USDL,
			Stalker_NRS,
			Stalker_SC1,
			Stalker_USD,
			Stalker_USDL,
			Stargate,
			Stargate_NRS,
			Stargate_USD,
			Stargate_USDL,
			Starport,
			Starport_NRS,
			Starport_USD,
			Starport_USDL,
			Stim,
			StimMarauder,
			StimMarauder_NRS,
			StimMarauder_USD,
			StimMarauder_USDL,
			StimPack,
			StimPack_NRS,
			StimPack_USD,
			StimPack_USDL,
			StimRedirect,
			StimRedirect_NRS,
			StimRedirect_USD,
			StimRedirect_USDL,
			Stim_NRS,
			Stim_USD,
			Stim_USDL,
			Stimpack,
			Stimpack_USD,
			Stimpack_USDL,
			Stop,
			StopGenerateCreep,
			StopGenerateCreep_NRS,
			StopGenerateCreep_SC1,
			StopGenerateCreep_USD,
			StopGenerateCreep_USDL,
			StopPlanetaryFortress,
			StopPlanetaryFortress_USD,
			StopPlanetaryFortress_USDL,
			StopRoachBurrowed,
			StopRoachBurrowed_USD,
			StopRoachBurrowed_USDL,
			StopSpecial,
			StopSpecial_USD,
			StopSpecial_USDL,
			Stop_NRS,
			Stop_USD,
			Stop_USDL,
			SummonNydusWorm,
			SummonNydusWorm_SC1,
			SummonNydusWorm_USD,
			SummonNydusWorm_USDL,
			SupplyDepot,
			SupplyDepot_NRS,
			SupplyDepot_USD,
			SupplyDepot_USDL,
			SupplyDrop,
			SupplyDrop_NRS,
			SupplyDrop_USD,
			SupplyDrop_USDL,
			TechLabBarracks,
			TechLabBarracks_NRS,
			TechLabBarracks_SC1,
			TechLabBarracks_USD,
			TechLabBarracks_USDL,
			TechLabFactory,
			TechLabFactory_NRS,
			TechLabFactory_SC1,
			TechLabFactory_USD,
			TechLabFactory_USDL,
			TechLabStarport,
			TechLabStarport_NRS,
			TechLabStarport_SC1,
			TechLabStarport_USD,
			TechLabStarport_USDL,
			TemplarArchive,
			TemplarArchive_NRS,
			TemplarArchive_USD,
			TemplarArchive_USDL,
			TemporalRift,
			TemporalRift_USD,
			TemporalRift_USDL,
			TerranBuild,
			TerranBuildAdvanced,
			TerranBuildAdvanced_NRS,
			TerranBuildAdvanced_USD,
			TerranBuildAdvanced_USDL,
			TerranBuild_USD,
			TerranBuild_USDL,
			TerranInfantryArmorLevel1,
			TerranInfantryArmorLevel1_USD,
			TerranInfantryArmorLevel1_USDL,
			TerranInfantryArmorLevel2,
			TerranInfantryArmorLevel2_USD,
			TerranInfantryArmorLevel2_USDL,
			TerranInfantryArmorLevel3,
			TerranInfantryArmorLevel3_USD,
			TerranInfantryArmorLevel3_USDL,
			TerranInfantryWeaponsLevel1,
			TerranInfantryWeaponsLevel1_NRS,
			TerranInfantryWeaponsLevel1_SC1,
			TerranInfantryWeaponsLevel1_USD,
			TerranInfantryWeaponsLevel1_USDL,
			TerranInfantryWeaponsLevel2,
			TerranInfantryWeaponsLevel2_NRS,
			TerranInfantryWeaponsLevel2_SC1,
			TerranInfantryWeaponsLevel2_USD,
			TerranInfantryWeaponsLevel2_USDL,
			TerranInfantryWeaponsLevel3,
			TerranInfantryWeaponsLevel3_NRS,
			TerranInfantryWeaponsLevel3_SC1,
			TerranInfantryWeaponsLevel3_USD,
			TerranInfantryWeaponsLevel3_USDL,
			TerranShipPlatingLevel1,
			TerranShipPlatingLevel1_SC1,
			TerranShipPlatingLevel1_USD,
			TerranShipPlatingLevel1_USDL,
			TerranShipPlatingLevel2,
			TerranShipPlatingLevel2_SC1,
			TerranShipPlatingLevel2_USD,
			TerranShipPlatingLevel2_USDL,
			TerranShipPlatingLevel3,
			TerranShipPlatingLevel3_SC1,
			TerranShipPlatingLevel3_USD,
			TerranShipPlatingLevel3_USDL,
			TerranShipWeaponsLevel1,
			TerranShipWeaponsLevel1_USD,
			TerranShipWeaponsLevel1_USDL,
			TerranShipWeaponsLevel2,
			TerranShipWeaponsLevel2_USD,
			TerranShipWeaponsLevel2_USDL,
			TerranShipWeaponsLevel3,
			TerranShipWeaponsLevel3_USD,
			TerranShipWeaponsLevel3_USDL,
			TerranVehiclePlatingLevel1,
			TerranVehiclePlatingLevel1_SC1,
			TerranVehiclePlatingLevel1_USD,
			TerranVehiclePlatingLevel1_USDL,
			TerranVehiclePlatingLevel2,
			TerranVehiclePlatingLevel2_SC1,
			TerranVehiclePlatingLevel2_USD,
			TerranVehiclePlatingLevel2_USDL,
			TerranVehiclePlatingLevel3,
			TerranVehiclePlatingLevel3_SC1,
			TerranVehiclePlatingLevel3_USD,
			TerranVehiclePlatingLevel3_USDL,
			TerranVehicleWeaponsLevel1,
			TerranVehicleWeaponsLevel1_NRS,
			TerranVehicleWeaponsLevel1_SC1,
			TerranVehicleWeaponsLevel1_USD,
			TerranVehicleWeaponsLevel1_USDL,
			TerranVehicleWeaponsLevel2,
			TerranVehicleWeaponsLevel2_NRS,
			TerranVehicleWeaponsLevel2_SC1,
			TerranVehicleWeaponsLevel2_USD,
			TerranVehicleWeaponsLevel2_USDL,
			TerranVehicleWeaponsLevel3,
			TerranVehicleWeaponsLevel3_NRS,
			TerranVehicleWeaponsLevel3_SC1,
			TerranVehicleWeaponsLevel3_USD,
			TerranVehicleWeaponsLevel3_USDL,
			Thor,
			Thor_NRS,
			Thor_SC1,
			Thor_USD,
			Thor_USDL,
			TimeWarp,
			TimeWarp_NRS,
			TimeWarp_USD,
			TimeWarp_USDL,
			Transfusion,
			Transfusion_USD,
			Transfusion_USDL,
			TransportMode,
			TransportMode_NRS,
			TransportMode_USD,
			TransportMode_USDL,
			Turn,
			Turn_USD,
			Turn_USDL,
			TwilightCouncil,
			TwilightCouncil_NRS,
			TwilightCouncil_USD,
			TwilightCouncil_USDL,
			Ultralisk,
			UltraliskCavern,
			UltraliskCavern_USD,
			UltraliskCavern_USDL,
			Ultralisk_NRS,
			Ultralisk_USD,
			Ultralisk_USDL,
			Unsiege,
			Unsiege_NRS,
			Unsiege_SC1,
			Unsiege_USD,
			Unsiege_USDL,
			UpgradeBuildingArmorLevel1,
			UpgradeBuildingArmorLevel1_USD,
			UpgradeBuildingArmorLevel1_USDL,
			UpgradeToPlanetaryFortress,
			UpgradeToPlanetaryFortress_USD,
			UpgradeToPlanetaryFortress_USDL,
			UpgradeToWarpGate,
			UpgradeToWarpGate_USD,
			UpgradeToWarpGate_USDL,
			VikingFighter,
			VikingFighter_NRS,
			VikingFighter_SC1,
			VikingFighter_USD,
			VikingFighter_USDL,
			VoidRay,
			VoidRayHallucination,
			VoidRayHallucination_SC1,
			VoidRayHallucination_USD,
			VoidRayHallucination_USDL,
			VoidRay_NRS,
			VoidRay_SC1,
			VoidRay_USD,
			VoidRay_USDL,
			Vortex,
			Vortex_NRS,
			Vortex_USD,
			Vortex_USDL,
			WarpPrism,
			WarpPrismHallucination,
			WarpPrismHallucination_NRS,
			WarpPrismHallucination_SC1,
			WarpPrismHallucination_USD,
			WarpPrismHallucination_USDL,
			WarpPrismLoad,
			WarpPrismLoad_NRS,
			WarpPrismLoad_USD,
			WarpPrismLoad_USDL,
			WarpPrismUnloadAll,
			WarpPrismUnloadAll_SC1,
			WarpPrismUnloadAll_USD,
			WarpPrismUnloadAll_USDL,
			WarpPrism_NRS,
			WarpPrism_SC1,
			WarpPrism_USD,
			WarpPrism_USDL,
			WeaponsFree,
			WeaponsFree_USD,
			WeaponsFree_USDL,
			WormholeTransit,
			WormholeTransit_USD,
			WormholeTransit_USDL,
			YamatoGun,
			YamatoGun_USD,
			YamatoGun_USDL,
			Zealot,
			ZealotHallucination,
			ZealotHallucination_NRS,
			ZealotHallucination_USD,
			ZealotHallucination_USDL,
			Zealot_NRS,
			Zealot_USD,
			Zealot_USDL,
			ZergBuild,
			ZergBuildAdvanced,
			ZergBuildAdvanced_NRS,
			ZergBuildAdvanced_USD,
			ZergBuildAdvanced_USDL,
			ZergBuild_USD,
			ZergBuild_USDL,
			Zergling,
			Zergling_NRS,
			Zergling_USD,
			Zergling_USDL,
			hydraliskspeed,
			hydraliskspeed_USD,
			hydraliskspeed_USDL,
			overlordspeed,
			overlordspeed_NRS,
			overlordspeed_USD,
			overlordspeed_USDL,
			zergflyerarmor1,
			zergflyerarmor1_USD,
			zergflyerarmor1_USDL,
			zergflyerarmor2,
			zergflyerarmor2_USD,
			zergflyerarmor2_USDL,
			zergflyerarmor3,
			zergflyerarmor3_USD,
			zergflyerarmor3_USDL,
			zergflyerattack1,
			zergflyerattack1_USD,
			zergflyerattack1_USDL,
			zergflyerattack2,
			zergflyerattack2_USD,
			zergflyerattack2_USDL,
			zergflyerattack3,
			zergflyerattack3_USD,
			zergflyerattack3_USDL,
			zerggroundarmor1,
			zerggroundarmor1_USD,
			zerggroundarmor1_USDL,
			zerggroundarmor2,
			zerggroundarmor2_USD,
			zerggroundarmor2_USDL,
			zerggroundarmor3,
			zerggroundarmor3_USD,
			zerggroundarmor3_USDL,
			zerglingattackspeed,
			zerglingattackspeed_USD,
			zerglingattackspeed_USDL,
			zerglingmovementspeed,
			zerglingmovementspeed_USD,
			zerglingmovementspeed_USDL,
			zergmeleeweapons1,
			zergmeleeweapons1_USD,
			zergmeleeweapons1_USDL,
			zergmeleeweapons2,
			zergmeleeweapons2_USD,
			zergmeleeweapons2_USDL,
			zergmeleeweapons3,
			zergmeleeweapons3_USD,
			zergmeleeweapons3_USDL,
			zergmissileweapons1,
			zergmissileweapons1_USD,
			zergmissileweapons1_USDL,
			zergmissileweapons2,
			zergmissileweapons2_USD,
			zergmissileweapons2_USDL,
			zergmissileweapons3,
			zergmissileweapons3_USD,
			zergmissileweapons3_USDL
		}

		[StructLayout(LayoutKind.Sequential, Size=1)]
		public struct Player
		{
			public static List<Data.Player> ActualPlayers
			{
				get
				{
					return GameData.getPlayersData().FindAll(delegate (Data.Player f) {
						if (f.playerType != PlayerType.Computer)
						{
							return f.playerType == PlayerType.User;
						}
						return true;
					});
				}
			}
			public static int ActualPlayerCount
			{
				get
				{
					return ActualPlayers.Count;
				}
			}
			public static int LocalPlayerIdleWorkersCount
			{
				get
				{
					return _2csAPI.FindWorkers().FindAll(f => f.commandQueuePointer == 0).Count;
				}
			}
			public static Data.Player LocalPlayer
			{
				get
				{
					try
					{
						return GameData.GetPlayer((int)LocalPlayerNumber);
					}
					catch
					{
						return new Data.Player(0);
					}
				}
			}
			public static Race LocalPlayerRace
			{
				get
				{
					return LocalPlayer.race;
				}
			}
			public static int LocalPlayerTeam
			{
				get
				{
					uint playerNumber = LocalPlayerNumber;
					Data.Player local = GameData.getPlayersData().Find(p => p.number == playerNumber);
					if (local != null)
						return local.team;
					else
						return 0;
				}
			}
			public static List<Data.Player> Enemies
			{
				get
				{
					int localTeam = LocalPlayerTeam;
					return GameData.getPlayersData().FindAll(p => (p.team != localTeam) && (p.playerType != PlayerType.Neutral));
				}
			}
			public static int Count
			{
				get
				{
					return GameData.getPlayersData().Count;
				}
			}
			public static uint LocalPlayerNumber
			{
				get
				{
					byte num = (byte)GameData.offsets.ReadStructMember(ORNames.LocalPlayer, ORNames.LocalPlayer);
					return num < 16 ? num : 0u;
				}
			}
			public static bool LocalPlayerGameOver
			{
				get
				{
					if (LocalPlayer.victoryStatus == VictoryStatus.Playing)
					{
						return false;
					}
					return true;
				}
			}
		}

		public enum UIGameHotkeys
		{
			None,
			Taunt,
			Accept_Hotkey,
			Apply_Hotkey,
			Back_Hotkey,
			BattleNet_Hotkey,
			Campaign_Hotkey,
			CampaignPanelContinue_Hotkey,
			CampaignPanelLoad_Hotkey,
			CampaignPanelNewCampaign_Hotkey,
			CampaignPanelTutorial_Hotkey,
			ScreenReplayBack_Hotkey,
			ScreenReplayLoad_Hotkey,
			ScreenSingleChallenge_Hotkey,
			ScreenSingleCustomGame_Hotkey,
			ChallengePanelBack_Hotkey,
			ChallengePanelPlay_Hotkey,
			CancelMulti_Hotkey,
			Cancel_Hotkey,
			Challenge_Hotkey,
			Chat_Hotkey,
			Clan_Hotkey,
			Close_Hotkey,
			Continue_Hotkey,
			CreateGame_Hotkey,
			Credits_Hotkey,
			CustomGame_Hotkey,
			DebugMap_Hotkey,
			Defaults_Hotkey,
			Delete_Hotkey,
			DropPlayers_Hotkey,
			ExitDialog_No_Hotkey,
			ExitDialog_Yes_Hotkey,
			GameLaunchVersionQueryReplayLoadButton_Hotkey,
			GameLaunchVersionQuerySaveLoadButton_Hotkey,
			GameMenuAbortMission_Hotkey,
			GameMenuAccept_Hotkey,
			GameMenuCancel_Hotkey,
			GameMenuClose_Hotkey,
			GameMenuDefault_Hotkey,
			GameMenuDelete_Hotkey,
			GameMenuExitProgram_Hotkey,
			GameMenuGameplay_Hotkey,
			GameMenuGraphics_Hotkey,
			GameMenuHotkeys_Hotkey,
			GameMenuLoad_Hotkey,
			GameMenuLoadLast_Hotkey,
			GameMenuOptions_Hotkey,
			GameMenuPause_Hotkey,
			GameMenuQuitCampaign_Hotkey,
			GameMenuQuitReplay_Hotkey,
			GameMenuQuit_Hotkey,
			GameMenuReplay_Hotkey,
			GameMenuRestart_Hotkey,
			GameMenuResume_Hotkey,
			GameMenuReturn_Hotkey,
			GameMenuSave_Hotkey,
			GameMenuScoreScreen_Hotkey,
			GameMenuSound_Hotkey,
			GameMenuSurrender_Hotkey,
			GameMenuVideo_Hotkey,
			GameMenuVoice_Hotkey,
			Gameplay_Hotkey,
			Graph_Hotkey,
			Graphics_Hotkey,
			Help_Hotkey,
			Hotkeys_Hotkey,
			JoinGame_Hotkey,
			Ladder_Hotkey,
			Load_Hotkey,
			Login_Hotkey,
			Mail_Hotkey,
			MercenaryPanelClose_Hotkey,
			Multiplayer_Hotkey,
			New_Hotkey,
			News_Hotkey,
			Ok_Hotkey,
			Options_Hotkey,
			Overview_Hotkey,
			PlanetPanelLaunch_Hotkey,
			PlayGame_Hotkey,
			Play_Hotkey,
			Profile_Hotkey,
			Quit_Hotkey,
			Ready_Hotkey,
			Replay_Hotkey,
			Resources_Hotkey,
			SaveReplay_Hotkey,
			Save_Hotkey,
			SendChat_Hotkey,
			Singleplayer_Hotkey,
			Skirmish_Hotkey,
			Social_Hotkey,
			Sound_Hotkey,
			StandardDialog_Cancel_Hotkey,
			StandardDialog_No_Hotkey,
			StandardDialog_Ok_Hotkey,
			StandardDialog_Yes_Hotkey,
			StartGame_Hotkey,
			Stereoscopic_Hotkey,
			Structures_Hotkey,
			Table_Hotkey,
			Units_Hotkey,
			VictoryPanelContinue_Hotkey,
			VictoryPanelPlay_Again_Hotkey,
			Video_Hotkey,
			ViewReplay_Hotkey,
			Voice_Hotkey,
			WaitingForServerSurrender_Hotkey,
			AICommunication,
			AIAssault,
			AICancel,
			AIDefend,
			AIDetect,
			AIExpand,
			AIHarass,
			AIRally,
			AIScout,
			AIMinerals,
			AIVespene,
			AlertRecall,
			CameraCenter,
			CameraFollow,
			CameraMoveDown,
			CameraMoveLeft,
			CameraMoveRight,
			CameraMoveUp,
			CameraSave0,
			CameraSave0_NRS,
			CameraSave0_GRS,
			CameraSave1,
			CameraSave1_NRS,
			CameraSave1_GRS,
			CameraSave2,
			CameraSave2_NRS,
			CameraSave2_GRS,
			CameraSave3,
			CameraSave3_NRS,
			CameraSave3_GRS,
			CameraSave4,
			CameraSave4_NRS,
			CameraSave4_GRS,
			CameraSave5,
			CameraSave5_NRS,
			CameraSave5_GRS,
			CameraSave6,
			CameraSave6_NRS,
			CameraSave6_GRS,
			CameraSave7,
			CameraSave7_NRS,
			CameraSave7_GRS,
			CameraTurnLeft,
			CameraTurnRight,
			CameraView0,
			CameraView0_NRS,
			CameraView0_GRS,
			CameraView1,
			CameraView1_NRS,
			CameraView1_GRS,
			CameraView2,
			CameraView2_NRS,
			CameraView2_GRS,
			CameraView3,
			CameraView3_NRS,
			CameraView3_GRS,
			CameraView4,
			CameraView4_NRS,
			CameraView4_GRS,
			CameraView5,
			CameraView5_NRS,
			CameraView5_GRS,
			CameraView6,
			CameraView6_NRS,
			CameraView6_GRS,
			CameraView7,
			CameraView7_NRS,
			CameraView7_GRS,
			CameraZoomFirst,
			CameraZoomLast,
			CameraZoomNext,
			CameraZoomPrev,
			CameraPush,
			ChatAll,
			ChatAll_NRS,
			ChatAll_GRS,
			ChatAllies,
			ChatAllies_NRS,
			ChatAllies_GRS,
			ChatCancel,
			ChatCancel_NRS,
			ChatCancel_GRS,
			ChatDefault,
			ChatDefault_NRS,
			ChatDefault_GRS,
			ChatIndividual,
			ChatIndividual_NRS,
			ChatRecipient,
			ChatSend,
			CinematicSkip,
			CinematicSkip_NRS,
			CinematicSkip_GRS,
			CommandButton00_GLS,
			CommandButton00_GRS,
			CommandButton01_GLS,
			CommandButton01_GRS,
			CommandButton02_GLS,
			CommandButton02_GRS,
			CommandButton03_GLS,
			CommandButton03_GRS,
			CommandButton04_GLS,
			CommandButton04_GRS,
			CommandButton05_GLS,
			CommandButton05_GRS,
			CommandButton06_GLS,
			CommandButton06_GRS,
			CommandButton07_GLS,
			CommandButton07_GRS,
			CommandButton08_GLS,
			CommandButton08_GRS,
			CommandButton09_GLS,
			CommandButton09_GRS,
			CommandButton10_GLS,
			CommandButton10_GRS,
			CommandButton11_GLS,
			CommandButton11_GRS,
			CommandButton12_GLS,
			CommandButton12_GRS,
			CommandButton13_GLS,
			CommandButton13_GRS,
			CommandButton14_GLS,
			CommandButton14_GRS,
			ControlGroupAppend1,
			ControlGroupAppend1_NRS,
			ControlGroupAppend1_GRS,
			ControlGroupAppend2,
			ControlGroupAppend2_NRS,
			ControlGroupAppend2_GRS,
			ControlGroupAppend3,
			ControlGroupAppend3_NRS,
			ControlGroupAppend3_GRS,
			ControlGroupAppend4,
			ControlGroupAppend4_NRS,
			ControlGroupAppend4_GRS,
			ControlGroupAppend5,
			ControlGroupAppend5_NRS,
			ControlGroupAppend5_GRS,
			ControlGroupAppend6,
			ControlGroupAppend6_NRS,
			ControlGroupAppend6_GRS,
			ControlGroupAppend7,
			ControlGroupAppend7_NRS,
			ControlGroupAppend7_GRS,
			ControlGroupAppend8,
			ControlGroupAppend8_NRS,
			ControlGroupAppend8_GRS,
			ControlGroupAppend9,
			ControlGroupAppend9_NRS,
			ControlGroupAppend9_GRS,
			ControlGroupAppend0,
			ControlGroupAppend0_NRS,
			ControlGroupAppend0_GRS,
			ControlGroupAssign1,
			ControlGroupAssign1_NRS,
			ControlGroupAssign1_GRS,
			ControlGroupAssign2,
			ControlGroupAssign2_NRS,
			ControlGroupAssign2_GRS,
			ControlGroupAssign3,
			ControlGroupAssign3_NRS,
			ControlGroupAssign3_GRS,
			ControlGroupAssign4,
			ControlGroupAssign4_NRS,
			ControlGroupAssign4_GRS,
			ControlGroupAssign5,
			ControlGroupAssign5_NRS,
			ControlGroupAssign5_GRS,
			ControlGroupAssign6,
			ControlGroupAssign6_NRS,
			ControlGroupAssign6_GRS,
			ControlGroupAssign7,
			ControlGroupAssign7_NRS,
			ControlGroupAssign7_GRS,
			ControlGroupAssign8,
			ControlGroupAssign8_NRS,
			ControlGroupAssign8_GRS,
			ControlGroupAssign9,
			ControlGroupAssign9_NRS,
			ControlGroupAssign9_GRS,
			ControlGroupAssign0,
			ControlGroupAssign0_NRS,
			ControlGroupAssign0_GRS,
			ControlGroupRecall1,
			ControlGroupRecall1_NRS,
			ControlGroupRecall1_GRS,
			ControlGroupRecall2,
			ControlGroupRecall2_NRS,
			ControlGroupRecall2_GRS,
			ControlGroupRecall3,
			ControlGroupRecall3_NRS,
			ControlGroupRecall3_GRS,
			ControlGroupRecall4,
			ControlGroupRecall4_NRS,
			ControlGroupRecall4_GRS,
			ControlGroupRecall5,
			ControlGroupRecall5_NRS,
			ControlGroupRecall5_GRS,
			ControlGroupRecall6,
			ControlGroupRecall6_NRS,
			ControlGroupRecall6_GRS,
			ControlGroupRecall7,
			ControlGroupRecall7_NRS,
			ControlGroupRecall7_GRS,
			ControlGroupRecall8,
			ControlGroupRecall8_NRS,
			ControlGroupRecall8_GRS,
			ControlGroupRecall9,
			ControlGroupRecall9_NRS,
			ControlGroupRecall9_GRS,
			ControlGroupRecall0,
			ControlGroupRecall0_NRS,
			ControlGroupRecall0_GRS,
			ConversationSkipAll,
			ConversationSkipAll_NRS,
			ConversationSkipAll_GRS,
			ConversationSkipOne,
			DialogDismiss,
			DialogDismiss_NRS,
			DialogDismiss_GRS,
			GameSpeedDec,
			GameSpeedInc,
			HeroSelect0,
			HeroSelect0_NRS,
			HeroSelect0_GRS,
			HeroSelect1,
			HeroSelect1_NRS,
			HeroSelect1_GRS,
			HeroSelect2,
			HeroSelect2_NRS,
			HeroSelect2_GRS,
			HeroSelect3,
			HeroSelect3_NRS,
			HeroSelect3_GRS,
			HeroSelect4,
			HeroSelect4_NRS,
			HeroSelect4_GRS,
			HeroSelect5,
			HeroSelect5_NRS,
			HeroSelect5_GRS,
			HeroSelect6,
			HeroSelect6_NRS,
			HeroSelect6_GRS,
			HeroSelect7,
			IdleWorker,
			IdleWorker_NRS,
			IdleWorker_GRS,
			InventoryButtonAlt0,
			InventoryButtonAlt1,
			InventoryButtonAlt2,
			InventoryButtonAlt3,
			InventoryButtonAlt4,
			InventoryButtonAlt5,
			InventoryButtonAlt6,
			InventoryButtonAlt7,
			InventoryButtonUse0,
			InventoryButtonUse1,
			InventoryButtonUse2,
			InventoryButtonUse3,
			InventoryButtonUse4,
			InventoryButtonUse5,
			InventoryButtonUse6,
			InventoryButtonUse7,
			LeaderAPM,
			LeaderArmy,
			LeaderIncome,
			LeaderNone,
			LeaderProduction,
			LeaderResources,
			LeaderSpending,
			LeaderUnits,
			LeaderUnitsLost,
			MenuAchievements,
			MenuAchievements_NRS,
			MenuAchievements_GRS,
			MenuGame,
			MenuGame_NRS,
			MenuGame_GRS,
			MenuMessages,
			MenuMessages_NRS,
			MenuMessages_GRS,
			MenuHelp,
			MenuHelp_NRS,
			MenuHelp_GRS,
			MinimapColors,
			MinimapPing,
			MinimapPing_NRS,
			MinimapTerrain,
			Music,
			ObserveAllPlayers,
			ObserveAutoCamera,
			ObserveClearSelection,
			ObserveClearSelection_NRS,
			ObserveClearSelection_GRS,
			ObserveCommentator,
			ObservePlayer0,
			ObservePlayer0_NRS,
			ObservePlayer0_GRS,
			ObservePlayer1,
			ObservePlayer1_NRS,
			ObservePlayer1_GRS,
			ObservePlayer2,
			ObservePlayer2_NRS,
			ObservePlayer2_GRS,
			ObservePlayer3,
			ObservePlayer3_NRS,
			ObservePlayer3_GRS,
			ObservePlayer4,
			ObservePlayer4_NRS,
			ObservePlayer4_GRS,
			ObservePlayer5,
			ObservePlayer5_NRS,
			ObservePlayer5_GRS,
			ObservePlayer6,
			ObservePlayer6_NRS,
			ObservePlayer6_GRS,
			ObservePlayer7,
			ObservePlayer7_NRS,
			ObservePlayer7_GRS,
			ObservePlayer8,
			ObservePlayer8_NRS,
			ObservePlayer8_GRS,
			ObservePlayer9,
			ObservePlayer9_NRS,
			ObservePlayer9_GRS,
			ObservePlayer10,
			ObservePlayer10_NRS,
			ObservePlayer10_GRS,
			ObservePlayer11,
			ObservePlayer11_NRS,
			ObservePlayer11_GRS,
			ObservePlayer12,
			ObservePlayer12_NRS,
			ObservePlayer12_GRS,
			ObservePlayer13,
			ObservePlayer13_NRS,
			ObservePlayer13_GRS,
			ObservePlayer14,
			ObservePlayer14_NRS,
			ObservePlayer14_GRS,
			ObservePlayer15,
			ObservePlayer15_NRS,
			ObservePlayer15_GRS,
			ObservePreview,
			ObserveSelected,
			ObserveStatusBars,
			PauseGame,
			PTT,
			QuickSave,
			ReplayPlayPause,
			ReplayRestart,
			ReplaySkipBack,
			ReplaySkipNext,
			ReplaySpeedDec,
			ReplaySpeedDec_NRS,
			ReplaySpeedDec_GRS,
			ReplaySpeedInc,
			ReplaySpeedInc_NRS,
			ReplaySpeedInc_GRS,
			ReplayStop,
			ReplayHide,
			Screenshot,
			SelectionCancelDrag,
			SelectionCancelDrag_NRS,
			SelectionCancelDrag_GRS,
			Sound,
			StatusAll,
			StatusOwner,
			StatusOwner_NRS,
			StatusOwner_GRS,
			StatusEnemy,
			StatusEnemy_NRS,
			StatusEnemy_GRS,
			StatusAlly,
			StatusAlly_NRS,
			StatusAlly_GRS,
			SubgroupNext,
			SubgroupNext_NRS,
			SubgroupNext_GRS,
			SubgroupPrev,
			SubgroupPrev_NRS,
			SubgroupPrev_GRS,
			TeamResources,
			TownCamera,
			TownCamera_NRS,
			TownCamera_GRS,
			VideoRecord,
			VideoRecord_NRS,
			VideoRecord_GRS,
			WarpIn,
			WarpIn_NRS,
			WarpIn_GRS,
			WarpIn_GLS,
			WarpIn_SC1,
			Taunt_USD,
			Accept_Hotkey_USD,
			Apply_Hotkey_USD,
			Back_Hotkey_USD,
			BattleNet_Hotkey_USD,
			Campaign_Hotkey_USD,
			CampaignPanelContinue_Hotkey_USD,
			CampaignPanelLoad_Hotkey_USD,
			CampaignPanelNewCampaign_Hotkey_USD,
			CampaignPanelTutorial_Hotkey_USD,
			ScreenReplayBack_Hotkey_USD,
			ScreenReplayLoad_Hotkey_USD,
			ScreenSingleChallenge_Hotkey_USD,
			ScreenSingleCustomGame_Hotkey_USD,
			ChallengePanelBack_Hotkey_USD,
			ChallengePanelPlay_Hotkey_USD,
			CancelMulti_Hotkey_USD,
			Cancel_Hotkey_USD,
			Challenge_Hotkey_USD,
			Chat_Hotkey_USD,
			Clan_Hotkey_USD,
			Close_Hotkey_USD,
			Continue_Hotkey_USD,
			CreateGame_Hotkey_USD,
			Credits_Hotkey_USD,
			CustomGame_Hotkey_USD,
			DebugMap_Hotkey_USD,
			Defaults_Hotkey_USD,
			Delete_Hotkey_USD,
			DropPlayers_Hotkey_USD,
			ExitDialog_No_Hotkey_USD,
			ExitDialog_Yes_Hotkey_USD,
			GameLaunchVersionQueryReplayLoadButton_Hotkey_USD,
			GameLaunchVersionQuerySaveLoadButton_Hotkey_USD,
			GameMenuAbortMission_Hotkey_USD,
			GameMenuAccept_Hotkey_USD,
			GameMenuCancel_Hotkey_USD,
			GameMenuClose_Hotkey_USD,
			GameMenuDefault_Hotkey_USD,
			GameMenuDelete_Hotkey_USD,
			GameMenuExitProgram_Hotkey_USD,
			GameMenuGameplay_Hotkey_USD,
			GameMenuGraphics_Hotkey_USD,
			GameMenuHotkeys_Hotkey_USD,
			GameMenuLoad_Hotkey_USD,
			GameMenuLoadLast_Hotkey_USD,
			GameMenuOptions_Hotkey_USD,
			GameMenuPause_Hotkey_USD,
			GameMenuQuitCampaign_Hotkey_USD,
			GameMenuQuitReplay_Hotkey_USD,
			GameMenuQuit_Hotkey_USD,
			GameMenuReplay_Hotkey_USD,
			GameMenuRestart_Hotkey_USD,
			GameMenuResume_Hotkey_USD,
			GameMenuReturn_Hotkey_USD,
			GameMenuSave_Hotkey_USD,
			GameMenuScoreScreen_Hotkey_USD,
			GameMenuSound_Hotkey_USD,
			GameMenuSurrender_Hotkey_USD,
			GameMenuVideo_Hotkey_USD,
			GameMenuVoice_Hotkey_USD,
			Gameplay_Hotkey_USD,
			Graph_Hotkey_USD,
			Graphics_Hotkey_USD,
			Help_Hotkey_USD,
			Hotkeys_Hotkey_USD,
			JoinGame_Hotkey_USD,
			Ladder_Hotkey_USD,
			Load_Hotkey_USD,
			Login_Hotkey_USD,
			Mail_Hotkey_USD,
			MercenaryPanelClose_Hotkey_USD,
			Multiplayer_Hotkey_USD,
			New_Hotkey_USD,
			News_Hotkey_USD,
			Ok_Hotkey_USD,
			Options_Hotkey_USD,
			Overview_Hotkey_USD,
			PlanetPanelLaunch_Hotkey_USD,
			PlayGame_Hotkey_USD,
			Play_Hotkey_USD,
			Profile_Hotkey_USD,
			Quit_Hotkey_USD,
			Ready_Hotkey_USD,
			Replay_Hotkey_USD,
			Resources_Hotkey_USD,
			SaveReplay_Hotkey_USD,
			Save_Hotkey_USD,
			SendChat_Hotkey_USD,
			Singleplayer_Hotkey_USD,
			Skirmish_Hotkey_USD,
			Social_Hotkey_USD,
			Sound_Hotkey_USD,
			StandardDialog_Cancel_Hotkey_USD,
			StandardDialog_No_Hotkey_USD,
			StandardDialog_Ok_Hotkey_USD,
			StandardDialog_Yes_Hotkey_USD,
			StartGame_Hotkey_USD,
			Stereoscopic_Hotkey_USD,
			Structures_Hotkey_USD,
			Table_Hotkey_USD,
			Units_Hotkey_USD,
			VictoryPanelContinue_Hotkey_USD,
			VictoryPanelPlay_Again_Hotkey_USD,
			Video_Hotkey_USD,
			ViewReplay_Hotkey_USD,
			Voice_Hotkey_USD,
			WaitingForServerSurrender_Hotkey_USD,
			AICommunication_USD,
			AIAssault_USD,
			AICancel_USD,
			AIDefend_USD,
			AIDetect_USD,
			AIExpand_USD,
			AIHarass_USD,
			AIRally_USD,
			AIScout_USD,
			AIMinerals_USD,
			AIVespene_USD,
			AlertRecall_USD,
			CameraCenter_USD,
			CameraFollow_USD,
			CameraMoveDown_USD,
			CameraMoveLeft_USD,
			CameraMoveRight_USD,
			CameraMoveUp_USD,
			CameraSave0_USD,
			CameraSave1_USD,
			CameraSave2_USD,
			CameraSave3_USD,
			CameraSave4_USD,
			CameraSave5_USD,
			CameraSave6_USD,
			CameraSave7_USD,
			CameraTurnLeft_USD,
			CameraTurnRight_USD,
			CameraView0_USD,
			CameraView1_USD,
			CameraView2_USD,
			CameraView3_USD,
			CameraView4_USD,
			CameraView5_USD,
			CameraView6_USD,
			CameraView7_USD,
			CameraZoomFirst_USD,
			CameraZoomLast_USD,
			CameraZoomNext_USD,
			CameraZoomPrev_USD,
			CameraPush_USD,
			ChatAll_USD,
			ChatAllies_USD,
			ChatCancel_USD,
			ChatDefault_USD,
			ChatIndividual_USD,
			ChatRecipient_USD,
			ChatSend_USD,
			CinematicSkip_USD,
			ControlGroupAppend1_USD,
			ControlGroupAppend2_USD,
			ControlGroupAppend3_USD,
			ControlGroupAppend4_USD,
			ControlGroupAppend5_USD,
			ControlGroupAppend6_USD,
			ControlGroupAppend7_USD,
			ControlGroupAppend8_USD,
			ControlGroupAppend9_USD,
			ControlGroupAppend0_USD,
			ControlGroupAssign1_USD,
			ControlGroupAssign2_USD,
			ControlGroupAssign3_USD,
			ControlGroupAssign4_USD,
			ControlGroupAssign5_USD,
			ControlGroupAssign6_USD,
			ControlGroupAssign7_USD,
			ControlGroupAssign8_USD,
			ControlGroupAssign9_USD,
			ControlGroupAssign0_USD,
			ControlGroupRecall1_USD,
			ControlGroupRecall2_USD,
			ControlGroupRecall3_USD,
			ControlGroupRecall4_USD,
			ControlGroupRecall5_USD,
			ControlGroupRecall6_USD,
			ControlGroupRecall7_USD,
			ControlGroupRecall8_USD,
			ControlGroupRecall9_USD,
			ControlGroupRecall0_USD,
			ConversationSkipAll_USD,
			ConversationSkipOne_USD,
			DialogDismiss_USD,
			GameSpeedDec_USD,
			GameSpeedInc_USD,
			HeroSelect0_USD,
			HeroSelect1_USD,
			HeroSelect2_USD,
			HeroSelect3_USD,
			HeroSelect4_USD,
			HeroSelect5_USD,
			HeroSelect6_USD,
			HeroSelect7_USD,
			IdleWorker_USD,
			InventoryButtonAlt0_USD,
			InventoryButtonAlt1_USD,
			InventoryButtonAlt2_USD,
			InventoryButtonAlt3_USD,
			InventoryButtonAlt4_USD,
			InventoryButtonAlt5_USD,
			InventoryButtonAlt6_USD,
			InventoryButtonAlt7_USD,
			InventoryButtonUse0_USD,
			InventoryButtonUse1_USD,
			InventoryButtonUse2_USD,
			InventoryButtonUse3_USD,
			InventoryButtonUse4_USD,
			InventoryButtonUse5_USD,
			InventoryButtonUse6_USD,
			InventoryButtonUse7_USD,
			LeaderAPM_USD,
			LeaderArmy_USD,
			LeaderIncome_USD,
			LeaderNone_USD,
			LeaderProduction_USD,
			LeaderResources_USD,
			LeaderSpending_USD,
			LeaderUnits_USD,
			LeaderUnitsLost_USD,
			MenuAchievements_USD,
			MenuGame_USD,
			MenuMessages_USD,
			MenuHelp_USD,
			MinimapColors_USD,
			MinimapPing_USD,
			MinimapTerrain_USD,
			Music_USD,
			ObserveAllPlayers_USD,
			ObserveAutoCamera_USD,
			ObserveClearSelection_USD,
			ObserveCommentator_USD,
			ObservePlayer0_USD,
			ObservePlayer1_USD,
			ObservePlayer2_USD,
			ObservePlayer3_USD,
			ObservePlayer4_USD,
			ObservePlayer5_USD,
			ObservePlayer6_USD,
			ObservePlayer7_USD,
			ObservePlayer8_USD,
			ObservePlayer9_USD,
			ObservePlayer10_USD,
			ObservePlayer11_USD,
			ObservePlayer12_USD,
			ObservePlayer13_USD,
			ObservePlayer14_USD,
			ObservePlayer15_USD,
			ObservePreview_USD,
			ObserveSelected_USD,
			ObserveStatusBars_USD,
			PauseGame_USD,
			PTT_USD,
			QuickSave_USD,
			ReplayPlayPause_USD,
			ReplayRestart_USD,
			ReplaySkipBack_USD,
			ReplaySkipNext_USD,
			ReplaySpeedDec_USD,
			ReplaySpeedInc_USD,
			ReplayStop_USD,
			ReplayHide_USD,
			Screenshot_USD,
			SelectionCancelDrag_USD,
			Sound_USD,
			StatusAll_USD,
			StatusOwner_USD,
			StatusEnemy_USD,
			StatusAlly_USD,
			SubgroupNext_USD,
			SubgroupPrev_USD,
			TeamResources_USD,
			TownCamera_USD,
			VideoRecord_USD,
			WarpIn_USD,
			Taunt_USDL,
			Accept_Hotkey_USDL,
			Apply_Hotkey_USDL,
			Back_Hotkey_USDL,
			BattleNet_Hotkey_USDL,
			Campaign_Hotkey_USDL,
			CampaignPanelContinue_Hotkey_USDL,
			CampaignPanelLoad_Hotkey_USDL,
			CampaignPanelNewCampaign_Hotkey_USDL,
			CampaignPanelTutorial_Hotkey_USDL,
			ScreenReplayBack_Hotkey_USDL,
			ScreenReplayLoad_Hotkey_USDL,
			ScreenSingleChallenge_Hotkey_USDL,
			ScreenSingleCustomGame_Hotkey_USDL,
			ChallengePanelBack_Hotkey_USDL,
			ChallengePanelPlay_Hotkey_USDL,
			CancelMulti_Hotkey_USDL,
			Cancel_Hotkey_USDL,
			Challenge_Hotkey_USDL,
			Chat_Hotkey_USDL,
			Clan_Hotkey_USDL,
			Close_Hotkey_USDL,
			Continue_Hotkey_USDL,
			CreateGame_Hotkey_USDL,
			Credits_Hotkey_USDL,
			CustomGame_Hotkey_USDL,
			DebugMap_Hotkey_USDL,
			Defaults_Hotkey_USDL,
			Delete_Hotkey_USDL,
			DropPlayers_Hotkey_USDL,
			ExitDialog_No_Hotkey_USDL,
			ExitDialog_Yes_Hotkey_USDL,
			GameLaunchVersionQueryReplayLoadButton_Hotkey_USDL,
			GameLaunchVersionQuerySaveLoadButton_Hotkey_USDL,
			GameMenuAbortMission_Hotkey_USDL,
			GameMenuAccept_Hotkey_USDL,
			GameMenuCancel_Hotkey_USDL,
			GameMenuClose_Hotkey_USDL,
			GameMenuDefault_Hotkey_USDL,
			GameMenuDelete_Hotkey_USDL,
			GameMenuExitProgram_Hotkey_USDL,
			GameMenuGameplay_Hotkey_USDL,
			GameMenuGraphics_Hotkey_USDL,
			GameMenuHotkeys_Hotkey_USDL,
			GameMenuLoad_Hotkey_USDL,
			GameMenuLoadLast_Hotkey_USDL,
			GameMenuOptions_Hotkey_USDL,
			GameMenuPause_Hotkey_USDL,
			GameMenuQuitCampaign_Hotkey_USDL,
			GameMenuQuitReplay_Hotkey_USDL,
			GameMenuQuit_Hotkey_USDL,
			GameMenuReplay_Hotkey_USDL,
			GameMenuRestart_Hotkey_USDL,
			GameMenuResume_Hotkey_USDL,
			GameMenuReturn_Hotkey_USDL,
			GameMenuSave_Hotkey_USDL,
			GameMenuScoreScreen_Hotkey_USDL,
			GameMenuSound_Hotkey_USDL,
			GameMenuSurrender_Hotkey_USDL,
			GameMenuVideo_Hotkey_USDL,
			GameMenuVoice_Hotkey_USDL,
			Gameplay_Hotkey_USDL,
			Graph_Hotkey_USDL,
			Graphics_Hotkey_USDL,
			Help_Hotkey_USDL,
			Hotkeys_Hotkey_USDL,
			JoinGame_Hotkey_USDL,
			Ladder_Hotkey_USDL,
			Load_Hotkey_USDL,
			Login_Hotkey_USDL,
			Mail_Hotkey_USDL,
			MercenaryPanelClose_Hotkey_USDL,
			Multiplayer_Hotkey_USDL,
			New_Hotkey_USDL,
			News_Hotkey_USDL,
			Ok_Hotkey_USDL,
			Options_Hotkey_USDL,
			Overview_Hotkey_USDL,
			PlanetPanelLaunch_Hotkey_USDL,
			PlayGame_Hotkey_USDL,
			Play_Hotkey_USDL,
			Profile_Hotkey_USDL,
			Quit_Hotkey_USDL,
			Ready_Hotkey_USDL,
			Replay_Hotkey_USDL,
			Resources_Hotkey_USDL,
			SaveReplay_Hotkey_USDL,
			Save_Hotkey_USDL,
			SendChat_Hotkey_USDL,
			Singleplayer_Hotkey_USDL,
			Skirmish_Hotkey_USDL,
			Social_Hotkey_USDL,
			Sound_Hotkey_USDL,
			StandardDialog_Cancel_Hotkey_USDL,
			StandardDialog_No_Hotkey_USDL,
			StandardDialog_Ok_Hotkey_USDL,
			StandardDialog_Yes_Hotkey_USDL,
			StartGame_Hotkey_USDL,
			Stereoscopic_Hotkey_USDL,
			Structures_Hotkey_USDL,
			Table_Hotkey_USDL,
			Units_Hotkey_USDL,
			VictoryPanelContinue_Hotkey_USDL,
			VictoryPanelPlay_Again_Hotkey_USDL,
			Video_Hotkey_USDL,
			ViewReplay_Hotkey_USDL,
			Voice_Hotkey_USDL,
			WaitingForServerSurrender_Hotkey_USDL,
			AICommunication_USDL,
			AIAssault_USDL,
			AICancel_USDL,
			AIDefend_USDL,
			AIDetect_USDL,
			AIExpand_USDL,
			AIHarass_USDL,
			AIRally_USDL,
			AIScout_USDL,
			AIMinerals_USDL,
			AIVespene_USDL,
			AlertRecall_USDL,
			CameraCenter_USDL,
			CameraFollow_USDL,
			CameraMoveDown_USDL,
			CameraMoveLeft_USDL,
			CameraMoveRight_USDL,
			CameraMoveUp_USDL,
			CameraSave0_USDL,
			CameraSave1_USDL,
			CameraSave2_USDL,
			CameraSave3_USDL,
			CameraSave4_USDL,
			CameraSave5_USDL,
			CameraSave6_USDL,
			CameraSave7_USDL,
			CameraTurnLeft_USDL,
			CameraTurnRight_USDL,
			CameraView0_USDL,
			CameraView1_USDL,
			CameraView2_USDL,
			CameraView3_USDL,
			CameraView4_USDL,
			CameraView5_USDL,
			CameraView6_USDL,
			CameraView7_USDL,
			CameraZoomFirst_USDL,
			CameraZoomLast_USDL,
			CameraZoomNext_USDL,
			CameraZoomPrev_USDL,
			CameraPush_USDL,
			ChatAll_USDL,
			ChatAllies_USDL,
			ChatCancel_USDL,
			ChatDefault_USDL,
			ChatIndividual_USDL,
			ChatRecipient_USDL,
			ChatSend_USDL,
			CinematicSkip_USDL,
			ControlGroupAppend1_USDL,
			ControlGroupAppend2_USDL,
			ControlGroupAppend3_USDL,
			ControlGroupAppend4_USDL,
			ControlGroupAppend5_USDL,
			ControlGroupAppend6_USDL,
			ControlGroupAppend7_USDL,
			ControlGroupAppend8_USDL,
			ControlGroupAppend9_USDL,
			ControlGroupAppend0_USDL,
			ControlGroupAssign1_USDL,
			ControlGroupAssign2_USDL,
			ControlGroupAssign3_USDL,
			ControlGroupAssign4_USDL,
			ControlGroupAssign5_USDL,
			ControlGroupAssign6_USDL,
			ControlGroupAssign7_USDL,
			ControlGroupAssign8_USDL,
			ControlGroupAssign9_USDL,
			ControlGroupAssign0_USDL,
			ControlGroupRecall1_USDL,
			ControlGroupRecall2_USDL,
			ControlGroupRecall3_USDL,
			ControlGroupRecall4_USDL,
			ControlGroupRecall5_USDL,
			ControlGroupRecall6_USDL,
			ControlGroupRecall7_USDL,
			ControlGroupRecall8_USDL,
			ControlGroupRecall9_USDL,
			ControlGroupRecall0_USDL,
			ConversationSkipAll_USDL,
			ConversationSkipOne_USDL,
			DialogDismiss_USDL,
			GameSpeedDec_USDL,
			GameSpeedInc_USDL,
			HeroSelect0_USDL,
			HeroSelect1_USDL,
			HeroSelect2_USDL,
			HeroSelect3_USDL,
			HeroSelect4_USDL,
			HeroSelect5_USDL,
			HeroSelect6_USDL,
			HeroSelect7_USDL,
			IdleWorker_USDL,
			InventoryButtonAlt0_USDL,
			InventoryButtonAlt1_USDL,
			InventoryButtonAlt2_USDL,
			InventoryButtonAlt3_USDL,
			InventoryButtonAlt4_USDL,
			InventoryButtonAlt5_USDL,
			InventoryButtonAlt6_USDL,
			InventoryButtonAlt7_USDL,
			InventoryButtonUse0_USDL,
			InventoryButtonUse1_USDL,
			InventoryButtonUse2_USDL,
			InventoryButtonUse3_USDL,
			InventoryButtonUse4_USDL,
			InventoryButtonUse5_USDL,
			InventoryButtonUse6_USDL,
			InventoryButtonUse7_USDL,
			LeaderAPM_USDL,
			LeaderArmy_USDL,
			LeaderIncome_USDL,
			LeaderNone_USDL,
			LeaderProduction_USDL,
			LeaderResources_USDL,
			LeaderSpending_USDL,
			LeaderUnits_USDL,
			LeaderUnitsLost_USDL,
			MenuAchievements_USDL,
			MenuGame_USDL,
			MenuMessages_USDL,
			MenuHelp_USDL,
			MinimapColors_USDL,
			MinimapPing_USDL,
			MinimapTerrain_USDL,
			Music_USDL,
			ObserveAllPlayers_USDL,
			ObserveAutoCamera_USDL,
			ObserveClearSelection_USDL,
			ObserveCommentator_USDL,
			ObservePlayer0_USDL,
			ObservePlayer1_USDL,
			ObservePlayer2_USDL,
			ObservePlayer3_USDL,
			ObservePlayer4_USDL,
			ObservePlayer5_USDL,
			ObservePlayer6_USDL,
			ObservePlayer7_USDL,
			ObservePlayer8_USDL,
			ObservePlayer9_USDL,
			ObservePlayer10_USDL,
			ObservePlayer11_USDL,
			ObservePlayer12_USDL,
			ObservePlayer13_USDL,
			ObservePlayer14_USDL,
			ObservePlayer15_USDL,
			ObservePreview_USDL,
			ObserveSelected_USDL,
			ObserveStatusBars_USDL,
			PauseGame_USDL,
			PTT_USDL,
			QuickSave_USDL,
			ReplayPlayPause_USDL,
			ReplayRestart_USDL,
			ReplaySkipBack_USDL,
			ReplaySkipNext_USDL,
			ReplaySpeedDec_USDL,
			ReplaySpeedInc_USDL,
			ReplayStop_USDL,
			ReplayHide_USDL,
			Screenshot_USDL,
			SelectionCancelDrag_USDL,
			Sound_USDL,
			StatusAll_USDL,
			StatusOwner_USDL,
			StatusEnemy_USDL,
			StatusAlly_USDL,
			SubgroupNext_USDL,
			SubgroupPrev_USDL,
			TeamResources_USDL,
			TownCamera_USDL,
			VideoRecord_USDL,
			WarpIn_USDL
		}
	}
}

