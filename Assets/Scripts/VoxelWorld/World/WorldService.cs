using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Utils;
using VoxelWorld.Chunks;
using VoxelWorld.Generators;

namespace VoxelWorld.World
{
	public class WorldService
	{
		public static readonly Vector3Int CHUNK_SIZE = new Vector3Int(16, 256, 16);
		public static readonly byte CHUNK_SUBDIVISIONS = 16;

		private static WorldService _instance;
		public static WorldService Instance => _instance ?? (_instance = new WorldService());

		public event Action<IEnumerable<Vector2Int>> OnChunksLoaded;
		public event Action<IEnumerable<Vector2Int>> OnChunksUnloaded;

		/// <summary>
		/// All existing chunks, including unloaded chunks
		/// </summary>
		private readonly Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

		/// <summary>
		/// Rendered chunks (i.e. play area)
		/// </summary>
		private readonly HashSet<Vector2Int> loadedChunks = new HashSet<Vector2Int>();

		private readonly ITerrainGenerator terrainGenerator;

		private readonly List<Vector2Int> chunkLoadQueue = new List<Vector2Int>();

		private bool isLoadingChunks;

		public WorldService()
		{
			this.terrainGenerator = TerrainGeneratorFactory.GetTerrainGenerator(TerrainGeneratorSettings.Instance.TerrainGeneratorType);
		}

		public void LoadChunks(IEnumerable<Vector2Int> chunkPositions)
		{
			foreach (Vector2Int pos in chunkPositions)
			{
				if (this.loadedChunks.Contains(pos)) continue;
				this.chunkLoadQueue.Add(pos);
				MonoBehaviourHelper.Start(LoadChunksRoutine());
			}
		}

		public void LoadChunk(Vector2Int chunkPos)
		{
			this.chunkLoadQueue.Add(chunkPos);
			MonoBehaviourHelper.Start(LoadChunksRoutine());
		}

		private IEnumerator LoadChunksRoutine()
		{
			if (this.isLoadingChunks) yield break;
			this.isLoadingChunks = true;

			while (this.chunkLoadQueue.Count > 0)
			{
				Vector2Int chunkPos = this.chunkLoadQueue[0];
				if (this.loadedChunks.Contains(chunkPos)) continue;

				this.chunkLoadQueue.RemoveAt(0);
				LoadOrCreateChunk(chunkPos);

				OnChunksLoaded?.Invoke(new[] {chunkPos});

				yield return null;
			}

			this.isLoadingChunks = false;
		}

		public void UnloadChunks(IEnumerable<Vector2Int> chunkPositions)
		{
			HashSet<Vector2Int> res = new HashSet<Vector2Int>();
			foreach (Vector2Int pos in chunkPositions)
			{
				this.chunkLoadQueue.Remove(pos);

				if (this.loadedChunks.Remove(pos))
					res.Add(pos);
			}
			if (res.Count > 0) OnChunksUnloaded?.Invoke(res);
		}

		public void UnloadChunk(Vector2Int chunkPos)
		{
			this.chunkLoadQueue.Remove(chunkPos);

			if (this.loadedChunks.Remove(chunkPos))
				OnChunksUnloaded?.Invoke(new[] {chunkPos});
		}

		private void LoadOrCreateChunk(Vector2Int chunkPos)
		{
			if (this.chunks.ContainsKey(chunkPos))
			{
				this.loadedChunks.Add(chunkPos);
				return;
			}

			Chunk newChunk = new Chunk(CHUNK_SIZE, CHUNK_SUBDIVISIONS);
			newChunk.SetChunkSpacePosition(chunkPos);
			this.chunks.Add(chunkPos, newChunk);

			this.terrainGenerator.Generate(ref newChunk);

			foreach (Neighbor neighborPos in Neighbor.All)
			{
				this.chunks.TryGetValue(chunkPos + neighborPos.Value, out Chunk neighborChunk);
				if (neighborChunk == null) continue;
				newChunk.SetNeighbor(neighborPos, neighborChunk);
				neighborChunk.SetNeighbor(Neighbor.Opposite(neighborPos), newChunk);

				foreach (var subChunk in neighborChunk.GetSubChunks())
				{
					subChunk.InvalidateMesh();
				}
			}

			foreach (var subChunk in newChunk.GetSubChunks())
			{
				subChunk.InvalidateMesh();
			}

			/*CoordinateIterator itr = new CoordinateIterator(new Vector3Int(16, 48, 16), Vector3Int.zero);
			foreach (Vector3Int c in itr) newChunk.SetBlockAtLocalPosition(in c, 1);*/

			this.loadedChunks.Add(chunkPos);

			//if chunk exists render it, else have the terrain generator create it first

			//terrain generator generates by column
			//chunks are stored in 2d space, have sub chunks for rendering?

			//terrain generator must output in the same format as chunks for easier copying
			//terrain generator has noise object

			//structures are generated separately in a second layer
			//this is why noise should be separate (caching values)

			//
		}

