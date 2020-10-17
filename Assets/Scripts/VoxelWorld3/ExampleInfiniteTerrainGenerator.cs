using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using VoxelWorld2.Generators.Common;
using VoxelWorld2.Generators.Structures;
using VoxelWorld2.Utils;
using VoxelWorld3.Chunks;

namespace VoxelWorld3
{
	public class ExampleInfiniteTerrainGenerator
	{
		private int seed;
		private int octaves;
		private Vector3 noiseOffset;
		private float scale;
		private float persistence;
		private float lacunarity;
		private System.Random rng;
		private Vector3[] octaveOffsets;
		private int chunkSize;
		private int baselineHeight;
		private int waterLevel;

		public void Initialize()
		{
			this.seed = VoxelSettings.Instance.Seed;
			this.octaves = VoxelSettings.Instance.Octaves;
			this.noiseOffset = VoxelSettings.Instance.Offset;
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
					rng.Next(-100000, 100000) + noiseOffset.x,
					rng.Next(-100000, 100000) + noiseOffset.y,
					rng.Next(-100000, 100000) + noiseOffset.z
				);
			}
		}

		public void Generate(ref Chunk chunk)
		{
			CoordinateIterator iterator = new CoordinateIterator(
				new Vector3Int(chunk.GetSize().x, 1, chunk.GetSize().z),
				Vector3Int.zero
			);

			Vector3Int offset = new Vector3Int(chunk.GetWorldSpacePosition().x, 0, chunk.GetWorldSpacePosition().y);

			float[,] noiseMap = new float[iterator.size.x, iterator.size.z];

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

				noiseMap[pos.x, pos.z] = noise;
			}

			iterator.Reset();

			foreach (Vector3Int pos in iterator)
			{
				float variance = this.baselineHeight * noiseMap[pos.x, pos.z];

				int height = Mathf.FloorToInt((noiseMap[pos.x, pos.z] * 2 - 1) * variance + Mathf.Max(this.baselineHeight - variance, 0));

				noiseMap[pos.x, pos.z] = height;
			}

			iterator.Reset();

			foreach (Vector3Int pos in iterator)
			{
				int height = (int) noiseMap[pos.x, pos.z];

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

				if ((pos.x + offset.x + this.chunkSize / 2) % this.chunkSize != 0 || (pos.z + offset.z + this.chunkSize / 2) % this.chunkSize != 0 || height < this.waterLevel) continue;
				
				new TreeStructure().Generate(new Vector3Int(pos.x + offset.x, height, pos.z + offset.z), out IBlockGeneratorResult tree);
				
				CoordinateIterator treeIterator = new CoordinateIterator(tree.GetSize(), Vector3Int.zero);
				foreach (Vector3Int treeBlockPos in treeIterator)
				{
					byte? treeBlock = tree.GetBlockAt(treeBlockPos);
					if(treeBlock.HasValue)
						chunk.SetBlockAtWorldPosition(tree.GetOffset() + treeBlockPos, treeBlock.Value);
				}
			}
		}
	}
}