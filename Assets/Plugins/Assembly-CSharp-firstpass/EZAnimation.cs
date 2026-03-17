using System;
using UnityEngine;

public abstract class EZAnimation : IEZLinkedListItem<EZAnimation>
{
	public enum ANIM_TYPE
	{
		AnimClip = 0,
		FadeSprite = 1,
		FadeMaterial = 2,
		FadeText = 3,
		Translate = 4,
		PunchPosition = 5,
		Crash = 6,
		SmoothCrash = 7,
		Shake = 8,
		Scale = 9,
		PunchScale = 10,
		Rotate = 11,
		PunchRotation = 12,
		ShakeRotation = 13,
		CrashRotation = 14,
		FadeAudio = 15,
		TuneAudio = 16,
		TranslateScreen = 17,
		FadeSpriteAlpha = 18,
		FadeTextAlpha = 19,
		RotateEuler = 20
	}

	public enum ANIM_MODE
	{
		By = 0,
		To = 1,
		FromTo = 2
	}

	public enum EASING_TYPE
	{
		Default = -1,
		Linear = 0,
		BackIn = 1,
		BackOut = 2,
		BackInOut = 3,
		BackOutIn = 4,
		BounceIn = 5,
		BounceOut = 6,
		BounceInOut = 7,
		BounceOutIn = 8,
		CircularIn = 9,
		CircularOut = 10,
		CircularInOut = 11,
		CircularOutIn = 12,
		CubicIn = 13,
		CubicOut = 14,
		CubicInOut = 15,
		CubicOutIn = 16,
		ElasticIn = 17,
		ElasticOut = 18,
		ElasticInOut = 19,
		ElasticOutIn = 20,
		ExponentialIn = 21,
		ExponentialOut = 22,
		ExponentialInOut = 23,
		ExponentialOutIn = 24,
		QuadraticIn = 25,
		QuadraticOut = 26,
		QuadraticInOut = 27,
		QuadraticOutIn = 28,
		QuarticIn = 29,
		QuarticOut = 30,
		QuarticInOut = 31,
		QuarticOutIn = 32,
		QuinticIn = 33,
		QuinticOut = 34,
		QuinticInOut = 35,
		QuinticOutIn = 36,
		SinusoidalIn = 37,
		SinusoidalOut = 38,
		SinusoidalInOut = 39,
		SinusoidalOutIn = 40,
		Spring = 41
	}

	public delegate void CompletionDelegate(EZAnimation anim);

	public delegate float Interpolator(float time, float start, float delta, float duration);

	public ANIM_TYPE type;

	public bool pingPong = true;

	public bool repeatDelay;

	public bool restartOnRepeat;

	public bool running;

	protected bool m_paused;

	protected object data;

	protected ANIM_MODE m_mode;

	protected float direction = 1f;

	protected float timeElapsed;

	protected float wait;

	protected float m_wait;

	protected float duration;

	protected float interval;

	protected CompletionDelegate completedDelegate;

	protected CompletionDelegate startDelegate;

	protected Interpolator interpolator;

	protected EZAnimation m_prev;

	protected EZAnimation m_next;

	public object Data
	{
		get
		{
			return data;
		}
		set
		{
			data = value;
		}
	}

	public float Duration
	{
		get
		{
			return duration;
		}
	}

	public float Wait
	{
		get
		{
			return wait;
		}
	}

	public bool Paused
	{
		get
		{
			return m_paused;
		}
		set
		{
			m_paused = running && value;
		}
	}

	public ANIM_MODE Mode
	{
		get
		{
			return m_mode;
		}
	}

	public CompletionDelegate CompletedDelegate
	{
		get
		{
			return completedDelegate;
		}
		set
		{
			completedDelegate = value;
		}
	}

	public CompletionDelegate StartDelegate
	{
		get
		{
			return startDelegate;
		}
		set
		{
			startDelegate = value;
		}
	}

	public EZAnimation prev
	{
		get
		{
			return m_prev;
		}
		set
		{
			m_prev = value;
		}
	}

	public EZAnimation next
	{
		get
		{
			return m_next;
		}
		set
		{
			m_next = value;
		}
	}

	public void Clear()
	{
		completedDelegate = null;
		startDelegate = null;
		data = null;
	}

	public abstract bool Start(GameObject sub, AnimParams parms);

	protected abstract void DoAnim();

