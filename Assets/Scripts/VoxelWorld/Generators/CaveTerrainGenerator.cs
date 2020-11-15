using UnityEngine;
using Utils;
using VoxelWorld.Chunks;

namespace VoxelWorld.Generators
{
	public class CaveTerrainGenerator : TerrainGenerator
	{
		private float minValue;
		private float maxValue;

		public override void Initialize()
		{
			base.Initialize();

			this.minValue = float.MaxValue;
			this.maxValue = float.MinValue;

			CoordinateIterator iterator = new CoordinateIterator(new Vector3Int(200, 100, 200), new Vector3Int(-100, 0, -100));

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
			}
		}

		public override void Generate(ref Chunk chunk, CoordinateIterator iterator)
		{
			Vector3Int offset = new Vector3Int(chunk.GetWorldSpacePosition().x, 0, chunk.GetWorldSpacePosition().y);

			iterator = new CoordinateIterator(new Vector3Int(iterator.size.x, chunk.GetSize().y, iterator.size.z), iterator.offset);

			foreach (Vector3Int pos in iterator)
			{
				float amplitude = 1;
				float frequency = 1;
				float noise = 0;

				for (int i = 0; i < octaves; i++)
				{
					Vector3 samplePos = (Vector3)(pos + offset) * cavernScale * frequency + octaveOffsets[i];

					float val = PerlinSample3D(samplePos.x, samplePos.y, samplePos.z) * 2 - 1;
					noise += val * amplitude;

					amplitude *= persistence;
					frequency *= lacunarity;
				}

				if (Mathf.InverseLerp(this.minValue, this.maxValue, noise) <
				    this.density.Evaluate((float) pos.y / chunk.GetSize().y))
					chunk.SetBlockAtLocalPosition(in pos, 1);
			}
		}

		private static float PerlinSample3D(float x, float y, float z)
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
	}
}