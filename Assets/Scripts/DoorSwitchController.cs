using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DoorSwitchController : MonoBehaviour
{
    public DoorController door;
    public Sprite pressed;
    public Sprite unpressed;

    private GameObject overlap;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Player") || collision.CompareTag("Clone"))
		{
            door.OpenDoor();
            GetComponent<SpriteRenderer>().sprite = pressed;
        }
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
        if (collision.CompareTag("Player") || collision.CompareTag("Clone"))
        {
            door.CloseDoor();
            GetComponent<SpriteRenderer>().sprite = unpressed;
        }
    }
	/*private void Update()
	{
        overlap = Physics2D.OverlapCircle(transform.position, GetComponent<CircleCollider2D>().radius).gameObject;

        if (overlap.CompareTag("Player") || overlap.CompareTag("Clone"))
		{
            door.OpenDoor();
            GetComponent<SpriteRenderer>().sprite = pressed;
        }
        else
		{
            door.CloseDoor();
            GetComponent<SpriteRenderer>().sprite = unpressed;
        }
	}*/
}
