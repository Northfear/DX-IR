public class AutoSprite : AutoSpriteBase
{
	public TextureAnim[] textureAnimations;

	public override TextureAnim[] States
	{
		get
		{
			return textureAnimations;
		}
		set
		{
			textureAnimations = value;
		}
	}

	protected override void Awake()
	{
		if (textureAnimations == null)
		{
			textureAnimations = new TextureAnim[0];
		}
		base.Awake();
		Init();
	}

	protected override void Init()
	{
		base.Init();
	}
}
