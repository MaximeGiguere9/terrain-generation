using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.Chunks
{
	/// <summary>
	/// Encapsulates a bunch of boilerplate code for handling a mesh, since chunks are built using multiple meshes.
	/// </summary>
	public class ChunkMeshModel
	{
		private readonly Mesh meshBuffer;
		private readonly List<Vector3> verticesBuffer;
		private readonly List<int> trianglesBuffer;
		private readonly List<Vector2> uvsBuffer;

		public ChunkMeshModel()
		{
			meshBuffer = new Mesh();
			verticesBuffer = new List<Vector3>();
			trianglesBuffer = new List<int>();
			uvsBuffer = new List<Vector2>();
		}

		public void Clear()
		{
			meshBuffer.Clear();
			verticesBuffer.Clear();
			trianglesBuffer.Clear();
			uvsBuffer.Clear();
		}

		public void Rebuild()
		{
			meshBuffer.SetVertices(verticesBuffer);
			meshBuffer.SetTriangles(trianglesBuffer, 0);
			meshBuffer.SetUVs(0, uvsBuffer);
			meshBuffer.RecalculateNormals();
			meshBuffer.RecalculateTangents();
		}

		public void GetMesh(out Mesh mesh)
		{
			mesh = meshBuffer;
		}

		public Mesh GetMesh()
		{
			return meshBuffer;
		}

		public void GetVertexCount(out int count)
		{
			count = verticesBuffer.Count;
		}

		public void AddVertices(in IEnumerable<Vector3> verts)
		{
			verticesBuffer.AddRange(verts);
		}

		public void AddTriangles(in IEnumerable<int> tris)
		{
			trianglesBuffer.AddRange(tris);
		}

		public void AddTriangle(in int tri)
		{
			trianglesBuffer.Add(tri);
		}

		public void AddUVs(in IEnumerable<Vector2> uvs)
		{
			uvsBuffer.AddRange(uvs);
		}
	}
}
