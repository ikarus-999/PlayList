using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.IO;

public class FileItemHandler : MonoBehaviour
{
    public static FileItemHandler Instance;
    public GameObject CellPrefab;

    // private string Selecteditem;
    //string[] myFilePathList = new string[10];

    // List<string> FileList;

    DirectoryInfo MusicFolder;
    private int index = 0;
    
    public string[] MusicList;

    private GameObject[] listings;
    string myFilePath;
    AudioClip myClip;

    public AudioClip[] audioClips;
    public int songNumber = 0;

    AudioSource audioSource;

    public int currentTrack = 0;
    public Text songName;
    public Text timeText;

    int fullLength, playTime, sec, min;
    
    public bool isPaused = false;

    // UnityEvent clicking = new UnityEvent();

    string AndroidRootPath()
    {
        string[] temp = (Application.persistentDataPath.Replace("Android", "")).Split(new string[] { "//" }, System.StringSplitOptions.None);
        return (temp[0] + "/");
    }
    // Start is called before the first frame update
    void Start()
    {
        // clicking.AddListener(myAction);
        StartCoroutine(UpdateMusicLibrary());
        
    }

    void Awake()
    {
        Instance = this;
        if (Application.platform == RuntimePlatform.Android)
        {
            myFilePath = AndroidRootPath() + "Download/data_audio/";
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor)
        {
            myFilePath = Application.dataPath + "/Resources/Music";
        }
        else if (Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            myFilePath = Application.dataPath + "/Resources/Music";
        }
        else
        {
            myFilePath = Application.dataPath + "/Resources/Music";
            Debug.Log(Application.platform);
        }
        MusicFolder = new DirectoryInfo(myFilePath);
    }

    private IEnumerator UpdateMusicLibrary()
    {
        // Debug.Log("ÆÄÀÏ °¹¼ö: " + MusicFolder.GetFiles().Length);

        audioClips = new AudioClip[MusicFolder.GetFiles().Length];

        for (int i=0; i<MusicFolder.GetFiles().Length; i++)
        {
            if (MusicFolder.GetFiles()[i].Extension == ".meta"){ continue; }

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + MusicFolder.GetFiles()[i].FullName, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (Application.platform == RuntimePlatform.Android && www.result == UnityWebRequest.Result.Success)
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                }
                else if (www.result != UnityWebRequest.Result.Success)
                {
                    // myClip = DownloadHandlerAudioClip.GetContent(www);
                    Debug.Log("Errors :" + www.error);
                }
                else
                {
                    myClip = DownloadHandlerAudioClip.GetContent(www);
                    Debug.Log("Status: " + myClip.loadState + " / " + myClip.length);
                }
                songNumber++;
                GameObject obj = Instantiate(CellPrefab);
                obj.transform.SetParent(this.gameObject.transform, false);
                obj.transform.GetChild(0).GetComponent<Text>().text = MusicFolder.GetFiles()[i].Name;
                obj.transform.GetChild(1).GetComponent<Text>().text = MusicFolder.GetFiles()[i].Extension;
                

                audioClips[i] = myClip;
                audioClips[i].name = MusicFolder.GetFiles()[i].Name;
                while (audioClips[i].loadState != AudioDataLoadState.Loaded)
                {
                    yield return null;
                }
            }
        }

        // audioSource.clip = audioClips[index];
        // songName.text = audioClips[index].name;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //void myAction()
    //{
    //    Debug.Log("click" + Selecteditem);
    //}
}