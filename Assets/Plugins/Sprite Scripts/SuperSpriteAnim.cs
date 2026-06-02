using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SuperSpriteAnim
{
	public enum ANIM_END_ACTION
	{
		Do_Nothing = 0,
		Play_Default_Anim = 1,
		Deactivate = 2,
		Destroy = 3
	}

	public delegate void AnimCompletedDelegate(SuperSpriteAnim anim);

	[HideInInspector]
	public int index;

	public string name;

	public SuperSpriteAnimElement[] spriteAnims = new SuperSpriteAnimElement[0];

	public int loopCycles;

	public bool pingPong;

	public ANIM_END_ACTION onAnimEnd;

	public bool deactivateNonPlaying;

	public bool deactivateRecursively;

	protected int curAnim;

	protected int stepDir = 1;

	protected int numLoops;

	protected bool isRunning;

	private AnimCompletedDelegate endDelegate;

	public bool IsRunning
	{
		get
		{
			return isRunning;
		}
	}

	public SpriteBase CurrentSprite
	{
		get
		{
			if (curAnim < 0 || curAnim >= spriteAnims.Length)
			{
				return null;
			}
			return spriteAnims[curAnim].sprite;
		}
	}

	public void Init(int idx, AnimCompletedDelegate del, SpriteBase.AnimFrameDelegate frameDel)
	{
		endDelegate = del;
		index = idx;
		List<SuperSpriteAnimElement> list = new List<SuperSpriteAnimElement>();
		for (int i = 0; i < spriteAnims.Length; i++)
		{
			if (spriteAnims[i] != null && spriteAnims[i].sprite != null)
			{
				spriteAnims[i].Init();
				list.Add(spriteAnims[i]);
				if (frameDel != null)
				{
					spriteAnims[i].sprite.SetAnimFrameDelegate(frameDel);
				}
				HideSprite(spriteAnims[i].sprite, true);
			}
		}
		spriteAnims = list.ToArray();
	}

	public void SetAnimFrameDelegate(SpriteBase.AnimFrameDelegate frameDel)
	{
		for (int i = 0; i < spriteAnims.Length; i++)
		{
			if (spriteAnims[i] != null && spriteAnims[i].sprite != null)
			{
				spriteAnims[i].sprite.SetAnimFrameDelegate(frameDel);
			}
		}
	}

	private void AnimFinished(SpriteBase sp)
	{
		if (curAnim + stepDir >= spriteAnims.Length || curAnim + stepDir < 0)
		{
			if (stepDir > 0 && pingPong)
			{
				stepDir = -1;
				((AutoSpriteBase)sp).PlayAnimInReverse(spriteAnims[curAnim].anim, spriteAnims[curAnim].anim.GetFrameCount() - 2);
				return;
			}
			if (numLoops + 1 > loopCycles && loopCycles != -1)
			{
				isRunning = false;
				sp.SetAnimCompleteDelegate(null);
				if (endDelegate != null)
				{
					endDelegate(this);
				}
				return;
			}
			numLoops++;
			if (pingPong)
			{
				spriteAnims[curAnim].sprite.PlayAnim(spriteAnims[curAnim].anim, 1);
				stepDir *= -1;
				return;
			}
			HideSprite(sp, true);
			sp.SetAnimCompleteDelegate(null);
			curAnim = 0;
		}
		else
		{
			sp.SetAnimCompleteDelegate(null);
			HideSprite(sp, true);
			curAnim += stepDir;
		}
		HideSprite(spriteAnims[curAnim].sprite, false);
		spriteAnims[curAnim].sprite.SetAnimCompleteDelegate(AnimFinished);
		if (stepDir > 0)
		{
			spriteAnims[curAnim].Play();
		}
		else
		{
			spriteAnims[curAnim].PlayInReverse();
		}
	}

	public void Reset()
	{
		Stop();
		curAnim = 0;
		stepDir = 1;
		numLoops = 0;
		for (int i = 1; i < spriteAnims.Length; i++)
		{
			HideSprite(spriteAnims[i].sprite, true);
		}
	}

	public void Play()
	{
		isRunning = true;
		spriteAnims[curAnim].sprite.SetAnimCompleteDelegate(AnimFinished);
		HideSprite(spriteAnims[curAnim].sprite, false);
		spriteAnims[curAnim].Play();
	}

	public void Stop()
	{
		isRunning = false;
		spriteAnims[curAnim].sprite.StopAnim();
		spriteAnims[curAnim].sprite.SetAnimCompleteDelegate(null);
	}

	public void Pause()
	{
		isRunning = false;
		spriteAnims[curAnim].sprite.PauseAnim();
	}

	public void Unpause()
	{
		isRunning = true;
		spriteAnims[curAnim].sprite.UnpauseAnim();
	}

	public void Hide(bool tf)
	{
		if (curAnim >= 0 && curAnim < spriteAnims.Length)
		{
			HideSprite(spriteAnims[curAnim].sprite, tf);
		}
	}

	public bool IsHidden()
	{
		if (curAnim < 0 || curAnim >= spriteAnims.Length)
		{
			return false;
		}
		return spriteAnims[curAnim].sprite.IsHidden();
	}

	protected void HideSprite(SpriteBase sp, bool tf)
	{
		if (deactivateNonPlaying)
		{
			if (deactivateRecursively)
			{
				sp.gameObject.SetActiveRecursively(!tf);
			}
			else
			{
				sp.gameObject.active = !tf;
			}
		}
		else
		{
			sp.Hide(tf);
		}
	}

	public void Delete()
	{
		for (int i = 0; i < spriteAnims.Length; i++)
		{
			if (spriteAnims[i].sprite != null)
			{
				spriteAnims[i].sprite.Delete();
				UnityEngine.Object.Destroy(spriteAnims[i].sprite);
			}
		}
	}
}
