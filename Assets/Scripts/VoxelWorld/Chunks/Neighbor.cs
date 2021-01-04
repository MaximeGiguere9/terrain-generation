using System;
using UnityEngine;

namespace VoxelWorld.Chunks
{
	/// <summary>
	/// North and South refer to the z axis in 3d space, which means they are reversed when compared to axes on a 2d plane.
	/// Going counter-clockwise on the xz axis means going from +x to -z first.
	/// </summary>
	public sealed class Neighbor
	{
		/// <summary>
		/// (1, 0)
		/// </summary>
		public static readonly Neighbor East = new Neighbor(0, Vector2Int.right);
		/// <summary>
		/// (0, -1)
		/// </summary>
		public static readonly Neighbor North = new Neighbor(1, Vector2Int.down);
		/// <summary>
		/// (-1, 0)
		/// </summary>
		public static readonly Neighbor West = new Neighbor(2, Vector2Int.left);
		/// <summary>
		/// (0, 1)
		/// </summary>
		public static readonly Neighbor South = new Neighbor(3, Vector2Int.up);
		public static Neighbor[] All = { East, North, West, South };

		public static Neighbor Opposite(Neighbor value)
		{
			if (value == East) return West;
			if (value == West) return East;
			if (value == South) return North;
			if (value == North) return South;
			throw new ArgumentException();
		}

		public readonly int Index;
		public readonly Vector2Int Value;

		private Neighbor(int index, Vector2Int value)
		{
			this.Index = index;
			this.Value = value;
		}
	}
}