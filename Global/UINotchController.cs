using UnityEngine;
using GlobalDefine;

public class UINotchController : MonoBehaviour
{
	public enum eChangeTypeOfNotch
	{
		None,
		LeftPosition,
		RightPosition,
		BottomPosition,
		SideScale,
	}

	public eChangeTypeOfNotch _changeTypeOfNotch;
	public float ratio = 1;

#if UNITY_EDITOR || UNITY_IOS
	private void Start()
	{
		Scaling();
	}
	private void Scaling()
	{
		var rect = GetComponent<RectTransform>();
		RectTransform canvasRect = null;
		if (App.Ins.GetSceneState() == eSceneType.Lobby)
		{
			canvasRect = AppLobby.Ins.GetCanvas().GetComponent<RectTransform>();
		}
		else if (App.Ins.GetSceneState() == eSceneType.InGame)
		{
			canvasRect = AppInGame.Ins.GetCanvas().GetComponent<RectTransform>();
		}

		if (rect && canvasRect)
		{
			float safeWidthRatio = 1 - (Screen.safeArea.width / Screen.width);
			float safeHeightRatio = 1 - (Screen.safeArea.height / Screen.height);

			float safeWidth = canvasRect.rect.width * safeWidthRatio;
			float safeHeight = canvasRect.rect.height * safeHeightRatio;
			switch (_changeTypeOfNotch)
			{
				case eChangeTypeOfNotch.BottomPosition:
					rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + safeHeight);
					break;
				case eChangeTypeOfNotch.LeftPosition:
					rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + (safeWidth / 2 * ratio), rect.anchoredPosition.y);
					break;
				case eChangeTypeOfNotch.RightPosition:
					rect.anchoredPosition = new Vector2(rect.anchoredPosition.x - (safeWidth / 2 * ratio), rect.anchoredPosition.y);
					break;
				case eChangeTypeOfNotch.SideScale:
					rect.transform.localScale = new Vector3(1 - (safeWidthRatio * ratio), rect.transform.localScale.y, rect.transform.localScale.z);
					break;
			}
		}
	}
#endif
}
