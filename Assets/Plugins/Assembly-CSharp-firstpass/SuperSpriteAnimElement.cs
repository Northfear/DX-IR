using System;
using UnityEngine;

[Serializable]
public class SuperSpriteAnimElement
{
	public AutoSpriteBase sprite;

	public string animName;

	[HideInInspector]
	public UVAnimation anim;

	public void Init()
	{
		bool flag = false;
		if (sprite != null)
		{
			if (!sprite.gameObject.active)
			{
				flag = true;
				sprite.gameObject.active = true;
			}
			anim = sprite.GetAnim(animName);
			if (anim == null)
			{
				Debug.LogError("SuperSprite error: No animation by the name of \"" + animName + "\" was found on sprite \"" + sprite.name + "\". Please verify the spelling and capitalization of the name, including any extra spaces, etc.");
			}
			if (flag)
			{
				sprite.gameObject.active = false;
			}
		}
	}

	public void Play()
	{
		sprite.PlayAnim(anim);
	}

	public void PlayInReverse()
	{
		sprite.PlayAnimInReverse(anim);
	}
}
