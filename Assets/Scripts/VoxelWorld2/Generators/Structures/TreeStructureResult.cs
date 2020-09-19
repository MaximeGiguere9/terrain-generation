using System;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld2.Generators.Common;
using VoxelWorld2.Utils;

namespace VoxelWorld2.Generators.Structures
{
	public class TreeStructureResult : IBlockGeneratorResult
	{
		private const int LEAVES_RADIUS = 2;

		private readonly int height;
		private readonly Vector3Int offset;
		private readonly Vector3Int size;
		private readonly Dictionary<Vector3Int, byte> blocks;

		public TreeStructureResult(Vector3Int position, int height)
		{
			this.height = height;
			this.offset = position - new Vector3Int(LEAVES_RADIUS, 0, LEAVES_RADIUS);
			this.size = new Vector3Int(LEAVES_RADIUS * 2 + 1, this.height + LEAVES_RADIUS, LEAVES_RADIUS * 2 + 1);
			this.blocks = GenerateStructure();
		}

		public Dictionary<Vector3Int, byte> GenerateStructure()
		{
			Dictionary<Vector3Int, byte> result = new Dictionary<Vector3Int, byte>();

			foreach (Vector3Int pos in new CoordinateIterator(
				new Vector3Int(LEAVES_RADIUS * 2 + 1, 2, LEAVES_RADIUS * 2 + 1), 
				new Vector3Int(0, this.height - 2, 0)
				)
			)
				result[pos] = 5;

			foreach (Vector3Int pos in new CoordinateIterator(
				new Vector3Int(LEAVES_RADIUS + 1, 1, LEAVES_RADIUS + 1), 
				new Vector3Int(LEAVES_RADIUS / 2, this.height, LEAVES_RADIUS / 2)
				)
			)
				result[pos] = 5;

			foreach (Vector3Int pos in new CoordinateIterator(
				new Vector3Int(1, this.height + LEAVES_RADIUS, 1),
				new Vector3Int(LEAVES_RADIUS, 0, LEAVES_RADIUS)
				)
			)
				result[pos] = pos.y < this.height ? (byte) 10 : (byte) 5;

			return result;
		}

		public Vector3Int GetOffset() => this.offset;

		public Vector3Int GetSize() => this.size;

		public byte? GetBlockAt(in Vector3Int position)
		{
			if (position.x < 0 || position.y < 0 || position.z < 0)
				throw new ArgumentOutOfRangeException(nameof(position));

			return this.blocks.TryGetValue(position, out byte block) ? block : (byte?) null;
		}

		public void SetBlockAt(in Vector3Int position, byte blockId)
		{
			throw new NotSupportedException();
		}

		public void Resize(in Vector3Int newSize)
		{
			throw new NotSupportedException();
		}
	}
}