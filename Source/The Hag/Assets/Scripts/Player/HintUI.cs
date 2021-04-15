using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintUI : MonoBehaviour
{
    public static HintUI instance;
    public Animator animator;
    private TMP_Text tmpText;

    void Awake()
    {
        //Singleton instance
        if (instance != null)
            Debug.LogWarning("More than one instance of Inventory found!");
        instance = this;

        tmpText = GetComponentInChildren<TMP_Text>();
    }

    public void displayHintMessage(string newHintText)
    {
        bool canDisplay = true;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("HintAnim") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f)
            canDisplay = false;

        if (canDisplay)
        {
            tmpText.text = newHintText;
            animator.SetTrigger("ShowHint");
        }
    }
}
