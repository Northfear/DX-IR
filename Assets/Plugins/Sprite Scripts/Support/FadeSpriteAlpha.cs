using UnityEngine;

public class FadeSpriteAlpha : EZAnimation
{
	protected Color start;

	protected Color delta;

	protected Color end;

	protected SpriteRoot sprite;

	protected Color temp;

	public FadeSpriteAlpha()
	{
		type = ANIM_TYPE.FadeSpriteAlpha;
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
		temp = start;
	}

	protected override void DoAnim()
	{
		if (sprite == null)
		{
			_stop();
			return;
		}
		temp.a = interpolator(timeElapsed, start.a, delta.a, interval);
		sprite.SetColor(temp);
	}

	public static FadeSpriteAlpha Do(SpriteRoot sprt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeSpriteAlpha fadeSpriteAlpha = (FadeSpriteAlpha)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeSpriteAlpha);
		fadeSpriteAlpha.Start(sprt, mode, begin, dest, interp, dur, delay, startDel, del);
		return fadeSpriteAlpha;
	}

	public static FadeSpriteAlpha Do(SpriteRoot sprt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeSpriteAlpha fadeSpriteAlpha = (FadeSpriteAlpha)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeSpriteAlpha);
		fadeSpriteAlpha.Start(sprt, mode, dest, interp, dur, delay, startDel, del);
		return fadeSpriteAlpha;
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
		start = sprite.Color;
		start.a = begin.a;
		m_mode = mode;
		if (mode == ANIM_MODE.By)
		{
			delta = new Color(0f, 0f, 0f, dest.a);
		}
		else
		{
			delta = new Color(0f, 0f, 0f, dest.a - start.a);
		}
		end = start + delta;
		temp = start;
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
