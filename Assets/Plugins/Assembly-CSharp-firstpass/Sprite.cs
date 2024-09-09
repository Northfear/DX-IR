using System;
using UnityEngine;

[ExecuteInEditMode]
public class Sprite : SpriteBase
{
	public Vector2 lowerLeftPixel;

	public Vector2 pixelDimensions;

	public UVAnimation_Multi[] animations;

	protected UVAnimation_Multi curAnim;

	public override Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader)
	{
		return pixelDimensions;
	}

	protected override void Awake()
	{
		base.Awake();
		Init();
		if (animations == null)
		{
			animations = new UVAnimation_Multi[0];
		}
		for (int i = 0; i < animations.Length; i++)
		{
			animations[i].index = i;
			animations[i].BuildUVAnim(this);
		}
	}

	protected override void Init()
	{
		base.Init();
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

	public override void Clear()
	{
		base.Clear();
		if (curAnim != null)
		{
			PauseAnim();
			curAnim = null;
		}
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
		if (uvsInitialized)
		{
			Init();
			InitUVs();
			SetBleedCompensation();
		}
		else
		{
			Init();
		}
	}

	public override void Copy(SpriteRoot s)
	{
		base.Copy(s);
		if (!(s is Sprite))
		{
			return;
		}
		Sprite sprite = (Sprite)s;
		lowerLeftPixel = sprite.lowerLeftPixel;
		pixelDimensions = sprite.pixelDimensions;
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
		if (sprite.animations.Length > 0)
		{
			animations = new UVAnimation_Multi[sprite.animations.Length];
			for (int i = 0; i < animations.Length; i++)
			{
				animations[i] = sprite.animations[i].Clone();
			}
		}
		for (int j = 0; j < animations.Length; j++)
		{
			animations[j].BuildUVAnim(this);
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
	}

	public void AddAnimation(UVAnimation_Multi anim)
	{
		UVAnimation_Multi[] array = animations;
		animations = new UVAnimation_Multi[array.Length + 1];
		array.CopyTo(animations, 0);
		anim.index = animations.Length - 1;
		animations[anim.index] = anim;
	}

	public override bool StepAnim(float time)
	{
		if (curAnim == null)
		{
			return false;
		}
		timeSinceLastFrame += Mathf.Max(0f, time);
		framesToAdvance = timeSinceLastFrame / timeBetweenAnimFrames;
		if (framesToAdvance < 1f)
		{
			if (crossfadeFrames)
			{
				SetColor(new Color(1f, 1f, 1f, 1f - framesToAdvance));
			}
			return true;
		}
		while (framesToAdvance >= 1f)
		{
			if (curAnim.GetNextFrame(ref frameInfo))
			{
				framesToAdvance -= 1f;
				timeSinceLastFrame -= timeBetweenAnimFrames;
				continue;
			}
			if (crossfadeFrames)
			{
				SetColor(Color.white);
			}
			switch (curAnim.onAnimEnd)
			{
			case UVAnimation.ANIM_END_ACTION.Do_Nothing:
				PauseAnim();
				uvRect = frameInfo.uvs;
				SetBleedCompensation();
				if (autoResize || pixelPerfect)
				{
					CalcSize();
				}
				break;
			case UVAnimation.ANIM_END_ACTION.Revert_To_Static:
				RevertToStatic();
				curAnim = null;
				break;
			case UVAnimation.ANIM_END_ACTION.Play_Default_Anim:
				if (animCompleteDelegate != null)
				{
					animCompleteDelegate(this);
				}
				PlayAnim(defaultAnim);
				return false;
			case UVAnimation.ANIM_END_ACTION.Hide:
				Hide(true);
				break;
			case UVAnimation.ANIM_END_ACTION.Deactivate:
				base.gameObject.active = false;
				break;
			case UVAnimation.ANIM_END_ACTION.Destroy:
				if (animCompleteDelegate != null)
				{
					animCompleteDelegate(this);
				}
				Delete();
				UnityEngine.Object.Destroy(base.gameObject);
				break;
			}
			if (animCompleteDelegate != null)
			{
				animCompleteDelegate(this);
			}
			if (!animating)
			{
				curAnim = null;
			}
			return false;
		}
		if (crossfadeFrames)
		{
			UVAnimation currentClip = curAnim.GetCurrentClip();
			int curClipNum = curAnim.GetCurClipNum();
			int curPosition = currentClip.GetCurPosition();
			int stepDirection = curAnim.StepDirection;
			int stepDirection2 = currentClip.StepDirection;
			curAnim.GetNextFrame(ref nextFrameInfo);
			Vector2[] uvs = m_spriteMesh.uvs2;
			Rect uvs2 = nextFrameInfo.uvs;
			uvs[0].x = uvs2.xMin;
			uvs[0].y = uvs2.yMax;
			uvs[1].x = uvs2.xMin;
			uvs[1].y = uvs2.yMin;
			uvs[2].x = uvs2.xMax;
			uvs[2].y = uvs2.yMin;
			uvs[3].x = uvs2.xMax;
			uvs[3].y = uvs2.yMax;
			curAnim.SetCurClipNum(curClipNum);
			currentClip.SetCurrentFrame(curPosition);
			curAnim.StepDirection = stepDirection;
			currentClip.StepDirection = stepDirection2;
			SetColor(new Color(1f, 1f, 1f, 1f - framesToAdvance));
		}
		uvRect = frameInfo.uvs;
		SetBleedCompensation();
		if (autoResize || pixelPerfect)
		{
			CalcSize();
		}
		return true;
	}

	public void PlayAnim(UVAnimation_Multi anim)
	{
		if (deleted || !base.gameObject.active)
		{
			return;
		}
		if (!m_started)
		{
			Start();
		}
		curAnim = anim;
		curAnimIndex = curAnim.index;
		curAnim.Reset();
		if (anim.framerate != 0f)
		{
			timeBetweenAnimFrames = 1f / anim.framerate;
		}
		else
		{
			timeBetweenAnimFrames = 1f;
		}
		timeSinceLastFrame = timeBetweenAnimFrames;
		if ((anim.GetFrameCount() > 1 || anim.onAnimEnd != 0) && anim.framerate != 0f)
		{
			StepAnim(0f);
			if (!animating)
			{
				AddToAnimatedList();
			}
			return;
		}
		PauseAnim();
		if (animCompleteDelegate != null)
		{
			animCompleteDelegate(this);
		}
		StepAnim(0f);
	}

	public override void PlayAnim(int index)
	{
		if (index >= animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
		}
		else
		{
			PlayAnim(animations[index]);
		}
	}

	public override void PlayAnim(string name)
	{
		for (int i = 0; i < animations.Length; i++)
		{
			if (animations[i].name == name)
			{
				PlayAnim(animations[i]);
				return;
			}
		}
		Debug.LogError("ERROR: Animation \"" + name + "\" not found!");
	}

	public void PlayAnimInReverse(UVAnimation_Multi anim)
	{
		if (deleted || !base.gameObject.active)
		{
			return;
		}
		if (!m_started)
		{
			Start();
		}
		curAnim = anim;
		curAnim.Reset();
		curAnim.PlayInReverse();
		if (anim.framerate != 0f)
		{
			timeBetweenAnimFrames = 1f / anim.framerate;
		}
		else
		{
			timeBetweenAnimFrames = 1f;
		}
		timeSinceLastFrame = timeBetweenAnimFrames;
		if ((anim.GetFrameCount() > 1 || anim.onAnimEnd != 0) && anim.framerate != 0f)
		{
			StepAnim(0f);
			if (!animating)
			{
				AddToAnimatedList();
			}
			return;
		}
		PauseAnim();
		if (animCompleteDelegate != null)
		{
			animCompleteDelegate(this);
		}
		StepAnim(0f);
	}

	public override void PlayAnimInReverse(int index)
	{
		if (index >= animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
		}
		else
		{
			PlayAnimInReverse(animations[index]);
		}
	}

	public override void PlayAnimInReverse(string name)
	{
		for (int i = 0; i < animations.Length; i++)
		{
			if (animations[i].name == name)
			{
				animations[i].PlayInReverse();
				PlayAnimInReverse(animations[i]);
				return;
			}
		}
		Debug.LogError("ERROR: Animation \"" + name + "\" not found!");
	}

	public void DoAnim(int index)
	{
		if (curAnim == null)
		{
			PlayAnim(index);
		}
		else if (curAnim != animations[index] || !animating)
		{
			PlayAnim(index);
		}
	}

	public void DoAnim(string name)
	{
		if (curAnim == null)
		{
			PlayAnim(name);
		}
		else if (curAnim.name != name || !animating)
		{
			PlayAnim(name);
		}
	}

	public void DoAnim(UVAnimation_Multi anim)
	{
		if (curAnim != anim || !animating)
		{
			PlayAnim(anim);
		}
	}

	public override void StopAnim()
	{
		RemoveFromAnimatedList();
		if (curAnim != null)
		{
			curAnim.Reset();
		}
		RevertToStatic();
	}

	public void UnpauseAnim()
	{
		if (curAnim != null)
		{
			AddToAnimatedList();
		}
	}

	protected override void AddToAnimatedList()
	{
		if (!animating && Application.isPlaying && base.gameObject.active)
		{
			animating = true;
			SpriteAnimationPump.Add(this);
		}
	}

	protected override void RemoveFromAnimatedList()
	{
		SpriteAnimationPump.Remove(this);
		animating = false;
	}

	public UVAnimation_Multi GetCurAnim()
	{
		return curAnim;
	}

	public UVAnimation_Multi GetAnim(string name)
	{
		for (int i = 0; i < animations.Length; i++)
		{
			if (animations[i].name == name)
			{
				return animations[i];
			}
		}
		return null;
	}

	public override int GetStateIndex(string stateName)
	{
		for (int i = 0; i < animations.Length; i++)
		{
			if (string.Equals(animations[i].name, stateName, StringComparison.CurrentCultureIgnoreCase))
			{
				return i;
			}
		}
		return -1;
	}

	public override void SetState(int index)
	{
		PlayAnim(index);
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
		uvRect.xMax -= bleedCompensationUV.x * 2f;
		uvRect.yMax -= bleedCompensationUV.y * 2f;
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
				mirror = new SpriteMirror();
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

	public static Sprite Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (Sprite)gameObject.AddComponent(typeof(Sprite));
	}

	public static Sprite Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (Sprite)gameObject.AddComponent(typeof(Sprite));
	}
}
