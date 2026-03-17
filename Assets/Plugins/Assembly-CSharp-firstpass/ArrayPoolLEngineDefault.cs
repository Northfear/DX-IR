using System;
using System.Runtime.InteropServices;

public class ArrayPoolLEngineDefault : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	internal ArrayPoolLEngineDefault(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public ArrayPoolLEngineDefault()
		: this(AkSoundEnginePINVOKE.CSharp_new_ArrayPoolLEngineDefault(), true)
	{
	}

	internal static HandleRef getCPtr(ArrayPoolLEngineDefault obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~ArrayPoolLEngineDefault()
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
					AkSoundEnginePINVOKE.CSharp_delete_ArrayPoolLEngineDefault(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}

	public static int Get()
	{
		return AkSoundEnginePINVOKE.CSharp_ArrayPoolLEngineDefault_Get();
	}
}
