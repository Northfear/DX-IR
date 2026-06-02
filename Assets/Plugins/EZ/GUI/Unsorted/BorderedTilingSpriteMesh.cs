using System.Collections.Generic;
using UnityEngine;

internal class BorderedTilingSpriteMesh : SpriteMesh
{
	private IBorderedTilingSprite _btSprite;

	private List<float> _hTilePoints = new List<float>();

	private List<float> _vTilePoints = new List<float>();

	private List<Vector3> _vertices = new List<Vector3>();

	private List<Vector2> _uvs = new List<Vector2>();

	private Color[] _colors;

	private List<Vector2> _uv2s = new List<Vector2>();

	private List<int> _indices = new List<int>();

	private Vector2 _sizePerUV;

	public override SpriteRoot sprite
	{
		get
		{
			return base.sprite;
		}
		set
		{
			base.sprite = value;
			_btSprite = value as IBorderedTilingSprite;
		}
	}

	private void RebuildMesh()
	{
		if (m_mesh == null)
		{
			CreateMesh();
		}
		m_mesh.Clear();
		if (m_texture == null)
		{
			return;
		}
		Rect clipRect = default(Rect);
		switch (m_sprite.plane)
		{
		case SpriteRoot.SPRITE_PLANE.XY:
			clipRect = Rect.MinMaxRect(m_sprite.TopLeft.x, m_sprite.BottomRight.y, m_sprite.BottomRight.x, m_sprite.TopLeft.y);
			break;
		case SpriteRoot.SPRITE_PLANE.YZ:
			clipRect = Rect.MinMaxRect(m_sprite.TopLeft.z, m_sprite.BottomRight.y, m_sprite.BottomRight.z, m_sprite.TopLeft.y);
			break;
		case SpriteRoot.SPRITE_PLANE.XZ:
			clipRect = Rect.MinMaxRect(m_sprite.TopLeft.x, m_sprite.BottomRight.z, m_sprite.BottomRight.x, m_sprite.TopLeft.z);
			break;
		}
		if (clipRect.width <= 0f || clipRect.height <= 0f)
		{
			_colors = null;
			return;
		}
		_hTilePoints.Clear();
		_vTilePoints.Clear();
		_vertices.Clear();
		_uvs.Clear();
		_uv2s.Clear();
		_indices.Clear();
		Rect unclippedUVRect = _btSprite.UnclippedUVRect;
		Vector2 topLeftBorder = _btSprite.TopLeftBorder;
		Vector2 bottomRightBorder = _btSprite.BottomRightBorder;
		topLeftBorder.x /= m_texture.width;
		topLeftBorder.y /= m_texture.height;
		bottomRightBorder.x /= m_texture.width;
		bottomRightBorder.y /= m_texture.height;
		if (_btSprite.PixelPerTexel)
		{
			_btSprite.SizePerTexel = new Vector2(_btSprite.SizePerPixel, _btSprite.SizePerPixel);
		}
		_sizePerUV = _btSprite.SizePerTexel;
		_sizePerUV.x *= m_texture.width;
		_sizePerUV.y *= m_texture.height;
		Rect rect = Rect.MinMaxRect(m_sprite.UnclippedTopLeft.x, m_sprite.UnclippedBottomRight.y, m_sprite.UnclippedBottomRight.x, m_sprite.UnclippedTopLeft.y);
		Rect rect2 = Rect.MinMaxRect(rect.x + _sizePerUV.x * topLeftBorder.x, rect.y + _sizePerUV.y * bottomRightBorder.y, rect.xMax - _sizePerUV.x * bottomRightBorder.x, rect.yMax - _sizePerUV.y * topLeftBorder.y);
		Vector2 vector = new Vector2(unclippedUVRect.width - topLeftBorder.x - bottomRightBorder.x, unclippedUVRect.height - bottomRightBorder.y - topLeftBorder.y);
		Vector2 vector2 = new Vector2(_sizePerUV.x * vector.x, _sizePerUV.y * vector.y);
		if (!_btSprite.StretchHorizontally)
		{
			float num = rect2.x - _sizePerUV.x * (_btSprite.TilingOffset.x / (float)m_texture.width) % vector2.x;
			_hTilePoints.Add(num);
			do
			{
				num += vector2.x;
				_hTilePoints.Add(num);
			}
			while (num < rect2.xMax);
		}
		else
		{
			_hTilePoints.Add(rect2.x);
			_hTilePoints.Add(rect2.xMax);
		}
		if (!_btSprite.StretchVertically)
		{
			float num2 = rect2.y - _sizePerUV.y * (_btSprite.TilingOffset.y / (float)m_texture.height) % vector2.y;
			_vTilePoints.Add(num2);
			do
			{
				num2 += vector2.y;
				_vTilePoints.Add(num2);
			}
			while (num2 < rect2.yMax);
		}
		else
		{
			_vTilePoints.Add(rect2.y);
			_vTilePoints.Add(rect2.yMax);
		}
		ClipAndAddQuad(Rect.MinMaxRect(rect.x, rect.y, rect2.x, rect2.y), new Rect(unclippedUVRect.x, unclippedUVRect.y, topLeftBorder.x, bottomRightBorder.y), clipRect);
		ClipAndAddQuad(Rect.MinMaxRect(rect.x, rect2.yMax, rect2.x, rect.yMax), new Rect(unclippedUVRect.x, unclippedUVRect.yMax - topLeftBorder.y, topLeftBorder.x, topLeftBorder.y), clipRect);
		ClipAndAddQuad(Rect.MinMaxRect(rect2.xMax, rect2.yMax, rect.xMax, rect.yMax), new Rect(unclippedUVRect.xMax - bottomRightBorder.x, unclippedUVRect.yMax - topLeftBorder.y, bottomRightBorder.x, topLeftBorder.y), clipRect);
		ClipAndAddQuad(Rect.MinMaxRect(rect2.xMax, rect.y, rect.xMax, rect2.y), new Rect(unclippedUVRect.xMax - bottomRightBorder.x, unclippedUVRect.y, bottomRightBorder.x, bottomRightBorder.y), clipRect);
		if (topLeftBorder.x > 0f)
		{
			Rect clipRect2 = Rect.MinMaxRect(Mathf.Clamp(rect.x, clipRect.x, clipRect.xMax), Mathf.Clamp(rect2.y, clipRect.y, clipRect.yMax), Mathf.Clamp(rect2.x, clipRect.x, clipRect.xMax), Mathf.Clamp(rect2.yMax, clipRect.y, clipRect.yMax));
			for (int i = 0; i < _vTilePoints.Count - 1; i++)
			{
				ClipAndAddQuad(Rect.MinMaxRect(rect.x, _vTilePoints[i], rect2.x, _vTilePoints[i + 1]), new Rect(unclippedUVRect.x, unclippedUVRect.y + bottomRightBorder.y, topLeftBorder.x, vector.y), clipRect2);
			}
		}
		if (topLeftBorder.y > 0f)
		{
			Rect clipRect2 = Rect.MinMaxRect(Mathf.Clamp(rect2.x, clipRect.x, clipRect.xMax), Mathf.Clamp(rect2.yMax, clipRect.y, clipRect.yMax), Mathf.Clamp(rect2.xMax, clipRect.x, clipRect.xMax), Mathf.Clamp(rect.yMax, clipRect.y, clipRect.yMax));
			for (int j = 0; j < _hTilePoints.Count - 1; j++)
			{
				ClipAndAddQuad(Rect.MinMaxRect(_hTilePoints[j], rect2.yMax, _hTilePoints[j + 1], rect.yMax), new Rect(unclippedUVRect.x + topLeftBorder.x, unclippedUVRect.yMax - topLeftBorder.y, vector.x, topLeftBorder.y), clipRect2);
			}
		}
		if (bottomRightBorder.x > 0f)
		{
			Rect clipRect2 = Rect.MinMaxRect(Mathf.Clamp(rect2.xMax, clipRect.x, clipRect.xMax), Mathf.Clamp(rect2.y, clipRect.y, clipRect.yMax), Mathf.Clamp(rect.xMax, clipRect.x, clipRect.xMax), Mathf.Clamp(rect2.yMax, clipRect.y, clipRect.yMax));
			for (int k = 0; k < _vTilePoints.Count - 1; k++)
			{
				ClipAndAddQuad(Rect.MinMaxRect(rect2.xMax, _vTilePoints[k], rect.xMax, _vTilePoints[k + 1]), new Rect(unclippedUVRect.xMax - bottomRightBorder.x, unclippedUVRect.y + bottomRightBorder.y, bottomRightBorder.x, vector.y), clipRect2);
			}
		}
		if (bottomRightBorder.y > 0f)
		{
			Rect clipRect2 = Rect.MinMaxRect(Mathf.Clamp(rect2.x, clipRect.x, clipRect.xMax), Mathf.Clamp(rect.y, clipRect.y, clipRect.yMax), Mathf.Clamp(rect2.xMax, clipRect.x, clipRect.xMax), Mathf.Clamp(rect2.y, clipRect.y, clipRect.yMax));
			for (int l = 0; l < _hTilePoints.Count - 1; l++)
			{
				ClipAndAddQuad(Rect.MinMaxRect(_hTilePoints[l], rect.y, _hTilePoints[l + 1], rect2.y), new Rect(unclippedUVRect.x + topLeftBorder.x, unclippedUVRect.y, vector.x, bottomRightBorder.y), clipRect2);
			}
		}
		if (_btSprite.HasCenter)
		{
			Rect clipRect3 = Rect.MinMaxRect(Mathf.Clamp(rect2.x, clipRect.x, clipRect.xMax), Mathf.Clamp(rect2.y, clipRect.y, clipRect.yMax), Mathf.Clamp(rect2.xMax, clipRect.x, clipRect.xMax), Mathf.Clamp(rect2.yMax, clipRect.y, clipRect.yMax));
			for (int m = 0; m < _hTilePoints.Count - 1; m++)
			{
				for (int n = 0; n < _vTilePoints.Count - 1; n++)
				{
					ClipAndAddQuad(Rect.MinMaxRect(_hTilePoints[m], _vTilePoints[n], _hTilePoints[m + 1], _vTilePoints[n + 1]), new Rect(unclippedUVRect.x + topLeftBorder.x, unclippedUVRect.y + bottomRightBorder.y, vector.x, vector.y), clipRect3);
				}
			}
		}
		_colors = new Color[_vertices.Count];
		for (int num3 = 0; num3 < _colors.Length; num3++)
		{
			_colors[num3] = m_sprite.color;
		}
		m_mesh.vertices = _vertices.ToArray();
		m_mesh.uv = _uvs.ToArray();
		m_mesh.colors = _colors;
		m_mesh.triangles = _indices.ToArray();
		m_mesh.RecalculateBounds();
		if (_vertices.Count >= 300)
		{
			Debug.LogWarning("BorderedTilingSpriteMesh: Mesh has 300 or more vertices and might not be dynamically batched!");
		}
	}

