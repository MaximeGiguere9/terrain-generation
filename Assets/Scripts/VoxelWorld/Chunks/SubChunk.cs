using UnityEngine;
using Utils;

namespace VoxelWorld.Chunks
{
	public class SubChunk
	{
		private readonly Chunk chunk;
		private readonly byte subdivisionIndex;
		private readonly Vector3Int offset;
		private readonly Vector3Int size;

		private readonly ChunkMeshBuilder chunkRenderer;

		public SubChunk(in Chunk chunk, byte subdivisionIndex)
		{
			this.chunk = chunk;
			this.subdivisionIndex = subdivisionIndex;

			this.chunkRenderer = new ChunkMeshBuilder(chunk, subdivisionIndex);

			this.offset = chunk.GetSize().y / chunk.GetSubdivisionCount() *
			              this.subdivisionIndex * Vector3Int.up;

			this.size = chunk.GetSize();
			this.size.y /= chunk.GetSubdivisionCount();
		}

		public Chunk GetChunk() => this.chunk;

		public ChunkMeshBuilder GetRenderer() => this.chunkRenderer;

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
	}
}