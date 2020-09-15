using UnityEngine;
using VoxelWorld2.Generators.Common;

namespace VoxelWorld2.Generators.Structures
{
	public class TreeStructure : IBlockGenerator
	{
		public IBlockGeneratorResult Generate(Vector3Int offset) => 
			new TreeStructureResult(offset, Random.Range(3, 8));
	}
}