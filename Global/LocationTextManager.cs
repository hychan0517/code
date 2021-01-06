using GlobalDefine;
using System.Collections.Generic;
using UnityEngine;

public class LocationTextManager : MonoBehaviour
{
    private static LocationTextManager _instance = null;
    Dictionary<string, string> _lobbyTaxtTable = new Dictionary<string, string>();
    private static bool applicationIsQuitting = false;

    public static LocationTextManager Ins
    {
        get
        {
            if (applicationIsQuitting)
                return null;

            if (_instance == null)
            {
                _instance = (LocationTextManager)FindObjectOfType(typeof(LocationTextManager));
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "LocationTextManager";
                    _instance = container.AddComponent(typeof(LocationTextManager)) as LocationTextManager;
                }
            }
            return _instance;
        }

    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        RunOnStart();
        ReadLocationTextCSV();
    }

    // 종료시 OnDestroy() 혹은 OnDisable() 에서 LocalTextManager.Instance 가 호출될시
    // 오류가 되므로 종료시 LocalTextManager.Instance 값을 null 로 바꿔주는 작업
    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        Application.quitting += () => applicationIsQuitting = true;
    }

    private void ReadLocationTextCSV()
    {
        List<Dictionary<string, string>> lobbytext = FileIO.CSVRead("Data/LocationText");

        if (lobbytext.Count == 0)
            return;

        _lobbyTaxtTable.Clear();

        for (int i = 0; i < lobbytext.Count; i++)
        {
            string s1 = lobbytext[i]["index"];
            string s2 = lobbytext[i]["string"];
            _lobbyTaxtTable.Add(lobbytext[i]["index"], lobbytext[i]["string"]);

        }

    }

    public string GetUIText(string index)
    {
        if (_lobbyTaxtTable.ContainsKey(index))
            return _lobbyTaxtTable[index].Replace("\\n","\n");
        return "-";

    }
}