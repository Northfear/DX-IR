using UnityEngine;

[ExecuteInEditMode]
public class SimpleSprite : SpriteRoot
{
	public Vector2 lowerLeftPixel;

	public Vector2 pixelDimensions;

	protected bool nullCamera;

	public override Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader)
	{
		return pixelDimensions;
	}

	protected override void Awake()
	{
		base.Awake();
		Init();
	}

	protected override void Init()
	{
		nullCamera = renderCamera == null;
		base.Init();
	}

	public override void Start()
	{
		base.Start();
		if (UIManager.Exists() && nullCamera && UIManager.instance.uiCameras.Length > 0)
		{
			SetCamera(UIManager.instance.uiCameras[0].camera);
		}
	}

	public override void Clear()
	{
		base.Clear();
	}

	public void Setup(float w, float h, Vector2 lowerleftPixel, Vector2 pixeldimensions)
	{
		Setup(w, h, lowerleftPixel, pixeldimensions, m_spriteMesh.material);
	}

	public void Setup(float w, float h, Vector2 lowerleftPixel, Vector2 pixeldimensions, Material material)
	{
		width = w;
		height = h;
		lowerLeftPixel = lowerleftPixel;
		pixelDimensions = pixeldimensions;
		uvsInitialized = false;
		if (!managed)
		{
			((SpriteMesh)m_spriteMesh).material = material;
		}
		Init();
	}

	public override void Copy(SpriteRoot s)
	{
		base.Copy(s);
		if (s is SimpleSprite)
		{
			lowerLeftPixel = ((SimpleSprite)s).lowerLeftPixel;
			pixelDimensions = ((SimpleSprite)s).pixelDimensions;
			InitUVs();
			SetBleedCompensation(s.bleedCompensation);
			if (autoResize || pixelPerfect)
			{
				CalcSize();
			}
			else
			{
				SetSize(s.width, s.height);
			}
		}
	}

	public override void InitUVs()
	{
		tempUV = PixelCoordToUVCoord(lowerLeftPixel);
		uvRect.x = tempUV.x;
		uvRect.y = tempUV.y;
		tempUV = PixelSpaceToUVSpace(pixelDimensions);
		uvRect.xMax = uvRect.x + tempUV.x;
		uvRect.yMax = uvRect.y + tempUV.y;
		frameInfo.uvs = uvRect;
		base.InitUVs();
	}

	public void SetLowerLeftPixel(Vector2 lowerLeft)
	{
		lowerLeftPixel = lowerLeft;
		tempUV = PixelCoordToUVCoord(lowerLeftPixel);
		uvRect.x = tempUV.x;
		uvRect.y = tempUV.y;
		tempUV = PixelSpaceToUVSpace(pixelDimensions);
		uvRect.xMax = uvRect.x + tempUV.x;
		uvRect.yMax = uvRect.y + tempUV.y;
		frameInfo.uvs = uvRect;
		SetBleedCompensation(bleedCompensation);
		if (autoResize || pixelPerfect)
		{
			CalcSize();
		}
	}

	public void SetLowerLeftPixel(int x, int y)
	{
		SetLowerLeftPixel(new Vector2(x, y));
	}

	public void SetPixelDimensions(Vector2 size)
	{
		pixelDimensions = size;
		tempUV = PixelSpaceToUVSpace(pixelDimensions);
		uvRect.xMax = uvRect.x + tempUV.x;
		uvRect.yMax = uvRect.y + tempUV.y;
		frameInfo.uvs = uvRect;
		if (autoResize || pixelPerfect)
		{
			CalcSize();
		}
	}

	public void SetPixelDimensions(int x, int y)
	{
		SetPixelDimensions(new Vector2(x, y));
	}

	public override int GetStateIndex(string stateName)
	{
		return -1;
	}

	public override void SetState(int index)
	{
	}

	public static SimpleSprite Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (SimpleSprite)gameObject.AddComponent(typeof(SimpleSprite));
	}

	public static SimpleSprite Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (SimpleSprite)gameObject.AddComponent(typeof(SimpleSprite));
	}

	public override void DoMirror()
	{
		if (!Application.isPlaying)
		{
			if (screenSize.x == 0f || screenSize.y == 0f)
			{
				base.Start();
			}
			if (mirror == null)
			{
				mirror = new SimpleSpriteMirror();
				mirror.Mirror(this);
			}
			mirror.Validate(this);
			if (mirror.DidChange(this))
			{
				Init();
				mirror.Mirror(this);
			}
		}
	}
}
