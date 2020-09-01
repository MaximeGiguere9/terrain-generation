namespace VoxelWorld.Terrain.Generators.Abstractions
{
	public interface ITerrainGenerator
	{
		bool SupportsInfiniteGeneration();
		void GenerateAll();
		void Initialize();
		void GenerateVerticalChunks(int chunkX, int chunkZ);
	}
}