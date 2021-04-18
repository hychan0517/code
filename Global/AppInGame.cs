using System;
using UnityEngine;

public class AppInGame : MonoBehaviour
{
	#region SINGLETON
	static AppInGame _instance = null;

	public static AppInGame Ins
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(AppInGame)) as AppInGame;
				if (_instance == null)
				{
					_instance = new GameObject("AppInGame", typeof(AppInGame)).GetComponent<AppInGame>();
				}
			}
			return _instance;
		}
	}
	#endregion

	private Canvas _mainCanvas;
	private InGameUI _inGameUI;
	public Canvas GetCanvas()
	{
		return _mainCanvas;
	}
}
