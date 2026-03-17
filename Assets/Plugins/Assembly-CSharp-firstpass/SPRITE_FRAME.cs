using System;
using UnityEngine;

[Serializable]
public struct SPRITE_FRAME
{
	public Rect uvs;

	public Vector2 scaleFactor;

	public Vector2 topLeftOffset;

	public Vector2 bottomRightOffset;

	public SPRITE_FRAME(float dummy)
	{
		uvs = new Rect(1f, 1f, 1f, 1f);
		scaleFactor = new Vector2(0.5f, 0.5f);
		topLeftOffset = new Vector2(-1f, 1f);
		bottomRightOffset = new Vector2(1f, -1f);
	}

	public void Copy(CSpriteFrame f)
	{
		uvs = f.uvs;
		scaleFactor = f.scaleFactor;
		topLeftOffset = f.topLeftOffset;
		bottomRightOffset = f.bottomRightOffset;
	}
}
