using System;
using System.Runtime.InteropServices;

public class AkThreadProperties : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public int nPriority
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkThreadProperties_nPriority_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkThreadProperties_nPriority_set(swigCPtr, value);
		}
	}

	public uint uStackSize
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkThreadProperties_uStackSize_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkThreadProperties_uStackSize_set(swigCPtr, value);
		}
	}

	public int uSchedPolicy
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkThreadProperties_uSchedPolicy_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkThreadProperties_uSchedPolicy_set(swigCPtr, value);
		}
	}

	internal AkThreadProperties(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public AkThreadProperties()
		: this(AkSoundEnginePINVOKE.CSharp_new_AkThreadProperties(), true)
	{
	}

	internal static HandleRef getCPtr(AkThreadProperties obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~AkThreadProperties()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkThreadProperties(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}
}
