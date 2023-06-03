using UnityEngine;
using Utils;
using VoxelWorld.Blocks;
using VoxelWorld.World;

namespace VoxelWorld.Controls
{
	public class BlockTargetingController : MonoBehaviour
	{
		[SerializeField] private Transform cameraTransform;
		[SerializeField] private float maxTargetRange = 4;
		[SerializeField] private GameObject targetingIndicator;

		private IFirstPersonInputHandler inputHandler = new UnityInputHandler();

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

			if (inputHandler.GetBreakBlock())
			{
				// break block
				WorldService.Instance.SetBlockAt(targetHit.Position, 0);
			}
			else if (inputHandler.GetPlaceBlock())
			{
				// place block
				WorldService.Instance.SetBlockAt(targetHit.Position + targetHit.Normal, 6);
			}
		}
	}
}