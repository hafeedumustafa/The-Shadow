using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuScript : MonoBehaviour
{

    public List<GameObject> levels = new List<GameObject>();
    public List<GameObject> levelObjects = new List<GameObject>();
    public List<Vector2> levelGravity1 = new List<Vector2>();
    public List<Vector2> levelGravity2 = new List<Vector2>();
    public List<int> levelHues = new List<int>(); // 0 to 360
    public List<Color> bgColours = new List<Color>();
    public GameObject playButton;
    public GameObject menuPause;
    public GameObject menuComplete;
    public GameObject nextLevel;
    public GameObject levelCompleteText;
    public GameObject player;
    public Camera cam;
    [HideInInspector] public int enabledLevel=0; // 0 means no level enabled
    [SerializeField] float transitionTime;

    public void PressLevel(int levelNum) // colourbg, levelObjects
    {
            

        // find level button
        GameObject level = levels[levelNum-1];
        // scale button up
        RectTransform rt = level.GetComponent<RectTransform>();
        rt.localScale *= 1.1f;

        if(levelNum != enabledLevel) {

            // enable play button & set play button to colour


            //change hue of play to correct colour, scope deletes temp pcH etc.
            /*{
                Color playButtonColour = playButton.GetComponent<Unity.VectorGraphics.SVGImage>().color;
                
                
                float pbcH; float pbcS; float pbcV;
                Color.RGBToHSV(playButtonColour, out pbcH, out pbcS, out pbcV);
                Color finalPlayButtonColour = Color.HSVToRGB(levelHues[levelNum-1]/360f, pbcS, pbcV);
                
                if(playButton.activeSelf) {
                    StartCoroutine(ColourTransition(playButton, playButtonColour,  finalPlayButtonColour, 0, false, false));
                } else {
                    playButton.SetActive(true);
                    
                    StartCoroutine(ColourTransition(playButton, playButtonColour,  finalPlayButtonColour + Color.black, 0, false, false));

                }
            
            
            } */
            
            if(!playButton.activeSelf) {
                playButton.SetActive(true);
                if(playButton.GetComponent<Unity.VectorGraphics.SVGImage>().color.a == 0)
                    StartCoroutine(ColourTransition(playButton, playButton.GetComponent<Unity.VectorGraphics.SVGImage>().color,  playButton.GetComponent<Unity.VectorGraphics.SVGImage>().color + Color.black, 0, false, false));
        
                    
            }
            
            
            // change bg & level buttons colour
            
            foreach(GameObject lev in levels)
            {
                StartCoroutine(ColourTransition(lev, lev.GetComponent<Unity.VectorGraphics.SVGImage>().color, Color.HSVToRGB(levelHues[levelNum-1]/360f, 1, 1), 0, false, false));
            }

            StartCoroutine(BGColourTransition(cam.backgroundColor, bgColours[levelNum-1], 0));

            // enable new level objects and disable old level objects

            if(enabledLevel != 0){
                levelObjects[enabledLevel-1].SetActive(false);
            }
            levelObjects[levelNum-1].SetActive(true);

            // set player position


        
            enabledLevel = levelNum;
        
        }
        


    }
    

    //for images
    IEnumerator ColourTransition(GameObject image, Color startColor, Color endColor, float t, bool disableAtEnd, bool enableAtBeginning)
    {
        if(enableAtBeginning && t==0)
            image.SetActive(true);

        try {
            image.GetComponent<Unity.VectorGraphics.SVGImage>().color = Color.Lerp(startColor, endColor, t);
        } catch{
            image.GetComponent<TMP_Text>().color = Color.Lerp(startColor, endColor, t);
        }

        yield return new WaitForFixedUpdate();

        if(t<1)
        {
            t+=transitionTime/50f;
            StartCoroutine(ColourTransition(image, startColor, endColor, t, disableAtEnd, enableAtBeginning));

        }
        else if(disableAtEnd)
            image.SetActive(false);

    }

    //for bg
    IEnumerator BGColourTransition(Color startColor, Color endColor, float t)
    {

        cam.backgroundColor = Color.Lerp(startColor, endColor, t);

        yield return new WaitForFixedUpdate();

        if(t<1)
        {

            t+=transitionTime/50f;
            StartCoroutine(BGColourTransition(startColor, endColor, t));
        }


    }

    public void UnpressLevel(int levelNum)
    {
        // find level button
        GameObject level = levels[levelNum-1];
        // scale button down
        RectTransform rt = level.GetComponent<RectTransform>();
        rt.localScale /= 1.1f;
    }

    public void StartLevel()
    {

        // disable UI elements
        foreach(GameObject level in levels)
        {
            StartCoroutine(ColourTransition(level, level.GetComponent<Unity.VectorGraphics.SVGImage>().color, level.GetComponent<Unity.VectorGraphics.SVGImage>().color - Color.black, 0, true, false));
        }
        
        StartCoroutine(ColourTransition(playButton, playButton.GetComponent<Unity.VectorGraphics.SVGImage>().color,  playButton.GetComponent<Unity.VectorGraphics.SVGImage>().color - Color.black, 0, true, false));
        
        // apply gravity directions & enable player
        player.SetActive(true);
        player.GetComponent<PlayerScript>().gravimode1 = levelGravity1[enabledLevel-1];
        player.GetComponent<PlayerScript>().gravimode2 = levelGravity2[enabledLevel-1];
        player.GetComponent<PlayerScript>().changeGravityDirection(player.GetComponent<PlayerScript>().gravimode1); 

        // enable pause menu button
        StartCoroutine(ColourTransition(menuPause, menuPause.GetComponent<Unity.VectorGraphics.SVGImage>().color, menuPause.GetComponent<Unity.VectorGraphics.SVGImage>().color + Color.black, 0, false, true));
        
        if(enabledLevel == 1)
        {
            Transform canvas = levelObjects[0].transform.Find("Canvas");
            for(int i=0; i < 3; i++)
            {
                canvas.GetChild(i).position += new Vector3(0f, 8f,0f);
            }
        }


    }

    public void LevelSelect()
    {
        // disables all other UI
        menuComplete.SetActive(false);
        menuPause.SetActive(false);
        levelCompleteText.SetActive(false);
        nextLevel.SetActive(false);

        if(menuPause.GetComponent<Unity.VectorGraphics.SVGImage>().color.a == 1)
            menuPause.GetComponent<Unity.VectorGraphics.SVGImage>().color -= Color.black;
        if(nextLevel.GetComponent<Unity.VectorGraphics.SVGImage>().color.a == 1)
        {
            nextLevel.GetComponent<Unity.VectorGraphics.SVGImage>().color -= Color.black;
            menuComplete.GetComponent<Unity.VectorGraphics.SVGImage>().color -= Color.black;
            levelCompleteText.GetComponent<TMP_Text>().color -= Color.black;
        }

        // disables player movement + player
        player.GetComponent<PlayerScript>().rb.velocity = new Vector2(0,0);
        player.transform.position = new Vector3(0,0,0);
        player.GetComponent<PlayerScript>().gravimode1 = new Vector2(0,0);
        player.GetComponent<PlayerScript>().gravimode2 = new Vector2(0,0);
        player.GetComponent<PlayerScript>().changeGravityDirection(player.GetComponent<PlayerScript>().gravimode1);
        player.SetActive(false);

        // disables current level objects
        levelObjects[enabledLevel-1].SetActive(false);

        // enables menu UI


        foreach(GameObject level in levels)
        {
            level.GetComponent<Unity.VectorGraphics.SVGImage>().color += Color.black;
            level.SetActive(true);
        }

        enabledLevel = 0;

    }

    public void LevelComplete()
    {

        // disable pause menu button
        
        menuPause.GetComponent<Unity.VectorGraphics.SVGImage>().color -= Color.black;
        menuPause.SetActive(false);
        
        // disables player movement + player
        player.SetActive(false);

        // enable finish level UI
        StartCoroutine(ColourTransition(menuComplete, menuComplete.GetComponent<Unity.VectorGraphics.SVGImage>().color, menuComplete.GetComponent<Unity.VectorGraphics.SVGImage>().color + Color.black, 0, false, true));
        StartCoroutine(ColourTransition(nextLevel, nextLevel.GetComponent<Unity.VectorGraphics.SVGImage>().color, nextLevel.GetComponent<Unity.VectorGraphics.SVGImage>().color + Color.black, 0, false, true));
        StartCoroutine(ColourTransition(levelCompleteText, levelCompleteText.GetComponent<TMP_Text>().color, levelCompleteText.GetComponent<TMP_Text>().color + Color.black, 0, false, true));




    }

    public void NextLevel()
    {
        // disable UI elements
        StartCoroutine(ColourTransition(menuComplete, menuComplete.GetComponent<Unity.VectorGraphics.SVGImage>().color, menuComplete.GetComponent<Unity.VectorGraphics.SVGImage>().color - Color.black, 0, true, false));
        StartCoroutine(ColourTransition(nextLevel, nextLevel.GetComponent<Unity.VectorGraphics.SVGImage>().color, nextLevel.GetComponent<Unity.VectorGraphics.SVGImage>().color - Color.black, 0, true, false));
        StartCoroutine(ColourTransition(levelCompleteText, levelCompleteText.GetComponent<TMP_Text>().color, levelCompleteText.GetComponent<TMP_Text>().color - Color.black, 0, true, false));
        
        // disables current level
        levelObjects[enabledLevel-1].SetActive(false);

        // enables next level
        enabledLevel += 1;
        levelObjects[enabledLevel-1].SetActive(true);

        // apply gravity directions & enable player

        player.SetActive(true);
        player.transform.position = new Vector3(0,0,0);
        player.GetComponent<PlayerScript>().gravimode1 = levelGravity1[enabledLevel-1];
        player.GetComponent<PlayerScript>().gravimode2 = levelGravity2[enabledLevel-1];
        player.GetComponent<PlayerScript>().changeGravityDirection(player.GetComponent<PlayerScript>().gravimode1); 

        // enable pause menu button
        
        StartCoroutine(ColourTransition(menuPause, menuPause.GetComponent<Unity.VectorGraphics.SVGImage>().color, menuPause.GetComponent<Unity.VectorGraphics.SVGImage>().color + Color.black, 0, false, true));
        
        


    }
}
