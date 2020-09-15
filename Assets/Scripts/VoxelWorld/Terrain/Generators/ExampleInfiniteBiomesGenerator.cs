using System;
using UnityEngine;
using VoxelWorld.Terrain.Generators.Abstractions;
using VoxelWorld.Utils;
using VoxelWorld2.Generators.Common;
using VoxelWorld2.Generators.Terrain;

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

		public void GenerateAll(out IBlockGeneratorResult result) => throw new NotSupportedException();

		public void GenerateAllIntoExisting(ref IBlockGeneratorResult result) => throw new NotSupportedException();

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

		public void Generate(int chunkX, int chunkZ, out IBlockGeneratorResult result)
		{
			Generate(new CoordinateIterator(
				new Vector3Int(this.chunkSize, 1, this.chunkSize),
				new Vector3Int(chunkX * this.chunkSize, 0, chunkZ * this.chunkSize)
			), out result);
		}

		public void Generate(CoordinateIterator iterator, out IBlockGeneratorResult result)
		{
			result = new TerrainGeneratorResult(iterator.offset, iterator.size);
			GenerateIntoExisting(iterator, ref result);
		}

		public void GenerateIntoExisting(int chunkX, int chunkZ, ref IBlockGeneratorResult result)
		{
			GenerateIntoExisting(new CoordinateIterator(
				new Vector3Int(this.chunkSize, 1, this.chunkSize),
				new Vector3Int(chunkX * this.chunkSize, 0, chunkZ * this.chunkSize)
			), ref result);
		}

		public void GenerateIntoExisting(CoordinateIterator iterator, ref IBlockGeneratorResult result)
		{
			foreach (Vector3Int pos in iterator)
			{
				float noise = Mathf.PerlinNoise(pos.x * this.biomeScale, pos.z * this.biomeScale);

				CoordinateIterator itr = new CoordinateIterator(Vector3Int.one, pos);
				
				if (noise > 0.5f)
				{
					this.plateauGenerator.GenerateIntoExisting(itr, ref result);
				}
				else
				{
					this.plainsGenerator.GenerateIntoExisting(itr, ref result);
				}
			}
		}
	}
}