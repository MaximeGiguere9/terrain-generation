using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using VoxelWorld.Terrain;

namespace VoxelWorld2.Utils
{
	public class PlayableAreaController : MonoBehaviour
	{
		[SerializeField] private GameObject player;
		[SerializeField] private int radius;

		private List<Vector2Int> lastArea = new List<Vector2Int>();

		private void Update()
		{
			Vector3Int playerChunk = GetChunkPosition(player.transform.position);

			CoordinateIterator iterator = new CoordinateIterator(
				new Vector3Int(radius * 2, 1, radius * 2),
				new Vector3Int(playerChunk.x - radius, 0, playerChunk.z - radius)
			);

			List<Vector2Int> newArea = iterator.Select(pos => new Vector2Int(pos.x, pos.z)).ToList();

			VoxelTerrain.ActiveTerrain.LoadChunks(newArea.Where(c => !this.lastArea.Contains(c)));
			VoxelTerrain.ActiveTerrain.UnloadChunks(this.lastArea.Where(c => !newArea.Contains(c)));

			this.lastArea = newArea;

			Vector3Int start = new Vector3Int(playerChunk.x - radius, 0, playerChunk.z - radius) * VoxelSettings.Instance.ChunkSize;
			Vector3Int end = new Vector3Int(playerChunk.x + radius, 128, playerChunk.z + radius) * VoxelSettings.Instance.ChunkSize;



			Debug.DrawLine(start, new Vector3(end.x, start.y, start.z), Color.green);
			Debug.DrawLine(start, new Vector3(start.x, end.y, start.z), Color.green);
			Debug.DrawLine(start, new Vector3(start.x, start.y, end.z), Color.green);

			Debug.DrawLine(new Vector3(end.x, end.y, start.z), new Vector3(start.x, end.y, start.z), Color.green);
			Debug.DrawLine(new Vector3(end.x, end.y, start.z), new Vector3(end.x, start.y, start.z), Color.green);
			Debug.DrawLine(new Vector3(end.x, end.y, start.z), new Vector3(end.x, end.y, end.z), Color.green);

			Debug.DrawLine(new Vector3(start.x, end.y, end.z), new Vector3(end.x, end.y, end.z), Color.green);
			Debug.DrawLine(new Vector3(start.x, end.y, end.z), new Vector3(start.x, start.y, end.z), Color.green);
			Debug.DrawLine(new Vector3(start.x, end.y, end.z), new Vector3(start.x, end.y, start.z), Color.green);

			Debug.DrawLine(new Vector3(end.x, start.y, end.z), new Vector3(start.x, start.y, end.z), Color.green);
			Debug.DrawLine(new Vector3(end.x, start.y, end.z), new Vector3(end.x, end.y, end.z), Color.green);
			Debug.DrawLine(new Vector3(end.x, start.y, end.z), new Vector3(end.x, start.y, start.z), Color.green);
		}

		private Vector3Int GetChunkPosition(Vector3 worldPosition)
		{
			return Vector3Int.FloorToInt(worldPosition / VoxelSettings.Instance.ChunkSize);
		}
	}
}