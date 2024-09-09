using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class UIPanelBase : MonoBehaviour, IEZDragDrop, IUIContainer, IUIObject
{
	public delegate void TransitionCompleteDelegate(UIPanelBase panel, EZTransition transition);

	protected EZLinkedList<EZLinkedListNode<IUIObject>> uiObjs = new EZLinkedList<EZLinkedListNode<IUIObject>>();

	protected EZLinkedList<EZLinkedListNode<UIPanelBase>> childPanels = new EZLinkedList<EZLinkedListNode<UIPanelBase>>();

	[HideInInspector]
	public bool[] blockInput = new bool[4] { true, true, true, true };

	protected EZTransition prevTransition;

	protected int prevTransIndex;

	protected bool m_started;

	public int index;

	public bool deactivateAllOnDismiss;

	public bool detargetOnDisable;

	[NonSerialized]
	protected Dictionary<int, GameObject> subjects = new Dictionary<int, GameObject>();

	protected TransitionCompleteDelegate tempTransCompleteDel;

	protected bool m_controlIsEnabled = true;

	protected IUIContainer container;

	protected EZInputDelegate inputDelegate;

	protected EZValueChangedDelegate changeDelegate;

	protected EZDragDropDelegate dragDropDelegate;

	public abstract EZTransitionList Transitions { get; }

	public bool IsTransitioning
	{
		get
		{
			return prevTransition != null;
		}
	}

	public virtual bool controlIsEnabled
	{
		get
		{
			return m_controlIsEnabled;
		}
		set
		{
			m_controlIsEnabled = value;
		}
	}

	public virtual bool DetargetOnDisable
	{
		get
		{
			return detargetOnDisable;
		}
		set
		{
			detargetOnDisable = value;
		}
	}

	public virtual IUIContainer Container
	{
		get
		{
			return container;
		}
		set
		{
			IUIContainer iUIContainer = container;
			container = value;
			if (container != null)
			{
				foreach (KeyValuePair<int, GameObject> subject in subjects)
				{
					container.AddSubject(subject.Value);
				}
			}
			if (iUIContainer == null || iUIContainer == container)
			{
				return;
			}
			foreach (KeyValuePair<int, GameObject> subject2 in subjects)
			{
				iUIContainer.RemoveSubject(subject2.Value);
			}
		}
	}

	public object Data
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public bool IsDraggable
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public LayerMask DropMask
	{
		get
		{
			return -1;
		}
		set
		{
		}
	}

	public float DragOffset
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public EZAnimation.EASING_TYPE CancelDragEasing
	{
		get
		{
			return EZAnimation.EASING_TYPE.Default;
		}
		set
		{
		}
	}

	public float CancelDragDuration
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public bool IsDragging
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public GameObject DropTarget
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public bool DropHandled
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying)
		{
			if (EZAnimator.Exists())
			{
				EZAnimator.instance.Stop(base.gameObject);
			}
			if (detargetOnDisable && UIManager.Exists())
			{
				UIManager.instance.Detarget(this);
			}
		}
	}

	public virtual void Start()
	{
		if (!m_started)
		{
			m_started = true;
			ScanChildren();
			for (int i = 0; i < Transitions.list.Length; i++)
			{
				Transitions.list[i].MainSubject = base.gameObject;
			}
			SetupTransitionSubjects();
		}
	}

	public void ScanChildren()
	{
		uiObjs.Clear();
		Component[] componentsInChildren = base.transform.GetComponentsInChildren(typeof(IUIObject), true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!(componentsInChildren[i] == this) && !(componentsInChildren[i].gameObject == base.gameObject))
			{
				if (base.gameObject.layer == UIManager.instance.gameObject.layer)
				{
					UIPanelManager.SetLayerRecursively(componentsInChildren[i].gameObject, base.gameObject.layer);
				}
				IUIObject iUIObject = (IUIObject)componentsInChildren[i];
				uiObjs.Add(new EZLinkedListNode<IUIObject>(iUIObject));
				iUIObject.RequestContainership(this);
			}
		}
		componentsInChildren = base.transform.GetComponentsInChildren(typeof(UIPanelBase), true);
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			if (!(componentsInChildren[j] == this) && !(componentsInChildren[j].gameObject == base.gameObject))
			{
				if (base.gameObject.layer == UIManager.instance.gameObject.layer)
				{
					UIPanelManager.SetLayerRecursively(componentsInChildren[j].gameObject, base.gameObject.layer);
				}
				UIPanelBase uIPanelBase = (UIPanelBase)componentsInChildren[j];
				childPanels.Add(new EZLinkedListNode<UIPanelBase>(uIPanelBase));
				uIPanelBase.RequestContainership(this);
			}
		}
	}

	protected virtual void SetupTransitionSubjects()
	{
		for (int i = 0; i < 4; i++)
		{
			Transitions.list[i].AddTransitionEndDelegate(TransitionCompleted);
		}
		if (uiObjs.Rewind())
		{
			do
			{
				GameObject gameObject = ((Component)uiObjs.Current.val).gameObject;
				int hashCode = gameObject.GetHashCode();
				for (int j = 0; j < Transitions.list.Length; j++)
				{
					Transitions.list[j].AddSubSubject(gameObject);
				}
				if (!subjects.ContainsKey(hashCode))
				{
					subjects.Add(hashCode, gameObject);
				}
			}
			while (uiObjs.MoveNext());
		}
		Component[] componentsInChildren = base.transform.GetComponentsInChildren(typeof(SpriteRoot), true);
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			if (componentsInChildren[k].gameObject == base.gameObject)
			{
				continue;
			}
			GameObject gameObject = componentsInChildren[k].gameObject;
			int hashCode = gameObject.GetHashCode();
			if (!subjects.ContainsKey(hashCode))
			{
				for (int l = 0; l < Transitions.list.Length; l++)
				{
					Transitions.list[l].AddSubSubject(gameObject);
				}
				subjects.Add(hashCode, gameObject);
			}
		}
		Component[] componentsInChildren2 = base.transform.GetComponentsInChildren(typeof(SpriteText), true);
		for (int m = 0; m < componentsInChildren2.Length; m++)
		{
			if (componentsInChildren2[m].gameObject == base.gameObject)
			{
				continue;
			}
			GameObject gameObject = componentsInChildren2[m].gameObject;
			int hashCode = gameObject.GetHashCode();
			if (!subjects.ContainsKey(hashCode))
			{
				for (int n = 0; n < Transitions.list.Length; n++)
				{
					Transitions.list[n].AddSubSubject(gameObject);
				}
				subjects.Add(hashCode, gameObject);
			}
		}
		Component[] componentsInChildren3 = base.transform.GetComponentsInChildren(typeof(Renderer), true);
		for (int num = 0; num < componentsInChildren3.Length; num++)
		{
			if (componentsInChildren3[num].gameObject == base.gameObject)
			{
				continue;
			}
			GameObject gameObject = componentsInChildren3[num].gameObject;
			int hashCode = gameObject.GetHashCode();
			if (!subjects.ContainsKey(hashCode))
			{
				for (int num2 = 0; num2 < Transitions.list.Length; num2++)
				{
					Transitions.list[num2].AddSubSubject(gameObject);
				}
				subjects.Add(hashCode, gameObject);
			}
		}
	}

	public void AddChild(GameObject go)
	{
		IUIObject iUIObject = (IUIObject)go.GetComponent("IUIObject");
		if (iUIObject != null)
		{
			if (iUIObject.Container != this)
			{
				iUIObject.Container = this;
			}
			uiObjs.Add(new EZLinkedListNode<IUIObject>(iUIObject));
		}
		else
		{
			UIPanelBase uIPanelBase = (UIPanelBase)go.GetComponent(typeof(UIPanelBase));
			if (uIPanelBase != null)
			{
				if (uIPanelBase.Container != this)
				{
					uIPanelBase.Container = this;
				}
				childPanels.Add(new EZLinkedListNode<UIPanelBase>(uIPanelBase));
			}
		}
		if (!base.gameObject.active)
		{
			go.SetActiveRecursively(false);
		}
		AddSubject(go);
	}

	public void RemoveChild(GameObject go)
	{
		IUIObject iUIObject = (IUIObject)go.GetComponent("IUIObject");
		if (iUIObject != null)
		{
			if (uiObjs.Rewind())
			{
				do
				{
					if (uiObjs.Current.val == iUIObject)
					{
						uiObjs.Remove(uiObjs.Current);
						break;
					}
				}
				while (uiObjs.MoveNext());
			}
			if (iUIObject.Container == this)
			{
				iUIObject.Container = null;
			}
		}
		else
		{
			UIPanelBase uIPanelBase = (UIPanelBase)go.GetComponent(typeof(UIPanelBase));
			if (uIPanelBase != null)
			{
				if (childPanels.Rewind())
				{
					do
					{
						if (childPanels.Current.val == uIPanelBase)
						{
							childPanels.Remove(childPanels.Current);
							break;
						}
					}
					while (childPanels.MoveNext());
				}
				if (uIPanelBase.Container == this)
				{
					uIPanelBase.Container = null;
				}
			}
		}
		RemoveSubject(go);
	}

	public void MakeChild(GameObject go)
	{
		AddChild(go);
		go.transform.parent = base.transform;
	}

	public void AddSubject(GameObject go)
	{
		int hashCode = go.GetHashCode();
		if (!subjects.ContainsKey(hashCode))
		{
			subjects.Add(hashCode, go);
			for (int i = 0; i < Transitions.list.Length; i++)
			{
				Transitions.list[i].AddSubSubject(go);
			}
			if (container != null)
			{
				container.AddSubject(go);
			}
		}
	}

	public void RemoveSubject(GameObject go)
	{
		int hashCode = go.GetHashCode();
		if (subjects.ContainsKey(hashCode))
		{
			subjects.Remove(hashCode);
			for (int i = 0; i < Transitions.list.Length; i++)
			{
				Transitions.list[i].RemoveSubSubject(go);
			}
			if (container != null)
			{
				container.RemoveSubject(go);
			}
		}
	}

	public string[] GetTransitionNames()
	{
		if (Transitions == null)
		{
			return null;
		}
		string[] array = new string[Transitions.list.Length];
		for (int i = 0; i < Transitions.list.Length; i++)
		{
			array[i] = Transitions.list[i].name;
		}
		return array;
	}

	public EZTransition GetTransition(int index)
	{
		if (Transitions == null)
		{
			return null;
		}
		if (Transitions.list == null)
		{
			return null;
		}
		if (Transitions.list.Length <= index || index < 0)
		{
			return null;
		}
		return Transitions.list[index];
	}

	public EZTransition GetTransition(UIPanelManager.SHOW_MODE transition)
	{
		return GetTransition((int)transition);
	}

	public EZTransition GetTransition(string transName)
	{
		if (Transitions == null)
		{
			return null;
		}
		if (Transitions.list == null)
		{
			return null;
		}
		EZTransition[] list = Transitions.list;
		for (int i = 0; i < list.Length; i++)
		{
			if (string.Equals(list[i].name, transName, StringComparison.CurrentCultureIgnoreCase))
			{
				return list[i];
			}
		}
		return null;
	}

	public virtual void StartTransition(UIPanelManager.SHOW_MODE mode)
	{
		if (!m_started)
		{
			Start();
		}
		if (prevTransition != null)
		{
			prevTransition.StopSafe();
		}
		prevTransIndex = (int)mode;
		if (blockInput[prevTransIndex])
		{
			UIManager.instance.LockInput();
		}
		prevTransition = Transitions.list[prevTransIndex];
		if (deactivateAllOnDismiss && (mode == UIPanelManager.SHOW_MODE.BringInBack || mode == UIPanelManager.SHOW_MODE.BringInForward))
		{
			base.gameObject.SetActiveRecursively(true);
			Start();
		}
		prevTransition.Start();
	}

	public virtual void StartTransition(string transName)
	{
		if (!m_started)
		{
			Start();
		}
		EZTransition[] list = Transitions.list;
		for (int i = 0; i < list.Length; i++)
		{
			if (string.Equals(list[i].name, transName, StringComparison.CurrentCultureIgnoreCase))
			{
				if (prevTransition != null)
				{
					prevTransition.StopSafe();
				}
				prevTransIndex = i;
				if (blockInput[prevTransIndex])
				{
					UIManager.instance.LockInput();
				}
				prevTransition = list[prevTransIndex];
				if (deactivateAllOnDismiss && (prevTransition == list[1] || prevTransition == list[0]))
				{
					base.gameObject.SetActiveRecursively(true);
					Start();
				}
				prevTransition.Start();
			}
		}
	}

	public void TransitionCompleted(EZTransition transition)
	{
		prevTransition = null;
		if (deactivateAllOnDismiss && (transition == Transitions.list[2] || transition == Transitions.list[3]))
		{
			base.gameObject.SetActiveRecursively(false);
		}
		if (tempTransCompleteDel != null)
		{
			tempTransCompleteDel(this, transition);
		}
		tempTransCompleteDel = null;
		if (blockInput[prevTransIndex] && UIManager.Exists())
		{
			UIManager.instance.UnlockInput();
		}
	}

	public virtual void BringIn()
	{
		StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
	}

	public virtual void BringInImmediate()
	{
		StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
		EZTransition transition = GetTransition(UIPanelManager.SHOW_MODE.BringInForward);
		if (transition != null)
		{
			transition.End();
		}
	}

	public virtual void Dismiss()
	{
		StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
	}

	public virtual void DismissImmediate()
	{
		StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
		EZTransition transition = GetTransition(UIPanelManager.SHOW_MODE.DismissForward);
		if (transition != null)
		{
			transition.End();
		}
	}

	public static int CompareIndices(UIPanelBase a, UIPanelBase b)
	{
		return a.index - b.index;
	}

	public void AddTempTransitionDelegate(TransitionCompleteDelegate del)
	{
		tempTransCompleteDel = (TransitionCompleteDelegate)Delegate.Combine(tempTransCompleteDel, del);
	}

	public IUIObject GetControl(ref POINTER_INFO ptr)
	{
		return this;
	}

	public bool RequestContainership(IUIContainer cont)
	{
		Transform parent = base.transform.parent;
		Transform transform = ((Component)cont).transform;
		while (parent != null)
		{
			if (parent == transform)
			{
				container = cont;
				return true;
			}
			if (parent.gameObject.GetComponent("IUIContainer") != null)
			{
				return false;
			}
			parent = parent.parent;
		}
		return false;
	}

	public bool GotFocus()
	{
		return false;
	}

	public virtual void SetInputDelegate(EZInputDelegate del)
	{
		inputDelegate = del;
	}

	public virtual void AddInputDelegate(EZInputDelegate del)
	{
		inputDelegate = (EZInputDelegate)Delegate.Combine(inputDelegate, del);
	}

	public virtual void RemoveInputDelegate(EZInputDelegate del)
	{
		inputDelegate = (EZInputDelegate)Delegate.Remove(inputDelegate, del);
	}

	public virtual void SetValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate = del;
	}

	public virtual void AddValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate = (EZValueChangedDelegate)Delegate.Combine(changeDelegate, del);
	}

	public virtual void RemoveValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate = (EZValueChangedDelegate)Delegate.Remove(changeDelegate, del);
	}

	public virtual void OnInput(POINTER_INFO ptr)
	{
		if (Container != null)
		{
			ptr.callerIsControl = true;
			Container.OnInput(ptr);
		}
	}

	public void DragUpdatePosition(POINTER_INFO ptr)
	{
	}

	public void CancelDrag()
	{
	}

	public void OnEZDragDrop_Internal(EZDragDropParams parms)
	{
		if (dragDropDelegate != null)
		{
			dragDropDelegate(parms);
		}
	}

	public void AddDragDropDelegate(EZDragDropDelegate del)
	{
		dragDropDelegate = (EZDragDropDelegate)Delegate.Combine(dragDropDelegate, del);
	}

	public void RemoveDragDropDelegate(EZDragDropDelegate del)
	{
		dragDropDelegate = (EZDragDropDelegate)Delegate.Remove(dragDropDelegate, del);
	}

	public void SetDragDropDelegate(EZDragDropDelegate del)
	{
		dragDropDelegate = del;
	}

	public void SetDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del)
	{
	}

	public void AddDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del)
	{
	}

	public void RemoveDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del)
	{
	}

	public EZDragDropHelper.DragDrop_InternalDelegate GetDragDropInternalDelegate()
	{
		return null;
	}

	virtual GameObject IUIObject.get_gameObject()
	{
		return base.gameObject;
	}

	virtual Transform IUIObject.get_transform()
	{
		return base.transform;
	}

	virtual string IUIObject.get_name()
	{
		return base.name;
	}
}
