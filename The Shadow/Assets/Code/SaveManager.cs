using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class SaveManager : MonoBehaviour
{

    public static SaveManager instance; // allows for only one SaveManager
    public SaveData activeSave;

    public bool switchedScene = false; // whether or not the new scene is due to switching scenes or starting the game.


    void Awake() {

        if(instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        Load();

        if (activeSave.autosave){
            StartCoroutine(AutoSave());
        }
    }

    void Update()
    {
        
    }

    public void NewSceneStart() {
        string dataPath = Application.persistentDataPath;
        if (System.IO.File.Exists(dataPath + "/" + activeSave.SaveName + ".savefile")) {
            if (switchedScene == false) {
                GameManager.instance.LoadingValues();
            }
        }
        Save();
    }

    
    public void Save() {
        GameManager.instance.SavingValues();
        
        string dataPath = Application.persistentDataPath;
        var Serializer = new XmlSerializer(typeof(SaveData));
        var Stream = new FileStream(dataPath + "/" + activeSave.SaveName + ".savefile", FileMode.Create);

        Serializer.Serialize(Stream, activeSave);
        Stream.Close();

        print("saved");
    }

    public void Load() {
        string dataPath = Application.persistentDataPath;
        if (System.IO.File.Exists(dataPath + "/" + activeSave.SaveName + ".savefile")) {   
            if(GameManager.instance){
                GameManager.instance.LoadingValues();
            }
            var Serializer = new XmlSerializer(typeof(SaveData));
            var Stream = new FileStream(dataPath + "/" + activeSave.SaveName + ".savefile", FileMode.Open);

            activeSave = Serializer.Deserialize(Stream) as SaveData;
            Stream.Close();

            print("loaded");
            
        } else {
            activeSave.Init(false, false);
        }

    }

    public void DeleteData(bool reset) {
        string dataPath = Application.persistentDataPath;
        
        if (System.IO.File.Exists(dataPath + "/" + activeSave.SaveName + ".savefile")) {   
            File.Delete(Application.persistentDataPath + "/" + activeSave.SaveName + ".savefile");
            print("DATA DELETED");
            activeSave.Init(reset, true);
        }
    }

    IEnumerator AutoSave() {
        yield return new WaitForSeconds(activeSave.AutoSaveTime);
        if (activeSave.autosave == true) {
            print("auto");
            Save();
            StartCoroutine(AutoSave());
        }
    }

}

[System.Serializable]
public class SaveData
{
    public string SaveName;
    public string Scene;
    public int AutoSaveTime;
    public bool autosave;


    public Vector3 PlayersPosition;


    public void Init(bool reset, bool newGame)
    {
        
    }

}