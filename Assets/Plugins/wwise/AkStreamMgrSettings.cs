using System;
using System.Runtime.InteropServices;

public class AkStreamMgrSettings : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public uint uMemorySize
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkStreamMgrSettings_uMemorySize_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkStreamMgrSettings_uMemorySize_set(swigCPtr, value);
		}
	}

	internal AkStreamMgrSettings(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public AkStreamMgrSettings()
		: this(AkSoundEnginePINVOKE.CSharp_new_AkStreamMgrSettings(), true)
	{
	}

	internal static HandleRef getCPtr(AkStreamMgrSettings obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~AkStreamMgrSettings()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkStreamMgrSettings(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}
}
