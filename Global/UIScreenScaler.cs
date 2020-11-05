using UnityEngine;
using UnityEngine.UI;

public class UIScreenScaler : MonoBehaviour
{
	public enum eUIType
	{
		None,
		Base,
		GridLayout,
		Layout,
	}
	[SerializeField] private eUIType _uiType = eUIType.None;

	private bool _scaleFlag = false;
	private bool _startFlag = false;
	private void Start()
	{
		_startFlag = true;
		DoScale();
	}
	public void InitScaler(eUIType type)
	{
		_uiType = type;
		if(_startFlag == true && _scaleFlag == false)
			DoScale();
	}
	private void DoScale()
	{
		if (_uiType == eUIType.None)
		{
			LogManager.Instance.PrintLog(LogManager.eLogType.Normal, "UIType is None");
			return;
		}

		_scaleFlag = true;
		switch (_uiType)
		{
			case eUIType.Base:
				BicycleUtil.SetScaleToRatio(GetComponent<RectTransform>());
				break;
			case eUIType.GridLayout:
				var grid = GetComponent<GridLayoutGroup>();
				if (grid == null)
					return;

				Vector2 ratio = BicycleUtil.GetRatioOfScreenResolution();
				grid.cellSize *= ratio;
				break;
			case eUIType.Layout:
				GameObject[] childs = BicycleUtil.GetChildsObject(gameObject);
				foreach (GameObject child in childs)
				{
					BicycleUtil.SetScaleToRatio(child.GetComponent<RectTransform>());
				}
				break;
		}

	}
}
