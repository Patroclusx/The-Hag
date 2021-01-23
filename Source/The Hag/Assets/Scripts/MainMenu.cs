using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public AudioManager audioManager;
    public Camera menuCamera;
    public Canvas menuCanvas;

    public void startGame()
    {
        menuCanvas.enabled = false;
        StartCoroutine(ModifyFov(8f));
        audioManager.playSound("Sound_Startgame", false);
        sceneLoader.LoadNextScene();
    }
    public void openSettings()
    {

    }

    public void exitGame()
    {
        Debug.Log("Bazsy Buzsy!");
        Application.Quit();
    }

    IEnumerator ModifyFov(float fovSpeed)
    {
         float defaultFov = menuCamera.fieldOfView;;
         while (true)
         {
             menuCamera.fieldOfView -= defaultFov * Time.deltaTime / fovSpeed;

             yield return null;
         }
     }
}
