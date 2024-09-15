using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EZTransition
{
	public delegate void OnTransitionEndDelegate(EZTransition transition);

	public delegate void OnTransitionStartDelegate(EZTransition transition);

	public string name;

	public EZAnimation.ANIM_TYPE[] animationTypes = new EZAnimation.ANIM_TYPE[0];

	public AnimParams[] animParams = new AnimParams[0];

	[NonSerialized]
	protected EZLinkedList<EZLinkedListNode<EZAnimation>> runningAnims = new EZLinkedList<EZLinkedListNode<EZAnimation>>();

	protected EZLinkedList<EZLinkedListNode<EZAnimation>> idleAnims = new EZLinkedList<EZLinkedListNode<EZAnimation>>();

	[NonSerialized]
	protected GameObject mainSubject;

	[NonSerialized]
	protected EZLinkedList<EZLinkedListNode<GameObject>> subSubjects = new EZLinkedList<EZLinkedListNode<GameObject>>();

	[NonSerialized]
	protected OnTransitionEndDelegate onEndDelegates;

	[NonSerialized]
	protected OnTransitionStartDelegate onStartDelegates;

	public bool initialized;

	protected bool forcedStop;

	public EZLinkedList<EZLinkedListNode<GameObject>> SubSubjects
	{
		get
		{
			return subSubjects;
		}
	}

	public GameObject MainSubject
	{
		get
		{
			return mainSubject;
		}
		set
		{
			mainSubject = value;
		}
	}

	public EZTransition(string n)
	{
		name = n;
		runningAnims = null;
	}

	public void AddTransitionStartDelegate(OnTransitionStartDelegate del)
	{
		onStartDelegates = (OnTransitionStartDelegate)Delegate.Combine(onStartDelegates, del);
	}

	public void RemoveTransitionStartDelegate(OnTransitionStartDelegate del)
	{
		onStartDelegates = (OnTransitionStartDelegate)Delegate.Remove(onStartDelegates, del);
	}

	public void AddTransitionEndDelegate(OnTransitionEndDelegate del)
	{
		onEndDelegates = (OnTransitionEndDelegate)Delegate.Combine(onEndDelegates, del);
	}

	public void RemoveTransitionEndDelegate(OnTransitionEndDelegate del)
	{
		onEndDelegates = (OnTransitionEndDelegate)Delegate.Remove(onEndDelegates, del);
	}

	public void Copy(EZTransition src)
	{
		initialized = false;
		if (src.animationTypes != null)
		{
			animationTypes = new EZAnimation.ANIM_TYPE[src.animationTypes.Length];
			src.animationTypes.CopyTo(animationTypes, 0);
			animParams = new AnimParams[src.animParams.Length];
			for (int i = 0; i < animParams.Length; i++)
			{
				animParams[i] = new AnimParams(this);
				animParams[i].Copy(src.animParams[i]);
			}
		}
	}

	public void AddSubSubject(GameObject go)
	{
		if (subSubjects == null)
		{
			subSubjects = new EZLinkedList<EZLinkedListNode<GameObject>>();
		}
		subSubjects.Add(new EZLinkedListNode<GameObject>(go));
	}

	public void RemoveSubSubject(GameObject go)
	{
		if (subSubjects == null)
		{
			return;
		}
		EZLinkedListNode<GameObject> current = subSubjects.Current;
		if (subSubjects.Rewind())
		{
			do
			{
				if (subSubjects.Current.val == go)
				{
					subSubjects.Remove(subSubjects.Current);
					break;
				}
			}
			while (subSubjects.MoveNext());
		}
		subSubjects.Current = current;
	}

	public void OnAnimEnd(EZAnimation anim)
	{
		EZLinkedListNode<EZAnimation> eZLinkedListNode = (EZLinkedListNode<EZAnimation>)anim.Data;
		if (eZLinkedListNode == null || runningAnims == null)
		{
			return;
		}
		eZLinkedListNode.val = null;
		runningAnims.Remove(eZLinkedListNode);
		idleAnims.Add(eZLinkedListNode);
		if (onEndDelegates == null || forcedStop)
		{
			return;
		}
		EZLinkedListNode<EZAnimation> current = runningAnims.Current;
		if (runningAnims.Rewind())
		{
			do
			{
				if (runningAnims.Current.val.Duration > 0f)
				{
					return;
				}
			}
			while (runningAnims.MoveNext());
		}
		runningAnims.Current = current;
		CallEndDelegates();
	}

	public EZLinkedListNode<EZAnimation> AddRunningAnim()
	{
		if (runningAnims == null)
		{
			runningAnims = new EZLinkedList<EZLinkedListNode<EZAnimation>>();
			if (idleAnims == null)
			{
				idleAnims = new EZLinkedList<EZLinkedListNode<EZAnimation>>();
			}
		}
		EZLinkedListNode<EZAnimation> eZLinkedListNode;
		if (idleAnims.Count > 0)
		{
			eZLinkedListNode = idleAnims.Head;
			idleAnims.Remove(eZLinkedListNode);
		}
		else
		{
			eZLinkedListNode = new EZLinkedListNode<EZAnimation>(null);
		}
		runningAnims.Add(eZLinkedListNode);
		return eZLinkedListNode;
	}

	public void Start()
	{
		if (!(mainSubject == null) || (subSubjects != null && subSubjects.Count >= 1))
		{
			StopSafe();
			if (onStartDelegates != null)
			{
				onStartDelegates(this);
			}
			EZAnimator.instance.AddTransition(this);
			if (runningAnims == null || runningAnims.Count < 1)
			{
				CallEndDelegates();
			}
		}
	}

	public void End()
	{
		if (runningAnims == null || !runningAnims.Rewind())
		{
			return;
		}
		forcedStop = true;
		do
		{
			EZLinkedListNode<EZAnimation> current = runningAnims.Current;
			EZAnimation val = current.val;
			if (val != null)
			{
				val.CompletedDelegate = null;
				EZAnimator.instance.StopAnimation(val, true);
			}
			runningAnims.Remove(current);
			idleAnims.Add(current);
			current.val = null;
		}
		while (runningAnims.MoveNext());
		forcedStop = false;
		CallEndDelegates();
	}

	public void StopSafe()
	{
		if (runningAnims == null)
		{
			return;
		}
		EZLinkedListNode<EZAnimation> current = runningAnims.Current;
		if (runningAnims.Rewind())
		{
			forcedStop = true;
			do
			{
				EZLinkedListNode<EZAnimation> current2 = runningAnims.Current;
				EZAnimation val = current2.val;
				if (val != null)
				{
					val.CompletedDelegate = null;
					if (val.Mode == EZAnimation.ANIM_MODE.By)
					{
						EZAnimator.instance.StopAnimation(val, true);
					}
					else
					{
						EZAnimator.instance.StopAnimation(val, false);
					}
				}
				runningAnims.Remove(current2);
				idleAnims.Add(current2);
				current2.val = null;
			}
			while (runningAnims.MoveNext());
			forcedStop = false;
			CallEndDelegates();
		}
		runningAnims.Current = current;
	}

	public void Pause()
	{
		EZLinkedListIterator<EZLinkedListNode<EZAnimation>> eZLinkedListIterator = runningAnims.Begin();
		while (!eZLinkedListIterator.Done)
		{
			eZLinkedListIterator.Current.val.Paused = true;
			eZLinkedListIterator.Next();
		}
		eZLinkedListIterator.End();
	}

	public void Unpause()
	{
		EZLinkedListIterator<EZLinkedListNode<EZAnimation>> eZLinkedListIterator = runningAnims.Begin();
		while (!eZLinkedListIterator.Done)
		{
			eZLinkedListIterator.Current.val.Paused = false;
			eZLinkedListIterator.Next();
		}
		eZLinkedListIterator.End();
	}

	public bool IsRunning()
	{
		if (runningAnims == null)
		{
			return false;
		}
		EZLinkedListIterator<EZLinkedListNode<EZAnimation>> eZLinkedListIterator = runningAnims.Begin();
		while (!eZLinkedListIterator.Done)
		{
			if (eZLinkedListIterator.Current.val.Duration > 0f)
			{
				eZLinkedListIterator.End();
				return true;
			}
			eZLinkedListIterator.Next();
		}
		eZLinkedListIterator.End();
		return false;
	}

	public bool IsRunningAtAll()
	{
		if (runningAnims == null)
		{
			return false;
		}
		if (runningAnims.Count > 0)
		{
			return true;
		}
		return false;
	}

	protected void CallEndDelegates()
	{
		if (!forcedStop && onEndDelegates != null)
		{
			onEndDelegates(this);
		}
	}

	public int Add()
	{
		initialized = true;
		List<EZAnimation.ANIM_TYPE> list = new List<EZAnimation.ANIM_TYPE>();
		if (animationTypes.Length > 0)
		{
			list.AddRange(animationTypes);
		}
		list.Add(EZAnimation.ANIM_TYPE.Translate);
		animationTypes = list.ToArray();
		List<AnimParams> list2 = new List<AnimParams>();
		if (animParams.Length > 0)
		{
			list2.AddRange(animParams);
		}
		list2.Add(new AnimParams(this));
		animParams = list2.ToArray();
		return animationTypes.Length - 1;
	}

	public AnimParams AddElement(EZAnimation.ANIM_TYPE type)
	{
		int num = Add();
		animationTypes[num] = type;
		return animParams[num];
	}

	public void Remove(int index)
	{
		initialized = true;
		List<EZAnimation.ANIM_TYPE> list = new List<EZAnimation.ANIM_TYPE>();
		if (animationTypes.Length > 0)
		{
			list.AddRange(animationTypes);
		}
		list.RemoveAt(index);
		animationTypes = list.ToArray();
		List<AnimParams> list2 = new List<AnimParams>();
		if (animParams.Length > 0)
		{
			list2.AddRange(animParams);
		}
		list2.RemoveAt(index);
		animParams = list2.ToArray();
	}

	public void SetElementType(int index, EZAnimation.ANIM_TYPE type)
	{
		if (index < animationTypes.Length)
		{
			if (animationTypes[index] != type)
			{
				initialized = true;
			}
			animationTypes[index] = type;
		}
	}

	public string[] GetNames()
	{
		string[] array = new string[animationTypes.Length];
		for (int i = 0; i < animationTypes.Length; i++)
		{
			array[i] = i + " - " + Enum.GetName(typeof(EZAnimation.ANIM_TYPE), animationTypes[i]);
			if (animParams[i].transition != this)
			{
				animParams[i].transition = this;
			}
		}
		return array;
	}
}
