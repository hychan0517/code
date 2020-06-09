using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LinkCreator : MonoBehaviour
{
    private StringBuilder _stringBuilder = new StringBuilder();
    private StringBuilder _stringBuilderUI = new StringBuilder();
    private List<CachingObject> _cachingObjectList = new List<CachingObject>();
    private List<ButtonObject> _buttonObjectList = new List<ButtonObject>();
    public string _className;
    public string _getterMethodName;
    private void Start()
	{
        if(_className == string.Empty)
        {
            Debug.LogError("Class Name Is Null");
            return;
        }
        else if(_getterMethodName == string.Empty)
        {
            Debug.LogError("Getter Method Name Is Null");
            return;
        }
		#region using
		_stringBuilder.Append("using System;");
        _stringBuilder.Append("\n");
        _stringBuilder.Append("using System.Collections;");
        _stringBuilder.Append("\n");
        _stringBuilder.Append("using System.Collections.Generic;");
        _stringBuilder.Append("\n");
        _stringBuilder.Append("using UnityEngine;");
        _stringBuilder.Append("\n");
        _stringBuilder.Append("using UnityEngine.UI;");
        _stringBuilder.Append("\n");
        #endregion
        _stringBuilder.Append(string.Format("public class {0} : MonoBehaviour",_className));
        _stringBuilder.Append("\n");
        _stringBuilder.Append("{");
        _stringBuilder.Append("\n");


        _stringBuilderUI.Append(string.Format("GameObject[] gameObjects = {0}(gameObject);",_getterMethodName));
        _stringBuilderUI.Append("\n");
        _stringBuilderUI.Append("foreach(GameObject gameObject_i in gameObjects)");
        _stringBuilderUI.Append("\n");
        _stringBuilderUI.Append("{");
        _stringBuilderUI.Append("\n");

        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            if (i == 0)
            {
                SettingBaseUI(gameObject.transform.GetChild(i).gameObject, "gameObject",true);
            }
            else
            {
                SettingBaseUI(gameObject.transform.GetChild(i).gameObject, "gameObject", false);
            }
        }

        _stringBuilderUI.Append("}");
        _stringBuilderUI.Append("\n");

        InitCachingObject();
        InitUIObject();
        InitButtonList();
        InitButtonMethod();
        InitDestroy();

        //마지막
        _stringBuilder.Append("}");

        Debug.Log(_stringBuilder.ToString());
    }
    private void InitCachingObject()
    {
        foreach(CachingObject cachings in _cachingObjectList)
        {
            string objectName = cachings.name;
            objectName = string.Format("private {0} _{1}{2}; \n",cachings._cachingType.ToString(), objectName[0].ToString().ToLower(), objectName.Remove(0,1));
            _stringBuilder.Append(objectName);
        }
    }
    private void InitUIObject()
    {
        _stringBuilder.Append("public void InitUIObject()");
        _stringBuilder.Append("\n");
        _stringBuilder.Append("{");
        _stringBuilder.Append("\n");
        _stringBuilder.Append(_stringBuilderUI.ToString());
        _stringBuilder.Append("InitButtonObject();");
        _stringBuilder.Append("}");
        _stringBuilder.Append("\n");
    }
    private void SettingBaseUI(GameObject initObject, string parentObjectName, bool isFirst)
    {
        var isCaching = initObject.GetComponent<CachingObject>();
        var isButton = initObject.GetComponent<ButtonObject>();
        if (isCaching || isButton || initObject.transform.childCount > 0)
        {
            if (isFirst)
            {
                _stringBuilderUI.Append(string.Format("if (string.Equals({0}_i.name, \"{1}\"))", parentObjectName, initObject.name));
            }
            else
            {
                _stringBuilderUI.Append(string.Format("else if (string.Equals({0}_i.name, \"{1}\"))", parentObjectName, initObject.name));
            }
            _stringBuilderUI.Append("\n");
            _stringBuilderUI.Append("{");
            _stringBuilderUI.Append("\n");

            if (isCaching)
            {
                _cachingObjectList.Add(isCaching);
                if (isCaching._cachingType == CachingObject.eCachingType.GameObject)
                {
                    string objectName = string.Format("_{0}{1}", initObject.name[0].ToString().ToLower(), initObject.name.Remove(0, 1));
                    _stringBuilderUI.Append(string.Format("{0} = {1}_i;", objectName, parentObjectName));
                    _stringBuilderUI.Append("\n");
                }
                if (isCaching._cachingType == CachingObject.eCachingType.Button)
                {
                    string objectName = string.Format("_{0}{1}", initObject.name[0].ToString().ToLower(), initObject.name.Remove(0, 1));
                    _stringBuilderUI.Append(string.Format("{0} = {1}_i.GetComponent<Button>();", objectName, parentObjectName));
                    _stringBuilderUI.Append("\n");
                    _buttonObjectList.Add(initObject.GetComponent<ButtonObject>());
                }
                if (isCaching._cachingType == CachingObject.eCachingType.Text)
                {
                    string objectName = string.Format("_{0}{1}", initObject.name[0].ToString().ToLower(), initObject.name.Remove(0, 1));
                    _stringBuilderUI.Append(string.Format("{0} = {1}_i.GetComponent<Text>();", objectName, parentObjectName));
                    _stringBuilderUI.Append("\n");
                }
            }
            if (initObject.transform.childCount > 0)
            {
                GameObject[] objects = GetChildsObject(initObject);
                for (int i = 0; i < objects.Length; ++i)
                {
                    if (CheckChild(objects[i]) == false)
                    {
                        continue;
                    }
                    else
                    {
                        _stringBuilderUI.Append(string.Format("GameObject[] {0}s = {1}({2}_i);", initObject.name, _getterMethodName, parentObjectName));
                        _stringBuilderUI.Append("\n");
                        _stringBuilderUI.Append(string.Format("foreach(GameObject {0}_i in {0}s)", initObject.name));
                        _stringBuilderUI.Append("\n");
                        _stringBuilderUI.Append("{");
                        _stringBuilderUI.Append("\n");
                        if (i == 0)
                        {
                            SettingBaseUI(objects[i], initObject.name, true);
                        }
                        else
                        {
                            SettingBaseUI(objects[i], initObject.name, false);
                        }
                    }
                }
            }
            _stringBuilderUI.Append("}");
            _stringBuilderUI.Append("\n");
        }
    }
    private void InitButtonList()
    {
        _stringBuilder.Append("public void InitButtonObject()");
        _stringBuilder.Append("\n");
        _stringBuilder.Append("{");
        _stringBuilder.Append("\n");
        foreach (ButtonObject buttons in _buttonObjectList)
        {
            string objectName = buttons.name;
            objectName = string.Format("_{0}{1}",objectName[0].ToString().ToLower(), objectName.Remove(0,1));
            _stringBuilder.Append(string.Format("if({0})",objectName));
            _stringBuilder.Append("\n");
            if(buttons._methodName == string.Empty)
            {
                _stringBuilder.Append(string.Format("{0}.onClick.AddListener(OnClick{1});", objectName, buttons.name));
            }
            else
            {
                _stringBuilder.Append(string.Format("{0}.onClick.AddListener({1});", objectName, buttons._methodName));
            }
            
            _stringBuilder.Append("\n");
        }
        _stringBuilder.Append("}");
        _stringBuilder.Append("\n");
    }
    private void InitButtonMethod()
    {
        foreach (ButtonObject buttons in _buttonObjectList)
        {
            if (buttons._methodName == string.Empty)
            {
                _stringBuilder.Append(string.Format("private void OnClick{0}()", buttons.name));
            }
            else
            {
                _stringBuilder.Append(string.Format("private void {0}()", buttons._methodName));
            }
            
            _stringBuilder.Append("\n");
            _stringBuilder.Append("{");
            _stringBuilder.Append("\n");
            _stringBuilder.Append("}");
            _stringBuilder.Append("\n");
        }
    }
    private void InitDestroy()
    {
        _stringBuilder.Append("private void OnDestroy()");
        _stringBuilder.Append("\n");
        _stringBuilder.Append("{");
        _stringBuilder.Append("\n");
        foreach (ButtonObject buttons in _buttonObjectList)
        {
            string objectName = buttons.name;
            objectName = string.Format("_{0}{1}", objectName[0].ToString().ToLower(), objectName.Remove(0,1));
            _stringBuilder.Append(string.Format("if({0})", objectName));
            _stringBuilder.Append("\n");
            _stringBuilder.Append(string.Format("{0}.onClick.RemoveAllListeners();", objectName));
            _stringBuilder.Append("\n");
        }
        _stringBuilder.Append("}");
        _stringBuilder.Append("\n");
    }
    /// <summary> 바로 밑의 자식들만 찾아온다</summary>
    public static GameObject[] GetChildsObject(GameObject obj)
    {
        if (obj == null)
            return null;

        GameObject[] objects = new GameObject[obj.transform.childCount];
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            objects[i] = obj.transform.GetChild(i).gameObject;
        }
        return objects;
    }

    private bool CheckChild(GameObject obj)
    {
        if (obj == null)
            return false;

        Transform[] childs = obj.transform.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < childs.Length; i++)
        {
            if (childs[i].GetComponent<CachingObject>())
                return true;
        }
        return false;
    }
}
