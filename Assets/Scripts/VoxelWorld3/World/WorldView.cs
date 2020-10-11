using System.Collections.Generic;
using UnityEngine;
using VoxelWorld3.Chunks;
using Object = UnityEngine.Object;

namespace VoxelWorld3.World
{
	public class WorldView : MonoBehaviour
	{
		[SerializeField] private GameObject chunkViewPrefab;
		
		/// <summary>
		/// Rendered chunks (i.e. play area)
		/// </summary>
		private readonly Dictionary<Vector2Int, ChunkView> chunkViews = new Dictionary<Vector2Int, ChunkView>();
		
		private WorldService worldService;

		private void Start()
		{
			this.worldService = WorldService.Instance;
			this.worldService.OnChunksLoaded += this.OnChunksLoaded;
			this.worldService.OnChunksUnloaded += this.OnChunksUnloaded;
		}

		private void OnChunksLoaded(IEnumerable<Vector2Int> positions)
		{
			foreach (Vector2Int position in positions)
			{
				if (this.chunkViews.ContainsKey(position)) continue;
				Chunk chunk = this.worldService.GetChunkFromChunkPosition(position);
				ChunkView chunkView = Object.Instantiate(this.chunkViewPrefab, this.transform).GetComponent<ChunkView>();
				chunkView.SetChunk(chunk);
				this.chunkViews.Add(position, chunkView);
			}
			RenderAll();
		}

		private void OnChunksUnloaded(IEnumerable<Vector2Int> positions)
		{
			foreach (Vector2Int position in positions)
			{
				if (!this.chunkViews.ContainsKey(position)) continue;
				this.chunkViews[position].Destroy();
				this.chunkViews.Remove(position);
			}
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