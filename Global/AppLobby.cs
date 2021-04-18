using UnityEngine;

public class AppLobby : MonoBehaviour
{
	#region SINGLETON
	static AppLobby _instance = null;

	public static AppLobby Ins
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(AppLobby)) as AppLobby;
				if (_instance == null)
				{
					_instance = new GameObject("AppLobby", typeof(AppLobby)).GetComponent<AppLobby>();
				}
			}
			return _instance;
		}
	}
	#endregion

	private Canvas _mainCanvas;
	private LobbyUI _lobbyUI;
	public Canvas GetCanvas()
	{
		return _mainCanvas;
	}
}
