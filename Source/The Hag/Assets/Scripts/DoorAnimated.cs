using UnityEngine;

public class DoorAnimated : MonoBehaviour
{
    public Animator animator;

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        animator.SetBool("Open", true);
    }
    public void CloseDoor()
    {
        animator.SetBool("Open", false);
    }
}
