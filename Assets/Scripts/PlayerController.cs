using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed = 5;
    public int availableRuns = 3;

    private int remainingRuns;
    private Vector3 startingPosition;

    void Start()
    {
        startingPosition = transform.position;
        remainingRuns = availableRuns;
    }

    void Update()
    {
        bool resetRun = Input.GetButtonDown("Reset Run"), resetAllRuns = Input.GetButtonDown("Reset All Runs");
        if(resetRun || resetAllRuns) { ResetRun(resetAllRuns); }

        if(Input.GetButtonDown("Lock In Run")) { remainingRuns--; }
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(movement * speed * Time.deltaTime);
    }

    private void ResetRun(bool allRuns = false)
    {
        transform.position = startingPosition;
        if(allRuns)
        {
            remainingRuns = availableRuns;
        }
    }
}
