public class ControlBaseMirror
{
	private string text;

	private float textOffsetZ;

	public virtual void Mirror(ControlBase c)
	{
		text = c.text;
		textOffsetZ = c.textOffsetZ;
	}

	public virtual bool DidChange(ControlBase c)
	{
		if (text != c.text)
		{
			c.Text = c.text;
			return true;
		}
		if (textOffsetZ != c.textOffsetZ)
		{
			if (c.spriteText != null)
			{
				c.spriteText.offsetZ = textOffsetZ;
			}
			return true;
		}
		return false;
	}

	public virtual void Validate(ControlBase c)
	{
	}
}
