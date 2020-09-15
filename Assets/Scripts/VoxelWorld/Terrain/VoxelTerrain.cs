using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelWorld.Terrain.Generators.Abstractions;

namespace VoxelWorld.Terrain
{
	public class VoxelTerrain : MonoBehaviour
	{
		public static VoxelTerrain ActiveTerrain;
		private static void SetActiveTerrain(VoxelTerrain world) => ActiveTerrain = world; 

		private static GameObject ChunkPrefab;
		private static int ChunkSize;

		[SerializeField] private TerrainGeneratorType terrainGeneratorType;

		private bool redrawQueued = false;

		private ITerrainGenerator terrainGenerator;

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
			ChunkSize = VoxelSettings.Instance.ChunkSize;
			ChunkPrefab = VoxelSettings.Instance.ChunkPrefab;
		}

		private void Start()
		{
			this.terrainGenerator = TerrainGeneratorFactory.GetTerrainGenerator(this.terrainGeneratorType);

			if (!this.terrainGenerator.SupportsInfiniteGeneration())
			{
				this.terrainGenerator.GenerateAll();
				foreach (VoxelChunk chunk in this.ChunkMap.Values) chunk.Loaded = true;
				StartCoroutine(QueueRedrawAll());
				return;
			}

			this.terrainGenerator.Initialize();

			// in theory supports generating any arbitrary chunk at runtime (infinite gen)
			/*for (int x = -10; x < 10; x++)
			{
				for (int z = -10; z < 10; z++)
				{
					terrainGenerator.Generate(x, z);
					StartCoroutine(QueueRedrawAll());
				}
			}*/
			
		}

		private void Update()
		{
			foreach (VoxelChunk chunk in this.redrawQueue)
			{
				int s = VoxelSettings.Instance.ChunkSize;
				Vector3Int p = chunk.Position;

				Debug.DrawLine(p * s, (p + new Vector3Int(1, 0, 0)) * s, Color.magenta);
				Debug.DrawLine(p * s, (p + new Vector3Int(0, 1, 0)) * s, Color.magenta);
				Debug.DrawLine(p * s, (p + new Vector3Int(0, 0, 1)) * s, Color.magenta);

				Debug.DrawLine((p + new Vector3Int(1, 1, 0)) * s, (p + new Vector3Int(0, 1, 0)) * s, Color.magenta);
				Debug.DrawLine((p + new Vector3Int(1, 1, 0)) * s, (p + new Vector3Int(1, 0, 0)) * s, Color.magenta);
				Debug.DrawLine((p + new Vector3Int(1, 1, 0)) * s, (p + new Vector3Int(1, 1, 1)) * s, Color.magenta);

				Debug.DrawLine((p + new Vector3Int(0, 1, 1)) * s, (p + new Vector3Int(1, 1, 1)) * s, Color.magenta);
				Debug.DrawLine((p + new Vector3Int(0, 1, 1)) * s, (p + new Vector3Int(0, 0, 1)) * s, Color.magenta);
				Debug.DrawLine((p + new Vector3Int(0, 1, 1)) * s, (p + new Vector3Int(0, 1, 0)) * s, Color.magenta);

				Debug.DrawLine((p + new Vector3Int(1, 0, 1)) * s, (p + new Vector3Int(0, 0, 1)) * s, Color.magenta);
				Debug.DrawLine((p + new Vector3Int(1, 0, 1)) * s, (p + new Vector3Int(1, 1, 1)) * s, Color.magenta);
				Debug.DrawLine((p + new Vector3Int(1, 0, 1)) * s, (p + new Vector3Int(1, 0, 0)) * s, Color.magenta);
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
				VoxelChunk chunk = null;
				while (chunk == null && this.redrawQueue.Count > 0)
				{
					chunk = this.redrawQueue.Dequeue();
				}
				if (chunk != null)
				{
					chunk.Redraw();
				}
			}
		}

