using UnityEngine;
using VoxelWorld.Chunks;
using VoxelWorld.Utils;

namespace VoxelWorld.Generators.Biomes
{
	public class BiomesTerrainGenerator : TerrainGenerator
	{
		private ITerrainGenerator plainsBiomeGenerator;
		private ITerrainGenerator plateauBiomeGenerator;
		private ITerrainGenerator snowyBiomeGenerator;

		public override void Initialize()
		{
			base.Initialize();

			this.plainsBiomeGenerator = new PlainsBiomeTerrainGenerator();
			this.plainsBiomeGenerator.Initialize();

			this.plateauBiomeGenerator = new PlateauBiomeTerrainGenerator();
			this.plateauBiomeGenerator.Initialize();

			this.snowyBiomeGenerator = new SnowyBiomeTerrainGenerator();
			this.snowyBiomeGenerator.Initialize();
		}

		public override void Generate(ref Chunk chunk, CoordinateIterator iterator)
		{
			Vector3Int offset = new Vector3Int(chunk.GetWorldSpacePosition().x, 0, chunk.GetWorldSpacePosition().y);

			foreach (Vector3Int pos in iterator)
			{
				Vector3Int worldPos = pos + offset;

				float noise = Mathf.PerlinNoise(worldPos.x * this.biomeScale, worldPos.z * this.biomeScale);

				CoordinateIterator itr = new CoordinateIterator(Vector3Int.one, pos);

				ITerrainGenerator selectedBiome;

				if (noise > 0.5f)
					selectedBiome = this.plateauBiomeGenerator;
				else if (noise < 0.4f)
					selectedBiome = this.snowyBiomeGenerator;
				else
					selectedBiome = this.plainsBiomeGenerator;


				selectedBiome.Generate(ref chunk, itr);
			}
		}
	}
}