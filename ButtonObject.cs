using UnityEngine;

[RequireComponent(typeof(CachingObject))]
public class ButtonObject : MonoBehaviour
{
	public string _methodName;
	private void Awake()
	{
		GetComponent<CachingObject>()._cachingType = CachingObject.eCachingType.Button;
	}
}
