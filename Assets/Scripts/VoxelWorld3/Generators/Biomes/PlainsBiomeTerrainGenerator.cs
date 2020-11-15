using UnityEngine;
using Utils;
using VoxelWorld3.Chunks;

namespace VoxelWorld3.Generators.Biomes
{
	public class PlainsBiomeTerrainGenerator : TerrainGenerator
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

				float variance = this.baselineHeight * noise;

				int height = Mathf.FloorToInt((noise * 2 - 1) * variance + Mathf.Max(this.baselineHeight - variance, 0));
				
				for (int y = 0; y < height; y++)
				{
					byte blockId;

					if (height >= this.waterLevel)
					{
						if (y == 0) blockId = 2;
						else if (y == height - 1) blockId = 4;
						else if (y > height - 5) blockId = 3;
						else blockId = 1;
					}
					else
					{
						if (y == 0) blockId = 2;
						else if (y > height - 5) blockId = 3;
						else blockId = 1;
					}

					chunk.SetBlockAtLocalPosition(new Vector3Int(pos.x, y, pos.z), blockId);
				}

				for (int y = height; y < this.waterLevel; y++)
				{
					chunk.SetBlockAtLocalPosition(new Vector3Int(pos.x, y, pos.z), 7);
				}

				if ((pos.x + offset.x + chunk.GetSize().x / 2) % chunk.GetSize().x != 0 || 
				    (pos.z + offset.z + chunk.GetSize().z / 2) % chunk.GetSize().z != 0 || 
				    height < this.waterLevel)
					continue;

				new VoxelWorld2.Generators.Structures.TreeStructure().Generate(new Vector3Int(pos.x + offset.x, height, pos.z + offset.z), out VoxelWorld2.Generators.Common.IBlockGeneratorResult tree);

				CoordinateIterator treeIterator = new CoordinateIterator(tree.GetSize(), Vector3Int.zero);
				foreach (Vector3Int treeBlockPos in treeIterator)
				{
					byte? treeBlock = tree.GetBlockAt(treeBlockPos);
					if (treeBlock.HasValue)
						chunk.SetBlockAtWorldPosition(tree.GetOffset() + treeBlockPos, treeBlock.Value);
				}
			}
		}
	}
}