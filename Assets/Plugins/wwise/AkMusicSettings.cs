using System;
using System.Runtime.InteropServices;

public class AkMusicSettings : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public float fStreamingLookAheadRatio
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkMusicSettings_fStreamingLookAheadRatio_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkMusicSettings_fStreamingLookAheadRatio_set(swigCPtr, value);
		}
	}

	internal AkMusicSettings(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public AkMusicSettings()
		: this(AkSoundEnginePINVOKE.CSharp_new_AkMusicSettings(), true)
	{
	}

	internal static HandleRef getCPtr(AkMusicSettings obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~AkMusicSettings()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkMusicSettings(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}
}
