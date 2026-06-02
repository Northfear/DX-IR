using UnityEngine;

public interface IBorderedTilingSprite
{
	Rect UnclippedUVRect { get; }

	float SizePerPixel { get; }

	bool StretchVertically { get; set; }

	bool StretchHorizontally { get; set; }

	Vector2 TopLeftBorder { get; set; }

	Vector2 BottomRightBorder { get; set; }

	Vector2 TilingOffset { get; set; }

	bool HasCenter { get; set; }

	Vector2 SizePerTexel { get; set; }

	bool PixelPerTexel { get; set; }
}
