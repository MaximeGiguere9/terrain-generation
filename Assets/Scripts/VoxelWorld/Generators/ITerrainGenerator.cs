using VoxelWorld.Chunks;
using VoxelWorld.Utils;

namespace VoxelWorld.Generators
{
	public interface ITerrainGenerator
	{
		void Initialize();
		void Generate(ref Chunk chunk);
		void Generate(ref Chunk chunk, CoordinateIterator iterator);
	}
}