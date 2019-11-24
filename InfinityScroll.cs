using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfinityScroll : MonoBehaviour
{
	public ContentItem itemExam;
	public GameObject scrollView;
	private Rect itemSize;
	private Rect scrolViewSize;

	public int upPadding;			//위,왼쪽 여유 공간을 위한 변수
	public int leftPadding;

	private List<ContentItem> contentList = new List<ContentItem>();		//화면에 보여지는 Content Item의 리스트, Item의 크기에 따라 자동 셋팅
	private List<PlayerSkillData> itemList = new List<PlayerSkillData>();   //스크롤에 보여줄 아이템들의 리스트

	private int verCount;			//화면 크기와 아이템크기,UpPadding 대비 보여줄 수 있는 상,하 아이템의 갯수
	private int horCount;           //화면 크기와 아이템크기,LeftPadding 대비 보여줄 수 있는 좌,우 아이템의 갯수
	private int showCount;			//전체 ScrollView에 보여줄 수 있는 아이템의 갯수

	private Vector3 changePos;
	private int headContent;        //위 아래로 보여주고 있는 contentList Index
	private int tailContent;        
	private int firstItem;          //위 아래로 보여주고 있는 itemList Index
	private int lastItem;

	public void Setting()
	{
		itemList = JsonMng.Ins.playerSkillDataTable.ToList();
		itemSize = itemExam.GetComponent<RectTransform>().rect;
		scrolViewSize = scrollView.GetComponent<RectTransform>().rect;
		//스크롤뷰의 크기를 기준으로 화면에 보여줄 content의 갯수를 지정
		horCount = (int)Mathf.Floor(scrolViewSize.width / (itemSize.width + leftPadding));
		verCount = (int)Mathf.Floor(scrolViewSize.height / (itemSize.height + upPadding));
		showCount = horCount * verCount;
		//스크롤뷰 전체 크기 설정
		GetComponent<RectTransform>().sizeDelta = new Vector2(0, (itemSize.height + upPadding) * Mathf.CeilToInt(((float)itemList.Count / (float)horCount)));
		float c = 0;
		if (horCount != 0)
		{
			c = leftPadding + itemSize.width;
		}
		changePos = new Vector3(c, itemSize.height + upPadding, 0);
		GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

		//화면의 크기가 전체 아이템의 크기보다 크면 기본 스크롤 사용
		if (showCount >= itemList.Count) SetBasicScroll(itemList.Count);
		else SetInfinityScroll(itemList.Count);
		itemExam.gameObject.SetActive(false);
		SetItemList();
	}
	private void SetInfinityScroll(int itemCount)
	{
		CreateUpSpace();
		SetBasicScroll(itemCount);
	}
	private void SetBasicScroll(int itemCount)
	{
		for (int i = 0; i < verCount; ++i)
		{
			for (int j = 0; j < horCount; ++j)
			{
				GameObject o = Instantiate(itemExam.gameObject, transform);
				contentList.Add(o.GetComponent<ContentItem>());
				Vector2 pos = new Vector2(itemSize.width * j + (leftPadding * (j + 1)) + (itemSize.width / 2), -itemSize.height * i - (upPadding * (i + 1)) - (itemSize.height / 2));
				o.GetComponent<RectTransform>().localPosition = pos;
				itemCount--;
				if (itemCount == 0)
				{
					return;
				}
			}
		}
	}
	private void CreateUpSpace()
	{
		//크기에 맞는 content를 미리 생성
		for (int i = 0; i < horCount; ++i)
		{
			GameObject o = Instantiate(itemExam.gameObject, transform);
			contentList.Add(o.GetComponent<ContentItem>());
			Vector2 pos = new Vector2(itemSize.width * i + (leftPadding * (i + 1)) + (itemSize.width / 2),
									  itemSize.height / 2);
			o.GetComponent<RectTransform>().localPosition = pos;
			o.gameObject.SetActive(false);
		}

	}

	private void SetItemList()
	{
		//아이템의 이미지,텍스트 값의 변경을 위한 함수
		if (itemList.Count <= showCount)
		{
			scrollView.GetComponent<ScrollRect>().vertical = false;
			for (int i = 0; i < itemList.Count; ++i)
			{
				contentList[i].Setting(itemList[i].skillID);
			}
		}
		else
		{
			for (int i = horCount; i < contentList.Count; ++i)
			{
				contentList[i].Setting(itemList[i - horCount].skillID);
			}
		}
		headContent = 0;
		tailContent = contentList.Count - 1;
		firstItem = 0;
		lastItem = contentList.Count - 1 - horCount;
	}

	private void Update()
	{
		//매 틱마다 스크롤위치와 아이템 크기를 비교하여 보여주는 라인이 변경되었는지 체크
		float cPos = transform.localPosition.y;
		float topValue = Mathf.Ceil(cPos / (itemSize.size.y + upPadding));
		if (topValue > firstItem / horCount)
		{
			ChangeDownScroll();
		}
		else if (topValue < firstItem / horCount)
		{
			ChangeUpScroll();
		}
	}
	private void ChangeDownScroll()
	{
		Vector3 nextStartPos = contentList[tailContent].transform.localPosition;
		nextStartPos.x = contentList[headContent].transform.localPosition.x;
		for (int i = 0; i < horCount; ++i)
		{
			firstItem++;
			lastItem++;
			contentList[headContent].transform.localPosition =
				nextStartPos + new Vector3(changePos.x * i, -changePos.y, 0);
			if (lastItem < itemList.Count)
			{
				contentList[headContent].Setting(itemList[lastItem].skillID);
			}
			else
			{
				contentList[headContent].gameObject.SetActive(false);
			}
			tailContent = headContent;
			headContent++;
			if (headContent == contentList.Count) headContent = 0;
		}
	}
	private void ChangeUpScroll()
	{
		Vector3 nextStartPos = contentList[headContent].transform.localPosition;
		int saveTail = tailContent - horCount;
		if (saveTail < 0) saveTail = contentList.Count - 1;
		int saveHead = tailContent - horCount + 1;
		int savefirst = firstItem - horCount * 2;
		firstItem = savefirst;
		tailContent = saveHead;
		for (int i = 0; i < horCount; ++i)
		{
			contentList[tailContent].transform.localPosition =
				nextStartPos + new Vector3(changePos.x * i, +changePos.y, 0);
			if (firstItem < 0)
			{
				contentList[tailContent].gameObject.SetActive(false);
			}
			else
			{
				contentList[tailContent].Setting(itemList[firstItem].skillID);
			}
			tailContent++;
			firstItem++;
			lastItem--;
		}
		headContent = saveHead;
		tailContent = saveTail;
		firstItem = savefirst + horCount;
	}
}
