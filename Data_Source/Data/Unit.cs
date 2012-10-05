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

		public static void ResetUnits()
		{
			Locks.Units.EnterWriteLock();
			_Units = new Dictionary<ushort, Unit>();
			Locks.Units.ExitWriteLock();
		}

		public static void UpdateUnits()
		{
			if(Locks.Units.TryEnterWriteLock(250))
			{
				int max = GameData.offsets.GetArrayCount(ORNames.Units);
				int numInvalid = 0;
				for (ushort i = 0; i < max; i++) //it seems that unit 16383 is sometimes messed up, but no map should ever have that many units anyway.
				{
					if (Units.ContainsKey(i))
					{
						if (Units[i].Outdated)
							Units.Remove(i);
						else
						{
							Units[i].Update();
							if (!Units[i].isAlive)
								Units.Remove(i);
							continue;
						}
					}

					Unit Current = new Unit(i);
					if (Current.modelPtr == 0)
					{
						if (++numInvalid >= 50 && numInvalid >= i / 10)
							break;
						continue;
					}

					if (!Current.isAlive)
						continue;

					Units.Add(i, Current);
				}
				Locks.Units.ExitWriteLock();
			}
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
		private fixed32 _minimapRadius;
		private ushort _Index;
		private ushort _unitType;
		private byte[] _Data;
		private ReaderWriterLockSlim _DataLock;

		public void Update()
		{
			if(_DataLock.TryEnterWriteLock(50))
			{
				_Data = GameData.offsets.ReadArrayElement(ORNames.Units, Index);
				_DataLock.ExitWriteLock();
			}
		}

		public Unit(int index)
		{
			_Index = (ushort)index;

			_name = string.Empty;
			_textID = string.Empty;
			_unitType = 0;
			_minimapRadius = -1;

			_DataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
			_DataLock.EnterWriteLock();

			_memoryLocation = 0;

			_Data = new byte[1];
			Update();

			_ID = (uint)((ushort)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.times_used, _Data) + ((ushort)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.token, _Data) << 16));
			_DataLock.ExitWriteLock();
		}

		public bool Outdated
		{
			get
			{
				ushort newTU = (ushort)GameData.offsets.ReadArrayElementMember(ORNames.Units, Index, ORNames.times_used);
				return timesUsed != newTU;
			}
		}
		public Unit PreviousUnit
		{
			get
			{
				_DataLock.EnterReadLock();
				ushort PrevIndex = (ushort)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.prev_unit, _Data);
				_DataLock.ExitReadLock();
				if (PrevIndex != 0xffff)
				{
					Unit ReturnVal;
					Locks.Units.EnterReadLock();
					if (Units.ContainsKey(PrevIndex))
						ReturnVal = Units[PrevIndex];
					else
						ReturnVal = new Unit(PrevIndex);
					Locks.Units.ExitReadLock();
					return ReturnVal;
				}
				else
					return null;
			}
		}
		public Unit NextUnit
		{
			get
			{
				_DataLock.EnterReadLock();
				ushort NextIndex = (ushort)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.next_unit, _Data);
				_DataLock.ExitReadLock();
				if (NextIndex != 0xffff)
				{
					if (!Locks.Units.IsWriteLockHeld && !Locks.Units.IsUpgradeableReadLockHeld)
					{
						Unit ReturnVal;
						Locks.Units.EnterReadLock();
						if (Units.ContainsKey(NextIndex))
							ReturnVal = Units[NextIndex];
						else
							ReturnVal = new Unit(NextIndex);
						Locks.Units.ExitReadLock();
						return ReturnVal;
					}
					else
					{
						if (Units.ContainsKey(NextIndex))
							return Units[NextIndex];
						else
							return new Unit(NextIndex);
					}
				}
				else
					return null;
			}
		}

		public ushort PreviousIndex
		{
			get
			{
				_DataLock.EnterReadLock();
				ushort PrevIndex = (ushort)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.prev_unit, _Data);
				_DataLock.ExitReadLock();
				return PrevIndex;
			}
		}
		public ushort NextIndex
		{
			get
			{
				_DataLock.EnterReadLock();
				ushort NextIndex = (ushort)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.next_unit, _Data);
				_DataLock.ExitReadLock();
				return NextIndex;
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
				_DataLock.EnterReadLock();
				uint ReturnVal = (uint)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.unit_model, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public string name
		{
			get
			{
				if (_name == string.Empty && modelPtr != 0)
				{
					string UINameAsText = null;
					uint NameDataAddress = (uint)GameData.mem.ReadMemory((uint)GameData.offsets.ReadStructMember(ORNames.UnitModel, ORNames.pName_address, (int)(modelPtr << 5)), typeof(uint));
					if (NameDataAddress != 0 && NameDataAddress < uint.MaxValue) //I'm not sure if checking against MaxValue is necessary, but there have been overflows near here.
					{	
						uint pUINameAddress = (uint)GameData.mem.ReadMemory(NameDataAddress + 0x1c, typeof(uint));
						if (pUINameAddress != 0 && pUINameAddress < uint.MaxValue) //I'm not sure if checking against MaxValue is necessary, but there have been overflows near here.
						{
							uint UINameLength = (uint)GameData.mem.ReadMemory(pUINameAddress + 0x8, typeof(uint));
							uint UINameAddress = pUINameAddress + 0x10;

							if (((byte)GameData.mem.ReadMemory(pUINameAddress + 12, typeof(byte)) & 4) != 0) //sometimes the string is right in this struct, other times it's a pointer.
								UINameAddress = (uint)GameData.mem.ReadMemory(pUINameAddress + 16, typeof(uint));

							if (UINameAddress != 0 && UINameLength > 0 && UINameLength < 1024) //it needs an upper limit incase the length is garbage.
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
					uint NameDataAddress = (uint)GameData.mem.ReadMemory((uint)GameData.offsets.ReadStructMember(ORNames.UnitModel, ORNames.pName_address, (int)(modelPtr << 5)), typeof(uint));
					if (NameDataAddress != 0 && NameDataAddress < uint.MaxValue) //I'm not sure if checking against MaxValue is necessary, but there have been overflows near here.
					{
						uint NameLength = (uint)GameData.mem.ReadMemory(NameDataAddress, typeof(uint));
						if (NameLength > 10 && NameLength < 1024) //it needs an upper limit incase the length is garbage.
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
		public fixed32 minimapRadius
		{
			get
			{
				if (_minimapRadius < 0)
				{
					_minimapRadius = (fixed32)GameData.offsets.ReadStructMember(ORNames.UnitModel, ORNames.minimap_radius, (int)(modelPtr << 5));
				}
				return _minimapRadius;
			}
		}
		public float timeScale
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.time_scale, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
			set
			{
				GameData.offsets.WriteArrayElementMember(ORNames.Units, Index, ORNames.time_scale, (fixed32)value);
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
				_DataLock.EnterReadLock();
				byte ReturnVal = (byte)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.player_owner, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public ushort unitType
		{
			get
			{
				if(_unitType == 0)
					_unitType = (ushort)GameData.offsets.ReadStructMember(ORNames.UnitModel, ORNames.unit_type, (int)(modelPtr << 5));
				return _unitType;
			}
		}
		public UnitStateOld state
		{
			get
			{
				_DataLock.EnterReadLock();
				UnitStateOld ReturnVal = (UnitStateOld)((byte)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.state, _Data));
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}

		public bool isImmobile
		{
			get
			{
				_DataLock.EnterReadLock();
				byte ReturnVal = (byte)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.isImmobile, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal != 0;
			}
		}
		public TargetFilter targetFilterFlags
		{
			get
			{
				_DataLock.EnterReadLock();
				TargetFilter ReturnVal = (TargetFilter)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.targetFilter_flags, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
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
				_DataLock.EnterReadLock();
				UnitMoveState ReturnVal = (UnitMoveState)((byte)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.move_state, _Data));
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public UnitSubMoveState subMoveState
		{
			get
			{
				_DataLock.EnterReadLock();
				UnitSubMoveState ReturnVal = (UnitSubMoveState)((byte)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.sub_move_state, _Data));
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public UnitLastOrder lastOrder
		{
			get
			{
				_DataLock.EnterReadLock();
				UnitLastOrder ReturnVal = (UnitLastOrder)((uint)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.last_order, _Data));
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public DeathType deathType
		{
			get
			{
				_DataLock.EnterReadLock();
				DeathType ReturnVal = (DeathType)((uint)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.death_type, _Data));
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}

		public float currentEnergy
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.energy, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public float shieldDamage
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.shield_damage, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public float healthDamage
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.health_damage, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		
		public float locationX
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.position_x, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public float locationY
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.position_y, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public float locationZ
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.position_z, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}

		public float destinationX
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.destination_x, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public float destinationY
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.destination_y, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public float destinationZ
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.destination_z, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}

		public float destination2X
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.destination2_x, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public float destination2Y
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.destination2_y, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}

		public int kills
		{
			get
			{
				_DataLock.EnterReadLock();
				ushort ReturnVal = (ushort)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.kills, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}

		public float rotation
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.rotation, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public float rotationX
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.rotation_x, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public float rotationY
		{
			get
			{
				_DataLock.EnterReadLock();
				fixed32 ReturnVal = (fixed32)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.rotation_y, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}

		public int moveSpeed
		{
			get
			{
				_DataLock.EnterReadLock();
				int ReturnVal = (int)((uint)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.move_speed, _Data));
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}
		public uint memoryLocation
		{
			get
			{
				if(_memoryLocation == 0)
					_memoryLocation = (uint)GameData.offsets.GetArrayElementAddress(ORNames.Units, Index);
				return _memoryLocation;
			}
		}
		public uint commandQueuePointer
		{
			get
			{
				_DataLock.EnterReadLock();
				uint ReturnVal = (uint)GameData.offsets.ReadStructMember(ORNames.Unit, ORNames.commandQueue_pointer, _Data);
				_DataLock.ExitReadLock();
				return ReturnVal;
			}
		}

		public override string ToString()
		{
			string ReturnValue = base.ToString();
			TargetFilter tf = targetFilterFlags;
			if (textID != null && textID != string.Empty)
				ReturnValue = textID;
			if ((tf & TargetFilter.Dead) != 0)
				ReturnValue += " (Dead)";
			else
			{
				if((tf & TargetFilter.Cloaked) != 0)
					ReturnValue += " (Cloaked)";
				if ((tf & TargetFilter.Detector) != 0)
					ReturnValue += " (Detector)";
			}

			return ReturnValue;
		}
	}
}

