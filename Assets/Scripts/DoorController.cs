using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public string levelToLoad;

    private bool isOpen = false;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OpenDoor()
    {
        isOpen = true;
        spriteRenderer.enabled = false;
    }

    public void CloseDoor()
	{
        isOpen = false;
        spriteRenderer.enabled = true;
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && isOpen)
        {
            SceneManager.LoadScene(levelToLoad);
        }
    }
}
