using UnityEngine;
using VoxelWorld.Chunks;
using VoxelWorld.Utils;

namespace VoxelWorld.Generators.Biomes
{
	public class OceanBiomeTerrainGenerator : TerrainGenerator
	{
		public override void Generate(ref Chunk chunk, CoordinateIterator iterator)
		{
			Vector3Int offset = new Vector3Int(chunk.GetWorldSpacePosition().x, 0, chunk.GetWorldSpacePosition().y);

			foreach (Vector3Int pos in iterator)
			{
				float amplitude = 1;
				float frequency = 1;
				float noise = 0;

				for (int i = 0; i < octaves; i++)
				{
					Vector3 samplePos = (Vector3)(pos + offset) * scale * frequency + octaveOffsets[i];

					float val = Mathf.PerlinNoise(samplePos.x, samplePos.z) * 2 - 1;
					noise += val * amplitude;

					amplitude *= persistence;
					frequency *= lacunarity;
				}

				float variance = (this.baselineHeight - this.waterLevel) * noise;

				int height = Mathf.FloorToInt((noise * 2 - 1) * variance + Mathf.Max(this.baselineHeight - this.waterLevel - variance, 0));

				for (int y = 0; y < height; y++)
				{
					byte blockId;

					if (y == 0) blockId = 2;
					else if (y > height - 5) blockId = 3;
					else blockId = 1;

					chunk.SetBlockAtWorldPosition(new Vector3Int(pos.x, y, pos.z), blockId);
				}

				for (int y = height; y < this.waterLevel; y++)
				{
					chunk.SetBlockAtWorldPosition(new Vector3Int(pos.x, y, pos.z), 7);
				}
			}
		}
	}
}