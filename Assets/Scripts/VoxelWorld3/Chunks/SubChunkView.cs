using UnityEngine;

namespace VoxelWorld3.Chunks
{
	public class SubChunkView : MonoBehaviour
	{
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private MeshCollider meshCollider;

		public void SetMesh(Mesh mesh)
		{
			if (this.meshFilter.mesh == mesh) return;
			this.meshFilter.mesh = mesh;
			this.meshCollider.sharedMesh = this.meshFilter.sharedMesh;
			this.gameObject.SetActive(mesh.vertexCount > 0);
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.gameObject);
		}
	}
}