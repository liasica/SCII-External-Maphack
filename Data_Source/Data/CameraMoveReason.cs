namespace Data
{
	using System;

	public enum CameraMoveReason
	{
		Alert = 0,
		Any = -1,
		KeyScroll = 1,
		Minimap = 2,
		MouseScroll = 3,
		Selection = 4,
		Town = 5,
		View = 6
	}
}

