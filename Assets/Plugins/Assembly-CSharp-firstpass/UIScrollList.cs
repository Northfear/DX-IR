using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Scroll List")]
public class UIScrollList : MonoBehaviour, IEZDragDrop, IUIObject
{
	public enum ORIENTATION
	{
		HORIZONTAL = 0,
		VERTICAL = 1
	}

	public enum DIRECTION
	{
		TtoB_LtoR = 0,
		BtoT_RtoL = 1
	}

	public enum ALIGNMENT
	{
		LEFT_TOP = 0,
		CENTER = 1,
		RIGHT_BOTTOM = 2
	}

	protected delegate float ItemAlignmentDel(IUIListObject item);

	protected delegate bool SnapCoordProc(float val);

	public delegate void ItemSnappedDelegate(IUIListObject item);

	protected const float reboundSpeed = 1f;

	protected const float overscrollAllowance = 0.5f;

	protected const float lowPassKernelWidthInSeconds = 0.045f;

	protected const float backgroundColliderOffset = 0.01f;

	private const float scrollStopThreshold = 0.0001f;

	public bool touchScroll = true;

	public float scrollWheelFactor = 100f;

	public float scrollDecelCoef = 0.04f;

	public bool snap;

	public float minSnapDuration = 1f;

	public EZAnimation.EASING_TYPE snapEasing = EZAnimation.EASING_TYPE.ExponentialOut;

	public UISlider slider;

	public ORIENTATION orientation;

	public DIRECTION direction;

	public ALIGNMENT alignment = ALIGNMENT.CENTER;

	public Vector2 viewableArea;

	protected Vector2 viewableAreaActual;

	public bool unitsInPixels;

	public Camera renderCamera;

	protected Rect3D clientClippingRect;

	public float itemSpacing;

	protected float itemSpacingActual;

	public bool spacingAtEnds = true;

	public float extraEndSpacing;

	protected float extraEndSpacingActual;

	public bool activateWhenAdding = true;

	public bool clipContents = true;

	public bool clipWhenMoving;

	public bool positionItemsImmediately = true;

	public float dragThreshold = float.NaN;

	public GameObject[] sceneItems = new GameObject[0];

	public PrefabListItem[] prefabItems = new PrefabListItem[0];

	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvokeOnSelect;

	public SpriteManager manager;

	public bool detargetOnDisable;

	public EZAnimation.EASING_TYPE positionEasing = EZAnimation.EASING_TYPE.ExponentialOut;

	public float positionEaseDuration = 0.5f;

	public float positionEaseDelay;

	public bool blockInputWhileEasing = true;

	protected bool doItemEasing;

	protected bool doPosEasing;

	protected List<EZAnimation> itemEasers = new List<EZAnimation>();

	protected EZAnimation scrollPosAnim;

	[NonSerialized]
	[HideInInspector]
	public bool repositionOnEnable = true;

	protected float contentExtents;

	protected IUIListObject selectedItem;

	protected IUIObject lastClickedControl;

	protected float scrollPos;

	protected GameObject mover;

	protected List<IUIListObject> items = new List<IUIListObject>();

	protected List<IUIListObject> visibleItems = new List<IUIListObject>();

	protected List<IUIListObject> tempVisItems = new List<IUIListObject>();

	protected bool m_controlIsEnabled = true;

	protected IUIContainer container;

	protected EZInputDelegate inputDelegate;

	protected EZValueChangedDelegate changeDelegate;

	protected ItemSnappedDelegate itemSnappedDel;

	protected Vector3 cachedPos;

	protected Quaternion cachedRot;

	protected Vector3 cachedScale;

	protected bool m_started;

	protected bool m_awake;

	protected List<IUIListObject> newItems = new List<IUIListObject>();

	protected bool itemsInserted;

	protected bool isScrolling;

	protected bool noTouch = true;

	protected float lowPassFilterFactor;

	private float scrollInertia;

	protected float scrollMax;

	private float scrollDelta;

	private float scrollStopThresholdLog = Mathf.Log10(0.0001f);

	private float lastTime;

	private float timeDelta;

	private float inertiaLerpInterval = 0.06f;

	private float inertiaLerpTime;

	private float amtOfPlay;

	private float autoScrollDuration;

	private float autoScrollStart;

	private float autoScrollPos;

	private float autoScrollDelta;

	private float autoScrollTime;

	private bool autoScrolling;

	private bool listMoved;

	private EZAnimation.Interpolator autoScrollInterpolator;

	private IUIListObject snappedItem;

	private float localUnitsPerPixel;

	protected EZDragDropDelegate dragDropDelegate;

	public float ScrollPosition
	{
		get
		{
			return scrollPos;
		}
		set
		{
			ScrollListTo(value);
		}
	}

	public IUIListObject SnappedItem
	{
		get
		{
			return snappedItem;
		}
	}

	public float ContentExtents
	{
		get
		{
			return contentExtents;
		}
	}

	public float UnviewableArea
	{
		get
		{
			return amtOfPlay;
		}
	}

	public IUIListObject SelectedItem
	{
		get
		{
			return selectedItem;
		}
		set
		{
			IUIListObject iUIListObject = selectedItem;
			if (selectedItem != null)
			{
				selectedItem.selected = false;
			}
			if (value == null)
			{
				selectedItem = null;
				return;
			}
			selectedItem = value;
			selectedItem.selected = true;
			if (iUIListObject != selectedItem && changeDelegate != null)
			{
				changeDelegate(this);
			}
		}
	}

	public IUIObject LastClickedControl
	{
		get
		{
			return lastClickedControl;
		}
	}

	public int Count
	{
		get
		{
			return items.Count;
		}
	}

