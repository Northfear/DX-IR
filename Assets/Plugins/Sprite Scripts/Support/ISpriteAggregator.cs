using UnityEngine;

public interface ISpriteAggregator
{
	Texture2D[] SourceTextures { get; }

	CSpriteFrame[] SpriteFrames { get; }

	CSpriteFrame DefaultFrame { get; }

	GameObject gameObject { get; }

	bool DoNotTrimImages { get; }

	void Aggregate(PathFromGUIDDelegate guid2Path, LoadAssetDelegate load, GUIDFromPathDelegate path2Guid);

	Material GetPackedMaterial(out string errString);

	void SetUVs(Rect uvs);
}
