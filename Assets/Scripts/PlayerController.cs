using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed = 5;
    private bool canMove = true;
    
    private RunManager runManager;
    private Rigidbody2D rb;
    private ContactFilter2D movementFilter;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    void Start()
    {  
        runManager = GetComponent<RunManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool resetRun = Input.GetButtonDown("Reset Run"), resetAllRuns = Input.GetButtonDown("Reset All Runs");
        if(resetRun || resetAllRuns) { 
            runManager.ResetRun(resetAllRuns);
            canMove = true;
        }

        if(Input.GetButtonDown("Lock In Run")) { 
            runManager.LockInRun(); 
        }
    }

    void FixedUpdate()
    {
        if(canMove) {
            Move();
        }
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        if(movement != Vector2.zero) { runManager.StartRun(); }

        // Check for potential collisions
		int count = rb.Cast (
			movement, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
			movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
			castCollisions, // List of collisions to store the found collisions into after the Cast is finished
			speed * Time.fixedDeltaTime); // The amount to cast equal to the movement

        if(count == 0){
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0f, 0f, angle + 180f);
        }
    }


    public void setCanMove(bool b)
	{
        canMove = b;
	}
}