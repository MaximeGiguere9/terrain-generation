using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld
{
	public class ChunkManager : MonoBehaviour
	{
		[SerializeField] private GameObject chunkPrefab;

		private void Awake()
		{
			ChunkPrefab = chunkPrefab;
			TerrainContainer = transform;
		}

		private static GameObject ChunkPrefab;

		private static Transform TerrainContainer;

		private static readonly Dictionary<Vector3Int, Chunk> ChunkMap = new Dictionary<Vector3Int, Chunk>();

		public static Block GetBlockAt(Vector3Int position)
		{
			Vector3Int chunkPosition = GetContainingChunkPosition(position);

			Chunk chunk = GetChunkAt(chunkPosition);
			return chunk == null ? null : chunk.GetBlockAt(position);
		}

		public static Chunk GetChunkAt(Vector3Int position)
		{
			ChunkMap.TryGetValue(position, out Chunk chunk);
			return chunk;
		}

		public static void RegisterExistingChunk(Chunk chunk)
		{
			ChunkMap.Add(chunk.Position, chunk);
		}

		public static Vector3Int GetContainingChunkPosition(Vector3Int position) =>
			new Vector3Int(
				Mathf.FloorToInt((float)position.x / Chunk.ChunkSize),
				Mathf.FloorToInt((float)position.y / Chunk.ChunkSize),
				Mathf.FloorToInt((float)position.z / Chunk.ChunkSize)
			);


		public static Chunk CreateChunk(Vector3Int position)
		{
			Chunk chunk = Instantiate(ChunkPrefab, Vector3.zero, Quaternion.identity, TerrainContainer).GetComponent<Chunk>();
			chunk.Position = position;
			ChunkMap.Add(position, chunk);
			return chunk;
		}

		public static void AddBlock(Block block)
		{
			Vector3Int chunkPosition = GetContainingChunkPosition(block.Position);

			Chunk chunk = GetChunkAt(chunkPosition);

			if (chunk == null) chunk = CreateChunk(chunkPosition);
			chunk.AddBlock(block);
		}

		public static void RedrawAll()
		{
			foreach (Chunk chunk in ChunkMap.Values)
			{
				chunk.Redraw();
			}
		}
	}
}