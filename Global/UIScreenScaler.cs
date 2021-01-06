using System;
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
		VerRatio,
		HorRatio,
	}
	[SerializeField] private eUIType _uiType = eUIType.None;

	private bool _scaleFlag = false;
	private Action _scaleEndCallback;
	private void Start()
	{
		if (_scaleFlag == false)
			DoScale();
	}
	public void InitScaler(eUIType type, Action scaleEndCallback = null)
	{
		_uiType = type;
		_scaleEndCallback = scaleEndCallback;
	}
	private void DoScale()
	{
		if (_uiType == eUIType.None)
		{
			LogManager.Instance.PrintLog(LogManager.eLogType.Normal, "UIType is None");
			return;
		}
		var rate = GetRatioOfScreenResolution();
		var rect = gameObject.GetComponent<RectTransform>();
		_scaleFlag = true;
		switch (_uiType)
		{
			case eUIType.Base:
				SetScaleToRatio(GetComponent<RectTransform>());
				break;
			case eUIType.GridLayout:
				var grid = GetComponent<GridLayoutGroup>();
				if (grid == null)
					return;

				Vector2 ratio = GetRatioOfScreenResolution();
				grid.cellSize *= ratio;
				grid.spacing *= ratio;
				break;
			case eUIType.Layout:
				GameObject[] childs = gameObject.GetChildsObject();
				foreach (GameObject child in childs)
				{
					SetScaleToRatio(child.GetComponent<RectTransform>());
				}
				break;
			case eUIType.VerRatio:
				if (rect)
					rect.sizeDelta = new Vector2(rect.sizeDelta.x * rate.y, rect.sizeDelta.y * rate.y);
				break;
			case eUIType.HorRatio:
				if (rect)
					rect.sizeDelta = new Vector2(rect.sizeDelta.x * rate.x, rect.sizeDelta.y * rate.x);
				break;
		}
		_scaleEndCallback?.Invoke();
	}
	private Vector2 GetRatioOfScreenResolution()
	{
		Vector2 _rateioforScreenResolution = new Vector2();
		const float width = 1280.0f;
		const float heigh = 800.0f;
		_rateioforScreenResolution.x = 1.0f;  // width Rate
		_rateioforScreenResolution.y = 1.0f;  // heigh Rate
		GameObject canversGO = GameObject.Find("Canvas") as GameObject;
		if (canversGO != null)
		{
			_rateioforScreenResolution.x = canversGO.GetComponent<RectTransform>().sizeDelta.x / width;
			_rateioforScreenResolution.y = canversGO.GetComponent<RectTransform>().sizeDelta.y / heigh;
		}
		return _rateioforScreenResolution;
	}
	private void SetScaleToRatio(RectTransform rect)
	{
		var rate = GetRatioOfScreenResolution();
		rect.sizeDelta = new Vector2(rect.sizeDelta.x * rate.x, rect.sizeDelta.y * rate.y);
	}
}
