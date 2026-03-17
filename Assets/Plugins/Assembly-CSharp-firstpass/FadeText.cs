using UnityEngine;

public class FadeText : EZAnimation
{
	protected Color start;

	protected Color delta;

	protected Color end;

	protected SpriteText text;

	protected Color temp;

	public FadeText()
	{
		type = ANIM_TYPE.FadeText;
	}

	public override object GetSubject()
	{
		return text;
	}

	public override void _end()
	{
		if (text != null)
		{
			text.SetColor(end);
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
		if (base.Mode == ANIM_MODE.By && text != null)
		{
			start = text.color;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (text == null)
		{
			_stop();
			return;
		}
		temp.r = interpolator(timeElapsed, start.r, delta.r, interval);
		temp.g = interpolator(timeElapsed, start.g, delta.g, interval);
		temp.b = interpolator(timeElapsed, start.b, delta.b, interval);
		temp.a = interpolator(timeElapsed, start.a, delta.a, interval);
		text.SetColor(temp);
	}

	public static FadeText Do(SpriteText txt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeText fadeText = (FadeText)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeText);
		fadeText.Start(txt, mode, begin, dest, interp, dur, delay, startDel, del);
		return fadeText;
	}

	public static FadeText Do(SpriteText txt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeText fadeText = (FadeText)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeText);
		fadeText.Start(txt, mode, dest, interp, dur, delay, startDel, del);
		return fadeText;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
		{
			return false;
		}
		text = (SpriteText)sub.GetComponent(typeof(SpriteText));
		if (text == null)
		{
			return false;
		}
		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;
		if (parms.mode == ANIM_MODE.FromTo)
		{
			Start(text, parms.mode, parms.color, parms.color2, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}
		else
		{
			Start(text, parms.mode, text.color, parms.color, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}
		return true;
	}

	public void Start(SpriteText txt, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Start(txt, mode, txt.color, dest, interp, dur, delay, startDel, del);
	}

	public void Start(SpriteText txt, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		text = txt;
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
			text.SetColor(start);
		}
		EZAnimator.instance.AddAnimation(this);
	}
}
