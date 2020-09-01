﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Terrain.Generators.Abstractions;

namespace VoxelWorld.Terrain
{
	public class VoxelTerrain : MonoBehaviour
	{
		public static VoxelTerrain ActiveTerrain;
		private static void SetActiveTerrain(VoxelTerrain world) => ActiveTerrain = world; 

		private static GameObject ChunkPrefab => VoxelSettings.Instance.ChunkPrefab;
		private static int ChunkSize => VoxelSettings.Instance.ChunkSize;

		[SerializeField] private TerrainGeneratorType terrainGeneratorType;

		private bool redrawQueued = false;

		public static readonly Vector3Int[] Neighbors =
		{
			Vector3Int.zero,
			Vector3Int.right,
			Vector3Int.left,
			Vector3Int.up,
			Vector3Int.down,
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 0, -1)
		};

		private readonly Dictionary<Vector3Int, VoxelChunk> ChunkMap = new Dictionary<Vector3Int, VoxelChunk>();

		private readonly Queue<VoxelChunk> redrawQueue = new Queue<VoxelChunk>();

		private void Awake()
		{
			SetActiveTerrain(this);
		}

		private void Start()
		{
			ITerrainGenerator terrainGenerator = TerrainGeneratorFactory.GetTerrainGenerator(this.terrainGeneratorType);

			if (!terrainGenerator.SupportsInfiniteGeneration())
			{
				terrainGenerator.GenerateAll();
				StartCoroutine(QueueRedrawAll());
				return;
			}

			terrainGenerator.Initialize();

			// in theory supports generating any arbitrary chunk at runtime (infinite gen)
			for (int x = -10; x < 10; x++)
			{
				for (int z = -10; z < 10; z++)
				{
					terrainGenerator.GenerateVerticalChunks(x, z);
					StartCoroutine(QueueRedrawAll());
				}
			}
			
		}

		private IEnumerator QueueRedrawAll()
		{
			if (this.redrawQueued) yield break;
			this.redrawQueued = true;
			yield return new WaitForEndOfFrame();
			RedrawAll();
			this.redrawQueued = false;
		}

		public void Redraw(VoxelChunk chunk, bool immediate = false)
		{
			if (immediate)
			{
				chunk.Redraw();
				return;
			}

			if (chunk == null || this.redrawQueue.Contains(chunk)) return;
			this.redrawQueue.Enqueue(chunk);

			if (this.redrawQueue.Count == 1)
				StartCoroutine(RedrawChunksCoroutine());
		}

		public void RedrawAll(bool immediate = false)
		{
			foreach (VoxelChunk chunk in ChunkMap.Values)
				Redraw(chunk, immediate);
		}

		private IEnumerator RedrawChunksCoroutine()
		{
			while (this.redrawQueue.Count > 0)
			{
				yield return null;
				VoxelChunk chunk = this.redrawQueue.Dequeue();
				chunk.Redraw();
			}
		}

		public void SetBlockAt(Vector3Int position, byte blockId, bool redraw = false)
		{
			Vector3Int chunkPosition = GetContainingChunkPosition(position);

			VoxelChunk chunk = GetChunkAt(chunkPosition);

			if (chunk == null) chunk = CreateChunk(chunkPosition);
			chunk.SetBlockAt(position, blockId);

			if (!redraw) return;

			RedrawAround(position);
		}

		public byte GetBlockAt(Vector3Int position)
		{
			Vector3Int chunkPosition = GetContainingChunkPosition(position);

			VoxelChunk chunk = GetChunkAt(chunkPosition);
			return chunk == null ? (byte) 0 : chunk.GetBlockAt(position);
		}

		public byte RemoveBlockAt(Vector3Int position)
		{
			Vector3Int chunkPosition = GetContainingChunkPosition(position);

			VoxelChunk chunk = GetChunkAt(chunkPosition);
			if (chunk == null) return 0;

			byte removedBlock = chunk.RemoveBlockAt(position);
			if (removedBlock == 0) return 0;

			RedrawAround(position);

			return removedBlock;
		}

		private void RedrawAround(Vector3Int position)
		{
			foreach (Vector3Int offset in Neighbors)
			{
				Vector3Int chunkPosition = GetContainingChunkPosition(position + offset);
				Redraw(GetChunkAt(chunkPosition));
			}
		}

		public VoxelChunk CreateChunk(Vector3Int position)
		{
			VoxelChunk chunk = Instantiate(ChunkPrefab, Vector3.zero, Quaternion.identity, this.transform).GetComponent<VoxelChunk>();
			chunk.Position = position;
			RegisterChunk(chunk);
			return chunk;
		}

		public void RegisterChunk(VoxelChunk chunk)
		{
			ChunkMap.Add(chunk.Position, chunk);
		}

		public VoxelChunk GetChunkAt(Vector3Int position)
		{
			ChunkMap.TryGetValue(position, out VoxelChunk chunk);
			return chunk;
		}

		public Vector3Int GetContainingChunkPosition(Vector3Int position)
		{
			return new Vector3Int(
				Mathf.FloorToInt((float) position.x / ChunkSize),
				Mathf.FloorToInt((float) position.y / ChunkSize),
				Mathf.FloorToInt((float) position.z / ChunkSize)
			);
		}

		

		
	}
}