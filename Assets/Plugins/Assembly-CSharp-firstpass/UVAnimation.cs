using System;
using UnityEngine;

[Serializable]
public class UVAnimation
{
	public enum ANIM_END_ACTION
	{
		Do_Nothing = 0,
		Revert_To_Static = 1,
		Play_Default_Anim = 2,
		Hide = 3,
		Deactivate = 4,
		Destroy = 5
	}

	protected SPRITE_FRAME[] frames;

	protected int curFrame = -1;

	protected int stepDir = 1;

	protected int numLoops;

	protected bool playInReverse;

	protected float length;

	public string name;

	public int loopCycles;

	public bool loopReverse;

	[HideInInspector]
	public float framerate = 15f;

	[HideInInspector]
	public int index = -1;

	[HideInInspector]
	public ANIM_END_ACTION onAnimEnd;

	public int StepDirection
	{
		get
		{
			return stepDir;
		}
		set
		{
			SetStepDir(value);
		}
	}

	public UVAnimation(UVAnimation anim)
	{
		frames = new SPRITE_FRAME[anim.frames.Length];
		anim.frames.CopyTo(frames, 0);
		name = anim.name;
		loopCycles = anim.loopCycles;
		loopReverse = anim.loopReverse;
		framerate = anim.framerate;
		onAnimEnd = anim.onAnimEnd;
		curFrame = anim.curFrame;
		stepDir = anim.stepDir;
		numLoops = anim.numLoops;
		playInReverse = anim.playInReverse;
		length = anim.length;
		CalcLength();
	}

	public UVAnimation()
	{
		frames = new SPRITE_FRAME[0];
	}

	public UVAnimation Clone()
	{
		return new UVAnimation(this);
	}

	public void Reset()
	{
		curFrame = -1;
		stepDir = 1;
		numLoops = 0;
		playInReverse = false;
	}

	public void PlayInReverse()
	{
		stepDir = -1;
		curFrame = frames.Length;
		numLoops = 0;
		playInReverse = true;
	}

	public void SetStepDir(int dir)
	{
		if (dir < 0)
		{
			stepDir = -1;
			playInReverse = true;
		}
		else
		{
			stepDir = 1;
		}
	}

