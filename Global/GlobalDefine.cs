using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace GlobalDefine
{
	static public class Define
	{
		public const int RANDOM_POOL_COUNT = 1024;
        public static WaitForEndOfFrame WAIT_FOR_END_OF_FRAME = new WaitForEndOfFrame();
        public static WaitForSeconds WAIT_FOR_SECONDS_ONE = new WaitForSeconds(1.0f);
        public static WaitForSeconds WAIT_FOR_SECONDS_TWO = new WaitForSeconds(2.0f);
        public static WaitForSeconds WAIT_FOR_SECONDS_THREE = new WaitForSeconds(3.0f);
        public static WaitForSeconds WAIT_FOR_SECONDS_FIVE = new WaitForSeconds(5.0f);
        public static WaitForSeconds WAIT_FOR_SECONDS_TEN = new WaitForSeconds(10.0f);
        public static WaitForSeconds WAIT_FOR_SECONS_POINT_ONE = new WaitForSeconds(0.1f);
        public static WaitForSeconds WAIT_FOR_SECONS_POINT_FIVE = new WaitForSeconds(0.5f);
	}
	static public class Rand // 만분율 기준 0~9999까지 저장
	{
		private static int _index = 0;
		private static int[] _randomArr = new int[Define.RANDOM_POOL_COUNT];

		static Rand()
		{
			for (int i = 0; i < Define.RANDOM_POOL_COUNT; ++i)
			{
				_randomArr[i] = UnityEngine.Random.Range(0, 10000);
			}
		}

		private static int nIndex
		{
			get
			{
				int nTemp = _index++;

				if (_index >= Define.RANDOM_POOL_COUNT) { _index = 0; }

				return nTemp;
			}
		}

		public static int Random() { return _randomArr[nIndex]; }

		public static bool Percent(float a_nPercent) { return _randomArr[nIndex] <= (a_nPercent * 100); }
		public static bool Permile(float a_nPermile) { return _randomArr[nIndex] <= (a_nPermile * 10); }
		public static bool Permilad(int a_nPermilad) { return _randomArr[nIndex] <= a_nPermilad; }
		public static int Range(int a_nStart, int a_nEnd)
		{
			if (a_nStart > a_nEnd)
			{
				int nTemp = a_nStart;
				a_nStart = a_nEnd;
				a_nEnd = nTemp;
			}

			return (Random() % (a_nEnd - a_nStart)) + a_nStart;
		}
	}

	static public class AESEncrypt
	{
		private static SHA256Managed sha256Managed = new SHA256Managed();
		private static RijndaelManaged aes = new RijndaelManaged();
		private static readonly string privateKey = "R[=n#m!*h@~";

		static AESEncrypt()
		{
			aes.KeySize = 256;
			aes.BlockSize = 128;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;
		}

		// 암호화
		public static byte[] AESEncrypt256(string strEncryptData)
		{
			byte[] encryptData = Encoding.UTF8.GetBytes(strEncryptData);
			var salt = sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(privateKey.Length.ToString()));
			//var PBKDF2Key = new Rfc2898DeriveBytes(privateKey, salt, 65535);    //반복 65535
			var PBKDF2Key = new Rfc2898DeriveBytes(privateKey, salt, 5);    //반복 5
			var secretKey = PBKDF2Key.GetBytes(aes.KeySize / 8);
			var iv = PBKDF2Key.GetBytes(aes.BlockSize / 8);
			byte[] xBuff = null;
			using (var ms = new MemoryStream())
			{
				using (var cs = new CryptoStream(ms, aes.CreateEncryptor(secretKey, iv), CryptoStreamMode.Write))
				{
					cs.Write(encryptData, 0, encryptData.Length);
				}
				xBuff = ms.ToArray();
			}
			return xBuff;
		}

		// 복호화
		public static string AESDecrypt256(byte[] decryptData)
		{
			var salt = sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(privateKey.Length.ToString()));
			//var PBKDF2Key = new Rfc2898DeriveBytes(privateKey, salt, 65535);    //반복 65535
			var PBKDF2Key = new Rfc2898DeriveBytes(privateKey, salt, 5);    //반복 5
			var secretKey = PBKDF2Key.GetBytes(aes.KeySize / 8);
			var iv = PBKDF2Key.GetBytes(aes.BlockSize / 8);
			byte[] xBuff = null;
			using (var ms = new MemoryStream())
			{
				using (var cs = new CryptoStream(ms, aes.CreateDecryptor(secretKey, iv), CryptoStreamMode.Write))
				{
					cs.Write(decryptData, 0, decryptData.Length);
				}
				xBuff = ms.ToArray();
			}
			//return xBuff;
			return Encoding.UTF8.GetString(xBuff);
		}
	}
	static public class FileIO
	{
		public static List<Dictionary<string, string>> CSVRead(string strFileName)
		{
			List<Dictionary<string, string>> listData = new List<Dictionary<string, string>>();

			TextAsset resourceText = Resources.Load<TextAsset>(strFileName);
			if (resourceText == null)
			{
				return listData;
			}

			// 한 라인 씩 저장
			string[] strLines = resourceText.text.Split(new[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.None);
			if (strLines.Length <= 1)
				return listData;

			// 키 값 정보
			string[] strKeyTable = strLines[0].Split(',');

			for (int i = 1; i < strLines.Length; ++i)
			{
				if (strLines[i] == "" || strLines[i] == " ")
					continue;

				string[] strElementTable = strLines[i].Split(',');

				Dictionary<string, string> element = new Dictionary<string, string>();

				for (int k = 0; k < strElementTable.Length; ++k)
				{
					element.Add(strKeyTable[k], strElementTable[k]);
				}

				listData.Add(element);
			}

			return listData;
		}
	}
}