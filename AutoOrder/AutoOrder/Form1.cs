using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AutoOrder
{
	public struct Define
	{
		public static float EVENT_PRICE = 1000000;
	}

	public partial class Form1 : Form
	{
		public class LogManager
		{
			//코드 이름과 
			private Dictionary<string, List<string>> logDict = new Dictionary<string, List<string>>();
			private int buyCount;
			private int sellCount;
			private float lossMoney;
			private float gainMoney;

			public void AddLog(string code, string log)
			{
				if (logDict.ContainsKey(code) == false)
				{
					List<string> data = new List<string>();
					logDict.Add(code, data);
				}

				logDict[code].Add(log);

				PringLog();
			}

			public void AddBuyCount()
			{
				++buyCount;
			}

			public void AddLossMoney(float money)
			{
				++sellCount;
				lossMoney += money;
			}

			public void AddGainMoney(float money)
			{
				++sellCount;
				gainMoney += money;
			}

			private void PringLog()
			{
				if(logDict.Count > 0)
				{
					string path = string.Format("{0}/log.txt",Application.StartupPath);
					if (File.Exists(path) == false)
					{
						File.CreateText(path);
					}

					using (StreamWriter outputFile = new StreamWriter(path))
					{
						StringBuilder sb = new StringBuilder();
						string log = string.Format("BuyCount : {0} SellCount : {1} LossMoney : {2} GainMoney : {3}", buyCount, sellCount, lossMoney, gainMoney);
						outputFile.WriteLine(log);
						outputFile.WriteLine();
						outputFile.WriteLine();

						foreach (var i in logDict)
						{
							outputFile.WriteLine("--------------------------------------------");
							outputFile.WriteLine(string.Format("Code : {0}", i.Key));
							foreach (var j in i.Value)
							{
								outputFile.WriteLine();
								outputFile.WriteLine(j);
								outputFile.WriteLine();
							}
							sb.Append("--------------------------------------------");
						}
					}
				}
			}
		}
			
		public class BoughtEventData
		{
			public string eventName;
			public int processCount;
			public int firstPrice;
			public float boughtPrice;
			public int boughtCount;
			public float gainPrice;
			public float lossPrice;
			public DateTime processTime;

			public string SettingBoughtPrice(int startPrice, float nowPrice, string name)
			{
				processTime = DateTime.Now;
				eventName = name;
				boughtPrice = nowPrice;
				firstPrice = startPrice;
				processCount = 1;
				gainPrice = boughtPrice + (firstPrice * 0.02f);
				lossPrice = boughtPrice - (firstPrice * 0.02f);
				boughtCount = (int)(Define.EVENT_PRICE / boughtPrice);

				string log = string.Format("Buy -- FirstPrice : {3} NowPrice : {0} GainPrice : {1} LossPrice : {2} ", boughtPrice, gainPrice, lossPrice, firstPrice);
				Console.WriteLine();
				Console.WriteLine(log);
				Console.WriteLine();
				return log;
			}

			public bool IsSell(int nowPrice)
			{
				if ((DateTime.Now - processTime).TotalSeconds < 1f)
					return false;

				if (nowPrice >= gainPrice)
				{
					processTime = DateTime.Now;
					lossPrice = nowPrice - firstPrice * 0.01f;
					++processCount;
					gainPrice = boughtPrice + (processCount * firstPrice * 0.01f);
					//Console.WriteLine("\n");
					//string log = string.Format("UpDate Event\nName : {0}\nNowPrice : {1}\ngainPrice : {2}\nlossPrice : {3}\nProcessCount : {4}", eventName, nowPrice, gainPrice, lossPrice, processCount);
					//Console.WriteLine(log);
					//Console.WriteLine("\n");
				}

				if (nowPrice < lossPrice)
					return true;
				else
					return false;
			}
		}


		private const string MY_ACCOUNT = "5019875311";
		/// <summary> 첫  </summary>
		private const float TARGET_PRICE_RATE = 1.02f;
		//RQ
		private const string ORDER_NAME = "주문";

		//Screen
		private const string REAL_DATA_NO = "0001";
		private const string ORDER_NO = "0002";
		
		
		//조회종목. 매수하면 매수종목으로 이동. 매도 후 다시 조회종목으로 이동
		private List<string> TODAY_TARGET_LIST = new List<string>() { "004540", "049830", "014470", "041190", "314930", "039860", "224110", "007860", "289220", "048410", "027830", "039020", "020710", "019550", "021080", "297570", "302440", "025950", "092300", "080520", "307930", "054410", "007330", "043610", "000890", "100790", "309930", "023460", "003475", "026890", "310200", "124560", "109820", "037400", "032300", "001380", "066910", "054940", "225530", "064850", "025820", "317530", "047080", "205470", "263720", "053160", "031980", "020120", "027710", "027360", "075970", "007770", "014530", "323230", "053260", "007210", "094480", "024890", "153490", "222980", "014190", "009520", "300080", "003530", "950210", "086890", "000540", "084370", "265560" };


		//index별 매수 목표가격. 최초는 전날 종가 * 1.015. 매도 후 익절일경우 최고가 + 1틱?
		private Dictionary<string, float> TODAY_LIST_BUY_PRICE = new Dictionary<string, float>();
		//index별 최초 가격
		private Dictionary<string, int> TODAY_LIST_FIRST_PRICE = new Dictionary<string, int>();

		//매수종목 데이터
		private Dictionary<string, BoughtEventData> _boughtDataDict = new Dictionary<string, BoughtEventData>();

		private LogManager _logManager = new LogManager();

		private bool _processErrorFlag = false;
		private DateTime _lastProcessTime;
		private long _lastProcessTicks;
		private int _tickProcessCount;

		public Form1()
		{
			InitializeComponent();
			axKHOpenAPI2.CommConnect();
		}

		private void axKHOpenAPI2_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
		{
		}

		private void axKHOpenAPI2_OnReceiveRealData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
		{
			if(e.sRealType == "주식체결")
			{
				if (_lastProcessTicks == DateTime.Now.Ticks)
					return;

				_lastProcessTicks = DateTime.Now.Ticks;

				string code = e.sRealKey;
				int price = int.Parse(axKHOpenAPI2.GetCommRealData(code, 10).Trim().Replace("-", ""));
				
				if (_boughtDataDict.ContainsKey(code))
				{
					//매수 후 종목
					CheckSellProcessByRealData(e, code, price);
				}
				else
				{
					//매수 전 종목
					CheckBuyProcessByRealData(e, code, price);
				}
			}
		}

		/// <summary> 매수 전 종목 </summary>
		private void CheckBuyProcessByRealData(AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e, string code, int price)
		{
			//장외
			if (DateTime.Now.Hour < 9 || (DateTime.Now.Hour >= 15 && DateTime.Now.Minute >= 19))
				return;

			if (_processErrorFlag)
				return;

			if (TODAY_LIST_BUY_PRICE.ContainsKey(code))
			{
				if (price > 0)
				{
					float targetPrice = TODAY_LIST_BUY_PRICE[code];
					if (price >= targetPrice)
					{
						//매수
						ProcessBuyByRealData(e, code, TODAY_LIST_FIRST_PRICE[code], price);
						_logManager.AddBuyCount();

					}
				}
				else
				{
					string log = string.Format("Error On Find Now Price.\nCode : {0}\n", code);
					Console.WriteLine(log);
					_processErrorFlag = true;
				}
			}
			else
			{
				//targetPRice 등록
				//TODO : 시가 또는 price 파라미터값으로
				int startPrice = int.Parse(axKHOpenAPI2.GetCommRealData(code, 16).Trim().Replace("-", ""));
				if (startPrice != 0)
				{
					float targetPrice = startPrice * TARGET_PRICE_RATE;
					string log = string.Format("Init TargetPrice.\nName : {0}\nNow Price : {1}\nTarget Price : {2}", code, startPrice, targetPrice);
					Console.WriteLine("\n");
					Console.WriteLine(log);
					Console.WriteLine("\n");


					if (TODAY_LIST_BUY_PRICE.ContainsKey(code) == false)
					{
						TODAY_LIST_BUY_PRICE.Add(code, targetPrice);
					}
					else
					{
						Console.WriteLine("Error On Add Dict");
						_processErrorFlag = true;
						return;
					}
					if (TODAY_LIST_FIRST_PRICE.ContainsKey(code) == false)
					{
						TODAY_LIST_FIRST_PRICE.Add(code, startPrice);
					}
					else
					{
						Console.WriteLine("Error On Add Dict");
						_processErrorFlag = true;
						return;
					}

					CheckBuyProcessByRealData(e, code, price);
				}
				else
				{
					string log = string.Format("Error On Init TargetPrice.\nCode : {0}", code);
					Console.WriteLine(log);
					_processErrorFlag = true;
				}
			}
		}

		/// <summary> 매수 주문 </summary>
		private void ProcessBuyByRealData(AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e, string code, int firstPrice, int price)
		{
			if (_processErrorFlag)
				return;

			if (IsDelay() == false)
			{
				//매수정보 저장
				BoughtEventData temp = new BoughtEventData();
				string log = temp.SettingBoughtPrice(firstPrice, price, code);
				_logManager.AddLog(code, log);

				if (_boughtDataDict.ContainsKey(code) == false)
				{
					_boughtDataDict.Add(code, temp);
				}
				else
				{
					Console.WriteLine("Error On Add Buy Dict");
				}
				TODAY_LIST_BUY_PRICE[code] = float.MaxValue;
				//매수 주문
				//axKHOpenAPI2.SendOrder(ORDER_NAME, ORDER_NO, MY_ACCOUNT, 1, code, temp.boughtCount, 0, "03", string.Empty);
				++_tickProcessCount;
			}
		}

		/// <summary> 매수 후 종목 </summary>
		private void CheckSellProcessByRealData(AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e, string code, int price)
		{
			if (_processErrorFlag)
				return;

			if (_boughtDataDict.ContainsKey(code))
			{
				if (_boughtDataDict[code].IsSell(price) && IsDelay() == false)
				{
					if (_boughtDataDict[code].processCount > 1)
					{
						//익절이라면 최종 목표가로
						TODAY_LIST_BUY_PRICE[code] = _boughtDataDict[code].gainPrice;
					}
					else
					{
						////손절이라면 첫 목표가로
						//TODAY_LIST_BUY_PRICE[code] = _boughtDataDict[code].boughtPrice;
						//손절이라면 제거
						TODAY_LIST_BUY_PRICE.Remove(code);
						TODAY_LIST_FIRST_PRICE.Remove(code);
						axKHOpenAPI2.SetRealRemove(REAL_DATA_NO, code);
					}
					//매도 주문
					//axKHOpenAPI2.SendOrder(ORDER_NAME, ORDER_NO, MY_ACCOUNT, 2, code, _boughtDataDict[code].boughtCount, 0, "03", string.Empty);

					float gapMoney = (price - _boughtDataDict[code].boughtPrice) * _boughtDataDict[code].boughtCount;
					string priceLog = string.Format("Sell -- Bought : {1}, Now : {0}, ProcessCount : {2}, Gap : {3}, Margine : {4}", price, _boughtDataDict[code].boughtPrice, _boughtDataDict[code].processCount, price - _boughtDataDict[code].boughtPrice, gapMoney);
					if(gapMoney > 0)
					{
						_logManager.AddGainMoney(gapMoney);
					}
					else
					{
						_logManager.AddLossMoney(Math.Abs(gapMoney));
					}

					Console.WriteLine();
					Console.WriteLine(priceLog);
					Console.WriteLine();
					_logManager.AddLog(code, priceLog);


					_boughtDataDict.Remove(code);
					++_tickProcessCount;
				}
			}
			else
			{
				string log = string.Format("Error On CheckSellProcessByRealData.\nCode : {0}", code);
				Console.WriteLine(log);
				_processErrorFlag = true;
			}
		}

		/// <summary> 시세불러오기 버튼 콜백 </summary>
		private void OnStartProcess(object sender, EventArgs e)
		{
			OnProcessSearch();
		}

		private void OnProcessSearch()
		{
			_lastProcessTime = DateTime.Now;
			StringBuilder sb = new StringBuilder();
			string[] targetArr = TODAY_TARGET_LIST.ToArray();
			foreach (var i in targetArr)
			{
				sb.Append(i);
				sb.Append(";");
			}
			sb.Remove(sb.Length - 1, 1);
			string targetStr = sb.ToString();
			int result = axKHOpenAPI2.SetRealReg(REAL_DATA_NO, targetStr, "10", "0");
			Console.WriteLine(string.Format("SetRealReg : {0}", result));
		}

		private bool IsDelay()
		{
			if ((DateTime.Now - _lastProcessTime).TotalSeconds >= 1)
				_tickProcessCount = 0;

			if (_tickProcessCount > 4)
				return true;
			else
				return false;
		}
	}
}