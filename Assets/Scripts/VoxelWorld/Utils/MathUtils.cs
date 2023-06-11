namespace VoxelWorld.Utils
{
	public static class MathUtils
	{
		/// <summary>
		/// "true" modulus operation <br/>
		/// Returns a value that is always in the interval [0, rhs[ if rhs > 0, or ]rhs, 0] if rhs < 0. <br/>
		/// This differs from the default % operator (remainder), which returns a value in the ]-rhs, rhs[ interval.
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