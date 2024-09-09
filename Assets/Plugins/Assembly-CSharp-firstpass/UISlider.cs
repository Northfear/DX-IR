using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Slider")]
public class UISlider : AutoSpriteControlBase
{
	protected float m_value;

	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvoke = string.Empty;

	public float defaultValue;

	public float stopKnobFromEdge;

	public Vector3 knobOffset = new Vector3(0f, 0f, -0.1f);

	public Vector2 knobSize;

	public Vector2 knobColliderSizeFactor = new Vector2(1f, 1f);

	protected AutoSprite emptySprite;

	protected UIScrollKnob knob;

	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[5]
	{
		new TextureAnim("Filled bar"),
		new TextureAnim("Empty bar"),
		new TextureAnim("Knob, Normal"),
		new TextureAnim("Knob, Over"),
		new TextureAnim("Knob, Active")
	};

	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[5]
	{
		null,
		null,
		new EZTransitionList(new EZTransition[3]
		{
			new EZTransition("From Over"),
			new EZTransition("From Active"),
			new EZTransition("From Disabled")
		}),
		new EZTransitionList(new EZTransition[2]
		{
			new EZTransition("From Normal"),
			new EZTransition("From Active")
		}),
		new EZTransitionList(new EZTransition[2]
		{
			new EZTransition("From Normal"),
			new EZTransition("From Over")
		})
	};

	public SpriteRoot[] filledLayers = new SpriteRoot[0];

	public SpriteRoot[] emptyLayers = new SpriteRoot[0];

	public SpriteRoot[] knobLayers = new SpriteRoot[0];

	protected float truncFloor;

	protected float truncRange;

	protected int[] filledIndices;

	protected int[] emptyIndices;

	public override bool controlIsEnabled
	{
		get
		{
			return m_controlIsEnabled;
		}
		set
		{
			m_controlIsEnabled = value;
			if (knob != null)
			{
				knob.controlIsEnabled = value;
			}
		}
	}

	public float Value
	{
		get
		{
			return m_value;
		}
		set
		{
			float value2 = m_value;
			m_value = Mathf.Clamp01(value);
			if (m_value != value2)
			{
				UpdateValue();
			}
		}
	}

	public override TextureAnim[] States
	{
		get
		{
			return states;
		}
		set
		{
			states = value;
		}
	}

	public override EZTransitionList[] Transitions
	{
		get
		{
			return transitions;
		}
		set
		{
			transitions = value;
		}
	}

	public override IUIContainer Container
	{
		get
		{
			return base.Container;
		}
		set
		{
			if (value != container)
			{
				if (container != null)
				{
					container.RemoveChild(emptySprite.gameObject);
					container.RemoveChild(knob.gameObject);
				}
				if (value != null)
				{
					if (emptySprite != null)
					{
						value.AddChild(emptySprite.gameObject);
					}
					if (knob != null)
					{
						value.AddChild(knob.gameObject);
					}
				}
			}
			base.Container = value;
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
			if (!ignoreClipping)
			{
				base.Clipped = value;
				emptySprite.Clipped = value;
				knob.Clipped = value;
			}
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
			if (!ignoreClipping)
			{
				base.ClippingRect = value;
				emptySprite.ClippingRect = value;
				knob.ClippingRect = value;
			}
		}
	}

	public override EZTransitionList GetTransitions(int index)
	{
		if (index >= transitions.Length)
		{
			return null;
		}
		return transitions[index];
	}

	protected override void Awake()
	{
		base.Awake();
		m_value = defaultValue;
	}

