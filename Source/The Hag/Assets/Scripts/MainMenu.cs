using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public Camera menuCamera;
    public Canvas menuCanvas;

    public void startGame()
    {
        menuCanvas.enabled = false;
        StartCoroutine(ModifyFov(8f));
        FindObjectOfType<AudioManager>().playSound("StartGame");
        sceneLoader.LoadNextScene();
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
