using System;
using System.Collections;
using UnityEngine;

public abstract class AutoSpriteBase : SpriteBase, ISpriteAggregator, ISpritePackable
{
	protected Texture2D[] sourceTextures;

	protected CSpriteFrame[] spriteFrames;

	public bool doNotTrimImages;

	[HideInInspector]
	public UVAnimation[] animations;

	protected UVAnimation curAnim;

	public abstract TextureAnim[] States { get; set; }

	public virtual CSpriteFrame DefaultFrame
	{
		get
		{
			if (States[0].spriteFrames.Length != 0)
			{
				return States[0].spriteFrames[0];
			}
			return null;
		}
	}

	public virtual TextureAnim DefaultState
	{
		get
		{
			if (States != null && States.Length != 0)
			{
				return States[0];
			}
			return null;
		}
	}

	public virtual bool SupportsArbitraryAnimations
	{
		get
		{
			return false;
		}
	}

	public virtual bool DoNotTrimImages
	{
		get
		{
			return doNotTrimImages;
		}
		set
		{
			doNotTrimImages = value;
		}
	}

	public Texture2D[] SourceTextures
	{
		get
		{
			return sourceTextures;
		}
	}

	public CSpriteFrame[] SpriteFrames
	{
		get
		{
			return spriteFrames;
		}
	}

