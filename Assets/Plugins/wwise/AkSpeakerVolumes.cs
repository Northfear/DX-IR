using System;
using System.Runtime.InteropServices;

public class AkSpeakerVolumes : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public float fFrontLeft
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkSpeakerVolumes_fFrontLeft_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkSpeakerVolumes_fFrontLeft_set(swigCPtr, value);
		}
	}

	public float fFrontRight
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkSpeakerVolumes_fFrontRight_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkSpeakerVolumes_fFrontRight_set(swigCPtr, value);
		}
	}

	internal AkSpeakerVolumes(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public AkSpeakerVolumes()
		: this(AkSoundEnginePINVOKE.CSharp_new_AkSpeakerVolumes(), true)
	{
	}

	internal static HandleRef getCPtr(AkSpeakerVolumes obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~AkSpeakerVolumes()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkSpeakerVolumes(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}
}
