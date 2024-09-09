using System;
using UnityEngine;

[ExecuteInEditMode]
public abstract class ControlBase : MonoBehaviour, IEZDragDrop, IControl, IUIObject
{
	public const string DittoString = "[\"]";

	protected ControlBaseMirror mirror;

	public string text;

	public SpriteText spriteText;

	public float textOffsetZ = -0.1f;

	public bool includeTextInAutoCollider;

	protected SpriteText.Anchor_Pos defaultTextAnchor = SpriteText.Anchor_Pos.Middle_Center;

	protected SpriteText.Alignment_Type defaultTextAlignment = SpriteText.Alignment_Type.Center;

	protected bool deleted;

	public bool detargetOnDisable;

	protected bool customCollider;

	[HideInInspector]
	public object data;

	protected bool m_controlIsEnabled = true;

	protected IUIContainer container;

	protected EZInputDelegate inputDelegate;

	protected EZValueChangedDelegate changeDelegate;

	protected EZDragDropHelper dragDropHelper = new EZDragDropHelper();

	public bool isDraggable;

	public LayerMask dropMask = -1;

	public float dragOffset = float.NaN;

	public EZAnimation.EASING_TYPE cancelDragEasing = EZAnimation.EASING_TYPE.Default;

	public float cancelDragDuration = -1f;

