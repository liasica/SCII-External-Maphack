namespace Data
{
	using System;
	using Utilities.MemoryHandling;

	public class AbilBuildable : Abil
	{
		private int _duration;
		private int _elapsedTime;
		private ReadWriteMemory _mem;

		public AbilBuildable(uint address, ReadWriteMemory mem) : base(address, mem)
		{
			this._mem = mem;
		}

		public void Heal()
		{
			byte[] bytes = BitConverter.GetBytes(0);
			this._mem.WriteMemory((uint) (base.UnitAddress + 0x100), bytes.Length, ref bytes);
			this._mem.WriteMemory((uint) (base.UnitAddress + 260), bytes.Length, ref bytes);
		}

		public int Duration
		{
			get
			{
				this._duration = (int) this._mem.ReadMemory((uint) (base.Address + 0x24), this._duration.GetType());
				return this._duration;
			}
		}

		public int ElapsedTime
		{
			get
			{
				this._elapsedTime = (int) this._mem.ReadMemory((uint) (base.Address + 40), this._elapsedTime.GetType());
				return this._elapsedTime;
			}
			set
			{
				byte[] bytes = BitConverter.GetBytes(value);
				this._mem.WriteMemory((uint) (base.Address + 40), bytes.Length, ref bytes);
			}
		}
	}
}

