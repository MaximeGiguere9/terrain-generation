using Utils;
using VoxelWorld.Chunks;

namespace VoxelWorld.Generators
{
	public interface ITerrainGenerator
	{
		void Initialize();
		void Generate(ref Chunk chunk);
		void Generate(ref Chunk chunk, CoordinateIterator iterator);
	}
}