		public bool ChunkExists(in Vector2Int position)
		{
			return this.chunks.ContainsKey(position);
		}

		[CanBeNull]
		public Chunk GetChunkFromChunkPosition(in Vector2Int position)
		{
			this.chunks.TryGetValue(position, out Chunk chunk);
			return chunk;
		}

		[CanBeNull]
		public Chunk GetChunkFromWorldPosition(in Vector3Int worldPos)
		{
			Vector2Int chunkPos = new Vector2Int(
				Mathf.FloorToInt((float) worldPos.x / CHUNK_SIZE.x),
				Mathf.FloorToInt((float) worldPos.z / CHUNK_SIZE.z)
			);
			this.chunks.TryGetValue(chunkPos, out Chunk chunk);
			return chunk;
		}

		public void UpdateAroundWorldPositions(Vector3Int pos) => UpdateAroundWorldPositions(new[] { pos });

		public void UpdateAroundWorldPositions(IEnumerable<Vector3Int> positions)
		{
			HashSet<SubChunk> subChunksToInvalidate = new HashSet<SubChunk>();

			foreach (Vector3Int position in positions)
			{
				Chunk chunk = this.GetChunkFromWorldPosition(position);
				if (chunk == null) continue;

				Vector3Int chunkSize = chunk.GetSize();
				int subdivisionCount = chunk.GetSubdivisionCount();
				int subdivisionIndex = position.y / subdivisionCount;
				int subdivisionSize = chunkSize.y / subdivisionCount;

				subChunksToInvalidate.Add(chunk.GetSubChunk(subdivisionIndex));

				if (position.x == 0)
				{
					Chunk neighbor = chunk.GetNeighbor(Neighbor.West);
					if (neighbor != null) subChunksToInvalidate.Add(neighbor.GetSubChunk(subdivisionIndex));
				}
				else if (position.x == chunkSize.x)
				{
					Chunk neighbor = chunk.GetNeighbor(Neighbor.East);
					if (neighbor != null) subChunksToInvalidate.Add(neighbor.GetSubChunk(subdivisionIndex));
				}

				if (position.z == 0)
				{
					Chunk neighbor = chunk.GetNeighbor(Neighbor.North);
					if (neighbor != null) subChunksToInvalidate.Add(neighbor.GetSubChunk(subdivisionIndex));
				}
				else if (position.z == chunkSize.z)
				{
					Chunk neighbor = chunk.GetNeighbor(Neighbor.South);
					if (neighbor != null) subChunksToInvalidate.Add(neighbor.GetSubChunk(subdivisionIndex));
				}

				if (MathUtils.Mod(position.y, subdivisionSize) == 0 && subdivisionIndex > 0)
				{
					subChunksToInvalidate.Add(chunk.GetSubChunk(subdivisionIndex - 1));
				}
				else if (MathUtils.Mod(position.y, subdivisionSize) == subdivisionSize - 1 && subdivisionIndex < subdivisionCount)
				{
					subChunksToInvalidate.Add(chunk.GetSubChunk(subdivisionIndex + 1));
				}
			}

			foreach (SubChunk subChunk in subChunksToInvalidate) subChunk.InvalidateMesh();
		}

		public byte? GetBlockAt(in Vector3Int position)
		{
			Chunk chunk = GetChunkFromWorldPosition(in position);
			return chunk?.GetBlockAtWorldPosition(in position);
		}

		public bool SetBlockAt(in Vector3Int position, byte block)
		{
			Chunk chunk = GetChunkFromWorldPosition(in position);
			if (chunk == null) return false;
			chunk.SetBlockAtWorldPosition(in position, block);
			UpdateAroundWorldPositions(position);
			return true;
		}
	}
}