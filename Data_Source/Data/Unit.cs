namespace Data
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Threading;

	[StructLayout(LayoutKind.Sequential)]
	public class Unit
	{
		private static object _lock = new object();
		private static Dictionary<ushort, Unit> _Units;
		public static Dictionary<ushort, Unit> Units
		{
			get
			{
				if(_Units == null)
					_Units = new Dictionary<ushort, Unit>();
				return _Units;
			}
		}

		public static void UpdateUnits()
		{
			if (Monitor.TryEnter(Units))
			{
				try
				{
					ushort[] keys = new ushort[Units.Keys.Count];
					Units.Keys.CopyTo(keys, 0);

					foreach (ushort key in keys)
						if (Units[key].Outdated || !Units[key].isAlive)
							Units.Remove(key);

					int max = GameData.offsets.GetArrayCount("Units");
					for (ushort i = 0; i < max - 1; i++) //it seems that unit 16383 is sometimes messed up, and no map should ever have that many units anyway.
					{
						if (Units.ContainsKey(i))
							continue;

						Unit Current = new Unit(i);
						if (Current.modelPtr == 0 || !Current.isAlive)
							continue;

						Units.Add(i, Current);
					}
				}
				finally
				{
					Monitor.Exit(Units);
				}
			}
			else
			{ }
		}

		public float healthRegenDelay;
		public float healthRegenRate;
		public float currentHealth;
		public float maxHealth;

		public float shieldRegenDelay;
		public float shieldRegenRate;
		public float currentShield;
		public float maxShield;

		public float energyRegenDelay;
		public float energyRegenRate;
		public float energyDamage;
		public float maxEnergy;

		public string status;
		public List<QueueItem> currentQueueItem;
		public int priorityKill;
		public int trueCostOfProduction;

		private uint _ID;
		private string _name;
		private string _textID;
		private uint _memoryLocation;
		private uint _modelPtr;
		private ushort _Index;
		private ushort _unitType;

		public Unit(int index)
		{
			_Index = (ushort)index;

			_name = string.Empty;
			_textID = string.Empty;
			_unitType = 0;
			_modelPtr = 0;
			_memoryLocation = (uint)GameData.offsets.GetArrayElementAddress("Units", index);
			_ID = (uint)((ushort)GameData.offsets.ReadArrayElementMember("Units", Index, "times_used") + ((ushort)GameData.offsets.ReadArrayElementMember("Units", Index, "token") << 16));

			if (modelPtr != 0)
			{
				string NameAsText = null;
				string UINameAsText = null;
				if ((uint)GameData.offsets.ReadStructMember("UnitModel", "pName_address", (int)(modelPtr << 5)) != 0)
				{
					uint NameDataAddress = (uint)GameData.mem.ReadMemory((uint)GameData.offsets.ReadStructMember("UnitModel", "pName_address", (int)(modelPtr << 5)), typeof(uint));
					uint NameLength = (uint)GameData.mem.ReadMemory(NameDataAddress, typeof(uint));
					if (NameDataAddress != 0 && NameLength > 10)
					{
						byte[] NameAsBytes = new byte[NameLength];
						GameData.mem.ReadMemory((IntPtr)NameDataAddress + 0x24, (int)NameLength, out NameAsBytes);
						NameAsText = System.Text.Encoding.UTF8.GetString(NameAsBytes).Remove(0, 10);

						uint pUINameAddress = (uint)GameData.mem.ReadMemory(NameDataAddress + 0x1c, typeof(uint));
						if (pUINameAddress != 0)
						{
							uint UINameLength = (uint)GameData.mem.ReadMemory(pUINameAddress + 0x8, typeof(uint));
							uint UINameAddress = pUINameAddress + 0x10;

							byte fail = 0;
							if (((fail = (byte)GameData.mem.ReadMemory(pUINameAddress + 12, typeof(byte))) & 4) != 0) //sometimes the string is right in this struct, other times it's a pointer.
								UINameAddress = (uint)GameData.mem.ReadMemory(pUINameAddress + 16, typeof(uint));

							if (UINameAddress != 0 && UINameLength > 0)
							{
								byte[] UINameAsBytes = new byte[UINameLength];
								GameData.mem.ReadMemory((IntPtr)UINameAddress, (int)UINameLength, out UINameAsBytes);
								UINameAsText = System.Text.Encoding.UTF8.GetString(UINameAsBytes);
							}
						}

					}
				}
				if (NameAsText == null)
					NameAsText = string.Empty;
				if (UINameAsText == null)
					UINameAsText = string.Empty;

				_textID = NameAsText;
				_name = UINameAsText;
			}
		}

		public bool Outdated
		{
			get
			{
				return timesUsed != (ushort)GameData.offsets.ReadArrayElementMember("Units", Index, "times_used");
			}
		}
		public Unit PreviousUnit
		{
			get
			{
				ushort PrevIndex = (ushort)GameData.offsets.ReadArrayElementMember("Units", Index, "prev_unit");
				if (PrevIndex != 0xffff)
				{
					if(Units.ContainsKey(PrevIndex))
						return Units[PrevIndex];
					else
						return new Unit(PrevIndex);
				}
				else
					return null;
			}
		}
		public Unit NextUnit
		{
			get
			{
				ushort NextIndex = (ushort)GameData.offsets.ReadArrayElementMember("Units", Index, "next_unit");
				if (NextIndex != 0xffff)
				{
					if (Units.ContainsKey(NextIndex))
						return Units[NextIndex];
					else
						return new Unit(NextIndex);
				}
				else
					return null;
			}
		}

		public ushort PreviousIndex
		{
			get
			{
				return (ushort)GameData.offsets.ReadArrayElementMember("Units", Index, "prev_unit");
			}
		}
		public ushort NextIndex
		{
			get
			{
				return (ushort)GameData.offsets.ReadArrayElementMember("Units", Index, "next_unit");
			}
		}

		public ushort Index
		{
			get
			{
				return _Index;
			}
		}
		public uint modelPtr
		{
			get
			{
				if(_modelPtr == 0)
					_modelPtr = (uint)GameData.offsets.ReadArrayElementMember("Units", Index, "unit_model");
				return _modelPtr;
			}
		}
		public string name
		{
			get
			{
				if (_name == string.Empty && modelPtr != 0)
				{
					string UINameAsText = null;
					uint NameDataAddress = (uint)GameData.mem.ReadMemory((uint)GameData.offsets.ReadStructMember("UnitModel", "pName_address", (int)(modelPtr << 5)), typeof(uint));
					if (NameDataAddress != 0)
					{	
						uint pUINameAddress = (uint)GameData.mem.ReadMemory(NameDataAddress + 0x1c, typeof(uint));
						if (pUINameAddress != 0)
						{
							uint UINameLength = (uint)GameData.mem.ReadMemory(pUINameAddress + 0x8, typeof(uint));
							uint UINameAddress = pUINameAddress + 0x10;

							if (((byte)GameData.mem.ReadMemory(pUINameAddress + 12, typeof(byte)) & 4) != 0) //sometimes the string is right in this struct, other times it's a pointer.
								UINameAddress = (uint)GameData.mem.ReadMemory(pUINameAddress + 16, typeof(uint));

							if (UINameAddress != 0 && UINameLength > 0)
							{
								byte[] UINameAsBytes = new byte[UINameLength];
								GameData.mem.ReadMemory((IntPtr)UINameAddress, (int)UINameLength, out UINameAsBytes);
								UINameAsText = System.Text.Encoding.UTF8.GetString(UINameAsBytes);
							}
						}
					}
					if (UINameAsText == null)
						UINameAsText = string.Empty;

					_name = UINameAsText;
				}
				return _name;
			}
		}
		public string textID
		{
			get
			{
				if (_textID == string.Empty && modelPtr != 0)
				{
					string NameAsText = null;
					uint NameDataAddress = (uint)GameData.mem.ReadMemory((uint)GameData.offsets.ReadStructMember("UnitModel", "pName_address", (int)(modelPtr << 5)), typeof(uint));
					if (NameDataAddress != 0)
					{
						uint NameLength = (uint)GameData.mem.ReadMemory(NameDataAddress, typeof(uint));
						if (NameLength > 10)
						{
							byte[] NameAsBytes = new byte[NameLength];
							GameData.mem.ReadMemory((IntPtr)NameDataAddress + 0x24, (int)NameLength, out NameAsBytes);
							NameAsText = System.Text.Encoding.UTF8.GetString(NameAsBytes).Remove(0, 10);
						}
					}
					if (NameAsText == null)
						NameAsText = string.Empty;

					_textID = NameAsText;
				}
				return _textID;
			}
		}
		public float minimapRadius
		{
			get
			{
				return (fixed32)GameData.offsets.ReadStructMember("UnitModel", "minimap_radius", (int)(modelPtr << 5));
			}
		}
		public float timeScale
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "time_scale");
			}
			set
			{
				GameData.offsets.WriteArrayElementMember("Units", Index, "time_scale", (fixed32)value);
			}
		}
		public uint ID
		{
			get
			{
				return _ID;
			}
		}
		public ushort timesUsed
		{
			get
			{
				return (ushort)(ID & 0x0000FFFF);
			}
		}
		public ushort token
		{
			get
			{
				return (ushort)(ID >> 16);
			}
		}
		public uint playerNumber
		{
			get
			{
				return (byte)GameData.offsets.ReadArrayElementMember("Units", Index, "player_owner");
			}
		}
		public ushort unitType
		{
			get
			{
				if(_unitType == 0)
					_unitType = (ushort)GameData.offsets.ReadStructMember("UnitModel", "unit_type", (int)(modelPtr << 5));
				return _unitType;
			}
		}
		public UnitStateOld state
		{
			get
			{
				return (UnitStateOld)((byte)GameData.offsets.ReadArrayElementMember("Units", Index, "state"));
			}
		}

		public bool isImmobile
		{
			get
			{
				return (byte)GameData.offsets.ReadArrayElementMember("Units", Index, "isImmobile") != 0;
			}
		}
		public TargetFilter targetFilterFlags
		{
			get
			{
				return (TargetFilter)GameData.offsets.ReadArrayElementMember("Units", Index, "targetFilter_flags");
			}
		}
		public bool isAlive
		{
			get
			{
				return (targetFilterFlags & TargetFilter.Dead) == 0;
			}
		}
		public bool cloaked
		{
			get
			{
				return (targetFilterFlags & TargetFilter.Cloaked) != 0;
			}
		}
		public bool detector
		{
			get
			{
				return (targetFilterFlags & TargetFilter.Detector) != 0;
			}
		}

		public UnitMoveState moveState
		{
			get
			{
				return (UnitMoveState)((byte)GameData.offsets.ReadArrayElementMember("Units", Index, "move_state"));
			}
		}
		public UnitSubMoveState subMoveState
		{
			get
			{
				return (UnitSubMoveState)((byte)GameData.offsets.ReadArrayElementMember("Units", Index, "sub_move_state"));
			}
		}
		public UnitLastOrder lastOrder
		{
			get
			{
				return (UnitLastOrder)((uint)GameData.offsets.ReadArrayElementMember("Units", Index, "last_order"));
			}
		}
		public DeathType deathType
		{
			get
			{
				return (DeathType)((uint)GameData.offsets.ReadArrayElementMember("Units", Index, "death_type"));
			}
		}

		public float currentEnergy
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "energy");
			}
		}
		public float shieldDamage
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "shield_damage");
			}
		}
		public float healthDamage
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "health_damage");
			}
		}
		
		public float locationX
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "position_x");
			}
		}
		public float locationY
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "position_y");
			}
		}
		public float locationZ
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "position_z");
			}
		}

		public float destinationX
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "destination_x");
			}
		}
		public float destinationY
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "destination_y");
			}
		}
		public float destinationZ
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "destination_z");
			}
		}

		public float destination2X
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "destination2_x");
			}
		}
		public float destination2Y
		{
			get
			{
				return (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "destination2_y");
			}
		}

		public int kills
		{
			get
			{
				return (ushort)GameData.offsets.ReadArrayElementMember("Units", Index, "kills");
			}
		}

		public float rotation
		{
			get
			{
				return 180 - (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "rotation") * 45;
			}
		}
		public float rotationX
		{
			get
			{
				return 180 - (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "rotation_x") * 45;
			}
		}
		public float rotationY
		{
			get
			{
				return 180 - (fixed32)GameData.offsets.ReadArrayElementMember("Units", Index, "rotation_y") * 45;
			}
		}

		public int moveSpeed
		{
			get
			{
				return (int)((uint)GameData.offsets.ReadArrayElementMember("Units", Index, "move_speed"));
			}
		}
		public uint memoryLocation
		{
			get
			{
				if(_memoryLocation == 0)
					_memoryLocation = (uint)GameData.offsets.GetArrayElementAddress("Units", Index);
				return _memoryLocation;
			}
		}
		public uint commandQueuePointer
		{
			get
			{
				return (uint)GameData.offsets.ReadArrayElementMember("Units", Index, "commandQueue_pointer");
			}
		}

		public override string ToString()
		{
			string ReturnValue = base.ToString();
			if (textID != null && textID != string.Empty)
				ReturnValue = textID;
			if ((targetFilterFlags & TargetFilter.Dead) != 0)
				ReturnValue += " (Dead)";
			else
			{
				if((targetFilterFlags & TargetFilter.Cloaked) != 0)
					ReturnValue += " (Cloaked)";
				if ((targetFilterFlags & TargetFilter.Detector) != 0)
					ReturnValue += " (Detector)";
			}

			return ReturnValue;
		}
	}
}

