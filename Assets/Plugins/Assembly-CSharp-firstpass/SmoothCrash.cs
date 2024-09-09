using System;
using UnityEngine;

public class SmoothCrash : EZAnimation
{
	protected const float PIx2 = (float)Math.PI * 2f;

	protected Vector3 start;

	protected Vector3 magnitude;

	protected Vector3 oscillations;

	protected GameObject subject;

	protected Transform subTrans;

	protected Vector3 temp;

	protected float factor;

	protected float invFactor;

	public SmoothCrash()
	{
		type = ANIM_TYPE.SmoothCrash;
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
		factor = timeElapsed / interval;
		invFactor = 1f - factor;
		factor *= (float)Math.PI * 2f;
		temp.x = start.x + Mathf.Sin(factor * oscillations.x) * magnitude.x * invFactor;
		temp.y = start.y + Mathf.Sin(factor * oscillations.y) * magnitude.y * invFactor;
		temp.z = start.z + Mathf.Sin(factor * oscillations.z) * magnitude.z * invFactor;
		subTrans.localPosition = temp;
		subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
	}

	public static SmoothCrash Do(GameObject sub, Vector3 mag, Vector3 oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		SmoothCrash smoothCrash = (SmoothCrash)EZAnimator.instance.GetAnimation(ANIM_TYPE.SmoothCrash);
		smoothCrash.Start(sub, mag, oscill, dur, delay, startDel, del);
		return smoothCrash;
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
		Start(sub, sub.transform.localPosition, parms.vec, parms.vec2, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	public void Start(GameObject sub, Vector3 mag, Vector3 oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (!(sub == null))
		{
			Start(sub, sub.transform.localPosition, mag, oscill, dur, delay, startDel, del);
		}
	}

	public void Start(GameObject sub, Vector3 begin, Vector3 mag, Vector3 oscill, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localPosition = start;
		magnitude = mag;
		if (oscill.x < 0f)
		{
			oscill.x = UnityEngine.Random.Range(1f, 0f - oscill.x);
		}
		if (oscill.y < 0f)
		{
			oscill.y = UnityEngine.Random.Range(1f, 0f - oscill.y);
		}
		if (oscill.z < 0f)
		{
			oscill.z = UnityEngine.Random.Range(1f, 0f - oscill.z);
		}
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