	private void ClipAndAddQuad(Rect posRect, Rect texRect, Rect clipRect)
	{
		Rect rect = Rect.MinMaxRect(Mathf.Clamp(posRect.x, clipRect.x, clipRect.xMax), Mathf.Clamp(posRect.y, clipRect.y, clipRect.yMax), Mathf.Clamp(posRect.xMax, clipRect.x, clipRect.xMax), Mathf.Clamp(posRect.yMax, clipRect.y, clipRect.yMax));
		if (!(rect.width <= 0f) && !(rect.height <= 0f))
		{
			Vector2 vector = new Vector2(texRect.width / posRect.width, texRect.height / posRect.height);
			texRect = Rect.MinMaxRect(texRect.x + (rect.x - posRect.x) * vector.x, texRect.y + (rect.y - posRect.y) * vector.y, texRect.xMax - (posRect.xMax - rect.xMax) * vector.x, texRect.yMax - (posRect.yMax - rect.yMax) * vector.y);
			int count = _vertices.Count;
			switch (m_sprite.plane)
			{
			case SpriteRoot.SPRITE_PLANE.XY:
				_vertices.Add(new Vector3(rect.x, rect.yMax, m_sprite.offset.z));
				_vertices.Add(new Vector3(rect.x, rect.y, m_sprite.offset.z));
				_vertices.Add(new Vector3(rect.xMax, rect.y, m_sprite.offset.z));
				_vertices.Add(new Vector3(rect.xMax, rect.yMax, m_sprite.offset.z));
				break;
			case SpriteRoot.SPRITE_PLANE.XZ:
				_vertices.Add(new Vector3(rect.x, m_sprite.offset.y, rect.yMax));
				_vertices.Add(new Vector3(rect.x, m_sprite.offset.y, rect.y));
				_vertices.Add(new Vector3(rect.xMax, m_sprite.offset.y, rect.y));
				_vertices.Add(new Vector3(rect.xMax, m_sprite.offset.y, rect.yMax));
				break;
			case SpriteRoot.SPRITE_PLANE.YZ:
				_vertices.Add(new Vector3(m_sprite.offset.x, rect.yMax, rect.x));
				_vertices.Add(new Vector3(m_sprite.offset.x, rect.y, rect.x));
				_vertices.Add(new Vector3(m_sprite.offset.x, rect.y, rect.xMax));
				_vertices.Add(new Vector3(m_sprite.offset.x, rect.yMax, rect.xMax));
				break;
			}
			_uvs.Add(new Vector2(texRect.x, texRect.yMax));
			_uvs.Add(new Vector2(texRect.x, texRect.y));
			_uvs.Add(new Vector2(texRect.xMax, texRect.y));
			_uvs.Add(new Vector2(texRect.xMax, texRect.yMax));
			if (m_sprite.winding == SpriteRoot.WINDING_ORDER.CW)
			{
				_indices.Add(count);
				_indices.Add(count + 3);
				_indices.Add(count + 1);
				_indices.Add(count + 3);
				_indices.Add(count + 2);
				_indices.Add(count + 1);
			}
			else
			{
				_indices.Add(count);
				_indices.Add(count + 1);
				_indices.Add(count + 3);
				_indices.Add(count + 3);
				_indices.Add(count + 1);
				_indices.Add(count + 2);
			}
		}
	}

