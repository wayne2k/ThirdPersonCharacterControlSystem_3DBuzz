using UnityEngine;
using System.Collections;

namespace RollRoti.Simple_TPSystem
{
	public class TP_Camera : MonoBehaviour 
	{
		public static TP_Camera Instance;

		public Transform targetLookAt;
		public float distance = 5f;
		public float distanceMin = 3f;
		public float distanceMax = 10f;
		public float distanceSmooth = 0.05f;
		public float x_MouseSensitivity = 5f;
		public float y_MouseSensitivity = 5f;
		public float MouseWheelSensitivity = 5f;
		public float x_Smooth = 0.05f;
		public float y_Smooth = 0.1f;
		public float y_MinLimit = -40f;
		public float y_MaxLimit = 80f;

		private float mouseX;
		private float mouseY;
		private float velX;
		private float velY;
		private float velZ;
		private float startDistance;
		private float desiredDistance;
		private Vector3 desiredPosition;
		private Vector3 position;
		private float velDistance;

		void Awake ()
		{
			Instance = this;
		}

		void Start () 
		{
			distance = Mathf.Clamp (distance, distanceMin, distanceMax);
			startDistance = distance;
			Reset ();
		}

		void LateUpdate () 
		{
			if (targetLookAt == null) {
				return;
			}

			HandlePlayerInput ();

			CalculateDesiredPosition ();

			UpdatePosition ();
		}

		void HandlePlayerInput ()
		{
			float deadZone = 0.01f;

			if (Input.GetMouseButton (1)) 
			{
				mouseX += Input.GetAxis ("Mouse X") * x_MouseSensitivity;
				mouseY -= Input.GetAxis ("Mouse Y") * y_MouseSensitivity;
			}

			mouseY = Helper.ClampAngle (mouseY, y_MinLimit, y_MaxLimit);

			float mouseScrollWheel = Input.GetAxis ("Mouse ScrollWheel");

			if (mouseScrollWheel < -deadZone || mouseScrollWheel > deadZone) 
			{
				desiredDistance = Mathf.Clamp (distance - mouseScrollWheel * MouseWheelSensitivity, distanceMin, distanceMax);
			}
		}

		void CalculateDesiredPosition ()
		{
			distance = Mathf.SmoothDamp (distance, desiredDistance, ref velDistance, distanceSmooth);

			desiredPosition = CalculatePosition (mouseY, mouseX, distance);
		}

		Vector3 CalculatePosition (float rotationX, float rotationY, float distance)
		{
			Vector3 direction = new Vector3 (0f, 0f, -distance);
			Quaternion rotation = Quaternion.Euler (rotationX, rotationY, 0f);

			return targetLookAt.position + rotation * direction;
		}

		void UpdatePosition ()
		{
			float posX = Mathf.SmoothDamp (position.x, desiredPosition.x, ref velX, x_Smooth);
			float posy = Mathf.SmoothDamp (position.y, desiredPosition.y, ref velY, y_Smooth);
			float posz = Mathf.SmoothDamp (position.z, desiredPosition.z, ref velZ, x_Smooth);

			position = new Vector3 (posX, posy, posz);

			transform.position = position;

			transform.LookAt (targetLookAt);
		}

		void Reset ()
		{
			mouseX = 0f;
			mouseY = 10f;
			distance = startDistance;
			desiredDistance = distance;
		}

		public static void UseExistingOrCreateNewMainCamera ()
		{
			GameObject tempCamera;
			GameObject temptargetLookAt;
			TP_Camera myCamera;

			if (Camera.main != null) 
			{
				tempCamera = Camera.main.gameObject;
			} 
			else 
			{
				tempCamera = new GameObject ("Main Camera");
				tempCamera.AddComponent <Camera> ();
				tempCamera.tag = "MainCamera";
			}

			tempCamera.AddComponent <TP_Camera> ();
			myCamera = tempCamera.GetComponent <TP_Camera> ();

			temptargetLookAt = GameObject.Find ("TargetLookAt") as GameObject;

			if (temptargetLookAt == null) 
			{
				temptargetLookAt = new GameObject ("TargetLookAt");
				temptargetLookAt.transform.position = Vector3.zero;
			}

			myCamera.targetLookAt = temptargetLookAt.transform;
		}
	}
}