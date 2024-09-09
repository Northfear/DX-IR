using UnityEngine;

public static class FontStore
{
	private static SpriteFont[] fonts = new SpriteFont[0];

	public static SpriteFont GetFont(TextAsset fontDef)
	{
		if (fontDef == null)
		{
			return null;
		}
		for (int i = 0; i < fonts.Length; i++)
		{
			if (fonts[i].fontDef == fontDef)
			{
				if (!Application.isPlaying)
				{
					fonts[i] = new SpriteFont(fontDef);
				}
				return fonts[i];
			}
		}
		SpriteFont spriteFont = new SpriteFont(fontDef);
		AddFont(spriteFont);
		return spriteFont;
	}

	private static void AddFont(SpriteFont f)
	{
		SpriteFont[] array = new SpriteFont[fonts.Length + 1];
		fonts.CopyTo(array, 0);
		array[fonts.Length] = f;
		fonts = array;
	}
}
