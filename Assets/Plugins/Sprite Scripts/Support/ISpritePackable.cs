using UnityEngine;

public interface ISpritePackable
{
	GameObject gameObject { get; }

	TextureAnim[] States { get; set; }

	SpriteRoot.ANCHOR_METHOD Anchor { get; }

	Color Color { get; set; }

	bool SupportsArbitraryAnimations { get; }
}
