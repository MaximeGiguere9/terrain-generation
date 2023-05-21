using System.Collections.Generic;
using UnityEngine;
using Utils;
using VoxelWorld.Chunks;
using VoxelWorld.Generators.Structures;

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

				noise = Mathf.Round(noise * 20) / 20;

				float variance = this.baselineHeight * noise;

				int height = Mathf.FloorToInt((noise * 2 - 1) * variance + Mathf.Max(this.baselineHeight - variance, 0));

				for (int y = 0; y < height; y++)
				{
					byte blockId;

					if (y == 0) blockId = 2;
					else if (y > height - 5) blockId = 9;
					else blockId = 1;

					chunk.SetBlockAtWorldPosition(new Vector3Int(pos.x, y, pos.z), blockId);
				}

				for (int y = height; y < this.waterLevel; y++)
				{
					chunk.SetBlockAtWorldPosition(new Vector3Int(pos.x, y, pos.z), 7);
				}

				if ((pos.x + offset.x + chunk.GetSize().x / 2) % chunk.GetSize().x != 0 ||
					(pos.z + offset.z + chunk.GetSize().z / 2) % chunk.GetSize().z != 0 ||
					height < this.waterLevel)
					continue;

				CactusStructure cactus = new CactusStructure(new Vector3Int(pos.x + offset.x, height, pos.z + offset.z), Random.Range(1, 5));

				foreach (KeyValuePair<Vector3Int, byte> treeBlock in cactus.GetBlocks())
					chunk.SetBlockAtWorldPosition(cactus.GetOffset() + treeBlock.Key, treeBlock.Value);
			}
		}
	}
}