using UnityEngine;

public class CameraResolutionController : MonoBehaviour
{
	private void Awake()
	{
		Camera camera = GetComponent<Camera>();
		Rect cameraRect = camera.rect;
		float scaleHeight = (Screen.width / (float)Screen.height) / (16 / 10f);
		float scaleWidth = 1f / scaleHeight;
		if(scaleHeight < 1)
		{
			cameraRect.height = scaleHeight;
			cameraRect.y = (1f - scaleHeight) / 2f;
		}
		else
		{
			cameraRect.width = scaleWidth;
			cameraRect.x = (1f - scaleWidth) / 2f;
		}
		camera.rect = cameraRect;
	}
}
