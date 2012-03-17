namespace Data
{
	using System;
	using Utilities.MemoryHandling;

	public class AbilRally : Abil
	{
		private int _count;
		private ReadWriteMemory _mem;

		public AbilRally(uint address, ReadWriteMemory mem) : base(address, mem)
		{
			uint num;
			this._mem = mem;
			base.Address = address;
			this._mem.ReadMemory(base.Address + 0x30, 4, out num);
			base.Address = num;
		}

		public int Count
		{
			get
			{
				this._count = (int) this._mem.ReadMemory(base.Address, this._count.GetType());
				return this._count;
			}
		}

		public Rally[] Rallies
		{
			get
			{
				Rally[] rallyArray = new Rally[this.Count];
				for (uint i = 0; i < this.Count; i++)
				{
					rallyArray[i] = new Rally();
					rallyArray[i].X = (int) this._mem.ReadMemory((uint) ((base.Address + 12) + (i * 0x18)), rallyArray[i].X.GetType());
					rallyArray[i].Y = (int) this._mem.ReadMemory((uint) ((base.Address + 0x10) + (i * 0x18)), rallyArray[i].Y.GetType());
					rallyArray[i].Z = (int) this._mem.ReadMemory((uint) ((base.Address + 20) + (i * 0x18)), rallyArray[i].Z.GetType());
				}
				return rallyArray;
			}
		}

		public class Rally
		{
			private int _x;
			private int _y;
			private int _z;

			public int X
			{
				get
				{
					return this._x;
				}
				set
				{
					this._x = value;
				}
			}

			public int Y
			{
				get
				{
					return this._y;
				}
				set
				{
					this._y = value;
				}
			}

			public int Z
			{
				get
				{
					return this._z;
				}
				set
				{
					this._z = value;
				}
			}
		}
	}
}