	public virtual bool Step(float timeDelta)
	{
		if (m_paused)
		{
			return true;
		}
		if (wait > 0f)
		{
			wait -= timeDelta;
			if (!(wait < 0f))
			{
				return true;
			}
			if (startDelegate != null)
			{
				startDelegate(this);
			}
			timeDelta -= timeDelta + wait;
			WaitDone();
		}
		timeElapsed += timeDelta * direction;
		if (timeElapsed >= interval || timeElapsed < 0f)
		{
			if (!(duration < 0f))
			{
				_end();
				return false;
			}
			if (pingPong)
			{
				if (timeElapsed >= interval)
				{
					direction = -1f;
					timeElapsed = interval - (timeElapsed - interval);
				}
				else
				{
					if (repeatDelay)
					{
						wait = m_wait - (timeElapsed - interval);
					}
					else
					{
						if (startDelegate != null)
						{
							startDelegate(this);
						}
						timeElapsed *= -1f;
					}
					direction = 1f;
				}
			}
			else
			{
				if (repeatDelay)
				{
					wait = m_wait;
				}
				else if (startDelegate != null)
				{
					startDelegate(this);
				}
				LoopReset();
				timeElapsed -= interval;
			}
		}
		DoAnim();
		return true;
	}

	public virtual void Stop()
	{
		EZAnimator.instance.StopAnimation(this);
	}

	public void _stop()
	{
		running = false;
		Paused = false;
		if (completedDelegate != null)
		{
			completedDelegate(this);
		}
	}

	public void End()
	{
		EZAnimator.instance.StopAnimation(this, true);
	}

	public void _cancel()
	{
		running = false;
		Clear();
	}

	public virtual void _end()
	{
		_stop();
	}

	protected virtual void LoopReset()
	{
	}

	public abstract object GetSubject();

	protected virtual void WaitDone()
	{
	}

	protected void StartCommon()
	{
		wait = m_wait;
		if (wait == 0f && startDelegate != null)
		{
			startDelegate(this);
		}
		interval = Mathf.Abs(duration);
		direction = 1f;
		timeElapsed = 0f;
		Paused = false;
	}

	public void ResetDefaults()
	{
		pingPong = true;
		restartOnRepeat = false;
		data = null;
		completedDelegate = null;
		startDelegate = null;
	}

	public static float linear(float time, float start, float delta, float duration)
	{
		return delta * time / duration + start;
	}

	public static float quadraticIn(float time, float start, float delta, float duration)
	{
		time /= duration;
		return delta * time * time + start;
	}

	public static float quadraticOut(float time, float start, float delta, float duration)
	{
		time /= duration;
		return (0f - delta) * time * (time - 2f) + start;
	}

	public static float quadraticInOut(float time, float start, float delta, float duration)
	{
		time /= duration / 2f;
		if (time < duration / 2f)
		{
			return delta / 2f * time * time + start;
		}
		time -= 1f;
		return (0f - delta) / 2f * (time * (time - 2f) - 1f) + start;
	}

