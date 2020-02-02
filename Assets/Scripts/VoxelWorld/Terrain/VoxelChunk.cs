using System;
using System.Collections.Generic;
using Noise;
using UnityEngine;
using VoxelWorld.Utils;

namespace VoxelWorld.Terrain
{
	public class VoxelChunk : MonoBehaviour
	{
		private static int ChunkSize => VoxelWorldSettings.Instance.ChunkSize;

		private VoxelBlock[,,] blocks;

		public Vector3Int Position { get; set; }

		private void Awake()
		{
			this.blocks = new VoxelBlock[ChunkSize, ChunkSize, ChunkSize];
		}

		public void AddBlock(VoxelBlock block)
		{
			Vector3Int relPos = block.Position - this.Position * ChunkSize;
			this.blocks[relPos.x, relPos.y, relPos.z] = block;
		}

		public VoxelBlock GetBlockAt(Vector3Int position)
		{
			Vector3Int relPos = position - this.Position * ChunkSize;
			return this.blocks[relPos.x, relPos.y, relPos.z];
		}

		public VoxelBlock RemoveBlockAt(Vector3Int position)
		{
			Vector3Int relPos = position - this.Position * ChunkSize;
			VoxelBlock block = this.blocks[relPos.x, relPos.y, relPos.z];
			this.blocks[relPos.x, relPos.y, relPos.z] = null;
			return block;
		}

		public void Redraw()
		{
			Mesh mesh = new Mesh();

			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();

			CoordinateIterator itr = new CoordinateIterator(Vector3Int.one * ChunkSize, Vector3Int.zero);

			foreach (Vector3Int pos in itr)
			{
				VoxelBlock block = this.blocks[pos.x, pos.y, pos.z];
				if (block == null) continue;

				block.UpdateVisibility();

				for (int i = 0; i < VoxelBlock.Faces.Length; i++)
				{
					if (!block.IsFaceVisible(i)) continue;

					byte[] tris = VoxelBlock.Triangles[i];
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
						vertices.Add(block.Position + VoxelBlock.Vertices[vertexId]);
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
