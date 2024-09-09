using System.Collections.Generic;
using UnityEngine;

public class PackedSprite : AutoSpriteBase
{
	[HideInInspector]
	public string staticTexPath = string.Empty;

	[HideInInspector]
	public string staticTexGUID = string.Empty;

	[HideInInspector]
	public CSpriteFrame _ser_stat_frame_info = new CSpriteFrame();

	[HideInInspector]
	public SPRITE_FRAME staticFrameInfo;

	public TextureAnim[] textureAnimations;

	public override TextureAnim[] States
	{
		get
		{
			if (textureAnimations == null)
			{
				textureAnimations = new TextureAnim[0];
			}
			return textureAnimations;
		}
		set
		{
			textureAnimations = value;
		}
	}

	public override CSpriteFrame DefaultFrame
	{
		get
		{
			return _ser_stat_frame_info;
		}
	}

	public override TextureAnim DefaultState
	{
		get
		{
			if (textureAnimations != null && textureAnimations.Length != 0 && defaultAnim < textureAnimations.Length)
			{
				return textureAnimations[defaultAnim];
			}
			return null;
		}
	}

	public override bool SupportsArbitraryAnimations
	{
		get
		{
			return true;
		}
	}

	public override Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader)
	{
		if (staticTexGUID == string.Empty)
		{
			return Vector2.zero;
		}
		Texture2D texture2D = (Texture2D)loader(guid2Path(staticTexGUID), typeof(Texture2D));
		return new Vector2((float)texture2D.width * (1f / (_ser_stat_frame_info.scaleFactor.x * 2f)), (float)texture2D.height * (1f / (_ser_stat_frame_info.scaleFactor.y * 2f)));
	}

	protected override void Awake()
	{
		if (textureAnimations == null)
		{
			textureAnimations = new TextureAnim[0];
		}
		staticFrameInfo = _ser_stat_frame_info.ToStruct();
		base.Awake();
		Init();
	}

	public override void Start()
	{
		if (!m_started)
		{
			base.Start();
			if (playAnimOnStart && defaultAnim < animations.Length && Application.isPlaying)
			{
				PlayAnim(defaultAnim);
			}
		}
	}

	protected override void Init()
	{
		base.Init();
	}

	public override void Copy(SpriteRoot s)
	{
		base.Copy(s);
		if (!(s is PackedSprite))
		{
			return;
		}
		PackedSprite packedSprite = (PackedSprite)s;
		if (!packedSprite.m_started)
		{
			staticFrameInfo = packedSprite._ser_stat_frame_info.ToStruct();
		}
		else
		{
			staticFrameInfo = packedSprite.staticFrameInfo;
		}
		if (curAnim != null)
		{
			if (curAnim.index == -1)
			{
				PlayAnim(curAnim);
			}
			else
			{
				SetState(curAnim.index);
			}
		}
		else
		{
			frameInfo = staticFrameInfo;
			uvRect = frameInfo.uvs;
			if (autoResize || pixelPerfect)
			{
				CalcSize();
			}
			else
			{
				SetSize(s.width, s.height);
			}
		}
		SetBleedCompensation();
	}

	public override void InitUVs()
	{
		frameInfo = staticFrameInfo;
		uvRect = staticFrameInfo.uvs;
	}

	public void AddAnimation(UVAnimation anim)
	{
		UVAnimation[] array = animations;
		animations = new UVAnimation[array.Length + 1];
		array.CopyTo(animations, 0);
		animations[animations.Length - 1] = anim;
	}

	public override void Aggregate(PathFromGUIDDelegate guid2Path, LoadAssetDelegate load, GUIDFromPathDelegate path2Guid)
	{
		List<Texture2D> list = new List<Texture2D>();
		List<CSpriteFrame> list2 = new List<CSpriteFrame>();
		for (int i = 0; i < textureAnimations.Length; i++)
		{
			textureAnimations[i].Allocate();
			if (textureAnimations[i].frameGUIDs.Length >= textureAnimations[i].framePaths.Length)
			{
				for (int j = 0; j < textureAnimations[i].frameGUIDs.Length; j++)
				{
					string path = guid2Path(textureAnimations[i].frameGUIDs[j]);
					list.Add((Texture2D)load(path, typeof(Texture2D)));
					list2.Add(textureAnimations[i].spriteFrames[j]);
				}
				textureAnimations[i].framePaths = new string[0];
				continue;
			}
			textureAnimations[i].frameGUIDs = new string[textureAnimations[i].framePaths.Length];
			textureAnimations[i].spriteFrames = new CSpriteFrame[textureAnimations[i].framePaths.Length];
			for (int k = 0; k < textureAnimations[i].spriteFrames.Length; k++)
			{
				textureAnimations[i].spriteFrames[k] = new CSpriteFrame();
			}
			for (int l = 0; l < textureAnimations[i].framePaths.Length; l++)
			{
				if (textureAnimations[i].framePaths[l].Length >= 1)
				{
					textureAnimations[i].frameGUIDs[l] = path2Guid(textureAnimations[i].framePaths[l]);
					list.Add((Texture2D)load(textureAnimations[i].framePaths[l], typeof(Texture2D)));
					list2.Add(textureAnimations[i].spriteFrames[l]);
				}
			}
		}
		if (staticTexGUID.Length > 1)
		{
			staticTexPath = guid2Path(staticTexGUID);
		}
		else
		{
			staticTexGUID = path2Guid(staticTexPath);
		}
		list.Add((Texture2D)load(staticTexPath, typeof(Texture2D)));
		list2.Add(_ser_stat_frame_info);
		sourceTextures = list.ToArray();
		spriteFrames = list2.ToArray();
	}

	public static PackedSprite Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (PackedSprite)gameObject.AddComponent(typeof(PackedSprite));
	}

	public static PackedSprite Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (PackedSprite)gameObject.AddComponent(typeof(PackedSprite));
	}
}
