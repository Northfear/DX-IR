using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Progress Bar")]
public class UIProgressBar : AutoSpriteControlBase
{
	protected float m_value;

	protected AutoSprite emptySprite;

	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[2]
	{
		new TextureAnim("Filled"),
		new TextureAnim("Empty")
	};

	public SpriteRoot[] filledLayers = new SpriteRoot[0];

	public SpriteRoot[] emptyLayers = new SpriteRoot[0];

	protected int[] filledIndices;

	protected int[] emptyIndices;

	public float Value
	{
		get
		{
			return m_value;
		}
		set
		{
			m_value = Mathf.Clamp01(value);
			UpdateProgress();
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
			return null;
		}
		set
		{
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
				}
				if (value != null && emptySprite != null)
				{
					value.AddChild(emptySprite.gameObject);
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
			}
		}
	}

	public override EZTransitionList GetTransitions(int index)
	{
		return null;
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
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
			gameObject.name = base.name + " - Empty Bar";
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localRotation = Quaternion.identity;
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
			Value = m_value;
			if (container != null)
			{
				container.AddChild(gameObject);
			}
			SetState(0);
		}
		if (managed && m_hidden)
		{
			Hide(true);
		}
	}

	public override void SetSize(float width, float height)
	{
		base.SetSize(width, height);
		if (!(emptySprite == null))
		{
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
		if (s is UIProgressBar && Application.isPlaying)
		{
			UIProgressBar uIProgressBar = (UIProgressBar)s;
			if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance && emptySprite != null)
			{
				emptySprite.Copy(uIProgressBar.emptySprite);
			}
		}
	}

	public override void InitUVs()
	{
		if (states[0].spriteFrames.Length != 0)
		{
			frameInfo.Copy(states[0].spriteFrames[0]);
		}
		base.InitUVs();
	}

	protected void UpdateProgress()
	{
		TruncateRight(m_value);
		if (emptySprite != null)
		{
			emptySprite.TruncateLeft(1f - m_value);
		}
		for (int i = 0; i < filledLayers.Length; i++)
		{
			filledLayers[i].TruncateRight(m_value);
		}
		for (int j = 0; j < emptyLayers.Length; j++)
		{
			emptyLayers[j].TruncateLeft(1f - m_value);
		}
	}

	public override void Unclip()
	{
		if (!ignoreClipping)
		{
			base.Unclip();
			emptySprite.Unclip();
		}
	}

	public static UIProgressBar Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIProgressBar)gameObject.AddComponent(typeof(UIProgressBar));
	}

	public static UIProgressBar Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIProgressBar)gameObject.AddComponent(typeof(UIProgressBar));
	}

	public override void Hide(bool tf)
	{
		base.Hide(tf);
		if (emptySprite != null)
		{
			emptySprite.Hide(tf);
		}
	}

	public override void SetColor(Color c)
	{
		base.SetColor(c);
		if (emptySprite != null)
		{
			emptySprite.SetColor(c);
		}
	}
}
