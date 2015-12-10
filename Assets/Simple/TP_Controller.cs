using UnityEngine;
using System.Collections;

namespace RollRoti.Simple_TPSystem
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
			TP_Motor.Instance.UpdateMotor ();

		}

		void GetLocomotionInput ()
		{
			float deadZone = 0.1f;

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
		}
	}
}