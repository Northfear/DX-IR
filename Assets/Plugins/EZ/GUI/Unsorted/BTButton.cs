using UnityEngine;

public class BTButton : UIButton, IBorderedTilingSprite
{
	public bool stretchHorizontally = true;

	public bool stretchVertically = true;

	public Vector2 topLeftBorder;

	public Vector2 bottomRightBorder;

	public Vector2 tilingOffset;

	public bool hasCenter = true;

	public Vector2 sizePerTexel = new Vector2(1f, 1f);

	public bool pixelPerTexel;

	Rect IBorderedTilingSprite.UnclippedUVRect
	{
		get
		{
			return Rect.MinMaxRect(frameInfo.uvs.x + bleedCompensationUV.x, frameInfo.uvs.y + bleedCompensationUV.y, frameInfo.uvs.xMax + bleedCompensationUVMax.x, frameInfo.uvs.yMax + bleedCompensationUVMax.y);
		}
	}

	float IBorderedTilingSprite.SizePerPixel
	{
		get
		{
			return worldUnitsPerScreenPixel;
		}
	}

	bool IBorderedTilingSprite.StretchHorizontally
	{
		get
		{
			return stretchHorizontally;
		}
		set
		{
			stretchHorizontally = value;
		}
	}

	bool IBorderedTilingSprite.StretchVertically
	{
		get
		{
			return stretchVertically;
		}
		set
		{
			stretchVertically = value;
		}
	}

	Vector2 IBorderedTilingSprite.TopLeftBorder
	{
		get
		{
			return topLeftBorder;
		}
		set
		{
			topLeftBorder = value;
		}
	}

	Vector2 IBorderedTilingSprite.BottomRightBorder
	{
		get
		{
			return bottomRightBorder;
		}
		set
		{
			bottomRightBorder = value;
		}
	}

	Vector2 IBorderedTilingSprite.TilingOffset
	{
		get
		{
			return tilingOffset;
		}
		set
		{
			tilingOffset = value;
		}
	}

	bool IBorderedTilingSprite.HasCenter
	{
		get
		{
			return hasCenter;
		}
		set
		{
			hasCenter = value;
		}
	}

	Vector2 IBorderedTilingSprite.SizePerTexel
	{
		get
		{
			return sizePerTexel;
		}
		set
		{
			sizePerTexel = value;
		}
	}

	bool IBorderedTilingSprite.PixelPerTexel
	{
		get
		{
			return pixelPerTexel;
		}
		set
		{
			pixelPerTexel = value;
		}
	}

	protected override void Awake()
	{
		managed = false;
		manager = null;
		pixelPerfect = false;
		base.Awake();
		m_spriteMesh = new BorderedTilingSpriteMesh();
		m_spriteMesh.sprite = this;
	}

	public override void Start()
	{
		if (!uvsInitialized)
		{
			InitUVs();
			uvsInitialized = true;
		}
		if (!(m_spriteMesh is BorderedTilingSpriteMesh))
		{
			m_spriteMesh = new BorderedTilingSpriteMesh();
			m_spriteMesh.sprite = this;
		}
		base.Start();
	}

	public override void DoMirror()
	{
		if (mirror == null)
		{
			mirror = new BorderedTilingControlMirror();
			mirror.Mirror(this);
		}
		base.DoMirror();
	}
}
