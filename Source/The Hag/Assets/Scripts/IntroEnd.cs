using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroEnd : MonoBehaviour
{
    public SceneLoader sceneLoader;

    void endIntro()
    {
        sceneLoader.LoadNextScene();
    }
}