	public override void Init()
	{
		if (m_mesh == null)
		{
			CreateMesh();
		}
		m_mesh.Clear();
		base.Init();
		RebuildMesh();
	}

	public override void UpdateVerts()
	{
		if (_btSprite == null)
		{
			base.UpdateVerts();
		}
		else
		{
			RebuildMesh();
		}
	}

	public override void UpdateUVs()
	{
		if (_btSprite == null)
		{
			base.UpdateUVs();
		}
		else
		{
			RebuildMesh();
		}
	}

	public override void UpdateColors(Color color)
	{
		if (_btSprite == null || m_mesh == null)
		{
			base.UpdateColors(color);
		}
		else if (_colors != null)
		{
			for (int i = 0; i < _colors.Length; i++)
			{
				_colors[i] = color;
			}
			m_mesh.colors = _colors;
		}
	}

	public override void SetWindingOrder(SpriteRoot.WINDING_ORDER winding)
	{
		if (_btSprite == null || m_mesh == null)
		{
			base.SetWindingOrder(winding);
			return;
		}
		int[] triangles = m_mesh.triangles;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			int num = triangles[i + 1];
			triangles[i + 1] = triangles[i + 2];
			triangles[i + 2] = num;
		}
		m_mesh.triangles = triangles;
	}
}
