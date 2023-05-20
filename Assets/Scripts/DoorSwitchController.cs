using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DoorSwitchController : MonoBehaviour
{
    public DoorController door;
    public Sprite pressed;
    public Sprite unpressed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            door.OpenDoor();
            GetComponent<SpriteRenderer>().sprite = pressed;
        }
    }

	private void OnTriggerExit2D(Collider2D collision)
	{
		if(collision.CompareTag("Player"))
		{
            door.CloseDoor();
            GetComponent<SpriteRenderer>().sprite = unpressed;
		}
	}
}
