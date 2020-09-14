using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelWorld.Terrain;
using VoxelWorld.Utils;
using VoxelWorld2.Blocks;

namespace VoxelWorld2.Chunks
{
	public class ChunkRenderer
	{
		public const string VIEW_CONTAINER_ID = "VoxelChunkV2";
		public const string VIEW_PREFAB_PATH = "VoxelChunkV2";

		private readonly ChunkController chunkController;
		private readonly BlockService blockService;

		private Transform container;
		private GameObject viewPrefab;
		private ChunkView view;
		private Mesh cachedMesh;

		public ChunkRenderer(ChunkController chunkController)
		{
			this.chunkController = chunkController;
			this.blockService = new BlockService();
		}

		public ChunkView GetView() => this.view;

		public void CreateView()
		{
			if (this.container == null)
				this.container = GameObject.Find(VIEW_CONTAINER_ID).transform;

			if (this.viewPrefab == null)
				this.viewPrefab = Resources.Load<GameObject>(VIEW_PREFAB_PATH);

			GameObject viewObject = Object.Instantiate(
				this.viewPrefab,
				this.chunkController.GetPosition(),
				Quaternion.identity,
				this.container
			);

			this.InvalidateMesh();

			this.view = viewObject.GetComponent<ChunkView>();
			this.view.SetMesh(this.GenerateMesh());
		}

		public void UpdateView()
		{
			this.InvalidateMesh();

			if (this.view != null)
				this.view.SetMesh(this.GenerateMesh());
		}

		public void DestroyView()
		{
			this.view.Destroy();
			this.view = null;
		}

		private void InvalidateMesh() => this.cachedMesh = null;

		private Mesh GenerateMesh()
		{
			if (this.cachedMesh != null) return this.cachedMesh;

			Mesh mesh = new Mesh();

			List<Vector3> vertices = new List<Vector3>(10000);
			List<int> triangles = new List<int>(10000);
			List<Vector2> uvs = new List<Vector2>(10000);

			CoordinateIterator itr = new CoordinateIterator(Vector3Int.one * this.chunkController.GetSize(), Vector3Int.zero);

			var faces = BlockProperties.Faces;
			var faceTris = BlockProperties.FaceTriangles;

			foreach (Vector3Int pos in itr)
			{
				byte block = this.chunkController.GetBlockAt(pos);
				if (block == 0) continue;

				for (int i = 0; i < faces.Length; i++)
				{
					Vector3Int worldPos = pos + this.chunkController.GetPosition() * this.chunkController.GetSize();
					if (!this.blockService.IsFaceVisible(ref VoxelTerrain.ActiveTerrain, worldPos, i))
						continue;

					vertices.AddRange(this.blockService.GetFaceVertices(pos, i));
					uvs.AddRange(this.blockService.GetFaceUVs(block, i));
					triangles.AddRange(faceTris.Select(id => vertices.Count - 1 - id));
				}
			}

			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();

			this.cachedMesh = mesh;
			return this.cachedMesh;
		}
	}
}