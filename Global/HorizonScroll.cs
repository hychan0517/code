using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HorizonScroll : MonoBehaviour
{
	private GameObject _slotSample;
	private RectTransform _scrollContent;
	private ScrollRect _scrollRect;
	private float _slotSize = 0;
	[SerializeField] private float _leftPadding = 0;
	[SerializeField] private float _rightPadding = 0;
	[SerializeField] private float _spacing = 0;
	private int _showCount = 0;
	private int _firstSlotIndex = 0;                //스크롤 내릴 때 위치를 변경할 슬롯의 인덱스
	private int _lastDataIndex = 0;                 //가장 마지막에 출력된 출석보상 아이템의 인덱스
	private List<ScrollSlotBase> _scrollSlotList = new List<ScrollSlotBase>();
	private List<object> _scrollDataList = new List<object>();

	public void SetContentObject(RectTransform rect)
	{
		_scrollContent = rect;
	}
	public void SetScrollObject(ScrollRect rect)
	{
		_scrollRect = rect;
		_scrollRect.onValueChanged.AddListener(OnValueChangedScroll);
	}

	public void CreateScrollSlot()
	{
		if (_slotSample && _scrollContent && _scrollRect)
		{
			var rect = _slotSample.GetComponent<RectTransform>();
			_slotSize = rect.sizeDelta.x;
			_showCount = (int)(_scrollRect.GetComponent<RectTransform>().sizeDelta.x /_slotSize) + 1;
			for(int i = 0; i < _showCount; ++i)
			{
				var initObject = Instantiate(_slotSample, _scrollContent.transform);
				_scrollSlotList.Add(initObject.GetComponent<ScrollSlotBase>());
			}
		}
		else
		{
			Debug.LogError("Cannot Find Scroll Object");
		}
	}
	private void SettingScroll()
	{
		ResetData();
		SettingScrollData();
		SettingScrollScale();
		SettingScrollPosition(0);
	}

	private void ResetData()
	{
		gameObject.SetActive(true);
		_lastDataIndex = 0;
		_firstSlotIndex = 0;
		_scrollDataList.Clear();
		for (int i = 0; i < _scrollSlotList.Count; ++i)
		{
			_scrollSlotList[i].gameObject.SetActive(false);
		}
	}
	private void SettingScrollData()
	{
		//_attendanceInfoList = ;
	}
	
	
	private void SettingScrollScale()
	{
		var screenRate = Vector2.one;
		_slotSize *= screenRate.x;
		_leftPadding *= screenRate.x;
		_rightPadding *= screenRate.x;
		_spacing *= screenRate.x;
		for (int i = 0; i < _scrollSlotList.Count; ++i)
		{
			var rect = _scrollSlotList[i].GetRectTransform();
			rect.sizeDelta = new Vector2(rect.sizeDelta.x * screenRate.x, rect.sizeDelta.y * screenRate.y);
		}
		_scrollContent.sizeDelta = new Vector2((((_slotSize)) * (_scrollDataList.Count)), _scrollContent.sizeDelta.y * screenRate.y);
	}
	private void SettingScrollPosition(int firstShowDataIndex)
	{
		firstShowDataIndex = Mathf.Clamp(firstShowDataIndex, 0, _scrollDataList.Count - _showCount);
		int maxDataCount = _scrollDataList.Count;
		for (int i = 0; i < _scrollSlotList.Count; ++i)
		{
			if(i >= _scrollDataList.Count)
			{
				break;
			}
			SettingSlot(i, i);
			SetNextSlotIndex();
		}
		RePositionScrollView(firstShowDataIndex, maxDataCount);
		RePositionScrollContent(firstShowDataIndex);
	}
	private void RePositionScrollView(int firstShowDataIndex, int maxDataCount)
	{
		float scrollPositionPer = firstShowDataIndex / (float)maxDataCount;
		float maxSize = _slotSize * maxDataCount;
		_scrollContent.anchoredPosition = new Vector2(-maxSize * scrollPositionPer, _scrollContent.anchoredPosition.y);
	}
	private void RePositionScrollContent(int firstShowDataIndex)
	{
		int firstIndex = _lastDataIndex - _scrollSlotList.Count;
		while (true)
		{
			if (firstIndex < firstShowDataIndex)
			{
				RightScroll();
				++firstIndex;
			}
			else
			{
				break;
			}
		}
	}

	private void OnValueChangedScroll(Vector2 scrollPosition)
	{
		int firstDataIndex = _lastDataIndex - _scrollSlotList.Count;
		if (_scrollContent.anchoredPosition.x < -((firstDataIndex + 1) * _slotSize))
		{
			RightScroll();
		}
		else if (_scrollContent.anchoredPosition.x > -(firstDataIndex * _slotSize))
		{
			LeftScroll();
		}
	}
	private void RightScroll()
	{
		if (_lastDataIndex < _scrollDataList.Count)
		{
			SettingSlot(_firstSlotIndex, _lastDataIndex);
			SetNextSlotIndex();
		}
	}
	private void LeftScroll()
	{
		if (_lastDataIndex > _scrollSlotList.Count)
		{
			int lastSlotIndex = GetLastSlotIndex();
			int firstDataIndex = _lastDataIndex - _scrollSlotList.Count - 1;
			SettingSlot(lastSlotIndex, firstDataIndex);
			_firstSlotIndex = lastSlotIndex;
			_lastDataIndex--;
		}
	}
	private void SettingSlot(int slotIndex, int dataIndex)
	{
		_scrollSlotList[slotIndex].SettingSlot(_scrollDataList[dataIndex]);
		_scrollSlotList[slotIndex].transform.localPosition = new Vector3(dataIndex * _slotSize,
																	_scrollSlotList[slotIndex].transform.localPosition.y,
																	_scrollSlotList[slotIndex].transform.localPosition.z);
	}
	private void SetNextSlotIndex()
	{
		_lastDataIndex++;
		if (_firstSlotIndex == _scrollSlotList.Count - 1)
		{
			_firstSlotIndex = 0;
		}
		else
		{
			_firstSlotIndex++;
		}
	}

	private int GetLastSlotIndex()
	{
		if (_firstSlotIndex == 0)
		{
			return _scrollSlotList.Count - 1;
		}
		else
		{
			return _firstSlotIndex - 1;
		}
	}

	public void Lock()
	{
		_scrollRect.horizontal = false;
	}
	public void UnLock()
	{
		_scrollRect.horizontal = true;
	}

	private void OnDestroy()
	{
		if (_scrollRect)
			_scrollRect.onValueChanged.RemoveAllListeners();
	}
}