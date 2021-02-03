using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSender : MonoBehaviour
{
    public GameObject player;
    ObjectInteraction objectInteraction;

    void Awake()
    {
        objectInteraction = player.GetComponentInChildren<ObjectInteraction>();
    }

    void OnCollisionEnter(Collision collision)
    {
        objectInteraction.OnCollisionEnterCustom(collision, gameObject);
    }
}
