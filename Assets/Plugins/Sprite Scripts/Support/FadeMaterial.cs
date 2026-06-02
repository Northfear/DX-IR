using UnityEngine;

public class FadeMaterial : EZAnimation
{
	protected Color start;

	protected Color delta;

	protected Color end;

	protected Material mat;

	protected Color temp;

	public FadeMaterial()
	{
		type = ANIM_TYPE.FadeMaterial;
	}

	public override object GetSubject()
	{
		return mat;
	}

	public override void _end()
	{
		if (mat != null)
		{
			mat.color = end;
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
		if (base.Mode == ANIM_MODE.By && mat != null)
		{
			start = mat.color;
			end = start + delta;
		}
	}

	protected override void DoAnim()
	{
		if (mat == null)
		{
			_stop();
			return;
		}
		temp.r = interpolator(timeElapsed, start.r, delta.r, interval);
		temp.g = interpolator(timeElapsed, start.g, delta.g, interval);
		temp.b = interpolator(timeElapsed, start.b, delta.b, interval);
		temp.a = interpolator(timeElapsed, start.a, delta.a, interval);
		mat.color = temp;
	}

	public static FadeMaterial Do(Material material, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeMaterial fadeMaterial = (FadeMaterial)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeMaterial);
		fadeMaterial.Start(material, mode, begin, dest, interp, dur, delay, startDel, del);
		return fadeMaterial;
	}

	public static FadeMaterial Do(Material material, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		FadeMaterial fadeMaterial = (FadeMaterial)EZAnimator.instance.GetAnimation(ANIM_TYPE.FadeMaterial);
		fadeMaterial.Start(material, mode, dest, interp, dur, delay, startDel, del);
		return fadeMaterial;
	}

	public override bool Start(GameObject sub, AnimParams parms)
	{
		if (sub == null)
		{
			return false;
		}
		if (sub.renderer == null)
		{
			return false;
		}
		if (sub.renderer.material == null)
		{
			return false;
		}
		pingPong = parms.pingPong;
		restartOnRepeat = parms.restartOnRepeat;
		repeatDelay = parms.repeatDelay;
		if (parms.mode == ANIM_MODE.FromTo)
		{
			Start(sub.renderer.material, parms.mode, parms.color, parms.color2, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}
		else
		{
			Start(sub.renderer.material, parms.mode, sub.renderer.material.color, parms.color, EZAnimation.GetInterpolator(parms.easing), parms.duration, parms.delay, null, parms.transition.OnAnimEnd);
		}
		return true;
	}

	public void Start(Material material, ANIM_MODE mode, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		Start(material, mode, material.color, dest, interp, dur, delay, startDel, del);
	}

	public void Start(Material material, ANIM_MODE mode, Color begin, Color dest, Interpolator interp, float dur, float delay, CompletionDelegate startDel, CompletionDelegate del)
	{
		mat = material;
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
			mat.color = start;
		}
		EZAnimator.instance.AddAnimation(this);
	}

	public void Start()
	{
		if (!(mat == null))
		{
			direction = 1f;
			timeElapsed = 0f;
			wait = m_wait;
			if (m_mode == ANIM_MODE.By)
			{
				start = mat.color;
				end = start + delta;
			}
			EZAnimator.instance.AddAnimation(this);
		}
	}
}
