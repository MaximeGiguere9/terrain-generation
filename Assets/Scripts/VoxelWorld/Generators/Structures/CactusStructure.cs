using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace VoxelWorld.Generators.Structures
{
	public class CactusStructure : IStructure
	{
		private readonly int height;
		private readonly Vector3Int offset;
		private readonly Vector3Int size;
		private readonly Dictionary<Vector3Int, byte> blocks;

		public CactusStructure(Vector3Int position, int height)
		{
			this.height = height;
			this.offset = position;
			this.size = new Vector3Int(1, this.height, 1);
			this.blocks = GenerateStructure();
		}

		private Dictionary<Vector3Int, byte> GenerateStructure()
		{
			Dictionary<Vector3Int, byte> result = new Dictionary<Vector3Int, byte>();

			foreach (Vector3Int pos in new CoordinateIterator(
				new Vector3Int(1, this.height, 1),
				Vector3Int.zero
				)
			)
				result[pos] = 12;

			return result;
		}

		public Vector3Int GetOffset() => this.offset;

		public Vector3Int GetSize() => this.size;

		public IReadOnlyDictionary<Vector3Int, byte> GetBlocks() => this.blocks;
	}
}