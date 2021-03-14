using UnityEngine;
using UnityEngine.SceneManagement;

public class EndIntroScene : MonoBehaviour
{
    public SceneLoader sceneLoader;

    void endIntro()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            sceneLoader.LoadNextScene(4f);
        }
    }
}
