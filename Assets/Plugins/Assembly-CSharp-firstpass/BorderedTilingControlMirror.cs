using UnityEngine;

public class BorderedTilingControlMirror : AutoSpriteControlBaseMirror
{
	public bool stretchHorizontally;

	public bool stretchVertically;

	public Vector2 topLeftBorder;

	public Vector2 bottomRightBorder;

	public Vector2 tilingOffset;

	public bool hasCenter;

	public Vector2 sizePerTexel;

	public bool pixelPerTexel;

	public override void Mirror(SpriteRoot s)
	{
		base.Mirror(s);
		IBorderedTilingSprite borderedTilingSprite = (IBorderedTilingSprite)s;
		stretchHorizontally = borderedTilingSprite.StretchHorizontally;
		stretchVertically = borderedTilingSprite.StretchVertically;
		topLeftBorder = borderedTilingSprite.TopLeftBorder;
		bottomRightBorder = borderedTilingSprite.BottomRightBorder;
		tilingOffset = borderedTilingSprite.TilingOffset;
		hasCenter = borderedTilingSprite.HasCenter;
		sizePerTexel = borderedTilingSprite.SizePerTexel;
		pixelPerTexel = borderedTilingSprite.PixelPerTexel;
	}

	public override bool DidChange(SpriteRoot s)
	{
		if (base.DidChange(s))
		{
			return true;
		}
		IBorderedTilingSprite borderedTilingSprite = (IBorderedTilingSprite)s;
		if (borderedTilingSprite.StretchHorizontally != stretchHorizontally || borderedTilingSprite.StretchVertically != stretchVertically || borderedTilingSprite.TopLeftBorder != topLeftBorder || borderedTilingSprite.BottomRightBorder != bottomRightBorder || borderedTilingSprite.TilingOffset != tilingOffset || borderedTilingSprite.HasCenter != hasCenter || borderedTilingSprite.SizePerTexel != sizePerTexel || borderedTilingSprite.PixelPerTexel != pixelPerTexel)
		{
			return true;
		}
		return false;
	}

	public override bool Validate(SpriteRoot s)
	{
		s.managed = false;
		s.manager = null;
		s.pixelPerfect = false;
		base.Validate(s);
		IBorderedTilingSprite borderedTilingSprite = (IBorderedTilingSprite)s;
		borderedTilingSprite.TopLeftBorder = new Vector2(Mathf.Max(0f, borderedTilingSprite.TopLeftBorder.x), Mathf.Max(0f, borderedTilingSprite.TopLeftBorder.y));
		borderedTilingSprite.BottomRightBorder = new Vector2(Mathf.Max(0f, borderedTilingSprite.BottomRightBorder.x), Mathf.Max(0f, borderedTilingSprite.BottomRightBorder.y));
		borderedTilingSprite.SizePerTexel = new Vector2(Mathf.Max(0.001f, borderedTilingSprite.SizePerTexel.x), Mathf.Max(0.001f, borderedTilingSprite.SizePerTexel.y));
		return true;
	}
}
