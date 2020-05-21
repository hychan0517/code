using UnityEngine;
using System.Collections.Generic;
using System;
public class LogManager : MonoBehaviour
{
    #region SigleTon
    private static LogManager _instance = null;
    private static bool applicationIsQuitting = false;
    public static LogManager Instance
    {
        get
        {
            if (applicationIsQuitting)
                return null;

            if (_instance == null)
            {
                _instance = (LogManager)FindObjectOfType(typeof(LogManager));
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "LogManager";
                    _instance = container.AddComponent(typeof(LogManager)) as LogManager;
                }
            }
            return _instance;
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SetLogList();
    }
    #endregion
    private eLogType avtiveLogType = eLogType.SendHTTP | eLogType.ReceiveHTTP | eLogType.ErrorHTTP;
    private Dictionary<eLogType, string> logList = new Dictionary<eLogType, string>();
    /// <summary>
    /// 로그 추가시 추가할 내용
    ///  1.eLogType 추가
    ///  2.SetLogList 추가, 색 설정
    /// </summary>
    [Flags]
    public enum eLogType
    {
        None = 0 << 0,
        SendHTTP = 1 << 0,
        ReceiveHTTP = 1 << 1,
        ErrorHTTP = 1 << 2,
    }
    private void SetLogList()
    {
        logList.Add(eLogType.SendHTTP, "#00ff00");
        logList.Add(eLogType.ReceiveHTTP, "#0000ff");
        logList.Add(eLogType.ErrorHTTP, "#ff0000");
    }
    public void PrintLog(eLogType logType,string str)
    {
        if ((avtiveLogType & logType) != 0)
            Debug.Log(string.Format("<color={0}> {1} </color>",logList[logType],str));
    }
}
