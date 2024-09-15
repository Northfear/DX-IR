using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SpriteManager : MonoBehaviour
{
	public SpriteRoot.WINDING_ORDER winding = SpriteRoot.WINDING_ORDER.CW;

	public int allocBlockSize = 10;

	public bool autoUpdateBounds = true;

	public bool drawBoundingBox;

	public bool persistent;

	protected bool initialized;

	protected EZLinkedList<SpriteMesh_Managed> availableBlocks = new EZLinkedList<SpriteMesh_Managed>();

	protected bool vertsChanged;

	protected bool uvsChanged;

	protected bool colorsChanged;

	protected bool vertCountChanged;

	protected bool updateBounds;

	protected SpriteMesh_Managed[] sprites;

	protected EZLinkedList<SpriteMesh_Managed> activeBlocks = new EZLinkedList<SpriteMesh_Managed>();

	protected List<SpriteMesh_Managed> spriteDrawOrder = new List<SpriteMesh_Managed>();

	protected SpriteDrawLayerComparer drawOrderComparer = new SpriteDrawLayerComparer();

	protected float boundUpdateInterval;

	protected List<SpriteRoot> spriteAddQueue;

	protected SkinnedMeshRenderer meshRenderer;

	protected Mesh mesh;

	protected Texture texture;

	protected Transform[] bones;

	protected BoneWeight[] boneWeights;

	protected Matrix4x4[] bindPoses;

	protected Vector3[] vertices;

	protected int[] triIndices;

	protected Vector2[] UVs;

	protected Vector2[] UVs2;

	protected Color[] colors;

	protected SpriteMesh_Managed tempSprite;

	public bool IsInitialized
	{
		get
		{
			return initialized;
		}
	}

	public Vector2 PixelSpaceToUVSpace(Vector2 xy)
	{
		if (texture == null)
		{
			return Vector2.zero;
		}
		return new Vector2(xy.x / (float)texture.width, xy.y / (float)texture.height);
	}

	public Vector2 PixelSpaceToUVSpace(int x, int y)
	{
		return PixelSpaceToUVSpace(new Vector2(x, y));
	}

	public Vector2 PixelCoordToUVCoord(Vector2 xy)
	{
		if (texture == null)
		{
			return Vector2.zero;
		}
		return new Vector2(xy.x / ((float)texture.width - 1f), 1f - xy.y / ((float)texture.height - 1f));
	}

	public Vector2 PixelCoordToUVCoord(int x, int y)
	{
		return PixelCoordToUVCoord(new Vector2(x, y));
	}

	protected void SetupBoneWeights(SpriteMesh_Managed s)
	{
		boneWeights[s.mv1].boneIndex0 = s.index;
		boneWeights[s.mv1].weight0 = 1f;
		boneWeights[s.mv2].boneIndex0 = s.index;
		boneWeights[s.mv2].weight0 = 1f;
		boneWeights[s.mv3].boneIndex0 = s.index;
		boneWeights[s.mv3].weight0 = 1f;
		boneWeights[s.mv4].boneIndex0 = s.index;
		boneWeights[s.mv4].weight0 = 1f;
	}

	private void Awake()
	{
		if (spriteAddQueue == null)
		{
			spriteAddQueue = new List<SpriteRoot>();
		}
		meshRenderer = (SkinnedMeshRenderer)GetComponent(typeof(SkinnedMeshRenderer));
		if (meshRenderer != null && meshRenderer.sharedMaterial != null)
		{
			texture = meshRenderer.sharedMaterial.GetTexture("_MainTex");
		}
		if (meshRenderer.sharedMesh == null)
		{
			meshRenderer.sharedMesh = new Mesh();
		}
		mesh = meshRenderer.sharedMesh;
		if (persistent)
		{
			Object.DontDestroyOnLoad(this);
			Object.DontDestroyOnLoad(mesh);
		}
		EnlargeArrays(allocBlockSize);
		base.transform.rotation = Quaternion.identity;
		initialized = true;
		for (int i = 0; i < spriteAddQueue.Count; i++)
		{
			AddSprite(spriteAddQueue[i]);
		}
	}

	protected void InitArrays()
	{
		bones = new Transform[1];
		bones[0] = base.transform;
		bindPoses = new Matrix4x4[1];
		sprites = new SpriteMesh_Managed[1];
		sprites[0] = new SpriteMesh_Managed();
		vertices = new Vector3[4];
		UVs = new Vector2[4];
		colors = new Color[4];
		triIndices = new int[6];
		boneWeights = new BoneWeight[4];
		sprites[0].index = 0;
		sprites[0].mv1 = 0;
		sprites[0].mv2 = 1;
		sprites[0].mv3 = 2;
		sprites[0].mv4 = 3;
		SetupBoneWeights(sprites[0]);
	}

	protected int EnlargeArrays(int count)
	{
		int num;
		if (sprites == null)
		{
			InitArrays();
			num = 0;
			count--;
		}
		else
		{
			num = sprites.Length;
		}
		SpriteMesh_Managed[] array = sprites;
		sprites = new SpriteMesh_Managed[sprites.Length + count];
		array.CopyTo(sprites, 0);
		Transform[] array2 = bones;
		bones = new Transform[bones.Length + count];
		array2.CopyTo(bones, 0);
		Matrix4x4[] array3 = bindPoses;
		bindPoses = new Matrix4x4[bindPoses.Length + count];
		array3.CopyTo(bindPoses, 0);
		Vector3[] array4 = vertices;
		vertices = new Vector3[vertices.Length + count * 4];
		array4.CopyTo(vertices, 0);
		BoneWeight[] array5 = boneWeights;
		boneWeights = new BoneWeight[boneWeights.Length + count * 4];
		array5.CopyTo(boneWeights, 0);
		Vector2[] uVs = UVs;
		UVs = new Vector2[UVs.Length + count * 4];
		uVs.CopyTo(UVs, 0);
		Color[] array6 = colors;
		colors = new Color[colors.Length + count * 4];
		array6.CopyTo(colors, 0);
		int[] array7 = triIndices;
		triIndices = new int[triIndices.Length + count * 6];
		array7.CopyTo(triIndices, 0);
		for (int i = 0; i < num; i++)
		{
			sprites[i].SetBuffers(vertices, UVs, UVs2, colors);
		}
		for (int j = num; j < sprites.Length; j++)
		{
			sprites[j] = new SpriteMesh_Managed();
			sprites[j].index = j;
			sprites[j].manager = this;
			sprites[j].SetBuffers(vertices, UVs, UVs2, colors);
			sprites[j].mv1 = j * 4;
			sprites[j].mv2 = j * 4 + 1;
			sprites[j].mv3 = j * 4 + 2;
			sprites[j].mv4 = j * 4 + 3;
			sprites[j].uv1 = j * 4;
			sprites[j].uv2 = j * 4 + 1;
			sprites[j].uv3 = j * 4 + 2;
			sprites[j].uv4 = j * 4 + 3;
			sprites[j].cv1 = j * 4;
			sprites[j].cv2 = j * 4 + 1;
			sprites[j].cv3 = j * 4 + 2;
			sprites[j].cv4 = j * 4 + 3;
			availableBlocks.Add(sprites[j]);
			triIndices[j * 6] = j * 4;
			triIndices[j * 6 + 1] = j * 4 + 3;
			triIndices[j * 6 + 2] = j * 4 + 1;
			triIndices[j * 6 + 3] = j * 4 + 3;
			triIndices[j * 6 + 4] = j * 4 + 2;
			triIndices[j * 6 + 5] = j * 4 + 1;
			spriteDrawOrder.Add(sprites[j]);
			bones[j] = base.transform;
			bindPoses[j] = bones[j].worldToLocalMatrix * base.transform.localToWorldMatrix;
			SetupBoneWeights(sprites[j]);
		}
		vertsChanged = true;
		uvsChanged = true;
		colorsChanged = true;
		vertCountChanged = true;
		return num;
	}

	public bool AlreadyAdded(SpriteRoot sprite)
	{
		if (activeBlocks.Rewind())
		{
			do
			{
				if (activeBlocks.Current.sprite == sprite)
				{
					return true;
				}
			}
			while (activeBlocks.MoveNext());
		}
		return false;
	}

	public SpriteMesh_Managed AddSprite(GameObject go)
	{
		SpriteRoot spriteRoot = (SpriteRoot)go.GetComponent(typeof(SpriteRoot));
		if (spriteRoot == null)
		{
			return null;
		}
		return AddSprite(spriteRoot);
	}

	public SpriteMesh_Managed AddSprite(SpriteRoot sprite)
	{
		if (sprite.manager == this && sprite.AddedToManager)
		{
			return (SpriteMesh_Managed)sprite.spriteMesh;
		}
		if (!initialized)
		{
			if (spriteAddQueue == null)
			{
				spriteAddQueue = new List<SpriteRoot>();
			}
			spriteAddQueue.Add(sprite);
			return null;
		}
		if (availableBlocks.Empty)
		{
			EnlargeArrays(allocBlockSize);
		}
		int index = availableBlocks.Head.index;
		availableBlocks.Remove(availableBlocks.Head);
		SpriteMesh_Managed spriteMesh_Managed = (SpriteMesh_Managed)(sprite.spriteMesh = sprites[index]);
		sprite.manager = this;
		sprite.AddedToManager = true;
		spriteMesh_Managed.drawLayer = sprite.drawLayer;
		bones[index] = sprite.gameObject.transform;
		bindPoses[index] = bones[index].worldToLocalMatrix * sprite.transform.localToWorldMatrix;
		activeBlocks.Add(spriteMesh_Managed);
		spriteMesh_Managed.Init();
		SortDrawingOrder();
		vertCountChanged = true;
		vertsChanged = true;
		uvsChanged = true;
		return spriteMesh_Managed;
	}

	public SpriteRoot CreateSprite(GameObject prefab)
	{
		return CreateSprite(prefab, Vector3.zero, Quaternion.identity);
	}

	public SpriteRoot CreateSprite(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(prefab, position, rotation);
		SpriteRoot result = (SpriteRoot)gameObject.GetComponent(typeof(SpriteRoot));
		AddSprite(gameObject);
		return result;
	}

	public void RemoveSprite(SpriteRoot sprite)
	{
		if (sprite.spriteMesh is SpriteMesh_Managed && sprite.spriteMesh != null)
		{
			if (sprite.manager == this)
			{
				sprite.manager = null;
				sprite.AddedToManager = false;
			}
			RemoveSprite((SpriteMesh_Managed)sprite.spriteMesh);
		}
	}

	public void RemoveSprite(SpriteMesh_Managed sprite)
	{
		vertices[sprite.mv1] = Vector3.zero;
		vertices[sprite.mv2] = Vector3.zero;
		vertices[sprite.mv3] = Vector3.zero;
		vertices[sprite.mv4] = Vector3.zero;
		activeBlocks.Remove(sprite);
		if (base.gameObject != null)
		{
			bones[sprite.index] = base.transform;
		}
		sprite.Clear();
		sprite.sprite.spriteMesh = null;
		sprite.sprite = null;
		availableBlocks.Add(sprite);
		vertsChanged = true;
	}

	public void MoveToFront(SpriteMesh_Managed s)
	{
		int[] array = new int[6];
		int num = spriteDrawOrder.IndexOf(s) * 6;
		if (num >= 0)
		{
			s.drawLayer = spriteDrawOrder[spriteDrawOrder.Count - 1].drawLayer + 1;
			array[0] = triIndices[num];
			array[1] = triIndices[num + 1];
			array[2] = triIndices[num + 2];
			array[3] = triIndices[num + 3];
			array[4] = triIndices[num + 4];
			array[5] = triIndices[num + 5];
			for (int i = num; i < triIndices.Length - 6; i += 6)
			{
				triIndices[i] = triIndices[i + 6];
				triIndices[i + 1] = triIndices[i + 7];
				triIndices[i + 2] = triIndices[i + 8];
				triIndices[i + 3] = triIndices[i + 9];
				triIndices[i + 4] = triIndices[i + 10];
				triIndices[i + 5] = triIndices[i + 11];
				spriteDrawOrder[i / 6] = spriteDrawOrder[i / 6 + 1];
			}
			triIndices[triIndices.Length - 6] = array[0];
			triIndices[triIndices.Length - 5] = array[1];
			triIndices[triIndices.Length - 4] = array[2];
			triIndices[triIndices.Length - 3] = array[3];
			triIndices[triIndices.Length - 2] = array[4];
			triIndices[triIndices.Length - 1] = array[5];
			spriteDrawOrder[spriteDrawOrder.Count - 1] = s;
			vertCountChanged = true;
		}
	}

	public void MoveToBack(SpriteMesh_Managed s)
	{
		int[] array = new int[6];
		int num = spriteDrawOrder.IndexOf(s) * 6;
		if (num >= 0)
		{
			s.drawLayer = spriteDrawOrder[0].drawLayer - 1;
			array[0] = triIndices[num];
			array[1] = triIndices[num + 1];
			array[2] = triIndices[num + 2];
			array[3] = triIndices[num + 3];
			array[4] = triIndices[num + 4];
			array[5] = triIndices[num + 5];
			for (int num2 = num; num2 > 5; num2 -= 6)
			{
				triIndices[num2] = triIndices[num2 - 6];
				triIndices[num2 + 1] = triIndices[num2 - 5];
				triIndices[num2 + 2] = triIndices[num2 - 4];
				triIndices[num2 + 3] = triIndices[num2 - 3];
				triIndices[num2 + 4] = triIndices[num2 - 2];
				triIndices[num2 + 5] = triIndices[num2 - 1];
				spriteDrawOrder[num2 / 6] = spriteDrawOrder[num2 / 6 - 1];
			}
			triIndices[0] = array[0];
			triIndices[1] = array[1];
			triIndices[2] = array[2];
			triIndices[3] = array[3];
			triIndices[4] = array[4];
			triIndices[5] = array[5];
			spriteDrawOrder[0] = s;
			vertCountChanged = true;
		}
	}

	public void MoveInfrontOf(SpriteMesh_Managed toMove, SpriteMesh_Managed reference)
	{
		int[] array = new int[6];
		int num = spriteDrawOrder.IndexOf(toMove) * 6;
		int num2 = spriteDrawOrder.IndexOf(reference) * 6;
		if (num >= 0 && num <= num2)
		{
			toMove.drawLayer = reference.drawLayer + 1;
			array[0] = triIndices[num];
			array[1] = triIndices[num + 1];
			array[2] = triIndices[num + 2];
			array[3] = triIndices[num + 3];
			array[4] = triIndices[num + 4];
			array[5] = triIndices[num + 5];
			for (int i = num; i < num2; i += 6)
			{
				triIndices[i] = triIndices[i + 6];
				triIndices[i + 1] = triIndices[i + 7];
				triIndices[i + 2] = triIndices[i + 8];
				triIndices[i + 3] = triIndices[i + 9];
				triIndices[i + 4] = triIndices[i + 10];
				triIndices[i + 5] = triIndices[i + 11];
				spriteDrawOrder[i / 6] = spriteDrawOrder[i / 6 + 1];
			}
			triIndices[num2] = array[0];
			triIndices[num2 + 1] = array[1];
			triIndices[num2 + 2] = array[2];
			triIndices[num2 + 3] = array[3];
			triIndices[num2 + 4] = array[4];
			triIndices[num2 + 5] = array[5];
			spriteDrawOrder[num2 / 6] = toMove;
			vertCountChanged = true;
		}
	}

	public void MoveBehind(SpriteMesh_Managed toMove, SpriteMesh_Managed reference)
	{
		int[] array = new int[6];
		int num = spriteDrawOrder.IndexOf(toMove) * 6;
		int num2 = spriteDrawOrder.IndexOf(reference) * 6;
		if (num >= 0 && num >= num2)
		{
			toMove.drawLayer = reference.drawLayer - 1;
			array[0] = triIndices[num];
			array[1] = triIndices[num + 1];
			array[2] = triIndices[num + 2];
			array[3] = triIndices[num + 3];
			array[4] = triIndices[num + 4];
			array[5] = triIndices[num + 5];
			for (int num3 = num; num3 > num2; num3 -= 6)
			{
				triIndices[num3] = triIndices[num3 - 6];
				triIndices[num3 + 1] = triIndices[num3 - 5];
				triIndices[num3 + 2] = triIndices[num3 - 4];
				triIndices[num3 + 3] = triIndices[num3 - 3];
				triIndices[num3 + 4] = triIndices[num3 - 2];
				triIndices[num3 + 5] = triIndices[num3 - 1];
				spriteDrawOrder[num3 / 6] = spriteDrawOrder[num3 / 6 - 1];
			}
			triIndices[num2] = array[0];
			triIndices[num2 + 1] = array[1];
			triIndices[num2 + 2] = array[2];
			triIndices[num2 + 3] = array[3];
			triIndices[num2 + 4] = array[4];
			triIndices[num2 + 5] = array[5];
			spriteDrawOrder[num2 / 6] = toMove;
			vertCountChanged = true;
		}
	}

	public void SortDrawingOrder()
	{
		spriteDrawOrder.Sort(drawOrderComparer);
		if (winding == SpriteRoot.WINDING_ORDER.CCW)
		{
			for (int i = 0; i < spriteDrawOrder.Count; i++)
			{
				SpriteMesh_Managed spriteMesh_Managed = spriteDrawOrder[i];
				triIndices[i * 6] = spriteMesh_Managed.mv1;
				triIndices[i * 6 + 1] = spriteMesh_Managed.mv2;
				triIndices[i * 6 + 2] = spriteMesh_Managed.mv4;
				triIndices[i * 6 + 3] = spriteMesh_Managed.mv4;
				triIndices[i * 6 + 4] = spriteMesh_Managed.mv2;
				triIndices[i * 6 + 5] = spriteMesh_Managed.mv3;
			}
		}
		else
		{
			for (int j = 0; j < spriteDrawOrder.Count; j++)
			{
				SpriteMesh_Managed spriteMesh_Managed = spriteDrawOrder[j];
				triIndices[j * 6] = spriteMesh_Managed.mv1;
				triIndices[j * 6 + 1] = spriteMesh_Managed.mv4;
				triIndices[j * 6 + 2] = spriteMesh_Managed.mv2;
				triIndices[j * 6 + 3] = spriteMesh_Managed.mv4;
				triIndices[j * 6 + 4] = spriteMesh_Managed.mv3;
				triIndices[j * 6 + 5] = spriteMesh_Managed.mv2;
			}
		}
		vertCountChanged = true;
	}

	public SpriteMesh_Managed GetSprite(int i)
	{
		if (i < sprites.Length)
		{
			return sprites[i];
		}
		return null;
	}

	public void UpdatePositions()
	{
		vertsChanged = true;
	}

	public void UpdateUVs()
	{
		uvsChanged = true;
	}

	public void UpdateColors()
	{
		colorsChanged = true;
	}

	public void UpdateBounds()
	{
		updateBounds = true;
	}

	public void ScheduleBoundsUpdate(float seconds)
	{
		boundUpdateInterval = seconds;
		InvokeRepeating("UpdateBounds", seconds, seconds);
	}

	public void CancelBoundsUpdate()
	{
		CancelInvoke("UpdateBounds");
	}

	public virtual void LateUpdate()
	{
		if (vertCountChanged)
		{
			vertCountChanged = false;
			colorsChanged = false;
			vertsChanged = false;
			uvsChanged = false;
			updateBounds = false;
			meshRenderer.bones = bones;
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.bindposes = bindPoses;
			mesh.boneWeights = boneWeights;
			mesh.uv = UVs;
			mesh.colors = colors;
			mesh.triangles = triIndices;
			mesh.RecalculateNormals();
			if (autoUpdateBounds)
			{
				mesh.RecalculateBounds();
			}
			return;
		}
		if (vertsChanged)
		{
			vertsChanged = false;
			if (autoUpdateBounds)
			{
				updateBounds = true;
			}
			mesh.vertices = vertices;
		}
		if (updateBounds)
		{
			mesh.RecalculateBounds();
			updateBounds = false;
		}
		if (colorsChanged)
		{
			colorsChanged = false;
			mesh.colors = colors;
		}
		if (uvsChanged)
		{
			uvsChanged = false;
			mesh.uv = UVs;
		}
	}

	public virtual void DoMirror()
	{
		if (Application.isPlaying)
		{
			return;
		}
		if (vertCountChanged)
		{
			vertCountChanged = false;
			colorsChanged = false;
			vertsChanged = false;
			uvsChanged = false;
			updateBounds = false;
			meshRenderer.bones = bones;
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.bindposes = bindPoses;
			mesh.boneWeights = boneWeights;
			mesh.uv = UVs;
			mesh.colors = colors;
			mesh.triangles = triIndices;
			return;
		}
		if (vertsChanged)
		{
			vertsChanged = false;
			updateBounds = true;
			mesh.vertices = vertices;
		}
		if (updateBounds)
		{
			mesh.RecalculateBounds();
			updateBounds = false;
		}
		if (colorsChanged)
		{
			colorsChanged = false;
			mesh.colors = colors;
		}
		if (uvsChanged)
		{
			uvsChanged = false;
			mesh.uv = UVs;
		}
	}

	public virtual void OnDrawGizmos()
	{
		if (drawBoundingBox)
		{
			Gizmos.color = Color.yellow;
			DrawCenter();
			Gizmos.DrawWireCube(meshRenderer.bounds.center, meshRenderer.bounds.size);
		}
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		DrawCenter();
		Gizmos.DrawWireCube(meshRenderer.bounds.center, meshRenderer.bounds.size);
	}

	protected void DrawCenter()
	{
		float a = Mathf.Max(meshRenderer.bounds.size.x, meshRenderer.bounds.size.y);
		a = Mathf.Max(a, meshRenderer.bounds.size.z);
		float num = a * 0.015f;
		Gizmos.DrawLine(meshRenderer.bounds.center - Vector3.up * num, meshRenderer.bounds.center + Vector3.up * num);
		Gizmos.DrawLine(meshRenderer.bounds.center - Vector3.right * num, meshRenderer.bounds.center + Vector3.right * num);
		Gizmos.DrawLine(meshRenderer.bounds.center - Vector3.forward * num, meshRenderer.bounds.center + Vector3.forward * num);
	}
}
