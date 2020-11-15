using Utils;
using VoxelWorld3.Chunks;

namespace VoxelWorld3.Generators
{
	public interface ITerrainGenerator
	{
		void Initialize();
		void Generate(ref Chunk chunk);
		void Generate(ref Chunk chunk, CoordinateIterator iterator);
	}
}