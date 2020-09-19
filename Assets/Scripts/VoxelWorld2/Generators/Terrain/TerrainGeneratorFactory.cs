using System;
using VoxelWorld2.Generators.Terrain.Impl;

namespace VoxelWorld2.Generators.Terrain
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