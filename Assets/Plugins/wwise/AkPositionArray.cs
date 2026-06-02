using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AkPositionArray : IDisposable
{
	public IntPtr m_Buffer;

	private IntPtr m_Current;

	private uint m_MaxCount;

	private uint m_Count;

	public AkPositionArray(uint in_Count)
	{
		m_Buffer = Marshal.AllocHGlobal((int)(in_Count * 4 * 6));
		m_Current = m_Buffer;
		m_MaxCount = in_Count;
		m_Count = 0u;
	}

	~AkPositionArray()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (m_Buffer != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(m_Buffer);
			m_Buffer = IntPtr.Zero;
			m_MaxCount = 0u;
		}
	}

	public void Reset()
	{
		m_Current = m_Buffer;
		m_Count = 0u;
	}

	public void Add(Vector3 in_Pos, Vector3 in_Forward)
	{
		if (m_Count >= m_MaxCount)
		{
			throw new IndexOutOfRangeException("Out of range access in AkPositionArray");
		}
		Marshal.WriteInt32(m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_Pos.x), 0));
		m_Current = (IntPtr)(m_Current.ToInt64() + 4);
		Marshal.WriteInt32(m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_Pos.y), 0));
		m_Current = (IntPtr)(m_Current.ToInt64() + 4);
		Marshal.WriteInt32(m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_Pos.z), 0));
		m_Current = (IntPtr)(m_Current.ToInt64() + 4);
		Marshal.WriteInt32(m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_Forward.x), 0));
		m_Current = (IntPtr)(m_Current.ToInt64() + 4);
		Marshal.WriteInt32(m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_Forward.y), 0));
		m_Current = (IntPtr)(m_Current.ToInt64() + 4);
		Marshal.WriteInt32(m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_Forward.z), 0));
		m_Current = (IntPtr)(m_Current.ToInt64() + 4);
		m_Count++;
	}
}
