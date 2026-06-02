using UnityEngine;

public class Crash : EZAnimation
{
	protected Vector3 start;

	protected Vector3 magnitude;

	protected GameObject subject;

	protected Transform subTrans;

	protected Vector3 tempMag;

	protected Vector3 temp;

	protected float factor;

	public Crash()
	{
		type = ANIM_TYPE.Crash;
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
		tempMag.x = magnitude.x - factor * magnitude.x;
		tempMag.y = magnitude.y - factor * magnitude.y;
		tempMag.z = magnitude.z - factor * magnitude.z;
		temp.x = start.x + Random.Range(0f - tempMag.x, tempMag.x);
		temp.y = start.y + Random.Range(0f - tempMag.y, tempMag.y);
		temp.z = start.z + Random.Range(0f - tempMag.z, tempMag.z);
		subTrans.localPosition = temp;
		subTrans.BroadcastMessage("OnEZTranslated", SendMessageOptions.DontRequireReceiver);
	}

	public static Crash Do(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Crash crash = (Crash)EZAnimator.instance.GetAnimation(ANIM_TYPE.Crash);
		crash.Start(sub, mag, dur, delay, startDel, del);
		return crash;
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
		Start(sub, sub.transform.localPosition, parms.vec, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	public void Start(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (!(sub == null))
		{
			Start(sub, sub.transform.localPosition, mag, dur, delay, startDel, del);
		}
	}

	public void Start(GameObject sub, Vector3 begin, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localPosition = start;
		if (mag.x < 0f)
		{
			mag.x = Random.Range(1f, 0f - mag.x);
		}
		if (mag.y < 0f)
		{
			mag.y = Random.Range(1f, 0f - mag.y);
		}
		if (mag.z < 0f)
		{
			mag.z = Random.Range(1f, 0f - mag.z);
		}
		magnitude = mag;
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
