using System;
using UnityEngine;

[ExecuteInEditMode]
public abstract class SpriteRoot : MonoBehaviour, IEZLinkedListItem<ISpriteAnimatable>, IUseCamera
{
	public enum SPRITE_PLANE
	{
		XY = 0,
		XZ = 1,
		YZ = 2
	}

	public enum ANCHOR_METHOD
	{
		UPPER_LEFT = 0,
		UPPER_CENTER = 1,
		UPPER_RIGHT = 2,
		MIDDLE_LEFT = 3,
		MIDDLE_CENTER = 4,
		MIDDLE_RIGHT = 5,
		BOTTOM_LEFT = 6,
		BOTTOM_CENTER = 7,
		BOTTOM_RIGHT = 8,
		TEXTURE_OFFSET = 9
	}

	public enum WINDING_ORDER
	{
		CCW = 0,
		CW = 1
	}

	public delegate void SpriteResizedDelegate(float newWidth, float newHeight, SpriteRoot sprite);

	public bool managed;

	public SpriteManager manager;

	protected bool addedToManager;

	public int drawLayer;

	public bool persistent;

	public SPRITE_PLANE plane;

	public WINDING_ORDER winding = WINDING_ORDER.CW;

	public float width;

	public float height;

	public Vector2 bleedCompensation;

	public ANCHOR_METHOD anchor = ANCHOR_METHOD.TEXTURE_OFFSET;

	public bool pixelPerfect;

	public bool autoResize;

	protected Vector2 bleedCompensationUV;

	protected Vector2 bleedCompensationUVMax;

	protected SPRITE_FRAME frameInfo = new SPRITE_FRAME(0f);

	protected Rect uvRect;

	protected Vector2 scaleFactor = new Vector2(0.5f, 0.5f);

	protected Vector2 topLeftOffset = new Vector2(-1f, 1f);

	protected Vector2 bottomRightOffset = new Vector2(1f, -1f);

	protected Vector3 topLeft;

	protected Vector3 bottomRight;

	protected Vector3 unclippedTopLeft;

	protected Vector3 unclippedBottomRight;

	protected Vector2 tlTruncate = new Vector2(1f, 1f);

	protected Vector2 brTruncate = new Vector2(1f, 1f);

	protected bool truncated;

	protected Rect3D clippingRect;

	protected Rect localClipRect;

	protected float leftClipPct = 1f;

	protected float rightClipPct = 1f;

	protected float topClipPct = 1f;

	protected float bottomClipPct = 1f;

	protected bool clipped;

	[HideInInspector]
	public bool billboarded;

	[NonSerialized]
	public bool isClone;

	[NonSerialized]
	public bool uvsInitialized;

	protected bool m_started;

	protected bool deleted;

	public Vector3 offset = default(Vector3);

	public Color color = Color.white;

	protected ISpriteMesh m_spriteMesh;

	protected ISpriteAnimatable m_prev;

	protected ISpriteAnimatable m_next;

	protected Vector2 screenSize;

	public Camera renderCamera;

	protected Vector2 sizeUnitsPerUV;

	[HideInInspector]
	public Vector2 pixelsPerUV;

	[HideInInspector]
	public float worldUnitsPerScreenPixel;

	protected SpriteResizedDelegate resizedDelegate;

	protected EZScreenPlacement screenPlacer;

	public bool hideAtStart;

	protected bool m_hidden;

	public bool ignoreClipping;

	protected SpriteRootMirror mirror;

	protected Vector2 tempUV;

	protected Mesh oldMesh;

