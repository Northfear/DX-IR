using System;
using UnityEngine;

public class EZDragDropHelper
{
	public delegate void UpdateDragPositionDelegate(POINTER_INFO ptr);

	public delegate void DragDrop_InternalDelegate(ref POINTER_INFO ptr);

	public IUIObject host;

	protected UpdateDragPositionDelegate dragPosUpdateDel;

	protected DragDrop_InternalDelegate dragDrop_InternalDel;

	private Vector3 touchCompensationOffset = Vector3.zero;

	protected Vector3 dragOrigin;

	protected Vector3 dragOriginOffset;

	protected Plane dragPlane;

	protected bool isDragging;

	protected bool isCanceling;

	protected bool useDefaultCancelDragAnim = true;

	protected GameObject dropTarget;

	protected bool dropHandled;

	protected EZDragDropDelegate dragDropDelegate;

	private Plane DragPlane
	{
		get
		{
			return dragPlane;
		}
	}

	public bool UseDefaultCancelDragAnim
	{
		get
		{
			return useDefaultCancelDragAnim;
		}
		set
		{
			useDefaultCancelDragAnim = value;
		}
	}

	public bool IsDragging
	{
		get
		{
			return isDragging;
		}
		set
		{
			if (isDragging && !value)
			{
				CancelDrag();
			}
			isDragging = value;
		}
	}

	public bool IsCanceling
	{
		get
		{
			return isCanceling;
		}
	}

	public GameObject DropTarget
	{
		get
		{
			return dropTarget;
		}
		set
		{
			if (!(value == host.gameObject) && dropTarget != value)
			{
				if (dropTarget != null)
				{
					OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.DragExit, host, default(POINTER_INFO)));
				}
				dropTarget = value;
				if (value != null)
				{
					OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.DragEnter, host, default(POINTER_INFO)));
				}
			}
		}
	}

	public bool DropHandled
	{
		get
		{
			return dropHandled;
		}
		set
		{
			dropHandled = value;
		}
	}

	public EZDragDropHelper(IUIObject h)
	{
		host = h;
		dragPosUpdateDel = DefaultDragUpdatePosition;
	}

	public EZDragDropHelper()
	{
		dragPosUpdateDel = DefaultDragUpdatePosition;
	}

	public void SetDragDropInternalDelegate(DragDrop_InternalDelegate del)
	{
		dragDrop_InternalDel = del;
	}

	public void AddDragDropInternalDelegate(DragDrop_InternalDelegate del)
	{
		dragDrop_InternalDel = (DragDrop_InternalDelegate)Delegate.Combine(dragDrop_InternalDel, del);
	}

	public void RemoveDragDropInternalDelegate(DragDrop_InternalDelegate del)
	{
		dragDrop_InternalDel = (DragDrop_InternalDelegate)Delegate.Remove(dragDrop_InternalDel, del);
	}

	public DragDrop_InternalDelegate GetDragDropInternalDelegate()
	{
		return dragDrop_InternalDel;
	}

	public void CancelFinished()
	{
		isCanceling = false;
	}

	public void SetDragPosUpdater(UpdateDragPositionDelegate del)
	{
		dragPosUpdateDel = del;
	}

	public void DragUpdatePosition(POINTER_INFO ptr)
	{
		if (dragPosUpdateDel != null)
		{
			dragPosUpdateDel(ptr);
		}
	}

	public void DefaultDragUpdatePosition(POINTER_INFO ptr)
	{
		float enter;
		dragPlane.Raycast(ptr.ray, out enter);
		host.transform.position = touchCompensationOffset + ptr.ray.origin + ptr.ray.direction * (enter - host.DragOffset);
	}

	public void CancelDrag()
	{
		if (isDragging)
		{
			EZDragDropParams parms = new EZDragDropParams(EZDragDropEvent.Cancelled, host, default(POINTER_INFO));
			OnEZDragDrop_Internal(parms);
			dropTarget = null;
			dropHandled = false;
			isDragging = false;
			isCanceling = true;
			if (useDefaultCancelDragAnim)
			{
				DoDefaultCancelDrag();
			}
			POINTER_INFO ptr = default(POINTER_INFO);
			ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
			host.OnInput(ptr);
			if (UIManager.Exists())
			{
				UIManager.instance.Detarget(host);
			}
		}
	}

	public void DoDefaultCancelDrag()
	{
		AnimatePosition.Do(host.gameObject, EZAnimation.ANIM_MODE.To, dragOriginOffset, EZAnimation.GetInterpolator(host.CancelDragEasing), host.CancelDragDuration, 0f, null, FinishCancelDrag);
	}

	protected void FinishCancelDrag(EZAnimation anim)
	{
		if (host != null)
		{
			host.transform.localPosition = dragOrigin;
			isCanceling = false;
			OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.CancelDone, host, default(POINTER_INFO)));
		}
	}

	public void OnEZDragDrop_Internal(EZDragDropParams parms)
	{
		if (parms.evt == EZDragDropEvent.Begin)
		{
			if (isCanceling)
			{
				return;
			}
			isDragging = true;
			dropHandled = false;
			Transform transform = host.transform;
			dragOrigin = transform.localPosition;
			Transform transform2 = parms.ptr.camera.transform;
			dragPlane.SetNormalAndPosition(transform2.forward * -1f, transform.position);
			Ray ray = parms.ptr.camera.ScreenPointToRay(parms.ptr.camera.WorldToScreenPoint(transform.position));
			float enter;
			dragPlane.Raycast(ray, out enter);
			dragOriginOffset = ray.origin + ray.direction * (enter - host.DragOffset);
			if (transform.parent != null)
			{
				dragOriginOffset = transform.parent.InverseTransformPoint(dragOriginOffset);
			}
			dragPlane.Raycast(parms.ptr.ray, out enter);
			touchCompensationOffset = transform.position - (parms.ptr.ray.origin + parms.ptr.ray.direction * enter);
		}
		if (dragDropDelegate != null)
		{
			dragDropDelegate(parms);
		}
		if (dropTarget != null)
		{
			dropTarget.SendMessage("OnEZDragDrop", parms, SendMessageOptions.DontRequireReceiver);
		}
		host.gameObject.SendMessage("OnEZDragDrop", parms, SendMessageOptions.DontRequireReceiver);
		if (parms.evt == EZDragDropEvent.Dropped && parms.dragObj.Equals(host))
		{
			if (dropHandled)
			{
				isDragging = false;
				dropTarget = null;
			}
			else
			{
				CancelDrag();
			}
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
}
