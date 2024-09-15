using UnityEngine;

public class FadeSprite : EZAnimation
{
	protected Color start;

	protected Color delta;

	protected Color end;

	protected SpriteRoot sprite;

	protected Color temp;

	public FadeSprite()
	{
		type = ANIM_TYPE.FadeSprite;
	}

	public override object GetSubject()
	{
		return sprite;
	}

	public override void _end()
	{
		if (sprite != null)
		{
			sprite.SetColor(end);
		}
		base._end();
	}

	protected override void LoopReset()
	{
		if (base.Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end = start + delta;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();
		if (base.Mode == ANIM_MODE.By && sprite != null)
		{
			start = sprite.color;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (sprite == null)
		{
			_stop();
			return;
		}
		temp.r = interpolator(timeElapsed, start.r, delta.r, interval);
		temp.g = interpolator(timeElapsed, start.g, delta.g, interval);
		temp.b = interpolator(timeElapsed, start.b, delta.b, interval);
		temp.a = interpolator(timeElapsed, start.a, delta.a, interval);
		sprite.SetColor(temp);
	}

	public static FadeSprite Do(SpriteRoot sprt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeSprite fadeSprite = (FadeSprite)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeSprite);
		fadeSprite.Start(sprt, mode, begin, dest, interp, dur, delay, startDel, del);
		return fadeSprite;
	}

	public static FadeSprite Do(SpriteRoot sprt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeSprite fadeSprite = (FadeSprite)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeSprite);
		fadeSprite.Start(sprt, mode, dest, interp, dur, delay, startDel, del);
		return fadeSprite;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
		{
			return false;
		}
		sprite = (SpriteRoot)sub.GetComponent(typeof(SpriteRoot));
		if (sprite == null)
		{
			return false;
		}
		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;
		if (parms.mode == ANIM_MODE.FromTo)
		{
			Start(sprite, parms.mode, parms.color, parms.color2, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}
		else
		{
			Start(sprite, parms.mode, sprite.color, parms.color, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}
		return true;
	}

	public void Start(SpriteRoot sprt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Start(sprt, mode, sprt.color, dest, interp, dur, delay, startDel, del);
	}

	public void Start(SpriteRoot sprt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		sprite = sprt;
		start = begin;
		m_mode = mode;
		if (mode == ANIM_MODE.By)
		{
			delta = dest;
		}
		else
		{
			delta = new Color(dest.r - start.r, dest.g - start.g, dest.b - start.b, dest.a - start.a);
		}
		end = start + delta;
		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;
		StartCommon();
		if (mode == ANIM_MODE.FromTo && delay == 0f)
		{
			sprite.SetColor(start);
		}
		EZAnimator.instance.AddAnimation(this);
	}

	public void Start()
	{
		if (!(sprite == null))
		{
			direction = 1f;
			timeElapsed = 0f;
			wait = m_wait;
			if (m_mode == ANIM_MODE.By)
			{
				start = sprite.color;
				end = start + delta;
			}
			EZAnimator.instance.AddAnimation(this);
		}
	}
}
