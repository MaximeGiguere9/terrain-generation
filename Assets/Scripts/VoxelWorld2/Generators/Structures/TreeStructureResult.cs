using System;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Utils;
using VoxelWorld2.Generators.Common;

namespace VoxelWorld2.Generators.Structures
{
	public class TreeStructureResult : IBlockGeneratorResult
	{
		private const int LEAVES_RADIUS = 2;

		private readonly Vector3Int position;
		private readonly int height;
		private readonly Dictionary<Vector3Int, byte> leaves;

		public TreeStructureResult(Vector3Int position, int height)
		{
			this.position = position;
			this.height = height;
			this.leaves = GenerateLeavesPattern();
		}

		public Dictionary<Vector3Int, byte> GenerateLeavesPattern()
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
				new Vector3Int(LEAVES_RADIUS/2, this.height, LEAVES_RADIUS / 2)
				)
			)
				result[pos] = 5;

			return result;
		}

		public Vector3Int GetOffset() => 
			this.position - new Vector3Int(LEAVES_RADIUS, 0, LEAVES_RADIUS);

		public Vector3Int GetSize() =>
			new Vector3Int(LEAVES_RADIUS * 2 + 1, this.height + LEAVES_RADIUS, LEAVES_RADIUS * 2 + 1);

		public byte? GetBlockAt(Vector3Int position)
		{
			if (position.x < 0 || position.y < 0 || position.z < 0)
				throw new ArgumentOutOfRangeException(nameof(position));

			if (position.x == LEAVES_RADIUS && position.z == LEAVES_RADIUS)
				return position.y < this.height ? (byte) 10 : (byte) 5;

			if (this.leaves.ContainsKey(position))
				return this.leaves[position];

			return null;
		}
	}
}