using UnityEngine;

public class Ladder : MonoBehaviour
{
    PlayerMovement playerMovement;
    bool canClimb;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (canClimb)
        {
            float facingDotProduct = calcFacingDotProduct();
            
            if(facingDotProduct > 0.6f)
            {
                playerMovement.isClimbing = true;
            }
            else
            {
                playerMovement.isClimbing = false;
            }
        }
    }

    float calcFacingDotProduct()
    {
        Vector3 ladderVector = -gameObject.transform.right;
        Vector3 cameraVector = Camera.main.transform.right;

        return Vector3.Dot(cameraVector, ladderVector);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            canClimb = false;
            playerMovement.isClimbing = false;
        }
    }
}
