using System;
using UnityEngine;

[Serializable]
public class CSpriteFrame
{
	public Rect uvs = new Rect(1f, 1f, 1f, 1f);

	public Vector2 scaleFactor = new Vector2(0.5f, 0.5f);

	public Vector2 topLeftOffset = new Vector2(-1f, 1f);

	public Vector2 bottomRightOffset = new Vector2(1f, -1f);

	public CSpriteFrame()
	{
	}

	public CSpriteFrame(CSpriteFrame f)
	{
		Copy(f);
	}

	public CSpriteFrame(SPRITE_FRAME f)
	{
		Copy(f);
	}

	public void Copy(SPRITE_FRAME f)
	{
		uvs = f.uvs;
		scaleFactor = f.scaleFactor;
		topLeftOffset = f.topLeftOffset;
		bottomRightOffset = f.bottomRightOffset;
	}

	public void Copy(CSpriteFrame f)
	{
		uvs = f.uvs;
		scaleFactor = f.scaleFactor;
		topLeftOffset = f.topLeftOffset;
		bottomRightOffset = f.bottomRightOffset;
	}

	public SPRITE_FRAME ToStruct()
	{
		SPRITE_FRAME result = default(SPRITE_FRAME);
		result.uvs = uvs;
		result.scaleFactor = scaleFactor;
		result.topLeftOffset = topLeftOffset;
		result.bottomRightOffset = bottomRightOffset;
		return result;
	}
}
