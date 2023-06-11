using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Utils;

namespace VoxelWorld.Generators.Structures
{
	public class TreeStructure : IStructure
	{
		private const int LEAVES_RADIUS = 2;

		private readonly int height;
		private readonly Vector3Int offset;
		private readonly Vector3Int size;
		private readonly Dictionary<Vector3Int, byte> blocks;

		public TreeStructure(Vector3Int position, int height)
		{
			this.height = height;
			this.offset = position - new Vector3Int(LEAVES_RADIUS, 0, LEAVES_RADIUS);
			this.size = new Vector3Int(LEAVES_RADIUS * 2 + 1, this.height + LEAVES_RADIUS, LEAVES_RADIUS * 2 + 1);
			this.blocks = GenerateStructure();
		}

		private Dictionary<Vector3Int, byte> GenerateStructure()
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

		public IReadOnlyDictionary<Vector3Int, byte> GetBlocks() => this.blocks;
	}
}