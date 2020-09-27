using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld3
{
	public class ChunkView : MonoBehaviour
	{
		[SerializeField] private GameObject subChunkPrefab;

		private Chunk chunk;

		private readonly List<SubChunkView> subChunkViews = new List<SubChunkView>();

		public void SetChunk(Chunk chunk)
		{
			this.chunk = chunk;
			foreach (SubChunkView v in this.subChunkViews) v.Destroy();
			this.subChunkViews.Clear();
			for (byte i = 0; i < chunk.GetSubdivisionCount(); i++)
			{
				SubChunkView v = Instantiate(this.subChunkPrefab, this.transform).GetComponent<SubChunkView>();
				this.subChunkViews.Add(v);
			}
		}

		public void Render()
		{
			for (byte i = 0; i < this.subChunkViews.Count; i++)
			{
				SubChunk subChunk = this.chunk.GetSubChunk(i);
				this.subChunkViews[i].SetMesh(subChunk.GetMesh());
			}

			Vector2Int pos = this.chunk.GetWorldSpacePosition();
			gameObject.transform.position = new Vector3(pos.x, 0, pos.y);
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.gameObject);
		}
	}
}