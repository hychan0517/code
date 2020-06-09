using UnityEngine;

public class CachingObject : MonoBehaviour
{
	public enum eCachingType
	{
		GameObject,
		Image,
		Text,
		Button,
	}
	public eCachingType _cachingType;
}
