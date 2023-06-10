using System.Runtime.CompilerServices;
using UnityEngine;

namespace VoxelWorld.Blocks
{
	/// <summary>
	/// Contains data about block geometry and some helper methods to build block meshes.
	/// This class is stateless and only provides block geometry data.
	/// </summary>
	public static class BlockMeshModel
	{
		/// <summary>
		/// Positions of all 8 vertices that form a block, as an array of vectors.
		/// Vertices[i] returns the coordinates of one vertex as a vector.
		/// Vertices[i][j] returns the component (x, y or z) of the vertex position.
		/// </summary>
		public static readonly Vector3Int[] Vertices =
		{
			new Vector3Int(0, 0, 0),
			new Vector3Int(1, 0, 0),
			new Vector3Int(1, 0, 1),
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 1, 0),
			new Vector3Int(1, 1, 0),
			new Vector3Int(1, 1, 1),
			new Vector3Int(0, 1, 1)
		};

		/// <summary>
		/// The 6 face normals of a the block as an array of vectors.
		/// Faces[i] returns one of the normals as a vector.
		/// Faces[i][j] returns the component [x, y or z] of a normal direction.
		/// </summary>
		public static readonly Vector3Int[] FaceNormals =
		{
			new Vector3Int(1, 0, 0),
			new Vector3Int(-1, 0, 0),
			new Vector3Int(0, 1, 0),
			new Vector3Int(0, -1, 0),
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 0, -1)
		};

		/// <summary>
		/// The 4 vertices forming a face of a block. 
		/// <br/>
		/// FaceVertexOrder[i] corresponds to the index of the face that is formed by the vertices. 
		/// It relates to the corresponding face normal in <see cref="FaceNormals"/>.
		/// <br/>
		/// FaceVertexOrder[i][j] corresponds to the indices of the 4 vertices that form the face. 
		/// They relate to the corresponding vertices in <see cref="Vertices"/>.
		/// </summary>
		public static readonly byte[][] FaceVertexOrder =
		{
			new byte[] {1, 2, 6, 5},
			new byte[] {3, 0, 4, 7},
			new byte[] {4, 5, 6, 7},
			new byte[] {3, 2, 1, 0},
			new byte[] {2, 3, 7, 6},
			new byte[] {0, 1, 5, 4}
		};

		/// <summary>
		/// Order in which the vertex indices defined in <see cref="FaceVertexOrder"/> can be used to make triangles.
		/// A block face is made of four vertices that are used in two triangles.
		/// </summary>
		public static readonly byte[] FaceTriangleOrder = { 0, 1, 2, 0, 2, 3 };

		/// <summary>
		/// Allocates a new array to hold the four face vertices of a block.
		/// Use this array when invoking <see cref="GetFaceVerticesInWorldSpace(in int, in int, in int, in int, ref Vector3[])"/>
		/// and reuse it to limit memory allocation.
		/// </summary>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3[] AllocateFaceVerticesArray()
		{
			return new Vector3[4];
		}

		/// <summary>
		/// Given the position of a block in world space, calculates the position of the vertices of one of the block's faces.
		/// </summary>
		/// <param name="x">coordinate component in world space</param>
		/// <param name="y">coordinate component in world space</param>
		/// <param name="z">coordinate component in world space</param>
		/// <param name="faceIndex">Index of the block face to create vertices for. The the face order defined in <see cref="FaceVertexOrder"/></param>
		/// <param name="vertices">Vector array to hold the positions of the four vertices</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetFaceVerticesInWorldSpace(in int x, in int y, in int z, in int faceIndex, ref Vector3[] vertices)
		{
			vertices[0].x = x + Vertices[FaceVertexOrder[faceIndex][0]][0];
			vertices[0].y = y + Vertices[FaceVertexOrder[faceIndex][0]][1];
			vertices[0].z = z + Vertices[FaceVertexOrder[faceIndex][0]][2];

			vertices[1].x = x + Vertices[FaceVertexOrder[faceIndex][1]][0];
			vertices[1].y = y + Vertices[FaceVertexOrder[faceIndex][1]][1];
			vertices[1].z = z + Vertices[FaceVertexOrder[faceIndex][1]][2];

			vertices[2].x = x + Vertices[FaceVertexOrder[faceIndex][2]][0];
			vertices[2].y = y + Vertices[FaceVertexOrder[faceIndex][2]][1];
			vertices[2].z = z + Vertices[FaceVertexOrder[faceIndex][2]][2];

			vertices[3].x = x + Vertices[FaceVertexOrder[faceIndex][3]][0];
			vertices[3].y = y + Vertices[FaceVertexOrder[faceIndex][3]][1];
			vertices[3].z = z + Vertices[FaceVertexOrder[faceIndex][3]][2];
		}

		/// <summary>
		/// Allocates a new array to hold the UVs associated with the four face vertices of a block.
		/// Use this array when invoking <see cref="GetFaceUVs(byte, int, ref Vector2[])"/>
		/// and reuse it to limit memory allocation.
		/// </summary>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2[] AllocateFaceUVsArray()
		{
			return new Vector2[4];
		}

		/// <summary>
		/// Given a texture index in an atlas, offsets the UVs of a block face to show that texture.
		/// </summary>
		/// <param name="textureIndex">Texture index to use for the block face</param>
		/// <param name="uvs">Vector array to hold the UVs of the four vertices</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetFaceUVs(int textureIndex, ref Vector2[] uvs)
		{
			int x = textureIndex % 16;
			int y = textureIndex / 16;

			uvs[0].x = x / 16f;
			uvs[0].y = y / 16f;

			uvs[1].x = (x + 1) / 16f;
			uvs[1].y = y / 16f;

			uvs[2].x = (x + 1) / 16f;
			uvs[2].y = (y + 1) / 16f;

			uvs[3].x = x / 16f;
			uvs[3].y = (y + 1) / 16f;
		}

	}
}