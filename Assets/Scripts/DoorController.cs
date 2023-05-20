using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{

    SpriteRenderer spriteRenderer;
    Collider2D boxCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void OpenDoor()
    {
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;
    }

    public void CloseDoor()
    {
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;

    }


}
