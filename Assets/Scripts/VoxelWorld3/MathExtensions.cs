namespace VoxelWorld3
{
	public static class MathUtils
	{
		/// <summary>
		/// "true" modulus operation (always in the interval [0, rhs] if rhs greater than 0 or [rhs, 0] if rhs lower than 0)
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static int Mod(int lhs, int rhs)
		{
			int res = lhs % rhs;
			return rhs > 0 && res < 0 || rhs < 0 && res > 0 ? res + rhs : res;
		}
	}
}