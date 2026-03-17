using System;
using System.Runtime.InteropServices;

public class Iterator : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public PlaylistItem pItem
	{
		get
		{
			IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_Iterator_pItem_get(swigCPtr);
			return (!(intPtr == IntPtr.Zero)) ? new PlaylistItem(intPtr, false) : null;
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_Iterator_pItem_set(swigCPtr, PlaylistItem.getCPtr(value));
		}
	}

	internal Iterator(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public Iterator()
		: this(AkSoundEnginePINVOKE.CSharp_new_Iterator(), true)
	{
	}

	internal static HandleRef getCPtr(Iterator obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~Iterator()
	{
		Dispose();
	}

	public virtual void Dispose()
	{
		lock (this)
		{
			if (swigCPtr.Handle != IntPtr.Zero)
			{
				if (swigCMemOwn)
				{
					swigCMemOwn = false;
					AkSoundEnginePINVOKE.CSharp_delete_Iterator(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}

	public Iterator NextIter()
	{
		return new Iterator(AkSoundEnginePINVOKE.CSharp_Iterator_NextIter(swigCPtr), false);
	}

	public Iterator PrevIter()
	{
		return new Iterator(AkSoundEnginePINVOKE.CSharp_Iterator_PrevIter(swigCPtr), false);
	}

	public PlaylistItem GetItem()
	{
		return new PlaylistItem(AkSoundEnginePINVOKE.CSharp_Iterator_GetItem(swigCPtr), false);
	}

	public bool IsEqualTo(Iterator in_rOp)
	{
		return AkSoundEnginePINVOKE.CSharp_Iterator_IsEqualTo(swigCPtr, getCPtr(in_rOp));
	}

	public bool IsDifferentFrom(Iterator in_rOp)
	{
		return AkSoundEnginePINVOKE.CSharp_Iterator_IsDifferentFrom(swigCPtr, getCPtr(in_rOp));
	}
}
