using UnityEngine;
using Utils;
using VoxelWorld2.Utils;
using VoxelWorld3.Chunks;

namespace VoxelWorld3.Generators
{
	public abstract class TerrainGenerator : ITerrainGenerator
	{
		protected int seed;
		protected int octaves;
		protected Vector3 noiseOffset;
		protected float scale;
		protected float persistence;
		protected float lacunarity;
		protected System.Random rng;
		protected Vector3[] octaveOffsets;
		protected int baselineHeight;
		protected int waterLevel;
		protected float biomeScale;
		protected AnimationCurve density;
		protected float cavernScale;

		public virtual void Initialize()
		{
			this.seed = TerrainGeneratorSettings.Instance.Seed;
			this.octaves = TerrainGeneratorSettings.Instance.Octaves;
			this.noiseOffset = TerrainGeneratorSettings.Instance.Offset;
			this.scale = TerrainGeneratorSettings.Instance.Scale;
			this.persistence = TerrainGeneratorSettings.Instance.Persistence;
			this.lacunarity = TerrainGeneratorSettings.Instance.Lacunarity;
			this.baselineHeight = TerrainGeneratorSettings.Instance.BaselineHeight;
			this.waterLevel = TerrainGeneratorSettings.Instance.WaterLevel;
			this.biomeScale = TerrainGeneratorSettings.Instance.BiomeScale;
			this.density = TerrainGeneratorSettings.Instance.Density;
			this.cavernScale = TerrainGeneratorSettings.Instance.CavernScale;

			this.rng = new System.Random(seed);

			this.octaveOffsets = new Vector3[octaves];
			for (int i = 0; i < octaves; i++)
			{
				octaveOffsets[i] = new Vector3(
					rng.Next(-100000, 100000) + noiseOffset.x,
					rng.Next(-100000, 100000) + noiseOffset.y,
					rng.Next(-100000, 100000) + noiseOffset.z
				);
			}
		}

		public virtual void Generate(ref Chunk chunk)
		{
			CoordinateIterator iterator = new CoordinateIterator(
				new Vector3Int(chunk.GetSize().x, 1, chunk.GetSize().z),
				Vector3Int.zero
			);
			Generate(ref chunk, iterator);
		}

		public virtual void Generate(ref Chunk chunk, CoordinateIterator iterator)
		{
			//do nothing
		}
	}
}