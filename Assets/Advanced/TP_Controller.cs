using UnityEngine;
using System.Collections;

namespace RollRoti.Advanced_TPSystem
{
	public class TP_Controller : MonoBehaviour 
	{
		public static CharacterController CharacterController;
		public static TP_Controller Instance;

		void Awake () 
		{
			Instance = this;
			CharacterController = GetComponent <CharacterController> ();
			TP_Camera.UseExistingOrCreateNewMainCamera ();
		}

		void Update () 
		{
			if (Camera.main == null)
				return;

			GetLocomotionInput ();
			HandleActionInput ();

			TP_Motor.Instance.UpdateMotor ();

		}

		void GetLocomotionInput ()
		{
			float deadZone = 0.1f;

			TP_Motor.Instance.VerticalVelocity = TP_Motor.Instance.moveVector.y;
			TP_Motor.Instance.moveVector = Vector3.zero;

			float h = Input.GetAxis ("Horizontal");
			float v = Input.GetAxis ("Vertical");

			if (v > deadZone || v < -deadZone)
			{
				TP_Motor.Instance.moveVector += new Vector3 (0f, 0f, v);
			}

			if (h > deadZone || h < -deadZone)
			{
				TP_Motor.Instance.moveVector += new Vector3 (h, 0f, 0f);
			}

			TP_Animator.Instance.DetermineCurrentMoveDirection ();
		}

		void HandleActionInput ()
		{
			if (Input.GetButton ("Jump"))
			{
				Jump ();
			}
		}

		void Jump ()
		{
			TP_Motor.Instance.Jump ();
		}
	}
}