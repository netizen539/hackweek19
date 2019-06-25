using UnityEngine;

public class LookTowardsMouse_Debug_Mono : MonoBehaviour
{
	void Update()
	{
		Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
		Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

		float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
		transform.rotation = Quaternion.Euler(new Vector3(0f, -angle, 0f));
	}

	float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
	{
		return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
	}

}
