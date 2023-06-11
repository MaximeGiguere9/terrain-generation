using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.Utils
{
	/// <summary>
	/// 1-loop shortcut to 3-loop x,y,z iteration.
	/// The loop is "x-major", then prioritizes z, then y.
	/// </summary>
	public class CoordinateIterator : IEnumerator<Vector3Int>, IEnumerable<Vector3Int>
	{
		public readonly Vector3Int size;
		public readonly Vector3Int offset;
		public readonly Vector3Int target;

		private Vector3Int current;

		public Vector3Int Current => current;

		object IEnumerator.Current => current;

		/// <summary>
		/// 1d offset of the current position relative to the start of the iterator
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// 1d size of the iterator (size.x * size.y * size.z)
		/// </summary>
		public int Volume { get; }

		public CoordinateIterator(Vector3Int size, Vector3Int offset)
		{
			if (size.x <= 0 || size.y <= 0 || size.z <= 0)
				throw new ArgumentException("Size must be greater than 0");

			this.size = size;
			this.offset = offset;
			target = this.offset + this.size - Vector3Int.one;

			current = new Vector3Int(this.offset.x - 1, this.offset.y, this.offset.z);

			Index = -1;
			Volume = this.size.x * this.size.y * this.size.z;
		}

		public void Reset()
		{
			//place pointer before first element
			Index = -1;

			current.x = offset.x - 1;
			current.y = offset.y;
			current.z = offset.z;
		}

		public bool MoveNext()
		{
			if (current.x < target.x)
			{
				//move first left to right
				current.x++;
			}
			else if (current.z < target.z)
			{
				//if line is done, start the next one back to front
				current.z++;
				current.x = offset.x;
			}
			else if (current.y < target.y)
			{
				//if layer is done start the next one down to up
				current.y++;
				current.x = offset.x;
				current.z = offset.z;
			}
			else
			{
				//end
				return false;
			}

			Index++;
			return true;
		}

		public void Dispose() { }

		public IEnumerator<Vector3Int> GetEnumerator() => this;

		IEnumerator IEnumerable.GetEnumerator() => this;
	}
}