	public bool controlIsEnabled
	{
		get
		{
			return m_controlIsEnabled;
		}
		set
		{
			m_controlIsEnabled = value;
			for (int i = 0; i < items.Count; i++)
			{
				items[i].controlIsEnabled = value;
			}
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
			if (value != container)
			{
				if (container != null)
				{
					RemoveItemsFromContainer();
				}
				container = value;
				AddItemsToContainer();
			}
			else
			{
				container = value;
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

	protected void Awake()
	{
		if (!m_awake)
		{
			m_awake = true;
			mover = new GameObject();
			mover.name = "Mover";
			mover.transform.parent = base.transform;
			mover.transform.localPosition = Vector3.zero;
			mover.transform.localRotation = Quaternion.identity;
			mover.transform.localScale = Vector3.one;
			if (direction == DIRECTION.BtoT_RtoL)
			{
				scrollPos = 1f;
			}
			autoScrollInterpolator = EZAnimation.GetInterpolator(snapEasing);
			lowPassFilterFactor = inertiaLerpInterval / 0.045f;
		}
	}

	protected void Start()
	{
		if (m_started)
		{
			return;
		}
		m_started = true;
		SetupCameraAndSizes();
		lastTime = Time.realtimeSinceStartup;
		cachedPos = base.transform.position;
		cachedRot = base.transform.rotation;
		cachedScale = base.transform.lossyScale;
		CalcClippingRect();
		if (slider != null)
		{
			slider.AddValueChangedDelegate(SliderMoved);
			slider.AddInputDelegate(SliderInputDel);
		}
		if (base.collider == null && touchScroll)
		{
			BoxCollider boxCollider = (BoxCollider)base.gameObject.AddComponent(typeof(BoxCollider));
			boxCollider.size = new Vector3(viewableAreaActual.x, viewableAreaActual.y, 0.001f);
			boxCollider.center = Vector3.forward * 0.01f;
			boxCollider.isTrigger = true;
		}
		for (int i = 0; i < sceneItems.Length; i++)
		{
			if (sceneItems[i] != null)
			{
				AddItem(sceneItems[i]);
			}
		}
		for (int j = 0; j < prefabItems.Length; j++)
		{
			if (prefabItems[j] == null)
			{
				continue;
			}
			if (prefabItems[j].item == null)
			{
				if (prefabItems[0].item != null)
				{
					CreateItem(prefabItems[0].item, (!(prefabItems[j].itemText == string.Empty)) ? prefabItems[j].itemText : null);
				}
			}
			else
			{
				CreateItem(prefabItems[j].item, (!(prefabItems[j].itemText == string.Empty)) ? prefabItems[j].itemText : null);
			}
		}
		if (float.IsNaN(dragThreshold))
		{
			dragThreshold = UIManager.instance.dragThreshold;
		}
	}

	public void UpdateCamera()
	{
		SetupCameraAndSizes();
		CalcClippingRect();
		RepositionItems();
	}

	public void SetupCameraAndSizes()
	{
		if (renderCamera == null)
		{
			if (UIManager.Exists() && UIManager.instance.uiCameras[0].camera != null)
			{
				renderCamera = UIManager.instance.uiCameras[0].camera;
			}
			else
			{
				renderCamera = Camera.main;
			}
		}
		if (unitsInPixels)
		{
			CalcScreenToWorldUnits();
			viewableAreaActual = new Vector2(viewableArea.x * localUnitsPerPixel, viewableArea.y * localUnitsPerPixel);
			itemSpacingActual = itemSpacing * localUnitsPerPixel;
			extraEndSpacingActual = extraEndSpacing * localUnitsPerPixel;
		}
		else
		{
			viewableAreaActual = viewableArea;
			itemSpacingActual = itemSpacing;
			extraEndSpacingActual = extraEndSpacing;
		}
		for (int i = 0; i < items.Count; i++)
		{
			items[i].UpdateCamera();
		}
	}

	protected void CalcScreenToWorldUnits()
	{
		float distanceToPoint = new Plane(renderCamera.transform.forward, renderCamera.transform.position).GetDistanceToPoint(base.transform.position);
		localUnitsPerPixel = Vector3.Distance(renderCamera.ScreenToWorldPoint(new Vector3(0f, 1f, distanceToPoint)), renderCamera.ScreenToWorldPoint(new Vector3(0f, 0f, distanceToPoint)));
	}

	protected void CalcClippingRect()
	{
		clientClippingRect.FromPoints(new Vector3((0f - viewableAreaActual.x) * 0.5f, viewableAreaActual.y * 0.5f, 0f), new Vector3(viewableAreaActual.x * 0.5f, viewableAreaActual.y * 0.5f, 0f), new Vector3((0f - viewableAreaActual.x) * 0.5f, (0f - viewableAreaActual.y) * 0.5f, 0f));
		clientClippingRect.MultFast(base.transform.localToWorldMatrix);
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].TextObj != null)
			{
				items[i].TextObj.ClippingRect = clientClippingRect;
			}
		}
	}

	public void SliderMoved(IUIObject slider)
	{
		ScrollListTo_Internal(((UISlider)slider).Value);
	}

	public void SliderInputDel(ref POINTER_INFO ptr)
	{
		if (snap && (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP || ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE || ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE_OFF))
		{
			CalcSnapItem();
		}
	}

	protected void ScrollListTo_Internal(float pos)
	{
		if (!float.IsNaN(pos) && !(mover == null))
		{
			if (orientation == ORIENTATION.VERTICAL)
			{
				float num = ((direction != 0) ? (-1f) : 1f);
				mover.transform.localPosition = Vector3.up * num * Mathf.Clamp(amtOfPlay, 0f, amtOfPlay) * pos;
			}
			else
			{
				float num2 = ((direction != 0) ? 1f : (-1f));
				mover.transform.localPosition = Vector3.right * num2 * Mathf.Clamp(amtOfPlay, 0f, amtOfPlay) * pos;
			}
			scrollPos = pos;
			ClipItems();
			if (slider != null)
			{
				slider.Value = scrollPos;
			}
		}
	}

	public void ScrollListTo(float pos)
	{
		scrollInertia = 0f;
		scrollDelta = 0f;
		isScrolling = false;
		autoScrolling = false;
		ScrollListTo_Internal(pos);
	}

	public void ScrollToItem(IUIListObject item, float scrollTime, EZAnimation.EASING_TYPE easing)
	{
		snappedItem = item;
		if (newItems.Count != 0)
		{
			if (itemsInserted || doItemEasing)
			{
				RepositionItems();
			}
			else
			{
				PositionNewItems();
			}
			itemsInserted = false;
			newItems.Clear();
		}
		if (orientation == ORIENTATION.HORIZONTAL)
		{
			if (direction == DIRECTION.TtoB_LtoR)
			{
				autoScrollPos = Mathf.Clamp01(item.transform.localPosition.x / amtOfPlay);
			}
			else
			{
				autoScrollPos = Mathf.Clamp01((0f - item.transform.localPosition.x) / amtOfPlay);
			}
		}
		else if (direction == DIRECTION.TtoB_LtoR)
		{
			autoScrollPos = Mathf.Clamp01((0f - item.transform.localPosition.y) / amtOfPlay);
		}
		else
		{
			autoScrollPos = Mathf.Clamp01(item.transform.localPosition.y / amtOfPlay);
		}
		autoScrollInterpolator = EZAnimation.GetInterpolator(easing);
		autoScrollStart = scrollPos;
		autoScrollDelta = autoScrollPos - scrollPos;
		autoScrollDuration = scrollTime;
		autoScrollTime = 0f;
		autoScrolling = true;
		scrollDelta = 0f;
		isScrolling = false;
		if (itemSnappedDel != null)
		{
			itemSnappedDel(snappedItem);
		}
	}

	public void ScrollToItem(int index, float scrollTime, EZAnimation.EASING_TYPE easing)
	{
		if (index >= 0 && index < items.Count)
		{
			ScrollToItem(items[index], scrollTime, easing);
		}
	}

	public void ScrollToItem(IUIListObject item, float scrollTime)
	{
		ScrollToItem(item, scrollTime, snapEasing);
	}

	public void ScrollToItem(int index, float scrollTime)
	{
		ScrollToItem(index, scrollTime, snapEasing);
	}

