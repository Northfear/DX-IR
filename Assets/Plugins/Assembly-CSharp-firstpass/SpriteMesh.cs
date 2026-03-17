using UnityEngine;

public class SpriteMesh : ISpriteMesh
{
	protected SpriteRoot m_sprite;

	protected MeshFilter meshFilter;

	protected MeshRenderer meshRenderer;

	protected Mesh m_mesh;

	protected Texture m_texture;

	protected Vector3[] m_vertices = new Vector3[4];

	protected Color[] m_colors = new Color[4];

	protected Vector2[] m_uvs = new Vector2[4];

	protected Vector2[] m_uvs2 = new Vector2[4];

	protected int[] m_faces = new int[6];

	protected bool m_useUV2;

	public virtual SpriteRoot sprite
	{
		get
		{
			return m_sprite;
		}
		set
		{
			m_sprite = value;
			if (m_sprite != null)
			{
				if (m_sprite.spriteMesh != this)
				{
					m_sprite.spriteMesh = this;
				}
				meshFilter = (MeshFilter)m_sprite.gameObject.GetComponent(typeof(MeshFilter));
				if (meshFilter == null)
				{
					meshFilter = (MeshFilter)m_sprite.gameObject.AddComponent(typeof(MeshFilter));
				}
				meshRenderer = (MeshRenderer)m_sprite.gameObject.GetComponent(typeof(MeshRenderer));
				if (meshRenderer == null)
				{
					meshRenderer = (MeshRenderer)m_sprite.gameObject.AddComponent(typeof(MeshRenderer));
				}
				m_mesh = meshFilter.sharedMesh;
				if (meshRenderer.sharedMaterial != null)
				{
					m_texture = meshRenderer.sharedMaterial.GetTexture("_MainTex");
				}
			}
		}
	}

	public virtual Texture texture
	{
		get
		{
			return m_texture;
		}
	}

	public virtual Material material
	{
		get
		{
			return meshRenderer.sharedMaterial;
		}
		set
		{
			meshRenderer.sharedMaterial = value;
			m_texture = meshRenderer.sharedMaterial.mainTexture;
			if (m_sprite != null && m_texture != null)
			{
				m_sprite.SetPixelToUV(m_texture);
			}
		}
	}

	public virtual Vector3[] vertices
	{
		get
		{
			return m_vertices;
		}
	}

	public virtual Vector2[] uvs
	{
		get
		{
			return m_uvs;
		}
	}

	public virtual Vector2[] uvs2
	{
		get
		{
			return m_uvs2;
		}
	}

	public virtual bool UseUV2
	{
		get
		{
			return m_useUV2;
		}
		set
		{
			m_useUV2 = value;
		}
	}

	public virtual Mesh mesh
	{
		get
		{
			if (m_mesh == null)
			{
				CreateMesh();
			}
			return m_mesh;
		}
		set
		{
			m_mesh = value;
		}
	}

	public virtual void Init()
	{
		if (m_mesh == null)
		{
			CreateMesh();
		}
		m_mesh.Clear();
		m_mesh.vertices = m_vertices;
		m_mesh.uv = m_uvs;
		m_mesh.colors = m_colors;
		m_mesh.triangles = m_faces;
		SetWindingOrder(m_sprite.winding);
		if (!m_sprite.uvsInitialized)
		{
			m_sprite.InitUVs();
			m_sprite.uvsInitialized = true;
		}
		m_sprite.SetBleedCompensation(m_sprite.bleedCompensation);
		if (m_sprite.pixelPerfect)
		{
			if (m_texture == null && meshRenderer.sharedMaterial != null)
			{
				m_texture = meshRenderer.sharedMaterial.GetTexture("_MainTex");
			}
			if (m_texture != null)
			{
				m_sprite.SetPixelToUV(m_texture);
			}
			if (m_sprite.renderCamera == null)
			{
				m_sprite.SetCamera(Camera.mainCamera);
			}
			else
			{
				m_sprite.SetCamera(m_sprite.renderCamera);
			}
		}
		else if (!m_sprite.hideAtStart)
		{
			m_sprite.SetSize(m_sprite.width, m_sprite.height);
		}
		m_mesh.RecalculateNormals();
		m_sprite.SetColor(m_sprite.color);
	}

	protected void CreateMesh()
	{
		meshFilter.sharedMesh = new Mesh();
		m_mesh = meshFilter.sharedMesh;
		if (m_sprite.persistent)
		{
			Object.DontDestroyOnLoad(m_mesh);
		}
	}

	public virtual void UpdateVerts()
	{
		m_mesh.vertices = m_vertices;
		m_mesh.RecalculateBounds();
	}

	public virtual void UpdateUVs()
	{
		if (m_mesh != null)
		{
			m_mesh.uv = m_uvs;
			if (m_useUV2)
			{
				m_mesh.uv2 = m_uvs2;
			}
		}
	}

	public virtual void UpdateColors(Color color)
	{
		m_colors[0] = color;
		m_colors[1] = color;
		m_colors[2] = color;
		m_colors[3] = color;
		if (m_mesh != null)
		{
			m_mesh.colors = m_colors;
		}
	}

	public virtual void Hide(bool tf)
	{
		if (!(meshRenderer == null))
		{
			meshRenderer.enabled = !tf;
		}
	}

	public virtual bool IsHidden()
	{
		if (meshRenderer == null)
		{
			return true;
		}
		return !meshRenderer.enabled;
	}

	public void SetPersistent()
	{
		if (m_mesh != null)
		{
			Object.DontDestroyOnLoad(m_mesh);
		}
	}

	public virtual void SetWindingOrder(SpriteRoot.WINDING_ORDER winding)
	{
		m_faces[0] = 0;
		m_faces[1] = 3;
		m_faces[2] = 1;
		m_faces[3] = 3;
		m_faces[4] = 2;
		m_faces[5] = 1;
		if (m_mesh != null)
		{
			m_mesh.triangles = m_faces;
		}
	}
}
