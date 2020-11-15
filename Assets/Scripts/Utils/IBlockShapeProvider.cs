using UnityEngine;

namespace Utils
{
	public interface IBlockShapeProvider
	{
		Vector3Int[] GetVertexOrder();
		Vector3Int[] GetFaceOrder();
		byte[][] GetFaceVertexOrder();
		byte[] GetFaceTriangleOrder();
	}
}