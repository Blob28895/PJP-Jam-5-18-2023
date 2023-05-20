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

	private void Update()
	{
        overlap = Physics2D.OverlapCircle(transform.position, GetComponent<CircleCollider2D>().radius).gameObject;
        /*if(overlap != null)
		{
            Debug.Log(overlap.name);
		}*/
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
	}
}
