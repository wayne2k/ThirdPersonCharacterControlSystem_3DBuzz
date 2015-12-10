using UnityEngine;

namespace RollRoti.Advanced_TPSystem
{
	public static class Helper 
	{
		public struct ClipPlanePoints 
		{
			public Vector3 upperLeft;
			public Vector3 upperRight;
			public Vector3 lowerLeft;
			public Vector3 lowerRight;
		}

		public static ClipPlanePoints ClipPlaneAtNear (Vector3 pos)
		{
			var clipPlanePoints = new ClipPlanePoints ();

			if (Camera.main == null)
				return clipPlanePoints;

			var camT = Camera.main.transform;
			var halfFOV = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;
			var aspect = Camera.main.aspect;
			var distance = Camera.main.nearClipPlane;
			var height = distance * Mathf.Tan(halfFOV);
			var width = height * aspect;

			clipPlanePoints.lowerRight = pos + camT.right * width;
			clipPlanePoints.lowerRight -= camT.up * height;
			clipPlanePoints.lowerRight += camT.forward * distance;

			clipPlanePoints.lowerLeft = pos - camT.right * width;
			clipPlanePoints.lowerLeft -= camT.up * height;
			clipPlanePoints.lowerLeft += camT.forward * distance;

			clipPlanePoints.upperRight = pos + camT.right * width;
			clipPlanePoints.upperRight += camT.up * height;
			clipPlanePoints.upperRight += camT.forward * distance;

			clipPlanePoints.upperLeft = pos - camT.right * width;
			clipPlanePoints.upperLeft += camT.up * height;
			clipPlanePoints.upperLeft += camT.forward * distance;

			return clipPlanePoints;
		}

		public static float ClampAngle (float angle, float min, float max)
		{
			do 
			{
				if (angle < -360f)
					angle += 360f;
				if (angle > 360f)
					angle -= 360f;
			}
			while (angle < -360f || angle > 360f);

			return Mathf.Clamp (angle, min, max);
		}
	}
}