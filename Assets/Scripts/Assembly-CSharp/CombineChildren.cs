using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren : MonoBehaviour
{
	public int frameToWait;

	public bool generateTriangleStrips = true;

	public bool combineOnStart = true;

	public bool destroyAfterOptimized;

	public bool castShadow = true;

	public bool receiveShadow = true;

	public bool keepLayer = true;

	public bool addMeshCollider;

	private void Start()
	{
		if (combineOnStart && frameToWait == 0)
		{
			Combine();
		}
		else
		{
			StartCoroutine(CombineLate());
		}
	}

	private IEnumerator CombineLate()
	{
		for (int i = 0; i < frameToWait; i++)
		{
			yield return 0;
		}
		Combine();
	}

	[ContextMenu("Combine Now on Childs")]
	public void CallCombineOnAllChilds()
	{
		CombineChildren[] componentsInChildren = base.gameObject.GetComponentsInChildren<CombineChildren>();
		int num = componentsInChildren.Length;
		for (int i = 0; i < num; i++)
		{
			if (componentsInChildren[i] != this)
			{
				componentsInChildren[i].Combine();
			}
		}
		bool flag2 = (base.enabled = false);
		combineOnStart = flag2;
	}

	[ContextMenu("Combine Now")]
	public void Combine()
	{
		Component[] componentsInChildren = GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Hashtable hashtable = new Hashtable();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			MeshFilter meshFilter = (MeshFilter)componentsInChildren[i];
			Renderer renderer = componentsInChildren[i].renderer;
			MeshCombineUtility.MeshInstance meshInstance = default(MeshCombineUtility.MeshInstance);
			meshInstance.mesh = meshFilter.sharedMesh;
			if (!(renderer != null) || !renderer.enabled || !(meshInstance.mesh != null))
			{
				continue;
			}
			meshInstance.transform = worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				meshInstance.subMeshIndex = Math.Min(j, meshInstance.mesh.subMeshCount - 1);
				ArrayList arrayList = (ArrayList)hashtable[sharedMaterials[j]];
				if (arrayList != null)
				{
					arrayList.Add(meshInstance);
					continue;
				}
				arrayList = new ArrayList();
				arrayList.Add(meshInstance);
				hashtable.Add(sharedMaterials[j], arrayList);
			}
			if (Application.isPlaying && destroyAfterOptimized && combineOnStart)
			{
				UnityEngine.Object.Destroy(renderer.gameObject);
			}
			else if (destroyAfterOptimized)
			{
				UnityEngine.Object.DestroyImmediate(renderer.gameObject);
			}
			else
			{
				renderer.enabled = false;
			}
		}
		foreach (DictionaryEntry item in hashtable)
		{
			ArrayList arrayList2 = (ArrayList)item.Value;
			MeshCombineUtility.MeshInstance[] combines = (MeshCombineUtility.MeshInstance[])arrayList2.ToArray(typeof(MeshCombineUtility.MeshInstance));
			if (hashtable.Count == 1)
			{
				if (GetComponent(typeof(MeshFilter)) == null)
				{
					base.gameObject.AddComponent(typeof(MeshFilter));
				}
				if (!GetComponent("MeshRenderer"))
				{
					base.gameObject.AddComponent("MeshRenderer");
				}
				MeshFilter meshFilter2 = (MeshFilter)GetComponent(typeof(MeshFilter));
				if (Application.isPlaying)
				{
					meshFilter2.mesh = MeshCombineUtility.Combine(combines, generateTriangleStrips);
				}
				else
				{
					meshFilter2.sharedMesh = MeshCombineUtility.Combine(combines, generateTriangleStrips);
				}
				base.renderer.material = (Material)item.Key;
				base.renderer.enabled = true;
				if (addMeshCollider)
				{
					base.gameObject.AddComponent<MeshCollider>();
				}
				base.renderer.castShadows = castShadow;
				base.renderer.receiveShadows = receiveShadow;
				continue;
			}
			GameObject gameObject = new GameObject("Combined mesh");
			if (keepLayer)
			{
				gameObject.layer = base.gameObject.layer;
			}
			gameObject.transform.parent = base.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.AddComponent(typeof(MeshFilter));
			gameObject.AddComponent("MeshRenderer");
			gameObject.renderer.material = (Material)item.Key;
			MeshFilter meshFilter3 = (MeshFilter)gameObject.GetComponent(typeof(MeshFilter));
			if (Application.isPlaying)
			{
				meshFilter3.mesh = MeshCombineUtility.Combine(combines, generateTriangleStrips);
			}
			else
			{
				meshFilter3.sharedMesh = MeshCombineUtility.Combine(combines, generateTriangleStrips);
			}
			gameObject.renderer.castShadows = castShadow;
			gameObject.renderer.receiveShadows = receiveShadow;
			if (addMeshCollider)
			{
				gameObject.AddComponent<MeshCollider>();
			}
		}
	}
}
