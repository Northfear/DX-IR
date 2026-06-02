using UnityEngine;

public class AnimateScreenPosition : EZAnimation
{
	protected Vector3 start;

	protected Vector3 delta;

	protected Vector3 end;

	protected GameObject subject;

	protected EZScreenPlacement subSP;

	protected Vector3 temp;

	public AnimateScreenPosition()
	{
		type = ANIM_TYPE.TranslateScreen;
	}

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subSP != null)
		{
			subSP.screenPos = end;
			subSP.SetCamera();
			subSP.PositionOnScreen();
			subSP.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
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
		if (base.Mode == ANIM_MODE.By && subSP != null)
		{
			start = subSP.screenPos;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (subSP == null)
		{
			_stop();
			return;
		}
		temp.x = interpolator(timeElapsed, start.x, delta.x, interval);
		temp.y = interpolator(timeElapsed, start.y, delta.y, interval);
		temp.z = interpolator(timeElapsed, start.z, delta.z, interval);
		subSP.screenPos = temp;
		subSP.PositionOnScreen();
		subSP.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
	}

	public static AnimateScreenPosition Do(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimateScreenPosition animateScreenPosition = (AnimateScreenPosition)EZAnimator.instance.GetAnimation(ANIM_TYPE.TranslateScreen);
		animateScreenPosition.Start(sub, mode, begin, dest, interp, dur, delay, startDel, del);
		return animateScreenPosition;
	}

	public static AnimateScreenPosition Do(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimateScreenPosition animateScreenPosition = (AnimateScreenPosition)EZAnimator.instance.GetAnimation(ANIM_TYPE.TranslateScreen);
		animateScreenPosition.Start(sub, mode, dest, interp, dur, delay, startDel, del);
		return animateScreenPosition;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
		{
			return false;
		}
		EZScreenPlacement eZScreenPlacement = (EZScreenPlacement)sub.GetComponent(typeof(EZScreenPlacement));
		if (eZScreenPlacement == null)
		{
			Debug.LogError(string.Format("{0} has no EZScreenPlacement attached - but it's required for using AnimateScreenPosition/TranslateScreen!", sub.name));
			return false;
		}
		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;
		if (parms.mode == ANIM_MODE.FromTo)
		{
			Start(sub, parms.mode, parms.vec, parms.vec2, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}
		else
		{
			Start(sub, parms.mode, eZScreenPlacement.screenPos, parms.vec, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}
		return true;
	}

	public void Start(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (!(sub == null))
		{
			EZScreenPlacement eZScreenPlacement = (EZScreenPlacement)sub.GetComponent(typeof(EZScreenPlacement));
			if (eZScreenPlacement == null)
			{
				Debug.LogError(string.Format("{0} has no EZScreenPlacement attached - but it's required for using AnimateScreenPosition/TranslateScreen!", sub.name));
			}
			else
			{
				Start(sub, mode, eZScreenPlacement.screenPos, dest, interp, dur, delay, startDel, del);
			}
		}
	}

	public void Start(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (sub == null)
		{
			return;
		}
		EZScreenPlacement eZScreenPlacement = (EZScreenPlacement)sub.GetComponent(typeof(EZScreenPlacement));
		if (eZScreenPlacement == null)
		{
			Debug.LogError(string.Format("{0} has no EZScreenPlacement attached - but it's required for using AnimateScreenPosition/TranslateScreen!", sub.name));
			return;
		}
		subject = sub;
		subSP = eZScreenPlacement;
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
			subSP.screenPos = start;
		}
		EZAnimator.instance.AddAnimation(this);
	}

	public void Start()
	{
		if (!(subject == null))
		{
			direction = 1f;
			timeElapsed = 0f;
			wait = m_wait;
			if (m_mode == ANIM_MODE.By)
			{
				start = subject.transform.localEulerAngles;
				end = start + delta;
			}
			EZAnimator.instance.AddAnimation(this);
		}
	}
}
