using System;
using UnityEngine;

[Serializable]
public class SuperSprite : MonoBehaviour
{
	public delegate void AnimCompleteDelegate(SuperSprite sprite);

	public bool playDefaultAnimOnStart;

	public int defaultAnim;

	public SuperSpriteAnim[] animations = new SuperSpriteAnim[0];

	protected SuperSpriteAnim curAnim;

	protected bool animating;

	protected AnimCompleteDelegate animCompleteDelegate;

	protected SpriteBase.AnimFrameDelegate animFrameDelegate;

	protected bool m_started;

	public SpriteRoot CurrentSprite
	{
		get
		{
			if (curAnim == null)
			{
				return null;
			}
			return curAnim.CurrentSprite;
		}
	}

	public void Start()
	{
		if (m_started)
		{
			return;
		}
		m_started = true;
		for (int i = 0; i < animations.Length; i++)
		{
			if (animations[i] != null)
			{
				animations[i].Init(i, AnimFinished, animFrameDelegate);
			}
		}
		if (playDefaultAnimOnStart)
		{
			PlayAnim(animations[defaultAnim]);
		}
	}

	public void PlayAnim(SuperSpriteAnim anim)
	{
		if (!m_started)
		{
			Start();
		}
		if (curAnim != null)
		{
			curAnim.Hide(true);
		}
		curAnim = anim;
		curAnim.Reset();
		animating = true;
		anim.Play();
	}

	public void PlayAnim(int index)
	{
		if (index >= 0 && index < animations.Length)
		{
			PlayAnim(animations[index]);
		}
	}

	public void PlayAnim(string anim)
	{
		for (int i = 0; i < animations.Length; i++)
		{
			if (animations[i].name == anim)
			{
				PlayAnim(animations[i]);
				break;
			}
		}
	}

	public void DoAnim(SuperSpriteAnim anim)
	{
		if ((curAnim != anim) | !animating)
		{
			PlayAnim(anim);
		}
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

	public void StopAnim()
	{
		if (curAnim != null)
		{
			curAnim.Stop();
		}
		animating = false;
	}

	public void PauseAnim()
	{
		if (curAnim != null)
		{
			curAnim.Pause();
		}
		animating = false;
	}

	public void UnpauseAnim()
	{
		if (curAnim != null)
		{
			curAnim.Unpause();
			animating = true;
		}
	}

	public void Hide(bool tf)
	{
		if (curAnim == null)
		{
			return;
		}
		curAnim.Hide(tf);
		if (!tf)
		{
			if (animating)
			{
				curAnim.Pause();
			}
		}
		else if (animating)
		{
			curAnim.Unpause();
		}
	}

	public bool IsHidden()
	{
		if (curAnim == null)
		{
			return false;
		}
		return curAnim.IsHidden();
	}

	public SuperSpriteAnim GetCurAnim()
	{
		return curAnim;
	}

	public SuperSpriteAnim GetAnim(int index)
	{
		if (index < 0 || index >= animations.Length)
		{
			return null;
		}
		return animations[index];
	}

	public SuperSpriteAnim GetAnim(string name)
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

	public bool IsAnimating()
	{
		return animating;
	}

	protected void AnimFinished(SuperSpriteAnim anim)
	{
		animating = false;
		if (animCompleteDelegate != null)
		{
			animCompleteDelegate(this);
		}
		if (curAnim == null)
		{
			return;
		}
		switch (curAnim.onAnimEnd)
		{
		case SuperSpriteAnim.ANIM_END_ACTION.Play_Default_Anim:
			PlayAnim(defaultAnim);
			break;
		case SuperSpriteAnim.ANIM_END_ACTION.Deactivate:
			base.gameObject.SetActiveRecursively(false);
			break;
		case SuperSpriteAnim.ANIM_END_ACTION.Destroy:
		{
			for (int i = 0; i < animations.Length; i++)
			{
				animations[i].Delete();
			}
			UnityEngine.Object.Destroy(base.gameObject);
			break;
		}
		}
	}

	public void SetAnimCompleteDelegate(AnimCompleteDelegate del)
	{
		animCompleteDelegate = del;
	}

	public void SetAnimFrameDelegate(SpriteBase.AnimFrameDelegate del)
	{
		animFrameDelegate = del;
		for (int i = 0; i < animations.Length; i++)
		{
			animations[i].SetAnimFrameDelegate(animFrameDelegate);
		}
	}
}
