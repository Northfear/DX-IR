using System;
using UnityEngine;

[Serializable]
public class AnimParams
{
	public EZAnimation.ANIM_MODE mode = EZAnimation.ANIM_MODE.To;

	public float delay;

	public float duration;

	public bool pingPong;

	public bool restartOnRepeat;

	public bool repeatDelay;

	public EZAnimation.EASING_TYPE easing;

	[NonSerialized]
	protected EZTransition m_transition;

	public Color color = Color.white;

	public Color color2 = Color.white;

	public Vector3 vec;

	public Vector3 vec2;

	public Vector3 axis;

	public float floatVal;

	public float floatVal2;

	public string strVal = string.Empty;

	public EZTransition transition
	{
		get
		{
			return m_transition;
		}
		set
		{
			m_transition = value;
		}
	}

	public AnimParams(EZTransition trans)
	{
		m_transition = trans;
	}

	public void Copy(AnimParams src)
	{
		mode = src.mode;
		delay = src.delay;
		duration = src.duration;
		easing = src.easing;
		color = src.color;
		vec = src.vec;
		axis = src.axis;
		floatVal = src.floatVal;
		color2 = src.color2;
		vec2 = src.vec2;
		floatVal2 = src.floatVal2;
		strVal = src.strVal;
		pingPong = src.pingPong;
		repeatDelay = src.repeatDelay;
		restartOnRepeat = src.restartOnRepeat;
	}

