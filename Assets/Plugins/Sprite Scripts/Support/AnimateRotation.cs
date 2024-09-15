using UnityEngine;

public class AnimateRotation : EZAnimation
{
	protected GameObject subject;

	protected Transform subTrans;

	protected Quaternion temp;

	protected Quaternion delta;

	protected Quaternion start;

	protected Quaternion end;

	public AnimateRotation()
	{
		type = ANIM_TYPE.Rotate;
	}

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localRotation = end;
			subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
		}
		base._end();
	}

	protected override void LoopReset()
	{
		if (base.Mode == ANIM_MODE.By && !restartOnRepeat)
		{
			start = end;
			end.x = start.x + delta.x;
			end.y = start.y + delta.y;
			end.z = start.z + delta.z;
			end.w = start.w + delta.w;
		}
	}

	protected override void WaitDone()
	{
		base.WaitDone();
		if (base.Mode == ANIM_MODE.By && subTrans != null)
		{
			start = subTrans.localRotation;
			end.x = start.x + delta.x;
			end.y = start.y + delta.y;
			end.z = start.z + delta.z;
			end.w = start.w + delta.w;
		}
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}
		temp = Quaternion.Slerp(start, end, interpolator(timeElapsed, 0f, 1f, interval));
		subTrans.localRotation = temp;
		subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
	}

	public static AnimateRotation Do(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimateRotation animateRotation = (AnimateRotation)EZAnimator.instance.GetAnimation(ANIM_TYPE.Rotate);
		animateRotation.Start(sub, mode, begin, dest, interp, dur, delay, startDel, del);
		return animateRotation;
	}

	public static AnimateRotation Do(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		AnimateRotation animateRotation = (AnimateRotation)EZAnimator.instance.GetAnimation(ANIM_TYPE.Rotate);
		animateRotation.Start(sub, mode, dest, interp, dur, delay, startDel, del);
		return animateRotation;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
		{
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
			Start(sub, parms.mode, sub.transform.localEulerAngles, parms.vec, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}
		return true;
	}

	public void Start(GameObject sub, ANIM_MODE mode, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (!(sub == null))
		{
			Start(sub, mode, sub.transform.localEulerAngles, dest, interp, dur, delay, startDel, del);
		}
	}

	public void Start(GameObject sub, ANIM_MODE mode, Vector3 begin, Vector3 dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = Quaternion.Euler(begin);
		m_mode = mode;
		if (mode == ANIM_MODE.By)
		{
			Quaternion quaternion = Quaternion.Euler(begin + dest);
			delta = new Quaternion(quaternion.x - start.x, quaternion.y - start.y, quaternion.z - start.z, quaternion.w - start.w);
		}
		else
		{
			Quaternion quaternion2 = Quaternion.Euler(dest);
			delta = new Quaternion(quaternion2.x - start.x, quaternion2.y - start.y, quaternion2.z - start.z, quaternion2.w - start.w);
		}
		end.x = start.x + delta.x;
		end.y = start.y + delta.y;
		end.z = start.z + delta.z;
		end.w = start.w + delta.w;
		interpolator = interp;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;
		StartCommon();
		if (mode == ANIM_MODE.FromTo && delay == 0f)
		{
			subTrans.localRotation = start;
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
				start = subject.transform.localRotation;
				end.x = start.x + delta.x;
				end.y = start.y + delta.y;
				end.z = start.z + delta.z;
				end.w = start.w + delta.w;
			}
			EZAnimator.instance.AddAnimation(this);
		}
	}
}
