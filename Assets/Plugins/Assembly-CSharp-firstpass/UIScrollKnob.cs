using UnityEngine;

public class UIScrollKnob : UIButton
{
	protected Vector3 origPos;

	protected UISlider slider;

	protected float maxScrollPos;

	protected Plane ctrlPlane;

	protected Vector2 colliderSizeFactor;

	protected float colliderExtent;

	private float dist;

	private Vector3 inputPoint;

	private Vector3 newPos;

	private Vector3 prevPoint;

	protected override void Awake()
	{
		base.Awake();
		origPos = base.transform.localPosition;
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		base.OnInput(ref ptr);
		if (m_controlIsEnabled)
		{
			switch (ptr.evt)
			{
			case POINTER_INFO.INPUT_EVENT.PRESS:
				prevPoint = GetLocalInputPoint(ptr.ray);
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
				inputPoint = GetLocalInputPoint(ptr.ray);
				dist = inputPoint.x - prevPoint.x;
				prevPoint = inputPoint;
				newPos = base.transform.localPosition;
				newPos.x = Mathf.Clamp(newPos.x + dist, origPos.x, origPos.x + maxScrollPos);
				base.transform.localPosition = newPos;
				prevPoint.x = Mathf.Clamp(prevPoint.x, origPos.x - colliderExtent, origPos.x + colliderExtent + maxScrollPos);
				slider.ScrollKnobMoved(this, GetScrollPos());
				break;
			}
		}
	}

	public void SetStartPos(Vector3 startPos)
	{
		origPos = startPos;
	}

	protected Vector3 GetLocalInputPoint(Ray ray)
	{
		ctrlPlane.SetNormalAndPosition(base.transform.forward * -1f, base.transform.position);
		ctrlPlane.Raycast(ray, out dist);
		return base.transform.parent.InverseTransformPoint(ray.origin + ray.direction * dist);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (s is UIScrollKnob)
		{
			UIScrollKnob uIScrollKnob = (UIScrollKnob)s;
			if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
			{
				origPos = uIScrollKnob.origPos;
				ctrlPlane = uIScrollKnob.ctrlPlane;
				slider = uIScrollKnob.slider;
			}
			if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
			{
				maxScrollPos = uIScrollKnob.maxScrollPos;
				colliderSizeFactor = uIScrollKnob.colliderSizeFactor;
			}
		}
	}

	public void SetColliderSizeFactor(Vector2 csf)
	{
		colliderSizeFactor = csf;
	}

	public override void UpdateCollider()
	{
		base.UpdateCollider();
		if (base.collider is BoxCollider && !IsHidden())
		{
			BoxCollider boxCollider = (BoxCollider)base.collider;
			boxCollider.size = new Vector3(boxCollider.size.x * colliderSizeFactor.x, boxCollider.size.y * colliderSizeFactor.y, 0.001f);
			colliderExtent = boxCollider.size.x * 0.5f;
		}
	}

	public float GetScrollPos()
	{
		return (base.transform.localPosition.x - origPos.x) / maxScrollPos;
	}

	public void SetPosition(float pos)
	{
		base.transform.localPosition = origPos + Vector3.right * maxScrollPos * pos;
	}

	public void SetSlider(UISlider s)
	{
		slider = s;
	}

	public UISlider GetSlider()
	{
		return slider;
	}

	public void SetMaxScroll(float max)
	{
		maxScrollPos = max;
	}

	public void SetupAppearance()
	{
		Start();
		InitUVs();
		UpdateUVs();
	}

	public new static UIScrollKnob Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIScrollKnob)gameObject.AddComponent(typeof(UIScrollKnob));
	}

	public new static UIScrollKnob Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIScrollKnob)gameObject.AddComponent(typeof(UIScrollKnob));
	}
}
