namespace Data
{
	using System;
	using Utilities.MemoryHandling;

	public class AbilQueue : Abil
	{
		private ushort _count;
		private int _extraSlots;
		private ReadWriteMemory _mem;
		private Queue[] _queues;

		public AbilQueue(uint address, ReadWriteMemory mem) : base(address, mem)
		{
			this._mem = mem;
		}

		public ushort Count
		{
			get
			{
				this._count = (ushort) this._mem.ReadMemory((uint) (base.Address + 0x24), this._count.GetType());
				return this._count;
			}
		}

		public int ExtraSlots
		{
			get
			{
				this._extraSlots = (int) this._mem.ReadMemory((uint) (base.Address + 0x44), this._extraSlots.GetType());
				return this._extraSlots;
			}
			set
			{
				byte[] bytes = BitConverter.GetBytes(value);
				this._mem.WriteMemory((uint) (base.Address + 0x44), bytes.Length, ref bytes);
			}
		}

		public Queue[] Queues
		{
			get
			{
				Queue[] queueArray = new Queue[this.Count];
				for (uint i = 0; i < queueArray.Length; i++)
				{
					uint num2;
					this._mem.ReadMemory(base.Address + 0x30, 4, out num2);
					this._mem.ReadMemory(num2 + (i * 4), 4, out num2);
					queueArray[i] = new Queue(num2);
				}
				this._queues = queueArray;
				return this._queues;
			}
		}

		public class Queue
		{
			private uint _address;
			private int _duration;
			private int _elapsedTime;
			private int _foodCost;
			private ReadWriteMemory _mem = new ReadWriteMemory(GameData.SC2Handle);
			private int _mineralCost;
			private Unit _unit;
			private int _vespeneCost;

			public Queue(uint address)
			{
				this._address = address;
			}

			public uint Address
			{
				get
				{
					return this._address;
				}
			}

			public int Duration
			{
				get
				{
					this._duration = (int) this._mem.ReadMemory((uint) (this.Address + 0xc0), this._duration.GetType());
					return this._duration;
				}
			}

			public int ElapsedTime
			{
				get
				{
					this._elapsedTime = (int) this._mem.ReadMemory((uint) (this.Address + 0xc4), this._elapsedTime.GetType());
					return this._elapsedTime;
				}
				set
				{
					byte[] bytes = BitConverter.GetBytes(value);
					this._mem.WriteMemory((uint) (this.Address + 0xc4), bytes.Length, ref bytes);
				}
			}

			public int FoodCost
			{
				get
				{
					this._foodCost = (int) this._mem.ReadMemory((uint) (this.Address + 0xb8), this._foodCost.GetType());
					return this._foodCost;
				}
			}

			public int MineralCost
			{
				get
				{
					this._mineralCost = (int) this._mem.ReadMemory((uint) (this.Address + 0x5c), this._mineralCost.GetType());
					return this._mineralCost;
				}
			}

			public Unit Unit
			{
				get
				{
					return this._unit;
				}
			}

			public int VespeneCost
			{
				get
				{
					this._vespeneCost = (int) this._mem.ReadMemory((uint) (this.Address + 0x60), this._vespeneCost.GetType());
					return this._vespeneCost;
				}
			}
		}
	}
}

