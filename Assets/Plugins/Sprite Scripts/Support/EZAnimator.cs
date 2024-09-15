using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EZAnimator : MonoBehaviour
{
	private static EZAnimator s_Instance = null;

	protected static Dictionary<EZAnimation.ANIM_TYPE, EZLinkedList<EZAnimation>> freeAnimPool = new Dictionary<EZAnimation.ANIM_TYPE, EZLinkedList<EZAnimation>>();

	protected static EZLinkedList<EZAnimation> animations = new EZLinkedList<EZAnimation>();

	protected static bool pumpIsRunning = false;

	protected static bool pumpIsDone = true;

	protected static float _timeScale = 1f;

	protected static float startTime;

	protected static float time;

	protected static float elapsed;

	protected static EZAnimation anim;

	protected static float timePaused;

	private int i;

	public static EZAnimator instance
	{
		get
		{
			if (s_Instance == null)
			{
				GameObject gameObject = new GameObject("EZAnimator");
				s_Instance = (EZAnimator)gameObject.AddComponent(typeof(EZAnimator));
			}
			return s_Instance;
		}
	}

	public static float timeScale
	{
		get
		{
			return _timeScale;
		}
		set
		{
			_timeScale = Mathf.Max(0f, value);
		}
	}

	public static bool Exists()
	{
		return s_Instance != null;
	}

	public void OnDestroy()
	{
		s_Instance = null;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void OnLevelWasLoaded(int level)
	{
		if (animations != null)
		{
			EZLinkedListIterator<EZAnimation> eZLinkedListIterator = animations.Begin();
			while (!eZLinkedListIterator.Done)
			{
				EZAnimation current = eZLinkedListIterator.Current;
				current._cancel();
				animations.Remove(current);
				ReturnAnimToPool(current);
			}
			eZLinkedListIterator.End();
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			timePaused = Time.realtimeSinceStartup;
			return;
		}
		float num = Time.realtimeSinceStartup - timePaused;
		startTime += num;
	}

	protected static IEnumerator AnimPump()
	{
		EZLinkedListIterator<EZAnimation> i = animations.Begin();
		startTime = Time.realtimeSinceStartup;
		pumpIsDone = false;
		while (pumpIsRunning)
		{
			time = Time.realtimeSinceStartup;
			elapsed = (time - startTime) * _timeScale;
			startTime = time;
			i.Begin(animations);
			while (!i.Done)
			{
				if (!i.Current.Step(elapsed))
				{
					anim = i.Current;
					animations.Remove(anim);
					ReturnAnimToPool(anim);
				}
				else
				{
					i.NextNoRemove();
				}
			}
			yield return null;
		}
		pumpIsDone = true;
	}

	public void StartAnimationPump()
	{
		if (!pumpIsRunning && base.gameObject.active)
		{
			pumpIsRunning = true;
			StartCoroutine(PumpStarter());
		}
	}

	protected IEnumerator PumpStarter()
	{
		while (!pumpIsDone)
		{
			yield return null;
		}
		StartCoroutine(AnimPump());
	}

	public static void StopAnimationPump()
	{
	}

	protected EZAnimation CreateNewAnimation(EZAnimation.ANIM_TYPE type)
	{
		switch (type)
		{
		case EZAnimation.ANIM_TYPE.AnimClip:
			return new RunAnimClip();
		case EZAnimation.ANIM_TYPE.Crash:
			return new Crash();
		case EZAnimation.ANIM_TYPE.CrashRotation:
			return new CrashRotation();
		case EZAnimation.ANIM_TYPE.FadeAudio:
			return new FadeAudio();
		case EZAnimation.ANIM_TYPE.FadeSprite:
			return new FadeSprite();
		case EZAnimation.ANIM_TYPE.FadeMaterial:
			return new FadeMaterial();
		case EZAnimation.ANIM_TYPE.FadeText:
			return new FadeText();
		case EZAnimation.ANIM_TYPE.PunchPosition:
			return new PunchPosition();
		case EZAnimation.ANIM_TYPE.PunchRotation:
			return new PunchRotation();
		case EZAnimation.ANIM_TYPE.PunchScale:
			return new PunchScale();
		case EZAnimation.ANIM_TYPE.Rotate:
			return new AnimateRotation();
		case EZAnimation.ANIM_TYPE.Scale:
			return new AnimateScale();
		case EZAnimation.ANIM_TYPE.Shake:
			return new Shake();
		case EZAnimation.ANIM_TYPE.ShakeRotation:
			return new ShakeRotation();
		case EZAnimation.ANIM_TYPE.SmoothCrash:
			return new SmoothCrash();
		case EZAnimation.ANIM_TYPE.Translate:
			return new AnimatePosition();
		case EZAnimation.ANIM_TYPE.TranslateScreen:
			return new AnimateScreenPosition();
		case EZAnimation.ANIM_TYPE.TuneAudio:
			return new TuneAudio();
		case EZAnimation.ANIM_TYPE.FadeSpriteAlpha:
			return new FadeSpriteAlpha();
		case EZAnimation.ANIM_TYPE.FadeTextAlpha:
			return new FadeTextAlpha();
		case EZAnimation.ANIM_TYPE.RotateEuler:
			return new AnimateRotationEuler();
		default:
			return null;
		}
	}

	public EZAnimation GetAnimation(EZAnimation.ANIM_TYPE type)
	{
		EZLinkedList<EZAnimation> value;
		if (freeAnimPool.TryGetValue(type, out value) && !value.Empty)
		{
			EZAnimation head = value.Head;
			value.Remove(head);
			return head;
		}
		return CreateNewAnimation(type);
	}

	protected static void ReturnAnimToPool(EZAnimation anim)
	{
		anim.Clear();
		EZLinkedList<EZAnimation> value;
		if (!freeAnimPool.TryGetValue(anim.type, out value))
		{
			value = new EZLinkedList<EZAnimation>();
			freeAnimPool.Add(anim.type, value);
		}
		value.Add(anim);
		anim.ResetDefaults();
	}

	public void AddAnimation(EZAnimation a)
	{
		if (!a.running)
		{
			animations.Add(a);
			a.running = true;
		}
		StartAnimationPump();
	}

	public void AddTransition(EZTransition t)
	{
		if (t.animationTypes == null)
		{
			return;
		}
		for (int i = 0; i < t.animationTypes.Length; i++)
		{
			EZAnimation.ANIM_TYPE aNIM_TYPE = t.animationTypes[i];
			if (aNIM_TYPE == EZAnimation.ANIM_TYPE.FadeSprite || aNIM_TYPE == EZAnimation.ANIM_TYPE.FadeText || aNIM_TYPE == EZAnimation.ANIM_TYPE.FadeMaterial || aNIM_TYPE == EZAnimation.ANIM_TYPE.FadeTextAlpha || aNIM_TYPE == EZAnimation.ANIM_TYPE.FadeSpriteAlpha)
			{
				EZLinkedList<EZLinkedListNode<GameObject>> subSubjects = t.SubSubjects;
				if (subSubjects != null && subSubjects.Rewind())
				{
					do
					{
						EZAnimation eZAnimation = GetAnimation(aNIM_TYPE);
						t.animParams[i].transition = t;
						if (!eZAnimation.Start(subSubjects.Current.val, t.animParams[i]))
						{
							ReturnAnimToPool(eZAnimation);
						}
						else if (eZAnimation.running)
						{
							EZLinkedListNode<EZAnimation> eZLinkedListNode = t.AddRunningAnim();
							eZLinkedListNode.val = eZAnimation;
							eZAnimation.Data = eZLinkedListNode;
						}
					}
					while (subSubjects.MoveNext());
				}
			}
			if (!(t.MainSubject == null))
			{
				EZAnimation eZAnimation = GetAnimation(aNIM_TYPE);
				t.animParams[i].transition = t;
				if (!eZAnimation.Start(t.MainSubject, t.animParams[i]))
				{
					ReturnAnimToPool(eZAnimation);
				}
				else if (eZAnimation.running)
				{
					EZLinkedListNode<EZAnimation> eZLinkedListNode = t.AddRunningAnim();
					eZLinkedListNode.val = eZAnimation;
					eZAnimation.Data = eZLinkedListNode;
				}
			}
		}
	}

	public void Cancel(object obj)
	{
		EZLinkedListIterator<EZAnimation> eZLinkedListIterator = animations.Begin();
		while (!eZLinkedListIterator.Done)
		{
			if (eZLinkedListIterator.Current.GetSubject() == obj)
			{
				EZAnimation current = eZLinkedListIterator.Current;
				if (current.running)
				{
					current._cancel();
					animations.Remove(current);
					ReturnAnimToPool(current);
					continue;
				}
			}
			eZLinkedListIterator.Next();
		}
		eZLinkedListIterator.End();
	}

	public void StopAnimation(EZAnimation a)
	{
		StopAnimation(a, false);
	}

	public void StopAnimation(EZAnimation a, bool end)
	{
		if (a.running)
		{
			if (end)
			{
				a._end();
			}
			else
			{
				a._stop();
			}
			animations.Remove(a);
			ReturnAnimToPool(a);
			if (animations.Empty)
			{
				StopAnimationPump();
			}
		}
	}

	public void Stop(object obj)
	{
		Stop(obj, false);
	}

	public void Stop(object obj, bool end)
	{
		EZLinkedListIterator<EZAnimation> eZLinkedListIterator = animations.Begin();
		while (!eZLinkedListIterator.Done)
		{
			if (eZLinkedListIterator.Current.GetSubject() == obj)
			{
				EZAnimation current = eZLinkedListIterator.Current;
				if (current.running)
				{
					if (end)
					{
						current._end();
					}
					else
					{
						current._stop();
					}
					animations.Remove(current);
					ReturnAnimToPool(current);
					continue;
				}
			}
			eZLinkedListIterator.Next();
		}
		eZLinkedListIterator.End();
	}

	public void Stop(object obj, EZAnimation.ANIM_TYPE type, bool end)
	{
		EZLinkedListIterator<EZAnimation> eZLinkedListIterator = animations.Begin();
		while (!eZLinkedListIterator.Done)
		{
			if (eZLinkedListIterator.Current.GetSubject() == obj && eZLinkedListIterator.Current.type == type)
			{
				EZAnimation current = eZLinkedListIterator.Current;
				if (current.running)
				{
					if (end)
					{
						current._end();
					}
					else
					{
						current._stop();
					}
					animations.Remove(current);
					ReturnAnimToPool(current);
					continue;
				}
			}
			eZLinkedListIterator.Next();
		}
		eZLinkedListIterator.End();
	}

	public void End(object obj)
	{
		Stop(obj, true);
	}

	public void EndAll()
	{
		EZLinkedListIterator<EZAnimation> eZLinkedListIterator = animations.Begin();
		while (!eZLinkedListIterator.Done)
		{
			eZLinkedListIterator.Current.End();
		}
		eZLinkedListIterator.End();
	}

	public void StopAll()
	{
		EZLinkedListIterator<EZAnimation> eZLinkedListIterator = animations.Begin();
		while (!eZLinkedListIterator.Done)
		{
			eZLinkedListIterator.Current.Stop();
		}
		eZLinkedListIterator.End();
	}

	public void PauseAll()
	{
		EZLinkedListIterator<EZAnimation> eZLinkedListIterator = animations.Begin();
		while (!eZLinkedListIterator.Done)
		{
			eZLinkedListIterator.Current.Paused = true;
			eZLinkedListIterator.Next();
		}
		eZLinkedListIterator.End();
	}

	public void UnpauseAll()
	{
		EZLinkedListIterator<EZAnimation> eZLinkedListIterator = animations.Begin();
		while (!eZLinkedListIterator.Done)
		{
			eZLinkedListIterator.Current.Paused = false;
			eZLinkedListIterator.Next();
		}
		eZLinkedListIterator.End();
	}

	public static int GetNumAnimations()
	{
		return animations.Count;
	}
}
