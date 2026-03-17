using System;
using System.Runtime.InteropServices;

public class ArrayPoolDefault : IDisposable
{
	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	internal ArrayPoolDefault(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = new HandleRef(this, cPtr);
	}

	public ArrayPoolDefault()
		: this(AkSoundEnginePINVOKE.CSharp_new_ArrayPoolDefault(), true)
	{
	}

	internal static HandleRef getCPtr(ArrayPoolDefault obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	~ArrayPoolDefault()
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
					AkSoundEnginePINVOKE.CSharp_delete_ArrayPoolDefault(swigCPtr);
				}
				swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			GC.SuppressFinalize(this);
		}
	}

	public static int Get()
	{
		return AkSoundEnginePINVOKE.CSharp_ArrayPoolDefault_Get();
	}
}
