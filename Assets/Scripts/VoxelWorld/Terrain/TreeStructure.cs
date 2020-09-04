using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Utils;

namespace VoxelWorld.Terrain
{
	public class TreeStructure
	{
		public static Dictionary<Vector3Int, byte> Generate()
		{
			Dictionary<Vector3Int, byte> result = new Dictionary<Vector3Int, byte>();

			foreach (Vector3Int pos in new CoordinateIterator(new Vector3Int(5, 2, 5), new Vector3Int(-2, 3, -2)))
			{
				result[pos] = 5;
			}

			foreach (Vector3Int pos in new CoordinateIterator(new Vector3Int(3, 1, 3), new Vector3Int(-1, 5, -1)))
			{
				result[pos] = 5;
			}

			result[new Vector3Int(0, 6, 0)] = 5;

			result[new Vector3Int(0, 0, 0)] = 10;
			result[new Vector3Int(0, 1, 0)] = 10;
			result[new Vector3Int(0, 2, 0)] = 10;
			result[new Vector3Int(0, 3, 0)] = 10;
			result[new Vector3Int(0, 4, 0)] = 10;

			return result;
		}
	}
}