		public void SetBlockAt(Vector3Int position, byte blockId, bool redraw = false)
		{
			Vector3Int chunkPosition = GetContainingChunkPosition(position);

			ChunkMap.TryGetValue(chunkPosition, out VoxelChunk chunk);

			if (chunk == null) chunk = CreateChunk(chunkPosition);
			chunk.SetBlockAt(position, blockId);

			if (!redraw) return;

			RedrawAround(position);
		}

		public byte GetBlockAt(Vector3Int position)
		{
			Vector3Int chunkPosition = GetContainingChunkPosition(position);

			ChunkMap.TryGetValue(chunkPosition, out VoxelChunk chunk);
			return chunk == null ? (byte) 0 : chunk.GetBlockAt(position);
		}

		public byte RemoveBlockAt(Vector3Int position)
		{
			Vector3Int chunkPosition = GetContainingChunkPosition(position);

			ChunkMap.TryGetValue(chunkPosition, out VoxelChunk chunk);
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
				ChunkMap.TryGetValue(chunkPosition, out VoxelChunk chunk);
				Redraw(chunk);
			}
		}

		public VoxelChunk CreateChunk(Vector3Int position)
		{
			if (ChunkMap.ContainsKey(position))
			{
				return ChunkMap[position];
			}

			VoxelChunk chunk = Instantiate(ChunkPrefab, Vector3.zero, Quaternion.identity, this.transform).GetComponent<VoxelChunk>();
			chunk.Position = position;
			RegisterChunk(chunk);
			return chunk;
		}

		public void RegisterChunk(VoxelChunk chunk)
		{
			ChunkMap.Add(chunk.Position, chunk);
		}

		public Vector3Int GetContainingChunkPosition(Vector3Int position)
		{
			return new Vector3Int(
				Mathf.FloorToInt((float) position.x / ChunkSize),
				Mathf.FloorToInt((float) position.y / ChunkSize),
				Mathf.FloorToInt((float) position.z / ChunkSize)
			);
		}

		public void LoadChunks(IEnumerable<Vector2Int> chunkPositions)
		{
			if (!this.terrainGenerator.SupportsInfiniteGeneration()) return;

			foreach (Vector2Int pos in chunkPositions)
			{
				this.terrainGenerator.Generate(pos.x, pos.y);
				IEnumerable<KeyValuePair<Vector3Int, VoxelChunk>> chunksToRedraw = this.ChunkMap.Where(kvp =>
					kvp.Key.x >= pos.x - 1 &&
					kvp.Key.x <= pos.x + 1 &&
					kvp.Key.z >= pos.y - 1 &&
					kvp.Key.z <= pos.y + 1
				);
				foreach (KeyValuePair<Vector3Int, VoxelChunk> kvp in chunksToRedraw)
				{
					if (kvp.Key.x == pos.x && kvp.Key.z == pos.y)
					{
						kvp.Value.Loaded = true;
					}
					Redraw(kvp.Value);
				}
			}
		}

		public void UnloadChunks(IEnumerable<Vector2Int> chunkPositions)
		{
			if (!this.terrainGenerator.SupportsInfiniteGeneration()) return;

			List<Vector3Int> keysToRemove = new List<Vector3Int>();

			foreach (Vector2Int pos in chunkPositions)
			{
				IEnumerable<KeyValuePair<Vector3Int, VoxelChunk>> chunksToDelete = this.ChunkMap.Where(kvp =>
					kvp.Key.x >= pos.x - 1 &&
					kvp.Key.x <= pos.x + 1 &&
					kvp.Key.z >= pos.y - 1 &&
					kvp.Key.z <= pos.y + 1
				);
				foreach (KeyValuePair<Vector3Int, VoxelChunk> kvp in chunksToDelete)
				{
					if (kvp.Key.x == pos.x && kvp.Key.z == pos.y)
					{
						kvp.Value.Loaded = false;
						keysToRemove.Add(kvp.Key);
						Redraw(kvp.Value, true);
					}
					else
					{
						Redraw(kvp.Value);
					}
				}
			}

			foreach (Vector3Int key in keysToRemove)
			{
				this.ChunkMap.Remove(key);
			}
		}
	}
}