using UnityEngine;

public class IntroEnd : MonoBehaviour
{
    public SceneLoader sceneLoader;

    void endIntro()
    {
        sceneLoader.LoadNextScene();
    }
}
