using UnityEngine;

namespace VoxelWorld2.Generators.Common
{
	public interface IBlockGenerator
	{
		void Generate(Vector3Int offset, out IBlockGeneratorResult result);
	}
}