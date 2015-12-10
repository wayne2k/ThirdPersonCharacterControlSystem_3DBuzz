using UnityEngine;
using System.Collections;

namespace RollRoti.Advanced_TPSystem
{
	public class TP_Motor : MonoBehaviour 
	{
		public static TP_Motor Instance;

		public float forwardSpeed = 10f;
		public float backwardSpeed = 2f;
		public float strafingSpeed = 5f;
		public float slideSpeed = 10f;
		public float jumpSpeed = 6f;
		public float gravity = 21;
		public float terminalVelocity = 20f;
		public float slideThreshold = 0.6f;
		public float maxControllableSlideMagnitude = 0.4f;

		Vector3 slideDirection;

		public Vector3 moveVector { get; set; }
		public float VerticalVelocity { get; set; }

		void Awake () 
		{
			Instance = this;
		}

		void ProcessMotion ()
		{
			moveVector = transform.TransformDirection (moveVector);

			if (moveVector.magnitude > 1f)
				moveVector = Vector3.Normalize (moveVector);

			ApplySlide ();

			moveVector *= MoveSpeed ();

			moveVector = new Vector3 (moveVector.x, VerticalVelocity , moveVector.z);

			ApplyGravity ();
			
			TP_Controller.CharacterController.Move (moveVector * Time.deltaTime);
		}

		float MoveSpeed ()
		{
			var moveSpeed = 0f; 

			switch (TP_Animator.Instance.MoveDirection)
			{
				case TP_Animator.Direction.Stationary :  moveSpeed = 0f;
					break;
				case TP_Animator.Direction.Forward :  moveSpeed = forwardSpeed;
					break;
				case TP_Animator.Direction.Backward :  moveSpeed = backwardSpeed;
					break;
				case TP_Animator.Direction.Left :  moveSpeed = strafingSpeed;
					break;
				case TP_Animator.Direction.Right :  moveSpeed = strafingSpeed;
					break;
				case TP_Animator.Direction.LeftForward :  moveSpeed = forwardSpeed;
					break;
				case TP_Animator.Direction.RightForward :  moveSpeed = forwardSpeed;
					break;
				case TP_Animator.Direction.LeftBackward :  moveSpeed = backwardSpeed;
					break;
				case TP_Animator.Direction.RightBackward :  moveSpeed = backwardSpeed;
					break;
			}

			if (slideDirection.sqrMagnitude > 0f)
				moveSpeed = slideSpeed;

			return moveSpeed;
		}

		void SnapAlignCharacterWithCamera ()
		{
			if (moveVector.x != 0 || moveVector.z != 0)
			{
				transform.rotation = Quaternion.Euler (transform.eulerAngles.x,
				                                       Camera.main.transform.eulerAngles.y,
				                                       transform.eulerAngles.z);
			}
		}

		void ApplyGravity ()
		{
			if (moveVector.y > -terminalVelocity)
			{
				moveVector = new Vector3 (moveVector.x, moveVector.y - gravity * Time.deltaTime , moveVector.z);
			}

			if (TP_Controller.CharacterController.isGrounded && moveVector.y < -1f)
			{
				moveVector = new Vector3 (moveVector.x, -1f , moveVector.z);
			}
		}

		void ApplySlide ()
		{
			if (!TP_Controller.CharacterController.isGrounded)
			{
				return;
			}

			slideDirection = Vector3.zero;

			RaycastHit hitInfo;

			//Debug.DrawRay (transform.position, Vector3.down, Color.red, .1f);

			if (Physics.Raycast (transform.position, Vector3.down, out hitInfo))
			{
				if (hitInfo.normal.y < slideThreshold)
				{
					slideDirection = new Vector3 (hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z);
				}
			}

			if (slideDirection.magnitude < maxControllableSlideMagnitude)
			{
				moveVector += slideDirection;
			}
			else
			{
				moveVector = slideDirection;
			}
		}

		public void Jump ()
		{
			if (TP_Controller.CharacterController.isGrounded)
			{
				VerticalVelocity = jumpSpeed;
			}
		}

		public void UpdateMotor () 
		{
			SnapAlignCharacterWithCamera ();
			ProcessMotion ();
		}
	}
}