namespace Data
{
	using System;
	using System.Text;
	using Utilities.MemoryHandling;

	public class Abil
	{
		private uint _address;
		private AbilClass _className;
		private ushort _id;
		private byte _index;
		private ReadWriteMemory _mem;
		private ushort _nextID;
		private ushort _previousID;
		private ushort _timesUsed;
		private uint _unitAddress;

		public Abil() : this(0, null)
		{
		}

		public Abil(uint address, ReadWriteMemory mem)
		{
			if (address == 0)
			{
				throw new Exception("InvalidAddress");
			}
			if (mem == null)
			{
				throw new Exception("Need Proper ReadWriteMemory Object");
			}
			this._address = address;
			this._mem = mem;
			this.RefreshData();
		}

		private AbilClass getClassName()
		{
			uint num;
			this._mem.ReadMemory(this.Address, 4, out num);
			num += 4;
			this._mem.ReadMemory(num, 4, out num);
			num++;
			this._mem.ReadMemory(num, 4, out num);
			byte[] lpBuffer = new byte[0x100];
			this._mem.ReadMemory(num, lpBuffer.Length, out lpBuffer);
			string str = Encoding.ASCII.GetString(lpBuffer);
			str = str.Substring(0, str.IndexOf("\0"));
			try
			{
				return (AbilClass) Enum.Parse(typeof(AbilClass), str);
			}
			catch
			{
				return AbilClass.Unknown;
			}
		}

		public void RefreshData()
		{
			this._className = this.getClassName();
		}

		public Abil Ability
		{
			get
			{
				if (this.ClassName == AbilClass.CAbilQueue)
				{
					return new AbilQueue(this.Address, this._mem);
				}
				if (this.ClassName == AbilClass.CAbilRally)
				{
					return new AbilRally(this.Address, this._mem);
				}
				if (this.ClassName == AbilClass.CAbilBuildable)
				{
					return new AbilBuildable(this.Address, this._mem);
				}
				return this;
			}
		}

		public uint Address
		{
			get
			{
				return this._address;
			}
			set
			{
				this._address = value;
			}
		}

		public AbilClass ClassName
		{
			get
			{
				return this._className;
			}
		}

		public ushort Id
		{
			get
			{
				this._id = (ushort) this._mem.ReadMemory((uint) (this.Address + 6), this._id.GetType());
				return this._id;
			}
		}

		public byte Index
		{
			get
			{
				this._index = (byte) this._mem.ReadMemory((uint) (this.Address + 0x20), this._index.GetType());
				return this._index;
			}
		}

		public ushort NextID
		{
			get
			{
				this._nextID = (ushort) this._mem.ReadMemory((uint) (this.Address + 10), this._nextID.GetType());
				return this._nextID;
			}
		}

		public ushort PreviousID
		{
			get
			{
				this._previousID = (ushort) this._mem.ReadMemory((uint) (this.Address + 8), this._previousID.GetType());
				return this._previousID;
			}
		}

		public ushort TimesUsed
		{
			get
			{
				this._timesUsed = (ushort) this._mem.ReadMemory((uint) (this.Address + 4), this._timesUsed.GetType());
				return this._timesUsed;
			}
		}

		/*public Unit Unit
		{
			get
			{
				unit_s us = (unit_s) this._mem.ReadMemory(this.UnitAddress, typeof(unit_s));
				return GameData.ParseUnit(us);
			}
		}*/

		public uint UnitAddress
		{
			get
			{
				this._unitAddress = (uint) this._mem.ReadMemory((uint) (this.Address + 0x10), this._unitAddress.GetType());
				return this._unitAddress;
			}
		}

		public enum AbilClass
		{
			CAbilQueue,
			CAbilRally,
			CAbilBuildable,
			Unknown
		}
	}
}

