using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public Camera mainCamera;

    public void startGame()
    {

        FindObjectOfType<AudioManager>().playSound("StartGame");
        sceneLoader.LoadNextScene();
    }
}
