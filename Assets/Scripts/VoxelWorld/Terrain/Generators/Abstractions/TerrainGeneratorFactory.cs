using System;

namespace VoxelWorld.Terrain.Generators.Abstractions
{
	public class TerrainGeneratorFactory
	{
		public static ITerrainGenerator GetTerrainGenerator(TerrainGeneratorType type)
		{
			switch (type)
			{
				case TerrainGeneratorType.ExampleInfiniteTerrain:
					return new ExampleInfiniteTerrainGenerator();
				case TerrainGeneratorType.ExampleCave:
					return new ExampleCaveGenerator();
				case TerrainGeneratorType.ExampleTerrain:
					return new ExampleTerrainGenerator();
				default:
					throw new InvalidOperationException();
			}
		}
	}
}