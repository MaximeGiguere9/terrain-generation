using UnityEngine;
using VoxelWorld.Terrain.Generators.Abstractions;
using VoxelWorld.Utils;
using VoxelWorld2.Generators.Common;
using VoxelWorld2.Generators.Terrain;

namespace VoxelWorld.Terrain.Generators
{
	public class ExampleCaveGenerator : ITerrainGenerator
	{
		private float[,,] noiseMap;

		public bool SupportsInfiniteGeneration() => false;

		public void GenerateAll(out IBlockGeneratorResult result)
		{
			Vector3Int size = VoxelSettings.Instance.WorldSize;
			result = new TerrainGeneratorResult(Vector3Int.zero, size);
			GenerateAllIntoExisting(ref result);
		}

		public void GenerateAllIntoExisting(ref IBlockGeneratorResult result)
		{
			int seed = VoxelSettings.Instance.Seed;
			int octaves = VoxelSettings.Instance.Octaves;
			Vector3 offset = VoxelSettings.Instance.Offset;
			Vector3Int size = VoxelSettings.Instance.WorldSize;
			float scale = VoxelSettings.Instance.Scale;
			float persistence = VoxelSettings.Instance.Persistence;
			float lacunarity = VoxelSettings.Instance.Lacunarity;
			AnimationCurve density = VoxelSettings.Instance.Density;


			System.Random rng = new System.Random(seed);
			Vector3[] octaveOffsets = new Vector3[octaves];
			for (int i = 0; i < octaves; i++)
				octaveOffsets[i] = new Vector3(rng.Next(-100000, 100000) + offset.x, rng.Next(-100000, 100000) + offset.y, rng.Next(-100000, 100000) + offset.z);

			float minValue = float.MaxValue;
			float maxValue = float.MinValue;

			this.noiseMap = new float[size.x, size.y, size.z];

			CoordinateIterator iterator = new CoordinateIterator(size, Vector3Int.zero);

			foreach (Vector3Int pos in iterator)
			{
				float amplitude = 1;
				float frequency = 1;
				float noise = 0;

				for (int i = 0; i < octaves; i++)
				{
					Vector3 samplePos = (Vector3)pos * scale * frequency + octaveOffsets[i];

					float val = PerlinSample3D(samplePos.x, samplePos.y, samplePos.z) * 2 - 1;
					noise += val * amplitude;

					amplitude *= persistence;
					frequency *= lacunarity;
				}

				if (noise < minValue)
					minValue = noise;

				if (noise > maxValue)
					maxValue = noise;

				this.noiseMap[pos.x, pos.y, pos.z] = noise;
			}

			iterator.Reset();

			foreach (Vector3Int pos in iterator)
			{
				int x = pos.x;
				int y = pos.y;
				int z = pos.z;

				this.noiseMap[x, y, z] = Mathf.InverseLerp(minValue, maxValue, this.noiseMap[x, y, z]);
				if (this.noiseMap[x, y, z] > density.Evaluate((float)y / size.y)) continue;

				result.SetBlockAt(in pos, 1);
			}
		}

		public float PerlinSample3D(float x, float y, float z)
		{
			float AB = Mathf.PerlinNoise(x, y);
			float BC = Mathf.PerlinNoise(y, z);
			float AC = Mathf.PerlinNoise(x, z);
			float BA = Mathf.PerlinNoise(y, x);
			float CB = Mathf.PerlinNoise(z, y);
			float CA = Mathf.PerlinNoise(z, x);

			float ABC = AB + BC + AC + BA + CB + CA;
			return ABC / 6f;
		}

		public void Initialize()
		{
			throw new System.NotSupportedException();
		}

		public void Generate(int chunkX, int chunkZ, out IBlockGeneratorResult result)
		{
			throw new System.NotSupportedException();
		}

		public void Generate(CoordinateIterator iterator, out IBlockGeneratorResult result)
		{
			throw new System.NotSupportedException();
		}

		public void GenerateIntoExisting(int chunkX, int chunkZ, ref IBlockGeneratorResult result)
		{
			throw new System.NotSupportedException();
		}

		public void GenerateIntoExisting(CoordinateIterator iterator, ref IBlockGeneratorResult result)
		{
			throw new System.NotSupportedException();
		}
	}
}