	public virtual void DrawGUI(EZAnimation.ANIM_TYPE type, GameObject go, IGUIHelper gui, bool inspector)
	{
		float pixels = 0f;
		float pixels2 = 0f;
		bool changed = GUI.changed;
		GUI.changed = false;
		delay = gui.FloatField("Delay:", delay);
		if (!inspector)
		{
			GUILayout.Space(pixels);
		}
		duration = gui.FloatField("Duration:", duration);
		if (duration < 0f)
		{
			repeatDelay = GUILayout.Toggle(repeatDelay, new GUIContent("Rep. Delay", "Repeats the delay on each loop iteration"));
			if (type != 0 || type != EZAnimation.ANIM_TYPE.Crash || type != EZAnimation.ANIM_TYPE.CrashRotation || type != EZAnimation.ANIM_TYPE.PunchPosition || type != EZAnimation.ANIM_TYPE.PunchRotation || type != EZAnimation.ANIM_TYPE.PunchScale || type != EZAnimation.ANIM_TYPE.Shake || type != EZAnimation.ANIM_TYPE.ShakeRotation || type != EZAnimation.ANIM_TYPE.SmoothCrash)
			{
				pingPong = GUILayout.Toggle(pingPong, new GUIContent("PingPong", "Ping-Pong: Causes the animated value to go back and forth as it loops."));
			}
		}
		if (!inspector)
		{
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Space(pixels2);
		}
		if (type == EZAnimation.ANIM_TYPE.FadeMaterial || type == EZAnimation.ANIM_TYPE.FadeSprite || type == EZAnimation.ANIM_TYPE.FadeSpriteAlpha || type == EZAnimation.ANIM_TYPE.FadeAudio || type == EZAnimation.ANIM_TYPE.TuneAudio || type == EZAnimation.ANIM_TYPE.FadeText || type == EZAnimation.ANIM_TYPE.FadeTextAlpha || type == EZAnimation.ANIM_TYPE.Rotate || type == EZAnimation.ANIM_TYPE.Scale || type == EZAnimation.ANIM_TYPE.Translate || type == EZAnimation.ANIM_TYPE.TranslateScreen || type == EZAnimation.ANIM_TYPE.RotateEuler)
		{
			easing = (EZAnimation.EASING_TYPE)(object)gui.EnumField("Easing:", easing);
		}
		if (!inspector)
		{
			GUILayout.Space(pixels);
		}
		if (type == EZAnimation.ANIM_TYPE.FadeMaterial || type == EZAnimation.ANIM_TYPE.FadeSprite || type == EZAnimation.ANIM_TYPE.FadeSpriteAlpha || type == EZAnimation.ANIM_TYPE.FadeAudio || type == EZAnimation.ANIM_TYPE.TuneAudio || type == EZAnimation.ANIM_TYPE.FadeText || type == EZAnimation.ANIM_TYPE.FadeTextAlpha || type == EZAnimation.ANIM_TYPE.Rotate || type == EZAnimation.ANIM_TYPE.Scale || type == EZAnimation.ANIM_TYPE.Translate || type == EZAnimation.ANIM_TYPE.TranslateScreen || type == EZAnimation.ANIM_TYPE.RotateEuler)
		{
			mode = (EZAnimation.ANIM_MODE)(object)gui.EnumField("Mode:", mode);
		}
		if (duration < 0f && (type == EZAnimation.ANIM_TYPE.FadeMaterial || type == EZAnimation.ANIM_TYPE.FadeSprite || type == EZAnimation.ANIM_TYPE.FadeSpriteAlpha || type == EZAnimation.ANIM_TYPE.FadeAudio || type == EZAnimation.ANIM_TYPE.TuneAudio || type == EZAnimation.ANIM_TYPE.FadeText || type == EZAnimation.ANIM_TYPE.FadeTextAlpha || type == EZAnimation.ANIM_TYPE.Rotate || type == EZAnimation.ANIM_TYPE.Scale || type == EZAnimation.ANIM_TYPE.Translate || type == EZAnimation.ANIM_TYPE.TranslateScreen || type == EZAnimation.ANIM_TYPE.RotateEuler))
		{
			restartOnRepeat = GUILayout.Toggle(restartOnRepeat, new GUIContent("Restart on Loop", "Resets the starting value on each loop iteration. Set this to false if you want something like continuous movement in the same direction without going back to the starting point."));
		}
		if (!inspector)
		{
			GUILayout.EndHorizontal();
		}
		switch (type)
		{
		case EZAnimation.ANIM_TYPE.FadeSprite:
		case EZAnimation.ANIM_TYPE.FadeMaterial:
		case EZAnimation.ANIM_TYPE.FadeText:
		case EZAnimation.ANIM_TYPE.FadeSpriteAlpha:
		case EZAnimation.ANIM_TYPE.FadeTextAlpha:
			if (mode == EZAnimation.ANIM_MODE.FromTo)
			{
				color = gui.ColorField("Start Color:", color);
				color2 = gui.ColorField("End Color:", color2);
			}
			else
			{
				color = gui.ColorField("Color:", color);
			}
			break;
		case EZAnimation.ANIM_TYPE.Rotate:
		case EZAnimation.ANIM_TYPE.RotateEuler:
			if (mode == EZAnimation.ANIM_MODE.FromTo)
			{
				vec = gui.Vector3Field("Start Angles:", vec);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
				{
					vec = go.transform.localEulerAngles;
					GUI.changed = true;
				}
				if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
				{
					go.transform.localEulerAngles = vec;
				}
				GUILayout.EndHorizontal();
				vec2 = gui.Vector3Field("End Angles:", vec2);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
				{
					vec2 = go.transform.localEulerAngles;
					GUI.changed = true;
				}
				if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
				{
					go.transform.localEulerAngles = vec2;
				}
				GUILayout.EndHorizontal();
				break;
			}
			vec = gui.Vector3Field("Angles:", vec);
			if (mode == EZAnimation.ANIM_MODE.To)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
				{
					vec = go.transform.localEulerAngles;
					GUI.changed = true;
				}
				if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
				{
					go.transform.localEulerAngles = vec;
				}
				GUILayout.EndHorizontal();
			}
			break;
		case EZAnimation.ANIM_TYPE.Scale:
			if (mode == EZAnimation.ANIM_MODE.FromTo)
			{
				vec = gui.Vector3Field("Start Scale:", vec);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
				{
					vec = go.transform.localScale;
					GUI.changed = true;
				}
				if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
				{
					go.transform.localScale = vec;
				}
				GUILayout.EndHorizontal();
				vec2 = gui.Vector3Field("End Scale:", vec2);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
				{
					vec2 = go.transform.localScale;
					GUI.changed = true;
				}
				if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
				{
					go.transform.localScale = vec2;
				}
				GUILayout.EndHorizontal();
				break;
			}
			vec = gui.Vector3Field("Scale:", vec);
			if (mode == EZAnimation.ANIM_MODE.To)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
				{
					vec = go.transform.localScale;
					GUI.changed = true;
				}
				if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
				{
					go.transform.localScale = vec;
				}
				GUILayout.EndHorizontal();
			}
			break;
		case EZAnimation.ANIM_TYPE.Translate:
			if (mode == EZAnimation.ANIM_MODE.FromTo)
			{
				vec = gui.Vector3Field("Start Pos:", vec);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
				{
					vec = go.transform.localPosition;
					GUI.changed = true;
				}
				if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
				{
					go.transform.localPosition = vec;
				}
				GUILayout.EndHorizontal();
				vec2 = gui.Vector3Field("End Pos:", vec2);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
				{
					vec2 = go.transform.localPosition;
					GUI.changed = true;
				}
				if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
				{
					go.transform.localPosition = vec2;
				}
				GUILayout.EndHorizontal();
			}
			else if (mode == EZAnimation.ANIM_MODE.By)
			{
				vec = gui.Vector3Field("Vector:", vec);
			}
			else
			{
				vec = gui.Vector3Field("Pos:", vec);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
				{
					vec = go.transform.localPosition;
					GUI.changed = true;
				}
				if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
				{
					go.transform.localPosition = vec;
				}
				GUILayout.EndHorizontal();
			}
			break;
		case EZAnimation.ANIM_TYPE.TranslateScreen:
		{
			EZScreenPlacement eZScreenPlacement = (EZScreenPlacement)go.GetComponent(typeof(EZScreenPlacement));
			if (eZScreenPlacement == null)
			{
				Debug.LogError("ERROR: A transition element of type TranslateScreen has been selected, but the object \"" + go.name + "\" does not have an EZScreenPlacement component attached.");
			}
			else if (mode == EZAnimation.ANIM_MODE.FromTo)
			{
				vec = gui.Vector3Field("Start Pos:", vec);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
				{
					vec = eZScreenPlacement.screenPos;
					GUI.changed = true;
				}
				if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
				{
					eZScreenPlacement.screenPos = vec;
					eZScreenPlacement.PositionOnScreen();
				}
				GUILayout.EndHorizontal();
				vec2 = gui.Vector3Field("End Pos:", vec2);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
				{
					vec2 = eZScreenPlacement.screenPos;
					GUI.changed = true;
				}
				if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
				{
					eZScreenPlacement.screenPos = vec2;
					eZScreenPlacement.PositionOnScreen();
				}
				GUILayout.EndHorizontal();
			}
			else if (mode == EZAnimation.ANIM_MODE.By)
			{
				vec = gui.Vector3Field("Vector:", vec);
			}
			else
			{
				vec = gui.Vector3Field("Pos:", vec);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Use Current", "Uses the object's current value for this field")))
				{
					vec = eZScreenPlacement.screenPos;
					GUI.changed = true;
				}
				if (GUILayout.Button(new GUIContent("Set as Current", "Applies the displayed values to the current object.")))
				{
					eZScreenPlacement.screenPos = vec;
					eZScreenPlacement.PositionOnScreen();
				}
				GUILayout.EndHorizontal();
			}
			break;
		}
		case EZAnimation.ANIM_TYPE.PunchPosition:
		case EZAnimation.ANIM_TYPE.Crash:
		case EZAnimation.ANIM_TYPE.PunchScale:
		case EZAnimation.ANIM_TYPE.PunchRotation:
			vec = gui.Vector3Field("Magnitude:", vec);
			break;
		case EZAnimation.ANIM_TYPE.Shake:
		case EZAnimation.ANIM_TYPE.ShakeRotation:
			vec = gui.Vector3Field("Magnitude:", vec);
			floatVal = gui.FloatField("Oscillations:", floatVal);
			break;
		case EZAnimation.ANIM_TYPE.SmoothCrash:
		case EZAnimation.ANIM_TYPE.CrashRotation:
			vec = gui.Vector3Field("Magnitude:", vec);
			vec2 = gui.Vector3Field("Oscillations:", vec2);
			break;
		case EZAnimation.ANIM_TYPE.AnimClip:
			strVal = gui.TextField("Anim Clip:", strVal);
			if (!inspector)
			{
				GUILayout.BeginHorizontal();
				floatVal = Mathf.Clamp01(gui.FloatField("Blend Weight:", floatVal));
				GUILayout.Space(15f);
				floatVal = GUILayout.HorizontalSlider(floatVal, 0f, 1f, GUILayout.Width(200f));
				GUILayout.EndHorizontal();
			}
			else
			{
				floatVal = Mathf.Clamp01(gui.FloatField("Blend Weight:", floatVal));
			}
			break;
		case EZAnimation.ANIM_TYPE.FadeAudio:
			if (mode == EZAnimation.ANIM_MODE.FromTo)
			{
				floatVal = gui.FloatField("Start Volume:", floatVal);
				floatVal2 = gui.FloatField("End Volume:", floatVal2);
			}
			else
			{
				floatVal = gui.FloatField("Volume:", floatVal);
			}
			break;
		case EZAnimation.ANIM_TYPE.TuneAudio:
			if (mode == EZAnimation.ANIM_MODE.FromTo)
			{
				floatVal = gui.FloatField("Start pitch:", floatVal);
				floatVal2 = gui.FloatField("End pitch:", floatVal2);
			}
			else
			{
				floatVal = gui.FloatField("Pitch:", floatVal);
			}
			break;
		}
		if (GUI.changed)
		{
			m_transition.initialized = true;
		}
		GUI.changed = changed || GUI.changed;
	}
}
