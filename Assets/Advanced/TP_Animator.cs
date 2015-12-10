using UnityEngine;
using System.Collections;

namespace RollRoti.Advanced_TPSystem
{
	public class TP_Animator : MonoBehaviour 
	{
		public enum Direction 
		{
			Stationary, Forward, Backward, Left, Right,
			LeftForward, RightForward, LeftBackward, RightBackward
		}

		public static TP_Animator Instance;

		public Direction MoveDirection { get; set; }

		void Awake () 
		{
			Instance = this;
		}
		
		void Update () {
		
		}

		public void DetermineCurrentMoveDirection ()
		{
			var forward = false;
			var backward = false;
			var left = false;
			var right = false;

			if(TP_Motor.Instance.moveVector.z > 0f)
			{
				forward = true;
			}
			if(TP_Motor.Instance.moveVector.z < 0f)
			{
				backward = true;
			}
			if(TP_Motor.Instance.moveVector.x > 0f)
			{
				right = true;
			}
			if(TP_Motor.Instance.moveVector.x < 0f)
			{
				left = true;
			}

			if (forward)
			{
				if (left)
					MoveDirection = Direction.LeftForward;
				else if (right)
					MoveDirection = Direction.RightForward;
				else
					MoveDirection = Direction.Forward;

			}
			else if (backward)
			{
				if (left)
					MoveDirection = Direction.LeftBackward;
				else if (right)
					MoveDirection = Direction.RightBackward;
				else
					MoveDirection = Direction.Backward;
			}
			else if (left)
			{
				MoveDirection = Direction.Left;
			}
			else if (right)
			{
				MoveDirection = Direction.Right;
			}
			else
			{
				MoveDirection = Direction.Stationary;
			}
		}
	}
}