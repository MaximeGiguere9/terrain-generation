using UnityEngine;
using VoxelWorld2.Generators.Common;

namespace VoxelWorld2.Generators.Structures
{
	public class TreeStructure : IBlockGenerator
	{
		public void Generate(Vector3Int offset, out IBlockGeneratorResult result) => 
			result = new TreeStructureResult(offset, Random.Range(3, 8));
	}
}