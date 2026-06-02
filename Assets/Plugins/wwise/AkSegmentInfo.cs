using System;
using System.Runtime.InteropServices;

public class AkSegmentInfo : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public int iCurrentPosition
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iCurrentPosition_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iCurrentPosition_set(swigCPtr, value);
		}
	}

	public int iPreEntryDuration
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iPreEntryDuration_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iPreEntryDuration_set(swigCPtr, value);
		}
	}

	public int iActiveDuration
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iActiveDuration_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iActiveDuration_set(swigCPtr, value);
		}
	}

	public int iPostExitDuration
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iPostExitDuration_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iPostExitDuration_set(swigCPtr, value);
		}
	}

	public int iRemainingLookAheadTime
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iRemainingLookAheadTime_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iRemainingLookAheadTime_set(swigCPtr, value);
		}
	}

	internal AkSegmentInfo(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public AkSegmentInfo()
		: this(AkSoundEnginePINVOKE.CSharp_new_AkSegmentInfo(), true)
	{
	}

	internal static HandleRef getCPtr(AkSegmentInfo obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~AkSegmentInfo()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkSegmentInfo(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}
}
