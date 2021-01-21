using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;

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
       StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int index)
    {
        transition.SetTrigger("EndFade");

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(index);
    }
}
