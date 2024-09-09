using System.IO;
using System.Text;
using UnityEngine;

public class ObjExporter
{
	public static string MeshToString(MeshFilter mf)
	{
		Mesh mesh = mf.mesh;
		Material[] sharedMaterials = mf.renderer.sharedMaterials;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("g ").Append(mf.name).Append("\n");
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector = vertices[i];
			stringBuilder.Append(string.Format("v {0} {1} {2}\n", vector.x, vector.y, vector.z));
		}
		stringBuilder.Append("\n");
		Vector3[] normals = mesh.normals;
		for (int j = 0; j < normals.Length; j++)
		{
			Vector3 vector2 = normals[j];
			stringBuilder.Append(string.Format("vn {0} {1} {2}\n", vector2.x, vector2.y, vector2.z));
		}
		stringBuilder.Append("\n");
		Vector2[] uv = mesh.uv;
		for (int k = 0; k < uv.Length; k++)
		{
			Vector3 vector3 = uv[k];
			stringBuilder.Append(string.Format("vt {0} {1}\n", vector3.x, vector3.y));
		}
		for (int l = 0; l < mesh.subMeshCount; l++)
		{
			stringBuilder.Append("\n");
			stringBuilder.Append("usemtl ").Append(sharedMaterials[l].name).Append("\n");
			stringBuilder.Append("usemap ").Append(sharedMaterials[l].name).Append("\n");
			int[] triangles = mesh.GetTriangles(l);
			for (int m = 0; m < triangles.Length; m += 3)
			{
				stringBuilder.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", triangles[m] + 1, triangles[m + 1] + 1, triangles[m + 2] + 1));
			}
		}
		return stringBuilder.ToString();
	}

	public static void MeshToFile(MeshFilter mf, string filename)
	{
		using (StreamWriter streamWriter = new StreamWriter(filename))
		{
			streamWriter.Write(MeshToString(mf));
		}
	}
}
