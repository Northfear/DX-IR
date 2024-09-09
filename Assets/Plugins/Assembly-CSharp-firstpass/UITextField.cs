using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Text Field")]
public class UITextField : AutoSpriteControlBase, IKeyFocusable
{
	public delegate void FocusDelegate(UITextField field);

	public delegate string ValidationDelegate(UITextField field, string text, ref int insertionPoint);

	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[2]
	{
		new TextureAnim("Field graphic"),
		new TextureAnim("Caret")
	};

	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[2]
	{
		null,
		new EZTransitionList(new EZTransition[1]
		{
			new EZTransition("Caret Flash")
		})
	};

	public Vector2 margins;

	protected Rect3D clientClippingRect;

	protected Vector2 marginTopLeft;

	protected Vector2 marginBottomRight;

	public int maxLength;

	public bool multiline;

	public bool password;

	public string maskingCharacter = "*";

	public Vector2 caretSize;

	public ANCHOR_METHOD caretAnchor = ANCHOR_METHOD.BOTTOM_LEFT;

	public Vector3 caretOffset = new Vector3(0f, 0f, -0.1f);

	public bool showCaretOnMobile;

	public bool allowClickCaretPlacement = true;

	protected bool maxLengthExceeded;

	public TouchScreenKeyboardType type;

	public bool autoCorrect;

	public bool alert;

	public bool hideInput;

	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvoke = string.Empty;

	protected EZKeyboardCommitDelegate commitDelegate;

	protected ValidationDelegate validationDelegate;

	public AudioSource typingSoundEffect;

	public AudioSource fieldFullSound;

	public bool customKeyboard;

	public bool commitOnLostFocus;

	public POINTER_INFO.INPUT_EVENT customFocusEvent = POINTER_INFO.INPUT_EVENT.PRESS;

	protected AutoSprite caret;

	protected FocusDelegate focusDelegate;

	protected int insert;

	protected Vector3 cachedPos;

	protected Quaternion cachedRot;

	protected Vector3 cachedScale;

	protected bool hasFocus;

	protected Vector3 origTextPos;

	protected int[,] stateIndices;

	public override bool controlIsEnabled
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

	public string Content
	{
		get
		{
			return Text;
		}
	}

