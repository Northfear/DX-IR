using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
	public enum KeyName
	{
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3,
		WalkToggle = 4,
		Walk = 5,
		CoverMove = 6,
		CrouchToggle = 7,
		Crouch = 8,
		Interact = 9,
		Fire = 10,
		Cover = 11,
		Reload = 12,
		Grenade = 13,
		Takedown = 14,
		Item = 15,
		Energy = 16,
		Health = 17,
		Holster = 18,
		Cloak = 19,
		Armor = 20,
		Xray = 21,
		Dash = 22,
		Augs = 23,
		Inventory = 24,
		Missions = 25,
		Map = 26,
		Logs = 27,
		Pause = 28,
		Save = 29,
		Load = 30
	}

	public class Key
	{
		public KeyName m_Name;

		public KeyCode m_KeyCode;

		public KeyCode m_AlternateCode;

		private bool m_LastDown;

		private bool m_Down;

		private bool m_Pressed;

		private bool m_Released;

		private bool m_Tapped;

		private bool m_Held;

		private float m_HeldTimer;

		private float m_HeldTime;

		public bool Down
		{
			get
			{
				return m_Down;
			}
			set
			{
				m_LastDown = m_Down;
				m_Down = value;
			}
		}

		public bool Pressed
		{
			get
			{
				return m_Pressed;
			}
			set
			{
				m_Pressed = value;
			}
		}

		public bool Released
		{
			get
			{
				return m_Released;
			}
		}

		public bool Tapped
		{
			get
			{
				return m_Tapped;
			}
			set
			{
				m_Tapped = value;
			}
		}

		public bool Held
		{
			get
			{
				return m_Held;
			}
		}

		public Key(KeyName name, KeyCode keycode, KeyCode alternate)
		{
			m_Name = name;
			m_KeyCode = keycode;
			m_AlternateCode = alternate;
			m_LastDown = false;
			m_Down = false;
			m_Pressed = false;
			m_Released = false;
			m_Tapped = false;
			m_Held = false;
			m_HeldTimer = 0f;
			m_HeldTime = 0.4f;
		}

		public void Update()
		{
			m_Pressed = false;
			m_Released = false;
			m_Tapped = false;
			//if (KeyBindingItem.IsBindingKey)
			//{
			//	m_Down = false;
			//	m_Held = false;
			//	m_HeldTimer = 0f;
			//	return;
			//}
			if (m_Down)
			{
				m_Pressed = !m_LastDown;
				m_HeldTimer += m_RealDeltaTime;
			}
			else
			{
				if (m_LastDown)
				{
					m_Tapped = m_HeldTimer < m_HeldTime;
					m_Released = true;
				}
				m_HeldTimer = 0f;
			}
			m_Held = m_HeldTimer >= m_HeldTime;
		}
	}

	public static KeyboardInput m_This;

	public TextAsset m_InputXmlFile;

	public static float m_RealDeltaTime;

	public static bool m_KeyboardEnabled;

	private static float m_LastTimeSinceStartup;

	private static List<Key> m_KeyList;

	private Vector3 m_LastMousePos;

	private Vector3 m_CurrentMousePos;

	public static List<Key> KeyList
	{
		get
		{
			return m_KeyList;
		}
	}

	private void Start()
	{
		m_KeyList = new List<Key>();
		LoadXml(false);
		m_LastMousePos = Vector3.zero;
		m_RealDeltaTime = 0f;
		m_LastTimeSinceStartup = 0f;
		m_KeyboardEnabled = true;
		m_This = this;
	}

	private void LoadXml(bool useDefault, bool forceSave = false)
	{
		string text = Path.Combine(Application.persistentDataPath, "KeyboardProfile.xml");
		XmlDocument xmlDocument = new XmlDocument();
		if (!useDefault && File.Exists(text))
		{
			xmlDocument.Load(text);
		}
		else
		{
			TextAsset textAsset = (TextAsset)Resources.Load("Input/InputProfile-" + Application.systemLanguage);
			try
			{
				xmlDocument.LoadXml(textAsset.text);
			}
			catch (Exception exception)
			{
				if (textAsset != null)
				{
					Debug.LogException(exception);
				}
				xmlDocument.LoadXml(m_InputXmlFile.text);
			}
		}
		XmlElement xmlElement = xmlDocument.FirstChild.NextSibling as XmlElement;
		XmlElement xmlElement2 = xmlElement.GetElementsByTagName("Keyboard")[0] as XmlElement;
		XmlNodeList elementsByTagName = xmlElement2.GetElementsByTagName("Key");
		m_KeyList.Clear();
		for (int i = 0; i < elementsByTagName.Count; i++)
		{
			XmlElement xmlElement3 = elementsByTagName[i] as XmlElement;
			string attribute = xmlElement3.GetAttribute("name");
			string innerText = xmlElement3.InnerText;
			string attribute2 = xmlElement3.GetAttribute("alt");
			m_KeyList.Add(new Key((KeyName)(int)Enum.Parse(typeof(KeyName), attribute, true), (KeyCode)(int)Enum.Parse(typeof(KeyCode), innerText, true), (KeyCode)(int)Enum.Parse(typeof(KeyCode), attribute2, true)));
		}
		if (!File.Exists(text) || forceSave)
		{
			SaveBindings();
		}
	}

	public static void RestoreDefaults()
	{
		if (m_This != null)
		{
			m_This.LoadXml(true, true);
		}
	}

	public static void SaveBindings()
	{
		XmlTextWriter xmlTextWriter = new XmlTextWriter(Path.Combine(Application.persistentDataPath, "KeyboardProfile.xml"), Encoding.UTF8);
		xmlTextWriter.Formatting = Formatting.Indented;
		xmlTextWriter.WriteStartDocument();
		xmlTextWriter.WriteStartElement("InputProfile");
		xmlTextWriter.WriteStartElement("Keyboard");
		for (int i = 0; i < m_KeyList.Count; i++)
		{
			xmlTextWriter.WriteStartElement("Key");
			xmlTextWriter.WriteAttributeString("name", m_KeyList[i].m_Name.ToString());
			xmlTextWriter.WriteAttributeString("alt", m_KeyList[i].m_AlternateCode.ToString());
			xmlTextWriter.WriteString(m_KeyList[i].m_KeyCode.ToString());
			xmlTextWriter.WriteEndElement();
		}
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndElement();
		xmlTextWriter.WriteEndDocument();
		xmlTextWriter.Close();
	}

	public static bool SetKeyBinding(KeyName name, KeyCode keyCode, bool isAlt)
	{
		if (m_This == null)
		{
			return false;
		}
		if (keyCode.ToString().ToLower().Contains("joystick"))
		{
			return false;
		}
		Key key = null;
		Key key2 = null;
		for (int i = 0; i < m_KeyList.Count; i++)
		{
			if (m_KeyList[i].m_Name == name)
			{
				key = m_KeyList[i];
			}
			if (m_KeyList[i].m_KeyCode == keyCode || m_KeyList[i].m_AlternateCode == keyCode)
			{
				key2 = m_KeyList[i];
			}
		}
		if (key == null)
		{
			m_KeyList.Add(new Key(name, keyCode, KeyCode.None));
			return true;
		}
		if (keyCode == KeyCode.None)
		{
			if (isAlt)
			{
				key.m_AlternateCode = KeyCode.None;
			}
		}
		else if (key2 == null)
		{
			if (isAlt)
			{
				key.m_AlternateCode = keyCode;
			}
			else
			{
				key.m_KeyCode = keyCode;
			}
		}
		else
		{
			bool flag = key2.m_AlternateCode == keyCode;
			KeyCode keyCode2 = ((!isAlt) ? key.m_KeyCode : key.m_AlternateCode);
			if (!flag && keyCode2 == KeyCode.None)
			{
				return false;
			}
			if (isAlt)
			{
				key.m_AlternateCode = keyCode;
			}
			else
			{
				key.m_KeyCode = keyCode;
			}
			if (flag)
			{
				key2.m_AlternateCode = keyCode2;
			}
			else
			{
				key2.m_KeyCode = keyCode2;
			}
		}
		return true;
	}

	public static bool GetKeyBinding(KeyName name, out KeyCode main, out KeyCode alt)
	{
		main = KeyCode.None;
		alt = KeyCode.None;
		if (m_This == null)
		{
			return false;
		}
		Key key = m_KeyList.Find((Key k) => k.m_Name == name);
		if (key == null)
		{
			return false;
		}
		main = key.m_KeyCode;
		alt = key.m_AlternateCode;
		return true;
	}

	private void Update()
	{
		if (m_This == null)
		{
			return;
		}
		m_RealDeltaTime = Time.realtimeSinceStartup - m_LastTimeSinceStartup;
		m_LastTimeSinceStartup = Time.realtimeSinceStartup;
		foreach (Key key in m_KeyList)
		{
			key.Down = Input.GetKey(key.m_KeyCode) || (key.m_AlternateCode != 0 && Input.GetKey(key.m_AlternateCode));
			key.Update();
		}
		m_LastMousePos = m_CurrentMousePos;
		m_CurrentMousePos = Input.mousePosition;
		Key[] array = m_KeyList.ToArray();
		int num = 0;
	}

	public static bool GetKey(KeyName name)
	{
		if (m_This == null)
		{
			return false;
		}
		Key key = m_KeyList.Find((Key k) => k.m_Name == name);
		if (key == null)
		{
			return false;
		}
		return key.Down;
	}

	public static bool GetKeyDown(KeyName name)
	{
		if (m_This == null)
		{
			return false;
		}
		Key key = m_KeyList.Find((Key k) => k.m_Name == name);
		if (key == null)
		{
			return false;
		}
		return key.Pressed;
	}

	public static bool GetKeyUp(KeyName name)
	{
		if (m_This == null)
		{
			return false;
		}
		Key key = m_KeyList.Find((Key k) => k.m_Name == name);
		if (key == null)
		{
			return false;
		}
		return key.Released;
	}

	public static bool GetKeyHeld(KeyName name)
	{
		if (m_This == null)
		{
			return false;
		}
		Key key = m_KeyList.Find((Key k) => k.m_Name == name);
		if (key == null)
		{
			return false;
		}
		return key.Held;
	}

	public static bool GetKeyTapped(KeyName name)
	{
		if (m_This == null)
		{
			return false;
		}
		Key key = m_KeyList.Find((Key k) => k.m_Name == name);
		if (key == null)
		{
			return false;
		}
		return key.Tapped;
	}

	public static void CancelKeyTapped(KeyName name)
	{
		if (!(m_This == null))
		{
			Key key = m_KeyList.Find((Key k) => k.m_Name == name);
			if (key != null)
			{
				key.Tapped = false;
			}
		}
	}

	public static float GetVertical()
	{
		return ((!GetKey(KeyName.Up)) ? 0f : 1f) + ((!GetKey(KeyName.Down)) ? 0f : (-1f));
	}

	public static float GetHorizontal()
	{
		return ((!GetKey(KeyName.Right)) ? 0f : 1f) + ((!GetKey(KeyName.Left)) ? 0f : (-1f));
	}

	public static Vector3 GetMouseDelta()
	{
		if (m_This != null)
		{
			return new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);
		}
		return Vector3.zero;
	}

	public static float GetScrollDelta()
	{
		if (m_This != null)
		{
			return Input.GetAxis("Mouse ScrollWheel");
		}
		return 0f;
	}

	public static string GetKeyCodeName(KeyName name)
	{
		if (m_This == null)
		{
			return string.Empty;
		}
		Key key = m_KeyList.Find((Key k) => k.m_Name == name);
		if (key == null)
		{
			return string.Empty;
		}
		return KeyCodeDisplayName(key.m_KeyCode);
	}

	public static string KeyCodeDisplayName(KeyCode key)
	{
		string empty = string.Empty;
		switch (key)
		{
		case KeyCode.None:
			return string.Empty;
		case KeyCode.Alpha0:
		case KeyCode.Alpha1:
		case KeyCode.Alpha2:
		case KeyCode.Alpha3:
		case KeyCode.Alpha4:
		case KeyCode.Alpha5:
		case KeyCode.Alpha6:
		case KeyCode.Alpha7:
		case KeyCode.Alpha8:
		case KeyCode.Alpha9:
			return key.ToString().Substring(5);
		case KeyCode.Ampersand:
			return "&";
		case KeyCode.Asterisk:
			return "*";
		case KeyCode.At:
			return "@";
		case KeyCode.BackQuote:
			return "`";
		case KeyCode.Backslash:
			return "\\";
		case KeyCode.Caret:
			return "^";
		case KeyCode.Colon:
			return ":";
		case KeyCode.Comma:
			return ",";
		case KeyCode.Dollar:
			return "$";
		case KeyCode.DoubleQuote:
			return "\"";
		case KeyCode.DownArrow:
			return "Down";
		case KeyCode.Equals:
			return "=";
		case KeyCode.Exclaim:
			return "!";
		case KeyCode.Greater:
			return ">";
		case KeyCode.Hash:
			return "#";
		case KeyCode.LeftArrow:
			return "Left";
		case KeyCode.LeftBracket:
			return "[";
		case KeyCode.LeftParen:
			return "(";
		case KeyCode.Less:
			return "<";
		case KeyCode.Minus:
			return "-";
		case KeyCode.Period:
			return ".";
		case KeyCode.Plus:
			return "+";
		case KeyCode.Question:
			return "?";
		case KeyCode.Quote:
			return "'";
		case KeyCode.RightArrow:
			return "Right";
		case KeyCode.RightBracket:
			return "]";
		case KeyCode.RightParen:
			return ")";
		case KeyCode.Semicolon:
			return ";";
		case KeyCode.Slash:
			return "/";
		case KeyCode.Underscore:
			return "_";
		case KeyCode.UpArrow:
			return "Up";
		default:
			return key.ToString();
		}
	}
}
