using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitchController : MonoBehaviour
{
    public DoorController door;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided with Door Switch");
        if(other.tag == "Player")
        {
            door.OpenDoor();
        }
    }
}
