using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SpriteFont
{
	protected delegate void ParserDel(string line);

	protected const float bleedCompensation = 0f;

	public TextAsset fontDef;

	protected Dictionary<int, int> charMap = new Dictionary<int, int>();

	protected SpriteChar[] chars;

	protected Vector2 bleedCompUV;

	protected Vector2 bleedCompUVMax;

	protected int lineHeight;

	protected int baseHeight;

	protected int texWidth;

	protected int texHeight;

	protected string face;

	protected int pxSize;

	protected float charSpacing = 1f;

	private int kerningsCount;

	public int LineHeight
	{
		get
		{
			return lineHeight;
		}
		set
		{
			lineHeight = value;
		}
	}

	public int BaseHeight
	{
		get
		{
			return baseHeight;
		}
	}

	public int PixelSize
	{
		get
		{
			return pxSize;
		}
	}

	public float CharacterSpacing
	{
		get
		{
			return charSpacing;
		}
		set
		{
			float num = charSpacing;
			charSpacing = value;
			if (num == charSpacing || chars == null)
			{
				return;
			}
			for (int i = 0; i < chars.Length; i++)
			{
				if (chars[i] == null)
				{
					continue;
				}
				chars[i].xAdvance *= charSpacing;
				if (chars[i].kernings != null)
				{
					int[] array = new int[chars[i].kernings.Keys.Count];
					chars[i].kernings.Keys.CopyTo(array, 0);
					for (int j = 0; j < array.Length; j++)
					{
						chars[i].kernings[array[j]] = charSpacing * chars[i].origKernings[array[j]];
					}
				}
			}
		}
	}

	public SpriteFont(TextAsset def)
	{
		Load(def);
	}

	public void Load(TextAsset def)
	{
		if (def == null)
		{
			return;
		}
		int num = 0;
		fontDef = def;
		string[] array = fontDef.text.Split('\n');
		int pos = ParseSection("info", array, HeaderParser, 0);
		pos = ParseSection("common", array, CommonParser, pos);
		pos = ParseSection("chars count", array, CharCountParser, pos);
		bleedCompUV = new Vector2(0f / (float)texWidth, 0f / (float)texHeight);
		bleedCompUVMax = bleedCompUV * -2f;
		while (pos < array.Length && num < chars.Length && CharParser(array[pos++], num))
		{
			num++;
		}
		pos--;
		pos = ParseSection("kernings count", array, KerningCountParser, pos);
		num = 0;
		while (pos < array.Length && num < kerningsCount)
		{
			if (KerningParser(array[pos++]))
			{
				num++;
			}
		}
		float characterSpacing = charSpacing;
		charSpacing = 0f;
		CharacterSpacing = characterSpacing;
	}

	private int ParseSection(string tag, string[] lines, ParserDel parser, int pos)
	{
		while (pos < lines.Length)
		{
			string text = lines[pos].Trim();
			if (text.Length >= 1 && text.StartsWith(tag))
			{
				parser(text);
				return ++pos;
			}
			pos++;
		}
		return pos;
	}

	private int FindField(string label, string[] fields, int pos, bool logError)
	{
		while (pos < fields.Length)
		{
			if (label == fields[pos].Trim())
			{
				return pos;
			}
			pos++;
		}
		if (logError)
		{
			Debug.LogError("Missing \"" + label + "\" field in font definition file \"" + fontDef.name + "\". Please check the file or re-create it.");
			return pos;
		}
		return -1;
	}

	private int FindField(string label, string[] fields, int pos)
	{
		return FindField(label, fields, pos, true);
	}

	private int FindFieldOptional(string label, string[] fields, int pos)
	{
		return FindField(label, fields, pos, false);
	}

	private void HeaderParser(string line)
	{
		string[] array = line.Split(' ', '=');
		int num = FindField("face", array, 1);
		face = array[num + 1].Trim('"');
		num = FindField("size", array, num);
		pxSize = Mathf.Abs(int.Parse(array[num + 1]));
		num = FindFieldOptional("charSpacing", array, num);
		if (num != -1)
		{
			charSpacing = Mathf.Abs(float.Parse(array[num + 1]));
		}
	}

	private void CommonParser(string line)
	{
		string[] array = line.Split(' ', '=');
		int num = FindField("lineHeight", array, 1);
		lineHeight = int.Parse(array[num + 1]);
		num = FindField("base", array, num);
		baseHeight = int.Parse(array[num + 1]);
		num = FindField("scaleW", array, num);
		texWidth = int.Parse(array[num + 1]);
		num = FindField("scaleH", array, num);
		texHeight = int.Parse(array[num + 1]);
		num = FindField("pages", array, num);
		if (int.Parse(array[num + 1]) > 1)
		{
			Debug.LogError("Multiple pages/textures detected for font \"" + face + "\". only one font atlas is supported.");
		}
	}

	private void CharCountParser(string line)
	{
		string[] array = line.Split('=');
		if (array.Length < 2)
		{
			Debug.LogError("Malformed \"chars count\" line in font definition file \"" + fontDef.name + "\". Please check the file or re-create it.");
		}
		else
		{
			chars = new SpriteChar[int.Parse(array[1]) + 1];
		}
	}

	private bool CharParser(string line, int charNum)
	{
		if (!line.StartsWith("char"))
		{
			return false;
		}
		string[] array = line.Split(' ', '=');
		int num = FindField("id", array, 1);
		chars[charNum] = new SpriteChar();
		chars[charNum].id = int.Parse(array[num + 1]);
		num = FindField("x", array, num);
		float num2 = float.Parse(array[num + 1]) / (float)texWidth;
		num = FindField("y", array, num);
		float num3 = 1f - float.Parse(array[num + 1]) / (float)texHeight;
		num = FindField("width", array, num);
		float num4 = float.Parse(array[num + 1]) / (float)texWidth;
		num = FindField("height", array, num);
		float num5 = float.Parse(array[num + 1]) / (float)texHeight;
		num = FindField("xoffset", array, num);
		chars[charNum].xOffset = float.Parse(array[num + 1]);
		num = FindField("yoffset", array, num);
		chars[charNum].yOffset = 0f - float.Parse(array[num + 1]);
		num = FindField("xadvance", array, num);
		chars[charNum].xAdvance = int.Parse(array[num + 1]);
		chars[charNum].UVs.x = num2 + bleedCompUV.x;
		chars[charNum].UVs.y = num3 - num5 + bleedCompUV.y;
		chars[charNum].UVs.xMax = num2 + num4 + bleedCompUVMax.x;
		chars[charNum].UVs.yMax = num3 + bleedCompUVMax.y;
		charMap.Add(Convert.ToChar(chars[charNum].id), charNum);
		return true;
	}

	private void KerningCountParser(string line)
	{
		string[] array = line.Split('=');
		kerningsCount = int.Parse(array[1]);
	}

	private bool KerningParser(string line)
	{
		if (!line.StartsWith("kerning"))
		{
			return false;
		}
		string[] array = line.Split(' ', '=');
		int num = FindField("first", array, 1);
		int value = int.Parse(array[num + 1]);
		num = FindField("second", array, num);
		int value2 = int.Parse(array[num + 1]);
		num = FindField("amount", array, num);
		int num2 = int.Parse(array[num + 1]);
		SpriteChar spriteChar = GetSpriteChar(Convert.ToChar(value2));
		if (spriteChar == null)
		{
			return true;
		}
		if (spriteChar.kernings == null)
		{
			spriteChar.kernings = new Dictionary<int, float>();
			spriteChar.origKernings = new Dictionary<int, float>();
		}
		spriteChar.kernings.Add(Convert.ToChar(value), num2);
		spriteChar.origKernings.Add(Convert.ToChar(value), num2);
		return true;
	}

	public SpriteChar GetSpriteChar(char ch)
	{
		int value;
		if (!charMap.TryGetValue(ch, out value))
		{
			return null;
		}
		return chars[value];
	}

	public bool ContainsCharacter(char ch)
	{
		return charMap.ContainsKey(ch);
	}

	public float GetWidth(string str)
	{
		float num = 0f;
		if (str.Length < 1)
		{
			return 0f;
		}
		SpriteChar spriteChar = GetSpriteChar(str[0]);
		if (spriteChar != null)
		{
			num = spriteChar.xAdvance;
		}
		for (int i = 1; i < str.Length; i++)
		{
			spriteChar = GetSpriteChar(str[i]);
			if (spriteChar != null)
			{
				num += spriteChar.xAdvance + spriteChar.GetKerning(str[i - 1]);
			}
		}
		return num;
	}

	public float GetWidth(string str, int start, int end)
	{
		float num = 0f;
		if (start >= str.Length || end < start)
		{
			return 0f;
		}
		end = Mathf.Clamp(end, 0, str.Length - 1);
		SpriteChar spriteChar = GetSpriteChar(str[start]);
		if (spriteChar != null)
		{
			num = spriteChar.xAdvance;
		}
		for (int i = start + 1; i <= end; i++)
		{
			spriteChar = GetSpriteChar(str[i]);
			if (spriteChar != null)
			{
				num += spriteChar.xAdvance + spriteChar.GetKerning(str[i - 1]);
			}
		}
		return num;
	}

	public float GetWidth(StringBuilder sb, int start, int end)
	{
		float num = 0f;
		if (start >= sb.Length || end < start)
		{
			return 0f;
		}
		end = Mathf.Clamp(end, 0, sb.Length - 1);
		SpriteChar spriteChar = GetSpriteChar(sb[start]);
		if (spriteChar != null)
		{
			num = spriteChar.xAdvance;
		}
		for (int i = start + 1; i <= end; i++)
		{
			spriteChar = GetSpriteChar(sb[i]);
			if (spriteChar != null)
			{
				num += spriteChar.xAdvance + spriteChar.GetKerning(sb[i - 1]);
			}
		}
		return num;
	}

	public float GetWidth(char prevChar, char c)
	{
		SpriteChar spriteChar = GetSpriteChar(c);
		if (spriteChar == null)
		{
			return 0f;
		}
		return spriteChar.xAdvance + spriteChar.GetKerning(prevChar);
	}

	public float GetAdvance(char c)
	{
		SpriteChar spriteChar = GetSpriteChar(c);
		if (spriteChar == null)
		{
			return 0f;
		}
		return spriteChar.xAdvance;
	}

	public string RemoveUnsupportedCharacters(string str)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < str.Length; i++)
		{
			if (charMap.ContainsKey(str[i]) || str[i] == '\n' || str[i] == '\t' || str[i] == '#' || str[i] == '[' || str[i] == ']' || str[i] == '(' || str[i] == ')' || str[i] == ',')
			{
				stringBuilder.Append(str[i]);
			}
		}
		return stringBuilder.ToString();
	}
}
