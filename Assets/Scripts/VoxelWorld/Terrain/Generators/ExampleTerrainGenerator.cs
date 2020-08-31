using UnityEngine;
using VoxelWorld.Terrain.Generators.Abstractions;
using VoxelWorld.Utils;

namespace VoxelWorld.Terrain.Generators
{
	public class ExampleTerrainGenerator : ITerrainGenerator
	{
		private float[,] noiseMap;

		public void GenerateAll()
		{
			int seed = VoxelSettings.Instance.Seed;
			int octaves = VoxelSettings.Instance.Octaves;
			Vector3 offset = VoxelSettings.Instance.Offset;
			Vector3Int size = VoxelSettings.Instance.WorldSize;
			float scale = VoxelSettings.Instance.Scale;
			float persistence = VoxelSettings.Instance.Persistence;
			float lacunarity = VoxelSettings.Instance.Lacunarity;

			System.Random rng = new System.Random(seed);

			Vector3[] octaveOffsets = new Vector3[octaves];
			for (int i = 0; i < octaves; i++)
			{
				octaveOffsets[i] = new Vector3(
					rng.Next(-100000, 100000) + offset.x,
					rng.Next(-100000, 100000) + offset.y,
					rng.Next(-100000, 100000) + offset.z
				);
			}
			
			float minValue = float.MaxValue;
			float maxValue = float.MinValue;

			this.noiseMap = new float[size.x, size.z];

			CoordinateIterator iterator = new CoordinateIterator(new Vector3Int(size.x, 1, size.z), Vector3Int.zero);

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

				if (noise < minValue)
					minValue = noise;

				if (noise > maxValue)
					maxValue = noise;

				this.noiseMap[pos.x, pos.z] = noise;
			}

			iterator.Reset();

			foreach (Vector3Int pos in iterator)
			{
				int x = pos.x;
				int z = pos.z;

				this.noiseMap[x, z] = Mathf.InverseLerp(minValue, maxValue, this.noiseMap[x, z]);

				float varianceMult = this.noiseMap[x, z];

				float variance = size.y * varianceMult;

				int height = Mathf.FloorToInt((this.noiseMap[x, z] * 2 - 1) * variance + Mathf.Max(size.y - variance, 0));

				for (int y = 0; y < height; y++)
				{
					byte blockId;

					if (y == 0) blockId = 2;
					else if (y == height - 1) blockId = 5;
					else if (y == height - 2) blockId = 4;
					else if (y > height - 5) blockId = 3;
					else blockId = 1;

					VoxelTerrain.ActiveTerrain.SetBlockAt(new Vector3Int(x, y, z), blockId);
				}

			}
		}
	}
}
