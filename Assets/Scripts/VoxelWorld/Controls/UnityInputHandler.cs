using UnityEngine;

namespace VoxelWorld.Controls
{
	public class UnityInputHandler : IFirstPersonInputHandler
	{
		public bool GetBreakBlock() => Input.GetButtonDown("Fire1");

		public bool GetCrouch() => Input.GetButton("Fire3");

		public bool GetJump() => Input.GetButton("Jump");

		public float GetAimX() => Input.GetAxis("Mouse X");

		public float GetAimY() => Input.GetAxis("Mouse Y");

		public float GetMoveX() => Input.GetAxis("Horizontal");

		public float GetMoveY() => Input.GetAxis("Vertical");

		public bool GetPlaceBlock() => Input.GetButtonDown("Fire2");

		public bool GetSprint() => Input.GetButton("Fire2");
	}
}