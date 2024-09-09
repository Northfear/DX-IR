using System;
using UnityEngine;

public class Shake : EZAnimation
{
	protected const float PIx2 = (float)Math.PI * 2f;

	protected Vector3 start;

	protected Vector3 magnitude;

	protected float oscillations;

	protected GameObject subject;

	protected Transform subTrans;

	protected Vector3 temp;

	protected float factor;

	public Shake()
	{
		type = ANIM_TYPE.Shake;
		pingPong = false;
	}

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localPosition = start;
			subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
		}
		base._end();
	}

	protected override void WaitDone()
	{
		base.WaitDone();
		if (subTrans != null)
		{
			start = subTrans.localPosition;
		}
	}

	protected override void DoAnim()
	{
		if (subTrans == null)
		{
			_stop();
			return;
		}
		factor = Mathf.Sin(timeElapsed / interval * ((float)Math.PI * 2f) * oscillations);
		temp.x = start.x + factor * magnitude.x;
		temp.y = start.y + factor * magnitude.y;
		temp.z = start.z + factor * magnitude.z;
		subTrans.localPosition = temp;
		subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
	}

	public static Shake Do(GameObject sub, Vector3 mag, float oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Shake shake = (Shake)EZAnimator.instance.GetAnimation(ANIM_TYPE.Shake);
		shake.Start(sub, mag, oscill, dur, delay, startDel, del);
		return shake;
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
		Start(sub, sub.transform.localPosition, parms.vec, parms.floatVal, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	public void Start(GameObject sub, Vector3 mag, float oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (!(sub == null))
		{
			Start(sub, sub.transform.localPosition, mag, oscill, dur, delay, startDel, del);
		}
	}

	public void Start(GameObject sub, Vector3 begin, Vector3 mag, float oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localPosition = start;
		if (mag.x < 0f)
		{
			mag.x = UnityEngine.Random.Range(1f, 0f - mag.x);
		}
		if (mag.y < 0f)
		{
			mag.y = UnityEngine.Random.Range(1f, 0f - mag.y);
		}
		if (mag.z < 0f)
		{
			mag.z = UnityEngine.Random.Range(1f, 0f - mag.z);
		}
		if (oscill < 0f)
		{
			oscill = UnityEngine.Random.Range(1f, 0f - oscill);
		}
		magnitude = mag;
		oscillations = oscill;
		m_mode = ANIM_MODE.By;
		duration = dur;
		m_wait = delay;
		completedDelegate = del;
		startDelegate = startDel;
		StartCommon();
		EZAnimator.instance.Stop(subject, type, true);
		EZAnimator.instance.AddAnimation(this);
	}
}
