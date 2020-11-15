using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using VoxelWorld3.Blocks;
using BlockService = VoxelWorld3.Blocks.BlockService;

namespace VoxelWorld3.Chunks
{
	public class SubChunk
	{
		public event Action OnMeshInvalidated;

		private readonly Chunk chunk;
		private readonly byte subdivisionIndex;
		private readonly Vector3Int offset;
		private readonly Vector3Int size;

		private readonly BlockService blockService;

		private Mesh cachedMesh;

		public SubChunk(in Chunk chunk, byte subdivisionIndex)
		{
			this.blockService = BlockService.Instance;

			this.chunk = chunk;
			this.subdivisionIndex = subdivisionIndex;

			this.offset = chunk.GetSize().y / chunk.GetSubdivisionCount() *
			              this.subdivisionIndex * Vector3Int.up;

			this.size = chunk.GetSize();
			this.size.y /= chunk.GetSubdivisionCount();
		}

		public Chunk GetChunk() => this.chunk;

		public byte GetSubdivisionIndex() => this.subdivisionIndex;

		public CoordinateIterator GetLocalSpaceIterator()
		{
			return new CoordinateIterator(this.size, this.offset);
		}

		public CoordinateIterator GetWorldSpaceIterator()
		{
			Vector2Int chunkPos = this.chunk.GetWorldSpacePosition();
			return new CoordinateIterator(this.size, new Vector3Int(chunkPos.x, this.offset.y, chunkPos.y));
		}

		public void InvalidateMesh()
		{
			this.cachedMesh = null;
			OnMeshInvalidated?.Invoke();
		}

		public Mesh GetMesh()
		{
			if (this.cachedMesh == null)
				this.cachedMesh = GenerateMesh();
			return this.cachedMesh;
		}

		private Mesh GenerateMesh()
		{
			Mesh mesh = new Mesh();

			List<Vector3> vertices = new List<Vector3>(10000);
			List<int> triangles = new List<int>(10000);
			List<Vector2> uvs = new List<Vector2>(10000);

			CoordinateIterator itr = GetLocalSpaceIterator();

			var faces = this.blockService.GetFaceOrder();
			var faceTris = this.blockService.GetFaceTriangleOrder();

			foreach (Vector3Int pos in itr)
			{
				byte block = this.chunk.GetBlockAtLocalPosition(pos);
				if (block == 0) continue;

				BlockModel blockModel = this.blockService.GetBlockModel(block);

				for (int i = 0; i < faces.Length; i++)
				{
					Vector3Int neighborPos = pos + faces[i];
					Chunk neighborChunk = this.chunk;

					if (neighborPos.x < 0)
						neighborChunk = this.chunk.GetNeighbor(Neighbor.West);
					else if (neighborPos.x >= this.chunk.GetSize().x)
						neighborChunk = this.chunk.GetNeighbor(Neighbor.East);
					else if (neighborPos.z < 0)
						neighborChunk = this.chunk.GetNeighbor(Neighbor.North);
					else if (neighborPos.z >= this.chunk.GetSize().z)
						neighborChunk = this.chunk.GetNeighbor(Neighbor.South);

					byte? neighborBlock = null;

					if (neighborPos.y >= 0 && neighborPos.y < this.chunk.GetSize().y)
					{
						if (neighborChunk == this.chunk)
						{
							neighborBlock = this.chunk.GetBlockAtLocalPosition(neighborPos);
						}
						else if (neighborChunk != null)
						{
							Vector3Int neighborSize = neighborChunk.GetSize();
							Vector3Int neighborBlockLocalPos = new Vector3Int(
								MathUtils.Mod(neighborPos.x, neighborSize.x),
								neighborPos.y,
								MathUtils.Mod(neighborPos.z, neighborSize.z)
							);
							neighborBlock = neighborChunk.GetBlockAtLocalPosition(neighborBlockLocalPos);
						}
					}

					if (neighborBlock.HasValue && neighborBlock.Value > 0)
					{
						BlockModel neighborBlockModel = this.blockService.GetBlockModel(neighborBlock.Value);

						if (blockModel == null || neighborBlockModel == null)
							throw new InvalidOperationException("missing block model(s)");

						bool isFaceVisible = neighborBlockModel.Transparent && !(block == neighborBlock.Value && blockModel.HideConnectingFaces);

						if (!isFaceVisible) continue;
					}

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

			return mesh;
		}
	}
}