using System;
using UnityEngine;

[Serializable]
public class UVAnimation_Auto : UVAnimation
{
	public Vector2 start;

	public Vector2 pixelsToNextColumnAndRow;

	public int cols;

	public int rows;

	public int totalCells;

	public UVAnimation_Auto()
	{
	}

	public UVAnimation_Auto(UVAnimation_Auto anim)
		: base(anim)
	{
		start = anim.start;
		pixelsToNextColumnAndRow = anim.pixelsToNextColumnAndRow;
		cols = anim.cols;
		rows = anim.rows;
		totalCells = anim.totalCells;
	}

	public new UVAnimation_Auto Clone()
	{
		return new UVAnimation_Auto(this);
	}

	public SPRITE_FRAME[] BuildUVAnim(SpriteRoot s)
	{
		if (totalCells < 1)
		{
			return null;
		}
		return BuildUVAnim(s.PixelCoordToUVCoord(start), s.PixelSpaceToUVSpace(pixelsToNextColumnAndRow), cols, rows, totalCells);
	}
}
