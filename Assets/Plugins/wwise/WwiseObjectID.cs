using System;
using System.Runtime.InteropServices;

public class WwiseObjectID : WwiseObjectIDext
{
	private HandleRef swigCPtr;

	internal WwiseObjectID(IntPtr cPtr, bool cMemoryOwn)
		: base(AkSoundEnginePINVOKE.CSharp_WwiseObjectID_SWIGUpcast(cPtr), cMemoryOwn)
	{
		swigCPtr = new HandleRef(this, cPtr);
	}

	public WwiseObjectID()
		: this(AkSoundEnginePINVOKE.CSharp_new_WwiseObjectID__SWIG_0(), true)
	{
	}

	public WwiseObjectID(uint in_ID)
		: this(AkSoundEnginePINVOKE.CSharp_new_WwiseObjectID__SWIG_1(in_ID), true)
	{
	}

	public WwiseObjectID(uint in_ID, bool in_bIsBus)
		: this(AkSoundEnginePINVOKE.CSharp_new_WwiseObjectID__SWIG_2(in_ID, in_bIsBus), true)
	{
	}

	public WwiseObjectID(uint in_ID, AkNodeType in_eNodeType)
		: this(AkSoundEnginePINVOKE.CSharp_new_WwiseObjectID__SWIG_3(in_ID, (int)in_eNodeType), true)
	{
	}

	internal static HandleRef getCPtr(WwiseObjectID obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~WwiseObjectID()
	{
		Dispose();
	}

	public override void Dispose()
	{
		lock (this)
		{
			if (swigCPtr.Handle != IntPtr.Zero)
			{
				if (swigCMemOwn)
				{
					swigCMemOwn = false;
					AkSoundEnginePINVOKE.CSharp_delete_WwiseObjectID(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
			base.Dispose();
		}
	}
}
