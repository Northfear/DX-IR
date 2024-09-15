public class AutoSpriteControlBaseMirror : SpriteRootMirror
{
	private string text;

	private float textOffsetZ;

	public override void Mirror(SpriteRoot s)
	{
		AutoSpriteControlBase autoSpriteControlBase = (AutoSpriteControlBase)s;
		base.Mirror(s);
		text = autoSpriteControlBase.text;
		textOffsetZ = autoSpriteControlBase.textOffsetZ;
	}

	public override bool DidChange(SpriteRoot s)
	{
		AutoSpriteControlBase autoSpriteControlBase = (AutoSpriteControlBase)s;
		if (text != autoSpriteControlBase.text)
		{
			autoSpriteControlBase.Text = autoSpriteControlBase.text;
			return true;
		}
		if (textOffsetZ != autoSpriteControlBase.textOffsetZ)
		{
			if (autoSpriteControlBase.spriteText != null)
			{
				autoSpriteControlBase.spriteText.offsetZ = textOffsetZ;
			}
			return true;
		}
		return base.DidChange(s);
	}
}
