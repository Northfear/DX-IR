using UnityEngine;

public struct POINTER_INFO
{
	public enum INPUT_EVENT
	{
		NO_CHANGE = 0,
		PRESS = 1,
		RELEASE = 2,
		TAP = 3,
		MOVE = 4,
		MOVE_OFF = 5,
		RELEASE_OFF = 6,
		DRAG = 7
	}

	public enum POINTER_TYPE
	{
		MOUSE = 1,
		TOUCHPAD = 2,
		MOUSE_TOUCHPAD = 3,
		RAY = 4
	}

	public POINTER_TYPE type;

	public Camera camera;

	public int id;

	public int actionID;

	public INPUT_EVENT evt;

	public RaycastHit hitInfo;

	public bool active;

	public Vector3 devicePos;

	public Vector3 origPos;

	public Vector3 inputDelta;

	public bool isTap;

	public Ray ray;

	public Ray prevRay;

	public float rayDepth;

	public IUIObject targetObj;

	public int layerMask;

	public bool callerIsControl;

	public float activeTime;

	public void Copy(POINTER_INFO ptr)
	{
		type = ptr.type;
		camera = ptr.camera;
		id = ptr.id;
		actionID = ptr.actionID;
		evt = ptr.evt;
		active = ptr.active;
		devicePos = ptr.devicePos;
		origPos = ptr.origPos;
		inputDelta = ptr.inputDelta;
		ray = ptr.ray;
		prevRay = ptr.prevRay;
		rayDepth = ptr.rayDepth;
		isTap = ptr.isTap;
		targetObj = ptr.targetObj;
		layerMask = ptr.layerMask;
		hitInfo = ptr.hitInfo;
		activeTime = ptr.activeTime;
	}

	public void Reuse(POINTER_INFO ptr)
	{
		evt = ptr.evt;
		actionID = ptr.actionID;
		active = ptr.active;
		devicePos = ptr.devicePos;
		origPos = ptr.origPos;
		inputDelta = ptr.inputDelta;
		isTap = ptr.isTap;
		hitInfo = default(RaycastHit);
		activeTime = ptr.activeTime;
	}

	public void Reset(int actID)
	{
		actionID = actID;
		evt = INPUT_EVENT.NO_CHANGE;
		active = false;
		devicePos = Vector3.zero;
		origPos = Vector3.zero;
		inputDelta = Vector3.zero;
		ray = default(Ray);
		prevRay = default(Ray);
		isTap = true;
		hitInfo = default(RaycastHit);
		activeTime = 0f;
	}
}
