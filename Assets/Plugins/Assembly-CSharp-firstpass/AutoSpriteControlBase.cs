using System;
using UnityEngine;

public abstract class AutoSpriteControlBase : AutoSpriteBase, IEZDragDrop, IControl, IPackableControl, IUIObject, ISpritePackable
{
	public const string DittoString = "[\"]";

	protected bool nullCamera;

	public string text;

	public SpriteText spriteText;

	public float textOffsetZ = -0.1f;

	public bool includeTextInAutoCollider = true;

	protected SpriteText.Anchor_Pos defaultTextAnchor = SpriteText.Anchor_Pos.Middle_Center;

	protected SpriteText.Alignment_Type defaultTextAlignment = SpriteText.Alignment_Type.Center;

	public bool detargetOnDisable;

	protected bool customCollider;

	protected Vector3 savedColliderSize;

	protected Vector2 topLeftEdge;

	protected Vector2 bottomRightEdge;

	[HideInInspector]
	public object data;

	protected SpriteRoot[][] aggregateLayers;

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
				spriteText.Persistent = persistent;
				spriteText.Parent = this;
				spriteText.anchor = defaultTextAnchor;
				spriteText.alignment = defaultTextAlignment;
				spriteText.pixelPerfect = true;
				spriteText.SetCamera(renderCamera);
				if (Application.isPlaying)
				{
					spriteText.Persistent = persistent;
				}
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

	public override bool Clipped
	{
		get
		{
			return base.Clipped;
		}
		set
		{
			if (ignoreClipping)
			{
				return;
			}
			base.Clipped = value;
			if (spriteText != null)
			{
				spriteText.Clipped = value;
			}
			if (aggregateLayers != null)
			{
				for (int i = 0; i < aggregateLayers.Length; i++)
				{
					if (aggregateLayers[i] != null)
					{
						for (int j = 0; j < aggregateLayers[i].Length; j++)
						{
							aggregateLayers[i][j].Clipped = value;
						}
					}
				}
			}
			UpdateCollider();
		}
	}

	public override Rect3D ClippingRect
	{
		get
		{
			return base.ClippingRect;
		}
		set
		{
			if (ignoreClipping)
			{
				return;
			}
			base.ClippingRect = value;
			if (spriteText != null)
			{
				spriteText.ClippingRect = value;
			}
			if (aggregateLayers != null)
			{
				for (int i = 0; i < aggregateLayers.Length; i++)
				{
					if (aggregateLayers[i] != null)
					{
						for (int j = 0; j < aggregateLayers[i].Length; j++)
						{
							aggregateLayers[i][j].ClippingRect = value;
						}
					}
				}
			}
			UpdateCollider();
		}
	}

	public override Camera RenderCamera
	{
		get
		{
			return base.RenderCamera;
		}
		set
		{
			base.RenderCamera = value;
			if (spriteText != null)
			{
				spriteText.RenderCamera = value;
			}
		}
	}

	public Vector2 TopLeftEdge
	{
		get
		{
			return topLeftEdge;
		}
	}

