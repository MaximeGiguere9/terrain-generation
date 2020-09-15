using UnityEngine;

namespace VoxelWorld2.Generators.Common
{
	public interface IBlockGenerator
	{
		IBlockGeneratorResult Generate(Vector3Int offset);
	}
}