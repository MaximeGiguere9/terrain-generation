using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace VoxelWorld3.Chunks
{
	[Serializable]
	public class Chunk
	{
		[SerializeField] private byte[] blocks;
		[SerializeField] private Vector3Int size;
		[SerializeField] private byte subdivisions;
		[SerializeField] private Vector2Int chunkSpacePosition;
		[SerializeField] private Vector2Int worldSpacePosition;

		/// <summary>
		/// use this when rendering blocks on chunk boundaries so you don't have to go through the entire world
		/// e.g.if the block's x pos is zero, check neighbor at (-1,0)
		/// still O(1) lookup, but less coupling and more control over call stack
		/// </summary>
		private Dictionary<Neighbor, Chunk> neighbors;

		private SubChunk[] subChunks;

		public Chunk(Vector3Int size, byte subdivisions)
		{
			this.size = size;
			this.subdivisions = subdivisions;
			this.neighbors = new Dictionary<Neighbor, Chunk>();

			if(this.size.y % this.subdivisions != 0)
				throw new InvalidOperationException("Cannot subdivide cleanly");

			this.blocks = new byte[this.size.x * this.size.y * this.size.z];

			this.subChunks = new SubChunk[this.subdivisions];
			for (byte i = 0; i < this.subdivisions; i++)
			{
				this.subChunks[i] = new SubChunk(this, i);
			}

		}

		public Vector3Int GetSize()
		{
			return this.size;
		}

		public byte GetSubdivisionCount()
		{
			return this.subdivisions;
		}

		public Vector2Int GetChunkSpacePosition()
		{
			return this.chunkSpacePosition;
		}

		public void SetChunkSpacePosition(Vector2Int position)
		{
			this.chunkSpacePosition = position;
			this.worldSpacePosition = new Vector2Int(position.x * this.size.x, position.y * this.size.z);
		}

		public Vector2Int GetWorldSpacePosition()
		{
			return this.worldSpacePosition;
		}

		public void SetWorldSpacePosition(Vector2Int position)
		{
			if(position.x % this.size.x != 0 || position.y % this.size.z != 0)
				throw new ArgumentException("World space position must be a multiple of chunk size", nameof(position));
			this.worldSpacePosition = position;
			this.chunkSpacePosition = new Vector2Int(position.x / this.size.x, position.y / this.size.z);
		}

		public byte GetBlockAtWorldPosition(in Vector3Int position)
		{
			Vector3Int localPos = new Vector3Int(position.x % this.size.x, position.y, position.z % this.size.z);
			return GetBlockAtLocalPosition(in localPos);
		}

		public void SetBlockAtWorldPosition(in Vector3Int position, byte blockId)
		{
			Vector3Int localPos = new Vector3Int(position.x % this.size.x, position.y, position.z % this.size.z);
			SetBlockAtLocalPosition(in localPos, blockId);
		}

		public byte GetBlockAtLocalPosition(in Vector3Int position)
		{
			int offset = GetOffset(in position, in this.size);
			if (offset < 0 || offset >= this.blocks.Length)
				throw new ArgumentOutOfRangeException(nameof(position));
			return this.blocks[offset];
		}

		public void SetBlockAtLocalPosition(in Vector3Int position, byte blockId)
		{
			int offset = GetOffset(in position, in this.size);
			if(offset < 0 || offset >= this.blocks.Length)
				throw new ArgumentOutOfRangeException(nameof(position));
			this.blocks[offset] = blockId;
		}

		public SubChunk[] GetSubChunks()
		{
			return this.subChunks;
		}

		public SubChunk GetSubChunk(byte index)
		{
			if(index >= this.subdivisions)
				throw new ArgumentOutOfRangeException(nameof(index));
			return this.subChunks[index];
		}

		[CanBeNull]
		public Chunk GetNeighbor(Neighbor neighborPos)
		{
			return this.neighbors.TryGetValue(neighborPos, out Chunk chunk) ? chunk : null;
		}

		public void SetNeighbor(Neighbor neighborPos, Chunk neighborChunk)
		{
			this.neighbors[neighborPos] = neighborChunk;
		}


		public static int GetOffset(in Vector3Int position, in Vector3Int size) =>
			position.y * size.x * size.z + position.z * size.x + position.x;

	}
}