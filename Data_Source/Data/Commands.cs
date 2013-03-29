namespace Data
{
	using System;
	using System.Collections.Generic;

	[Flags]
	public enum TargetFlags : uint
	{
		OverrideUnitPositon = 0x1,
		Unknown02 = 0x2,
		Unknown04 = 0x4,
		TargetIsPoint = 0x8,
		TargetIsUnit = 0x10,
		UseUnitPosition = 0x20
	}

	[Flags]
	public enum OrderFlags : uint
	{
		Alternate = 0x1,
		Queued = 0x2, //added to end of queue.
		Preempt = 0x4, //inserted at the start of queue.
		SmartClick = 0x8,
		SmartRally = 0x10,
		Subgroup = 0x20,
		SetAutoCast = 0x40,
		SetAutoCastOn = 0x80,
		Required = 0x100,
		Unknown200 = 0x200, //v these two are checked if target is neither unit nor point.
		Unknown400 = 0x400, //^ other one.
		Minimap = 0x400000 //this may be wrong.
	}

	public class QueuedCommand
	{
		uint _Address;
		byte[] _Data;

		public uint NextCommandPtr
		{ get { return (uint)GameData.offsets.ReadStructMember(ORNames.QueuedCommand, ORNames.pNextCommand, _Data); } }

		public QueuedCommand(uint Address)
		{
			_Address = Address;
			_Data = null;
			Update();
		}
		public void Update()
		{
			_Data = GameData.offsets.ReadStruct(ORNames.QueuedCommand, _Address);
		}
		public TargetFlags TargetFlags
		{ get { return (TargetFlags)(uint)GameData.offsets.ReadStructMember(ORNames.QueuedCommand, ORNames.TargetFlags, _Data); } }
		public OrderFlags Flags
		{ get { return (OrderFlags)(uint)GameData.offsets.ReadStructMember(ORNames.QueuedCommand, ORNames.Flags, _Data); } }
		public uint TargetUnitID
		{ get { return (uint)GameData.offsets.ReadStructMember(ORNames.QueuedCommand, ORNames.TargetUnitID, _Data); } }
		public Unit TargetUnit
		{
			get
			{
				uint ID = TargetUnitID;
				if (ID == 0)
					return null;
				else return Unit.GetUnit(ID);
			}
		}
		public fixed32 TargetX
		{ get { return (fixed32)GameData.offsets.ReadStructMember(ORNames.QueuedCommand, ORNames.TargetX, _Data); } }
		public fixed32 TargetY
		{ get { return (fixed32)GameData.offsets.ReadStructMember(ORNames.QueuedCommand, ORNames.TargetY, _Data); } }
		public fixed32 TargetZ
		{ get { return (fixed32)GameData.offsets.ReadStructMember(ORNames.QueuedCommand, ORNames.TargetZ, _Data); } }
		public uint AbilityPointer
		{ get { return (uint)GameData.offsets.ReadStructMember(ORNames.QueuedCommand, ORNames.AbilityPointer, _Data); } }
		public string AbilityName
		{
			get
			{
				return Abilities.Names[AbilityPointer];
			}
		}

		public byte AbilityCommand
		{ get { return (byte)GameData.offsets.ReadStructMember(ORNames.QueuedCommand, ORNames.AbilityCommand, _Data); } }
		public byte PlayerNumber
		{ get { return (byte)GameData.offsets.ReadStructMember(ORNames.QueuedCommand, ORNames.Player, _Data); } }
	}

	public class Command
	{
		byte[] _Data;

		public Command()
		{
			_Data = new byte[GameData.offsets.GetStructSize(ORNames.Command)];
		}
		public void Read(uint Address)
		{
			_Data = GameData.offsets.ReadStruct(ORNames.Command, Address);
		}
		public void Write(uint Address)
		{
			GameData.offsets.WriteStruct(ORNames.Command, _Data, Address);
		}

		public uint AbilityPointer
		{
			get { return (uint)GameData.offsets.ReadStructMember(ORNames.Command, ORNames.AbilityPointer, _Data); }
			set { GameData.offsets.WriteStructMember(ORNames.Command, ORNames.AbilityPointer, value, ref _Data); }
		}
		public uint TargetUnitID
		{
			get { return (uint)GameData.offsets.ReadStructMember(ORNames.Command, ORNames.TargetUnitID, _Data); }
			set { GameData.offsets.WriteStructMember(ORNames.Command, ORNames.TargetUnitID, value, ref _Data); }
		}
		public uint TargetUnitModelPtr
		{
			get { return (uint)GameData.offsets.ReadStructMember(ORNames.Command, ORNames.TargetUnitModelPtr, _Data); }
			set { GameData.offsets.WriteStructMember(ORNames.Command, ORNames.TargetUnitModelPtr, value, ref _Data); }
		}
		public Unit TargetUnit
		{
			get
			{
				uint ID = TargetUnitID;
				if (ID == 0)
					return null;
				else return Unit.GetUnit(ID);
			}
			set
			{
				GameData.offsets.WriteStructMember(ORNames.Command, ORNames.TargetFlags, value.ID, ref _Data);
			}
		}
		public fixed32 TargetX
		{
			get { return (fixed32)GameData.offsets.ReadStructMember(ORNames.Command, ORNames.TargetX, _Data); }
			set { GameData.offsets.WriteStructMember(ORNames.Command, ORNames.TargetX, value, ref _Data); }
		}
		public fixed32 TargetY
		{
			get { return (fixed32)GameData.offsets.ReadStructMember(ORNames.Command, ORNames.TargetY, _Data); }
			set { GameData.offsets.WriteStructMember(ORNames.Command, ORNames.TargetY, value, ref _Data); }
		}
		public fixed32 TargetZ
		{
			get { return (fixed32)GameData.offsets.ReadStructMember(ORNames.Command, ORNames.TargetZ, _Data); }
			set { GameData.offsets.WriteStructMember(ORNames.Command, ORNames.TargetZ, value, ref _Data); }
		}
		public uint Unknown
		{
			get { return (uint)GameData.offsets.ReadStructMember(ORNames.Command, ORNames.Unknown, _Data); }
			set { GameData.offsets.WriteStructMember(ORNames.Command, ORNames.Unknown, value, ref _Data); }
		}
		public TargetFlags TargetFlags
		{
			get { return (TargetFlags)(uint)GameData.offsets.ReadStructMember(ORNames.Command, ORNames.TargetFlags, _Data); }
			set { GameData.offsets.WriteStructMember(ORNames.Command, ORNames.TargetFlags, value, ref _Data); }
		}
		public OrderFlags Flags
		{
			get { return (OrderFlags)(uint)GameData.offsets.ReadStructMember(ORNames.Command, ORNames.Flags, _Data); }
			set { GameData.offsets.WriteStructMember(ORNames.Command, ORNames.Flags, value, ref _Data); }
		}
		public byte AbilityCommand
		{
			get { return (byte)GameData.offsets.ReadStructMember(ORNames.Command, ORNames.AbilityCommand, _Data); }
			set { GameData.offsets.WriteStructMember(ORNames.Command, ORNames.AbilityCommand, value, ref _Data); }
		}
		public byte PlayerNumber
		{
			get { return (byte)GameData.offsets.ReadStructMember(ORNames.Command, ORNames.Player, _Data); }
			set { GameData.offsets.WriteStructMember(ORNames.Command, ORNames.Player, value, ref _Data); }
		}
	}

	public class CommandQueue
	{
		Unit _Unit;
		List<QueuedCommand> _Queue;
		public List<QueuedCommand> Queue
		{ get { return _Queue; } }
		public Unit Unit
		{ get { return _Unit; } }
		public int Count
		{ get { return _Queue.Count; } }

		public CommandQueue(Unit unit)
		{
			_Unit = unit;
			_Queue = new List<QueuedCommand>();
			Update();
		}
		public void Update()
		{
			List<QueuedCommand> NewQueue = new List<QueuedCommand>();
			uint ptr = _Unit.commandQueuePointer;
			if (ptr != 0)
				ptr = (uint)GameData.mem.ReadMemory(ptr, typeof(uint)) & 0xFFFFFFFE;

			for (int i = 0; (ptr & 1) == 0 && i < 5000; i++) //i is to avoid an infinite loop if something goes wrong.
			{
				QueuedCommand temp = new QueuedCommand(ptr & 0xFFFFFFFE);
				NewQueue.Add(temp);
				ptr = temp.NextCommandPtr;
			}
			_Queue = NewQueue;
		}

	}

}

