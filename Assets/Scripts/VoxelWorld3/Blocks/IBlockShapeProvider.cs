using UnityEngine;

namespace VoxelWorld3.Blocks
{
	public interface IBlockShapeProvider
	{
		Vector3Int[] GetVertexOrder();
		Vector3Int[] GetFaceOrder();
		byte[][] GetFaceVertexOrder();
		byte[] GetFaceTriangleOrder();
	}
}