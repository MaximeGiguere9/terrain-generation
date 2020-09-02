using System;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Terrain.Generators.Abstractions;
using VoxelWorld.Utils;
using Random = UnityEngine.Random;

namespace VoxelWorld.Terrain.Generators
{
	public class ExampleInfiniteBiomesGenerator : ITerrainGenerator
	{
		private int seed;
		private int octaves;
		private Vector3 offset;
		private float scale;
		private float persistence;
		private float lacunarity;
		private System.Random rng;
		private Vector3[] octaveOffsets;
		private int chunkSize;
		private int baselineHeight;
		private float biomeScale;

		private ITerrainGenerator plainsGenerator;
		private ITerrainGenerator plateauGenerator;

		public bool SupportsInfiniteGeneration() => true;

		public void GenerateAll() => throw new NotSupportedException();

		public void Initialize()
		{
			this.seed = VoxelSettings.Instance.Seed;
			this.octaves = VoxelSettings.Instance.Octaves;
			this.offset = VoxelSettings.Instance.Offset;
			this.scale = VoxelSettings.Instance.Scale;
			this.persistence = VoxelSettings.Instance.Persistence;
			this.lacunarity = VoxelSettings.Instance.Lacunarity;
			this.chunkSize = VoxelSettings.Instance.ChunkSize;
			this.baselineHeight = VoxelSettings.Instance.BaselineHeight;
			this.biomeScale = VoxelSettings.Instance.BiomeScale;

			this.rng = new System.Random(seed);

			this.octaveOffsets = new Vector3[octaves];
			for (int i = 0; i < octaves; i++)
			{
				octaveOffsets[i] = new Vector3(
					rng.Next(-100000, 100000) + offset.x,
					rng.Next(-100000, 100000) + offset.y,
					rng.Next(-100000, 100000) + offset.z
				);
			}

			this.plainsGenerator = new ExampleInfiniteTerrainGenerator();
			this.plainsGenerator.Initialize();

			this.plateauGenerator = new ExampleInfinitePlateauGenerator();
			this.plateauGenerator.Initialize();
		}

		public void GenerateVerticalChunks(int chunkX, int chunkZ)
		{
			float noise = Mathf.PerlinNoise(chunkX * this.biomeScale, chunkZ * this.biomeScale);
			if (noise > 0.5f)
			{
				this.plateauGenerator.GenerateVerticalChunks(chunkX, chunkZ);
			}
			else
			{
				this.plainsGenerator.GenerateVerticalChunks(chunkX, chunkZ);
			}
		}
	}
}