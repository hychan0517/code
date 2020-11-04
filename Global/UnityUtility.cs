using UnityEngine;

public class UnityUtility : MonoBehaviour
{
    /// <summary> 자식 오브젝트 전부 Destroy</summary>
    static public void DestroyChildObject(GameObject parent)
    {
        GameObject[] childs = parent.GetChildsObject();
        foreach (GameObject i in childs)
        {
            Destroy(i);
        }
    }
}
