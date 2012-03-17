namespace Data
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct Unit
	{
		public float timeScale;
		public bool isAlive;
		public bool isImmobile;
		public TargetFilter targetFilterFlags;
		public uint playerNumber;
		public UnitType unitType;
		public float locationX;
		public float locationY;
		public float locationZ;
		public float destinationX;
		public float destinationY;
		public float destinationZ;

		public float healthRegenDelay;
		public float healthRegenRate;
		public float healthDamage;
		public float currentHealth;
		public float maxHealth;

		public float shieldRegenDelay;
		public float shieldRegenRate;
		public float shieldDamage;
		public float currentShield;
		public float maxShield;

		public float energyRegenDelay;
		public float energyRegenRate;
		public float energyDamage;
		public float currentEnergy;
		public float maxEnergy;

		public float destination2X;
		public float destination2Y;
		public UnitStateOld state;
		public UnitMoveState moveState;
		public UnitSubMoveState subMoveState;
		public UnitLastOrder lastOrder;
		public DeathType deathType;
		public int ID;
		public float rotation;
		public float rotationX;
		public float rotationY;
		public int kills;
		public int moveSpeed;
		public bool cloaked;
		public bool detector;
		public string status;
		public uint memoryLocation;
		public uint commandQueuePointer;
		public List<QueueItem> currentQueueItem;
		public int priorityKill;
		public int trueCostOfProduction;

		public string textID;
		public float minimapRadius;

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

