using System;
using UnityEngine;

[Serializable]
public class TextureAnim
{
	public string name;

	public int loopCycles;

	public bool loopReverse;

	public float framerate = 15f;

	public UVAnimation.ANIM_END_ACTION onAnimEnd;

	[HideInInspector]
	public string[] framePaths;

	[HideInInspector]
	public string[] frameGUIDs;

	[HideInInspector]
	public CSpriteFrame[] spriteFrames;

	public TextureAnim()
	{
		Allocate();
	}

	public TextureAnim(string n)
	{
		name = n;
		Allocate();
	}

	public void Allocate()
	{
		bool flag = false;
		if (framePaths == null)
		{
			framePaths = new string[0];
		}
		if (frameGUIDs == null)
		{
			frameGUIDs = new string[0];
		}
		if (spriteFrames == null)
		{
			flag = true;
		}
		else if (spriteFrames.Length != frameGUIDs.Length)
		{
			flag = true;
		}
		if (flag)
		{
			spriteFrames = new CSpriteFrame[Mathf.Max(frameGUIDs.Length, framePaths.Length)];
			for (int i = 0; i < spriteFrames.Length; i++)
			{
				spriteFrames[i] = new CSpriteFrame();
			}
		}
	}

	public void Copy(TextureAnim a)
	{
		name = a.name;
		loopCycles = a.loopCycles;
		loopReverse = a.loopReverse;
		framerate = a.framerate;
		onAnimEnd = a.onAnimEnd;
		framePaths = new string[a.framePaths.Length];
		a.framePaths.CopyTo(framePaths, 0);
		frameGUIDs = new string[a.frameGUIDs.Length];
		a.frameGUIDs.CopyTo(frameGUIDs, 0);
		spriteFrames = new CSpriteFrame[a.spriteFrames.Length];
		for (int i = 0; i < spriteFrames.Length; i++)
		{
			spriteFrames[i] = new CSpriteFrame(a.spriteFrames[i]);
		}
	}
}
