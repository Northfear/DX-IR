using UnityEngine;

public interface IUIListObject : IEZDragDrop, IUIObject
{
	Vector2 TopLeftEdge { get; }

	Vector2 BottomRightEdge { get; }

	bool Managed { get; }

	Rect3D ClippingRect { get; set; }

	bool Clipped { get; set; }

	int Index { get; set; }

	string Text { get; set; }

	SpriteText TextObj { get; }

	bool selected { get; set; }

	Camera RenderCamera { get; set; }

	bool IsContainer();

	void FindOuterEdges();

	void Hide(bool tf);

	void Unclip();

	void UpdateCollider();

	void SetList(UIScrollList c);

	UIScrollList GetScrollList();

	void Delete();

	void UpdateCamera();
}