	public void SetViewableAreaPixelDimensions(Camera cam, int width, int height)
	{
		float distanceToPoint = new Plane(cam.transform.forward, cam.transform.position).GetDistanceToPoint(base.transform.position);
		float num = Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(0f, 1f, distanceToPoint)), cam.ScreenToWorldPoint(new Vector3(0f, 0f, distanceToPoint)));
		viewableAreaActual = new Vector2((float)width * num, (float)height * num);
		CalcClippingRect();
		RepositionItems();
	}

	public void InsertItem(IUIListObject item, int position)
	{
		InsertItem(item, position, null, false);
	}

	public void InsertItem(IUIListObject item, int position, bool doEasing)
	{
		InsertItem(item, position, null, doEasing);
	}

	public void InsertItem(IUIListObject item, int position, string text)
	{
		InsertItem(item, position, text, false);
	}

	public void InsertItem(IUIListObject item, int position, string text, bool doEasing)
	{
		if (position >= items.Count)
		{
			doItemEasing = false;
		}
		else
		{
			doItemEasing = doEasing;
		}
		doPosEasing = doEasing;
		if (!m_awake)
		{
			Awake();
		}
		if (!m_started)
		{
			Start();
		}
		if (activateWhenAdding && !((Component)item).gameObject.active)
		{
			((Component)item).gameObject.SetActiveRecursively(true);
		}
		if (!base.gameObject.active)
		{
			((Component)item).gameObject.SetActiveRecursively(false);
		}
		item.gameObject.layer = base.gameObject.layer;
		if (container != null)
		{
			container.AddChild(item.gameObject);
		}
		item.transform.parent = mover.transform;
		item.transform.localRotation = Quaternion.identity;
		item.transform.localScale = Vector3.one;
		item.transform.localPosition = Vector3.zero;
		item.SetList(this);
		if (text != null)
		{
			item.Text = text;
		}
		position = Mathf.Clamp(position, 0, items.Count);
		if (clipContents)
		{
			item.Hide(true);
			if (!item.Managed)
			{
				item.gameObject.SetActiveRecursively(false);
			}
		}
		item.Index = position;
		newItems.Add(item);
		if (position != items.Count)
		{
			itemsInserted = true;
			items.Insert(position, item);
			if (visibleItems.Count == 0)
			{
				visibleItems.Add(item);
			}
			else if (item.Index > 0)
			{
				int num = visibleItems.IndexOf(items[item.Index - 1]);
				if (num == -1)
				{
					if (visibleItems[0].Index >= item.Index)
					{
						visibleItems.Insert(0, item);
					}
					else
					{
						visibleItems.Add(item);
					}
				}
				else
				{
					visibleItems.Insert(num + 1, item);
				}
			}
		}
		else
		{
			items.Add(item);
			visibleItems.Add(item);
		}
		if (positionItemsImmediately)
		{
			if (itemsInserted || doItemEasing)
			{
				RepositionItems();
				itemsInserted = false;
				newItems.Clear();
			}
			else
			{
				PositionNewItems();
			}
		}
	}

	protected void PositionNewItems()
	{
		IUIListObject iUIListObject = null;
		float num = 0f;
		for (int i = 0; i < newItems.Count; i++)
		{
			if (newItems[i] == null)
			{
				continue;
			}
			int index = newItems[i].Index;
			IUIListObject iUIListObject2 = items[index];
			iUIListObject2.FindOuterEdges();
			iUIListObject2.UpdateCollider();
			float x = 0f;
			float y = 0f;
			bool flag = false;
			if (orientation == ORIENTATION.HORIZONTAL)
			{
				if (index > 0)
				{
					flag = true;
					iUIListObject = items[index - 1];
					x = ((direction != 0) ? (iUIListObject.transform.localPosition.x - iUIListObject.BottomRightEdge.x - itemSpacingActual + iUIListObject2.TopLeftEdge.x) : (iUIListObject.transform.localPosition.x + iUIListObject.BottomRightEdge.x + itemSpacingActual - iUIListObject2.TopLeftEdge.x));
				}
				else
				{
					if (spacingAtEnds)
					{
						flag = true;
					}
					x = ((direction != 0) ? (viewableAreaActual.x * 0.5f - iUIListObject2.BottomRightEdge.x - ((!spacingAtEnds) ? 0f : itemSpacingActual) - extraEndSpacingActual) : (viewableAreaActual.x * -0.5f - iUIListObject2.TopLeftEdge.x + ((!spacingAtEnds) ? 0f : itemSpacingActual) + extraEndSpacingActual));
				}
				switch (alignment)
				{
				case ALIGNMENT.CENTER:
					y = 0f;
					break;
				case ALIGNMENT.LEFT_TOP:
					y = viewableAreaActual.y * 0.5f - iUIListObject2.TopLeftEdge.y;
					break;
				case ALIGNMENT.RIGHT_BOTTOM:
					y = viewableAreaActual.y * -0.5f - iUIListObject2.BottomRightEdge.y;
					break;
				}
				num += iUIListObject2.BottomRightEdge.x - iUIListObject2.TopLeftEdge.x + ((!flag || iUIListObject == null) ? 0f : itemSpacingActual);
			}
			else
			{
				if (index > 0)
				{
					flag = true;
					iUIListObject = items[index - 1];
					y = ((direction != 0) ? (iUIListObject.transform.localPosition.y - iUIListObject.BottomRightEdge.y + itemSpacingActual + iUIListObject2.TopLeftEdge.y) : (iUIListObject.transform.localPosition.y + iUIListObject.BottomRightEdge.y - itemSpacingActual - iUIListObject2.TopLeftEdge.y));
				}
				else
				{
					if (spacingAtEnds)
					{
						flag = true;
					}
					y = ((direction != 0) ? (viewableAreaActual.y * -0.5f - iUIListObject2.BottomRightEdge.y + ((!spacingAtEnds) ? 0f : itemSpacingActual) + extraEndSpacingActual) : (viewableAreaActual.y * 0.5f - iUIListObject2.TopLeftEdge.y - ((!spacingAtEnds) ? 0f : itemSpacingActual) - extraEndSpacingActual));
				}
				switch (alignment)
				{
				case ALIGNMENT.CENTER:
					x = 0f;
					break;
				case ALIGNMENT.LEFT_TOP:
					x = viewableAreaActual.x * -0.5f - iUIListObject2.TopLeftEdge.x;
					break;
				case ALIGNMENT.RIGHT_BOTTOM:
					x = viewableAreaActual.x * 0.5f - iUIListObject2.BottomRightEdge.x;
					break;
				}
				num += iUIListObject2.TopLeftEdge.y - iUIListObject2.BottomRightEdge.y + ((!flag || iUIListObject == null) ? 0f : itemSpacingActual);
			}
			iUIListObject2.transform.localPosition = new Vector3(x, y, 0f);
		}
		UpdateContentExtents(num);
		ClipItems();
		newItems.Clear();
	}

	public void AddItem(GameObject itemGO)
	{
		IUIListObject iUIListObject = (IUIListObject)itemGO.GetComponent(typeof(IUIListObject));
		if (iUIListObject == null)
		{
			Debug.LogWarning("GameObject \"" + itemGO.name + "\" does not contain any list item component suitable to be added to scroll list \"" + base.name + "\".");
		}
		else
		{
			AddItem(iUIListObject, null);
		}
	}

	public void AddItem(IUIListObject item)
	{
		AddItem(item, null);
	}

	public void AddItem(IUIListObject item, string text)
	{
		if (!m_awake)
		{
			Awake();
		}
		if (!m_started)
		{
			Start();
		}
		InsertItem(item, items.Count, text, false);
	}

	public IUIListObject CreateItem(GameObject prefab)
	{
		if (!m_awake)
		{
			Awake();
		}
		if (!m_started)
		{
			Start();
		}
		return CreateItem(prefab, items.Count, null);
	}

	public IUIListObject CreateItem(GameObject prefab, string text)
	{
		if (!m_awake)
		{
			Awake();
		}
		if (!m_started)
		{
			Start();
		}
		return CreateItem(prefab, items.Count, text);
	}

	public IUIListObject CreateItem(GameObject prefab, int position, bool doEasing)
	{
		return CreateItem(prefab, position, null, doEasing);
	}

	public IUIListObject CreateItem(GameObject prefab, int position)
	{
		return CreateItem(prefab, position, null, false);
	}

	public IUIListObject CreateItem(GameObject prefab, int position, string text)
	{
		return CreateItem(prefab, position, text, false);
	}

	public IUIListObject CreateItem(GameObject prefab, int position, string text, bool doEasing)
	{
		IUIListObject iUIListObject = (IUIListObject)prefab.GetComponent(typeof(IUIListObject));
		if (iUIListObject == null)
		{
			return null;
		}
		iUIListObject.RenderCamera = renderCamera;
		GameObject gameObject;
		if (manager != null)
		{
			if (iUIListObject.IsContainer())
			{
				gameObject = (GameObject)UnityEngine.Object.Instantiate(prefab);
				Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(SpriteRoot));
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					manager.AddSprite((SpriteRoot)componentsInChildren[i]);
				}
			}
			else
			{
				SpriteRoot spriteRoot = manager.CreateSprite(prefab);
				if (spriteRoot == null)
				{
					return null;
				}
				gameObject = spriteRoot.gameObject;
			}
		}
		else
		{
			gameObject = (GameObject)UnityEngine.Object.Instantiate(prefab);
		}
		iUIListObject = (IUIListObject)gameObject.GetComponent(typeof(IUIListObject));
		if (iUIListObject == null)
		{
			return null;
		}
		InsertItem(iUIListObject, position, text, doEasing);
		return iUIListObject;
	}

	protected void UpdateContentExtents(float change)
	{
		float num = amtOfPlay;
		float num2 = ((!spacingAtEnds) ? 0f : (itemSpacingActual * 2f)) + extraEndSpacingActual * 2f;
		contentExtents += change;
		if (orientation == ORIENTATION.HORIZONTAL)
		{
			amtOfPlay = contentExtents + num2 - viewableAreaActual.x;
			scrollMax = viewableAreaActual.x / (contentExtents + num2 - viewableAreaActual.x) * 0.5f;
		}
		else
		{
			amtOfPlay = contentExtents + num2 - viewableAreaActual.y;
			scrollMax = viewableAreaActual.y / (contentExtents + num2 - viewableAreaActual.y) * 0.5f;
		}
		float num3 = num * scrollPos / amtOfPlay;
		if (doPosEasing && num3 > 1f)
		{
			scrollPosAnim = AnimatePosition.Do(base.gameObject, EZAnimation.ANIM_MODE.By, Vector3.zero, ScrollPosInterpolator, positionEaseDuration, positionEaseDelay, null, OnPosEasingDone);
			scrollPosAnim.Data = new Vector2(num3, 1f - num3);
			itemEasers.Add(scrollPosAnim);
		}
		else
		{
			ScrollListTo_Internal(Mathf.Clamp01(num3));
		}
		doPosEasing = false;
	}

	protected float ScrollPosInterpolator(float time, float start, float delta, float duration)
	{
		Vector2 vector = (Vector2)scrollPosAnim.Data;
		ScrollListTo_Internal(EZAnimation.GetInterpolator(positionEasing)(time, vector.x, vector.y, duration));
		if (time >= duration)
		{
			scrollPosAnim = null;
		}
		return start;
	}

	protected float GetYCentered(IUIListObject item)
	{
		return 0f;
	}

	protected float GetYAlignTop(IUIListObject item)
	{
		return viewableAreaActual.y * 0.5f - item.TopLeftEdge.y;
	}

	protected float GetYAlignBottom(IUIListObject item)
	{
		return viewableAreaActual.y * -0.5f - item.BottomRightEdge.y;
	}

	protected float GetXCentered(IUIListObject item)
	{
		return 0f;
	}

	protected float GetXAlignLeft(IUIListObject item)
	{
		return viewableAreaActual.x * -0.5f - item.TopLeftEdge.x;
	}

	protected float GetXAlignRight(IUIListObject item)
	{
		return viewableAreaActual.x * 0.5f - item.BottomRightEdge.x;
	}

	public void PositionItems()
	{
		if (itemEasers.Count > 0)
		{
			for (int i = 0; i < itemEasers.Count; i++)
			{
				itemEasers[i].CompletedDelegate = null;
				itemEasers[i].End();
			}
			itemEasers.Clear();
			if (blockInputWhileEasing)
			{
				UIManager.instance.UnlockInput();
			}
		}
		if (orientation == ORIENTATION.HORIZONTAL)
		{
			PositionHorizontally(false);
		}
		else
		{
			PositionVertically(false);
		}
		UpdateContentExtents(0f);
		ClipItems();
		if (itemEasers.Count > 0 && blockInputWhileEasing)
		{
			UIManager.instance.LockInput();
		}
		doItemEasing = false;
	}

	public void RepositionItems()
	{
		if (itemEasers.Count > 0)
		{
			for (int i = 0; i < itemEasers.Count; i++)
			{
				itemEasers[i].CompletedDelegate = null;
				itemEasers[i].End();
			}
			itemEasers.Clear();
			if (blockInputWhileEasing)
			{
				UIManager.instance.UnlockInput();
			}
		}
		if (orientation == ORIENTATION.HORIZONTAL)
		{
			PositionHorizontally(true);
		}
		else
		{
			PositionVertically(true);
		}
		UpdateContentExtents(0f);
		ClipItems();
		if (itemEasers.Count > 0 && blockInputWhileEasing)
		{
			UIManager.instance.LockInput();
		}
		doItemEasing = false;
	}

	protected void PositionHorizontally(bool updateExtents)
	{
		contentExtents = 0f;
		ItemAlignmentDel itemAlignmentDel;
		switch (alignment)
		{
		case ALIGNMENT.CENTER:
			itemAlignmentDel = GetYCentered;
			break;
		case ALIGNMENT.LEFT_TOP:
			itemAlignmentDel = GetYAlignTop;
			break;
		case ALIGNMENT.RIGHT_BOTTOM:
			itemAlignmentDel = GetYAlignBottom;
			break;
		default:
			itemAlignmentDel = GetYCentered;
			break;
		}
		Vector3 vector;
		float num;
		if (direction == DIRECTION.TtoB_LtoR)
		{
			num = viewableAreaActual.x * -0.5f + ((!spacingAtEnds) ? 0f : itemSpacingActual) + extraEndSpacingActual;
			for (int i = 0; i < items.Count; i++)
			{
				if (updateExtents)
				{
					items[i].FindOuterEdges();
					items[i].UpdateCollider();
				}
				vector = new Vector3(num - items[i].TopLeftEdge.x, itemAlignmentDel(items[i]), 0f);
				if (doItemEasing)
				{
					if (newItems.Contains(items[i]))
					{
						items[i].transform.localPosition = vector;
					}
					else
					{
						itemEasers.Add(AnimatePosition.Do(items[i].gameObject, EZAnimation.ANIM_MODE.To, vector, EZAnimation.GetInterpolator(positionEasing), positionEaseDuration, positionEaseDelay, null, OnPosEasingDone));
					}
				}
				else
				{
					items[i].transform.localPosition = vector;
				}
				float num2 = items[i].BottomRightEdge.x - items[i].TopLeftEdge.x + itemSpacingActual;
				contentExtents += num2;
				num += num2;
				items[i].Index = i;
			}
			if (!spacingAtEnds)
			{
				contentExtents -= itemSpacingActual;
			}
			return;
		}
		num = viewableAreaActual.x * 0.5f - ((!spacingAtEnds) ? 0f : itemSpacingActual) - extraEndSpacingActual;
		for (int j = 0; j < items.Count; j++)
		{
			if (updateExtents)
			{
				items[j].FindOuterEdges();
				items[j].UpdateCollider();
			}
			vector = new Vector3(num - items[j].BottomRightEdge.x, itemAlignmentDel(items[j]), 0f);
			if (doItemEasing)
			{
				if (newItems.Contains(items[j]))
				{
					items[j].transform.localPosition = vector;
				}
				else
				{
					itemEasers.Add(AnimatePosition.Do(items[j].gameObject, EZAnimation.ANIM_MODE.To, vector, EZAnimation.GetInterpolator(positionEasing), positionEaseDuration, positionEaseDelay, null, OnPosEasingDone));
				}
			}
			else
			{
				items[j].transform.localPosition = vector;
			}
			float num2 = items[j].BottomRightEdge.x - items[j].TopLeftEdge.x + itemSpacingActual;
			contentExtents += num2;
			num -= num2;
			items[j].Index = j;
		}
		if (!spacingAtEnds)
		{
			contentExtents -= itemSpacingActual;
		}
	}

	protected void PositionVertically(bool updateExtents)
	{
		contentExtents = 0f;
		ItemAlignmentDel itemAlignmentDel;
		switch (alignment)
		{
		case ALIGNMENT.CENTER:
			itemAlignmentDel = GetXCentered;
			break;
		case ALIGNMENT.LEFT_TOP:
			itemAlignmentDel = GetXAlignLeft;
			break;
		case ALIGNMENT.RIGHT_BOTTOM:
			itemAlignmentDel = GetXAlignRight;
			break;
		default:
			itemAlignmentDel = GetXCentered;
			break;
		}
		Vector3 vector;
		float num;
		if (direction == DIRECTION.TtoB_LtoR)
		{
			num = viewableAreaActual.y * 0.5f - ((!spacingAtEnds) ? 0f : itemSpacingActual) - extraEndSpacingActual;
			for (int i = 0; i < items.Count; i++)
			{
				if (updateExtents)
				{
					items[i].FindOuterEdges();
					items[i].UpdateCollider();
				}
				vector = new Vector3(itemAlignmentDel(items[i]), num - items[i].TopLeftEdge.y, 0f);
				if (doItemEasing)
				{
					if (newItems.Contains(items[i]))
					{
						items[i].transform.localPosition = vector;
					}
					else
					{
						itemEasers.Add(AnimatePosition.Do(items[i].gameObject, EZAnimation.ANIM_MODE.To, vector, EZAnimation.GetInterpolator(positionEasing), positionEaseDuration, positionEaseDelay, null, OnPosEasingDone));
					}
				}
				else
				{
					items[i].transform.localPosition = vector;
				}
				float num2 = items[i].TopLeftEdge.y - items[i].BottomRightEdge.y + itemSpacingActual;
				contentExtents += num2;
				num -= num2;
				items[i].Index = i;
			}
			if (!spacingAtEnds)
			{
				contentExtents -= itemSpacingActual;
			}
			return;
		}
		num = viewableAreaActual.y * -0.5f + ((!spacingAtEnds) ? 0f : itemSpacingActual) + extraEndSpacingActual;
		for (int j = 0; j < items.Count; j++)
		{
			if (updateExtents)
			{
				items[j].FindOuterEdges();
				items[j].UpdateCollider();
			}
			vector = new Vector3(itemAlignmentDel(items[j]), num - items[j].BottomRightEdge.y, 0f);
			if (doItemEasing)
			{
				if (newItems.Contains(items[j]))
				{
					items[j].transform.localPosition = vector;
				}
				else
				{
					itemEasers.Add(AnimatePosition.Do(items[j].gameObject, EZAnimation.ANIM_MODE.To, vector, EZAnimation.GetInterpolator(positionEasing), positionEaseDuration, positionEaseDelay, null, OnPosEasingDone));
				}
			}
			else
			{
				items[j].transform.localPosition = vector;
			}
			float num2 = items[j].TopLeftEdge.y - items[j].BottomRightEdge.y + itemSpacingActual;
			contentExtents += num2;
			num += num2;
			items[j].Index = j;
		}
		if (!spacingAtEnds)
		{
			contentExtents -= itemSpacingActual;
		}
	}

	protected void OnPosEasingDone(EZAnimation anim)
	{
		itemEasers.Remove(anim);
		if (itemEasers.Count == 0 && blockInputWhileEasing)
		{
			UIManager.instance.UnlockInput();
		}
	}

	protected void ClipItems()
	{
		if (mover == null || items.Count < 1 || !clipContents || !base.gameObject.active)
		{
			return;
		}
		IUIListObject iUIListObject = null;
		IUIListObject iUIListObject2 = null;
		if (orientation == ORIENTATION.HORIZONTAL)
		{
			float x = mover.transform.localPosition.x;
			float num = viewableAreaActual.x * -0.5f - x;
			float num2 = viewableAreaActual.x * 0.5f - x;
			int i = (int)((float)(items.Count - 1) * Mathf.Clamp01(scrollPos));
			if (direction == DIRECTION.TtoB_LtoR)
			{
				float x2 = items[i].transform.localPosition.x;
				if (items[i].BottomRightEdge.x + x2 >= num)
				{
					for (i--; i > -1; i--)
					{
						x2 = items[i].transform.localPosition.x;
						if (items[i].BottomRightEdge.x + x2 < num)
						{
							break;
						}
					}
					iUIListObject = items[i + 1];
				}
				else
				{
					for (; i < items.Count; i++)
					{
						x2 = items[i].transform.localPosition.x;
						if (items[i].BottomRightEdge.x + x2 >= num)
						{
							iUIListObject = items[i];
							break;
						}
					}
				}
				if (iUIListObject != null)
				{
					tempVisItems.Add(iUIListObject);
					if (!iUIListObject.gameObject.active)
					{
						iUIListObject.gameObject.SetActiveRecursively(true);
					}
					iUIListObject.Hide(false);
					iUIListObject.ClippingRect = clientClippingRect;
					x2 = iUIListObject.transform.localPosition.x;
					if (iUIListObject.BottomRightEdge.x + x2 < num2)
					{
						for (i = iUIListObject.Index + 1; i < items.Count; i++)
						{
							x2 = items[i].transform.localPosition.x;
							if (items[i].BottomRightEdge.x + x2 >= num2)
							{
								if (!items[i].gameObject.active)
								{
									items[i].gameObject.SetActiveRecursively(true);
								}
								items[i].Hide(false);
								items[i].ClippingRect = clientClippingRect;
								tempVisItems.Add(items[i]);
								break;
							}
							if (!items[i].gameObject.active)
							{
								items[i].gameObject.SetActiveRecursively(true);
							}
							items[i].Hide(false);
							items[i].Clipped = false;
							tempVisItems.Add(items[i]);
						}
					}
				}
			}
			else
			{
				float x2 = items[i].transform.localPosition.x;
				if (items[i].TopLeftEdge.x + x2 <= num2)
				{
					for (i--; i > -1; i--)
					{
						x2 = items[i].transform.localPosition.x;
						if (items[i].TopLeftEdge.x + x2 > num2)
						{
							break;
						}
					}
					iUIListObject = items[i + 1];
				}
				else
				{
					for (; i < items.Count; i++)
					{
						x2 = items[i].transform.localPosition.x;
						if (items[i].TopLeftEdge.x + x2 <= num2)
						{
							iUIListObject = items[i];
							break;
						}
					}
				}
				if (iUIListObject != null)
				{
					tempVisItems.Add(iUIListObject);
					if (!iUIListObject.gameObject.active)
					{
						iUIListObject.gameObject.SetActiveRecursively(true);
					}
					iUIListObject.Hide(false);
					iUIListObject.ClippingRect = clientClippingRect;
					x2 = iUIListObject.transform.localPosition.x;
					if (iUIListObject.TopLeftEdge.x + x2 > num)
					{
						for (i = iUIListObject.Index + 1; i < items.Count; i++)
						{
							x2 = items[i].transform.localPosition.x;
							if (items[i].TopLeftEdge.x + x2 <= num)
							{
								if (!items[i].gameObject.active)
								{
									items[i].gameObject.SetActiveRecursively(true);
								}
								items[i].Hide(false);
								items[i].ClippingRect = clientClippingRect;
								tempVisItems.Add(items[i]);
								break;
							}
							if (!items[i].gameObject.active)
							{
								items[i].gameObject.SetActiveRecursively(true);
							}
							items[i].Hide(false);
							items[i].Clipped = false;
							tempVisItems.Add(items[i]);
						}
					}
				}
			}
		}
		else
		{
			float y = mover.transform.localPosition.y;
			float num3 = viewableAreaActual.y * 0.5f - y;
			float num4 = viewableAreaActual.y * -0.5f - y;
			int j = (int)((float)(items.Count - 1) * Mathf.Clamp01(scrollPos));
			if (direction == DIRECTION.TtoB_LtoR)
			{
				float y2 = items[j].transform.localPosition.y;
				if (items[j].BottomRightEdge.y + y2 <= num3)
				{
					for (j--; j > -1; j--)
					{
						y2 = items[j].transform.localPosition.y;
						if (items[j].BottomRightEdge.y + y2 > num3)
						{
							break;
						}
					}
					iUIListObject = items[j + 1];
				}
				else
				{
					for (; j < items.Count; j++)
					{
						y2 = items[j].transform.localPosition.y;
						if (items[j].BottomRightEdge.y + y2 <= num3)
						{
							iUIListObject = items[j];
							break;
						}
					}
				}
				if (iUIListObject != null)
				{
					tempVisItems.Add(iUIListObject);
					if (!iUIListObject.gameObject.active)
					{
						iUIListObject.gameObject.SetActiveRecursively(true);
					}
					iUIListObject.Hide(false);
					iUIListObject.ClippingRect = clientClippingRect;
					y2 = iUIListObject.transform.localPosition.y;
					if (iUIListObject.BottomRightEdge.y + y2 > num4)
					{
						for (j = iUIListObject.Index + 1; j < items.Count; j++)
						{
							y2 = items[j].transform.localPosition.y;
							if (items[j].BottomRightEdge.y + y2 <= num4)
							{
								if (!items[j].gameObject.active)
								{
									items[j].gameObject.SetActiveRecursively(true);
								}
								items[j].Hide(false);
								items[j].ClippingRect = clientClippingRect;
								tempVisItems.Add(items[j]);
								break;
							}
							if (!items[j].gameObject.active)
							{
								items[j].gameObject.SetActiveRecursively(true);
							}
							items[j].Hide(false);
							items[j].Clipped = false;
							tempVisItems.Add(items[j]);
						}
					}
				}
			}
			else
			{
				float y2 = items[j].transform.localPosition.y;
				if (items[j].TopLeftEdge.y + y2 >= num4)
				{
					for (j--; j > -1; j--)
					{
						y2 = items[j].transform.localPosition.y;
						if (items[j].TopLeftEdge.y + y2 < num4)
						{
							break;
						}
					}
					iUIListObject = items[j + 1];
				}
				else
				{
					for (; j < items.Count; j++)
					{
						y2 = items[j].transform.localPosition.y;
						if (items[j].TopLeftEdge.y + y2 >= num4)
						{
							iUIListObject = items[j];
							break;
						}
					}
				}
				if (iUIListObject != null)
				{
					tempVisItems.Add(iUIListObject);
					if (!iUIListObject.gameObject.active)
					{
						iUIListObject.gameObject.SetActiveRecursively(true);
					}
					iUIListObject.Hide(false);
					iUIListObject.ClippingRect = clientClippingRect;
					y2 = iUIListObject.transform.localPosition.y;
					if (iUIListObject.TopLeftEdge.y + y2 < num3)
					{
						for (j = iUIListObject.Index + 1; j < items.Count; j++)
						{
							y2 = items[j].transform.localPosition.y;
							if (items[j].TopLeftEdge.y + y2 >= num3)
							{
								if (!items[j].gameObject.active)
								{
									items[j].gameObject.SetActiveRecursively(true);
								}
								items[j].Hide(false);
								items[j].ClippingRect = clientClippingRect;
								tempVisItems.Add(items[j]);
								break;
							}
							if (!items[j].gameObject.active)
							{
								items[j].gameObject.SetActiveRecursively(true);
							}
							items[j].Hide(false);
							items[j].Clipped = false;
							tempVisItems.Add(items[j]);
						}
					}
				}
			}
		}
		if (iUIListObject == null)
		{
			return;
		}
		iUIListObject2 = tempVisItems[tempVisItems.Count - 1];
		if (visibleItems.Count > 0)
		{
			if (visibleItems[0].Index > iUIListObject2.Index || visibleItems[visibleItems.Count - 1].Index < iUIListObject.Index)
			{
				for (int k = 0; k < visibleItems.Count; k++)
				{
					visibleItems[k].Hide(true);
					if (!visibleItems[k].Managed)
					{
						visibleItems[k].gameObject.SetActiveRecursively(false);
					}
				}
			}
			else
			{
				for (int l = 0; l < visibleItems.Count && visibleItems[l].Index < iUIListObject.Index; l++)
				{
					visibleItems[l].Hide(true);
					if (!visibleItems[l].Managed)
					{
						visibleItems[l].gameObject.SetActiveRecursively(false);
					}
				}
				int num5 = visibleItems.Count - 1;
				while (num5 > -1 && visibleItems[num5].Index > iUIListObject2.Index)
				{
					visibleItems[num5].Hide(true);
					if (!visibleItems[num5].Managed)
					{
						visibleItems[num5].gameObject.SetActiveRecursively(false);
					}
					num5--;
				}
			}
		}
		List<IUIListObject> list = visibleItems;
		visibleItems = tempVisItems;
		tempVisItems = list;
		tempVisItems.Clear();
	}

	public void DidSelect(IUIListObject item)
	{
		if (selectedItem != null)
		{
			selectedItem.selected = false;
		}
		selectedItem = item;
		item.selected = true;
		DidClick(item);
	}

	public void DidClick(IUIObject item)
	{
		lastClickedControl = item;
		if (scriptWithMethodToInvoke != null)
		{
			scriptWithMethodToInvoke.Invoke(methodToInvokeOnSelect, 0f);
		}
		if (changeDelegate != null)
		{
			changeDelegate(this);
		}
	}

	public void ListDragged(POINTER_INFO ptr)
	{
		if (!touchScroll || !controlIsEnabled)
		{
			return;
		}
		autoScrolling = false;
		Plane plane = default(Plane);
		if (Mathf.Approximately(ptr.inputDelta.sqrMagnitude, 0f))
		{
			scrollDelta = 0f;
			return;
		}
		listMoved = true;
		plane.SetNormalAndPosition(mover.transform.forward * -1f, mover.transform.position);
		float enter;
		plane.Raycast(ptr.ray, out enter);
		Vector3 position = ptr.ray.origin + ptr.ray.direction * enter;
		plane.Raycast(ptr.prevRay, out enter);
		Vector3 position2 = ptr.prevRay.origin + ptr.prevRay.direction * enter;
		position = base.transform.InverseTransformPoint(position);
		position2 = base.transform.InverseTransformPoint(position2);
		Vector3 vector = position - position2;
		if (orientation == ORIENTATION.HORIZONTAL)
		{
			scrollDelta = (0f - vector.x) / amtOfPlay;
		}
		else
		{
			scrollDelta = vector.y / amtOfPlay;
		}
		float num = scrollPos + scrollDelta;
		if (num > 1f)
		{
			scrollDelta *= Mathf.Clamp01(1f - (num - 1f) / scrollMax);
		}
		else if (num < 0f)
		{
			scrollDelta *= Mathf.Clamp01(1f + num / scrollMax);
		}
		if (direction == DIRECTION.BtoT_RtoL)
		{
			scrollDelta *= -1f;
		}
		ScrollListTo_Internal(scrollPos + scrollDelta);
		noTouch = false;
		isScrolling = true;
	}

	public void ScrollWheel(float amt)
	{
		if (direction == DIRECTION.BtoT_RtoL)
		{
			amt *= -1f;
		}
		ScrollListTo(Mathf.Clamp01(scrollPos - amt * scrollWheelFactor / amtOfPlay));
	}

	public void PointerReleased()
	{
		noTouch = true;
		if (scrollInertia != 0f)
		{
			scrollDelta = scrollInertia;
		}
		scrollInertia = 0f;
		if (snap && listMoved)
		{
			CalcSnapItem();
		}
		listMoved = false;
	}

	public void OnEnable()
	{
		base.gameObject.SetActiveRecursively(true);
		if (repositionOnEnable)
		{
			RepositionItems();
		}
		ClipItems();
	}

	protected virtual void OnDisable()
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

	public void SetSelectedItem(int index)
	{
		IUIListObject iUIListObject = selectedItem;
		if (index < 0 || index >= items.Count)
		{
			if (selectedItem != null)
			{
				selectedItem.selected = false;
			}
			selectedItem = null;
			if (iUIListObject != selectedItem && changeDelegate != null)
			{
				changeDelegate(this);
			}
			return;
		}
		IUIListObject iUIListObject2 = items[index];
		if (selectedItem != null)
		{
			selectedItem.selected = false;
		}
		selectedItem = iUIListObject2;
		iUIListObject2.selected = true;
		if (iUIListObject != selectedItem && changeDelegate != null)
		{
			changeDelegate(this);
		}
	}

	public IUIListObject GetItem(int index)
	{
		if (index < 0 || index >= items.Count)
		{
			return null;
		}
		return items[index];
	}

	public void RemoveItem(int index, bool destroy)
	{
		RemoveItem(index, destroy, false);
	}

	public void RemoveItem(int index, bool destroy, bool doEasing)
	{
		if (index >= 0 && index < items.Count)
		{
			if (index == items.Count - 1)
			{
				doItemEasing = false;
			}
			else
			{
				doItemEasing = doEasing;
			}
			doPosEasing = doEasing;
			if (container != null)
			{
				container.RemoveChild(items[index].gameObject);
			}
			if (selectedItem == items[index])
			{
				selectedItem = null;
				items[index].selected = false;
			}
			if (lastClickedControl != null && (lastClickedControl == items[index] || (lastClickedControl.Container != null && lastClickedControl.Container.Equals(items[index]))))
			{
				lastClickedControl = null;
			}
			visibleItems.Remove(items[index]);
			if (destroy)
			{
				items[index].Delete();
				UnityEngine.Object.Destroy(items[index].gameObject);
			}
			else
			{
				items[index].transform.parent = null;
				items[index].gameObject.SetActiveRecursively(false);
			}
			items.RemoveAt(index);
			PositionItems();
		}
	}

	public void RemoveItem(IUIListObject item, bool destroy)
	{
		RemoveItem(item, destroy, false);
	}

	public void RemoveItem(IUIListObject item, bool destroy, bool doEasing)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i] == item)
			{
				RemoveItem(i, destroy, doEasing);
				break;
			}
		}
	}

	public void ClearList(bool destroy)
	{
		RemoveItemsFromContainer();
		selectedItem = null;
		lastClickedControl = null;
		for (int i = 0; i < items.Count; i++)
		{
			items[i].transform.parent = null;
			if (destroy)
			{
				UnityEngine.Object.Destroy(items[i].gameObject);
			}
			else
			{
				items[i].gameObject.SetActiveRecursively(false);
			}
		}
		visibleItems.Clear();
		items.Clear();
		PositionItems();
	}

	public void OnInput(POINTER_INFO ptr)
	{
		if (!m_controlIsEnabled)
		{
			if (Container != null)
			{
				ptr.callerIsControl = true;
				Container.OnInput(ptr);
			}
			return;
		}
		if (Vector3.SqrMagnitude(ptr.origPos - ptr.devicePos) > dragThreshold * dragThreshold)
		{
			ptr.isTap = false;
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			{
				ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
			}
		}
		else
		{
			ptr.isTap = true;
		}
		if (inputDelegate != null)
		{
			inputDelegate(ref ptr);
		}
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
			if (ptr.active)
			{
				ListDragged(ptr);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.DRAG:
			if (!ptr.isTap)
			{
				ListDragged(ptr);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.RELEASE:
		case POINTER_INFO.INPUT_EVENT.TAP:
		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			PointerReleased();
			break;
		}
		if (scrollWheelFactor != 0f && ptr.inputDelta.z != 0f && ptr.type != POINTER_INFO.POINTER_TYPE.RAY)
		{
			ScrollWheel(ptr.inputDelta.z);
		}
		if (Container != null)
		{
			ptr.callerIsControl = true;
			Container.OnInput(ptr);
		}
	}

	public void LateUpdate()
	{
		if (newItems.Count != 0)
		{
			if (itemsInserted || doItemEasing)
			{
				RepositionItems();
			}
			else
			{
				PositionNewItems();
			}
			itemsInserted = false;
			newItems.Clear();
		}
		timeDelta = Time.realtimeSinceStartup - lastTime;
		lastTime = Time.realtimeSinceStartup;
		inertiaLerpTime += timeDelta;
		if (cachedPos != base.transform.position || cachedRot != base.transform.rotation || cachedScale != base.transform.lossyScale)
		{
			cachedPos = base.transform.position;
			cachedRot = base.transform.rotation;
			cachedScale = base.transform.lossyScale;
			CalcClippingRect();
			if (clipWhenMoving)
			{
				ClipItems();
			}
		}
		if (itemEasers.Count > 0)
		{
			ClipItems();
		}
		if (!noTouch && inertiaLerpTime >= inertiaLerpInterval)
		{
			scrollInertia = Mathf.Lerp(scrollInertia, scrollDelta, lowPassFilterFactor);
			scrollDelta = 0f;
			inertiaLerpTime %= inertiaLerpInterval;
		}
		if (isScrolling && noTouch && !autoScrolling)
		{
			scrollDelta -= scrollDelta * scrollDecelCoef;
			if (scrollPos < 0f)
			{
				scrollPos -= scrollPos * 1f * (timeDelta / 0.166f);
				scrollDelta *= Mathf.Clamp01(1f + scrollPos / scrollMax);
			}
			else if (scrollPos > 1f)
			{
				scrollPos -= (scrollPos - 1f) * 1f * (timeDelta / 0.166f);
				scrollDelta *= Mathf.Clamp01(1f - (scrollPos - 1f) / scrollMax);
			}
			if (Mathf.Abs(scrollDelta) < 0.0001f)
			{
				scrollDelta = 0f;
				if (scrollPos > -0.0001f && scrollPos < 0.0001f)
				{
					scrollPos = Mathf.Clamp01(scrollPos);
				}
			}
			ScrollListTo_Internal(scrollPos + scrollDelta);
			if (scrollPos >= 0f && scrollPos <= 1.001f && scrollDelta == 0f)
			{
				isScrolling = false;
			}
		}
		else if (autoScrolling)
		{
			autoScrollTime += timeDelta;
			if (autoScrollTime >= autoScrollDuration)
			{
				autoScrolling = false;
				scrollPos = autoScrollPos;
			}
			else
			{
				scrollPos = autoScrollInterpolator(autoScrollTime, autoScrollStart, autoScrollDelta, autoScrollDuration);
			}
			ScrollListTo_Internal(scrollPos);
		}
	}

	protected void CalcSnapItem()
	{
		float num = 100000000f;
		IUIListObject iUIListObject = null;
		IUIListObject iUIListObject2 = null;
		int num2 = 1;
		if (items.Count < 1)
		{
			return;
		}
		float num3;
		float scrollTime;
		if (Mathf.Approximately(scrollDelta, 0f))
		{
			scrollTime = minSnapDuration;
			num3 = scrollPos;
		}
		else
		{
			num3 = scrollPos + scrollDelta / scrollDecelCoef;
			float num4 = Mathf.Abs(scrollDelta);
			scrollTime = Time.fixedDeltaTime * (scrollStopThresholdLog - Mathf.Log10(num4)) / Mathf.Log10((num4 - num4 * scrollDecelCoef) / num4);
			scrollTime = Mathf.Max(scrollTime, minSnapDuration);
		}
		if (num3 >= 1f || num3 <= 0f)
		{
			if (num3 <= 0f)
			{
				ScrollToItem(0, scrollTime);
			}
			else
			{
				ScrollToItem(items.Count - 1, scrollTime);
			}
			return;
		}
		int num5 = (int)Mathf.Clamp((float)(items.Count - 1) * num3, 0f, items.Count - 1);
		if (orientation == ORIENTATION.HORIZONTAL)
		{
			float num6 = ((direction != 0) ? 1f : (-1f));
			iUIListObject = items[num5];
			num = Mathf.Abs(num3 + num6 * iUIListObject.transform.localPosition.x / amtOfPlay);
			if (num5 + num2 < items.Count)
			{
				iUIListObject2 = items[num5 + num2];
				float num7 = Mathf.Abs(num3 + num6 * iUIListObject2.transform.localPosition.x / amtOfPlay);
				if (num7 < num)
				{
					num = num7;
					iUIListObject = iUIListObject2;
					num5 += num2;
				}
				else
				{
					num2 = -1;
				}
			}
			else
			{
				num2 = -1;
			}
			for (int i = num5 + num2; i > -1 && i < items.Count; i += num2)
			{
				float num7 = Mathf.Abs(num3 + num6 * items[i].transform.localPosition.x / amtOfPlay);
				if (num7 < num)
				{
					num = num7;
					iUIListObject = items[i];
					continue;
				}
				break;
			}
			ScrollToItem(iUIListObject, scrollTime);
			return;
		}
		float num8 = ((direction != 0) ? (-1f) : 1f);
		iUIListObject = items[num5];
		num = Mathf.Abs(num3 + num8 * iUIListObject.transform.localPosition.y / amtOfPlay);
		if (num5 + num2 < items.Count)
		{
			iUIListObject2 = items[num5 + num2];
			float num7 = Mathf.Abs(num3 + num8 * iUIListObject2.transform.localPosition.y / amtOfPlay);
			if (num7 < num)
			{
				num = num7;
				iUIListObject = iUIListObject2;
				num5 += num2;
			}
			else
			{
				num2 = -1;
			}
		}
		else
		{
			num2 = -1;
		}
		for (int j = num5 + num2; j > -1 && j < items.Count; j += num2)
		{
			float num7 = Mathf.Abs(num3 + num8 * items[j].transform.localPosition.y / amtOfPlay);
			if (num7 < num)
			{
				num = num7;
				iUIListObject = items[j];
				continue;
			}
			break;
		}
		ScrollToItem(iUIListObject, scrollTime);
	}

	protected void AddItemsToContainer()
	{
		if (container != null)
		{
			for (int i = 0; i < items.Count; i++)
			{
				container.AddChild(items[i].gameObject);
			}
		}
	}

	protected void RemoveItemsFromContainer()
	{
		if (container != null)
		{
			for (int i = 0; i < items.Count; i++)
			{
				container.RemoveChild(items[i].gameObject);
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

	public void SetInputDelegate(EZInputDelegate del)
	{
		inputDelegate = del;
	}

	public void AddInputDelegate(EZInputDelegate del)
	{
		inputDelegate = (EZInputDelegate)Delegate.Combine(inputDelegate, del);
	}

	public void RemoveInputDelegate(EZInputDelegate del)
	{
		inputDelegate = (EZInputDelegate)Delegate.Remove(inputDelegate, del);
	}

	public void SetValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate = del;
	}

	public void AddValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate = (EZValueChangedDelegate)Delegate.Combine(changeDelegate, del);
	}

	public void RemoveValueChangedDelegate(EZValueChangedDelegate del)
	{
		changeDelegate = (EZValueChangedDelegate)Delegate.Remove(changeDelegate, del);
	}

	public void AddItemSnappedDelegate(ItemSnappedDelegate del)
	{
		itemSnappedDel = (ItemSnappedDelegate)Delegate.Combine(itemSnappedDel, del);
	}

	public void RemoveItemSnappedDelegate(ItemSnappedDelegate del)
	{
		itemSnappedDel = (ItemSnappedDelegate)Delegate.Remove(itemSnappedDel, del);
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

	private void OnDrawGizmosSelected()
	{
		SetupCameraAndSizes();
		Vector3 vector = base.transform.position - base.transform.TransformDirection(Vector3.right * viewableAreaActual.x * 0.5f * base.transform.lossyScale.x) + base.transform.TransformDirection(Vector3.up * viewableAreaActual.y * 0.5f * base.transform.lossyScale.y);
		Vector3 vector2 = base.transform.position - base.transform.TransformDirection(Vector3.right * viewableAreaActual.x * 0.5f * base.transform.lossyScale.x) - base.transform.TransformDirection(Vector3.up * viewableAreaActual.y * 0.5f * base.transform.lossyScale.y);
		Vector3 vector3 = base.transform.position + base.transform.TransformDirection(Vector3.right * viewableAreaActual.x * 0.5f * base.transform.lossyScale.x) - base.transform.TransformDirection(Vector3.up * viewableAreaActual.y * 0.5f * base.transform.lossyScale.y);
		Vector3 vector4 = base.transform.position + base.transform.TransformDirection(Vector3.right * viewableAreaActual.x * 0.5f * base.transform.lossyScale.x) + base.transform.TransformDirection(Vector3.up * viewableAreaActual.y * 0.5f * base.transform.lossyScale.y);
		Gizmos.color = new Color(1f, 0f, 0.5f, 1f);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector4, vector);
	}

	public static UIScrollList Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIScrollList)gameObject.AddComponent(typeof(UIScrollList));
	}

	public static UIScrollList Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIScrollList)gameObject.AddComponent(typeof(UIScrollList));
	}

	public void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvokeOnSelect);
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