	public bool MaxLengthExceeded
	{
		get
		{
			return maxLengthExceeded;
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
				if (container != null && caret != null)
				{
					container.RemoveChild(caret.gameObject);
				}
				if (value != null && caret != null)
				{
					value.AddChild(caret.gameObject);
				}
			}
			base.Container = value;
		}
	}

	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			bool flag = spriteText == null;
			if (Application.isPlaying && !m_started)
			{
				Start();
			}
			bool flag2 = text == null || insert == text.Length;
			base.Text = value;
			if (maxLength > 0)
			{
				if (value.Length > maxLength)
				{
					maxLengthExceeded = true;
				}
				else
				{
					maxLengthExceeded = false;
				}
			}
			if (flag && spriteText != null)
			{
				spriteText.transform.localPosition = new Vector4(width * -0.5f + margins.x, height * 0.5f + margins.y);
				spriteText.removeUnsupportedCharacters = true;
				spriteText.parseColorTags = false;
				spriteText.multiline = multiline;
			}
			if (cachedPos != base.transform.position || cachedRot != base.transform.rotation || cachedScale != base.transform.lossyScale)
			{
				cachedPos = base.transform.position;
				cachedRot = base.transform.rotation;
				cachedScale = base.transform.lossyScale;
				CalcClippingRect();
			}
			if (flag2)
			{
				insert = Text.Length;
			}
			PositionCaret();
			if (changeDelegate != null)
			{
				changeDelegate(this);
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
				CalcClippingRect();
			}
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
				CalcClippingRect();
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

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
		{
			return;
		}
		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}
		if (inputDelegate != null)
		{
			inputDelegate(ref ptr);
		}
		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}
		if (ptr.evt == customFocusEvent && focusDelegate != null)
		{
			focusDelegate(this);
		}
		base.OnInput(ref ptr);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (s is UITextField)
		{
			UITextField uITextField = (UITextField)s;
			if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
			{
				maxLength = uITextField.maxLength;
				multiline = uITextField.multiline;
				password = uITextField.password;
				maskingCharacter = uITextField.maskingCharacter;
				customKeyboard = uITextField.customKeyboard;
				customFocusEvent = uITextField.customFocusEvent;
				margins = uITextField.margins;
				type = uITextField.type;
				autoCorrect = uITextField.autoCorrect;
				alert = uITextField.alert;
				hideInput = uITextField.hideInput;
				typingSoundEffect = uITextField.typingSoundEffect;
				fieldFullSound = uITextField.fieldFullSound;
			}
			if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
			{
				scriptWithMethodToInvoke = uITextField.scriptWithMethodToInvoke;
				methodToInvoke = uITextField.methodToInvoke;
			}
			if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
			{
				caret.Copy(uITextField.caret);
				caretSize = uITextField.caretSize;
				caretOffset = uITextField.caretOffset;
				caretAnchor = uITextField.caretAnchor;
				showCaretOnMobile = uITextField.showCaretOnMobile;
			}
			if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
			{
				insert = uITextField.insert;
				Text = uITextField.Text;
			}
			SetMargins(margins);
		}
	}

	public override bool GotFocus()
	{
		if (customKeyboard)
		{
			return false;
		}
		hasFocus = m_controlIsEnabled;
		return m_controlIsEnabled;
	}

	public string GetInputText(ref KEYBOARD_INFO info)
	{
		info.insert = insert;
		info.type = type;
		info.autoCorrect = autoCorrect;
		info.multiline = multiline;
		info.secure = password;
		info.alert = alert;
		info.hideInput = hideInput;
		ShowCaret();
		return text;
	}

	public string SetInputText(string inputText, ref int insertPt)
	{
		if (!multiline)
		{
			int startIndex;
			if ((startIndex = inputText.IndexOf('\n')) != -1)
			{
				inputText = inputText.Remove(startIndex, 1);
				UIManager.instance.FocusObject = null;
			}
			if ((startIndex = inputText.IndexOf('\r')) != -1)
			{
				inputText = inputText.Remove(startIndex, 1);
				UIManager.instance.FocusObject = null;
			}
		}
		if (validationDelegate != null)
		{
			inputText = validationDelegate(this, inputText, ref insertPt);
		}
		if (inputText.Length > maxLength && maxLength > 0)
		{
			EZValueChangedDelegate eZValueChangedDelegate = changeDelegate;
			changeDelegate = null;
			Text = inputText.Substring(0, maxLength);
			insert = Mathf.Clamp(insertPt, 0, maxLength);
			maxLengthExceeded = true;
			changeDelegate = eZValueChangedDelegate;
			if (changeDelegate != null)
			{
				changeDelegate(this);
			}
			if (fieldFullSound != null)
			{
				fieldFullSound.PlayOneShot(fieldFullSound.clip);
			}
		}
		else
		{
			Text = inputText;
			insert = insertPt;
			if (typingSoundEffect != null)
			{
				typingSoundEffect.PlayOneShot(typingSoundEffect.clip);
			}
			if (changeDelegate != null)
			{
				changeDelegate(this);
			}
		}
		if (caret != null && caret.IsHidden() && hasFocus)
		{
			caret.Hide(false);
		}
		PositionCaret();
		if (UIManager.instance.FocusObject == null && !commitOnLostFocus)
		{
			Commit();
		}
		return text;
	}

	public void LostFocus()
	{
		if (commitOnLostFocus)
		{
			Commit();
		}
		hasFocus = false;
		HideCaret();
	}

	public void Commit()
	{
		if (scriptWithMethodToInvoke != null && !string.IsNullOrEmpty(methodToInvoke))
		{
			scriptWithMethodToInvoke.Invoke(methodToInvoke, 0f);
		}
		if (commitDelegate != null)
		{
			commitDelegate(this);
		}
	}

	protected void ShowCaret()
	{
		if (caret == null)
		{
			return;
		}
		CalcClippingRect();
		caret.Hide(false);
		PositionCaret();
		if (!caret.IsHidden())
		{
			transitions[1].list[0].Start();
			if (caret.animations.Length > 0)
			{
				caret.DoAnim(0);
			}
		}
	}

	public override void Hide(bool tf)
	{
		base.Hide(tf);
		if (caret != null)
		{
			if (!tf && hasFocus)
			{
				caret.Hide(tf);
			}
			else
			{
				caret.Hide(true);
			}
		}
		if (!tf)
		{
			CalcClippingRect();
		}
	}

	protected void HideCaret()
	{
		if (!(caret == null))
		{
			transitions[1].list[0].StopSafe();
			caret.Hide(true);
		}
	}

	protected void PositionText(bool recur)
	{
		Vector3 vector = base.transform.InverseTransformPoint(spriteText.GetInsertionPointPos(spriteText.PlainIndexToDisplayIndex(insert)));
		Vector3 vector2 = vector + Vector3.up * spriteText.BaseHeight * spriteText.transform.localScale.y;
		if (!recur)
		{
			return;
		}
		if (multiline)
		{
			if (vector2.y > marginTopLeft.y)
			{
				spriteText.transform.localPosition -= Vector3.up * spriteText.LineSpan;
				PositionText(false);
				spriteText.ClippingRect = clientClippingRect;
			}
			else if (vector.y < marginBottomRight.y)
			{
				spriteText.transform.localPosition += Vector3.up * spriteText.LineSpan;
				PositionText(false);
				spriteText.ClippingRect = clientClippingRect;
			}
		}
		else if (vector.x < marginTopLeft.x)
		{
			Vector3 centerPoint = GetCenterPoint();
			Vector3 localPosition = spriteText.transform.localPosition + Vector3.right * Mathf.Abs(centerPoint.x - vector.x);
			localPosition.x = Mathf.Min(localPosition.x, origTextPos.x);
			spriteText.transform.localPosition = localPosition;
			PositionText(false);
			spriteText.ClippingRect = clientClippingRect;
		}
		else if (vector.x > marginBottomRight.x)
		{
			Vector3 centerPoint2 = GetCenterPoint();
			Vector3 localPosition2 = spriteText.transform.localPosition - Vector3.right * Mathf.Abs(centerPoint2.x - vector.x);
			spriteText.transform.localPosition = localPosition2;
			PositionText(false);
			spriteText.ClippingRect = clientClippingRect;
		}
	}

	protected void PositionCaret()
	{
		PositionCaret(true);
	}

	protected void PositionCaret(bool recur)
	{
		if (spriteText == null)
		{
			return;
		}
		if (caret == null)
		{
			PositionText(true);
			return;
		}
		Vector3 vector = base.transform.InverseTransformPoint(spriteText.GetInsertionPointPos(spriteText.PlainIndexToDisplayIndex(insert)));
		Vector3 vector2 = vector + Vector3.up * spriteText.BaseHeight * spriteText.transform.localScale.y;
		if (recur)
		{
			if (multiline)
			{
				if (vector2.y > marginTopLeft.y)
				{
					spriteText.transform.localPosition -= Vector3.up * spriteText.LineSpan;
					PositionCaret(false);
					spriteText.ClippingRect = clientClippingRect;
					return;
				}
				if (vector.y < marginBottomRight.y)
				{
					spriteText.transform.localPosition += Vector3.up * spriteText.LineSpan;
					PositionCaret(false);
					spriteText.ClippingRect = clientClippingRect;
					return;
				}
			}
			else
			{
				if (vector.x < marginTopLeft.x)
				{
					Vector3 centerPoint = GetCenterPoint();
					Vector3 localPosition = spriteText.transform.localPosition + Vector3.right * Mathf.Abs(centerPoint.x - vector.x);
					localPosition.x = Mathf.Min(localPosition.x, origTextPos.x);
					spriteText.transform.localPosition = localPosition;
					PositionCaret(false);
					spriteText.ClippingRect = clientClippingRect;
					return;
				}
				if (vector.x > marginBottomRight.x)
				{
					Vector3 centerPoint2 = GetCenterPoint();
					Vector3 localPosition2 = spriteText.transform.localPosition - Vector3.right * Mathf.Abs(centerPoint2.x - vector.x);
					spriteText.transform.localPosition = localPosition2;
					PositionCaret(false);
					spriteText.ClippingRect = clientClippingRect;
					return;
				}
			}
		}
		transitions[1].list[0].StopSafe();
		caret.transform.localPosition = vector;
		transitions[1].list[0].Start();
		if (caret.animations.Length > 0)
		{
			caret.DoAnim(0);
		}
		caret.ClippingRect = clientClippingRect;
	}

	protected void PositionInsertionPoint(Vector3 pt)
	{
		if (!(caret == null) && !(spriteText == null))
		{
			insert = spriteText.DisplayIndexToPlainIndex(spriteText.GetNearestInsertionPoint(pt));
			UIManager.instance.InsertionPoint = insert;
			PositionCaret(true);
		}
	}

	public void GoUp()
	{
		Vector3 insertionPointPos = spriteText.GetInsertionPointPos(spriteText.PlainIndexToDisplayIndex(insert));
		insertionPointPos += spriteText.transform.up * spriteText.LineSpan * spriteText.transform.lossyScale.y;
		insert = spriteText.DisplayIndexToPlainIndex(spriteText.GetNearestInsertionPoint(insertionPointPos));
		UIManager.instance.InsertionPoint = insert;
		PositionCaret(true);
	}

	public void GoDown()
	{
		Vector3 insertionPointPos = spriteText.GetInsertionPointPos(spriteText.PlainIndexToDisplayIndex(insert));
		insertionPointPos -= spriteText.transform.up * spriteText.LineSpan * spriteText.transform.lossyScale.y;
		insert = spriteText.DisplayIndexToPlainIndex(spriteText.GetNearestInsertionPoint(insertionPointPos));
		UIManager.instance.InsertionPoint = insert;
		PositionCaret(true);
	}

	public void SetCommitDelegate(EZKeyboardCommitDelegate del)
	{
		commitDelegate = del;
	}

	public void AddCommitDelegate(EZKeyboardCommitDelegate del)
	{
		commitDelegate = (EZKeyboardCommitDelegate)Delegate.Combine(commitDelegate, del);
	}

	public void RemoveCommitDelegate(EZKeyboardCommitDelegate del)
	{
		commitDelegate = (EZKeyboardCommitDelegate)Delegate.Remove(commitDelegate, del);
	}

	public void SetFocusDelegate(FocusDelegate del)
	{
		focusDelegate = del;
	}

	public void AddFocusDelegate(FocusDelegate del)
	{
		focusDelegate = (FocusDelegate)Delegate.Combine(focusDelegate, del);
	}

	public void RemoveFocusDelegate(FocusDelegate del)
	{
		focusDelegate = (FocusDelegate)Delegate.Remove(focusDelegate, del);
	}

	public void SetValidationDelegate(ValidationDelegate del)
	{
		validationDelegate = del;
	}

	public void AddValidationDelegate(ValidationDelegate del)
	{
		validationDelegate = (ValidationDelegate)Delegate.Combine(validationDelegate, del);
	}

	public void RemoveValidationDelegate(ValidationDelegate del)
	{
		validationDelegate = (ValidationDelegate)Delegate.Remove(validationDelegate, del);
	}

	protected override void Awake()
	{
		base.Awake();
		defaultTextAlignment = SpriteText.Alignment_Type.Left;
		defaultTextAnchor = SpriteText.Anchor_Pos.Upper_Left;
	}

	public override void Start()
	{
		if (m_started)
		{
			return;
		}
		base.Start();
		if (spriteText == null)
		{
			Text = " ";
			Text = string.Empty;
		}
		if (spriteText != null)
		{
			spriteText.password = password;
			spriteText.maskingCharacter = maskingCharacter;
			spriteText.multiline = multiline;
			origTextPos = spriteText.transform.localPosition;
			SetMargins(margins);
		}
		insert = Text.Length;
		if (Application.isPlaying)
		{
			if (base.collider == null)
			{
				AddCollider();
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				if (showCaretOnMobile)
				{
					CreateCaret();
				}
			}
			else
			{
				CreateCaret();
			}
		}
		cachedPos = base.transform.position;
		cachedRot = base.transform.rotation;
		cachedScale = base.transform.lossyScale;
		CalcClippingRect();
		if (managed && m_hidden)
		{
			Hide(true);
		}
	}

	protected void CreateCaret()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = base.name + " - caret";
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.layer = base.gameObject.layer;
		caret = (AutoSprite)gameObject.AddComponent(typeof(AutoSprite));
		caret.plane = plane;
		caret.offset = caretOffset;
		caret.SetAnchor(caretAnchor);
		caret.persistent = persistent;
		if (!managed)
		{
			if (caret.spriteMesh != null)
			{
				((SpriteMesh)caret.spriteMesh).material = base.renderer.sharedMaterial;
			}
		}
		else if (manager != null)
		{
			caret.Managed = managed;
			manager.AddSprite(caret);
			caret.SetDrawLayer(drawLayer + 1);
		}
		else
		{
			Debug.LogError("Sprite on object \"" + base.name + "\" not assigned to a SpriteManager!");
		}
		caret.autoResize = autoResize;
		if (pixelPerfect)
		{
			caret.pixelPerfect = pixelPerfect;
		}
		else
		{
			caret.SetSize(caretSize.x, caretSize.y);
		}
		if (states[1].spriteFrames.Length != 0)
		{
			caret.animations = new UVAnimation[1];
			caret.animations[0] = new UVAnimation();
			caret.animations[0].SetAnim(states[1], 0);
			caret.PlayAnim(0, 0);
		}
		caret.renderCamera = renderCamera;
		caret.SetCamera(renderCamera);
		caret.Hide(true);
		transitions[1].list[0].MainSubject = caret.gameObject;
		PositionCaret();
		if (container != null)
		{
			container.AddSubject(caret.gameObject);
		}
		if (autoResize)
		{
			caret.Start();
			caret.SetSize(caretSize.x, caretSize.y);
		}
	}

	public void CalcClippingRect()
	{
		if (!(spriteText == null))
		{
			Vector3 vector = marginTopLeft;
			Vector3 vector2 = marginBottomRight;
			if (clipped)
			{
				Vector3 vector3 = vector;
				Vector3 vector4 = vector2;
				vector.x = Mathf.Clamp(localClipRect.x, vector3.x, vector4.x);
				vector2.x = Mathf.Clamp(localClipRect.xMax, vector3.x, vector4.x);
				vector.y = Mathf.Clamp(localClipRect.yMax, vector4.y, vector3.y);
				vector2.y = Mathf.Clamp(localClipRect.y, vector4.y, vector3.y);
			}
			clientClippingRect.FromRect(Rect.MinMaxRect(vector.x, vector2.y, vector2.x, vector.y));
			clientClippingRect.MultFast(base.transform.localToWorldMatrix);
			spriteText.ClippingRect = clientClippingRect;
			if (caret != null)
			{
				caret.ClippingRect = clientClippingRect;
			}
		}
	}

	public void OnEZTranslated()
	{
		CalcClippingRect();
	}

	public void OnEZRotated()
	{
		CalcClippingRect();
	}

	public void OnEZScaled()
	{
		CalcClippingRect();
	}

	public void SetMargins(Vector2 marg)
	{
		margins = marg;
		Vector3 centerPoint = GetCenterPoint();
		marginTopLeft = new Vector3(centerPoint.x + margins.x - width * 0.5f, centerPoint.y - margins.y + height * 0.5f, 0f);
		marginBottomRight = new Vector3(centerPoint.x - margins.x + width * 0.5f, centerPoint.y + margins.y - height * 0.5f, 0f);
		if (multiline)
		{
			float num = 0f;
			switch (spriteText.anchor)
			{
			case SpriteText.Anchor_Pos.Upper_Left:
			case SpriteText.Anchor_Pos.Middle_Left:
			case SpriteText.Anchor_Pos.Lower_Left:
				num = marginBottomRight.x - origTextPos.x;
				break;
			case SpriteText.Anchor_Pos.Upper_Center:
			case SpriteText.Anchor_Pos.Middle_Center:
			case SpriteText.Anchor_Pos.Lower_Center:
				num = (marginBottomRight.x - marginTopLeft.x) * 2f - 2f * Mathf.Abs(origTextPos.x);
				break;
			case SpriteText.Anchor_Pos.Upper_Right:
			case SpriteText.Anchor_Pos.Middle_Right:
			case SpriteText.Anchor_Pos.Lower_Right:
				num = origTextPos.x - marginTopLeft.x;
				break;
			}
			spriteText.maxWidth = 1f / spriteText.transform.localScale.x * num;
		}
		else if (spriteText != null)
		{
			spriteText.maxWidth = 0f;
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

	public static UITextField Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UITextField)gameObject.AddComponent(typeof(UITextField));
	}

	public static UITextField Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UITextField)gameObject.AddComponent(typeof(UITextField));
	}

	public override void Unclip()
	{
		if (!ignoreClipping)
		{
			base.Unclip();
			CalcClippingRect();
		}
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvoke);
	}

	public override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = new Color(1f, 0f, 0.5f, 1f);
		Gizmos.DrawLine(clientClippingRect.topLeft, clientClippingRect.bottomLeft);
		Gizmos.DrawLine(clientClippingRect.bottomLeft, clientClippingRect.bottomRight);
		Gizmos.DrawLine(clientClippingRect.bottomRight, clientClippingRect.topRight);
		Gizmos.DrawLine(clientClippingRect.topRight, clientClippingRect.topLeft);
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
				mirror = new UITextFieldMirror();
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
}
