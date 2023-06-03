using UnityEngine;

namespace VoxelWorld.Blocks
{
	public interface IBlockService
	{
		Vector3 GetBlockSize();
		Vector3Int[] GetVertexOrder();
		Vector3Int[] GetFaceOrder();
		byte[][] GetFaceVertexOrder();
		byte[] GetFaceTriangleOrder();
	}
}