using UnityEngine;

namespace RollRoti.Simple_TPSystem
{
	public static class Helper 
	{
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