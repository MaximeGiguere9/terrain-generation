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

		private static readonly Dictionary<Vector3Int, VoxelChunk> ChunkMap = new Dictionary<Vector3Int, VoxelChunk>();

		public static Block GetBlockAt(Vector3Int position)
		{
			Vector3Int chunkPosition = GetContainingChunkPosition(position);

			VoxelChunk chunk = GetChunkAt(chunkPosition);
			return chunk == null ? null : chunk.GetBlockAt(position);
		}

		public static VoxelChunk GetChunkAt(Vector3Int position)
		{
			ChunkMap.TryGetValue(position, out VoxelChunk chunk);
			return chunk;
		}

		public static void RegisterExistingChunk(VoxelChunk chunk)
		{
			ChunkMap.Add(chunk.Position, chunk);
		}

		public static Vector3Int GetContainingChunkPosition(Vector3Int position) =>
			new Vector3Int(
				Mathf.FloorToInt((float)position.x / VoxelChunk.ChunkSize),
				Mathf.FloorToInt((float)position.y / VoxelChunk.ChunkSize),
				Mathf.FloorToInt((float)position.z / VoxelChunk.ChunkSize)
			);


		public static VoxelChunk CreateChunk(Vector3Int position)
		{
			VoxelChunk chunk = Instantiate(ChunkPrefab, Vector3.zero, Quaternion.identity, TerrainContainer).GetComponent<VoxelChunk>();
			chunk.Position = position;
			ChunkMap.Add(position, chunk);
			return chunk;
		}

		public static void AddBlock(Block block)
		{
			Vector3Int chunkPosition = GetContainingChunkPosition(block.Position);

			VoxelChunk chunk = GetChunkAt(chunkPosition);

			if (chunk == null) chunk = CreateChunk(chunkPosition);
			chunk.AddBlock(block);
		}

		public static void RedrawAll()
		{
			foreach (VoxelChunk chunk in ChunkMap.Values)
			{
				chunk.Redraw();
			}
		}
	}
}