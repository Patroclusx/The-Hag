using System.Collections;
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
        audioManager.playSound2D("Sound_StartGame", false, 0f);
        sceneLoader.LoadNextScene(4f);
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
