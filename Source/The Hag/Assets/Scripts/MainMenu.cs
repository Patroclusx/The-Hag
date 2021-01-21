using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public AudioSource musicSource;

    public void startGame()
    {
        musicSource.Stop();
        sceneLoader.LoadNextScene();
    }
}
