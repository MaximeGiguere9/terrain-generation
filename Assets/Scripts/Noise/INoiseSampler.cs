using UnityEngine;

namespace Noise
{
	public interface INoiseSampler
	{
		float Sample(float x, float y, float z);

		float Sample(Vector3 position);
	}
}