using UnityEngine;

public class PerlinView : MonoBehaviour
{
	[SerializeField] private float seed;
	[SerializeField] private float samplingInterval = 1f;
	[SerializeField] private Vector3 dimensions;

	private void OnValidate()
	{
		if (this.samplingInterval <= 0.1f) this.samplingInterval = 0.1f;
		PerlinNoise.Seed = seed;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(transform.position, dimensions);

		float x0 = transform.position.x - dimensions.x / 2f;
		float x1 = transform.position.x + dimensions.x / 2f;
		float y0 = transform.position.y - dimensions.y / 2f;
		float y1 = transform.position.y + dimensions.y / 2f;
		float z0 = transform.position.z - dimensions.z / 2f;
		float z1 = transform.position.z + dimensions.z / 2f;

		for (float z = z0; z < z1; z += samplingInterval)
		{
			for (float y = y0; y < y1; y += samplingInterval)
			{
				for (float x = x0; x < x1; x += samplingInterval)
				{
					Vector3 pos = new Vector3(x, y, z);
					float val = SimplePerlin3D.Sample(x, y, z);
					Gizmos.color = val <= 0.4f ? Color.black : Color.white;
					Gizmos.DrawSphere(pos, samplingInterval / 8f);
				}
			}
		}
	}
}
