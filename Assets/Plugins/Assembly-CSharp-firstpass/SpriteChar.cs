using System.Collections.Generic;
using UnityEngine;

public class SpriteChar
{
	public int id;

	public Rect UVs;

	public float xOffset;

	public float yOffset;

	public float xAdvance;

	public Dictionary<int, float> kernings;

	public Dictionary<int, float> origKernings;

	public float GetKerning(int prevChar)
	{
		if (kernings == null)
		{
			return 0f;
		}
		float value = 0f;
		kernings.TryGetValue(prevChar, out value);
		return value;
	}
}
