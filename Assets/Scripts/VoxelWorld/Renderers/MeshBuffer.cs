using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.Renderers
{
	public class MeshBuffer
	{
		private readonly Mesh meshBuffer;
		private readonly List<Vector3> verticesBuffer;
		private readonly List<int> trianglesBuffer;
		private readonly List<Vector2> uvsBuffer;

		public MeshBuffer()
		{
			this.meshBuffer = new Mesh();
			this.verticesBuffer = new List<Vector3>();
			this.trianglesBuffer = new List<int>();
			this.uvsBuffer = new List<Vector2>();
		}

		public void Clear()
		{
			this.meshBuffer.Clear();
			this.verticesBuffer.Clear();
			this.trianglesBuffer.Clear();
			this.uvsBuffer.Clear();
		}

		public void Rebuild()
		{
			this.meshBuffer.SetVertices(verticesBuffer);
			this.meshBuffer.SetTriangles(trianglesBuffer, 0);
			this.meshBuffer.SetUVs(0, uvsBuffer);
			this.meshBuffer.RecalculateNormals();
			this.meshBuffer.RecalculateTangents();
		}

		public void GetMesh(out Mesh mesh)
		{
			mesh = this.meshBuffer;
		}

		public Mesh GetMesh()
		{
			return this.meshBuffer;
		}

		public void GetVertexCount(out int count)
		{
			count = this.verticesBuffer.Count;
		}

		public void AddVertices(in IEnumerable<Vector3> verts)
		{
			this.verticesBuffer.AddRange(verts);
		}

		public void AddTriangles(in IEnumerable<int> tris)
		{
			this.trianglesBuffer.AddRange(tris);
		}

		public void AddTriangle(in int tri)
		{
			this.trianglesBuffer.Add(tri);
		}

		public void AddUVs(in IEnumerable<Vector2> uvs)
		{
			this.uvsBuffer.AddRange(uvs);
		}
	}
}