	public override void Start()
	{
		if (m_started)
		{
			return;
		}
		base.Start();
		aggregateLayers = new SpriteRoot[2][];
		aggregateLayers[0] = filledLayers;
		aggregateLayers[1] = emptyLayers;
		if (Application.isPlaying)
		{
			truncFloor = stopKnobFromEdge / width;
			truncRange = 1f - truncFloor * 2f;
			filledIndices = new int[filledLayers.Length];
			emptyIndices = new int[emptyLayers.Length];
			for (int i = 0; i < filledLayers.Length; i++)
			{
				if (filledLayers[i] == null)
				{
					Debug.LogError("A null layer sprite was encountered on control \"" + base.name + "\". Please fill in the layer reference, or remove the empty element.");
					continue;
				}
				filledIndices[i] = filledLayers[i].GetStateIndex("filled");
				if (filledIndices[i] != -1)
				{
					filledLayers[i].SetState(filledIndices[i]);
				}
			}
			for (int j = 0; j < emptyLayers.Length; j++)
			{
				if (emptyLayers[j] == null)
				{
					Debug.LogError("A null layer sprite was encountered on control \"" + base.name + "\". Please fill in the layer reference, or remove the empty element.");
					continue;
				}
				emptyIndices[j] = emptyLayers[j].GetStateIndex("empty");
				if (emptyIndices[j] != -1)
				{
					emptyLayers[j].SetState(emptyIndices[j]);
				}
			}
			GameObject gameObject = new GameObject();
			gameObject.name = base.name + " - Knob";
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = CalcKnobStartPos();
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.layer = base.gameObject.layer;
			knob = (UIScrollKnob)gameObject.AddComponent(typeof(UIScrollKnob));
			knob.plane = plane;
			knob.SetOffset(knobOffset);
			knob.persistent = persistent;
			knob.bleedCompensation = bleedCompensation;
			if (!managed)
			{
				if (knob.spriteMesh != null)
				{
					((SpriteMesh)knob.spriteMesh).material = base.renderer.sharedMaterial;
				}
			}
			else if (manager != null)
			{
				knob.Managed = managed;
				manager.AddSprite(knob);
				knob.SetDrawLayer(drawLayer + 1);
				knob.SetControlState(UIButton.CONTROL_STATE.ACTIVE);
				knob.SetControlState(UIButton.CONTROL_STATE.NORMAL);
			}
			else
			{
				Debug.LogError("Sprite on object \"" + base.name + "\" not assigned to a SpriteManager!");
			}
			if (pixelPerfect)
			{
				knob.pixelPerfect = true;
			}
			else
			{
				knob.SetSize(knobSize.x, knobSize.y);
			}
			knob.ignoreClipping = ignoreClipping;
			knob.color = color;
			knob.SetColliderSizeFactor(knobColliderSizeFactor);
			knob.SetSlider(this);
			knob.SetMaxScroll(width - stopKnobFromEdge * 2f);
			knob.SetInputDelegate(inputDelegate);
			knob.transitions[0] = transitions[2];
			knob.transitions[1] = transitions[3];
			knob.transitions[2] = transitions[4];
			knob.layers = knobLayers;
			for (int k = 0; k < knobLayers.Length; k++)
			{
				knobLayers[k].transform.parent = knob.transform;
			}
			knob.animations[0].SetAnim(states[2], 0);
			knob.animations[1].SetAnim(states[3], 1);
			knob.animations[2].SetAnim(states[4], 2);
			knob.SetupAppearance();
			knob.SetCamera(renderCamera);
			knob.Hide(IsHidden());
			knob.autoResize = autoResize;
			gameObject = new GameObject();
			gameObject.name = base.name + " - Empty Bar";
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.layer = base.gameObject.layer;
			emptySprite = (AutoSprite)gameObject.AddComponent(typeof(AutoSprite));
			emptySprite.plane = plane;
			emptySprite.autoResize = autoResize;
			emptySprite.pixelPerfect = pixelPerfect;
			emptySprite.persistent = persistent;
			emptySprite.ignoreClipping = ignoreClipping;
			emptySprite.bleedCompensation = bleedCompensation;
			if (!managed)
			{
				emptySprite.renderer.sharedMaterial = base.renderer.sharedMaterial;
			}
			else if (manager != null)
			{
				emptySprite.Managed = managed;
				manager.AddSprite(emptySprite);
				emptySprite.SetDrawLayer(drawLayer);
			}
			else
			{
				Debug.LogError("Sprite on object \"" + base.name + "\" not assigned to a SpriteManager!");
			}
			emptySprite.color = color;
			emptySprite.SetAnchor(anchor);
			emptySprite.Setup(width, height, m_spriteMesh.material);
			if (states[1].spriteFrames.Length != 0)
			{
				emptySprite.animations = new UVAnimation[1];
				emptySprite.animations[0] = new UVAnimation();
				emptySprite.animations[0].SetAnim(states[1], 0);
				emptySprite.PlayAnim(0, 0);
			}
			emptySprite.renderCamera = renderCamera;
			emptySprite.Hide(IsHidden());
			if (container != null)
			{
				container.AddChild(knob.gameObject);
				container.AddChild(emptySprite.gameObject);
			}
			SetState(0);
			m_value = -1f;
			Value = defaultValue;
		}
		if (managed && m_hidden)
		{
			Hide(true);
		}
	}