	public static float quadraticOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f)
		{
			return quadraticOut(time * 2f, start, delta / 2f, duration);
		}
		return quadraticIn(time * 2f - duration, start + delta / 2f, delta / 2f, duration);
	}

	public static float cubicIn(float time, float start, float delta, float duration)
	{
		time /= duration;
		return delta * time * time * time + start;
	}

	public static float cubicOut(float time, float start, float delta, float duration)
	{
		time /= duration;
		time -= 1f;
		return delta * (time * time * time + 1f) + start;
	}

	public static float cubicInOut(float time, float start, float delta, float duration)
	{
		time /= duration / 2f;
		if (time < 1f)
		{
			return delta / 2f * time * time * time + start;
		}
		time -= 2f;
		return delta / 2f * (time * time * time + 2f) + start;
	}

	public static float cubicOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f)
		{
			return cubicOut(time * 2f, start, delta / 2f, duration);
		}
		return cubicIn(time * 2f - duration, start + delta / 2f, delta / 2f, duration);
	}

	public static float quarticIn(float time, float start, float delta, float duration)
	{
		time /= duration;
		return delta * time * time * time * time + start;
	}

	public static float quarticOut(float time, float start, float delta, float duration)
	{
		time /= duration;
		time -= 1f;
		return (0f - delta) * (time * time * time * time - 1f) + start;
	}

	public static float quarticInOut(float time, float start, float delta, float duration)
	{
		time /= duration / 2f;
		if (time < 1f)
		{
			return delta / 2f * time * time * time * time + start;
		}
		time -= 2f;
		return (0f - delta) / 2f * (time * time * time * time - 2f) + start;
	}

	public static float quarticOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f)
		{
			return quarticOut(time * 2f, start, delta / 2f, duration);
		}
		return quarticIn(time * 2f - duration, start + delta / 2f, delta / 2f, duration);
	}

	public static float quinticIn(float time, float start, float delta, float duration)
	{
		time /= duration;
		return delta * time * time * time * time * time + start;
	}

	public static float quinticOut(float time, float start, float delta, float duration)
	{
		time /= duration;
		time -= 1f;
		return delta * (time * time * time * time * time + 1f) + start;
	}

	public static float quinticInOut(float time, float start, float delta, float duration)
	{
		time /= duration / 2f;
		if (time < 1f)
		{
			return delta / 2f * time * time * time * time * time + start;
		}
		time -= 2f;
		return delta / 2f * (time * time * time * time * time + 2f) + start;
	}

	public static float quinticOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f)
		{
			return quinticOut(time * 2f, start, delta / 2f, duration);
		}
		return quinticIn(time * 2f - duration, start + delta / 2f, delta / 2f, duration);
	}

	public static float sinusIn(float time, float start, float delta, float duration)
	{
		return (0f - delta) * Mathf.Cos(time / duration * ((float)Math.PI / 2f)) + delta + start;
	}

	public static float sinusOut(float time, float start, float delta, float duration)
	{
		return delta * Mathf.Sin(time / duration * ((float)Math.PI / 2f)) + start;
	}

	public static float sinusInOut(float time, float start, float delta, float duration)
	{
		return (0f - delta) / 2f * (Mathf.Cos((float)Math.PI * time / duration) - 1f) + start;
	}

	public static float sinusOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f)
		{
			return sinusOut(time * 2f, start, delta / 2f, duration);
		}
		return sinusIn(time * 2f - duration, start + delta / 2f, delta / 2f, duration);
	}

	public static float expIn(float time, float start, float delta, float duration)
	{
		return delta * Mathf.Pow(2f, 10f * (time / duration - 1f)) + start;
	}

	public static float expOut(float time, float start, float delta, float duration)
	{
		return delta * (0f - Mathf.Pow(2f, -10f * time / duration) + 1f) + start;
	}

	public static float expInOut(float time, float start, float delta, float duration)
	{
		time /= duration / 2f;
		if (time < 1f)
		{
			return delta / 2f * Mathf.Pow(2f, 10f * (time - 1f)) + start;
		}
		time -= 1f;
		return delta / 2f * (0f - Mathf.Pow(2f, -10f * time) + 2f) + start;
	}

	public static float expOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f)
		{
			return expOut(time * 2f, start, delta / 2f, duration);
		}
		return expIn(time * 2f - duration, start + delta / 2f, delta / 2f, duration);
	}

	public static float circIn(float time, float start, float delta, float duration)
	{
		time /= duration;
		return (0f - delta) * (Mathf.Sqrt(1f - time * time) - 1f) + start;
	}

	public static float circOut(float time, float start, float delta, float duration)
	{
		time /= duration;
		time -= 1f;
		return delta * Mathf.Sqrt(1f - time * time) + start;
	}

	public static float circInOut(float time, float start, float delta, float duration)
	{
		time /= duration / 2f;
		if (time < 1f)
		{
			return (0f - delta) / 2f * (Mathf.Sqrt(1f - time * time) - 1f) + start;
		}
		time -= 2f;
		return delta / 2f * (Mathf.Sqrt(1f - time * time) + 1f) + start;
	}

	public static float circOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f)
		{
			return circOut(time * 2f, start, delta / 2f, duration);
		}
		return circIn(time * 2f - duration, start + delta / 2f, delta / 2f, duration);
	}

	public static float punch(float amplitude, float value)
	{
		float num = 9f;
		if (value == 0f)
		{
			return 0f;
		}
		if (value == 1f)
		{
			return 0f;
		}
		float num2 = 0.3f;
		num = num2 / ((float)Math.PI * 2f) * Mathf.Asin(0f);
		return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - num) * ((float)Math.PI * 2f) / num2);
	}

	public static float spring(float time, float start, float delta, float duration)
	{
		float value = time / duration;
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * (float)Math.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
		return start + delta * value;
	}

	public static float elasticIn(float time, float start, float delta, float duration)
	{
		return elasticIn(time, start, delta, duration, 0f, duration * 0.3f);
	}

	public static float elasticIn(float time, float start, float delta, float duration, float amplitude, float period)
	{
		if (time == 0f)
		{
			return start;
		}
		if (delta == 0f)
		{
			return start;
		}
		if ((time /= duration) == 1f)
		{
			return start + delta;
		}
		float num;
		if (amplitude < Mathf.Abs(delta))
		{
			amplitude = delta;
			num = period / 4f;
		}
		else
		{
			num = period / ((float)Math.PI * 2f) * Mathf.Asin(delta / amplitude);
		}
		return 0f - amplitude * Mathf.Pow(2f, 10f * (time -= 1f)) * Mathf.Sin((time * duration - num) * ((float)Math.PI * 2f) / period) + start;
	}

	public static float elasticOut(float time, float start, float delta, float duration)
	{
		return elasticOut(time, start, delta, duration, 0f, duration * 0.3f);
	}

	public static float elasticOut(float time, float start, float delta, float duration, float amplitude, float period)
	{
		if (time == 0f)
		{
			return start;
		}
		if (delta == 0f)
		{
			return start;
		}
		if ((time /= duration) == 1f)
		{
			return start + delta;
		}
		float num;
		if (amplitude < Mathf.Abs(delta))
		{
			amplitude = delta;
			num = period / 4f;
		}
		else
		{
			num = period / ((float)Math.PI * 2f) * Mathf.Asin(delta / amplitude);
		}
		return amplitude * Mathf.Pow(2f, -10f * time) * Mathf.Sin((time * duration - num) * ((float)Math.PI * 2f) / period) + delta + start;
	}

	public static float elasticInOut(float time, float start, float delta, float duration)
	{
		return elasticInOut(time, start, delta, duration, 0f, duration * 0.3f * 1.5f);
	}

	public static float elasticInOut(float time, float start, float delta, float duration, float amplitude, float period)
	{
		if (time == 0f)
		{
			return start;
		}
		if (delta == 0f)
		{
			return start;
		}
		if ((time /= duration / 2f) == 2f)
		{
			return start + delta;
		}
		float num;
		if (amplitude < Mathf.Abs(delta))
		{
			amplitude = delta;
			num = period / 4f;
		}
		else
		{
			num = period / ((float)Math.PI * 2f) * Mathf.Asin(delta / amplitude);
		}
		if (time < 1f)
		{
			return -0.5f * (amplitude * Mathf.Pow(2f, 10f * (time -= 1f)) * Mathf.Sin((time * duration - num) * ((float)Math.PI * 2f) / period)) + start;
		}
		return amplitude * Mathf.Pow(2f, -10f * (time -= 1f)) * Mathf.Sin((time * duration - num) * ((float)Math.PI * 2f) / period) * 0.5f + delta + start;
	}

	public static float elasticOutIn(float time, float start, float delta, float duration)
	{
		return elasticOutIn(time, start, delta, duration, 0f, duration * 0.3f);
	}

	public static float elasticOutIn(float time, float start, float delta, float duration, float amplitude, float period)
	{
		if (time < duration / 2f)
		{
			return elasticOut(time * 2f, start, delta / 2f, duration, amplitude, period);
		}
		return elasticIn(time * 2f - duration, start + delta / 2f, delta / 2f, duration, amplitude, period);
	}

	public static float backIn(float time, float start, float delta, float duration)
	{
		return backIn(time, start, delta, duration, 1.70158f);
	}

	public static float backIn(float time, float start, float delta, float duration, float overshootAmt)
	{
		return delta * (time /= duration) * time * ((overshootAmt + 1f) * time - overshootAmt) + start;
	}

	public static float backOut(float time, float start, float delta, float duration)
	{
		return backOut(time, start, delta, duration, 1.70158f);
	}

	public static float backOut(float time, float start, float delta, float duration, float overshootAmt)
	{
		return delta * ((time = time / duration - 1f) * time * ((overshootAmt + 1f) * time + overshootAmt) + 1f) + start;
	}

	public static float backInOut(float time, float start, float delta, float duration)
	{
		return backInOut(time, start, delta, duration, 1.70158f);
	}

	public static float backInOut(float time, float start, float delta, float duration, float overshootAmt)
	{
		if ((time /= duration / 2f) < 1f)
		{
			return delta / 2f * (time * time * (((overshootAmt *= 1.525f) + 1f) * time - overshootAmt)) + start;
		}
		return delta / 2f * ((time -= 2f) * time * (((overshootAmt *= 1.525f) + 1f) * time + overshootAmt) + 2f) + start;
	}

	public static float backOutIn(float time, float start, float delta, float duration)
	{
		return backOutIn(time, start, delta, duration, 1.70158f);
	}

	public static float backOutIn(float time, float start, float delta, float duration, float overshootAmt)
	{
		if (time < duration / 2f)
		{
			return backOut(time * 2f, start, delta / 2f, duration, overshootAmt);
		}
		return backIn(time * 2f - duration, start + delta / 2f, delta / 2f, duration, overshootAmt);
	}

	public static float bounceIn(float time, float start, float delta, float duration)
	{
		return delta - bounceOut(duration - time, 0f, delta, duration) + start;
	}

	public static float bounceOut(float time, float start, float delta, float duration)
	{
		if ((time /= duration) < 0.36363637f)
		{
			return delta * (7.5625f * time * time) + start;
		}
		if (time < 0.72727275f)
		{
			return delta * (7.5625f * (time -= 0.54545456f) * time + 0.75f) + start;
		}
		if (time < 0.90909094f)
		{
			return delta * (7.5625f * (time -= 0.8181818f) * time + 0.9375f) + start;
		}
		return delta * (7.5625f * (time -= 21f / 22f) * time + 63f / 64f) + start;
	}

	public static float bounceInOut(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f)
		{
			return bounceIn(time * 2f, 0f, delta, duration) * 0.5f + start;
		}
		return bounceOut(time * 2f - duration, 0f, delta, duration) * 0.5f + delta * 0.5f + start;
	}

	public static float bounceOutIn(float time, float start, float delta, float duration)
	{
		if (time < duration / 2f)
		{
			return bounceOut(time * 2f, start, delta / 2f, duration);
		}
		return bounceIn(time * 2f - duration, start + delta / 2f, delta / 2f, duration);
	}

	public static Interpolator GetInterpolator(EASING_TYPE type)
	{
		switch (type)
		{
		case EASING_TYPE.BackIn:
			return backIn;
		case EASING_TYPE.BackInOut:
			return backInOut;
		case EASING_TYPE.BackOut:
			return backOut;
		case EASING_TYPE.BackOutIn:
			return backOutIn;
		case EASING_TYPE.BounceIn:
			return bounceIn;
		case EASING_TYPE.BounceInOut:
			return bounceInOut;
		case EASING_TYPE.BounceOut:
			return bounceOut;
		case EASING_TYPE.BounceOutIn:
			return bounceOutIn;
		case EASING_TYPE.CircularIn:
			return circIn;
		case EASING_TYPE.CircularInOut:
			return circInOut;
		case EASING_TYPE.CircularOut:
			return circOut;
		case EASING_TYPE.CircularOutIn:
			return circOutIn;
		case EASING_TYPE.CubicIn:
			return cubicIn;
		case EASING_TYPE.CubicInOut:
			return cubicInOut;
		case EASING_TYPE.CubicOut:
			return cubicOut;
		case EASING_TYPE.CubicOutIn:
			return cubicOutIn;
		case EASING_TYPE.ElasticIn:
			return elasticIn;
		case EASING_TYPE.ElasticInOut:
			return elasticInOut;
		case EASING_TYPE.ElasticOut:
			return elasticOut;
		case EASING_TYPE.ElasticOutIn:
			return elasticOutIn;
		case EASING_TYPE.ExponentialIn:
			return expIn;
		case EASING_TYPE.ExponentialInOut:
			return expInOut;
		case EASING_TYPE.ExponentialOut:
			return expOut;
		case EASING_TYPE.ExponentialOutIn:
			return expOutIn;
		case EASING_TYPE.Linear:
			return linear;
		case EASING_TYPE.QuadraticIn:
			return quadraticIn;
		case EASING_TYPE.QuadraticInOut:
			return quadraticInOut;
		case EASING_TYPE.QuadraticOut:
			return quadraticOut;
		case EASING_TYPE.QuadraticOutIn:
			return quadraticOutIn;
		case EASING_TYPE.QuarticIn:
			return quarticIn;
		case EASING_TYPE.QuarticInOut:
			return quarticInOut;
		case EASING_TYPE.QuarticOut:
			return quarticOut;
		case EASING_TYPE.QuarticOutIn:
			return quarticOutIn;
		case EASING_TYPE.QuinticIn:
			return quinticIn;
		case EASING_TYPE.QuinticInOut:
			return quinticInOut;
		case EASING_TYPE.QuinticOut:
			return quinticOut;
		case EASING_TYPE.QuinticOutIn:
			return quinticOutIn;
		case EASING_TYPE.SinusoidalIn:
			return sinusIn;
		case EASING_TYPE.SinusoidalInOut:
			return sinusInOut;
		case EASING_TYPE.SinusoidalOut:
			return sinusOut;
		case EASING_TYPE.SinusoidalOutIn:
			return sinusOutIn;
		case EASING_TYPE.Spring:
			return spring;
		default:
			return linear;
		}
	}
}
