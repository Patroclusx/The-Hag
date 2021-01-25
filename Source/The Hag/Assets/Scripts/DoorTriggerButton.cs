using UnityEngine;

public class DoorTriggerButton : MonoBehaviour
{
    [SerializeField] private DoorAnimated door;
    public Collider coll;
    bool open;
    void Start()
    {
        coll = GetComponent<Collider>();
        open = false;
    }
    // Update is called once per frame
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 1.1f))
            {
                if (open)
                {
                    door.CloseDoor();
                    open = false;
                }
                else
                {
                    door.OpenDoor();
                    open = true;
                }
            }

            /*   if (open)
               {
                   if (Input.GetKeyDown(KeyCode.E))
                   {
                       door.CloseDoor();
                       open = false;
                   }
               }
               else
               {
                   if (Input.GetKeyDown(KeyCode.E))
                   {
                       door.OpenDoor();
                       open = true;
                   }
               }*/
        }
    }
}
