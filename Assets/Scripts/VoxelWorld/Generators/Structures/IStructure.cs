using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.Generators.Structures
{
	public interface IStructure
	{
		Vector3Int GetOffset();
		Vector3Int GetSize();
		IReadOnlyDictionary<Vector3Int, byte> GetBlocks();
	}
}
