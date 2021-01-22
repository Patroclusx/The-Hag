using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public AudioSource musicSource;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Submit") || Input.GetButton("Jump"))
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                LoadNextScene();
            }
        }
    }

    public void LoadNextScene()
    {
        StartCoroutine(AudioFade.FadeOut(musicSource, 2.5f));
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int index)
    {
        transition.SetTrigger("EndFade");

        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene(index);
    }
}
