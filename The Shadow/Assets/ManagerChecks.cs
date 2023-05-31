using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerChecks : MonoBehaviour
{
    
    void Awake()// get save/gamemanager if none
    {
        if (GameManager.instance == null || SaveManager.instance == null) {
            print("No Gamemanager / Savemanager");
            SceneManager.LoadScene("Main Menu");
        }
    }
}
