using VoxelWorld.Utils;

namespace VoxelWorld.Terrain.Generators.Abstractions
{
	public interface ITerrainGenerator
	{
		bool SupportsInfiniteGeneration();
		void GenerateAll();
		void Initialize();
		void Generate(int chunkX, int chunkZ);
		void Generate(CoordinateIterator iterator);
	}
}