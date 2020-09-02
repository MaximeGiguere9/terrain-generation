using System;

namespace VoxelWorld.Terrain.Generators.Abstractions
{
	public class TerrainGeneratorFactory
	{
		public static ITerrainGenerator GetTerrainGenerator(TerrainGeneratorType type)
		{
			switch (type)
			{
				case TerrainGeneratorType.ExampleCave:
					return new ExampleCaveGenerator();
				case TerrainGeneratorType.ExampleTerrain:
					return new ExampleTerrainGenerator();
				case TerrainGeneratorType.ExampleInfiniteTerrain:
					return new ExampleInfiniteTerrainGenerator();
				case TerrainGeneratorType.ExampleInfinitePlateau:
					return new ExampleInfinitePlateauGenerator();
				case TerrainGeneratorType.ExampleInfiniteBiomes:
					return new ExampleInfiniteBiomesGenerator();
				default:
					throw new InvalidOperationException();
			}
		}
	}
}