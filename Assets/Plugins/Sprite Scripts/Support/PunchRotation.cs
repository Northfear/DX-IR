using UnityEngine;

public class PunchRotation : EZAnimation
{
	protected Vector3 start;

	protected Vector3 magnitude;

	protected GameObject subject;

	protected Transform subTrans;

	protected Vector3 temp;

	protected float factor;

	public PunchRotation()
	{
		type = ANIM_TYPE.PunchRotation;
	}

	public override object GetSubject()
	{
		return subject;
	}

	public override void _end()
	{
		if (subTrans != null)
		{
			subTrans.localEulerAngles = start;
			subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
		}
		base._end();
	}

	protected override void WaitDone()
	{
		base.WaitDone();
		if (subTrans != null)
		{
			start = subTrans.localEulerAngles;
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
		subTrans.localRotation = Quaternion.Euler(temp);
		subTrans.BroadcastMessage("OnEZRotated", SendMessageOptions.DontRequireReceiver);
	}

	public static PunchRotation Do(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		PunchRotation punchRotation = (PunchRotation)EZAnimator.instance.GetAnimation(ANIM_TYPE.PunchRotation);
		punchRotation.Start(sub, mag, dur, delay, startDel, del);
		return punchRotation;
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
		Start(sub, sub.transform.localEulerAngles, parms.vec, parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		return true;
	}

	public void Start(GameObject sub, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		if (!(sub == null))
		{
			Start(sub, sub.transform.localEulerAngles, mag, dur, delay, startDel, del);
		}
	}

	public void Start(GameObject sub, Vector3 begin, Vector3 mag, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		subject = sub;
		subTrans = subject.transform;
		start = begin;
		subTrans.localEulerAngles = start;
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
