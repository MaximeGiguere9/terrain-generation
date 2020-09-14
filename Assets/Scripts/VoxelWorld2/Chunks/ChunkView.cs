using UnityEngine;

namespace VoxelWorld2.Chunks
{
	public class ChunkView : MonoBehaviour
	{
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private MeshCollider meshCollider;

		public void SetMesh(Mesh mesh)
		{
			this.meshFilter.mesh = mesh;
			this.meshCollider.sharedMesh = this.meshFilter.sharedMesh;
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.gameObject);
		}
	}
}