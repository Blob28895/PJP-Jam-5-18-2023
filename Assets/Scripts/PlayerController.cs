using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed = 5;
    private bool canMove = true;
    
    private RunManager runManager;

    void Start()
    {  
        runManager = GetComponent<RunManager>();    
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

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0);
        if(movement != Vector3.zero) { runManager.StartRun(); }

        transform.Translate(movement * speed * Time.deltaTime);
    }


    public void setCanMove(bool b)
	{
        canMove = b;
	}
}