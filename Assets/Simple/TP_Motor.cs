using UnityEngine;
using System.Collections;

namespace RollRoti.Simple_TPSystem
{
	public class TP_Motor : MonoBehaviour 
	{
		public static TP_Motor Instance;

		public float moveSpeed = 10f;

		public Vector3 moveVector { get; set; }

		void Awake () 
		{
			Instance = this;
		}

		void ProcessMotion ()
		{
			moveVector = transform.TransformDirection (moveVector);

			if (moveVector.magnitude > 1f)
				moveVector = Vector3.Normalize (moveVector);

			moveVector *= moveSpeed;

			moveVector *= Time.deltaTime;

			TP_Controller.CharacterController.Move (moveVector);
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

		public void UpdateMotor () 
		{
			SnapAlignCharacterWithCamera ();
			ProcessMotion ();
		}
	}
}