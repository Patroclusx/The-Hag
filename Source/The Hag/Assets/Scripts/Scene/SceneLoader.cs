using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public AudioManager audioManager;

    bool isSkipped = false;

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Cursor.visible = false;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (Input.GetButtonDown("Jump") && !isSkipped)
            {
                LoadNextScene(4f);
                isSkipped = true;
            }
        }
        else if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            Cursor.visible = true;
        }
    }

    public void LoadNextScene(float transitionLenghtInSeconds)
    {
        if(audioManager != null)
            audioManager.fadeOutAllAudio(2.5f);
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1, transitionLenghtInSeconds));
    }

    IEnumerator LoadLevel(int index, float transitionLenghtInSeconds)
    {
        transition.SetTrigger("EndFade");

        yield return new WaitForSeconds(transitionLenghtInSeconds);

        SceneManager.LoadScene(index);
    }
}
