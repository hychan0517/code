using UnityEngine;

public static class ExtensionMethod
{
	/// <summary> 이름으로 오브젝트 찾기</summary>
	public static GameObject GetChildObject(this GameObject obj, string strChildName)
	{
		if (obj == null)
			return null;

		Transform[] childs = obj.transform.GetComponentsInChildren<Transform>(true);

		for (int i = 0; i < childs.Length; i++)
		{
			if (childs[i].gameObject.name == strChildName)
				return childs[i].gameObject;
		}
		return null;
	}

	/// <summary> 바로 밑의 자식들만 찾아온다</summary>
	public static GameObject[] GetChildsObject(this GameObject obj)
	{
		if (obj == null)
			return null;

		GameObject[] objects = new GameObject[obj.transform.childCount];
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			objects[i] = obj.transform.GetChild(i).gameObject;
		}
		return objects;
	}
}
