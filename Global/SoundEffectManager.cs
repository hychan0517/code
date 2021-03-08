using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    // instance
    private static SoundEffectManager _instance = null;
    private List<AudioSource> _effectTable = new List<AudioSource>();
    private List<AudioSource> _soundTable = new List<AudioSource>();
    private List<AudioSource> _clickSoundList = new List<AudioSource>();

    private int _nowClickSoundIndex;
    private float _effectVolume = 1.0f;
    private float _soundVolume = 1.0f;

    private const float DEFAULT_SOUND_VOLUM = 0.46f;
    private const float DEFAULT_EFFECT_VOLUM = 0.46f;
    private const string SOUND_PATH = "Sound/Environment/{0}";
    private const string EFFECT_PATH = "Sound/Effect/{0}";
    public enum eEffectType
    {
        None,
    }
    public enum eSoundType
    {
        None,
    }
    private int ClickSoundIndex
    {
        get
        {
            if(_nowClickSoundIndex == _clickSoundList.Count)
            {
                _nowClickSoundIndex = 0;
            }
            return _nowClickSoundIndex;
        }
    }
    private const int MAX_CLICK_SOUND = 5;
    //===============================================================================================
    public static SoundEffectManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = (SoundEffectManager)FindObjectOfType(typeof(SoundEffectManager));
            if (!_instance)
            {
                GameObject container = new GameObject();
                container.name = "SoundEffectManagerContainer";
                _instance = container.AddComponent(typeof(SoundEffectManager)) as SoundEffectManager;
            }
            DontDestroyOnLoad(_instance.gameObject);
        }

        return _instance;
    }
    private void Awake()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsManager.PREFS_SOUND_VOLUME))
        {
            _soundVolume = PlayerPrefs.GetFloat(PlayerPrefsManager.PREFS_SOUND_VOLUME);
        }
        else
        {
            _soundVolume = DEFAULT_SOUND_VOLUM;
        }

        if (PlayerPrefs.HasKey(PlayerPrefsManager.PREFS_EFFECT_VOLUME))
        {
            _effectVolume = PlayerPrefs.GetFloat(PlayerPrefsManager.PREFS_EFFECT_VOLUME);
        }
        else
        {
            _effectVolume = DEFAULT_EFFECT_VOLUM;
        }
    }
    public void ChangeVolume(float effectValue, float soundValue)
    {
        bool changeFlag = false;
        if(effectValue != _effectVolume)
        {
            _effectVolume = effectValue;
            PlayerPrefs.SetFloat(PlayerPrefsManager.PREFS_EFFECT_VOLUME, effectValue);
            foreach(var i in _effectTable)
            {
                i.volume = effectValue;
            }
            foreach (var i in _clickSoundList)
            {
                i.volume = effectValue;
            }
            changeFlag = true;
        }
        if(soundValue != _soundVolume)
        {
            _soundVolume = soundValue;
            PlayerPrefs.SetFloat(PlayerPrefsManager.PREFS_SOUND_VOLUME, soundValue);
            foreach (var i in _soundTable)
            {
                i.volume = soundValue;
            }
            changeFlag = true;
        }
        if(changeFlag)
        {
            PlayerPrefs.Save();
        }

    }
    void Start()
    {
        RegisterAudioSource();
        RegisterClickAudioSource();
    }

    private void RegisterClickAudioSource()
    {
        for(int i = 0; i < MAX_CLICK_SOUND; ++i)
        {
            _clickSoundList.Add(Instantiate(Resources.Load<GameObject>("Prefabs/Common/ClickSound"),transform).GetComponent<AudioSource>());
            _clickSoundList[i].volume = _effectVolume;
        }
    }

    private void RegisterAudioSource()
    {
        //AddSound("None", true);
        //AddEffect("None", true);
    }
    private void AddEffect(string name,bool isLoop)
    {
        AudioSource addAudioSource = gameObject.AddComponent<AudioSource>();
        try
        {
            addAudioSource.clip = Resources.Load<AudioClip>(string.Format(EFFECT_PATH, name));
            addAudioSource.volume = _effectVolume;
            addAudioSource.loop = isLoop;
            _effectTable.Add(addAudioSource);
        }
        catch(Exception e)
        {
            _effectTable.Add(new AudioSource());
            LogManager.Instance.PrintLog(LogManager.eLogType.Error, e.Message);
            LogManager.Instance.PrintLog(LogManager.eLogType.Error, string.Format("Init Audio Err Sound Name : {0}",name));
        }
    }
    private void AddSound(string name, bool isLoop)
    {
        AudioSource addAudioSource = gameObject.AddComponent<AudioSource>();
        try
        {
            addAudioSource.clip = Resources.Load<AudioClip>(string.Format(SOUND_PATH, name));
            addAudioSource.volume = _soundVolume;
            addAudioSource.loop = isLoop;
            _soundTable.Add(addAudioSource);
        }
        catch (Exception e)
        {
            _soundTable.Add(new AudioSource());
            LogManager.Instance.PrintLog(LogManager.eLogType.Error, e.Message);
            LogManager.Instance.PrintLog(LogManager.eLogType.Error, string.Format("Init Audio Err Sound Name : {0}", name));
        }
    }
    public void PlayEffect(eEffectType soundType)
    {
        if (_effectVolume <= 0)
            return;
        if (_effectTable.Count > (int)soundType && _effectTable[(int)soundType].clip)
        {
            if(_effectTable[(int)soundType].isPlaying && _effectTable[(int)soundType].clip.length >= 1.0f)
            {
                GameObject audio = new GameObject("EffectAudio");
                var source = audio.AddComponent<AudioSource>();
                source.clip = Resources.Load(string.Format(EFFECT_PATH, _effectTable[(int)soundType].clip.name)) as AudioClip;
                source.volume = _effectVolume;
                source.loop = _effectTable[(int)soundType].loop;
                source.Play();
            }
            else
                _effectTable[(int)soundType].Play();
        }
    }
    public void StopEffect(eEffectType soundType)
    {
        if (_effectTable[(int)soundType])
        {
            _effectTable[(int)soundType].Stop();
        }
    }
    public void PlaySound(eSoundType soundType)
    {
        if (_soundVolume <= 0)
            return;
        if (_soundTable.Count > (int)soundType && _soundTable[(int)soundType].clip)
        {
            _soundTable[(int)soundType].Play();
        }
    }
    public void StopSound(eSoundType soundType)
    {
        int soundIndex = (int)soundType;
        if (_soundTable.Count > soundIndex && _soundTable[soundIndex].isPlaying)
        {
            _soundTable[(int)soundType].Stop();
        }
    }
    public void StopInGameSound()
    {

    }
    public void PlayClick()
    {
        if(_clickSoundList[ClickSoundIndex] && _clickSoundList[ClickSoundIndex].volume != 0)
            _clickSoundList[ClickSoundIndex].Play();
    }

    /// <summary> 특정 이유(특정 상황에서 사운드가 안나와야 하거나 중복 사운드 방지 등)로 잠시 볼륨을 꺼놓을 때 사용 </summary>
    public void SetOffEffect(eEffectType type)
    {
        if (_effectTable[(int)type])
            _effectTable[(int)type].volume = 0;
    }
    
    public float GetEffectVolume()
    {
        return _effectVolume;
    }
    public float GetSoundVolume()
    {
        return _soundVolume;
    }
}
