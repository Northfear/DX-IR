using System;
using UnityEngine;

[Serializable]
public class UVAnimation_Multi
{
	public string name;

	public int loopCycles;

	public bool loopReverse;

	public float framerate = 15f;

	public UVAnimation.ANIM_END_ACTION onAnimEnd;

	public UVAnimation_Auto[] clips;

	[HideInInspector]
	public int index;

	protected int curClip;

	protected int stepDir = 1;

	protected int numLoops;

	protected float duration;

	protected bool ret;

	protected int i;

	protected int framePos = -1;

	public int StepDirection
	{
		get
		{
			return stepDir;
		}
		set
		{
			stepDir = value;
		}
	}

	public UVAnimation_Multi()
	{
		if (clips == null)
		{
			clips = new UVAnimation_Auto[0];
		}
	}

	public UVAnimation_Multi(UVAnimation_Multi anim)
	{
		name = anim.name;
		loopCycles = anim.loopCycles;
		loopReverse = anim.loopReverse;
		framerate = anim.framerate;
		onAnimEnd = anim.onAnimEnd;
		curClip = anim.curClip;
		stepDir = anim.stepDir;
		numLoops = anim.numLoops;
		duration = anim.duration;
		clips = new UVAnimation_Auto[anim.clips.Length];
		for (int i = 0; i < clips.Length; i++)
		{
			clips[i] = anim.clips[i].Clone();
		}
		CalcDuration();
	}

	public UVAnimation_Multi Clone()
	{
		return new UVAnimation_Multi(this);
	}

	public UVAnimation_Auto GetCurrentClip()
	{
		return clips[curClip];
	}

	public UVAnimation_Auto[] BuildUVAnim(SpriteRoot s)
	{
		for (i = 0; i < clips.Length; i++)
		{
			clips[i].BuildUVAnim(s);
		}
		CalcDuration();
		return clips;
	}

	public bool GetNextFrame(ref SPRITE_FRAME nextFrame)
	{
		if (clips.Length < 1)
		{
			return false;
		}
		ret = clips[curClip].GetNextFrame(ref nextFrame);
		if (!ret)
		{
			if (curClip + stepDir >= clips.Length || curClip + stepDir < 0)
			{
				if (stepDir > 0 && loopReverse)
				{
					stepDir = -1;
					curClip += stepDir;
					curClip = Mathf.Clamp(curClip, 0, clips.Length - 1);
					clips[curClip].Reset();
					clips[curClip].PlayInReverse();
					clips[curClip].GetNextFrame(ref nextFrame);
				}
				else
				{
					if (numLoops + 1 > loopCycles && loopCycles != -1)
					{
						return false;
					}
					numLoops++;
					if (loopReverse)
					{
						stepDir *= -1;
						curClip += stepDir;
						curClip = Mathf.Clamp(curClip, 0, clips.Length - 1);
						clips[curClip].Reset();
						if (stepDir < 0)
						{
							clips[curClip].PlayInReverse();
						}
						clips[curClip].GetNextFrame(ref nextFrame);
					}
					else
					{
						curClip = 0;
						framePos = -1;
						clips[curClip].Reset();
					}
				}
			}
			else
			{
				curClip += stepDir;
				clips[curClip].Reset();
				if (stepDir < 0)
				{
					clips[curClip].PlayInReverse();
					clips[curClip].GetNextFrame(ref nextFrame);
				}
			}
			framePos += stepDir;
			clips[curClip].GetNextFrame(ref nextFrame);
			return true;
		}
		framePos += stepDir;
		return true;
	}

	public SPRITE_FRAME GetCurrentFrame()
	{
		return clips[Mathf.Clamp(curClip, 0, curClip)].GetCurrentFrame();
	}

	public void AppendAnim(int index, SPRITE_FRAME[] anim)
	{
		if (index < clips.Length)
		{
			clips[index].AppendAnim(anim);
			CalcDuration();
		}
	}

	public void AppendClip(UVAnimation clip)
	{
		UVAnimation[] array = clips;
		clips = new UVAnimation_Auto[clips.Length + 1];
		array.CopyTo(clips, 0);
		clips[clips.Length - 1] = (UVAnimation_Auto)clip;
		CalcDuration();
	}

	public void PlayInReverse()
	{
		for (i = 0; i < clips.Length; i++)
		{
			clips[i].PlayInReverse();
		}
		stepDir = -1;
		framePos = GetFrameCount() - 1;
		curClip = clips.Length - 1;
	}

	public void SetAnim(int index, SPRITE_FRAME[] frames)
	{
		if (index < clips.Length)
		{
			clips[index].SetAnim(frames);
			CalcDuration();
		}
	}

	public void Reset()
	{
		curClip = 0;
		stepDir = 1;
		numLoops = 0;
		framePos = -1;
		for (i = 0; i < clips.Length; i++)
		{
			clips[i].Reset();
		}
	}

	public void SetPosition(float pos)
	{
		pos = Mathf.Clamp01(pos);
		if (loopCycles < 1)
		{
			SetAnimPosition(pos);
			return;
		}
		float num = 1f / ((float)loopCycles + 1f);
		numLoops = Mathf.FloorToInt(pos / num);
		float num2 = pos - (float)numLoops * num;
		SetAnimPosition(num2 / num);
	}

	public void SetAnimPosition(float pos)
	{
		int num = 0;
		float num2 = pos;
		for (int i = 0; i < clips.Length; i++)
		{
			num += clips[i].GetFramesDisplayed();
		}
		if (loopReverse)
		{
			if (pos < 0.5f)
			{
				stepDir = 1;
				num2 *= 2f;
				for (int j = 0; j < clips.Length; j++)
				{
					float num3 = clips[j].GetFramesDisplayed() / num;
					if (num2 <= num3)
					{
						curClip = j;
						clips[curClip].SetPosition(num2 / num3);
						framePos = (int)num3 * (num - 1);
						break;
					}
					num2 -= num3;
				}
				return;
			}
			stepDir = -1;
			num2 = (num2 - 0.5f) / 0.5f;
			for (int num4 = clips.Length - 1; num4 >= 0; num4--)
			{
				float num3 = clips[num4].GetFramesDisplayed() / num;
				if (num2 <= num3)
				{
					curClip = num4;
					clips[curClip].SetPosition(1f - num2 / num3);
					clips[curClip].SetStepDir(-1);
					framePos = (int)num3 * (num - 1);
					break;
				}
				num2 -= num3;
			}
			return;
		}
		for (int k = 0; k < clips.Length; k++)
		{
			float num3 = clips[k].GetFramesDisplayed() / num;
			if (num2 <= num3)
			{
				curClip = k;
				clips[curClip].SetPosition(num2 / num3);
				framePos = (int)num3 * (num - 1);
				break;
			}
			num2 -= num3;
		}
	}

	protected void CalcDuration()
	{
		if (loopCycles < 0)
		{
			duration = -1f;
			return;
		}
		duration = 0f;
		for (int i = 0; i < clips.Length; i++)
		{
			duration += clips[i].GetDuration();
		}
		if (loopReverse)
		{
			duration *= 2f;
		}
		duration += (float)loopCycles * duration;
	}

	public float GetDuration()
	{
		return duration;
	}

	public int GetFrameCount()
	{
		int num = 0;
		for (int i = 0; i < clips.Length; i++)
		{
			num += clips[i].GetFramesDisplayed();
		}
		return num;
	}

	public int GetCurPosition()
	{
		return framePos;
	}

	public int GetCurClipNum()
	{
		return curClip;
	}

	public void SetCurClipNum(int index)
	{
		curClip = index;
	}
}
