public class InteractiveObject_Ladder : InteractiveObject_Base
{
	public override bool InteractWithObject(bool instant = false)
	{
		if (!base.InteractWithObject(instant))
		{
			return false;
		}
		return true;
	}
}
