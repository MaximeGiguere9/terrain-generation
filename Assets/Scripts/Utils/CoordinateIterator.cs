using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
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

		public Vector3Int Current => this.current;

		object IEnumerator.Current => this.current;

		/// <summary>
		/// 1d offset of the current position relative to the start of the iterator
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// 1d size of the iterator (size.x * size.y * size.z)
		/// </summary>
		public int Volume { get; }

		private int[] currentArr;
		private readonly int[] offsetArr;
		private readonly int[] targetArr;

		public CoordinateIterator(Vector3Int size, Vector3Int offset)
		{
			if(size.x <= 0 || size.y <= 0 || size.z <= 0)
				throw new ArgumentException("Size must be greater than 0");

			this.size = size;
			this.offset = offset;
			this.target = this.offset + this.size - Vector3Int.one;
			this.Volume = this.size.x * this.size.y * this.size.z;

			this.offsetArr = new[] { this.offset.x, this.offset.y, this.offset.z };
			this.targetArr = new[] { this.target.x, this.target.y, this.target.z };

			Reset();
		}

		public void Reset()
		{
			//place pointer before first element
			this.current = new Vector3Int(this.offset.x - 1, this.offset.y, this.offset.z);
			this.Index = -1;
			this.currentArr = new[] {this.offset.x - 1, this.offset.y, this.offset.z};
		}

		public bool MoveNext()
		{
			/*if (this.current.x < this.target.x)
			{
				//move first left to right
				this.current.x++;
			}
			else if (this.current.z < this.target.z)
			{
				//if line is done, start the next one back to front
				this.current.z++;
				this.current.x = this.offset.x;
			}
			else if (this.current.y < this.target.y)
			{
				//if layer is done start the next one down to up
				this.current.y++;
				this.current.x = this.offset.x;
				this.current.z = this.offset.z;
			}
			else
			{
				//end
				return false;
			}*/

			if (this.currentArr[0] < this.targetArr[0])
			{
				//move first left to right
				this.currentArr[0]++;

				this.current.x = this.currentArr[0];
			}
			else if (this.currentArr[2] < this.targetArr[2])
			{
				//if line is done, start the next one back to front
				this.currentArr[2]++;
				this.currentArr[0] = this.offsetArr[0];

				this.current.x = this.currentArr[0];
				this.current.z = this.currentArr[2];
			}
			else if (this.currentArr[1] < this.targetArr[1])
			{
				//if layer is done start the next one down to up
				this.currentArr[1]++;
				this.currentArr[0] = this.offsetArr[0];
				this.currentArr[2] = this.offsetArr[2];

				this.current.x = this.currentArr[0];
				this.current.y = this.currentArr[1];
				this.current.z = this.currentArr[2];
			}
			else
			{
				//end
				return false;
			}

			this.Index++;
			return true;
		}

		public void Dispose() { }

		public IEnumerator<Vector3Int> GetEnumerator() => this;

		IEnumerator IEnumerable.GetEnumerator() => this;
	}
}