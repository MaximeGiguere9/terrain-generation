using System;
using UnityEngine;

namespace VoxelWorld.Chunks
{
	public class SubChunkView : MonoBehaviour
	{
		[SerializeField] private MeshFilter[] meshFilters;
		[SerializeField] private MeshCollider meshCollider;

		private SubChunk subChunk;

		private bool shouldUpdateMesh;

		public void SetSubChunk(SubChunk subChunk)
		{
			if (this.subChunk == subChunk) return;
			if(this.subChunk != null) this.subChunk.GetRenderer().OnMeshInvalidated -= this.OnMeshInvalidated;
			this.subChunk = subChunk;
			this.subChunk.GetRenderer().OnMeshInvalidated += this.OnMeshInvalidated;
			this.OnMeshInvalidated();
		}

		private void OnMeshInvalidated()
		{
			this.shouldUpdateMesh = true;
		}

		private void Update()
		{
			if (this.shouldUpdateMesh)
			{
				// workaround for a regession causing error spam in unity 2021 when assigning a mesh that has 0 vertices
				for (int i = 0; i < this.meshFilters.Length - 1; i++)
				{
					this.meshFilters[i].mesh = null;
				}
				this.meshCollider.sharedMesh = null;

				Mesh[] mesh = this.subChunk.GetRenderer().GetMesh();

				for (int i = 0; i < mesh.Length - 1; i++)
				{
					try
					{
						if (this.meshFilters[i].mesh != mesh[i] && mesh[i].vertexCount != 0)
						{
							this.meshFilters[i].mesh = mesh[i];
						}
					}
					catch (IndexOutOfRangeException)
					{
						Debug.LogError($"Sub Chunk Views need exactly {mesh.Length} mesh filters and colliders.");
					}
				}

				if (mesh[mesh.Length - 1].vertexCount > 0)
				{
					this.meshCollider.sharedMesh = mesh[mesh.Length - 1];
				}

				this.shouldUpdateMesh = false;
			}
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.gameObject);
		}

		private void OnDestroy()
		{
			if (this.subChunk != null) this.subChunk.GetRenderer().OnMeshInvalidated -= this.OnMeshInvalidated;
		}
	}
}