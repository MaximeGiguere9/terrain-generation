using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using VoxelWorld2.Utils;

namespace VoxelWorld3.World
{
	public class PlayableAreaController : MonoBehaviour
	{
		[SerializeField] private GameObject player;
		[SerializeField] private int radius;

		private WorldService world;

		private List<Vector2Int> lastArea = new List<Vector2Int>();

		private void Start()
		{
			this.world = WorldService.Instance;
		}

		private void Update()
		{
			Vector3Int playerChunk = GetChunkPosition(player.transform.position);

			CoordinateIterator iterator = new CoordinateIterator(
				new Vector3Int(radius * 2 + 1, 1, radius * 2 + 1),
				new Vector3Int(playerChunk.x - radius, 0, playerChunk.z - radius)
			);

			List<Vector2Int> newArea = iterator.Select(pos => new Vector2Int(pos.x, pos.z)).ToList();

			this.world.LoadChunks(newArea.Where(c => !this.lastArea.Contains(c)));
			this.world.UnloadChunks(this.lastArea.Where(c => !newArea.Contains(c)));

			this.lastArea = newArea;

			Vector3Int start = new Vector3Int(playerChunk.x - radius, 0, playerChunk.z - radius) * WorldService.CHUNK_SIZE;
			Vector3Int end = new Vector3Int(playerChunk.x + radius, 128, playerChunk.z + radius) * WorldService.CHUNK_SIZE;
			DrawBounds(start, end);
		}

		private static Vector3Int GetChunkPosition(Vector3 worldPosition)
		{
			return Vector3Int.FloorToInt(worldPosition / VoxelSettings.Instance.ChunkSize);
		}

		private static void DrawBounds(Vector3Int a, Vector3Int b)
		{
			Debug.DrawLine(a, new Vector3(b.x, a.y, a.z), Color.green);
			Debug.DrawLine(a, new Vector3(a.x, b.y, a.z), Color.green);
			Debug.DrawLine(a, new Vector3(a.x, a.y, b.z), Color.green);

			Debug.DrawLine(new Vector3(b.x, b.y, a.z), new Vector3(a.x, b.y, a.z), Color.green);
			Debug.DrawLine(new Vector3(b.x, b.y, a.z), new Vector3(b.x, a.y, a.z), Color.green);
			Debug.DrawLine(new Vector3(b.x, b.y, a.z), new Vector3(b.x, b.y, b.z), Color.green);

			Debug.DrawLine(new Vector3(a.x, b.y, b.z), new Vector3(b.x, b.y, b.z), Color.green);
			Debug.DrawLine(new Vector3(a.x, b.y, b.z), new Vector3(a.x, a.y, b.z), Color.green);
			Debug.DrawLine(new Vector3(a.x, b.y, b.z), new Vector3(a.x, b.y, a.z), Color.green);

			Debug.DrawLine(new Vector3(b.x, a.y, b.z), new Vector3(a.x, a.y, b.z), Color.green);
			Debug.DrawLine(new Vector3(b.x, a.y, b.z), new Vector3(b.x, b.y, b.z), Color.green);
			Debug.DrawLine(new Vector3(b.x, a.y, b.z), new Vector3(b.x, a.y, a.z), Color.green);
		}
	}
}