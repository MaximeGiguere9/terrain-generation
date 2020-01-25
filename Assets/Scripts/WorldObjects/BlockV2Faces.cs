using UnityEngine;

namespace WorldObjects
{
	/// <summary>
	/// Block script v2.
	/// Each face is activated individually if unobstructed.
	/// Pros: less quads are rendered
	/// Cons: object itself and its collider stay active, leading to worse performance overall
	/// </summary>
	public class BlockV2Faces : MonoBehaviour
	{
		[SerializeField] private GameObject facesContainer;
		private GameObject[] faces;

		private static readonly Vector3[] Dirs = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };

		private void Start()
		{
			this.faces = new GameObject[6];
			for (int i = 0; i < 6; i++)
			{
				//assumes faces are ordering the same way as dirs
				this.faces[i] = this.facesContainer.transform.GetChild(i).gameObject;
				this.faces[i].SetActive(!Physics.Raycast(transform.position, Dirs[i], 1f));
			}
		}
	}
}