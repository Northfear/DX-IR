using UnityEngine;

public interface IEZDragDrop
{
	object Data { get; set; }

	bool IsDraggable { get; set; }

	LayerMask DropMask { get; set; }

	bool IsDragging { get; set; }

	GameObject DropTarget { get; set; }

	bool DropHandled { get; set; }

	float DragOffset { get; set; }

	EZAnimation.EASING_TYPE CancelDragEasing { get; set; }

	float CancelDragDuration { get; set; }

	void DragUpdatePosition(POINTER_INFO ptr);

	void CancelDrag();

	void OnEZDragDrop_Internal(EZDragDropParams parms);

	void AddDragDropDelegate(EZDragDropDelegate del);

	void RemoveDragDropDelegate(EZDragDropDelegate del);

	void SetDragDropDelegate(EZDragDropDelegate del);

	void SetDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del);

	void AddDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del);

	void RemoveDragDropInternalDelegate(EZDragDropHelper.DragDrop_InternalDelegate del);

	EZDragDropHelper.DragDrop_InternalDelegate GetDragDropInternalDelegate();
}