	public override void SetSize(float width, float height)
	{
		base.SetSize(width, height);
		if (!(knob == null))
		{
			knob.SetStartPos(CalcKnobStartPos());
			knob.SetMaxScroll(width - stopKnobFromEdge * 2f);
			knob.SetPosition(m_value);
			emptySprite.SetSize(width, height);
		}
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (!(s is UISlider))
		{
			return;
		}
		UISlider uISlider = (UISlider)s;
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			scriptWithMethodToInvoke = uISlider.scriptWithMethodToInvoke;
			methodToInvoke = uISlider.methodToInvoke;
		}
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			defaultValue = uISlider.defaultValue;
			stopKnobFromEdge = uISlider.stopKnobFromEdge;
			knobOffset = uISlider.knobOffset;
			knobSize = uISlider.knobSize;
			knobColliderSizeFactor = uISlider.knobColliderSizeFactor;
		}
		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance && Application.isPlaying)
		{
			if (emptySprite != null)
			{
				emptySprite.Copy(uISlider.emptySprite);
			}
			if (knob != null)
			{
				knob.Copy(uISlider.knob);
			}
			truncFloor = uISlider.truncFloor;
			truncRange = uISlider.truncRange;
		}
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			CalcKnobStartPos();
			Value = uISlider.Value;
		}
	}

	protected Vector3 CalcKnobStartPos()
	{
		Vector3 zero = Vector3.zero;
		switch (anchor)
		{
		case ANCHOR_METHOD.TEXTURE_OFFSET:
			zero.x = width * -0.5f + stopKnobFromEdge;
			break;
		case ANCHOR_METHOD.UPPER_LEFT:
			zero.x = stopKnobFromEdge;
			zero.y = height * -0.5f;
			break;
		case ANCHOR_METHOD.UPPER_CENTER:
			zero.x = width * -0.5f + stopKnobFromEdge;
			zero.y = height * -0.5f;
			break;
		case ANCHOR_METHOD.UPPER_RIGHT:
			zero.x = width * -1f + stopKnobFromEdge;
			zero.y = height * -0.5f;
			break;
		case ANCHOR_METHOD.MIDDLE_LEFT:
			zero.x = stopKnobFromEdge;
			break;
		case ANCHOR_METHOD.MIDDLE_CENTER:
			zero.x = width * -0.5f + stopKnobFromEdge;
			break;
		case ANCHOR_METHOD.MIDDLE_RIGHT:
			zero.x = width * -1f + stopKnobFromEdge;
			break;
		case ANCHOR_METHOD.BOTTOM_LEFT:
			zero.x = stopKnobFromEdge;
			zero.y = height * 0.5f;
			break;
		case ANCHOR_METHOD.BOTTOM_CENTER:
			zero.x = width * -0.5f + stopKnobFromEdge;
			zero.y = height * 0.5f;
			break;
		case ANCHOR_METHOD.BOTTOM_RIGHT:
			zero.x = width * -1f + stopKnobFromEdge;
			zero.y = height * 0.5f;
			break;
		}
		return zero;
	}

	public override void InitUVs()
	{
		if (states[0].spriteFrames.Length != 0)
		{
			frameInfo.Copy(states[0].spriteFrames[0]);
		}
		base.InitUVs();
	}

	protected void UpdateValue()
	{
		if (!(knob == null))
		{
			float truncVal = truncFloor + m_value * truncRange;
			UpdateAppearance(truncVal);
			knob.SetPosition(m_value);
			if (scriptWithMethodToInvoke != null)
			{
				scriptWithMethodToInvoke.Invoke(methodToInvoke, 0f);
			}
			if (changeDelegate != null)
			{
				changeDelegate(this);
			}
		}
	}

	public void ScrollKnobMoved(UIScrollKnob knob, float val)
	{
		m_value = val;
		float truncVal = truncFloor + m_value * truncRange;
		UpdateAppearance(truncVal);
		if (scriptWithMethodToInvoke != null)
		{
			scriptWithMethodToInvoke.Invoke(methodToInvoke, 0f);
		}
		if (changeDelegate != null)
		{
			changeDelegate(this);
		}
	}

	public override void SetInputDelegate(EZInputDelegate del)
	{
		if (knob != null)
		{
			knob.SetInputDelegate(del);
		}
		base.SetInputDelegate(del);
	}

	public override void AddInputDelegate(EZInputDelegate del)
	{
		if (knob != null)
		{
			knob.AddInputDelegate(del);
		}
		base.AddInputDelegate(del);
	}

	public override void RemoveInputDelegate(EZInputDelegate del)
	{
		if (knob != null)
		{
			knob.RemoveInputDelegate(del);
		}
		base.RemoveInputDelegate(del);
	}

	protected void UpdateAppearance(float truncVal)
	{
		TruncateRight(truncVal);
		if (emptySprite != null)
		{
			emptySprite.TruncateLeft(1f - truncVal);
		}
		for (int i = 0; i < filledLayers.Length; i++)
		{
			filledLayers[i].TruncateRight(truncVal);
		}
		for (int j = 0; j < emptyLayers.Length; j++)
		{
			emptyLayers[j].TruncateLeft(1f - truncVal);
		}
	}

	public UIScrollKnob GetKnob()
	{
		return knob;
	}

	public override void Unclip()
	{
		if (!ignoreClipping)
		{
			base.Unclip();
			emptySprite.Unclip();
			knob.Unclip();
		}
	}

	public static UISlider Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UISlider)gameObject.AddComponent(typeof(UISlider));
	}

	public static UISlider Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UISlider)gameObject.AddComponent(typeof(UISlider));
	}

	public override void Hide(bool tf)
	{
		base.Hide(tf);
		if (emptySprite != null)
		{
			emptySprite.Hide(tf);
		}
		if (knob != null)
		{
			knob.Hide(tf);
		}
	}

	public override void SetColor(Color c)
	{
		base.SetColor(c);
		if (emptySprite != null)
		{
			emptySprite.SetColor(c);
		}
		if (knob != null)
		{
			knob.SetColor(c);
		}
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvoke);
	}
}
