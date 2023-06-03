namespace VoxelWorld.Controls
{
	public interface IFirstPersonInputHandler
	{
		float GetAimX();
		float GetAimY();
		float GetMoveX();
		float GetMoveY();
		bool GetJump();
		bool GetCrouch();
		bool GetSprint();
		bool GetPlaceBlock();
		bool GetBreakBlock();
	}
}