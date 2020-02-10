using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelWorld.Utils;

namespace VoxelWorld.Terrain
{
	public class VoxelChunk : MonoBehaviour
	{
		private static int ChunkSize => VoxelWorldSettings.Instance.ChunkSize;

		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private MeshCollider meshCollider;

		private byte[,,] blocks;

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
			this.blocks = new byte[ChunkSize, ChunkSize, ChunkSize];
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
			Mesh mesh = new Mesh();

			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<Vector2> uvs = new List<Vector2>();

			CoordinateIterator itr = new CoordinateIterator(Vector3Int.one * ChunkSize, Vector3Int.zero);

			foreach (Vector3Int pos in itr)
			{
				byte block = this.blocks[pos.x, pos.y, pos.z];
				if (block == 0) continue;

				for (int i = 0; i < VoxelBlock.Faces.Length; i++)
				{
					if(!VoxelBlock.IsFaceVisible(GetWorldPosition(pos), i)) continue;

					vertices.AddRange(VoxelBlock.GetFaceVertices(pos, i));
					uvs.AddRange(VoxelBlock.GetFaceUVs(block, i));
					triangles.AddRange(VoxelBlock.FaceTriangles.Select(id => vertices.Count - 1 - id));
				}
			}

			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();

			this.meshFilter.mesh = mesh;
			this.meshCollider.sharedMesh = this.meshFilter.sharedMesh;
		}
	}
}
