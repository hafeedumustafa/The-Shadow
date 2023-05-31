using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    

    public static GameManager instance;
    public GameObject Player;


    void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

    }



    //saving/loading data values
    public void LoadingValues() {


        print("LOADED VALUES");
    }
    public void SavingValues() {
        SaveManager.instance.activeSave.PlayersPosition = Player.transform.position;
        SaveManager.instance.activeSave.Scene = SceneManager.GetActiveScene().name;
    }

    public void Death()
    {

    }

}
