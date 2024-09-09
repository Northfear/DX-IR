using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[AddComponentMenu("EZ GUI/Controls/Virtual Screen")]
[RequireComponent(typeof(MeshCollider))]
public class UIVirtualScreen : MonoBehaviour, IEZDragDrop, IUIObject
{
	public Camera screenCamera;

	public LayerMask layerMask = -1;

	public float rayDepth = float.PositiveInfinity;

	public bool processPointerInfo = true;

	public GameObject controlParent;

	public bool onlyRenderWhenNeeded;

	public float renderTimeout;

	protected List<IUIObject> controls = new List<IUIObject>();

	protected bool shuttingDown;

	protected IUIContainer container;

	public bool controlIsEnabled
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool DetargetOnDisable
	{
		get
		{
			return false;
		}
		set
		{
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
			container = value;
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
			return EZAnimation.EASING_TYPE.Linear;
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

	public virtual void Awake()
	{
		MeshCollider meshCollider = (MeshCollider)GetComponent(typeof(MeshCollider));
		if (meshCollider != null)
		{
			meshCollider.isTrigger = true;
		}
		else
		{
			Debug.LogError("The object \"" + base.name + "\" does not have the required MeshCollider attached.  Please add one, or else the screen functionality will not work.");
		}
		if (screenCamera == null)
		{
			screenCamera = Camera.main;
		}
		if (processPointerInfo)
		{
			SetupControls();
		}
	}

	public virtual IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		if (onlyRenderWhenNeeded && screenCamera != null)
		{
			screenCamera.gameObject.active = false;
		}
	}

	protected void SetupControls()
	{
		for (int i = 0; i < controls.Count; i++)
		{
			controls[i].RemoveInputDelegate(InputProcessor);
			controls[i].AddDragDropInternalDelegate(InputProcessor);
		}
		controls.Clear();
		if (!(controlParent == null))
		{
			Component[] componentsInChildren = controlParent.GetComponentsInChildren(typeof(IUIObject), true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				IUIObject iUIObject = (IUIObject)componentsInChildren[j];
				controls.Add(iUIObject);
				iUIObject.AddInputDelegate(InputProcessor);
				iUIObject.AddDragDropInternalDelegate(InputProcessor);
			}
		}
	}

	public void AddControl(IUIObject obj)
	{
		if (obj != null)
		{
			controls.Add(obj);
			obj.AddInputDelegate(InputProcessor);
		}
	}

	public void RemoveControl(IUIObject obj)
	{
		controls.Remove(obj);
		obj.RemoveInputDelegate(InputProcessor);
	}

	public void SetScreenCamera(Camera cam)
	{
		screenCamera = cam;
	}

	public void SetControlParent(GameObject go)
	{
		controlParent = go;
		if (processPointerInfo)
		{
			SetupControls();
		}
	}

	protected void InputProcessor(ref POINTER_INFO ptr)
	{
		shuttingDown = false;
		StopAllCoroutines();
		ptr.devicePos = new Vector3(ptr.hitInfo.textureCoord.x * screenCamera.pixelWidth, ptr.hitInfo.textureCoord.y * screenCamera.pixelHeight, 0f);
		Vector3 vector = ptr.devicePos;
		float z = ptr.inputDelta.z;
		RaycastHit hitInfo;
		if (ptr.prevRay.direction.sqrMagnitude > 0f && base.collider.Raycast(ptr.prevRay, out hitInfo, ptr.rayDepth))
		{
			vector = new Vector3(hitInfo.textureCoord.x * screenCamera.pixelWidth, hitInfo.textureCoord.y * screenCamera.pixelHeight, 0f);
			ptr.inputDelta = ptr.devicePos - vector;
		}
		else
		{
			ptr.inputDelta = Vector3.zero;
		}
		ptr.inputDelta.z = z;
		ptr.ray = screenCamera.ScreenPointToRay(ptr.devicePos);
		ptr.prevRay = screenCamera.ScreenPointToRay(vector);
		ptr.camera = screenCamera;
		ptr.rayDepth = rayDepth;
		ptr.layerMask = layerMask;
		Physics.Raycast(ptr.ray, out ptr.hitInfo, rayDepth, layerMask);
		if (!onlyRenderWhenNeeded)
		{
			return;
		}
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE_OFF || ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF)
		{
			StartCoroutine(DeactivateScreenCam(renderTimeout));
		}
		else if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Component component = (Component)ptr.targetObj;
			if (!component.collider.Raycast(ptr.ray, out hitInfo, rayDepth))
			{
				StartCoroutine(DeactivateScreenCam(renderTimeout));
			}
		}
	}

	protected IEnumerator DeactivateScreenCam(float timeout)
	{
		shuttingDown = true;
		if (renderTimeout == 0f)
		{
			yield return null;
		}
		else
		{
			yield return new WaitForSeconds(renderTimeout);
		}
		yield return new WaitForEndOfFrame();
		if (shuttingDown && screenCamera != null)
		{
			screenCamera.gameObject.active = false;
		}
	}

	public void RenderFrame()
	{
		if (!(screenCamera == null) && !screenCamera.gameObject.active)
		{
			screenCamera.gameObject.active = true;
			DeactivateScreenCam(0f);
		}
	}

	public void ForceOn()
	{
		if (!(screenCamera == null))
		{
			base.gameObject.active = true;
			screenCamera.gameObject.active = true;
			shuttingDown = false;
			StopAllCoroutines();
			onlyRenderWhenNeeded = false;
		}
	}

	public IUIObject GetControl(ref POINTER_INFO ptr)
	{
		Vector2 vector = new Vector2(ptr.hitInfo.textureCoord.x * screenCamera.pixelWidth, ptr.hitInfo.textureCoord.y * screenCamera.pixelHeight);
		IUIObject iUIObject = null;
		bool flag = !screenCamera.gameObject.active;
		if (flag)
		{
			screenCamera.gameObject.active = true;
		}
		Ray ray = screenCamera.ScreenPointToRay(vector);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, rayDepth, layerMask))
		{
			iUIObject = (IUIObject)hitInfo.collider.gameObject.GetComponent("IUIObject");
		}
		if (onlyRenderWhenNeeded && iUIObject != null && iUIObject.controlIsEnabled)
		{
			shuttingDown = false;
			StopAllCoroutines();
			if (flag)
			{
				flag = false;
			}
		}
		if (flag)
		{
			screenCamera.gameObject.active = false;
		}
		return iUIObject;
	}

	public bool RequestContainership(IUIContainer cont)
	{
		Transform parent = base.transform.parent;
		Transform transform = ((Component)cont).transform;
		while (parent != null)
		{
			if (parent == transform)
			{
				Container = cont;
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

	public void OnInput(POINTER_INFO ptr)
	{
	}

	public void SetInputDelegate(EZInputDelegate del)
	{
	}

	public void AddInputDelegate(EZInputDelegate del)
	{
	}

	public void RemoveInputDelegate(EZInputDelegate del)
	{
	}

	public void SetValueChangedDelegate(EZValueChangedDelegate del)
	{
	}

	public void AddValueChangedDelegate(EZValueChangedDelegate del)
	{
	}

	public void RemoveValueChangedDelegate(EZValueChangedDelegate del)
	{
	}

	public void DragUpdatePosition(POINTER_INFO ptr)
	{
	}

	public void CancelDrag()
	{
	}

	public void OnEZDragDrop_Internal(EZDragDropParams parms)
	{
	}

	public void AddDragDropDelegate(EZDragDropDelegate del)
	{
	}

	public void RemoveDragDropDelegate(EZDragDropDelegate del)
	{
	}

	public void SetDragDropDelegate(EZDragDropDelegate del)
	{
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
