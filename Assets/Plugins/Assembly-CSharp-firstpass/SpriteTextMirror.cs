using UnityEngine;

public class SpriteTextMirror
{
	public string text;

	public TextAsset font;

	public float offsetZ;

	public float characterSize;

	public float characterSpacing;

	public float lineSpacing;

	public SpriteText.Anchor_Pos anchor;

	public SpriteText.Alignment_Type alignment;

	public int tabSize;

	public Color color;

	public float maxWidth;

	public bool maxWidthInPixels;

	public bool pixelPerfect;

	public Camera renderCamera;

	public bool hideAtStart;

	public virtual void Mirror(SpriteText s)
	{
		text = s.text;
		font = s.font;
		offsetZ = s.offsetZ;
		characterSize = s.characterSize;
		characterSpacing = s.characterSpacing;
		lineSpacing = s.lineSpacing;
		anchor = s.anchor;
		alignment = s.alignment;
		tabSize = s.tabSize;
		color = s.color;
		maxWidth = s.maxWidth;
		maxWidthInPixels = s.maxWidthInPixels;
		pixelPerfect = s.pixelPerfect;
		renderCamera = s.renderCamera;
		hideAtStart = s.hideAtStart;
	}

	public virtual bool Validate(SpriteText s)
	{
		return true;
	}

	public virtual bool DidChange(SpriteText s)
	{
		if (s.text != text)
		{
			return true;
		}
		if (s.font != font)
		{
			return true;
		}
		if (s.offsetZ != offsetZ)
		{
			return true;
		}
		if (s.characterSize != characterSize)
		{
			return true;
		}
		if (s.characterSpacing != characterSpacing)
		{
			return true;
		}
		if (s.lineSpacing != lineSpacing)
		{
			return true;
		}
		if (s.anchor != anchor)
		{
			return true;
		}
		if (s.alignment != alignment)
		{
			return true;
		}
		if (s.tabSize != tabSize)
		{
			return true;
		}
		if (s.color.r != color.r || s.color.g != color.g || s.color.b != color.b || s.color.a != color.a)
		{
			return true;
		}
		if (maxWidth != s.maxWidth)
		{
			return true;
		}
		if (maxWidthInPixels != s.maxWidthInPixels)
		{
			return true;
		}
		if (s.pixelPerfect != pixelPerfect)
		{
			s.SetCamera(s.renderCamera);
			return true;
		}
		if (s.renderCamera != renderCamera)
		{
			s.SetCamera(s.renderCamera);
			return true;
		}
		if (s.hideAtStart != hideAtStart)
		{
			s.Hide(s.hideAtStart);
			return true;
		}
		return false;
	}
}
