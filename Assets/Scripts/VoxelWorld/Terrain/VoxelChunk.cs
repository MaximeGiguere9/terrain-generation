using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelWorld.Utils;
using VoxelWorld2.Blocks;

namespace VoxelWorld.Terrain
{
	public class VoxelChunk : MonoBehaviour
	{
		public static bool DebugMode { get; set; } = false;

		private static int ChunkSize;

		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private MeshCollider meshCollider;

		private readonly BlockService blockService = new BlockService();

		private byte[,,] blocks;

		public bool Loaded { get; set; }

		private Vector3Int position;
		public Vector3Int Position
		{
			get => this.position;
			set
			{
				if (this.position == value) return;
				this.position = value;
				transform.position = value * ChunkSize;
			}
		}

		private void Awake()
		{
			ChunkSize = VoxelSettings.Instance.ChunkSize;
			this.blocks = new byte[ChunkSize, ChunkSize, ChunkSize];
			this.blockService.LoadBlocks("blocks");
		}

		public void SetBlockAt(Vector3Int position, byte blockId)
		{
			Vector3Int relPos = position - this.Position * ChunkSize;
			this.blocks[relPos.x, relPos.y, relPos.z] = blockId;
		}

		public byte GetBlockAt(Vector3Int position)
		{
			Vector3Int relPos = position - this.Position * ChunkSize;
			return this.blocks[relPos.x, relPos.y, relPos.z];
		}

		public byte RemoveBlockAt(Vector3Int position)
		{
			Vector3Int relPos = position - this.Position * ChunkSize;
			byte block = this.blocks[relPos.x, relPos.y, relPos.z];
			this.blocks[relPos.x, relPos.y, relPos.z] = 0;
			return block;
		}

		private Vector3Int GetWorldPosition(Vector3Int relPos) => relPos + this.Position * ChunkSize;

		public void Redraw()
		{
			if (!this.Loaded)
			{
				Destroy(this.gameObject);
				return;
			}

			Mesh mesh = new Mesh();

			List<Vector3> vertices = new List<Vector3>(10000);
			List<int> triangles = new List<int>(10000);
			List<Vector2> uvs = new List<Vector2>(10000);

			CoordinateIterator itr = new CoordinateIterator(Vector3Int.one * ChunkSize, Vector3Int.zero);

			var faces = BlockProperties.Faces;
			var faceTris = BlockProperties.FaceTriangles;

			foreach (Vector3Int pos in itr)
			{
				byte block = this.blocks[pos.x, pos.y, pos.z];
				if (block == 0) continue;

				for (int i = 0; i < faces.Length; i++)
				{
					if (pos.x > 0 && pos.x < ChunkSize - 1 && pos.y > 0 && pos.y < ChunkSize - 1 && pos.z > 0 &&
					    pos.z < ChunkSize - 1)
					{
						Vector3Int neighborPos = pos + faces[i];
						if (!blockService.IsFaceVisible(this.blocks[pos.x, pos.y, pos.z],  this.blocks[neighborPos.x, neighborPos.y, neighborPos.z])) continue;
					}
					else
					{
						if (!blockService.IsFaceVisible(ref VoxelTerrain.ActiveTerrain, GetWorldPosition(pos), i)) continue;
					}

					

					vertices.AddRange(blockService.GetFaceVertices(pos, i));
					uvs.AddRange(blockService.GetFaceUVs(block, i));
					triangles.AddRange(faceTris.Select(id => vertices.Count - 1 - id));
				}
			}

			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();

			this.meshFilter.mesh = mesh;
			this.meshCollider.sharedMesh = this.meshFilter.sharedMesh;

			if(DebugMode) Debug.Log($"Redrew chunk at {this.position}");
		}
	}
}
