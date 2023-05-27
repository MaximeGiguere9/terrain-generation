using UnityEngine;
using Utils;
using VoxelWorld.Blocks;

namespace VoxelWorld.World
{
	public class BlockTargeterBehaviour : MonoBehaviour
	{
		[SerializeField] private Transform cameraTransform;
		[SerializeField] private float maxTargetRange = 4;
		[SerializeField] private GameObject targetingIndicator;

		private void Update()
		{
			BlockTargeter.HitPoint targetHit = BlockTargeter.Target(
				this.cameraTransform.position,
				this.cameraTransform.forward,
				this.maxTargetRange,
				pos => WorldService.Instance.GetBlockAt(pos),
				BlockService.Instance
			);

			if (targetHit == null)
			{
				this.targetingIndicator.SetActive(false);
				return;
			}

			// the hit position is the position of the block (min pos in world coords)
			// move the targeting indicator to the middle of the block face hit and activate it
			Vector3 centerBlockPosition = targetHit.Position + Vector3.one / 2;
			this.targetingIndicator.transform.position = centerBlockPosition + (Vector3) targetHit.Normal * 0.51f;
			this.targetingIndicator.transform.LookAt(centerBlockPosition);
			this.targetingIndicator.SetActive(true);

			if (Input.GetButtonDown("Fire1"))
			{
				// break block
				WorldService.Instance.SetBlockAt(targetHit.Position, 0);
			}
			else if (Input.GetButtonDown("Fire2"))
			{
				// place block
				WorldService.Instance.SetBlockAt(targetHit.Position + targetHit.Normal, 6);
			}
		}
	}
}