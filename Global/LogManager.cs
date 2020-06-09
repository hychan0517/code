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
	}
	#endregion
	// 출력하고 싶은 로그 타입
	private eLogType avtiveLogType = eLogType.Max;
	//private eLogType avtiveLogType = eLogType.None; //로그 전체 지우기
	private Dictionary<eLogType, string> logDict = new Dictionary<eLogType, string>();
	/// <summary>
	/// 로그 추가시 추가할 내용
	///  1.eLogType 추가
	///  2.SetLogList 추가, 색 설정
	/// </summary>
	[Flags]
	public enum eLogType
	{
		None = 0,
		Normal = 1 << 0,
		Err = 1 << 1,
		STARTEND = 1 << 2,
		DataTableErr = 1 << 3,
		Max = 4,
	}
	private void SetLogList()
	{
		//색 설정
		logDict.Add(eLogType.Normal, "#000000");
		logDict.Add(eLogType.Err, "#ff0000");
		logDict.Add(eLogType.STARTEND, "#00ff00");
		logDict.Add(eLogType.DataTableErr, "#0000ff");
	}
	public void PrintLog(eLogType logType, string str)
	{
		if (logDict.Count == 0)
		{
			SetLogList();
		}
		if ((avtiveLogType & logType) != 0)
			Debug.Log(string.Format("<color={0}> {1} </color>", logDict[logType], str));
	}
	public void PrintStartLog(string strType)
	{
		PrintLog(eLogType.STARTEND, string.Format("▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼{0} : Start▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼", strType));
	}
	public void PrintEndLog(string strType)
	{
		PrintLog(eLogType.STARTEND, string.Format("▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲{0} : End▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲", strType));
	}
	private void OnDestroy()
	{
		applicationIsQuitting = true;
	}
}
