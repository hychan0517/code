using UnityEngine;

static public class ExtensionMethod
{
	/// <summary> 이름으로 오브젝트 찾기</summary>
	static public GameObject GetChildObject(this GameObject obj, string strChildName)
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
	public static GameObject GetChildObjectOptimize(this GameObject obj, params string[] strChildName)
	{
		if (obj == null)
			return null;

		int iCount = 0;
		for (int i = 0; i < obj.transform.childCount; ++i)
		{
			if (obj.transform.GetChild(i).gameObject.name == strChildName[iCount])
			{
				obj = obj.transform.GetChild(i).gameObject;
				// 자식으로 바꾸고, 다시 for 문으로 갔을때 즉시 ++i가 되기 때문에 -1로 설정해야 0부터 루프 돈다
				i = -1;

				if (++iCount >= strChildName.Length)
				{
					return obj;
				}
				else
				{
					continue;
				}
			}
		}

		return null;
	}

	/// <summary> 바로 밑의 자식들만 찾아온다</summary>
	static public GameObject[] GetChildsObject(this GameObject obj)
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
