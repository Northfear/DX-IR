using UnityEngine;

public class PunchScale : EZAnimation
{
	protected Vector3 start;

	protected Vector3 magnitude;

	protected GameObject subject;

	protected Transform subTrans;

	protected Vector3 temp;

	protected float factor;

	public PunchScale()
	{
		type = ANIM_TYPE.PunchScale;
	}

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localScale = start;
			subTrans.BroadcastMessage("OnEZScaled", SendMessageOptions.DontRequireReceiver);
		}
		base._end();
	}

	protected override void WaitDone()
	{
		base.WaitDone();
		if (subTrans != null)
		{
			start = subTrans.localScale;
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
		temp.x = start.x + EZAnimation.punch(magnitude.x, factor);
		temp.y = start.y + EZAnimation.punch(magnitude.y, factor);
		temp.z = start.z + EZAnimation.punch(magnitude.z, factor);
		subTrans.localScale = temp;
		subTrans.BroadcastMessage("OnEZScaled", SendMessageOptions.DontRequireReceiver);
	}

	public static PunchScale Do(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		PunchScale punchScale = (PunchScale)EZAnimator.instance.GetAnimation(ANIM_TYPE.PunchScale);
		punchScale.Start(sub, mag, dur, delay, startDel, del);
		return punchScale;
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
		Start(sub, sub.transform.localScale, parms.vec, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	public void Start(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (!(sub == null))
		{
			Start(sub, sub.transform.localScale, mag, dur, delay, startDel, del);
		}
	}

	public void Start(GameObject sub, Vector3 begin, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localScale = start;
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
