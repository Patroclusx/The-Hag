using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroEnd : MonoBehaviour
{
    public SceneLoader sceneLoader;

    void endIntro()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            sceneLoader.LoadNextScene();
        }
    }
}
