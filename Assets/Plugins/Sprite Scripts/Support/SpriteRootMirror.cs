using UnityEngine;

public class SpriteRootMirror
{
	public bool managed;

	public SpriteManager manager;

	public int drawLayer;

	public SpriteRoot.SPRITE_PLANE plane;

	public SpriteRoot.WINDING_ORDER winding;

	public float width;

	public float height;

	public Vector2 bleedCompensation;

	public SpriteRoot.ANCHOR_METHOD anchor;

	public Vector3 offset;

	public Color color;

	public bool pixelPerfect;

	public bool autoResize;

	public Camera renderCamera;

	public bool hideAtStart;

	public virtual void Mirror(SpriteRoot s)
	{
		managed = s.managed;
		manager = s.manager;
		drawLayer = s.drawLayer;
		plane = s.plane;
		winding = s.winding;
		width = s.width;
		height = s.height;
		bleedCompensation = s.bleedCompensation;
		anchor = s.anchor;
		offset = s.offset;
		color = s.color;
		pixelPerfect = s.pixelPerfect;
		autoResize = s.autoResize;
		renderCamera = s.renderCamera;
		hideAtStart = s.hideAtStart;
	}

	public virtual bool Validate(SpriteRoot s)
	{
		if (s.pixelPerfect)
		{
			s.autoResize = true;
		}
		return true;
	}

	public virtual bool DidChange(SpriteRoot s)
	{
		if (s.managed != managed)
		{
			HandleManageState(s);
			return true;
		}
		if (s.manager != manager)
		{
			UpdateManager(s);
			return true;
		}
		if (s.drawLayer != drawLayer)
		{
			HandleDrawLayerChange(s);
			return true;
		}
		if (s.plane != plane)
		{
			return true;
		}
		if (s.winding != winding)
		{
			return true;
		}
		if (s.width != width)
		{
			return true;
		}
		if (s.height != height)
		{
			return true;
		}
		if (s.bleedCompensation != bleedCompensation)
		{
			return true;
		}
		if (s.anchor != anchor)
		{
			return true;
		}
		if (s.offset != offset)
		{
			return true;
		}
		if (s.color.r != color.r || s.color.g != color.g || s.color.b != color.b || s.color.a != color.a)
		{
			return true;
		}
		if (s.pixelPerfect != pixelPerfect)
		{
			return true;
		}
		if (s.autoResize != autoResize)
		{
			return true;
		}
		if (s.renderCamera != renderCamera)
		{
			return true;
		}
		if (s.hideAtStart != hideAtStart)
		{
			s.Hide(s.hideAtStart);
			return true;
		}
		return false;
	}

	protected virtual void HandleManageState(SpriteRoot s)
	{
		s.managed = managed;
		s.Managed = !managed;
	}

	public virtual void UpdateManager(SpriteRoot s)
	{
		if (!s.managed)
		{
			s.manager = null;
			return;
		}
		if (manager != null)
		{
			manager.RemoveSprite(s);
		}
		if (s.manager != null)
		{
			s.manager.AddSprite(s);
		}
	}

	protected virtual void HandleDrawLayerChange(SpriteRoot s)
	{
		if (!s.managed)
		{
			s.drawLayer = 0;
		}
		else
		{
			s.SetDrawLayer(s.drawLayer);
		}
	}
}
