using UnityEngine;

namespace Noise
{
	public abstract class NoiseSampler
	{
		public abstract float Sample(float x, float y, float z);

		public virtual float Sample(Vector3 position) => Sample(position.x, position.y, position.z);
	}
}