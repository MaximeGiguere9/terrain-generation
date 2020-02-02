using UnityEngine;

namespace Noise
{
	public class SimplePerlin2D : NoiseSampler
	{
		public override float Sample(float x, float y, float z) => Mathf.PerlinNoise(x, z);
	}
}