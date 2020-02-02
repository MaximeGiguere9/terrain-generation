using System.Collections.Generic;
using Noise;
using UnityEngine;

namespace VoxelWorld
{
	public class VoxelChunk : MonoBehaviour
	{
		public const int ChunkSize = 16;

		private readonly Block[,,] blocks = new Block[ChunkSize, ChunkSize, ChunkSize];

		public Vector3Int Position { get; set; }

		public void AddBlock(Block block)
		{
			Vector3Int relPos = block.Position - this.Position * ChunkSize;
			this.blocks[relPos.x, relPos.y, relPos.z] = block;
		}

		public Block GetBlockAt(Vector3Int position)
		{
			Vector3Int relPos = position - this.Position * ChunkSize;
			return this.blocks[relPos.x, relPos.y, relPos.z];
		}

		public void RemoveBlockAt(Vector3Int position)
		{
			Vector3Int relPos = position - this.Position * ChunkSize;
			this.blocks[relPos.x, relPos.y, relPos.z] = null;
		}

		public void Redraw()
		{
			Mesh mesh = new Mesh();

			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();

			CoordinateIterator itr = new CoordinateIterator(Vector3Int.one * ChunkSize, Vector3Int.zero);

			foreach (Vector3Int pos in itr)
			{
				Block block = this.blocks[pos.x, pos.y, pos.z];
				if (block == null) continue;

				block.UpdateVisibility();

				for (int i = 0; i < Block.Faces.Length; i++)
				{
					if ((block.VisibleFaces | Block.Bits[i]) != block.VisibleFaces) continue;

					byte[] tris = Block.TrianglesTable[i];
					foreach (byte vertexId in tris)
					{
						//weld vertices (leads to smooth shading)
						/*
						Vector3 v = block.Position + Block.Vertices[vertexId];
						int vi = vertices.IndexOf(v);
						if (vi == -1)
						{
							triangles.Add(vertices.Count);
							vertices.Add(v);
						}
						else
						{
							triangles.Add(vi);
						}*/

						triangles.Add(vertices.Count);
						vertices.Add(block.Position + Block.Vertices[vertexId]);
					}
				}
			}

			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();

			GetComponent<MeshFilter>().mesh = mesh;

			GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
		}
	}
}
