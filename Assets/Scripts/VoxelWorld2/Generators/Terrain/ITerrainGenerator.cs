using VoxelWorld2.Generators.Common;
using VoxelWorld2.Utils;

namespace VoxelWorld2.Generators.Terrain
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