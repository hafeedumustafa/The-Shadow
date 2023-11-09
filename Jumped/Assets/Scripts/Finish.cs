using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    
    
    [SerializeField] GameObject inline;
    [SerializeField] MenuScript menuScript;
    [SerializeField] GameObject body;
    
    



    // Update is called once per frame
    void Update()
    {
        inline.transform.eulerAngles += new Vector3(0,0,0.5f);
        body.transform.eulerAngles += new Vector3(0,0,0.5f);


    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // go to next level (UI)
        if(collider.gameObject.tag == "Player")
        {
            menuScript.LevelComplete();
            collider.GetComponent<PlayerScript>().finishLevelAnim = false;
            if(menuScript.enabledLevel == 1)
            {
                Transform canvas = menuScript.levelObjects[0].transform.Find("Canvas");
                for(int i=0; i < 3; i++)
                {
                    canvas.GetChild(i).position -= new Vector3(0f, 8f,0f);
                }
            }
        }

    }
}