	public override Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader)
	{
		TextureAnim defaultState = DefaultState;
		CSpriteFrame defaultFrame = DefaultFrame;
		if (defaultState == null)
		{
			return Vector2.zero;
		}
		if (defaultState.frameGUIDs == null)
		{
			return Vector2.zero;
		}
		if (defaultState.frameGUIDs.Length == 0)
		{
			return Vector2.zero;
		}
		if (defaultFrame == null)
		{
			Debug.LogWarning("Sprite \"" + base.name + "\" does not seem to have been built to an atlas yet.");
			return Vector2.zero;
		}
		Vector2 result = Vector2.zero;
		Texture2D texture2D = (Texture2D)loader(guid2Path(defaultState.frameGUIDs[0]), typeof(Texture2D));
		if (texture2D == null)
		{
			if (base.spriteMesh != null)
			{
				texture2D = (Texture2D)base.spriteMesh.material.GetTexture("_MainTex");
				result = new Vector2(defaultFrame.uvs.width * (float)texture2D.width, defaultFrame.uvs.height * (float)texture2D.height);
			}
		}
		else
		{
			result = new Vector2((float)texture2D.width * (1f / (defaultFrame.scaleFactor.x * 2f)), (float)texture2D.height * (1f / (defaultFrame.scaleFactor.y * 2f)));
		}
		return result;
	}

	protected override void Awake()
	{
		base.Awake();
		animations = new UVAnimation[States.Length];
		for (int i = 0; i < States.Length; i++)
		{
			animations[i] = new UVAnimation();
			animations[i].SetAnim(States[i], i);
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

	public void Setup(float w, float h)
	{
		Setup(w, h, m_spriteMesh.material);
	}

	public void Setup(float w, float h, Material material)
	{
		width = w;
		height = h;
		if (!managed)
		{
			((SpriteMesh)m_spriteMesh).material = material;
		}
		Init();
	}

	public override void Copy(SpriteRoot s)
	{
		base.Copy(s);
		if (!(s is AutoSpriteBase))
		{
			return;
		}
		AutoSpriteBase autoSpriteBase = (AutoSpriteBase)s;
		if (autoSpriteBase.spriteMesh != null)
		{
			if (autoSpriteBase.animations.Length > 0)
			{
				animations = new UVAnimation[autoSpriteBase.animations.Length];
				for (int i = 0; i < animations.Length; i++)
				{
					animations[i] = autoSpriteBase.animations[i].Clone();
				}
			}
		}
		else if (States != null)
		{
			animations = new UVAnimation[autoSpriteBase.States.Length];
			for (int j = 0; j < autoSpriteBase.States.Length; j++)
			{
				animations[j] = new UVAnimation();
				animations[j].SetAnim(autoSpriteBase.States[j], j);
			}
		}
	}

	public virtual void CopyAll(SpriteRoot s)
	{
		base.Copy(s);
		if (s is AutoSpriteBase)
		{
			AutoSpriteBase autoSpriteBase = (AutoSpriteBase)s;
			States = new TextureAnim[autoSpriteBase.States.Length];
			for (int i = 0; i < States.Length; i++)
			{
				States[i] = new TextureAnim();
				States[i].Copy(autoSpriteBase.States[i]);
			}
			animations = new UVAnimation[States.Length];
			for (int j = 0; j < States.Length; j++)
			{
				animations[j] = new UVAnimation();
				animations[j].SetAnim(States[j], j);
			}
			doNotTrimImages = autoSpriteBase.doNotTrimImages;
		}
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
			int curPosition = curAnim.GetCurPosition();
			int stepDirection = curAnim.StepDirection;
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
			curAnim.SetCurrentFrame(curPosition);
			curAnim.StepDirection = stepDirection;
			SetColor(new Color(1f, 1f, 1f, 1f - framesToAdvance));
		}
		uvRect = frameInfo.uvs;
		SetBleedCompensation();
		if (autoResize || pixelPerfect)
		{
			CalcSize();
		}
		else if (anchor == ANCHOR_METHOD.TEXTURE_OFFSET)
		{
			SetSize(width, height);
		}
		return true;
	}

	public void PlayAnim(UVAnimation anim, int frame)
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
		curAnim.SetCurrentFrame(frame - 1);
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

	public void PlayAnim(UVAnimation anim)
	{
		PlayAnim(anim, 0);
	}

	public void PlayAnim(int index, int frame)
	{
		if (index >= animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
		}
		else
		{
			PlayAnim(animations[index], frame);
		}
	}

	public override void PlayAnim(int index)
	{
		if (index >= animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
		}
		else
		{
			PlayAnim(animations[index], 0);
		}
	}

	public void PlayAnim(string name, int frame)
	{
		for (int i = 0; i < animations.Length; i++)
		{
			if (animations[i].name == name)
			{
				PlayAnim(animations[i], frame);
				return;
			}
		}
		Debug.LogError("ERROR: Animation \"" + name + "\" not found!");
	}

	public override void PlayAnim(string name)
	{
		PlayAnim(name, 0);
	}

	public void PlayAnimInReverse(UVAnimation anim)
	{
		if (deleted || !base.gameObject.active)
		{
			return;
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

	public void PlayAnimInReverse(UVAnimation anim, int frame)
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
		curAnim.SetCurrentFrame(frame + 1);
		anim.framerate = Mathf.Max(0.0001f, anim.framerate);
		timeBetweenAnimFrames = 1f / anim.framerate;
		timeSinceLastFrame = timeBetweenAnimFrames;
		if (anim.GetFrameCount() > 1)
		{
			StepAnim(0f);
			if (!animating)
			{
				AddToAnimatedList();
			}
		}
		else
		{
			if (animCompleteDelegate != null)
			{
				animCompleteDelegate(this);
			}
			StepAnim(0f);
		}
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

	public void PlayAnimInReverse(int index, int frame)
	{
		if (index >= animations.Length)
		{
			Debug.LogError("ERROR: Animation index " + index + " is out of bounds!");
		}
		else
		{
			PlayAnimInReverse(animations[index], frame);
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

	public void PlayAnimInReverse(string name, int frame)
	{
		for (int i = 0; i < animations.Length; i++)
		{
			if (animations[i].name == name)
			{
				animations[i].PlayInReverse();
				PlayAnimInReverse(animations[i], frame);
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
		else if (curAnim.index != index || !animating)
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

	public void DoAnim(UVAnimation anim)
	{
		if (curAnim != anim || !animating)
		{
			PlayAnim(anim);
		}
	}

	public void SetCurFrame(int index)
	{
		if (curAnim != null)
		{
			if (!m_started)
			{
				Start();
			}
			curAnim.SetCurrentFrame(index - curAnim.StepDirection);
			timeSinceLastFrame = timeBetweenAnimFrames;
			StepAnim(0f);
		}
	}

	public void SetFrame(UVAnimation anim, int frameNum)
	{
		PlayAnim(anim);
		if (IsAnimating())
		{
			PauseAnim();
		}
		SetCurFrame(frameNum);
	}

	public void SetFrame(string anim, int frameNum)
	{
		PlayAnim(anim);
		if (IsAnimating())
		{
			PauseAnim();
		}
		SetCurFrame(frameNum);
	}

	public void SetFrame(int anim, int frameNum)
	{
		PlayAnim(anim);
		if (IsAnimating())
		{
			PauseAnim();
		}
		SetCurFrame(frameNum);
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

	public UVAnimation GetCurAnim()
	{
		return curAnim;
	}

	public UVAnimation GetAnim(string name)
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

	public virtual Material GetPackedMaterial(out string errString)
	{
		errString = "Sprite \"" + base.name + "\" has not been assigned a material, and can therefore not be included in the atlas build.";
		if (base.spriteMesh == null)
		{
			if (managed)
			{
				if (manager != null)
				{
					return manager.renderer.sharedMaterial;
				}
				errString = "Sprite \"" + base.name + "\" is not associated with a SpriteManager, and can therefore not be included in the atlas build.";
				return null;
			}
			return base.renderer.sharedMaterial;
		}
		if (managed)
		{
			if (manager != null)
			{
				return manager.renderer.sharedMaterial;
			}
			errString = "Sprite \"" + base.name + "\" is not associated with a SpriteManager, and can therefore not be included in the atlas build.";
			return null;
		}
		return base.spriteMesh.material;
	}

	public virtual void Aggregate(PathFromGUIDDelegate guid2Path, LoadAssetDelegate load, GUIDFromPathDelegate path2Guid)
	{
		ArrayList arrayList = new ArrayList();
		ArrayList arrayList2 = new ArrayList();
		for (int i = 0; i < States.Length; i++)
		{
			States[i].Allocate();
			if (States[i].frameGUIDs.Length >= States[i].framePaths.Length)
			{
				for (int j = 0; j < States[i].frameGUIDs.Length; j++)
				{
					string path = guid2Path(States[i].frameGUIDs[j]);
					arrayList.Add(load(path, typeof(Texture2D)));
					arrayList2.Add(States[i].spriteFrames[j]);
				}
				States[i].framePaths = new string[0];
				continue;
			}
			States[i].frameGUIDs = new string[States[i].framePaths.Length];
			States[i].spriteFrames = new CSpriteFrame[States[i].framePaths.Length];
			for (int k = 0; k < States[i].spriteFrames.Length; k++)
			{
				States[i].spriteFrames[k] = new CSpriteFrame();
			}
			for (int l = 0; l < States[i].framePaths.Length; l++)
			{
				States[i].frameGUIDs[l] = path2Guid(States[i].framePaths[l]);
				arrayList.Add(load(States[i].framePaths[l], typeof(Texture2D)));
				arrayList2.Add(States[i].spriteFrames[l]);
			}
		}
		sourceTextures = (Texture2D[])arrayList.ToArray(typeof(Texture2D));
		spriteFrames = (CSpriteFrame[])arrayList2.ToArray(typeof(CSpriteFrame));
	}

	virtual void ISpriteAggregator.SetUVs(Rect uvs)
	{
		SetUVs(uvs);
	}

	virtual GameObject ISpriteAggregator.get_gameObject()
	{
		return base.gameObject;
	}

	virtual GameObject ISpritePackable.get_gameObject()
	{
		return base.gameObject;
	}

	virtual ANCHOR_METHOD ISpritePackable.get_Anchor()
	{
		return base.Anchor;
	}

	virtual Color ISpritePackable.get_Color()
	{
		return base.Color;
	}

	virtual void ISpritePackable.set_Color(Color value)
	{
		base.Color = value;
	}
}
