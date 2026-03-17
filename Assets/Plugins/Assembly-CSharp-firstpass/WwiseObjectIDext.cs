using System;
using System.Runtime.InteropServices;

public class WwiseObjectIDext : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public uint id
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_id_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_id_set(swigCPtr, value);
		}
	}

	public bool bIsBus
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_bIsBus_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_bIsBus_set(swigCPtr, value);
		}
	}

	internal WwiseObjectIDext(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public WwiseObjectIDext()
		: this(AkSoundEnginePINVOKE.CSharp_new_WwiseObjectIDext(), true)
	{
	}

	internal static HandleRef getCPtr(WwiseObjectIDext obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~WwiseObjectIDext()
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
					AkSoundEnginePINVOKE.CSharp_delete_WwiseObjectIDext(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}

	public bool IsEqualTo(WwiseObjectIDext in_rOther)
	{
		return AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_IsEqualTo(swigCPtr, getCPtr(in_rOther));
	}

	public new AkNodeType GetType()
	{
		return (AkNodeType)AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_GetType(swigCPtr);
	}
}
