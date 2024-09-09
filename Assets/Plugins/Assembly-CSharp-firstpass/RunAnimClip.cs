using UnityEngine;

public class RunAnimClip : EZAnimation
{
	protected GameObject subject;

	protected string m_clip;

	protected bool waitForClip = true;

	protected bool playedYet;

	protected float blending;

	public RunAnimClip()
	{
		type = ANIM_TYPE.AnimClip;
		pingPong = false;
	}

	public override object GetSubject()
	{
		return subject;
	}

	public override bool Step(float timeDelta)
	{
		if (wait > 0f)
		{
			wait -= timeDelta;
			if (!(wait < 0f))
			{
				return true;
			}
			timeDelta -= timeDelta + wait;
		}
		if (!playedYet)
		{
			if (duration == 0f && blending == 0f)
			{
				subject.animation.Play(m_clip);
			}
			else
			{
				subject.animation.Blend(m_clip, blending, duration);
			}
			playedYet = true;
			return true;
		}
		if (subject.animation.IsPlaying(m_clip))
		{
			return true;
		}
		_end();
		return false;
	}

	protected override void DoAnim()
	{
	}

	public static RunAnimClip Do(GameObject sub, string clip, float blend, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		RunAnimClip runAnimClip = (RunAnimClip)EZAnimator.instance.GetAnimation(ANIM_TYPE.AnimClip);
		runAnimClip.Start(sub, clip, blend, dur, delay, startDel, del);
		return runAnimClip;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
		{
			return false;
		}
		if (sub.animation == null)
		{
			return false;
		}
		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;
		Start(sub, parms.strVal, parms.floatVal, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	public void Start(GameObject sub, string clip, float blend, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (!(sub == null) && !(sub.animation == null))
		{
			playedYet = false;
			subject = sub;
			m_clip = clip;
			blending = blend;
			duration = dur;
			m_wait = delay;
			completedDelegate = del;
			startDelegate = startDel;
			StartCommon();
			EZAnimator.instance.AddAnimation(this);
		}
	}
}
