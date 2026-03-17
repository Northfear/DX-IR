using System;
using System.Runtime.InteropServices;

public class AkSoundPosition : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public AkVector Position
	{
		get
		{
			IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkSoundPosition_Position_get(swigCPtr);
			return (!(intPtr == IntPtr.Zero)) ? new AkVector(intPtr, false) : null;
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkSoundPosition_Position_set(swigCPtr, AkVector.getCPtr(value));
		}
	}

	public AkVector Orientation
	{
		get
		{
			IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkSoundPosition_Orientation_get(swigCPtr);
			return (!(intPtr == IntPtr.Zero)) ? new AkVector(intPtr, false) : null;
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkSoundPosition_Orientation_set(swigCPtr, AkVector.getCPtr(value));
		}
	}

	internal AkSoundPosition(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public AkSoundPosition()
		: this(AkSoundEnginePINVOKE.CSharp_new_AkSoundPosition(), true)
	{
	}

	internal static HandleRef getCPtr(AkSoundPosition obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~AkSoundPosition()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkSoundPosition(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}
}
