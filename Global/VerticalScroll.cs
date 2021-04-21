using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalScroll : MonoBehaviour
{
    private ScrollRect _scrollView;
    private RectTransform _content;
    private Vector2 _slotSize;
    private Dictionary<int, string> _dataList = new Dictionary<int, string>();
    private List<GameObject> _slotList = new List<GameObject>();
    private GameObject _slotSample;
    private float _scrollViewHeight;
    private float _spacing = 0;
    private int _showCount;  // 화면에 보여질 슬롯
    private int _slotIndex = 0;
    private int _dataIndex = 0;

    private void SettingScroll()
    {
        CreateRankSlot();
        SettingScrollView();
        SettingSlotBase();
    }

    private void CreateRankSlot()
    {
        if (_slotList.Count == 0 && _slotSample)
        {
            // 슬롯 크기
            Rect realSlotSize = _slotSample.GetComponent<RectTransform>().rect;
            Vector2 rate = BicycleUtil.GetRatioOfScreenResolution();

            // 비율 적용된 슬롯 사이즈
            _slotSize = new Vector2(realSlotSize.width * rate.x, (realSlotSize.height + _spacing) * rate.y);

            // 스크롤 뷰 사이즈
            Rect scrollViewHeight = transform.GetComponent<RectTransform>().rect;
            _scrollViewHeight = scrollViewHeight.height;

            // 화면에 보여질 슬롯 수
            var showCount = Convert.ToInt32(Math.Truncate(_scrollViewHeight / _slotSize.y));
            _showCount = showCount + 4;

            // 기본 슬롯 생성,배치
            for (int i = 0; i < _showCount; i++)
            {
                var slot = Instantiate(_slotSample, _content.transform);
                var slotPos = slot.GetComponent<RectTransform>().anchoredPosition;
                slot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -_slotSize.y * i);
                slot.AddComponent<UIScreenScaler>().InitScaler(UIScreenScaler.eUIType.Base);
                _slotList.Add(slot);
            }
        }
    }

    private void SettingScrollView()
    {
        foreach (var i in _slotList)
        {
            i.gameObject.SetActive(false);
        }
        _scrollView.velocity = Vector2.zero;
        _content.anchoredPosition = Vector2.zero;
        _content.sizeDelta = new Vector2(_content.rect.width, _dataList.Count * _slotSize.y);
        _slotIndex = 0;
        _dataIndex = 0;
    }

    private void SettingSlotBase()
    {
        for (int i = 0; i < _dataList.Count; i++)
        {
            if (i >= _showCount)
                break;
            SettingSlot(i, i);
        }
    }

    private void MoveScrollByDataIndex(int dataIndex)
    {
        float targetPosY = dataIndex * _slotSize.y - _scrollViewHeight;
        _content.anchoredPosition = new Vector2(_content.anchoredPosition.y, targetPosY);
    }

    private void OnValueChangedScroll()
    {
        if (_content.anchoredPosition.y > (_dataIndex + 1) * _slotSize.y)
        {
            DownScroll();
        }
        else if (_content.anchoredPosition.y < _dataIndex * _slotSize.x)
        {
            UpScroll();
        }
    }

    private void DownScroll()
    {
        if (_dataIndex + _showCount < _dataList.Count)
        {
            SettingSlot(_slotIndex, _dataIndex + _showCount);
            SetNextSlotIndex();
            OnValueChangedScroll();
        }
    }
    private void UpScroll()
    {
        if (_dataIndex > 0)
        {
            _slotIndex = GetLastSlotIndex();
            SettingSlot(_slotIndex, --_dataIndex);
            OnValueChangedScroll();
        }
    }
    private void SetNextSlotIndex()
    {
        //최상단 슬롯의 인덱스 갱신
        _dataIndex++;
        if (_slotIndex == _slotList.Count - 1)
        {
            _slotIndex = 0;
        }
        else
        {
            ++_slotIndex;
        }
    }
    private int GetLastSlotIndex()
    {
        if (_slotIndex == 0)
        {
            return _slotList.Count - 1;
        }
        else
        {
            return _slotIndex - 1;
        }
    }
    private void SettingSlot(int slotIndex, int dataIndex)
    {
        _slotList[slotIndex].transform.localPosition = new Vector3(_slotList[slotIndex].transform.localPosition.x,
                                                                    -dataIndex * _slotSize.y,
                                                                    _slotList[slotIndex].transform.localPosition.z);
    }

    private void OnDestroy()
    {
        if (_scrollView)
            _scrollView.onValueChanged.RemoveAllListeners();
    }
}
