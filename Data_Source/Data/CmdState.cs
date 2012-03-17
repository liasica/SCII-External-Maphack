namespace Data
{
	using System;

	public enum CmdState
	{
		CanAutoCast = 4,
		CantSpend = 0x10000,
		Cooldown = 0x80,
		Disabled = 0x40,
		Executing = 2,
		HasCharges = 0x20,
		Hidden = 1,
		IsAutoCast = 8,
		MaxCharges = 0x10,
		NoCustom = 0x4000,
		NoEnergy = 0x400,
		NoFood = 0x8000,
		NoLife = 0x100,
		NoMinerals = 0x800,
		NoShields = 0x200,
		NoTerrazine = 0x2000,
		NoVespene = 0x1000
	}
}

