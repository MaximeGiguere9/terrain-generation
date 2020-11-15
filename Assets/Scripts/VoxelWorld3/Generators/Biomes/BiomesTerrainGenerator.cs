using UnityEngine;
using Utils;
using VoxelWorld3.Chunks;

namespace VoxelWorld3.Generators.Biomes
{
	public class BiomesTerrainGenerator : TerrainGenerator
	{
		private ITerrainGenerator plainsBiomeGenerator;
		private ITerrainGenerator plateauBiomeGenerator;

		public override void Initialize()
		{
			base.Initialize();

			this.plainsBiomeGenerator = new PlainsBiomeTerrainGenerator();
			this.plainsBiomeGenerator.Initialize();

			this.plateauBiomeGenerator = new PlateauBiomeTerrainGenerator();
			this.plateauBiomeGenerator.Initialize();
		}

		public override void Generate(ref Chunk chunk, CoordinateIterator iterator)
		{
			Vector3Int offset = new Vector3Int(chunk.GetWorldSpacePosition().x, 0, chunk.GetWorldSpacePosition().y);

			foreach (Vector3Int pos in iterator)
			{
				Vector3Int worldPos = pos + offset;

				float noise = Mathf.PerlinNoise(worldPos.x * this.biomeScale, worldPos.z * this.biomeScale);

				CoordinateIterator itr = new CoordinateIterator(Vector3Int.one, pos);

				ITerrainGenerator selectedBiome = noise > 0.5f ? this.plateauBiomeGenerator : this.plainsBiomeGenerator;

				selectedBiome.Generate(ref chunk, itr);
			}
		}
	}
}