using UnityEngine;

public class UITextFieldMirror : AutoSpriteControlBaseMirror
{
	public Vector2 margins;

	public bool multiline;

	public override void Mirror(SpriteRoot s)
	{
		base.Mirror(s);
		UITextField uITextField = (UITextField)s;
		margins = uITextField.margins;
		multiline = uITextField.multiline;
	}

	public override bool Validate(SpriteRoot s)
	{
		return base.Validate(s);
	}

	public override bool DidChange(SpriteRoot s)
	{
		UITextField uITextField = (UITextField)s;
		if (margins.x != uITextField.margins.x || margins.y != uITextField.margins.y || width != uITextField.width || height != uITextField.height)
		{
			uITextField.SetMargins(uITextField.margins);
			uITextField.CalcClippingRect();
			margins = uITextField.margins;
		}
		if (multiline != uITextField.multiline)
		{
			if (uITextField.spriteText != null)
			{
				uITextField.spriteText.multiline = uITextField.multiline;
			}
			return true;
		}
		return base.DidChange(s);
	}
}
