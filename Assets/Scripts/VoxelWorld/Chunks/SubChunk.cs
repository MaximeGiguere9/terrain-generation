using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using VoxelWorld.Blocks;

namespace VoxelWorld.Chunks
{
	public class SubChunk
	{
		public event Action OnMeshInvalidated;

		private readonly Chunk chunk;
		private readonly byte subdivisionIndex;
		private readonly Vector3Int offset;
		private readonly Vector3Int size;

		private readonly BlockService blockService;

		private bool meshInvalidated;

		private Vector3Int positionBuffer;
		private Vector3[] faceVerticesArrayBuffer;
		private Vector2[] faceUVsArrayBuffer;
		private readonly Mesh meshBuffer;
		private readonly List<Vector3> verticesBuffer;
		private readonly List<int> trianglesBuffer;
		private readonly List<Vector2> uvsBuffer;
		private readonly CoordinateIterator itrBuffer;

		public SubChunk(in Chunk chunk, byte subdivisionIndex)
		{
			this.blockService = BlockService.Instance;

			this.chunk = chunk;
			this.subdivisionIndex = subdivisionIndex;

			this.offset = chunk.GetSize().y / chunk.GetSubdivisionCount() *
			              this.subdivisionIndex * Vector3Int.up;

			this.size = chunk.GetSize();
			this.size.y /= chunk.GetSubdivisionCount();


			this.positionBuffer = new Vector3Int();
			this.faceVerticesArrayBuffer = this.blockService.GetFaceVerticesArrayBuffer();
			this.faceUVsArrayBuffer = this.blockService.GetFaceUVsArrayBuffer();

			this.meshBuffer = new Mesh();

			this.verticesBuffer = new List<Vector3>(5000);
			this.trianglesBuffer = new List<int>(5000);
			this.uvsBuffer = new List<Vector2>(5000);

			this.itrBuffer = GetLocalSpaceIterator();
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
			this.meshInvalidated = true;
			OnMeshInvalidated?.Invoke();
		}

		public Mesh GetMesh()
		{
			if (this.meshInvalidated)
				GenerateMesh();
			return this.meshBuffer;
		}

		private void GenerateMesh()
		{
			
			this.meshBuffer.Clear();
			this.verticesBuffer.Clear();
			this.trianglesBuffer.Clear();
			this.uvsBuffer.Clear();

			var faces = this.blockService.GetFaceOrder();
			var faceTris = this.blockService.GetFaceTriangleOrder();

			for (int y = itrBuffer.offset.y; y <= itrBuffer.target.y; y++)
			{
				positionBuffer.y = y;

				for (int z = itrBuffer.offset.z; z <= itrBuffer.target.z; z++)
				{
					positionBuffer.z = z;

					for (int x = itrBuffer.offset.x; x <= itrBuffer.target.x; x++)
					{
						positionBuffer.x = x;

						byte block = this.chunk.GetBlockAtLocalPosition(in x, in y, in z);
						if (block == 0) continue;

						BlockModel blockModel = this.blockService.GetBlockModel(block);

						for (int i = 0; i < faces.Length; i++)
						{
							int neighborPosX = x + faces[i].x;
							int neighborPosY = y + faces[i].y;
							int neighborPosZ = z + faces[i].z;
							Chunk neighborChunk = this.chunk;

							if (neighborPosX < 0)
								neighborChunk = this.chunk.GetNeighbor(Neighbor.West);
							else if (neighborPosX >= this.chunk.GetSize().x)
								neighborChunk = this.chunk.GetNeighbor(Neighbor.East);
							else if (neighborPosZ < 0)
								neighborChunk = this.chunk.GetNeighbor(Neighbor.North);
							else if (neighborPosZ >= this.chunk.GetSize().z)
								neighborChunk = this.chunk.GetNeighbor(Neighbor.South);

							byte? neighborBlock = null;

							if (neighborPosY >= 0 && neighborPosY < this.chunk.GetSize().y)
							{
								if (neighborChunk == this.chunk)
								{
									neighborBlock = this.chunk.GetBlockAtLocalPosition(neighborPosX, neighborPosY, neighborPosZ);
								}
								else if (neighborChunk != null)
								{
									Vector3Int neighborSize = neighborChunk.GetSize();
									neighborBlock = neighborChunk.GetBlockAtLocalPosition(
										MathUtils.Mod(neighborPosX, neighborSize.x),
										neighborPosY,
										MathUtils.Mod(neighborPosZ, neighborSize.z)
									);
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

							this.blockService.GetFaceVertices(positionBuffer, i, ref faceVerticesArrayBuffer);
							verticesBuffer.AddRange(faceVerticesArrayBuffer);
							this.blockService.GetFaceUVs(block, i, ref faceUVsArrayBuffer);
							uvsBuffer.AddRange(faceUVsArrayBuffer);

							foreach (byte id in faceTris)
								trianglesBuffer.Add(verticesBuffer.Count - 1 - id);
						}
					}
				}
			}

			meshBuffer.vertices = verticesBuffer.ToArray();
			meshBuffer.triangles = trianglesBuffer.ToArray();
			meshBuffer.uv = uvsBuffer.ToArray();
			meshBuffer.RecalculateNormals();
			meshBuffer.RecalculateTangents();

			this.meshInvalidated = false;
		}
	}
}