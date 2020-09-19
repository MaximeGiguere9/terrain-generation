using System;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Terrain;
using VoxelWorld2.Generators.Common;
using VoxelWorld2.Utils;

namespace VoxelWorld2.Generators.Terrain.Impl
{
	public class ExampleInfinitePlateauGenerator : ITerrainGenerator
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
		private int waterLevel;

		public bool SupportsInfiniteGeneration() => true;

		public void GenerateAll(out IBlockGeneratorResult result)
		{
			throw new NotSupportedException();
		}

		public void GenerateAllIntoExisting(ref IBlockGeneratorResult result)
		{
			throw new NotSupportedException();
		}

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
			this.waterLevel = VoxelSettings.Instance.WaterLevel;

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
		}

		/// <summary>
		/// Generates a chunk column at the specified chunk coordinates
		/// </summary>
		/// <param name="chunkX"></param>
		/// <param name="chunkZ"></param>
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
			Dictionary<Tuple<int, int>, float> noiseMap = new Dictionary<Tuple<int, int>, float>();

			foreach (Vector3Int pos in iterator)
			{
				float amplitude = 1;
				float frequency = 1;
				float noise = 0;

				for (int i = 0; i < octaves; i++)
				{
					Vector3 samplePos = (Vector3)pos * scale * frequency + octaveOffsets[i];

					float val = Mathf.PerlinNoise(samplePos.x, samplePos.z) * 2 - 1;
					noise += val * amplitude;

					amplitude *= persistence;
					frequency *= lacunarity;
				}

				noiseMap[new Tuple<int, int>(pos.x, pos.z)] = Mathf.Round(noise * 20) / 20;
			}

			iterator.Reset();

			int maxHeight = this.waterLevel;

			HashSet<Vector3Int> heightMap = new HashSet<Vector3Int>();

			foreach (Vector3Int pos in iterator)
			{
				int x = pos.x;
				int z = pos.z;
				Tuple<int, int> key = new Tuple<int, int>(x, z);

				float varianceMult = noiseMap[key];

				float variance = this.baselineHeight * varianceMult;

				int height = Mathf.FloorToInt((noiseMap[key] * 2 - 1) * variance + Mathf.Max(this.baselineHeight - variance, 0));

				heightMap.Add(new Vector3Int(x, height, z));
				if (height > maxHeight) maxHeight = height;
			}

			if (result.GetSize().y < maxHeight)
			{
				Vector3Int res = result.GetSize();
				res.y = maxHeight;
				result.Resize(res);
			}

			foreach (Vector3Int pos in heightMap)
			{
				int x = pos.x - result.GetOffset().x;
				int z = pos.z - result.GetOffset().z;
				int height = pos.y - result.GetOffset().y;

				for (int y = 0; y < height; y++)
				{
					byte blockId;

					if (y == 0) blockId = 2;
					else if (y > height - 5) blockId = 9;
					else blockId = 1;

					result.SetBlockAt(new Vector3Int(x, y, z), blockId);
				}

				for (int y = height; y < this.waterLevel; y++)
				{
					result.SetBlockAt(new Vector3Int(x, y, z), 7);
				}

			}
		}
	}
}