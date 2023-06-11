using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Chunks;
using VoxelWorld.Generators.Structures;
using VoxelWorld.Utils;

namespace VoxelWorld.Generators.Biomes
{
	public class PlateauBiomeTerrainGenerator : TerrainGenerator
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

				noise = Mathf.Round(noise * 20) / 20; // gives jagged look to terrain

				float variance = this.baselineHeight * noise;

				int height = Mathf.FloorToInt((noise * 2 - 1) * variance + Mathf.Max(this.baselineHeight - variance, 0));
				height = Mathf.FloorToInt(Mathf.Pow(height, 0.9f));

				for (int y = 0; y < height; y++)
				{
					byte blockId;

					if (height >= this.waterLevel)
					{
						if (y == 0) blockId = 2;
						else if (y > height - 5) blockId = 9;
						else blockId = 1;
					}
					else
					{
						if (y == 0) blockId = 2;
						else if (y > height - 5) blockId = 13;
						else blockId = 1;
					}

					chunk.SetBlockAtWorldPosition(new Vector3Int(pos.x, y, pos.z), blockId);
				}

				for (int y = height; y < this.waterLevel; y++)
				{
					chunk.SetBlockAtWorldPosition(new Vector3Int(pos.x, y, pos.z), 7);
				}

				if (height < this.waterLevel)
					continue;

				if (this.rng.Next(1, 1000) < 995)
				{
					continue;
				}

				CactusStructure cactus = new CactusStructure(new Vector3Int(pos.x + offset.x, height, pos.z + offset.z), Random.Range(1, 5));

				foreach (KeyValuePair<Vector3Int, byte> treeBlock in cactus.GetBlocks())
					chunk.SetBlockAtWorldPosition(cactus.GetOffset() + treeBlock.Key, treeBlock.Value);
			}
		}
	}
}