	protected SpriteManager savedManager;

	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			SetColor(value);
		}
	}

	public virtual Camera RenderCamera
	{
		get
		{
			return renderCamera;
		}
		set
		{
			renderCamera = value;
			SetCamera(value);
		}
	}

	public Vector2 PixelSize
	{
		get
		{
			return new Vector2(width * worldUnitsPerScreenPixel, height * worldUnitsPerScreenPixel);
		}
		set
		{
			SetSize(value.x * worldUnitsPerScreenPixel, value.y * worldUnitsPerScreenPixel);
		}
	}

	public Vector2 ImageSize
	{
		get
		{
			return new Vector2(uvRect.width * pixelsPerUV.x, uvRect.height * pixelsPerUV.y);
		}
	}

	public bool Managed
	{
		get
		{
			return managed;
		}
		set
		{
			if (value)
			{
				if (!managed)
				{
					DestroyMesh();
				}
				managed = value;
				return;
			}
			if (managed)
			{
				if (manager != null)
				{
					manager.RemoveSprite(this);
				}
				manager = null;
			}
			managed = value;
			if (m_spriteMesh == null)
			{
				AddMesh();
			}
			else if (!(m_spriteMesh is SpriteMesh))
			{
				AddMesh();
			}
		}
	}

	public bool Started
	{
		get
		{
			return m_started;
		}
	}

	public virtual Rect3D ClippingRect
	{
		get
		{
			return clippingRect;
		}
		set
		{
			if (!ignoreClipping)
			{
				clippingRect = value;
				localClipRect = Rect3D.MultFast(clippingRect, base.transform.worldToLocalMatrix).GetRect();
				clipped = true;
				CalcSize();
				UpdateUVs();
			}
		}
	}

	public virtual bool Clipped
	{
		get
		{
			return clipped;
		}
		set
		{
			if (!ignoreClipping)
			{
				if (value && !clipped)
				{
					clipped = true;
					CalcSize();
				}
				else if (clipped)
				{
					Unclip();
				}
			}
		}
	}

	public ANCHOR_METHOD Anchor
	{
		get
		{
			return anchor;
		}
		set
		{
			SetAnchor(value);
		}
	}

	public Vector3 UnclippedTopLeft
	{
		get
		{
			if (!m_started)
			{
				Start();
			}
			return unclippedTopLeft;
		}
	}

	public Vector3 UnclippedBottomRight
	{
		get
		{
			if (!m_started)
			{
				Start();
			}
			return unclippedBottomRight;
		}
	}

	public Vector3 TopLeft
	{
		get
		{
			if (m_spriteMesh != null)
			{
				return m_spriteMesh.vertices[0];
			}
			return Vector3.zero;
		}
	}

	public Vector3 BottomRight
	{
		get
		{
			if (m_spriteMesh != null)
			{
				return m_spriteMesh.vertices[2];
			}
			return Vector3.zero;
		}
	}

	public ISpriteMesh spriteMesh
	{
		get
		{
			return m_spriteMesh;
		}
		set
		{
			m_spriteMesh = value;
			if (m_spriteMesh != null)
			{
				if (m_spriteMesh.sprite != this)
				{
					m_spriteMesh.sprite = this;
				}
				if (managed)
				{
					manager = ((SpriteMesh_Managed)m_spriteMesh).manager;
				}
			}
		}
	}

	public bool AddedToManager
	{
		get
		{
			return addedToManager;
		}
		set
		{
			addedToManager = value;
		}
	}

	public ISpriteAnimatable prev
	{
		get
		{
			return m_prev;
		}
		set
		{
			m_prev = value;
		}
	}

	public ISpriteAnimatable next
	{
		get
		{
			return m_next;
		}
		set
		{
			m_next = value;
		}
	}

	protected virtual void Awake()
	{
		screenSize.x = 0f;
		screenSize.y = 0f;
		if (base.name.EndsWith("(Clone)"))
		{
			isClone = true;
		}
		if (!managed)
		{
			MeshFilter meshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));
			if (meshFilter != null)
			{
				oldMesh = meshFilter.sharedMesh;
				meshFilter.sharedMesh = null;
			}
			AddMesh();
		}
		else if (manager != null)
		{
			manager.AddSprite(this);
		}
		else
		{
			Debug.LogError("Managed sprite \"" + base.name + "\" has not been assigned to a SpriteManager!");
		}
	}

	public virtual void Start()
	{
		m_started = true;
		if (!managed)
		{
			if (Application.isPlaying)
			{
				if (!isClone)
				{
					UnityEngine.Object.Destroy(oldMesh);
				}
				oldMesh = null;
			}
		}
		else if (m_spriteMesh != null)
		{
			Init();
		}
		if (persistent)
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			if (m_spriteMesh is SpriteMesh)
			{
				((SpriteMesh)m_spriteMesh).SetPersistent();
			}
		}
		if (m_spriteMesh == null && !managed)
		{
			AddMesh();
		}
		CalcSizeUnitsPerUV();
		if (m_spriteMesh != null && m_spriteMesh.texture != null)
		{
			SetPixelToUV(m_spriteMesh.texture);
		}
		if (renderCamera == null)
		{
			renderCamera = Camera.mainCamera;
		}
		SetCamera(renderCamera);
		if (clipped)
		{
			UpdateUVs();
		}
		if (hideAtStart)
		{
			Hide(true);
		}
	}

	protected void CalcSizeUnitsPerUV()
	{
		Rect uvs = frameInfo.uvs;
		if (uvs.width == 0f || uvs.height == 0f || (uvs.xMin == 1f && uvs.yMin == 1f))
		{
			sizeUnitsPerUV = Vector2.zero;
			return;
		}
		sizeUnitsPerUV.x = width / uvs.width;
		sizeUnitsPerUV.y = height / uvs.height;
	}

	protected virtual void Init()
	{
		screenPlacer = (EZScreenPlacement)GetComponent(typeof(EZScreenPlacement));
		if (screenPlacer != null)
		{
			screenPlacer.SetCamera(renderCamera);
		}
		if (m_spriteMesh != null)
		{
			if (persistent && !managed)
			{
				UnityEngine.Object.DontDestroyOnLoad(((SpriteMesh)m_spriteMesh).mesh);
			}
			if (m_spriteMesh.texture != null)
			{
				SetPixelToUV(m_spriteMesh.texture);
			}
			m_spriteMesh.Init();
		}
		if (!Application.isPlaying)
		{
			CalcSizeUnitsPerUV();
		}
	}

	public virtual void Clear()
	{
		billboarded = false;
		SetColor(Color.white);
		offset = Vector3.zero;
	}

	public virtual void Copy(SpriteRoot s)
	{
		if (!managed)
		{
			if (m_spriteMesh != null && s.spriteMesh != null)
			{
				((SpriteMesh)m_spriteMesh).material = s.spriteMesh.material;
			}
			else if (!s.managed)
			{
				base.renderer.sharedMaterial = s.renderer.sharedMaterial;
			}
		}
		drawLayer = s.drawLayer;
		if (s.renderCamera != null)
		{
			SetCamera(s.renderCamera);
		}
		if (renderCamera == null)
		{
			renderCamera = Camera.main;
		}
		if (m_spriteMesh != null)
		{
			if (m_spriteMesh.texture != null)
			{
				SetPixelToUV(m_spriteMesh.texture);
			}
			else if (!managed)
			{
				((SpriteMesh)m_spriteMesh).material = base.renderer.sharedMaterial;
				SetPixelToUV(m_spriteMesh.texture);
			}
		}
		plane = s.plane;
		winding = s.winding;
		offset = s.offset;
		anchor = s.anchor;
		bleedCompensation = s.bleedCompensation;
		autoResize = s.autoResize;
		pixelPerfect = s.pixelPerfect;
		ignoreClipping = s.ignoreClipping;
		uvRect = s.uvRect;
		scaleFactor = s.scaleFactor;
		topLeftOffset = s.topLeftOffset;
		bottomRightOffset = s.bottomRightOffset;
		width = s.width;
		height = s.height;
		SetColor(s.color);
	}

	public virtual void InitUVs()
	{
		uvRect = frameInfo.uvs;
	}

	public virtual void Delete()
	{
		deleted = true;
		if (!managed && Application.isPlaying)
		{
			UnityEngine.Object.Destroy(((SpriteMesh)spriteMesh).mesh);
			((SpriteMesh)spriteMesh).mesh = null;
		}
	}

	protected virtual void OnEnable()
	{
		if (managed && manager != null && m_started)
		{
			SPRITE_FRAME sPRITE_FRAME = frameInfo;
			manager.AddSprite(this);
			frameInfo = sPRITE_FRAME;
			uvRect = frameInfo.uvs;
			SetBleedCompensation();
		}
		else if (savedManager != null)
		{
			savedManager.AddSprite(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (managed && manager != null)
		{
			savedManager = manager;
			manager.RemoveSprite(this);
		}
	}

	public virtual void OnDestroy()
	{
		Delete();
	}

	public void CalcEdges()
	{
		switch (anchor)
		{
		case ANCHOR_METHOD.TEXTURE_OFFSET:
		{
			Vector2 vector = default(Vector2);
			vector.x = width * scaleFactor.x;
			vector.y = height * scaleFactor.y;
			topLeft.x = vector.x * topLeftOffset.x;
			topLeft.y = vector.y * topLeftOffset.y;
			bottomRight.x = vector.x * bottomRightOffset.x;
			bottomRight.y = vector.y * bottomRightOffset.y;
			break;
		}
		case ANCHOR_METHOD.UPPER_LEFT:
			topLeft.x = 0f;
			topLeft.y = 0f;
			bottomRight.x = width;
			bottomRight.y = 0f - height;
			break;
		case ANCHOR_METHOD.UPPER_CENTER:
			topLeft.x = width * -0.5f;
			topLeft.y = 0f;
			bottomRight.x = width * 0.5f;
			bottomRight.y = 0f - height;
			break;
		case ANCHOR_METHOD.UPPER_RIGHT:
			topLeft.x = 0f - width;
			topLeft.y = 0f;
			bottomRight.x = 0f;
			bottomRight.y = 0f - height;
			break;
		case ANCHOR_METHOD.MIDDLE_LEFT:
			topLeft.x = 0f;
			topLeft.y = height * 0.5f;
			bottomRight.x = width;
			bottomRight.y = height * -0.5f;
			break;
		case ANCHOR_METHOD.MIDDLE_CENTER:
			topLeft.x = width * -0.5f;
			topLeft.y = height * 0.5f;
			bottomRight.x = width * 0.5f;
			bottomRight.y = height * -0.5f;
			break;
		case ANCHOR_METHOD.MIDDLE_RIGHT:
			topLeft.x = 0f - width;
			topLeft.y = height * 0.5f;
			bottomRight.x = 0f;
			bottomRight.y = height * -0.5f;
			break;
		case ANCHOR_METHOD.BOTTOM_LEFT:
			topLeft.x = 0f;
			topLeft.y = height;
			bottomRight.x = width;
			bottomRight.y = 0f;
			break;
		case ANCHOR_METHOD.BOTTOM_CENTER:
			topLeft.x = width * -0.5f;
			topLeft.y = height;
			bottomRight.x = width * 0.5f;
			bottomRight.y = 0f;
			break;
		case ANCHOR_METHOD.BOTTOM_RIGHT:
			topLeft.x = 0f - width;
			topLeft.y = height;
			bottomRight.x = 0f;
			bottomRight.y = 0f;
			break;
		}
		unclippedTopLeft = topLeft + offset;
		unclippedBottomRight = bottomRight + offset;
		if (truncated)
		{
			topLeft.x = bottomRight.x - (bottomRight.x - topLeft.x) * tlTruncate.x;
			topLeft.y = bottomRight.y - (bottomRight.y - topLeft.y) * tlTruncate.y;
			bottomRight.x = topLeft.x - (topLeft.x - bottomRight.x) * brTruncate.x;
			bottomRight.y = topLeft.y - (topLeft.y - bottomRight.y) * brTruncate.y;
		}
		if (clipped && bottomRight.x - topLeft.x != 0f && topLeft.y - bottomRight.y != 0f)
		{
			Vector3 vector2 = topLeft;
			Vector3 vector3 = bottomRight;
			Rect rect = localClipRect;
			rect.x -= offset.x;
			rect.y -= offset.y;
			if (topLeft.x < rect.x)
			{
				leftClipPct = 1f - (rect.x - vector2.x) / (vector3.x - vector2.x);
				topLeft.x = Mathf.Clamp(rect.x, vector2.x, vector3.x);
				if (leftClipPct <= 0f)
				{
					topLeft.x = (bottomRight.x = rect.x);
				}
			}
			else
			{
				leftClipPct = 1f;
			}
			if (bottomRight.x > rect.xMax)
			{
				rightClipPct = (rect.xMax - vector2.x) / (vector3.x - vector2.x);
				bottomRight.x = Mathf.Clamp(rect.xMax, vector2.x, vector3.x);
				if (rightClipPct <= 0f)
				{
					bottomRight.x = (topLeft.x = rect.xMax);
				}
			}
			else
			{
				rightClipPct = 1f;
			}
			if (topLeft.y > rect.yMax)
			{
				topClipPct = (rect.yMax - vector3.y) / (vector2.y - vector3.y);
				topLeft.y = Mathf.Clamp(rect.yMax, vector3.y, vector2.y);
				if (topClipPct <= 0f)
				{
					topLeft.y = (bottomRight.y = rect.yMax);
				}
			}
			else
			{
				topClipPct = 1f;
			}
			if (bottomRight.y < rect.y)
			{
				bottomClipPct = 1f - (rect.y - vector3.y) / (vector2.y - vector3.y);
				bottomRight.y = Mathf.Clamp(rect.y, vector3.y, vector2.y);
				if (bottomClipPct <= 0f)
				{
					bottomRight.y = (topLeft.y = rect.y);
				}
			}
			else
			{
				bottomClipPct = 1f;
			}
		}
		if (winding == WINDING_ORDER.CCW)
		{
			topLeft.x *= -1f;
			bottomRight.x *= -1f;
		}
	}

	public void CalcSize()
	{
		if (uvRect.width == 0f)
		{
			uvRect.width = 1E-07f;
		}
		if (uvRect.height == 0f)
		{
			uvRect.height = 1E-07f;
		}
		if (pixelPerfect)
		{
			width = worldUnitsPerScreenPixel * frameInfo.uvs.width * pixelsPerUV.x;
			height = worldUnitsPerScreenPixel * frameInfo.uvs.height * pixelsPerUV.y;
		}
		else if (autoResize && sizeUnitsPerUV.x != 0f && sizeUnitsPerUV.y != 0f)
		{
			width = frameInfo.uvs.width * sizeUnitsPerUV.x;
			height = frameInfo.uvs.height * sizeUnitsPerUV.y;
		}
		SetSize(width, height);
	}

	public virtual void SetSize(float w, float h)
	{
		if (m_spriteMesh != null)
		{
			width = w;
			height = h;
			CalcSizeUnitsPerUV();
			switch (plane)
			{
			case SPRITE_PLANE.XY:
				SetSizeXY(width, height);
				break;
			case SPRITE_PLANE.XZ:
				SetSizeXZ(width, height);
				break;
			case SPRITE_PLANE.YZ:
				SetSizeYZ(width, height);
				break;
			}
			if (resizedDelegate != null)
			{
				resizedDelegate(width, height, this);
			}
		}
	}

	protected void SetSizeXY(float w, float h)
	{
		CalcEdges();
		Vector3[] vertices = m_spriteMesh.vertices;
		if (winding == WINDING_ORDER.CW)
		{
			vertices[0].x = offset.x + topLeft.x;
			vertices[0].y = offset.y + topLeft.y;
			vertices[0].z = offset.z;
			vertices[1].x = offset.x + topLeft.x;
			vertices[1].y = offset.y + bottomRight.y;
			vertices[1].z = offset.z;
			vertices[2].x = offset.x + bottomRight.x;
			vertices[2].y = offset.y + bottomRight.y;
			vertices[2].z = offset.z;
			vertices[3].x = offset.x + bottomRight.x;
			vertices[3].y = offset.y + topLeft.y;
			vertices[3].z = offset.z;
		}
		else
		{
			vertices[0].x = offset.x + topLeft.x;
			vertices[0].y = offset.y + topLeft.y;
			vertices[0].z = offset.z;
			vertices[1].x = offset.x + topLeft.x;
			vertices[1].y = offset.y + bottomRight.y;
			vertices[1].z = offset.z;
			vertices[2].x = offset.x + bottomRight.x;
			vertices[2].y = offset.y + bottomRight.y;
			vertices[2].z = offset.z;
			vertices[3].x = offset.x + bottomRight.x;
			vertices[3].y = offset.y + topLeft.y;
			vertices[3].z = offset.z;
		}
		m_spriteMesh.UpdateVerts();
	}

	protected void SetSizeXZ(float w, float h)
	{
		CalcEdges();
		Vector3[] vertices = m_spriteMesh.vertices;
		vertices[0].x = offset.x + topLeft.x;
		vertices[0].y = offset.y;
		vertices[0].z = offset.z + topLeft.y;
		vertices[1].x = offset.x + topLeft.x;
		vertices[1].y = offset.y;
		vertices[1].z = offset.z + bottomRight.y;
		vertices[2].x = offset.x + bottomRight.x;
		vertices[2].y = offset.y;
		vertices[2].z = offset.z + bottomRight.y;
		vertices[3].x = offset.x + bottomRight.x;
		vertices[3].y = offset.y;
		vertices[3].z = offset.z + topLeft.y;
		m_spriteMesh.UpdateVerts();
	}

	protected void SetSizeYZ(float w, float h)
	{
		CalcEdges();
		Vector3[] vertices = m_spriteMesh.vertices;
		vertices[0].x = offset.x;
		vertices[0].y = offset.y + topLeft.y;
		vertices[0].z = offset.z + topLeft.x;
		vertices[1].x = offset.x;
		vertices[1].y = offset.y + bottomRight.y;
		vertices[1].z = offset.z + topLeft.x;
		vertices[2].x = offset.x;
		vertices[2].y = offset.y + bottomRight.y;
		vertices[2].z = offset.z + bottomRight.x;
		vertices[3].x = offset.x;
		vertices[3].y = offset.y + topLeft.y;
		vertices[3].z = offset.z + bottomRight.x;
		m_spriteMesh.UpdateVerts();
	}

	public virtual void TruncateRight(float pct)
	{
		tlTruncate.x = 1f;
		brTruncate.x = Mathf.Clamp01(pct);
		if (brTruncate.x < 1f || tlTruncate.y < 1f || brTruncate.y < 1f)
		{
			truncated = true;
			UpdateUVs();
			CalcSize();
		}
		else
		{
			Untruncate();
		}
	}

	public virtual void TruncateLeft(float pct)
	{
		tlTruncate.x = Mathf.Clamp01(pct);
		brTruncate.x = 1f;
		if (tlTruncate.x < 1f || tlTruncate.y < 1f || brTruncate.y < 1f)
		{
			truncated = true;
			UpdateUVs();
			CalcSize();
		}
		else
		{
			Untruncate();
		}
	}

	public virtual void TruncateTop(float pct)
	{
		tlTruncate.y = Mathf.Clamp01(pct);
		brTruncate.y = 1f;
		if (tlTruncate.y < 1f || tlTruncate.x < 1f || brTruncate.x < 1f)
		{
			truncated = true;
			UpdateUVs();
			CalcSize();
		}
		else
		{
			Untruncate();
		}
	}

	public virtual void TruncateBottom(float pct)
	{
		tlTruncate.y = 1f;
		brTruncate.y = Mathf.Clamp01(pct);
		if (brTruncate.y < 1f || tlTruncate.x < 1f || brTruncate.x < 1f)
		{
			truncated = true;
			UpdateUVs();
			CalcSize();
		}
		else
		{
			Untruncate();
		}
	}

	public virtual void Untruncate()
	{
		tlTruncate.x = 1f;
		tlTruncate.y = 1f;
		brTruncate.x = 1f;
		brTruncate.y = 1f;
		truncated = false;
		uvRect = frameInfo.uvs;
		SetBleedCompensation();
		CalcSize();
	}

	public virtual void Unclip()
	{
		if (!ignoreClipping)
		{
			leftClipPct = 1f;
			rightClipPct = 1f;
			topClipPct = 1f;
			bottomClipPct = 1f;
			clipped = false;
			uvRect = frameInfo.uvs;
			SetBleedCompensation();
			CalcSize();
		}
	}

	public virtual void UpdateUVs()
	{
		scaleFactor = frameInfo.scaleFactor;
		topLeftOffset = frameInfo.topLeftOffset;
		bottomRightOffset = frameInfo.bottomRightOffset;
		if (truncated)
		{
			uvRect.x = frameInfo.uvs.xMax + bleedCompensationUV.x - frameInfo.uvs.width * tlTruncate.x * leftClipPct;
			uvRect.y = frameInfo.uvs.yMax + bleedCompensationUV.y - frameInfo.uvs.height * brTruncate.y * bottomClipPct;
			uvRect.xMax = frameInfo.uvs.x + bleedCompensationUVMax.x + frameInfo.uvs.width * brTruncate.x * rightClipPct;
			uvRect.yMax = frameInfo.uvs.y + bleedCompensationUVMax.y + frameInfo.uvs.height * tlTruncate.y * topClipPct;
		}
		else if (clipped)
		{
			Rect rect = Rect.MinMaxRect(frameInfo.uvs.x + bleedCompensationUV.x, frameInfo.uvs.y + bleedCompensationUV.y, frameInfo.uvs.xMax + bleedCompensationUVMax.x, frameInfo.uvs.yMax + bleedCompensationUVMax.y);
			uvRect.x = Mathf.Lerp(rect.xMax, rect.x, leftClipPct);
			uvRect.y = Mathf.Lerp(rect.yMax, rect.y, bottomClipPct);
			uvRect.xMax = Mathf.Lerp(rect.x, rect.xMax, rightClipPct);
			uvRect.yMax = Mathf.Lerp(rect.y, rect.yMax, topClipPct);
		}
		if (m_spriteMesh != null)
		{
			Vector2[] uvs = m_spriteMesh.uvs;
			uvs[0].x = uvRect.x;
			uvs[0].y = uvRect.yMax;
			uvs[1].x = uvRect.x;
			uvs[1].y = uvRect.y;
			uvs[2].x = uvRect.xMax;
			uvs[2].y = uvRect.y;
			uvs[3].x = uvRect.xMax;
			uvs[3].y = uvRect.yMax;
			m_spriteMesh.UpdateUVs();
		}
	}

	public void TransformBillboarded(Transform t)
	{
	}

	public virtual void SetColor(Color c)
	{
		color = c;
		if (m_spriteMesh != null)
		{
			m_spriteMesh.UpdateColors(color);
		}
	}

	public void SetPixelToUV(int texWidth, int texHeight)
	{
		Vector2 vector = pixelsPerUV;
		pixelsPerUV.x = texWidth;
		pixelsPerUV.y = texHeight;
		Rect uvs = frameInfo.uvs;
		if (uvs.width != 0f && uvs.height != 0f && vector.x != 0f && vector.y != 0f)
		{
			Vector2 vector2 = new Vector2(width / (uvs.width * vector.x), height / (uvs.height * vector.y));
			sizeUnitsPerUV.x = vector2.x * pixelsPerUV.x;
			sizeUnitsPerUV.y = vector2.y * pixelsPerUV.y;
		}
	}

	public void SetPixelToUV(Texture tex)
	{
		if (!(tex == null))
		{
			SetPixelToUV(tex.width, tex.height);
		}
	}

	public void CalcPixelToUV()
	{
		if (managed)
		{
			if (spriteMesh != null && spriteMesh.material != null && spriteMesh.material.mainTexture != null)
			{
				SetPixelToUV(spriteMesh.material.mainTexture);
			}
		}
		else if (base.renderer != null && base.renderer.sharedMaterial != null && base.renderer.sharedMaterial.mainTexture != null)
		{
			SetPixelToUV(base.renderer.sharedMaterial.mainTexture);
		}
	}

	public void SetTexture(Texture2D tex)
	{
		if (!managed && !(base.renderer == null))
		{
			base.renderer.material.mainTexture = tex;
			SetPixelToUV(tex);
			SetCamera();
		}
	}

	public void SetMaterial(Material mat)
	{
		if (!managed && !(base.renderer == null))
		{
			base.renderer.sharedMaterial = mat;
			SetPixelToUV(mat.mainTexture);
			SetCamera();
		}
	}

	public void UpdateCamera()
	{
		SetCamera(renderCamera);
	}

	public void SetCamera()
	{
		SetCamera(renderCamera);
	}

	public virtual void SetCamera(Camera c)
	{
		if (c == null || !m_started)
		{
			return;
		}
		Plane plane = new Plane(c.transform.forward, c.transform.position);
		if (!Application.isPlaying)
		{
			screenSize.x = c.pixelWidth;
			screenSize.y = c.pixelHeight;
			if (screenSize.x != 0f)
			{
				renderCamera = c;
				if (screenPlacer != null)
				{
					screenPlacer.SetCamera(renderCamera);
				}
				float distanceToPoint = plane.GetDistanceToPoint(base.transform.position);
				worldUnitsPerScreenPixel = Vector3.Distance(c.ScreenToWorldPoint(new Vector3(0f, 1f, distanceToPoint)), c.ScreenToWorldPoint(new Vector3(0f, 0f, distanceToPoint)));
				if (!hideAtStart)
				{
					CalcSize();
				}
			}
		}
		else
		{
			renderCamera = c;
			screenSize.x = c.pixelWidth;
			screenSize.y = c.pixelHeight;
			if (screenPlacer != null)
			{
				screenPlacer.SetCamera(renderCamera);
			}
			float distanceToPoint = plane.GetDistanceToPoint(base.transform.position);
			worldUnitsPerScreenPixel = Vector3.Distance(c.ScreenToWorldPoint(new Vector3(0f, 1f, distanceToPoint)), c.ScreenToWorldPoint(new Vector3(0f, 0f, distanceToPoint)));
			CalcSize();
		}
	}

	public virtual void Hide(bool tf)
	{
		if (m_spriteMesh != null)
		{
			m_spriteMesh.Hide(tf);
		}
		m_hidden = tf;
	}

	public bool IsHidden()
	{
		return m_hidden;
	}

	protected void DestroyMesh()
	{
		if (m_spriteMesh != null)
		{
			m_spriteMesh.sprite = null;
		}
		m_spriteMesh = null;
		if (base.renderer != null)
		{
			UnityEngine.Object.DestroyImmediate(base.renderer);
		}
		UnityEngine.Object component = base.gameObject.GetComponent(typeof(MeshFilter));
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
	}

	protected void AddMesh()
	{
		m_spriteMesh = new SpriteMesh();
		m_spriteMesh.sprite = this;
	}

	public void SetBleedCompensation()
	{
		SetBleedCompensation(bleedCompensation);
	}

	public void SetBleedCompensation(float x, float y)
	{
		SetBleedCompensation(new Vector2(x, y));
	}

	public void SetBleedCompensation(Vector2 xy)
	{
		bleedCompensation = xy;
		bleedCompensationUV = PixelSpaceToUVSpace(bleedCompensation);
		bleedCompensationUVMax = bleedCompensationUV * -2f;
		uvRect.x += bleedCompensationUV.x;
		uvRect.y += bleedCompensationUV.y;
		uvRect.xMax += bleedCompensationUVMax.x;
		uvRect.yMax += bleedCompensationUVMax.y;
		UpdateUVs();
	}

	public void SetPlane(SPRITE_PLANE p)
	{
		plane = p;
		SetSize(width, height);
	}

	public void SetWindingOrder(WINDING_ORDER order)
	{
		winding = order;
		if (!managed && m_spriteMesh != null)
		{
			((SpriteMesh)m_spriteMesh).SetWindingOrder(order);
		}
	}

	public void SetDrawLayer(int layer)
	{
		if (managed)
		{
			drawLayer = layer;
			((SpriteMesh_Managed)m_spriteMesh).drawLayer = layer;
			if (manager != null)
			{
				manager.SortDrawingOrder();
			}
		}
	}

	public void SetFrameInfo(SPRITE_FRAME fInfo)
	{
		frameInfo = fInfo;
		uvRect = fInfo.uvs;
		SetBleedCompensation();
		if (autoResize || pixelPerfect)
		{
			CalcSize();
		}
	}

	public void SetUVs(Rect uv)
	{
		frameInfo.uvs = uv;
		uvRect = uv;
		SetBleedCompensation();
		if (!Application.isPlaying)
		{
			CalcSizeUnitsPerUV();
		}
		if (autoResize || pixelPerfect)
		{
			CalcSize();
		}
	}

	public void SetUVsFromPixelCoords(Rect pxCoords)
	{
		tempUV = PixelCoordToUVCoord((int)pxCoords.x, (int)pxCoords.yMax);
		uvRect.x = tempUV.x;
		uvRect.y = tempUV.y;
		tempUV = PixelCoordToUVCoord((int)pxCoords.xMax, (int)pxCoords.y);
		uvRect.xMax = tempUV.x;
		uvRect.yMax = tempUV.y;
		frameInfo.uvs = uvRect;
		SetBleedCompensation();
		if (autoResize || pixelPerfect)
		{
			CalcSize();
		}
	}

	public Rect GetUVs()
	{
		return uvRect;
	}

	public Vector3[] GetVertices()
	{
		if (!managed)
		{
			return ((SpriteMesh)m_spriteMesh).mesh.vertices;
		}
		return m_spriteMesh.vertices;
	}

	public Vector3 GetCenterPoint()
	{
		if (m_spriteMesh == null)
		{
			return Vector3.zero;
		}
		Vector3[] vertices = m_spriteMesh.vertices;
		switch (plane)
		{
		case SPRITE_PLANE.XY:
			return new Vector3(vertices[0].x + 0.5f * (vertices[2].x - vertices[0].x), vertices[0].y - 0.5f * (vertices[0].y - vertices[2].y), offset.z);
		case SPRITE_PLANE.XZ:
			return new Vector3(vertices[0].x + 0.5f * (vertices[2].x - vertices[0].x), offset.y, vertices[0].z - 0.5f * (vertices[0].z - vertices[2].z));
		case SPRITE_PLANE.YZ:
			return new Vector3(offset.x, vertices[0].y - 0.5f * (vertices[0].y - vertices[2].y), vertices[0].z - 0.5f * (vertices[0].z - vertices[2].z));
		default:
			return new Vector3(vertices[0].x + 0.5f * (vertices[2].x - vertices[0].x), vertices[0].y - 0.5f * (vertices[0].y - vertices[2].y), offset.z);
		}
	}

	public void SetAnchor(ANCHOR_METHOD a)
	{
		anchor = a;
		SetSize(width, height);
	}

	public void SetOffset(Vector3 o)
	{
		offset = o;
		SetSize(width, height);
	}

	public abstract Vector2 GetDefaultPixelSize(PathFromGUIDDelegate guid2Path, AssetLoaderDelegate loader);

	public Vector2 PixelSpaceToUVSpace(Vector2 xy)
	{
		if (pixelsPerUV.x == 0f || pixelsPerUV.y == 0f)
		{
			return Vector2.zero;
		}
		return new Vector2(xy.x / pixelsPerUV.x, xy.y / pixelsPerUV.y);
	}

	public Vector2 PixelSpaceToUVSpace(int x, int y)
	{
		return PixelSpaceToUVSpace(new Vector2(x, y));
	}

	public Vector2 PixelCoordToUVCoord(Vector2 xy)
	{
		if (pixelsPerUV.x == 0f || pixelsPerUV.y == 0f)
		{
			return Vector2.zero;
		}
		return new Vector2(xy.x / (pixelsPerUV.x - 1f), 1f - xy.y / (pixelsPerUV.y - 1f));
	}

	public Vector2 PixelCoordToUVCoord(int x, int y)
	{
		return PixelCoordToUVCoord(new Vector2(x, y));
	}

	public abstract int GetStateIndex(string stateName);

	public abstract void SetState(int index);

	public virtual void DoMirror()
	{
		if (!Application.isPlaying)
		{
			if (screenSize.x == 0f || screenSize.y == 0f)
			{
				Start();
			}
			if (mirror == null)
			{
				mirror = new SpriteRootMirror();
				mirror.Mirror(this);
			}
			mirror.Validate(this);
			if (mirror.DidChange(this))
			{
				Init();
				mirror.Mirror(this);
			}
		}
	}

	public virtual void OnDrawGizmosSelected()
	{
		DoMirror();
	}

	public virtual void OnDrawGizmos()
	{
		DoMirror();
	}
}
