using UnityEngine;
using System.Collections;

namespace RollRoti.Advanced_TPSystem
{
	public class TP_Camera : MonoBehaviour 
	{
		public static TP_Camera Instance;

		public Transform targetLookAt;
		public float distance = 5f;
		public float distanceMin = 3f;
		public float distanceMax = 10f;
		public float distanceSmooth = 0.05f;
		public float distanceResumeSmooth = 1f;
		public float x_MouseSensitivity = 5f;
		public float y_MouseSensitivity = 5f;
		public float MouseWheelSensitivity = 5f;
		public float x_Smooth = 0.05f;
		public float y_Smooth = 0.1f;
		public float y_MinLimit = -40f;
		public float y_MaxLimit = 80f;
		public float occlusionDistanceStep = 0.5f;
		public int maxOcclusionChecks = 10;

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
		private float velDistanceResumeSmooth;
		private float preOccludedDistance;

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

			var count = 0;

			do 
			{
				CalculateDesiredPosition ();
				count ++;
			} 
			while (CheckIfOccluded (count));

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

				preOccludedDistance = desiredDistance;
				velDistanceResumeSmooth = distanceSmooth;
			}
		}

		void CalculateDesiredPosition ()
		{
			ResetDesiredDistance ();

			distance = Mathf.SmoothDamp (distance, desiredDistance, ref velDistance, distanceSmooth);

			desiredPosition = CalculatePosition (mouseY, mouseX, distance);
		}

		Vector3 CalculatePosition (float rotationX, float rotationY, float distance)
		{
			Vector3 direction = new Vector3 (0f, 0f, -distance);
			Quaternion rotation = Quaternion.Euler (rotationX, rotationY, 0f);

			return targetLookAt.position + rotation * direction;
		}

		bool CheckIfOccluded (int count)
		{
			bool isOccluded = false;

			var nearestDistance = CheckCameraPoints (targetLookAt.position, desiredPosition);

			if (nearestDistance != -1f)
			{
				if (count < maxOcclusionChecks)
				{
					isOccluded = true;
					distance -= occlusionDistanceStep;

					if (distance < 0.25f)
					{
						distance = 0.25f;
					}
				}
				else
				{
					distance = nearestDistance - Camera.main.nearClipPlane;
				}

				desiredDistance = distance;
				velDistanceResumeSmooth = distanceResumeSmooth;
			}

			return isOccluded;
		}

		void ResetDesiredDistance ()
		{
			if (desiredDistance < preOccludedDistance)
			{
				var pos = CalculatePosition (mouseY, mouseX, preOccludedDistance);

				var nearestDistance = CheckCameraPoints (targetLookAt.position, pos);

				if (nearestDistance == -1f || nearestDistance > preOccludedDistance)
				{
					desiredDistance = preOccludedDistance;
				}
			}
		}

		float CheckCameraPoints (Vector3 from, Vector3 to)
		{
			var nearestDistance = -1f;

			RaycastHit hitInfo;

			Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear (to);

			Debug.DrawLine (from, to + transform.forward * -Camera.main.nearClipPlane, Color.red);
			Debug.DrawLine (from, clipPlanePoints.upperLeft);
			Debug.DrawLine (from, clipPlanePoints.lowerLeft);
			Debug.DrawLine (from, clipPlanePoints.upperRight);
			Debug.DrawLine (from, clipPlanePoints.lowerRight);

			Debug.DrawLine (clipPlanePoints.upperLeft, clipPlanePoints.upperRight);
			Debug.DrawLine (clipPlanePoints.upperRight, clipPlanePoints.lowerRight);
			Debug.DrawLine (clipPlanePoints.lowerRight, clipPlanePoints.lowerLeft);
			Debug.DrawLine (clipPlanePoints.lowerLeft, clipPlanePoints.upperLeft);

			if (Physics.Linecast (from, clipPlanePoints.upperLeft, out hitInfo) && hitInfo.collider.CompareTag ("Player") == false)
			{
				nearestDistance = hitInfo.distance;
			}

			if (Physics.Linecast (from, clipPlanePoints.lowerLeft, out hitInfo) && hitInfo.collider.CompareTag ("Player") == false)
			{
				if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
					nearestDistance = hitInfo.distance;
			}

			if (Physics.Linecast (from, clipPlanePoints.upperRight, out hitInfo) && hitInfo.collider.CompareTag ("Player") == false)
			{
				if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
					nearestDistance = hitInfo.distance;
			}

			if (Physics.Linecast (from, clipPlanePoints.lowerRight, out hitInfo) && hitInfo.collider.CompareTag ("Player") == false)
			{
				if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
					nearestDistance = hitInfo.distance;
			}

			if (Physics.Linecast (from, to + transform.forward * -Camera.main.nearClipPlane, out hitInfo) && hitInfo.collider.CompareTag ("Player") == false)
			{
				if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
					nearestDistance = hitInfo.distance;
			}

			return nearestDistance;
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
			preOccludedDistance = distance;
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