	public virtual string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
			if (spriteText == null)
			{
				if (text == string.Empty)
				{
					return;
				}
				if (UIManager.instance == null)
				{
					Debug.LogWarning("Warning: No UIManager exists in the scene. A UIManager with a default font is required to automatically add text to a control.");
					return;
				}
				if (UIManager.instance.defaultFont == null)
				{
					Debug.LogWarning("Warning: No default font defined.  A UIManager object with a default font is required to automatically add text to a control.");
					return;
				}
				GameObject gameObject = new GameObject();
				gameObject.layer = base.gameObject.layer;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.name = "control_text";
				MeshRenderer meshRenderer = (MeshRenderer)gameObject.AddComponent(typeof(MeshRenderer));
				meshRenderer.material = UIManager.instance.defaultFontMaterial;
				spriteText = (SpriteText)gameObject.AddComponent(typeof(SpriteText));
				spriteText.font = UIManager.instance.defaultFont;
				spriteText.offsetZ = textOffsetZ;
				spriteText.Parent = this;
				spriteText.anchor = defaultTextAnchor;
				spriteText.alignment = defaultTextAlignment;
				spriteText.pixelPerfect = true;
				spriteText.Start();
			}
			spriteText.Text = text;
			text = spriteText.Text;
			if (includeTextInAutoCollider)
			{
				UpdateCollider();
			}
		}
	}

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

	public virtual bool IncludeTextInAutoCollider
	{
		get
		{
			return includeTextInAutoCollider;
		}
		set
		{
			includeTextInAutoCollider = value;
			UpdateCollider();
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
			return DetargetOnDisable;
		}
		set
		{
			DetargetOnDisable = value;
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
			if (container != null && spriteText != null)
			{
				container.RemoveChild(spriteText.gameObject);
			}
			if (value != null && spriteText != null)
			{
				value.AddChild(spriteText.gameObject);
			}
			container = value;
		}
	}

	public bool IsDraggable
	{
		get
		{
			return isDraggable;
		}
		set
		{
			isDraggable = value;
		}
	}

	public LayerMask DropMask
	{
		get
		{
			return dropMask;
		}
		set
		{
			dropMask = value;
		}
	}

	public float DragOffset
	{
		get
		{
			return dragOffset;
		}
		set
		{
			dragOffset = value;
			POINTER_INFO ptr;
			if (IsDragging && UIManager.Exists() && UIManager.instance.GetPointer(this, out ptr))
			{
				dragDropHelper.DragUpdatePosition(ptr);
			}
		}
	}

	public EZAnimation.EASING_TYPE CancelDragEasing
	{
		get
		{
			return cancelDragEasing;
		}
		set
		{
			cancelDragEasing = value;
		}
	}

	public float CancelDragDuration
	{
		get
		{
			return cancelDragDuration;
		}
		set
		{
			cancelDragDuration = value;
		}
	}

	public bool IsDragging
	{
		get
		{
			return dragDropHelper.IsDragging;
		}
		set
		{
			dragDropHelper.IsDragging = value;
		}
	}

	public GameObject DropTarget
	{
		get
		{
			return dragDropHelper.DropTarget;
		}
		set
		{
			dragDropHelper.DropTarget = value;
		}
	}

	public bool DropHandled
	{
		get
		{
			return dragDropHelper.DropHandled;
		}
		set
		{
			dragDropHelper.DropHandled = value;
		}
	}

	public bool UseDefaultCancelDragAnim
	{
		get
		{
			return dragDropHelper.UseDefaultCancelDragAnim;
		}
		set
		{
			dragDropHelper.UseDefaultCancelDragAnim = value;
		}
	}

	public abstract string[] States { get; }

	public abstract EZTransitionList[] Transitions { get; set; }

	protected virtual void Awake()
	{
		if (base.collider != null)
		{
			customCollider = true;
		}
		if (dragDropHelper == null)
		{
			dragDropHelper = new EZDragDropHelper(this);
		}
		else
		{
			dragDropHelper.host = this;
		}
	}

	public virtual void Start()
	{
		if (spriteText != null)
		{
			spriteText.Parent = this;
		}
		if (UIManager.Exists() && Application.isPlaying)
		{
			if (cancelDragEasing == EZAnimation.EASING_TYPE.Default)
			{
				cancelDragEasing = UIManager.instance.cancelDragEasing;
			}
			if (cancelDragDuration == -1f)
			{
				cancelDragDuration = UIManager.instance.cancelDragDuration;
			}
			if (float.IsNaN(dragOffset))
			{
				dragOffset = UIManager.instance.defaultDragOffset;
			}
		}
	}

	protected virtual void AddCollider()
	{
		if (!customCollider)
		{
			base.gameObject.AddComponent(typeof(BoxCollider));
			UpdateCollider();
		}
	}

	public virtual void UpdateCollider()
	{
		if (!customCollider && base.collider is BoxCollider)
		{
			BoxCollider boxCollider = (BoxCollider)base.collider;
			if (includeTextInAutoCollider && spriteText != null)
			{
				Bounds bounds = new Bounds(boxCollider.center, boxCollider.size);
				Matrix4x4 localToWorldMatrix = spriteText.transform.localToWorldMatrix;
				Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
				Vector3 point = worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(spriteText.TopLeft)) * 2f;
				Vector3 point2 = worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(spriteText.BottomRight)) * 2f;
				bounds.Encapsulate(point);
				bounds.Encapsulate(point2);
				boxCollider.size = bounds.extents;
				boxCollider.center = bounds.center * 0.5f;
			}
			boxCollider.isTrigger = true;
		}
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

	public virtual bool GotFocus()
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
		OnInput(ref ptr);
	}

	public virtual void OnInput(ref POINTER_INFO ptr)
	{
		if (Container != null)
		{
			ptr.callerIsControl = true;
			Container.OnInput(ptr);
		}
	}

	public virtual void OnEnable()
	{
	}

	public virtual void OnDisable()
	{
		if (Application.isPlaying)
		{
			if (EZAnimator.Exists())
			{
				EZAnimator.instance.Stop(base.gameObject);
				EZAnimator.instance.Stop(this);
			}
			if (detargetOnDisable && UIManager.Exists())
			{
				UIManager.instance.Detarget(this);
			}
		}
	}

	public virtual void OnDestroy()
	{
		deleted = true;
	}

	public virtual void Copy(IControl ctl)
	{
		Copy(ctl, ControlCopyFlags.All);
	}

	public virtual void Copy(IControl ctl, ControlCopyFlags flags)
	{
		if (!(ctl is ControlBase))
		{
			return;
		}
		ControlBase controlBase = (ControlBase)ctl;
		if ((flags & ControlCopyFlags.Transitions) == ControlCopyFlags.Transitions)
		{
			if (controlBase is UIStateToggleBtn3D)
			{
				if (controlBase.Transitions != null)
				{
					((UIStateToggleBtn3D)this).transitions = new EZTransitionList[controlBase.Transitions.Length];
					for (int i = 0; i < Transitions.Length; i++)
					{
						controlBase.Transitions[i].CopyToNew(Transitions[i], true);
					}
				}
			}
			else if (Transitions != null && controlBase.Transitions != null)
			{
				for (int j = 0; j < Transitions.Length && j < controlBase.Transitions.Length; j++)
				{
					controlBase.Transitions[j].CopyTo(Transitions[j], true);
				}
			}
		}
		if ((flags & ControlCopyFlags.Text) == ControlCopyFlags.Text)
		{
			if (spriteText == null && controlBase.spriteText != null)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(controlBase.spriteText.gameObject);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = controlBase.spriteText.transform.localPosition;
				gameObject.transform.localScale = controlBase.spriteText.transform.localScale;
				gameObject.transform.localRotation = controlBase.spriteText.transform.localRotation;
			}
			Text = controlBase.Text;
		}
		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance && base.collider.GetType() == controlBase.collider.GetType())
		{
			if (base.collider is BoxCollider)
			{
				BoxCollider boxCollider = (BoxCollider)base.collider;
				BoxCollider boxCollider2 = (BoxCollider)controlBase.collider;
				boxCollider.center = boxCollider2.center;
				boxCollider.size = boxCollider2.size;
			}
			else if (base.collider is SphereCollider)
			{
				SphereCollider sphereCollider = (SphereCollider)base.collider;
				SphereCollider sphereCollider2 = (SphereCollider)controlBase.collider;
				sphereCollider.center = sphereCollider2.center;
				sphereCollider.radius = sphereCollider2.radius;
			}
			else if (base.collider is CapsuleCollider)
			{
				CapsuleCollider capsuleCollider = (CapsuleCollider)base.collider;
				CapsuleCollider capsuleCollider2 = (CapsuleCollider)controlBase.collider;
				capsuleCollider.center = capsuleCollider2.center;
				capsuleCollider.radius = capsuleCollider2.radius;
				capsuleCollider.height = capsuleCollider2.height;
				capsuleCollider.direction = capsuleCollider2.direction;
			}
			else if (base.collider is MeshCollider)
			{
				MeshCollider meshCollider = (MeshCollider)base.collider;
				MeshCollider meshCollider2 = (MeshCollider)controlBase.collider;
				meshCollider.smoothSphereCollisions = meshCollider2.smoothSphereCollisions;
				meshCollider.convex = meshCollider2.convex;
				meshCollider.sharedMesh = meshCollider2.sharedMesh;
			}
			base.collider.isTrigger = controlBase.collider.isTrigger;
		}
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			changeDelegate = controlBase.changeDelegate;
			inputDelegate = controlBase.inputDelegate;
		}
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			Container = controlBase.Container;
			if (Application.isPlaying)
			{
				controlIsEnabled = controlBase.controlIsEnabled;
			}
		}
	}

	public void DragUpdatePosition(POINTER_INFO ptr)
	{
		dragDropHelper.DragUpdatePosition(ptr);
	}

	public void DefaultDragUpdatePosition(POINTER_INFO ptr)
	{
		dragDropHelper.DefaultDragUpdatePosition(ptr);
	}

	public void SetDragPosUpdater(EZDragDropHelper.UpdateDragPositionDelegate del)
	{
		dragDropHelper.SetDragPosUpdater(del);
	}

	public void CancelDrag()
	{
		dragDropHelper.CancelDrag();
	}

	public void CancelFinished()
	{
		dragDropHelper.CancelFinished();
	}

	public void DoDefaultCancelDrag()
	{
		dragDropHelper.DoDefaultCancelDrag();
	}

	public void OnEZDragDrop_Internal(EZDragDropParams parms)
	{
		dragDropHelper.OnEZDragDrop_Internal(parms);
	}

	public void AddDragDropDelegate(EZDragDropDelegate del)
	{
		dragDropHelper.AddDragDropDelegate(del);
	}

	public void RemoveDragDropDelegate(EZDragDropDelegate del)
	{
		dragDropHelper.RemoveDragDropDelegate(del);
	}

	public void SetDragDropDelegate(EZDragDropDelegate del)
	{
		dragDropHelper.SetDragDropDelegate(del);
	}

	public void SetDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del)
	{
		dragDropHelper.SetDragDropInternalDelegate(del);
	}

	public void AddDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del)
	{
		dragDropHelper.AddDragDropInternalDelegate(del);
	}

	public void RemoveDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del)
	{
		dragDropHelper.RemoveDragDropInternalDelegate(del);
	}

	public EZDragDropHelper.DragDrop_InternalDelegate GetDragDropInternalDelegate()
	{
		return dragDropHelper.GetDragDropInternalDelegate();
	}

	public virtual int DrawPreStateSelectGUI(int selState, bool inspector)
	{
		return 0;
	}

	public virtual int DrawPostStateSelectGUI(int selState)
	{
		return 0;
	}

	public virtual void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
	}

	public virtual string[] EnumStateElements()
	{
		return States;
	}

	public abstract EZTransitionList GetTransitions(int index);

	public virtual string GetStateLabel(int index)
	{
		return null;
	}

	public virtual void SetStateLabel(int index, string label)
	{
	}

	public virtual ASCSEInfo GetStateElementInfo(int stateNum)
	{
		ASCSEInfo aSCSEInfo = new ASCSEInfo();
		aSCSEInfo.transitions = GetTransitions(stateNum);
		aSCSEInfo.stateLabel = GetStateLabel(stateNum);
		return aSCSEInfo;
	}

	protected void UseStateLabel(int index)
	{
		string stateLabel = GetStateLabel(index);
		if (!(stateLabel == "[\"]") && (!(stateLabel == string.Empty) || !(spriteText == null)))
		{
			Text = stateLabel;
		}
	}

	public virtual void DoMirror()
	{
		if (!Application.isPlaying)
		{
			if (mirror == null)
			{
				mirror = new ControlBaseMirror();
				mirror.Mirror(this);
			}
			mirror.Validate(this);
			if (mirror.DidChange(this))
			{
				mirror.Mirror(this);
			}
		}
	}

	public virtual void OnDrawGizmos()
	{
		DoMirror();
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
