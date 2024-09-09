using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
[AddComponentMenu("EZ GUI/Controls/Label")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class SpriteText : MonoBehaviour, IUseCamera
{
	public enum Anchor_Pos
	{
		Upper_Left = 0,
		Upper_Center = 1,
		Upper_Right = 2,
		Middle_Left = 3,
		Middle_Center = 4,
		Middle_Right = 5,
		Lower_Left = 6,
		Lower_Center = 7,
		Lower_Right = 8
	}

	public enum Alignment_Type
	{
		Left = 0,
		Center = 1,
		Right = 2
	}

	protected struct NewlineInsertInfo
	{
		public int index;

		public int charDelta;

		public NewlineInsertInfo(int idx, int delta)
		{
			index = idx;
			charDelta = delta;
		}
	}

	[HideInInspector]
	public const string colorTag = "[#";

	public string text = "Hello World";

	public float offsetZ;

	public float characterSize = 1f;

	public float characterSpacing = 1f;

	public float lineSpacing = 1.1f;

	protected float lineSpaceSize;

	public Anchor_Pos anchor;

	public Alignment_Type alignment;

	public int tabSize = 4;

	protected string tabSpaces = "    ";

	public TextAsset font;

	public Color color = Color.white;

	public bool pixelPerfect;

	public float maxWidth;

	public bool maxWidthInPixels;

	public bool multiline = true;

	public bool dynamicLength;

	public bool removeUnsupportedCharacters = true;

	public bool parseColorTags = true;

	public bool password;

	public string maskingCharacter = "*";

	protected EZScreenPlacement screenPlacer;

	private IControl parentControl;

	protected bool clipped;

	protected bool updateClipping;

	protected Rect3D clippingRect;

	protected Rect localClipRect;

	protected Vector3 topLeft;

	protected Vector3 bottomRight;

	protected Vector3 unclippedTL;

	protected Vector3 unclippedBR;

	protected Color[] colors = new Color[0];

	protected bool updateColors;

	protected static string[] colDel = new string[4] { "RGBA(", "[#", ")", "]" };

	protected static char[] newLineDelimiter = new char[1] { '\n' };

	protected static char[] commaDelimiter = new char[1] { ',' };

	[HideInInspector]
	public bool isClone;

	protected bool m_awake;

	protected bool m_started;

	protected bool stringContentChanged = true;

	protected Vector2 screenSize;

	public Camera renderCamera;

	[HideInInspector]
	public Vector2 pixelsPerUV;

	protected float worldUnitsPerScreenPixel;

	protected float worldUnitsPerTexel;

	protected Vector2 worldUnitsPerUV;

	public bool hideAtStart;

	protected bool m_hidden;

	public bool persistent;

	public bool ignoreClipping;

	protected int capacity;

	protected string meshString = string.Empty;

	protected string plainText = string.Empty;

	protected string displayString = string.Empty;

	protected List<NewlineInsertInfo> newLineInserts = new List<NewlineInsertInfo>();

	protected float totalWidth;

	protected SpriteFont spriteFont;

	protected SpriteTextMirror mirror;

	protected Mesh oldMesh;

	protected Mesh mesh;

	protected MeshRenderer meshRenderer;

	protected MeshFilter meshFilter;

	protected Texture texture;

	protected Vector3[] vertices;

	protected int[] faces;

	protected Vector2[] UVs;

	protected Color[] meshColors;

	private StringBuilder displaySB = new StringBuilder();

	private StringBuilder plainSB = new StringBuilder();

	private List<int> colorInserts = new List<int>();

	private List<int> colorTags = new List<int>();

	private List<Color> cols = new List<Color>();

	private string[] lines;

	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			SetColor(value);
		}
	}

	public Camera RenderCamera
	{
		get
		{
			return renderCamera;
		}
		set
		{
			renderCamera = value;
			SetCamera(value);
		}
	}

	public bool Persistent
	{
		get
		{
			return persistent;
		}
		set
		{
			if (value)
			{
				UnityEngine.Object.DontDestroyOnLoad(this);
				UnityEngine.Object.DontDestroyOnLoad(mesh);
				persistent = value;
			}
		}
	}

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			if (!m_awake)
			{
				Awake();
			}
			if (spriteFont != null)
			{
				if (!m_started)
				{
					Start();
				}
				stringContentChanged = true;
				if (removeUnsupportedCharacters)
				{
					ProcessString(spriteFont.RemoveUnsupportedCharacters(value));
				}
				else
				{
					ProcessString(value);
				}
				UpdateMesh();
			}
		}
	}

	public string PlainText
	{
		get
		{
			return plainText;
		}
	}

	public string DisplayString
	{
		get
		{
			return displayString;
		}
	}

	public Rect3D ClippingRect
	{
		get
		{
			return clippingRect;
		}
		set
		{
			if (!ignoreClipping)
			{
				clippingRect = value;
				clipped = true;
				updateClipping = true;
				UpdateMesh();
			}
		}
	}

	public bool Clipped
	{
		get
		{
			return clipped;
		}
		set
		{
			if (!ignoreClipping)
			{
				if (value && !clipped)
				{
					clipped = true;
					updateClipping = true;
					UpdateMesh();
				}
				else if (clipped)
				{
					Unclip();
				}
			}
		}
	}

	public float BaseHeight
	{
		get
		{
			if (spriteFont != null)
			{
				return (float)spriteFont.BaseHeight * worldUnitsPerTexel;
			}
			return 0f;
		}
	}

	public float LineSpan
	{
		get
		{
			return lineSpaceSize;
		}
	}

	public Vector3 TopLeft
	{
		get
		{
			return topLeft;
		}
	}

	public Vector3 BottomRight
	{
		get
		{
			return bottomRight;
		}
	}

	public Vector3 UnclippedTopLeft
	{
		get
		{
			if (!m_started)
			{
				Start();
			}
			return unclippedTL;
		}
	}

	public Vector3 UnclippedBottomRight
	{
		get
		{
			if (!m_started)
			{
				Start();
			}
			return unclippedBR;
		}
	}

	public float TotalWidth
	{
		get
		{
			return totalWidth;
		}
	}

	public float TotalScreenWidth
	{
		get
		{
			if (renderCamera == null)
			{
				return 0f;
			}
			Plane plane = new Plane(renderCamera.transform.forward, renderCamera.transform.position);
			screenSize.x = renderCamera.pixelWidth;
			screenSize.y = renderCamera.pixelHeight;
			float distanceToPoint = plane.GetDistanceToPoint(base.transform.position);
			worldUnitsPerScreenPixel = Vector3.Distance(renderCamera.ScreenToWorldPoint(new Vector3(0f, 1f, distanceToPoint)), renderCamera.ScreenToWorldPoint(new Vector3(0f, 0f, distanceToPoint)));
			return totalWidth / worldUnitsPerScreenPixel;
		}
	}

	public Vector2 PixelSize
	{
		get
		{
			Vector2 vector = new Vector2(bottomRight.x - topLeft.x, topLeft.y - bottomRight.y);
			return new Vector2(vector.x * worldUnitsPerScreenPixel, vector.y * worldUnitsPerScreenPixel);
		}
	}

	public Anchor_Pos Anchor
	{
		get
		{
			return anchor;
		}
		set
		{
			SetAnchor(value);
		}
	}

	public float CharacterSpacing
	{
		get
		{
			return characterSpacing;
		}
		set
		{
			characterSpacing = value;
			LayoutText();
		}
	}

	public IControl Parent
	{
		get
		{
			return parentControl;
		}
		set
		{
			parentControl = value;
		}
	}

	protected virtual void Awake()
	{
		if (!m_awake)
		{
			m_awake = true;
			if (base.name.EndsWith("(Clone)"))
			{
				isClone = true;
			}
			meshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));
			meshRenderer = (MeshRenderer)GetComponent(typeof(MeshRenderer));
			oldMesh = meshFilter.sharedMesh;
			meshFilter.sharedMesh = null;
			Init();
		}
	}

	public virtual void Start()
	{
		if (m_started)
		{
			return;
		}
		m_started = true;
		if (!isClone && Application.isPlaying)
		{
			UnityEngine.Object.Destroy(oldMesh);
			oldMesh = null;
		}
		if (renderCamera == null)
		{
			if (UIManager.Exists() && UIManager.instance.uiCameras != null && UIManager.instance.uiCameras.Length > 0)
			{
				renderCamera = UIManager.instance.uiCameras[0].camera;
			}
			else
			{
				renderCamera = Camera.mainCamera;
			}
		}
		SetCamera(renderCamera);
		ProcessString(text);
		updateColors = true;
		UpdateMesh();
	}

	protected virtual void Init()
	{
		screenPlacer = (EZScreenPlacement)GetComponent(typeof(EZScreenPlacement));
		if (screenPlacer != null)
		{
			screenPlacer.SetCamera(renderCamera);
		}
		if (font == null && UIManager.Exists())
		{
			font = UIManager.instance.defaultFont;
		}
		if (meshRenderer.sharedMaterial == null && UIManager.Exists())
		{
			meshRenderer.sharedMaterial = UIManager.instance.defaultFontMaterial;
			if (meshRenderer.sharedMaterial != null)
			{
				texture = meshRenderer.sharedMaterial.mainTexture;
			}
		}
		else if (meshRenderer.sharedMaterial != null)
		{
			texture = meshRenderer.sharedMaterial.mainTexture;
		}
		if (texture == null && Application.isPlaying)
		{
			Debug.LogWarning("Text on GameObject \"" + base.name + "\" has not been assigned either a texture or a material.");
		}
		if (font != null)
		{
			spriteFont = FontStore.GetFont(font);
			if (spriteFont == null)
			{
				Debug.LogWarning("Warning: " + base.name + " was unable to load font \"" + font.name + "\"!");
			}
		}
		else if (Application.isPlaying)
		{
			Debug.LogWarning("Warning: " + base.name + " currently has no font assigned.");
		}
		if (mesh == null)
		{
			CreateMesh();
		}
		if (persistent)
		{
			Persistent = true;
		}
		if (texture != null)
		{
			SetPixelToUV(texture);
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < tabSize; i++)
		{
			stringBuilder.Append(' ');
		}
		tabSpaces = stringBuilder.ToString();
	}

	protected void CreateMesh()
	{
		if (!(meshFilter == null))
		{
			meshFilter.sharedMesh = new Mesh();
			mesh = meshFilter.sharedMesh;
			if (persistent)
			{
				UnityEngine.Object.DontDestroyOnLoad(mesh);
			}
		}
	}

	protected void ProcessString(string str)
	{
		colorInserts.Clear();
		colorTags.Clear();
		cols.Clear();
		newLineInserts.Clear();
		int num = 0;
		int num2 = -1;
		int num3 = -1;
		float num4 = ((!maxWidthInPixels) ? maxWidth : (maxWidth * worldUnitsPerScreenPixel));
		text = str;
		if (string.IsNullOrEmpty(str) || spriteFont == null)
		{
			plainText = string.Empty;
			displayString = string.Empty;
			return;
		}
		if (str.IndexOf('\t') != -1)
		{
			str = str.Replace("\t", tabSpaces);
		}
		int num5;
		if (parseColorTags)
		{
			num5 = 0;
			do
			{
				num5 = str.IndexOf(colDel[0], num5);
				if (num5 != -1)
				{
					colorTags.Add(num5);
					num5++;
				}
			}
			while (num5 != -1);
			num5 = 0;
			do
			{
				num5 = str.IndexOf(colDel[1], num5);
				if (num5 != -1)
				{
					colorTags.Add(num5);
					num5++;
				}
			}
			while (num5 != -1);
		}
		if (maskingCharacter.Length < 1)
		{
			maskingCharacter = "*";
		}
		if (colorTags.Count < 1)
		{
			plainText = str;
			displaySB.Remove(0, displaySB.Length);
			if (maxWidth > 0f && multiline)
			{
				num4 = ((!maxWidthInPixels) ? maxWidth : (maxWidth * worldUnitsPerScreenPixel));
				for (int i = 0; i < str.Length; i++)
				{
					if (char.IsWhiteSpace(str[i]))
					{
						if (str[i] == '\n')
						{
							num4 = ((!maxWidthInPixels) ? maxWidth : (maxWidth * worldUnitsPerScreenPixel));
						}
						num2 = displaySB.Length;
						num3 = i;
					}
					if (password && str[i] != '\n')
					{
						displaySB.Append(maskingCharacter[0]);
					}
					else
					{
						displaySB.Append(str[i]);
					}
					num4 = ((i == num3 + 1 || i <= 0) ? (num4 - spriteFont.GetAdvance(str[i]) * worldUnitsPerTexel * characterSpacing) : (num4 - spriteFont.GetWidth(str[i - 1], str[i]) * worldUnitsPerTexel * characterSpacing));
					if (!(num4 < 0f))
					{
						continue;
					}
					num4 = ((i != num2) ? (((!maxWidthInPixels) ? maxWidth : (maxWidth * worldUnitsPerScreenPixel)) - spriteFont.GetWidth(displaySB, num2 + 1, i) * worldUnitsPerTexel * characterSpacing) : ((!maxWidthInPixels) ? maxWidth : (maxWidth * worldUnitsPerScreenPixel)));
					if (num4 < 0f)
					{
						if (displaySB.Length > 0)
						{
							displaySB.Insert(displaySB.Length - 1, '\n');
							num4 = ((!maxWidthInPixels) ? maxWidth : (maxWidth * worldUnitsPerScreenPixel)) - spriteFont.GetAdvance(str[i]) * worldUnitsPerTexel * characterSpacing;
							newLineInserts.Add(new NewlineInsertInfo(i, 1));
						}
					}
					else if (num2 >= 0)
					{
						displaySB[num2] = '\n';
						newLineInserts.Add(new NewlineInsertInfo(num3, 0));
					}
				}
				displayString = displaySB.ToString();
			}
			else if (password)
			{
				displaySB.Remove(0, displaySB.Length);
				for (int j = 0; j < str.Length; j++)
				{
					displaySB.Append(maskingCharacter[0]);
				}
				displayString = displaySB.ToString();
			}
			else
			{
				displaySB.Append(str);
				displayString = str;
			}
			if (!multiline)
			{
				DoSingleLineTruncation();
				displayString = displaySB.ToString();
			}
			if (colors.Length < displayString.Length)
			{
				colors = new Color[displayString.Length];
			}
			for (int k = 0; k < colors.Length; k++)
			{
				colors[k] = this.color;
			}
			updateColors = true;
			return;
		}
		colorTags.Sort();
		colorTags.Add(-1);
		plainSB.Remove(0, plainSB.Length);
		displaySB.Remove(0, displaySB.Length);
		num5 = 0;
		while (num < str.Length)
		{
			int num6;
			if (num == colorTags[num5])
			{
				if (string.Compare(str, num, colDel[0], 0, colDel[0].Length) == 0)
				{
					num += colDel[0].Length;
					num6 = str.IndexOf(')', colorTags[num5]) - num;
					if (num6 < 0)
					{
						num6 = str.Length - num;
					}
					colorInserts.Add(displaySB.Length);
					cols.Add(ParseColor(str.Substring(num, num6)));
					num += num6 + 1;
				}
				else if (string.Compare(str, num, colDel[1], 0, colDel[1].Length) == 0)
				{
					num += colDel[1].Length;
					num6 = str.IndexOf(']', colorTags[num5]) - num;
					if (num6 < 0)
					{
						num6 = str.Length - num;
					}
					colorInserts.Add(displaySB.Length);
					cols.Add(ParseHexColor(str.Substring(num, num6)));
					num += num6 + 1;
				}
				num5++;
				continue;
			}
			if (maxWidth > 0f && multiline)
			{
				if (char.IsWhiteSpace(str[num]))
				{
					if (str[num] == '\n')
					{
						num4 = ((!maxWidthInPixels) ? maxWidth : (maxWidth * worldUnitsPerScreenPixel));
					}
					num2 = displaySB.Length;
					num3 = num;
				}
				if (password && str[num] != '\n')
				{
					displaySB.Append(maskingCharacter[0]);
				}
				else
				{
					displaySB.Append(str[num]);
				}
				plainSB.Append(str[num]);
				num4 = ((num == num3 + 1 || num <= 0) ? (num4 - spriteFont.GetAdvance(str[num]) * worldUnitsPerTexel * characterSpacing) : (num4 - spriteFont.GetWidth(str[num - 1], str[num]) * worldUnitsPerTexel * characterSpacing));
				if (num4 < 0f)
				{
					num4 = ((num != num3) ? (((!maxWidthInPixels) ? maxWidth : (maxWidth * worldUnitsPerScreenPixel)) - spriteFont.GetWidth(displaySB, num2 + 1, num) * worldUnitsPerTexel * characterSpacing) : ((!maxWidthInPixels) ? maxWidth : (maxWidth * worldUnitsPerScreenPixel)));
					if (num4 < 0f)
					{
						if (displaySB.Length > 0)
						{
							displaySB.Insert(displaySB.Length - 1, '\n');
							num4 = ((!maxWidthInPixels) ? maxWidth : (maxWidth * worldUnitsPerScreenPixel)) - spriteFont.GetAdvance(str[num]) * worldUnitsPerTexel * characterSpacing;
							newLineInserts.Add(new NewlineInsertInfo(plainSB.Length - 1, 1));
							int num7 = colorInserts.Count - 1;
							while (num7 >= 0 && colorInserts[num7] > newLineInserts[newLineInserts.Count - 1].index)
							{
								List<int> list;
								List<int> list2 = (list = colorInserts);
								int index;
								int index2 = (index = num7);
								index = list[index];
								list2[index2] = index + 1;
								num7--;
							}
						}
					}
					else if (num2 >= 0)
					{
						displaySB[num2] = '\n';
						newLineInserts.Add(new NewlineInsertInfo(num3, 0));
					}
				}
				num++;
				continue;
			}
			num6 = ((colorTags[num5] != -1) ? (colorTags[num5] - num) : (str.Length - num));
			plainSB.Append(str, num, num6);
			if (password)
			{
				for (int l = num; l < num + num6; l++)
				{
					if (spriteFont.ContainsCharacter(str[l]))
					{
						displaySB.Append(maskingCharacter[0]);
					}
				}
			}
			else
			{
				displaySB.Append(str, num, num6);
			}
			num += num6;
		}
		if (colorInserts.Count == 0)
		{
			colorInserts.Add(0);
			cols.Add(this.color);
		}
		if (!multiline)
		{
			DoSingleLineTruncation();
		}
		plainText = plainSB.ToString();
		displayString = displaySB.ToString();
		if (colors.Length < displayString.Length)
		{
			colors = new Color[displayString.Length];
		}
		Color color = this.color;
		int m = 0;
		int num8 = 0;
		for (; m < displayString.Length; m++)
		{
			if (m == colorInserts[num8])
			{
				color = cols[num8];
				num8 = (num8 + 1) % colorInserts.Count;
			}
			colors[m] = color;
		}
		updateColors = true;
	}

	protected void DoSingleLineTruncation()
	{
		int num = displayString.IndexOf('\n');
		if (num >= 0)
		{
			displaySB.Remove(num, displaySB.Length - num);
			displaySB.Append("...");
		}
		if (maxWidth > 0f)
		{
			float num2 = spriteFont.GetWidth(displaySB, 0, displaySB.Length - 1) * worldUnitsPerTexel * characterSpacing;
			float num3 = ((!maxWidthInPixels) ? maxWidth : (maxWidth * worldUnitsPerScreenPixel));
			if (num2 > num3)
			{
				int num4 = 0;
				float num5 = spriteFont.GetWidth("...") * worldUnitsPerTexel * characterSpacing;
				do
				{
					num4++;
					num2 = spriteFont.GetWidth(displaySB, 0, displaySB.Length - 1 - num4) * worldUnitsPerTexel * characterSpacing;
				}
				while (num2 + num5 > num3 && num2 != 0f);
				num4 = Mathf.Clamp(num4, 0, displaySB.Length);
				displaySB.Remove(displaySB.Length - num4, num4);
				displaySB.Append("...");
			}
		}
		if (password)
		{
			for (int i = 0; i < displaySB.Length; i++)
			{
				displaySB[i] = maskingCharacter[0];
			}
		}
	}

	protected Color ParseColor(string str)
	{
		string[] array = str.Split(commaDelimiter);
		if (array.Length != 4)
		{
			return color;
		}
		return color * new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
	}

	protected Color ParseHexColor(string str)
	{
		if (str.Length < 6)
		{
			return color;
		}
		try
		{
			int num = int.Parse(str.Substring(0, 2), NumberStyles.AllowHexSpecifier);
			int num2 = int.Parse(str.Substring(2, 2), NumberStyles.AllowHexSpecifier);
			int num3 = int.Parse(str.Substring(4, 2), NumberStyles.AllowHexSpecifier);
			int num4 = 255;
			if (str.Length == 8)
			{
				num4 = int.Parse(str.Substring(6, 2), NumberStyles.AllowHexSpecifier);
			}
			return color * new Color((float)num / 255f, (float)num2 / 255f, (float)num3 / 255f, (float)num4 / 255f);
		}
		catch
		{
			return color;
		}
	}

	protected void EnlargeMesh()
	{
		vertices = new Vector3[displayString.Length * 4];
		UVs = new Vector2[displayString.Length * 4];
		meshColors = new Color[displayString.Length * 4];
		faces = new int[displayString.Length * 6];
		for (int i = 0; i < displayString.Length; i++)
		{
			faces[i * 6] = i * 4;
			faces[i * 6 + 1] = i * 4 + 3;
			faces[i * 6 + 2] = i * 4 + 1;
			faces[i * 6 + 3] = i * 4 + 3;
			faces[i * 6 + 4] = i * 4 + 2;
			faces[i * 6 + 5] = i * 4 + 1;
		}
		capacity = displayString.Length;
	}

	public void UpdateMesh()
	{
		if (mesh == null)
		{
			CreateMesh();
		}
		if (spriteFont == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		if (meshString.Length < 15 && !updateClipping && !updateColors && stringContentChanged && meshString == displayString)
		{
			return;
		}
		if (displayString.Length < 1)
		{
			ClearMesh();
		}
		else
		{
			if (displayString.Length > capacity)
			{
				EnlargeMesh();
				flag = true;
			}
			if (clipped)
			{
				updateClipping = false;
				localClipRect = Rect3D.MultFast(clippingRect, base.transform.worldToLocalMatrix).GetRect();
			}
			if (stringContentChanged)
			{
				lines = null;
				int num = displayString.IndexOf('\n');
				if (num == -1)
				{
					flag2 = true;
				}
				else
				{
					lines = displayString.Split(newLineDelimiter);
					flag2 = false;
				}
			}
			if (flag2 || lines == null || lines.Length == 1)
			{
				Layout_Single_Line();
			}
			else
			{
				Layout_Multiline(lines);
			}
			unclippedTL = topLeft;
			unclippedBR = bottomRight;
			if (clipped)
			{
				topLeft.x = Mathf.Max(localClipRect.x, topLeft.x);
				topLeft.y = Mathf.Min(localClipRect.yMax, topLeft.y);
				bottomRight.x = Mathf.Min(localClipRect.xMax, bottomRight.x);
				bottomRight.y = Mathf.Max(localClipRect.y, bottomRight.y);
			}
		}
		stringContentChanged = false;
		meshString = displayString;
		if (flag)
		{
			mesh.Clear();
		}
		mesh.vertices = vertices;
		mesh.uv = UVs;
		mesh.colors = meshColors;
		mesh.triangles = faces;
		if (flag)
		{
			mesh.RecalculateNormals();
		}
		mesh.RecalculateBounds();
		if (parentControl == null)
		{
			return;
		}
		if (parentControl is AutoSpriteControlBase)
		{
			if (((AutoSpriteControlBase)parentControl).includeTextInAutoCollider)
			{
				((AutoSpriteControlBase)parentControl).UpdateCollider();
			}
			((AutoSpriteControlBase)parentControl).FindOuterEdges();
		}
		else if (parentControl is ControlBase && ((ControlBase)parentControl).includeTextInAutoCollider)
		{
			((ControlBase)parentControl).UpdateCollider();
		}
	}

	protected Vector3 GetStartPos_SingleLine(float baseHeight, float width)
	{
		switch (anchor)
		{
		case Anchor_Pos.Upper_Left:
			return new Vector3(0f, 0f, offsetZ);
		case Anchor_Pos.Upper_Center:
			return new Vector3(width * -0.5f, 0f, offsetZ);
		case Anchor_Pos.Upper_Right:
			return new Vector3(0f - width, 0f, offsetZ);
		case Anchor_Pos.Middle_Left:
			return new Vector3(0f, baseHeight * 0.5f, offsetZ);
		case Anchor_Pos.Middle_Center:
			return new Vector3(width * -0.5f, baseHeight * 0.5f, offsetZ);
		case Anchor_Pos.Middle_Right:
			return new Vector3(0f - width, baseHeight * 0.5f, offsetZ);
		case Anchor_Pos.Lower_Left:
			return new Vector3(0f, baseHeight, offsetZ);
		case Anchor_Pos.Lower_Center:
			return new Vector3(width * -0.5f, baseHeight, offsetZ);
		case Anchor_Pos.Lower_Right:
			return new Vector3(0f - width, baseHeight, offsetZ);
		default:
			return new Vector3(0f, 0f, offsetZ);
		}
	}

	public int GetDisplayLineCount(int charIndex, out int charLine, out int lineStart, out int lineEnd)
	{
		int num = 1;
		int num2 = 0;
		charLine = -1;
		int num3 = -1;
		lineStart = 0;
		lineEnd = -1;
		for (int i = 0; i < displayString.Length; i++)
		{
			if (displayString[i] == '\n')
			{
				if (num == charLine)
				{
					lineEnd = Mathf.Max(0, i - 1);
				}
				num3 = i;
				num++;
			}
			if (num2 == charIndex)
			{
				charLine = num;
				lineStart = num3 + 1;
			}
			num2++;
		}
		if (lineEnd < 0)
		{
			lineEnd = displayString.Length - 1;
		}
		if (charLine < 0)
		{
			charLine = num;
			lineStart = Mathf.Min(displayString.Length - 1, num3 + 1);
		}
		return num;
	}

	public int GetDisplayLineCount()
	{
		int num = 1;
		for (int i = 0; i < displayString.Length; i++)
		{
			if (displayString[i] == '\n')
			{
				num++;
			}
		}
		return num;
	}

	public int PlainIndexToDisplayIndex(int plainCharIndex)
	{
		int num = plainCharIndex;
		for (int i = 0; i < newLineInserts.Count && newLineInserts[i].index <= plainCharIndex; i++)
		{
			num += newLineInserts[i].charDelta;
		}
		return num;
	}

	public int DisplayIndexToPlainIndex(int dispCharIndex)
	{
		int num = dispCharIndex;
		for (int i = 0; i < newLineInserts.Count && newLineInserts[i].index <= dispCharIndex; i++)
		{
			num -= newLineInserts[i].charDelta;
		}
		return num;
	}

	protected float GetLineBaseline(int numLines, int lineNum)
	{
		float num = (float)spriteFont.BaseHeight * worldUnitsPerTexel;
		float num2 = lineSpaceSize - num;
		float num3 = lineSpaceSize - characterSize;
		float num4 = characterSize * (float)numLines + num3 * ((float)numLines - 1f);
		switch (anchor)
		{
		case Anchor_Pos.Upper_Left:
		case Anchor_Pos.Upper_Center:
		case Anchor_Pos.Upper_Right:
			return (float)lineNum * (0f - lineSpaceSize) + num2;
		case Anchor_Pos.Middle_Left:
		case Anchor_Pos.Middle_Center:
		case Anchor_Pos.Middle_Right:
			return num4 * 0.5f + (float)lineNum * (0f - lineSpaceSize) + num2;
		case Anchor_Pos.Lower_Left:
		case Anchor_Pos.Lower_Center:
		case Anchor_Pos.Lower_Right:
			return num4 + (float)lineNum * (0f - lineSpaceSize) + num2;
		default:
			return 0f;
		}
	}

	protected void Layout_Single_Line()
	{
		if (spriteFont == null)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		float num = (float)spriteFont.PixelSize * worldUnitsPerTexel;
		float num2 = (totalWidth = spriteFont.GetWidth(displayString) * worldUnitsPerTexel * characterSpacing);
		zero = (topLeft = GetStartPos_SingleLine(num, num2));
		bottomRight = new Vector3(topLeft.x + num2, topLeft.y - num, 0f);
		Layout_Line(zero, displayString, 0);
		if (displayString.Length < capacity)
		{
			for (int i = displayString.Length; i < capacity; i++)
			{
				vertices[i * 4] = Vector3.zero;
				vertices[i * 4 + 1] = Vector3.zero;
				vertices[i * 4 + 2] = Vector3.zero;
				vertices[i * 4 + 3] = Vector3.zero;
			}
		}
	}

	protected void Layout_Multiline(string[] lines)
	{
		float[] array = new float[lines.Length];
		float num = 0f;
		Vector3 vector = Vector3.zero;
		int num2 = 0;
		float num3 = lineSpaceSize - characterSize;
		float num4 = characterSize * (float)lines.Length + num3 * ((float)lines.Length - 1f);
		for (int i = 0; i < lines.Length; i++)
		{
			array[i] = spriteFont.GetWidth(lines[i]) * worldUnitsPerTexel * characterSpacing;
			if (num < array[i])
			{
				num = array[i];
			}
		}
		totalWidth = num;
		switch (anchor)
		{
		case Anchor_Pos.Upper_Left:
			vector = (topLeft = new Vector3(0f, 0f, offsetZ));
			bottomRight = new Vector3(num, 0f - num4, offsetZ);
			break;
		case Anchor_Pos.Upper_Center:
			vector = (topLeft = new Vector3(num * -0.5f, 0f, offsetZ));
			bottomRight = new Vector3(num * 0.5f, 0f - num4, offsetZ);
			break;
		case Anchor_Pos.Upper_Right:
			vector = (topLeft = new Vector3(0f - num, 0f, offsetZ));
			bottomRight = new Vector3(0f, 0f - num4, offsetZ);
			break;
		case Anchor_Pos.Middle_Left:
			vector = (topLeft = new Vector3(0f, num4 * 0.5f, offsetZ));
			bottomRight = new Vector3(num, num4 * -0.5f, offsetZ);
			break;
		case Anchor_Pos.Middle_Center:
			vector = (topLeft = new Vector3(num * -0.5f, num4 * 0.5f, offsetZ));
			bottomRight = new Vector3(num * 0.5f, num4 * -0.5f, offsetZ);
			break;
		case Anchor_Pos.Middle_Right:
			vector = (topLeft = new Vector3(0f - num, num4 * 0.5f, offsetZ));
			bottomRight = new Vector3(0f, num4 * -0.5f, offsetZ);
			break;
		case Anchor_Pos.Lower_Left:
			vector = (topLeft = new Vector3(0f, num4, offsetZ));
			bottomRight = new Vector3(num, 0f, offsetZ);
			break;
		case Anchor_Pos.Lower_Center:
			vector = (topLeft = new Vector3(num * -0.5f, num4, offsetZ));
			bottomRight = new Vector3(num * 0.5f, 0f, offsetZ);
			break;
		case Anchor_Pos.Lower_Right:
			vector = (topLeft = new Vector3(0f - num, num4, offsetZ));
			bottomRight = new Vector3(0f, 0f, offsetZ);
			break;
		}
		switch (alignment)
		{
		case Alignment_Type.Left:
		{
			for (int k = 0; k < lines.Length; k++)
			{
				Layout_Line(vector, lines[k], num2);
				num2 += lines[k].Length + 1;
				ZeroQuad(num2 - 1);
				vector.y -= lineSpaceSize;
			}
			break;
		}
		case Alignment_Type.Center:
		{
			for (int l = 0; l < lines.Length; l++)
			{
				Layout_Line(vector + Vector3.right * 0.5f * (num - array[l]), lines[l], num2);
				num2 += lines[l].Length + 1;
				ZeroQuad(num2 - 1);
				vector.y -= lineSpaceSize;
			}
			break;
		}
		case Alignment_Type.Right:
		{
			for (int j = 0; j < lines.Length; j++)
			{
				Layout_Line(vector + Vector3.right * (num - array[j]), lines[j], num2);
				num2 += lines[j].Length + 1;
				ZeroQuad(num2 - 1);
				vector.y -= lineSpaceSize;
			}
			break;
		}
		}
		if (num2 < capacity)
		{
			for (int m = num2; m < capacity; m++)
			{
				vertices[m * 4] = Vector3.zero;
				vertices[m * 4 + 1] = Vector3.zero;
				vertices[m * 4 + 2] = Vector3.zero;
				vertices[m * 4 + 3] = Vector3.zero;
			}
		}
	}

	protected void ZeroQuad(int i)
	{
		i *= 4;
		if (i < vertices.Length)
		{
			vertices[i] = (vertices[i + 1] = (vertices[i + 2] = (vertices[i + 3] = Vector3.zero)));
		}
	}

	protected void BuildCharacter(int vertNum, int charNum, Vector3 upperLeft, ref SpriteChar ch)
	{
		vertices[vertNum] = upperLeft;
		vertices[vertNum + 1].x = upperLeft.x;
		vertices[vertNum + 1].y = upperLeft.y - ch.UVs.height * worldUnitsPerUV.y;
		vertices[vertNum + 1].z = upperLeft.z;
		vertices[vertNum + 2] = vertices[vertNum + 1];
		vertices[vertNum + 2].x += ch.UVs.width * worldUnitsPerUV.x;
		vertices[vertNum + 3] = vertices[vertNum + 2];
		vertices[vertNum + 3].y = vertices[vertNum].y;
		UVs[vertNum].x = ch.UVs.x;
		UVs[vertNum].y = ch.UVs.yMax;
		UVs[vertNum + 1].x = ch.UVs.x;
		UVs[vertNum + 1].y = ch.UVs.y;
		UVs[vertNum + 2].x = ch.UVs.xMax;
		UVs[vertNum + 2].y = ch.UVs.y;
		UVs[vertNum + 3].x = ch.UVs.xMax;
		UVs[vertNum + 3].y = ch.UVs.yMax;
		meshColors[vertNum] = colors[charNum];
		meshColors[vertNum + 1] = colors[charNum];
		meshColors[vertNum + 2] = colors[charNum];
		meshColors[vertNum + 3] = colors[charNum];
		if (!clipped)
		{
			return;
		}
		if (vertices[vertNum].x < localClipRect.x)
		{
			if (vertices[vertNum + 2].x < localClipRect.x)
			{
				vertices[vertNum].x = (vertices[vertNum + 1].x = vertices[vertNum + 2].x);
				return;
			}
			float t = (localClipRect.x - vertices[vertNum].x) / (vertices[vertNum + 2].x - vertices[vertNum].x);
			vertices[vertNum].x = (vertices[vertNum + 1].x = localClipRect.x);
			UVs[vertNum].x = (UVs[vertNum + 1].x = Mathf.Lerp(UVs[vertNum].x, UVs[vertNum + 2].x, t));
		}
		else if (vertices[vertNum + 2].x > localClipRect.xMax)
		{
			if (vertices[vertNum].x > localClipRect.xMax)
			{
				vertices[vertNum + 2].x = (vertices[vertNum + 3].x = vertices[vertNum].x);
				return;
			}
			float t2 = (localClipRect.xMax - vertices[vertNum].x) / (vertices[vertNum + 2].x - vertices[vertNum].x);
			vertices[vertNum + 2].x = (vertices[vertNum + 3].x = localClipRect.xMax);
			UVs[vertNum + 2].x = (UVs[vertNum + 3].x = Mathf.Lerp(UVs[vertNum].x, UVs[vertNum + 2].x, t2));
		}
		if (vertices[vertNum].y > localClipRect.yMax)
		{
			if (vertices[vertNum + 2].y > localClipRect.yMax)
			{
				vertices[vertNum].y = (vertices[vertNum + 3].y = vertices[vertNum + 2].y);
				return;
			}
			float t3 = (vertices[vertNum].y - localClipRect.yMax) / (vertices[vertNum].y - vertices[vertNum + 1].y);
			vertices[vertNum].y = (vertices[vertNum + 3].y = localClipRect.yMax);
			UVs[vertNum].y = (UVs[vertNum + 3].y = Mathf.Lerp(UVs[vertNum].y, UVs[vertNum + 1].y, t3));
		}
		else if (vertices[vertNum + 2].y < localClipRect.y)
		{
			if (vertices[vertNum].y < localClipRect.y)
			{
				vertices[vertNum + 1].y = (vertices[vertNum + 2].y = vertices[vertNum].y);
				return;
			}
			float t4 = (vertices[vertNum].y - localClipRect.y) / (vertices[vertNum].y - vertices[vertNum + 1].y);
			vertices[vertNum + 1].y = (vertices[vertNum + 2].y = localClipRect.y);
			UVs[vertNum + 1].y = (UVs[vertNum + 2].y = Mathf.Lerp(UVs[vertNum].y, UVs[vertNum + 1].y, t4));
		}
	}

	protected void Layout_Line(Vector3 startPos, string txt, int charIdx)
	{
		if (txt.Length == 0)
		{
			return;
		}
		SpriteChar ch = spriteFont.GetSpriteChar(txt[0]);
		if (ch != null)
		{
			Vector3 upperLeft = startPos + new Vector3(ch.xOffset * worldUnitsPerTexel, ch.yOffset * worldUnitsPerTexel, 0f);
			BuildCharacter(charIdx * 4, charIdx, upperLeft, ref ch);
		}
		for (int i = 1; i < txt.Length; i++)
		{
			if (ch != null)
			{
				startPos.x += ch.xAdvance * worldUnitsPerTexel * characterSpacing;
			}
			ch = spriteFont.GetSpriteChar(txt[i]);
			if (ch != null)
			{
				startPos.x += ch.GetKerning(txt[i - 1]) * worldUnitsPerTexel * characterSpacing;
				Vector3 upperLeft = startPos + new Vector3(ch.xOffset * worldUnitsPerTexel, ch.yOffset * worldUnitsPerTexel, 0f);
				BuildCharacter((charIdx + i) * 4, charIdx + i, upperLeft, ref ch);
			}
		}
	}

	protected void ClearMesh()
	{
		if (vertices == null)
		{
			EnlargeMesh();
		}
		for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i] = Vector3.zero;
			meshColors[i] = color;
		}
		topLeft = Vector3.zero;
		bottomRight = Vector3.zero;
		unclippedTL = Vector3.zero;
		unclippedBR = Vector3.zero;
	}

	public void Unclip()
	{
		if (!ignoreClipping)
		{
			clipped = false;
			updateClipping = true;
			UpdateMesh();
		}
	}

	public void Delete()
	{
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(mesh);
			mesh = null;
		}
	}

	private void OnEnable()
	{
		if (parentControl != null && parentControl is AutoSpriteControlBase)
		{
			AutoSpriteControlBase autoSpriteControlBase = (AutoSpriteControlBase)parentControl;
			Hide(autoSpriteControlBase.IsHidden());
		}
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying && EZAnimator.Exists())
		{
			EZAnimator.instance.Stop(base.gameObject);
			EZAnimator.instance.Stop(this);
		}
	}

	public virtual void OnDestroy()
	{
		Delete();
	}

	public virtual void Copy(SpriteText s)
	{
		offsetZ = s.offsetZ;
		characterSize = s.characterSize;
		lineSpacing = s.lineSpacing;
		lineSpaceSize = s.lineSpaceSize;
		anchor = s.anchor;
		alignment = s.alignment;
		tabSize = s.tabSize;
		multiline = s.multiline;
		maxWidth = s.maxWidth;
		removeUnsupportedCharacters = s.removeUnsupportedCharacters;
		parseColorTags = s.parseColorTags;
		password = s.password;
		maskingCharacter = s.maskingCharacter;
		ignoreClipping = s.ignoreClipping;
		texture = s.texture;
		SetPixelToUV(texture);
		font = s.font;
		spriteFont = FontStore.GetFont(font);
		lineSpaceSize = lineSpacing * (float)spriteFont.LineHeight * worldUnitsPerTexel;
		color = s.color;
		text = s.text;
		pixelPerfect = s.pixelPerfect;
		dynamicLength = s.dynamicLength;
		SetCamera(s.renderCamera);
		Text = text;
		hideAtStart = s.hideAtStart;
		m_hidden = s.m_hidden;
		Hide(m_hidden);
	}

	public void CalcSize()
	{
		if (spriteFont != null)
		{
			if (pixelPerfect)
			{
				characterSize = (float)spriteFont.PixelSize * worldUnitsPerScreenPixel;
				worldUnitsPerTexel = worldUnitsPerScreenPixel;
				worldUnitsPerUV.x = worldUnitsPerTexel * pixelsPerUV.x;
				worldUnitsPerUV.y = worldUnitsPerTexel * pixelsPerUV.y;
			}
			lineSpaceSize = lineSpacing * (float)spriteFont.LineHeight * worldUnitsPerTexel;
			UpdateMesh();
		}
	}

	protected void LayoutText()
	{
		stringContentChanged = true;
		ProcessString(text);
		UpdateMesh();
	}

	public void SetColor(Color c)
	{
		color = c;
		updateColors = true;
		Text = text;
	}

	public void SetCharacterSize(float size)
	{
		if (spriteFont != null)
		{
			pixelPerfect = false;
			characterSize = size;
			SetPixelToUV(texture);
			lineSpaceSize = lineSpacing * (float)spriteFont.LineHeight * worldUnitsPerTexel;
			LayoutText();
		}
	}

	public void SetLineSpacing(float spacing)
	{
		lineSpacing = spacing;
		lineSpaceSize = lineSpacing * (float)spriteFont.LineHeight * worldUnitsPerTexel;
		LayoutText();
	}

	public void SetFont(TextAsset newFont, Material fontMaterial)
	{
		font = newFont;
		SetFont(FontStore.GetFont(newFont), fontMaterial);
	}

	public void SetFont(SpriteFont newFont, Material fontMaterial)
	{
		font = newFont.fontDef;
		spriteFont = newFont;
		base.renderer.sharedMaterial = fontMaterial;
		texture = fontMaterial.GetTexture("_MainTex");
		SetPixelToUV(texture);
		lineSpaceSize = lineSpacing * (float)spriteFont.LineHeight * worldUnitsPerTexel;
		CalcSize();
		LayoutText();
	}

	public void SetPixelToUV(int texWidth, int texHeight)
	{
		if (spriteFont != null)
		{
			pixelsPerUV.x = texWidth;
			pixelsPerUV.y = texHeight;
			worldUnitsPerTexel = characterSize / (float)spriteFont.PixelSize;
			worldUnitsPerUV.x = worldUnitsPerTexel * (float)texWidth;
			worldUnitsPerUV.y = worldUnitsPerTexel * (float)texHeight;
		}
	}

	public void SetPixelToUV(Texture tex)
	{
		if (!(tex == null))
		{
			SetPixelToUV(tex.width, tex.height);
		}
	}

	public void UpdateCamera()
	{
		SetCamera(renderCamera);
	}

	public void SetCamera()
	{
		SetCamera(renderCamera);
	}

	public void SetCamera(Camera c)
	{
		if (c == null || !m_started)
		{
			return;
		}
		Plane plane = new Plane(c.transform.forward, c.transform.position);
		if (!Application.isPlaying)
		{
			screenSize.x = c.pixelWidth;
			screenSize.y = c.pixelHeight;
			if (screenSize.x != 0f)
			{
				renderCamera = c;
				if (screenPlacer != null)
				{
					screenPlacer.SetCamera(renderCamera);
				}
				float distanceToPoint = plane.GetDistanceToPoint(base.transform.position);
				worldUnitsPerScreenPixel = Vector3.Distance(c.ScreenToWorldPoint(new Vector3(0f, 1f, distanceToPoint)), c.ScreenToWorldPoint(new Vector3(0f, 0f, distanceToPoint)));
				if (!hideAtStart)
				{
					CalcSize();
				}
			}
		}
		else
		{
			screenSize.x = c.pixelWidth;
			screenSize.y = c.pixelHeight;
			renderCamera = c;
			if (screenPlacer != null)
			{
				screenPlacer.SetCamera(renderCamera);
			}
			float distanceToPoint = plane.GetDistanceToPoint(base.transform.position);
			worldUnitsPerScreenPixel = Vector3.Distance(c.ScreenToWorldPoint(new Vector3(0f, 1f, distanceToPoint)), c.ScreenToWorldPoint(new Vector3(0f, 0f, distanceToPoint)));
			CalcSize();
		}
	}

	public virtual void Hide(bool tf)
	{
		m_hidden = tf;
		if (meshRenderer == null)
		{
			meshRenderer = (MeshRenderer)GetComponent(typeof(MeshRenderer));
		}
		meshRenderer.enabled = !tf;
	}

	public bool IsHidden()
	{
		return m_hidden;
	}

	public Vector3 GetInsertionPointPos(int charIndex)
	{
		if (spriteFont == null)
		{
			return Vector3.zero;
		}
		if (meshString.Length < 1)
		{
			float baseHeight = (float)spriteFont.BaseHeight * worldUnitsPerTexel;
			return base.transform.TransformPoint(GetStartPos_SingleLine(baseHeight, 0f).x, GetLineBaseline(1, 1), offsetZ);
		}
		int num = 1;
		if (charIndex >= displayString.Length)
		{
			num = 0;
		}
		int charLine;
		int lineStart;
		int lineEnd;
		int displayLineCount = GetDisplayLineCount(charIndex, out charLine, out lineStart, out lineEnd);
		charIndex = Mathf.Min(charIndex, displayString.Length - 1);
		if (charIndex < lineStart)
		{
			GetDisplayLineCount(charIndex - 1, out charLine, out lineStart, out lineEnd);
		}
		float num2 = spriteFont.GetWidth(displayString, lineStart, charIndex - num) * worldUnitsPerTexel * characterSpacing;
		switch (anchor)
		{
		case Anchor_Pos.Upper_Center:
		case Anchor_Pos.Middle_Center:
		case Anchor_Pos.Lower_Center:
			if (alignment == Alignment_Type.Left)
			{
				num2 -= totalWidth * 0.5f;
			}
			else if (alignment != Alignment_Type.Right)
			{
				num2 -= spriteFont.GetWidth(displayString, lineStart, lineEnd) * worldUnitsPerTexel * characterSpacing * 0.5f;
			}
			break;
		case Anchor_Pos.Upper_Left:
		case Anchor_Pos.Middle_Left:
		case Anchor_Pos.Lower_Left:
			if (alignment == Alignment_Type.Center)
			{
				num2 += totalWidth * 0.5f - spriteFont.GetWidth(displayString, lineStart, lineEnd) * worldUnitsPerTexel * characterSpacing * 0.5f;
			}
			else if (alignment == Alignment_Type.Right)
			{
				num2 += totalWidth - spriteFont.GetWidth(displayString, lineStart, lineEnd) * worldUnitsPerTexel * characterSpacing;
			}
			break;
		case Anchor_Pos.Upper_Right:
		case Anchor_Pos.Middle_Right:
		case Anchor_Pos.Lower_Right:
			num2 = ((alignment != Alignment_Type.Center) ? ((alignment != 0) ? (num2 + -1f * spriteFont.GetWidth(displayString, lineStart, lineEnd) * worldUnitsPerTexel * characterSpacing) : (num2 - totalWidth)) : (num2 + (totalWidth * -0.5f - spriteFont.GetWidth(displayString, lineStart, lineEnd) * worldUnitsPerTexel * characterSpacing * 0.5f)));
			break;
		}
		return base.transform.TransformPoint(num2, GetLineBaseline(displayLineCount, charLine), offsetZ);
	}

	public Vector3 GetNearestInsertionPointPos(Vector3 point, ref int insertionPt)
	{
		insertionPt = GetNearestInsertionPoint(point);
		return GetInsertionPointPos(insertionPt);
	}

	public int GetNearestInsertionPoint(Vector3 point)
	{
		point = base.transform.InverseTransformPoint(point);
		int displayLineCount = GetDisplayLineCount();
		int num = 0;
		if (displayLineCount > 1)
		{
			float num2 = float.PositiveInfinity;
			int num3 = 1;
			for (int i = 1; i <= displayLineCount; i++)
			{
				float f = point.y - (GetLineBaseline(displayLineCount, i) + BaseHeight * 0.5f);
				if (Mathf.Abs(f) < num2)
				{
					num2 = Mathf.Abs(f);
					num3 = i;
				}
			}
			int j = 0;
			int num4 = 1;
			for (; j < displayString.Length; j++)
			{
				if (num4 >= num3)
				{
					break;
				}
				if (displayString[j] == '\n')
				{
					num4++;
					num = j + 1;
				}
			}
		}
		int result = num;
		for (int k = num; k < displayString.Length && displayString[k] != '\n'; k++)
		{
			if (!char.IsWhiteSpace(displayString[k]))
			{
				result = k + 1;
				int num5 = k * 4;
				float num6 = vertices[num5].x + 0.5f * (vertices[num5 + 2].x - vertices[num5].x);
				if (num6 >= point.x)
				{
					result = k;
					break;
				}
			}
		}
		return result;
	}

	public Vector3[] GetVertices()
	{
		return mesh.vertices;
	}

	public Vector3 GetCenterPoint()
	{
		return new Vector3(topLeft.x + 0.5f * (bottomRight.x - topLeft.x), topLeft.y - 0.5f * (topLeft.y - bottomRight.y), offsetZ);
	}

	public float GetWidth(string s)
	{
		if (spriteFont == null)
		{
			return 0f;
		}
		return spriteFont.GetWidth(s) * worldUnitsPerTexel * characterSpacing;
	}

	public float GetScreenWidth(string s)
	{
		if (spriteFont == null)
		{
			return 0f;
		}
		if (renderCamera == null)
		{
			return 0f;
		}
		Plane plane = new Plane(renderCamera.transform.forward, renderCamera.transform.position);
		screenSize.x = renderCamera.pixelWidth;
		screenSize.y = renderCamera.pixelHeight;
		float distanceToPoint = plane.GetDistanceToPoint(base.transform.position);
		worldUnitsPerScreenPixel = Vector3.Distance(renderCamera.ScreenToWorldPoint(new Vector3(0f, 1f, distanceToPoint)), renderCamera.ScreenToWorldPoint(new Vector3(0f, 0f, distanceToPoint)));
		return GetWidth(s) / worldUnitsPerScreenPixel;
	}

	public void SetAnchor(Anchor_Pos a)
	{
		anchor = a;
		LayoutText();
	}

	public void SetAlignment(Alignment_Type a)
	{
		alignment = a;
		LayoutText();
	}

	public Vector2 PixelSpaceToUVSpace(Vector2 xy)
	{
		if (pixelsPerUV.x == 0f || pixelsPerUV.y == 0f)
		{
			return Vector2.zero;
		}
		return new Vector2(xy.x / pixelsPerUV.x, xy.y / pixelsPerUV.y);
	}

	public Vector2 PixelSpaceToUVSpace(int x, int y)
	{
		return PixelSpaceToUVSpace(new Vector2(x, y));
	}

	public Vector2 PixelCoordToUVCoord(Vector2 xy)
	{
		if (pixelsPerUV.x == 0f || pixelsPerUV.y == 0f)
		{
			return Vector2.zero;
		}
		return new Vector2(xy.x / (pixelsPerUV.x - 1f), 1f - xy.y / (pixelsPerUV.y - 1f));
	}

	public Vector2 PixelCoordToUVCoord(int x, int y)
	{
		return PixelCoordToUVCoord(new Vector2(x, y));
	}

	public virtual void DoMirror()
	{
		if (!Application.isPlaying)
		{
			if (screenSize.x == 0f || screenSize.y == 0f)
			{
				Start();
			}
			if (mirror == null)
			{
				mirror = new SpriteTextMirror();
				mirror.Mirror(this);
			}
			mirror.Validate(this);
			if (mirror.DidChange(this))
			{
				stringContentChanged = true;
				Init();
				meshString = string.Empty;
				ProcessString(text);
				CalcSize();
				mirror.Mirror(this);
			}
		}
	}

	public virtual void OnDrawGizmosSelected()
	{
		DoMirror();
	}

	public virtual void OnDrawGizmos()
	{
		DoMirror();
	}
}
