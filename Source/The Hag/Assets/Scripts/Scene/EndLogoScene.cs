using UnityEngine;
using UnityEngine.Video;

public class EndLogoScene : MonoBehaviour
{

    public SceneLoader sceneLoader;
    public VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.loopPointReached += endVideo;
    }

    void endVideo(VideoPlayer vp)
    {
        sceneLoader.LoadNextScene(1f);
    }
}
