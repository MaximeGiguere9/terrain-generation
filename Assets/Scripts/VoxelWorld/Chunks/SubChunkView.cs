using UnityEngine;

namespace VoxelWorld.Chunks
{
	public class SubChunkView : MonoBehaviour
	{
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private MeshCollider meshCollider;

		private SubChunk subChunk;

		public void SetSubChunk(SubChunk subChunk)
		{
			if (this.subChunk == subChunk) return;
			if(this.subChunk != null) this.subChunk.OnMeshInvalidated -= this.OnMeshInvalidated;
			this.subChunk = subChunk;
			this.subChunk.OnMeshInvalidated += this.OnMeshInvalidated;
			this.OnMeshInvalidated();
		}

		private void OnMeshInvalidated()
		{
			Mesh mesh = this.subChunk.GetMesh();
			if (this.meshFilter.mesh == mesh) return;
			this.meshFilter.mesh = mesh;
			this.meshCollider.sharedMesh = this.meshFilter.sharedMesh;
			this.gameObject.SetActive(mesh.vertexCount > 0);
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.gameObject);
		}

		private void OnDestroy()
		{
			if (this.subChunk != null) this.subChunk.OnMeshInvalidated -= this.OnMeshInvalidated;
		}
	}
}