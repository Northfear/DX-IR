using UnityEngine;

public class FadeAudio : EZAnimation
{
	protected float start;

	protected AudioSource subject;

	protected float delta;

	protected float end;

	public FadeAudio()
	{
		type = ANIM_TYPE.FadeAudio;
		pingPong = false;
	}

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subject != null)
		{
			subject.volume = end;
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
		if (base.Mode == ANIM_MODE.By && subject != null)
		{
			start = subject.volume;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (subject == null)
		{
			_stop();
		}
		else
		{
			subject.volume = interpolator(timeElapsed, start, delta, interval);
		}
	}

	public static FadeAudio Do(AudioSource audio, ANIM_MODE mode, float begin, float dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeAudio fadeAudio = (FadeAudio)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeAudio);
		fadeAudio.Start(audio, mode, begin, dest, interp, dur, delay, startDel, del);
		return fadeAudio;
	}

	public static FadeAudio Do(AudioSource audio, ANIM_MODE mode, float dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeAudio fadeAudio = (FadeAudio)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeAudio);
		fadeAudio.Start(audio, mode, dest, interp, dur, delay, startDel, del);
		return fadeAudio;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
		{
			return false;
		}
		subject = (AudioSource)sub.GetComponent(typeof(AudioSource));
		if (subject == null)
		{
			return false;
		}
		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;
		if (parms.mode == ANIM_MODE.FromTo)
		{
			Start(subject, parms.mode, parms.floatVal, parms.floatVal2, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}
		else
		{
			Start(subject, parms.mode, subject.volume, parms.floatVal, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}
		return true;
	}

	public void Start(AudioSource audio, ANIM_MODE mode, float dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Start(audio, mode, audio.volume, dest, interp, dur, delay, startDel, del);
	}

	public void Start(AudioSource sub, ANIM_MODE mode, float begin, float dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		start = begin;
		m_mode = mode;
		if (mode == ANIM_MODE.By)
		{
			delta = dest;
		}
		else
		{
			delta = dest - start;
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
			subject.volume = start;
		}
		EZAnimator.instance.AddAnimation(this);
	}
}
