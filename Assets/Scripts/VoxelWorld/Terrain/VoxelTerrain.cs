using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.Terrain
{
	public class VoxelTerrain : MonoBehaviour
	{
		public static VoxelTerrain ActiveTerrain;
		private static void SetActiveTerrain(VoxelTerrain world) => ActiveTerrain = world; 

		private static GameObject ChunkPrefab => VoxelWorldSettings.Instance.ChunkPrefab;
		private static int ChunkSize => VoxelWorldSettings.Instance.ChunkSize;

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

		private void Awake()
		{
			SetActiveTerrain(this);
		}

		public void SetBlockAt(Vector3Int position, byte blockId)
		{
			Vector3Int chunkPosition = GetContainingChunkPosition(position);

			VoxelChunk chunk = GetChunkAt(chunkPosition);

			if (chunk == null) chunk = CreateChunk(chunkPosition);
			chunk.SetBlockAt(position, blockId);
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

			foreach (Vector3Int offset in Neighbors)
				GetChunkAt(chunkPosition + offset)?.Redraw();

			return removedBlock;
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

		public void RedrawAll()
		{
			foreach (VoxelChunk chunk in ChunkMap.Values)
			{
				chunk.Redraw();
			}
		}
	}
}