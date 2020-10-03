using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
	/// <summary>
	/// 1-loop shortcut to 3-loop x,y,z iteration
	/// </summary>
	public class CoordinateIterator : IEnumerator<Vector3Int>, IEnumerable<Vector3Int>
	{
		public readonly Vector3Int size;
		public readonly Vector3Int offset;
		public readonly Vector3Int target;

		public Vector3Int Current { get; private set; }

		object IEnumerator.Current => Current;

		public CoordinateIterator(Vector3Int size, Vector3Int offset)
		{
			if(size.x <= 0 || size.y <= 0 || size.z <= 0)
				throw new ArgumentException("Size must be greater than 0");

			this.size = size;
			this.offset = offset;
			this.target = this.offset + this.size - Vector3Int.one;
			Reset();
		}

		public void Reset()
		{
			//place pointer before first element
			this.Current = new Vector3Int(this.offset.x - 1, this.offset.y, this.offset.z);
		}

		public bool MoveNext()
		{
			if (this.Current.x < this.target.x)
			{
				//move first left to right
				this.Current = new Vector3Int(this.Current.x + 1, this.Current.y, this.Current.z);
			}
			else if (this.Current.z < this.target.z)
			{
				//if line is done, start the next one back to front
				this.Current = new Vector3Int(this.offset.x, this.Current.y, this.Current.z + 1);
			}
			else if (this.Current.y < this.target.y)
			{
				//if layer is done start the next one down to up
				this.Current = new Vector3Int(this.offset.x, this.Current.y + 1, this.offset.z);
			}
			else
			{
				//end
				return false;
			}

			return true;
		}

		public void Dispose() { }

		public IEnumerator<Vector3Int> GetEnumerator() => this;

		IEnumerator IEnumerable.GetEnumerator() => this;
	}
}