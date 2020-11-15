using System;
using VoxelWorld3.Generators.Biomes;

namespace VoxelWorld3.Generators
{
	public class TerrainGeneratorFactory
	{
		public static ITerrainGenerator GetTerrainGenerator(TerrainGeneratorType type)
		{
			ITerrainGenerator terrainGenerator;
			switch (type)
			{
				case TerrainGeneratorType.Caverns:
					terrainGenerator = new CaveTerrainGenerator();
					break;
				case TerrainGeneratorType.AllBiomes:
					terrainGenerator = new BiomesTerrainGenerator();
					break;
				case TerrainGeneratorType.PlainsBiome:
					terrainGenerator = new PlainsBiomeTerrainGenerator();
					break;
				case TerrainGeneratorType.PlateauBiome:
					terrainGenerator = new PlateauBiomeTerrainGenerator();
					break;
				default:
					throw new InvalidOperationException();
			}
			terrainGenerator.Initialize();
			return terrainGenerator;
		}
	}
}