	public bool GetNextFrame(ref SPRITE_FRAME nextFrame)
	{
		if (frames.Length < 1)
		{
			return false;
		}
		if (curFrame + stepDir >= frames.Length || curFrame + stepDir < 0)
		{
			if (stepDir > 0 && loopReverse)
			{
				stepDir = -1;
				curFrame += stepDir;
				curFrame = Mathf.Clamp(curFrame, 0, frames.Length - 1);
				nextFrame = frames[curFrame];
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
					curFrame += stepDir;
					curFrame = Mathf.Clamp(curFrame, 0, frames.Length - 1);
				}
				else if (playInReverse)
				{
					curFrame = frames.Length - 1;
				}
				else
				{
					curFrame = 0;
				}
				nextFrame = frames[curFrame];
			}
		}
		else
		{
			curFrame += stepDir;
			nextFrame = frames[curFrame];
		}
		return true;
	}

	public SPRITE_FRAME GetCurrentFrame()
	{
		return frames[Mathf.Clamp(curFrame, 0, curFrame)];
	}

	public SPRITE_FRAME GetFrame(int frame)
	{
		return frames[frame];
	}

	public SPRITE_FRAME[] BuildUVAnim(Vector2 start, Vector2 cellSize, int cols, int rows, int totalCells)
	{
		int num = 0;
		frames = new SPRITE_FRAME[totalCells];
		frames[0] = new SPRITE_FRAME(0f);
		frames[0].uvs.x = start.x;
		frames[0].uvs.y = start.y;
		frames[0].uvs.xMax = start.x + cellSize.x;
		frames[0].uvs.yMax = start.y + cellSize.y;
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				if (num >= totalCells)
				{
					break;
				}
				frames[num] = new SPRITE_FRAME(0f);
				frames[num].uvs.x = start.x + cellSize.x * (float)j;
				frames[num].uvs.y = start.y - cellSize.y * (float)i;
				frames[num].uvs.xMax = frames[num].uvs.x + cellSize.x;
				frames[num].uvs.yMax = frames[num].uvs.y + cellSize.y;
				num++;
			}
		}
		CalcLength();
		return frames;
	}

	public SPRITE_FRAME[] BuildWrappedUVAnim(Vector2 start, Vector2 cellSize, int cols, int rows, int totalCells)
	{
		return BuildWrappedUVAnim(start, cellSize, totalCells);
	}

	public SPRITE_FRAME[] BuildWrappedUVAnim(Vector2 start, Vector2 cellSize, int totalCells)
	{
		int num = 0;
		frames = new SPRITE_FRAME[totalCells];
		frames[0] = new SPRITE_FRAME(0f);
		frames[0].uvs.x = start.x;
		frames[0].uvs.y = start.y;
		frames[0].uvs.xMax = start.x + cellSize.x;
		frames[0].uvs.yMax = start.y + cellSize.y;
		Vector2 vector = start;
		for (num = 1; num < totalCells; num++)
		{
			vector.x += cellSize.x;
			if (vector.x + cellSize.x > 1.01f)
			{
				vector.x = 0f;
				vector.y -= cellSize.y;
			}
			frames[num] = new SPRITE_FRAME(0f);
			frames[num].uvs.x = vector.x;
			frames[num].uvs.y = vector.y;
			frames[num].uvs.xMax = vector.x + cellSize.x;
			frames[num].uvs.yMax = vector.y + cellSize.y;
		}
		return frames;
	}

	public void SetAnim(SPRITE_FRAME[] anim)
	{
		frames = anim;
		CalcLength();
	}

	public void SetAnim(TextureAnim anim, int idx)
	{
		if (anim == null || anim.spriteFrames == null)
		{
			return;
		}
		frames = new SPRITE_FRAME[anim.spriteFrames.Length];
		index = idx;
		name = anim.name;
		loopCycles = anim.loopCycles;
		loopReverse = anim.loopReverse;
		framerate = anim.framerate;
		onAnimEnd = anim.onAnimEnd;
		try
		{
			for (int i = 0; i < frames.Length; i++)
			{
				frames[i] = anim.spriteFrames[i].ToStruct();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception caught in UVAnimation.SetAnim(). Make sure you have re-built your atlases!\nException: " + ex.Message);
		}
		CalcLength();
	}

	public void AppendAnim(SPRITE_FRAME[] anim)
	{
		SPRITE_FRAME[] array = frames;
		frames = new SPRITE_FRAME[frames.Length + anim.Length];
		array.CopyTo(frames, 0);
		anim.CopyTo(frames, array.Length);
		CalcLength();
	}

	public void SetCurrentFrame(int f)
	{
		f = Mathf.Clamp(f, -1, frames.Length + 1);
		curFrame = f;
	}

	public void SetPosition(float pos)
	{
		pos = Mathf.Clamp01(pos);
		if (loopCycles < 1)
		{
			SetClipPosition(pos);
			return;
		}
		float num = 1f / ((float)loopCycles + 1f);
		numLoops = Mathf.FloorToInt(pos / num);
		float num2 = pos - (float)numLoops * num;
		float num3 = num2 / num;
		if (loopReverse)
		{
			if (num3 < 0.5f)
			{
				curFrame = (int)(((float)frames.Length - 1f) * (num3 / 0.5f));
				stepDir = 1;
			}
			else
			{
				curFrame = frames.Length - 1 - (int)(((float)frames.Length - 1f) * ((num3 - 0.5f) / 0.5f));
				stepDir = -1;
			}
		}
		else
		{
			curFrame = (int)(((float)frames.Length - 1f) * num3);
		}
	}

	public void SetClipPosition(float pos)
	{
		curFrame = (int)(((float)frames.Length - 1f) * pos);
	}

	protected void CalcLength()
	{
		length = 1f / framerate * (float)frames.Length;
	}

	public float GetLength()
	{
		return length;
	}

	public float GetDuration()
	{
		if (loopCycles < 0)
		{
			return -1f;
		}
		float num = GetLength();
		if (loopReverse)
		{
			num *= 2f;
		}
		return num + (float)loopCycles * num;
	}

	public int GetFrameCount()
	{
		return frames.Length;
	}

	public int GetFramesDisplayed()
	{
		if (loopCycles == -1)
		{
			return -1;
		}
		int num = frames.Length + frames.Length * loopCycles;
		if (loopReverse)
		{
			num *= 2;
		}
		return num;
	}

	public int GetCurPosition()
	{
		return curFrame;
	}
}
