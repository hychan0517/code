using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.IO;
using UnityEngine.UI;

public class LevelDesignMng : MonoBehaviour
{
	#region SINGLETON
	static LevelDesignMng _instance = null;

	public static LevelDesignMng Ins
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(LevelDesignMng)) as LevelDesignMng;
				if (_instance == null)
				{
					_instance = new GameObject("LevelDesignMng", typeof(LevelDesignMng)).GetComponent<LevelDesignMng>();
				}
			}

			return _instance;
		}
	}
	#endregion
	public Dropdown stageDropDwon;
	public Dropdown worldDropDown;

	public MonsterExam monsterExam;
	//ViewPort
	public GameObject gameView;
	//Stage에 표시되는 Monster들의 정보
	public List<MonsterExam> monsterExamList;
	//전체 World와 Stage Data
	public Dictionary<int, Dictionary<int, List<StageDataTable>>> stageDataTable { get; private set; }
												= new Dictionary<int, Dictionary<int, List<StageDataTable>>>();
	public Text expText;
	public Text goldText;

	//Monster, World, Stage 추가 생성을 위한 UI
	public Text monsterIndex;
	public Text monsterLevel;
	public Toggle monsterToggle;
	public Text stageText;
	public Text worldText;
	private void Awake()
	{
		LoadStageData();
	}
	private void Setting()
	{
		//전체 데이터 기준으로 드롭다운 박스와 가장 낮은 Stage표시
		worldDropDown.ClearOptions();
		stageDropDwon.ClearOptions();
		List<int> stage = new List<int>();
		//드롭박스 셋팅
		var i = stageDataTable.GetEnumerator();
		while (i.MoveNext())
		{
			worldDropDown.options.Add(new Dropdown.OptionData() { text = i.Current.Key.ToString() });
		}
		int firstWorld = GetFirstWorld();
		var j = stageDataTable[firstWorld].GetEnumerator();
		while(j.MoveNext())
		{
			stageDropDwon.options.Add(new Dropdown.OptionData() { text = j.Current.Key.ToString() });
		}
		//가장 낮은 스테이지 셋팅
		int firstStage = GetFirstStage(firstWorld);
		ShowGetInfo(firstWorld);
		ShowStageInfo(firstWorld,firstStage);
		worldDropDown.value = 0;
		worldDropDown.captionText.text = firstWorld.ToString();
		stageDropDwon.value = 0;
		stageDropDwon.captionText.text = firstStage.ToString();
	}
	private void UpgaradeStageOption(int worldNumber)
	{
		//Stage 추가,제거 새로고침
		stageDropDwon.ClearOptions();
		var j = stageDataTable[worldNumber].GetEnumerator();
		while (j.MoveNext())
		{
			stageDropDwon.options.Add(new Dropdown.OptionData() { text = j.Current.Key.ToString() });
		}
	}


	private void ShowGetInfo(int stage)
	{
		////추가될 수 
		//var i = stageDataTable.GetEnumerator();
		//int count = 0;
		//while (i.MoveNext())
		//{
		//	if (i.Current.Key > stage) break;
		//	var temp = i.Current.Value;
		//	for(int j = 0; j < temp.Count; ++j)
		//	{
		//		count += temp[j].stageLevel;
		//	}
		//}
		//expText.text = string.Format("EXP : {0}", count.ToString());
		//goldText.text = string.Format("GOLD : {0}", count.ToString());
	}
	//가장 낮은 스테이지 불러오기
	private int GetFirstWorld()
	{
		var i = stageDataTable.GetEnumerator();
		int index = int.MaxValue;
		while (i.MoveNext())
		{
			if(i.Current.Key < index)
			{
				index = i.Current.Key;
			}
		}
		return index;
	}
	private int GetFirstStage(int WorldNumber)
	{
		var i = stageDataTable[WorldNumber].GetEnumerator();
		int index = int.MaxValue;
		while (i.MoveNext())
		{
			if (i.Current.Key < index)
			{
				index = i.Current.Key;
			}
		}
		return index;
	}

	public void ShowStageInfo(int worldNumber, int stageNumber)
	{
		//현재 월드,스테이지데이터로 화면상에 몬스터 표시
		RemoveMonsterExamList();
		List<StageDataTable> stageList = stageDataTable[worldNumber][stageNumber];
		for(int i = 0; i < stageList.Count; ++i)
		{
			float xPos = stageList[i].enemyPosX;
			float yPos = stageList[i].enemyPosY;
			int monsterIndex = stageList[i].enemyIndex;
			int monsterLevel = stageList[i].enemyLevel;
			GameObject o = Instantiate(monsterExam.gameObject, gameView.transform);
			MonsterExam e = o.GetComponent<MonsterExam>();
			e.Setting(monsterLevel.ToString(), monsterIndex.ToString(),xPos,yPos,stageList[i].boss);
			monsterExamList.Add(e);
		}
		ShowGetInfo(stageNumber);
	}
	public void ShowStageInfoToWorld(int worldNumber)
	{
		//world드롭박스 호출함수 월드 변경시 해당 월드 스테이지 드롭박스 갱신, 가장낮은 스테이지 표시
		RemoveMonsterExamList();
		List<StageDataTable> stageList = stageDataTable[worldNumber][GetFirstStage(worldNumber)];
		for (int i = 0; i < stageList.Count; ++i)
		{
			float xPos = stageList[i].enemyPosX;
			float yPos = stageList[i].enemyPosY;
			int monsterIndex = stageList[i].enemyIndex;
			int monsterLevel = stageList[i].enemyLevel;
			GameObject o = Instantiate(monsterExam.gameObject, gameView.transform);
			MonsterExam e = o.GetComponent<MonsterExam>();
			e.Setting(monsterLevel.ToString(), monsterIndex.ToString(), xPos, yPos, stageList[i].boss);
			monsterExamList.Add(e);
		}
		UpgaradeStageOption(worldNumber);
		stageDropDwon.value = 0;
		int firstStage = GetFirstStage(worldNumber);
		stageDropDwon.captionText.text = firstStage.ToString();
	}
	public void ShowStageInfo(int stageNumber)
	{
		RemoveMonsterExamList();
		List<StageDataTable> stageList = stageDataTable[int.Parse(worldDropDown.captionText.text)][stageNumber];
		for (int i = 0; i < stageList.Count; ++i)
		{
			float xPos = stageList[i].enemyPosX;
			float yPos = stageList[i].enemyPosY;
			int monsterIndex = stageList[i].enemyIndex;
			int monsterLevel = stageList[i].enemyLevel;
			GameObject o = Instantiate(monsterExam.gameObject, gameView.transform);
			MonsterExam e = o.GetComponent<MonsterExam>();
			e.Setting(monsterLevel.ToString(), monsterIndex.ToString(), xPos, yPos, stageList[i].boss);
			monsterExamList.Add(e);
		}
		ShowGetInfo(stageNumber);
		UpgaradeStageOption(int.Parse(worldDropDown.captionText.text));
		stageDropDwon.captionText.text = stageNumber.ToString();
	}
	public void SaveStage()
	{
		//수정한 스테이지 데이터 저장, 스테이지UI와 해당 스테이지 총 크기 비율로 계산한 위치,인덱스 등 저장
		List<StageDataTable> tempList = new List<StageDataTable>();
		for (int i = 0; i < monsterExamList.Count; ++i)
		{
			if (monsterExamList[i] == null) continue;
			StageDataTable temp = new StageDataTable();
			temp.worldLevel = int.Parse(worldDropDown.captionText.text);
			temp.stageLevel = int.Parse(stageDropDwon.captionText.text);
			temp.enemyIndex = int.Parse(monsterExamList[i].indexText.text);
			temp.enemyLevel = int.Parse(monsterExamList[i].levelText.text);
			temp.boss = monsterExamList[i].boss;
			Vector2 pos = monsterExamList[i].GetCoord();
			temp.enemyPosX = pos.x;
			temp.enemyPosY = pos.y;
			tempList.Add(temp);
		}
		stageDataTable[int.Parse(worldDropDown.captionText.text)][int.Parse(stageDropDwon.captionText.text)] = tempList;
		ShowGetInfo(int.Parse(stageDropDwon.captionText.text));
	}
	private void RemoveMonsterExamList()
	{
		//스테이지 또는 월드 갱신시 호출 화면에 있는 몬스터리스트 지우기
		for(int i = 0; i < monsterExamList.Count; ++i)
		{
			if (monsterExamList[i] == null) continue;
			Destroy(monsterExamList[i].gameObject);
		}
		monsterExamList.Clear();
	}

	public void RemoveStage()
	{
		stageDataTable[int.Parse(worldDropDown.captionText.text)].Remove(int.Parse(stageDropDwon.captionText.text));
		Sort();
		Setting();
	}
	public void RemoveWorld()
	{
		stageDataTable.Remove(int.Parse(worldDropDown.captionText.text));
		Sort();
		Setting();
	}
	public void CreateStage()
	{
		if (stageDataTable[int.Parse(worldText.text)].ContainsKey(int.Parse(stageText.text)))
		{
			Debug.LogError("이미 존재하는 Stage");
		}
		else
		{
			stageDataTable[int.Parse(worldText.text)].Add(int.Parse(stageText.text), new List<StageDataTable>());
			Sort();
			Setting();
		}
		stageText.text = "";
	}
	public void CreateMonster()
	{
		float xPos = 0;
		float yPos = 0;
		GameObject o = Instantiate(monsterExam.gameObject, gameView.transform);
		MonsterExam e = o.GetComponent<MonsterExam>();
		int boss;
		if (monsterToggle.isOn) boss = 1;
		else boss = 0;
		e.Setting(monsterLevel.text, monsterIndex.text, xPos, yPos, boss);
		monsterExamList.Add(e);
		monsterLevel.text = "";
		monsterIndex.text = "";
	}
	public void Sort()
	{
		//Stage 별로 다시 저장
		Dictionary<int, Dictionary<int, List<StageDataTable>>> temp = new Dictionary<int, Dictionary<int, List<StageDataTable>>>();

		while (stageDataTable.Count != 0)
		{
			int cIndex = int.MaxValue;
			var e = stageDataTable.GetEnumerator();
			while (e.MoveNext())
			{
				if (cIndex > e.Current.Key)
				{
					cIndex = e.Current.Key;
				}
			}
			temp.Add(cIndex, stageDataTable[cIndex]);
			stageDataTable.Remove(cIndex);
		}
		stageDataTable = temp;
		var i = stageDataTable.GetEnumerator();
		Dictionary<int, Dictionary<int, List<StageDataTable>>> tempAll = new Dictionary<int, Dictionary<int, List<StageDataTable>>>();
		while (i.MoveNext())
		{
			var sort = stageDataTable[i.Current.Key];
			Dictionary<int, List<StageDataTable>> tempValue = new Dictionary<int, List<StageDataTable>>();
			while (sort.Count != 0)
			{
				int cIndex = int.MaxValue;
				var e = i.Current.Value.GetEnumerator();
				while (e.MoveNext())
				{
					if (cIndex > e.Current.Key)
					{
						cIndex = e.Current.Key;
					}
				}
				tempValue.Add(cIndex, sort[cIndex]);
				sort.Remove(cIndex);
			}
			tempAll.Add(i.Current.Key, tempValue);
		}
		stageDataTable = tempAll;
	}
	/// <summary>
	/// 로드,수정,저장을 위한 테이블 불러오기
	/// </summary>
	private void LoadStageData()
	{
		StartCoroutine(StartLoad("StageDataTable", stageDataTable));
	}
	private IEnumerator StartLoad(string fileName, Dictionary<int, Dictionary<int,List<StageDataTable>>> table)
	{
		string path = string.Format("{0}/LitJson/{1}.json", Application.streamingAssetsPath, fileName);
		WWW www = new WWW(path);
		yield return www;
		string jsonString = www.text;
		JsonData jsonData = JsonMapper.ToObject(jsonString);
		for (int i = 0; i < jsonData.Count; ++i)
		{
			StageDataTable save = JsonMapper.ToObject<StageDataTable>(jsonData[i].ToJson());
			if (table.ContainsKey(save.GetTableID()))
			{
				if (table[save.GetTableID()].ContainsKey(save.GetStageID()))
				{
					table[save.GetTableID()][save.GetStageID()].Add(save);
				}
				else
				{
					List<StageDataTable> temp = new List<StageDataTable>();
					temp.Add(save);
					table[save.GetTableID()].Add(save.GetStageID(), temp);
				}
			}
			else
			{
				Dictionary<int, List<StageDataTable>> tempDic = new Dictionary<int, List<StageDataTable>>();
				List<StageDataTable> temp = new List<StageDataTable>();
				temp.Add(save);
				tempDic.Add(save.GetStageID(), temp);
				table.Add(save.GetTableID(), tempDic);
			}
		}

		Setting();
	}
	public void SaveAll()
	{
		string path = string.Format("{0}/LitJson/{1}.json", Application.streamingAssetsPath, "StageDataTable");

		List<StageDataTable> temp = new List<StageDataTable>();
		var i = stageDataTable.GetEnumerator();
		while (i.MoveNext())
		{
			var j = i.Current.Value.GetEnumerator();
			while(j.MoveNext())
			{
				if (j.Current.Value.Count == 0)
				{
					Debug.LogError("비어있는 스테이지가 있습니다.");
					return;
				}
				for (int k = 0; k < j.Current.Value.Count; ++k)
				{
					temp.Add(j.Current.Value[k]);
				}
				
			}
		}
		JsonData data = JsonMapper.ToJson(temp);
		File.WriteAllText(path, data.ToString());
	}
}
