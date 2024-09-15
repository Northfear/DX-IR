using UnityEngine;

public struct Rect3D
{
	private Vector3 m_tl;

	private Vector3 m_tr;

	private Vector3 m_bl;

	private Vector3 m_br;

	private float m_width;

	private float m_height;

	public Vector3 topLeft
	{
		get
		{
			return m_tl;
		}
	}

	public Vector3 topRight
	{
		get
		{
			return m_tr;
		}
	}

	public Vector3 bottomLeft
	{
		get
		{
			return m_bl;
		}
	}

	public Vector3 bottomRight
	{
		get
		{
			return m_br;
		}
	}

	public float width
	{
		get
		{
			if (float.IsNaN(m_width))
			{
				m_width = Vector3.Distance(m_tr, m_tl);
			}
			return m_width;
		}
	}

	public float height
	{
		get
		{
			if (float.IsNaN(m_height))
			{
				m_height = Vector3.Distance(m_tl, m_bl);
			}
			return m_height;
		}
	}

	public Rect3D(Vector3 tl, Vector3 tr, Vector3 bl)
	{
		m_tl = (m_tr = (m_bl = (m_br = Vector3.zero)));
		m_width = (m_height = 0f);
		FromPoints(tl, tr, bl);
	}

	public Rect3D(Rect r)
	{
		m_tl = (m_tr = (m_bl = (m_br = Vector3.zero)));
		m_width = (m_height = 0f);
		FromRect(r);
	}

	public void FromPoints(Vector3 tl, Vector3 tr, Vector3 bl)
	{
		m_tl = tl;
		m_tr = tr;
		m_bl = bl;
		m_br = tr + (bl - tl);
		m_width = (m_height = float.NaN);
	}

	public Rect GetRect()
	{
		return Rect.MinMaxRect(m_bl.x, m_bl.y, m_tr.x, m_tl.y);
	}

	public void FromRect(Rect r)
	{
		FromPoints(new Vector3(r.xMin, r.yMax, 0f), new Vector3(r.xMax, r.yMax, 0f), new Vector3(r.xMin, r.yMin, 0f));
	}

	public void MultFast(Matrix4x4 matrix)
	{
		m_tl = matrix.MultiplyPoint3x4(m_tl);
		m_tr = matrix.MultiplyPoint3x4(m_tr);
		m_bl = matrix.MultiplyPoint3x4(m_bl);
		m_br = matrix.MultiplyPoint3x4(m_br);
		m_width = (m_height = float.NaN);
	}

	public static Rect3D MultFast(Rect3D rect, Matrix4x4 matrix)
	{
		return new Rect3D(matrix.MultiplyPoint3x4(rect.m_tl), matrix.MultiplyPoint3x4(rect.m_tr), matrix.MultiplyPoint3x4(rect.m_bl));
	}
}
