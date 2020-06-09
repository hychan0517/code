using GlobalDefine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class UnityUtility : MonoBehaviour
{
    public string CreateDataFolder(string dirName)
    {
        string path = Path.Combine(Application.persistentDataPath, dirName);
        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);
        return path;
    }

    public void DeleteAllFile(string DirFullPath)
    {
        string[] fileList = Directory.GetFiles(DirFullPath);
        for (int i = 0; i < fileList.Length; i++)
        {
            string str = fileList[i];
            string filePath = Path.Combine(DirFullPath, str);
            FileInfo fi = new FileInfo(filePath);
            try
            {
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error Delete File : {0}", e));
                break;
            }
        }
    }
    public void DeleteFile(string filePath)
    {
        FileInfo fi = new FileInfo(filePath);
        try
        {
            if (fi.Exists)
            {
                fi.Delete();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("Error Delete File : {0}", e));
        }
    }

    public IEnumerable<string> GetFiles(string dirPath)
    {
        if (string.IsNullOrWhiteSpace(dirPath))
            return null;
        try
        {
            return Directory.GetFiles(dirPath).Select(file => Path.GetFileName(file));
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("Error FileSave : {0}", e));
        }
        return null;
    }

    public string[] GetFilesFullPath(string dirPath)
    {
        if (string.IsNullOrWhiteSpace(dirPath))
            return null;
        try
        {
            return Directory.GetFiles(dirPath);
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("Error FileSave : {0}", e));
        }
        return null;
    }

    public void FileWrite(string saveFilePath, byte[] bytes)
    {
        if (bytes.Length <= 0)
        {
            LogManager.Instance.PrintLog(LogManager.eLogType.Normal, string.Format("byte == null"));
            return;
        }
        try
        {
            FileStream fs = new FileStream(saveFilePath, FileMode.Create, FileAccess.ReadWrite);
            fs.Lock(0, bytes.Length);
            fs.Write(bytes, 0, bytes.Length);
            fs.Unlock(0, bytes.Length);
            fs.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("Error FileSave : {0}", e));
        }
    }


    public IEnumerator FileRead(string fileFullPath, Action<byte[]> endCallback)
    {
        if (string.IsNullOrWhiteSpace(fileFullPath))
        {
            Debug.LogError(string.Format("FileRead => fileFullPath == null"));
            endCallback?.Invoke(null);
            yield break;
        }

        if (IsAndroid())
        {
            byte[] bytes = null;
            UnityWebRequest www = null;
            bool isError = false;
            string strError = string.Empty;

            try
            {
                string localPath = string.Format("file://{0}", fileFullPath);
                www = UnityWebRequest.Get(localPath);
                UnityWebRequestAsyncOperation request = www.SendWebRequest();
                float _timeOut = 0;
                while (!request.isDone || !www.isDone)
                {
                    yield return Define.WAIT_FOR_SECONS_POINT_ONE;
                    if (_timeOut > 10)
                    {
                        isError = true;
                        strError = "FileRead timeOut!";
                        break;
                    }
                    _timeOut += Time.deltaTime;
                };

                if (www == null)
                {
                    isError = true;
                    strError = "FileRead www is null";
                }
                else if (www.isNetworkError || www.isHttpError)
                {
                    isError = true;
                    strError = www.error;
                }
                else
                {
                    bytes = www.downloadHandler.data;
                }
            }
            finally
            {
                www.Dispose();
            }


            if (isError == true)
            {
                string str = string.Format("****************FileRead Error,  ErrorMsg : {0}****************", strError);
                Debug.LogError(str);
                endCallback?.Invoke(null);
            }
            else
            {
                if (bytes != null && bytes.Length > 0)
                {
                    endCallback?.Invoke(bytes);
                }
                else
                {
                    Debug.LogError("****************File is null****************");
                    endCallback?.Invoke(null);
                    yield break;
                }
            }
        }
        else
        {
            byte[] bytes = File.ReadAllBytes(fileFullPath);
            float _timeOut = 0;
            while (bytes == null)
            {
                yield return Define.WAIT_FOR_SECONS_POINT_ONE;
                if (_timeOut > 10)
                    yield break;
                _timeOut += Time.deltaTime;
            }

            if (bytes != null)
            {
                if (bytes.Length > 0)
                {
                    endCallback?.Invoke(bytes);
                }
                else
                {
                    Debug.LogError("****************File is null****************");
                    endCallback?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError("****************File is null****************");
                endCallback?.Invoke(null);
            }
        }
    }
    public bool IsAndroid()
    {
        if (Application.platform == RuntimePlatform.Android &&
            Application.platform != RuntimePlatform.OSXEditor &&
            Application.platform != RuntimePlatform.WindowsEditor)
            return true;
        return false;
    }

    public T ParserJsonToObject<T>(string strJson)
    {
        return JsonUtility.FromJson<T>(strJson);
    }
    /// <summary> 자식 오브젝트 전부 Destroy</summary>
    public void DestroyChildObject(GameObject parent)
    {
        GameObject[] childs = parent.GetChildsObject();
        foreach (GameObject i in childs)
        {
            Destroy(i);
        }
    }
}