	public Vector2 BottomRightEdge
	{
		get
		{
			return bottomRightEdge;
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
			if (container != null)
			{
				if (aggregateLayers != null)
				{
					for (int i = 0; i < aggregateLayers.Length; i++)
					{
						if (aggregateLayers[i] != null)
						{
							for (int j = 0; j < aggregateLayers[i].Length; j++)
							{
								container.RemoveChild(aggregateLayers[i][j].gameObject);
							}
						}
					}
				}
				if (spriteText != null)
				{
					container.RemoveChild(spriteText.gameObject);
				}
			}
			if (value != null)
			{
				if (aggregateLayers != null)
				{
					for (int k = 0; k < aggregateLayers.Length; k++)
					{
						if (aggregateLayers[k] != null)
						{
							for (int l = 0; l < aggregateLayers[k].Length; l++)
							{
								value.AddChild(aggregateLayers[k][l].gameObject);
							}
						}
					}
				}
				if (spriteText != null)
				{
					value.AddChild(spriteText.gameObject);
				}
			}
			container = value;
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

	public abstract EZTransitionList[] Transitions { get; set; }

	protected override void Init()
	{
		nullCamera = renderCamera == null;
		base.Init();
	}

	public override void Start()
	{
		base.Start();
		if (UIManager.Exists())
		{
			if (nullCamera && UIManager.instance.uiCameras != null && UIManager.instance.uiCameras.Length > 0)
			{
				SetCamera(UIManager.instance.uiCameras[0].camera);
			}
			if (Application.isPlaying)
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
		if (spriteText != null)
		{
			spriteText.Persistent = persistent;
			spriteText.Parent = this;
		}
	}

	public override void TruncateTop(float pct)
	{
		base.TruncateTop(pct);
		if (aggregateLayers == null)
		{
			return;
		}
		for (int i = 0; i < aggregateLayers.Length; i++)
		{
			if (aggregateLayers[i] != null)
			{
				for (int j = 0; j < aggregateLayers[i].Length; j++)
				{
					aggregateLayers[i][j].TruncateTop(pct);
				}
			}
		}
	}

	public override void TruncateBottom(float pct)
	{
		base.TruncateBottom(pct);
		if (aggregateLayers == null)
		{
			return;
		}
		for (int i = 0; i < aggregateLayers.Length; i++)
		{
			if (aggregateLayers[i] != null)
			{
				for (int j = 0; j < aggregateLayers[i].Length; j++)
				{
					aggregateLayers[i][j].TruncateBottom(pct);
				}
			}
		}
	}

	public override void TruncateLeft(float pct)
	{
		base.TruncateLeft(pct);
		if (aggregateLayers == null)
		{
			return;
		}
		for (int i = 0; i < aggregateLayers.Length; i++)
		{
			if (aggregateLayers[i] != null)
			{
				for (int j = 0; j < aggregateLayers[i].Length; j++)
				{
					aggregateLayers[i][j].TruncateLeft(pct);
				}
			}
		}
	}

	public override void TruncateRight(float pct)
	{
		base.TruncateRight(pct);
		if (aggregateLayers == null)
		{
			return;
		}
		for (int i = 0; i < aggregateLayers.Length; i++)
		{
			if (aggregateLayers[i] != null)
			{
				for (int j = 0; j < aggregateLayers[i].Length; j++)
				{
					aggregateLayers[i][j].TruncateRight(pct);
				}
			}
		}
	}

	public override void Untruncate()
	{
		base.Untruncate();
		if (aggregateLayers == null)
		{
			return;
		}
		for (int i = 0; i < aggregateLayers.Length; i++)
		{
			if (aggregateLayers[i] != null)
			{
				for (int j = 0; j < aggregateLayers[i].Length; j++)
				{
					aggregateLayers[i][j].Untruncate();
				}
			}
		}
	}

	public override void Unclip()
	{
		if (ignoreClipping)
		{
			return;
		}
		base.Unclip();
		if (spriteText != null)
		{
			spriteText.Unclip();
		}
		if (aggregateLayers != null)
		{
			for (int i = 0; i < aggregateLayers.Length; i++)
			{
				if (aggregateLayers[i] != null)
				{
					for (int j = 0; j < aggregateLayers[i].Length; j++)
					{
						aggregateLayers[i][j].Unclip();
					}
				}
			}
		}
		UpdateCollider();
	}

	public override void SetCamera(Camera c)
	{
		base.SetCamera(c);
		if (spriteText != null)
		{
			spriteText.SetCamera(c);
		}
		if (pixelPerfect)
		{
			UpdateCollider();
		}
	}

	public override void Hide(bool tf)
	{
		if (!m_started)
		{
			Start();
		}
		if (!IsHidden() && tf)
		{
			if (base.collider is BoxCollider && Application.isPlaying)
			{
				savedColliderSize = ((BoxCollider)base.collider).size;
				((BoxCollider)base.collider).size = Vector3.zero;
			}
		}
		else if (IsHidden() && !tf && base.collider is BoxCollider)
		{
			((BoxCollider)base.collider).size = savedColliderSize;
		}
		base.Hide(tf);
		if (aggregateLayers != null)
		{
			for (int i = 0; i < aggregateLayers.Length; i++)
			{
				if (aggregateLayers[i] != null)
				{
					for (int j = 0; j < aggregateLayers[i].Length; j++)
					{
						aggregateLayers[i][j].Hide(tf);
					}
				}
			}
		}
		if (spriteText != null)
		{
			spriteText.Hide(tf);
		}
		if (!tf)
		{
			UpdateCollider();
		}
	}

	public void Copy(IControl c)
	{
		if (c is AutoSpriteControlBase)
		{
			Copy((SpriteRoot)c);
		}
	}

	public void Copy(IControl c, ControlCopyFlags flags)
	{
		if (c is AutoSpriteControlBase)
		{
			Copy((SpriteRoot)c, flags);
		}
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public virtual void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
		{
			if (Application.isPlaying && s.Started)
			{
				base.Copy(s);
			}
			else
			{
				base.CopyAll(s);
			}
			if (!(s is AutoSpriteControlBase))
			{
				if (autoResize || pixelPerfect)
				{
					CalcSize();
				}
				else
				{
					SetSize(s.width, s.height);
				}
				SetBleedCompensation();
				return;
			}
		}
		AutoSpriteControlBase autoSpriteControlBase = (AutoSpriteControlBase)s;
		if ((flags & ControlCopyFlags.Transitions) == ControlCopyFlags.Transitions)
		{
			if (autoSpriteControlBase is UIStateToggleBtn || !Application.isPlaying)
			{
				if (autoSpriteControlBase.Transitions != null)
				{
					Transitions = new EZTransitionList[autoSpriteControlBase.Transitions.Length];
					for (int i = 0; i < Transitions.Length; i++)
					{
						Transitions[i] = new EZTransitionList();
						autoSpriteControlBase.Transitions[i].CopyToNew(Transitions[i], true);
					}
				}
			}
			else if (Transitions != null && autoSpriteControlBase.Transitions != null)
			{
				for (int j = 0; j < Transitions.Length && j < autoSpriteControlBase.Transitions.Length; j++)
				{
					autoSpriteControlBase.Transitions[j].CopyTo(Transitions[j], true);
				}
			}
		}
		if ((flags & ControlCopyFlags.Text) == ControlCopyFlags.Text)
		{
			if (spriteText == null && autoSpriteControlBase.spriteText != null)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(autoSpriteControlBase.spriteText.gameObject);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = autoSpriteControlBase.spriteText.transform.localPosition;
				gameObject.transform.localScale = autoSpriteControlBase.spriteText.transform.localScale;
				gameObject.transform.localRotation = autoSpriteControlBase.spriteText.transform.localRotation;
			}
			if (spriteText != null)
			{
				spriteText.Copy(autoSpriteControlBase.spriteText);
			}
			text = autoSpriteControlBase.text;
			textOffsetZ = autoSpriteControlBase.textOffsetZ;
			includeTextInAutoCollider = autoSpriteControlBase.includeTextInAutoCollider;
		}
		if ((flags & ControlCopyFlags.Data) == ControlCopyFlags.Data)
		{
			data = autoSpriteControlBase.data;
		}
		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
		{
			if (autoSpriteControlBase.collider != null)
			{
				if (base.collider.GetType() == autoSpriteControlBase.collider.GetType())
				{
					if (autoSpriteControlBase.collider is BoxCollider)
					{
						if (base.collider == null)
						{
							base.gameObject.AddComponent(typeof(BoxCollider));
						}
						BoxCollider boxCollider = (BoxCollider)base.collider;
						BoxCollider boxCollider2 = (BoxCollider)autoSpriteControlBase.collider;
						boxCollider.center = boxCollider2.center;
						boxCollider.size = boxCollider2.size;
					}
					else if (autoSpriteControlBase.collider is SphereCollider)
					{
						if (base.collider == null)
						{
							base.gameObject.AddComponent(typeof(SphereCollider));
						}
						SphereCollider sphereCollider = (SphereCollider)base.collider;
						SphereCollider sphereCollider2 = (SphereCollider)autoSpriteControlBase.collider;
						sphereCollider.center = sphereCollider2.center;
						sphereCollider.radius = sphereCollider2.radius;
					}
					else if (autoSpriteControlBase.collider is MeshCollider)
					{
						if (base.collider == null)
						{
							base.gameObject.AddComponent(typeof(MeshCollider));
						}
						MeshCollider meshCollider = (MeshCollider)base.collider;
						MeshCollider meshCollider2 = (MeshCollider)autoSpriteControlBase.collider;
						meshCollider.smoothSphereCollisions = meshCollider2.smoothSphereCollisions;
						meshCollider.convex = meshCollider2.convex;
						meshCollider.sharedMesh = meshCollider2.sharedMesh;
					}
					else if (autoSpriteControlBase.collider is CapsuleCollider)
					{
						if (base.collider == null)
						{
							base.gameObject.AddComponent(typeof(CapsuleCollider));
						}
						CapsuleCollider capsuleCollider = (CapsuleCollider)base.collider;
						CapsuleCollider capsuleCollider2 = (CapsuleCollider)autoSpriteControlBase.collider;
						capsuleCollider.center = capsuleCollider2.center;
						capsuleCollider.radius = capsuleCollider2.radius;
						capsuleCollider.height = capsuleCollider2.height;
						capsuleCollider.direction = capsuleCollider2.direction;
					}
					if (base.collider != null)
					{
						base.collider.isTrigger = autoSpriteControlBase.collider.isTrigger;
					}
				}
			}
			else if (Application.isPlaying)
			{
				if (base.collider == null && width != 0f && height != 0f && !float.IsNaN(width) && !float.IsNaN(height))
				{
					BoxCollider boxCollider3 = (BoxCollider)base.gameObject.AddComponent(typeof(BoxCollider));
					boxCollider3.size = new Vector3(autoSpriteControlBase.width, autoSpriteControlBase.height, 0.001f);
					boxCollider3.center = autoSpriteControlBase.GetCenterPoint();
					boxCollider3.isTrigger = true;
				}
				else if (base.collider is BoxCollider)
				{
					BoxCollider boxCollider4 = (BoxCollider)base.collider;
					boxCollider4.size = new Vector3(autoSpriteControlBase.width, autoSpriteControlBase.height, 0.001f);
					boxCollider4.center = autoSpriteControlBase.GetCenterPoint();
				}
				else if (base.collider is SphereCollider)
				{
					SphereCollider sphereCollider3 = (SphereCollider)base.collider;
					sphereCollider3.radius = Mathf.Max(autoSpriteControlBase.width, autoSpriteControlBase.height);
					sphereCollider3.center = autoSpriteControlBase.GetCenterPoint();
				}
			}
		}
		if ((flags & ControlCopyFlags.DragDrop) == ControlCopyFlags.DragDrop)
		{
			isDraggable = autoSpriteControlBase.isDraggable;
			dragOffset = autoSpriteControlBase.dragOffset;
			cancelDragEasing = autoSpriteControlBase.cancelDragEasing;
			cancelDragDuration = autoSpriteControlBase.cancelDragDuration;
		}
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			detargetOnDisable = autoSpriteControlBase.detargetOnDisable;
		}
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			changeDelegate = autoSpriteControlBase.changeDelegate;
			inputDelegate = autoSpriteControlBase.inputDelegate;
		}
		if ((flags & ControlCopyFlags.State) != ControlCopyFlags.State && (flags & ControlCopyFlags.Appearance) != ControlCopyFlags.Appearance)
		{
			return;
		}
		Container = autoSpriteControlBase.Container;
		if (Application.isPlaying)
		{
			controlIsEnabled = autoSpriteControlBase.controlIsEnabled;
			Hide(autoSpriteControlBase.IsHidden());
		}
		if (curAnim != null)
		{
			if (curAnim.index == -1)
			{
				if (autoSpriteControlBase.curAnim != null)
				{
					curAnim = autoSpriteControlBase.curAnim.Clone();
				}
				PlayAnim(curAnim);
			}
			else
			{
				SetState(curAnim.index);
			}
		}
		else
		{
			SetState(0);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (dragDropHelper == null)
		{
			dragDropHelper = new EZDragDropHelper(this);
		}
		else
		{
			dragDropHelper.host = this;
		}
		if (base.collider != null)
		{
			customCollider = true;
		}
		Init();
		AddSpriteResizedDelegate(OnResize);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (managed && m_spriteMesh != null && m_hidden)
		{
			m_spriteMesh.Hide(true);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
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

	protected void OnResize(float newWidth, float newHeight, SpriteRoot sprite)
	{
		UpdateCollider();
	}

	protected virtual void AddCollider()
	{
		if (!customCollider && Application.isPlaying && m_started)
		{
			BoxCollider boxCollider = (BoxCollider)base.gameObject.AddComponent(typeof(BoxCollider));
			boxCollider.isTrigger = true;
			if (IsHidden())
			{
				boxCollider.size = Vector3.zero;
			}
			else
			{
				UpdateCollider();
			}
		}
	}

	public virtual void UpdateCollider()
	{
		if (deleted || m_spriteMesh == null || !(base.collider is BoxCollider) || IsHidden() || m_spriteMesh == null || customCollider)
		{
			return;
		}
		Vector3[] vertices = m_spriteMesh.vertices;
		Vector3 vector = vertices[1];
		Vector3 vector2 = vertices[3];
		if (includeTextInAutoCollider && spriteText != null)
		{
			Matrix4x4 localToWorldMatrix = spriteText.transform.localToWorldMatrix;
			Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
			Vector3 vector3 = worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(spriteText.TopLeft));
			Vector3 vector4 = worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(spriteText.BottomRight));
			if (vector4.x - vector3.x > 0f && vector3.y - vector4.y > 0f)
			{
				vector.x = Mathf.Min(vector.x, vector3.x);
				vector.y = Mathf.Min(vector.y, vector4.y);
				vector2.x = Mathf.Max(vector2.x, vector4.x);
				vector2.y = Mathf.Max(vector2.y, vector3.y);
			}
		}
		BoxCollider boxCollider = (BoxCollider)base.collider;
		boxCollider.size = vector2 - vector;
		boxCollider.center = vector + boxCollider.size * 0.5f;
		boxCollider.isTrigger = true;
	}

	public virtual void FindOuterEdges()
	{
		if (deleted)
		{
			return;
		}
		if (!m_started)
		{
			Start();
		}
		topLeftEdge = unclippedTopLeft;
		bottomRightEdge = unclippedBottomRight;
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		if (spriteText != null)
		{
			Matrix4x4 localToWorldMatrix = spriteText.transform.localToWorldMatrix;
			Vector3 vector = worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(spriteText.UnclippedTopLeft));
			Vector3 vector2 = worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(spriteText.UnclippedBottomRight));
			topLeftEdge.x = Mathf.Min(topLeftEdge.x, vector.x);
			topLeftEdge.y = Mathf.Max(topLeftEdge.y, vector.y);
			bottomRightEdge.x = Mathf.Max(bottomRightEdge.x, vector2.x);
			bottomRightEdge.y = Mathf.Min(bottomRightEdge.y, vector2.y);
		}
		if (aggregateLayers == null)
		{
			return;
		}
		for (int i = 0; i < aggregateLayers.Length; i++)
		{
			for (int j = 0; j < aggregateLayers[i].Length; j++)
			{
				if (!aggregateLayers[i][j].IsHidden() && aggregateLayers[i][j].gameObject.active)
				{
					Matrix4x4 localToWorldMatrix = aggregateLayers[i][j].transform.localToWorldMatrix;
					Vector3 vector = worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(aggregateLayers[i][j].UnclippedTopLeft));
					Vector3 vector2 = worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(aggregateLayers[i][j].UnclippedBottomRight));
					topLeftEdge.x = Mathf.Min(topLeftEdge.x, vector.x);
					topLeftEdge.y = Mathf.Max(topLeftEdge.y, vector.y);
					bottomRightEdge.x = Mathf.Max(bottomRightEdge.x, vector2.x);
					bottomRightEdge.y = Mathf.Min(bottomRightEdge.y, vector2.y);
				}
			}
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
		string[] array = new string[States.Length];
		for (int i = 0; i < States.Length; i++)
		{
			array[i] = States[i].name;
		}
		return array;
	}

	public virtual EZTransitionList GetTransitions(int index)
	{
		return null;
	}

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
		aSCSEInfo.stateObj = States[stateNum];
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

	public override void DoMirror()
	{
		if (!Application.isPlaying)
		{
			if (screenSize.x == 0f || screenSize.y == 0f)
			{
				Start();
			}
			if (mirror == null)
			{
				mirror = new AutoSpriteControlBaseMirror();
				mirror.Mirror(this);
			}
			mirror.Validate(this);
			if (mirror.DidChange(this))
			{
				Init();
				mirror.Mirror(this);
			}
		}
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
