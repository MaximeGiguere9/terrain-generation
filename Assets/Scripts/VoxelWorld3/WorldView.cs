﻿using System;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld2.Utils;
using VoxelWorld3.Chunks;
using Object = UnityEngine.Object;

namespace VoxelWorld3
{
	public class WorldView : MonoBehaviour
	{
		public static readonly Vector3Int CHUNK_SIZE = new Vector3Int(16, 256, 16);
		public static readonly byte CHUNK_SUBDIVISIONS = 16;

		[SerializeField] private GameObject chunkViewPrefab;

		/// <summary>
		/// All existing chunks, including unloaded chunks
		/// </summary>
		private readonly Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

		/// <summary>
		/// Rendered chunks (i.e. play area)
		/// </summary>
		private readonly Dictionary<Vector2Int, ChunkView> chunkViews = new Dictionary<Vector2Int, ChunkView>();

		private void Start()
		{
			LoadChunk(new Vector2Int(0, 0));
			LoadChunk(new Vector2Int(1, 0));
			LoadChunk(new Vector2Int(-1, 0));
			LoadChunk(new Vector2Int(0, 1));
			LoadChunk(new Vector2Int(0, -1));
			RenderAll();
		}

		//has a terrain generator

		public void LoadChunk(Vector2Int chunkPos)
		{
			if (this.chunkViews.ContainsKey(chunkPos)) return;

			if (!this.chunks.ContainsKey(chunkPos))
			{
				CreateChunk(chunkPos);
			}

			Chunk chunk = this.chunks[chunkPos];
			ChunkView chunkView = Object.Instantiate(this.chunkViewPrefab, this.transform).GetComponent<ChunkView>();
			chunkView.SetChunk(chunk);
			this.chunkViews.Add(chunkPos, chunkView);

			//if chunk exists render it, else have the terrain generator create it first

			//terrain generator generates by column
			//chunks are stored in 2d space, have sub chunks for rendering?

			//terrain generator must output in the same format as chunks for easier copying
			//terrain generator has noise object

			//structures are generated separately in a second layer
			//this is why noise should be separate (caching values)

			//
		}

		public void UnloadChunk(Vector2Int chunkPos)
		{
			if (!this.chunkViews.ContainsKey(chunkPos)) return;
			this.chunkViews[chunkPos].Destroy();
			this.chunkViews.Remove(chunkPos);
		}

		private void CreateChunk(Vector2Int chunkPos)
		{
			Chunk newChunk = new Chunk(CHUNK_SIZE, CHUNK_SUBDIVISIONS);
			newChunk.SetChunkSpacePosition(chunkPos);
			this.chunks.Add(chunkPos, newChunk);

			foreach (Neighbor neighborPos in Neighbor.All)
			{
				this.chunks.TryGetValue(chunkPos + neighborPos.Value, out Chunk neighborChunk);
				if (neighborChunk == null) continue;
				newChunk.SetNeighbor(neighborPos, neighborChunk);
				neighborChunk.SetNeighbor(Neighbor.Opposite(neighborPos), newChunk);
			}

			CoordinateIterator itr = new CoordinateIterator(new Vector3Int(16, 48, 16), Vector3Int.zero);
			foreach (Vector3Int c in itr) newChunk.SetBlockAtLocalPosition(in c, 1);
		}

		public void RenderAll()
		{
			foreach (var view in this.chunkViews)
			{
				view.Value.Render();
			}
		}
	}
}