using System;
using UnityEngine;
using VoxelWorld2.Generators.Common;

namespace VoxelWorld2.Generators.Terrain
{
	public class TerrainGeneratorResult : IBlockGeneratorResult
	{
		private readonly Vector3Int offset;
		private readonly Vector3Int size;
		private readonly byte[] blocks;

		public TerrainGeneratorResult(Vector3Int offset, Vector3Int size)
		{
			this.offset = offset;
			this.size = size;
			this.blocks = new byte[size.x * size.y * size.z];
		}

		public Vector3Int GetOffset() => this.offset;

		public Vector3Int GetSize() => this.size;

		public byte? GetBlockAt(in Vector3Int position)
		{
			int arrayIndex = GetArrayIndex(this.size, position);
			if(arrayIndex < 0 || arrayIndex >= this.blocks.Length)
				throw new ArgumentOutOfRangeException(nameof(position));
			return this.blocks[arrayIndex];
		}

		public void SetBlockAt(in Vector3Int position, byte blockId)
		{
			int arrayIndex = GetArrayIndex(this.size, position);
			if (arrayIndex < 0 || arrayIndex >= this.blocks.Length)
				throw new ArgumentOutOfRangeException(nameof(position));
			this.blocks[arrayIndex] = blockId;
		}

		private static int GetArrayIndex(Vector3Int size, Vector3Int position) =>
			position.y * size.x * size.z + position.z * size.x + position.x;
	}
}