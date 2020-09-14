using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld2.Chunks
{
	public class ChunkService
	{
		public const byte CHUNK_SIZE = 16;

		private readonly Dictionary<Vector3Int, Tuple<ChunkController, ChunkRenderer>> chunks;

		private readonly Queue<Vector3Int> viewUpdateQueue;

		private readonly HashSet<Vector3Int> loadedChunks;

		public ChunkService()
		{
			this.chunks = new Dictionary<Vector3Int, Tuple<ChunkController, ChunkRenderer>>();
			this.viewUpdateQueue = new Queue<Vector3Int>();
			this.loadedChunks = new HashSet<Vector3Int>();
		}

		public void QueueRedraw(Vector3Int position)
		{
			if (!this.viewUpdateQueue.Contains(position))
				this.viewUpdateQueue.Enqueue(position);
		}

		public void RedrawNext()
		{
			Vector3Int position;
			do
			{
				if (this.viewUpdateQueue.Count == 0) return;
				position = this.viewUpdateQueue.Dequeue();
			} while (!this.loadedChunks.Contains(position));

			this.chunks.TryGetValue(position, out var chunk);
			if (chunk == null) throw new InvalidOperationException("Chunk does not exist");

			chunk.Item2.UpdateView();
		}

		public int GetRedrawQueueSize() => this.viewUpdateQueue.Count;

		public void CreateChunk(Vector3Int position)
		{
			if (this.chunks.ContainsKey(position)) return;

			ChunkController controller = new ChunkController();
			controller.SetSize(CHUNK_SIZE);
			controller.SetPosition(position);

			ChunkRenderer renderer = new ChunkRenderer(controller);

			this.chunks.Add(position, new Tuple<ChunkController, ChunkRenderer>(controller, renderer));
		}

		public void LoadChunk(Vector3Int position)
		{
			if (this.loadedChunks.Contains(position))
				return;

			this.loadedChunks.Add(position);
			this.chunks.TryGetValue(position, out var chunk);
			if (chunk == null) throw new InvalidOperationException("Chunk does not exist");
			chunk.Item2.CreateView();
		}

		public void UnloadChunk(Vector3Int position)
		{
			if (!this.loadedChunks.Contains(position))
				return;

			this.loadedChunks.Remove(position);
			this.chunks.TryGetValue(position, out var chunk);
			chunk?.Item2.DestroyView();
		}

		public byte GetBlockAt(Vector3Int position)
		{
			Vector3Int chunkPos = GetContainingChunkPosition(position);
			Vector3Int blockOffset = position - chunkPos * CHUNK_SIZE;
			this.chunks.TryGetValue(chunkPos, out var chunk);
			if (chunk == null) throw new InvalidOperationException("Chunk does not exist");
			return chunk.Item1.GetBlockAt(blockOffset);
		}

		public void SetBlockAt(Vector3Int position, byte blockId)
		{
			Vector3Int chunkPos = GetContainingChunkPosition(position);
			Vector3Int blockOffset = position - chunkPos * CHUNK_SIZE;
			this.chunks.TryGetValue(chunkPos, out var chunk);
			if(chunk == null) throw new InvalidOperationException("Chunk does not exist");
			chunk.Item1.SetBlockAt(blockOffset, blockId);
		}

		private static Vector3Int GetContainingChunkPosition(Vector3Int position)
		{
			return new Vector3Int(
				Mathf.FloorToInt((float)position.x / CHUNK_SIZE),
				Mathf.FloorToInt((float)position.y / CHUNK_SIZE),
				Mathf.FloorToInt((float)position.z / CHUNK_SIZE)
			);
		}
	}
}