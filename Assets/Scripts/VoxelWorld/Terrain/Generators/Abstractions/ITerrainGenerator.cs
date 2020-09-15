using VoxelWorld.Utils;
using VoxelWorld2.Generators.Common;

namespace VoxelWorld.Terrain.Generators.Abstractions
{
	public interface ITerrainGenerator
	{
		bool SupportsInfiniteGeneration();
		void GenerateAll(out IBlockGeneratorResult result);
		void GenerateAllIntoExisting(ref IBlockGeneratorResult result);
		void Initialize();
		void Generate(int chunkX, int chunkZ, out IBlockGeneratorResult result);
		void Generate(CoordinateIterator iterator, out IBlockGeneratorResult result);
		void GenerateIntoExisting(int chunkX, int chunkZ, ref IBlockGeneratorResult result);
		void GenerateIntoExisting(CoordinateIterator iterator, ref IBlockGeneratorResult result);
	}
}