using System.Collections;
using System.Linq;
using UnityEngine;

namespace WorldObjects
{
	/// <summary>
	/// Block script v1
	/// A simple cube mesh is active if not surrounded by other cubes
	/// </summary>
	public class BlockV1Cube : MonoBehaviour
	{
		private static readonly Vector3[] Dirs = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };

		private void Start()
		{
			StartCoroutine(DeactivateIfSurrounded());
		}

		private IEnumerator DeactivateIfSurrounded()
		{
			//stay active if any direction is unobstructed
			if (Dirs.Any(dir => !Physics.Raycast(transform.position, dir, 1f))) yield break;
			//wait for other blocks to perform their own checks before changing state
			yield return new WaitForEndOfFrame();
			gameObject.SetActive(false);
		}
	}
}
