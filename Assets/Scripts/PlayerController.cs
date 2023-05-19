using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed = 5;
    public int availableRuns = 3;

    private bool runStarted = false;
    private int remainingRuns;
    private Vector3 startingPosition;

    private Recorder recorder;

    void Start()
    {  
        recorder = GetComponent<Recorder>();

        startingPosition = transform.position;
        remainingRuns = availableRuns;
    }

    void Update()
    {
        bool resetRun = Input.GetButtonDown("Reset Run"), resetAllRuns = Input.GetButtonDown("Reset All Runs");
        if(resetRun || resetAllRuns) { ResetRun(resetAllRuns); }

        if(Input.GetButtonDown("Lock In Run")) { LockInRun(); }
    }

    void FixedUpdate()
    {
        Move();
        if(runStarted) 
        {
            // also add timer code here
            recorder.RecordPosition(transform.position, remainingRuns); 
        }
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0);
        if(movement != Vector3.zero) { runStarted = true; }

        transform.Translate(movement * speed * Time.deltaTime);
    }

    private void ResetRun(bool allRuns = false)
    {
        Debug.Log("Remaining runs: " + remainingRuns);
        transform.position = startingPosition;

        if(allRuns) { remainingRuns = availableRuns; }
    }

    private void LockInRun()
    {
        if(!runStarted) { return; }
        if(remainingRuns <= 0) { return; }

        ResetRun();
        remainingRuns--;
        
        if(remainingRuns == 0) 
        {
            StartCoroutine(StartMultiplePlaybacks(1, availableRuns + 1));
        }
    }

    private void StartPlayback(int run)
    {
        GameObject playerClone = Instantiate(gameObject, startingPosition, Quaternion.identity);
        playerClone.GetComponent<PlayerController>().enabled = false;

        StartCoroutine(recorder.PlaybackRun(run, playerClone, speed));
    }

    private IEnumerator StartMultiplePlaybacks(int beginAtRun, int endAtRun)
    {
        for(int i = beginAtRun; i < endAtRun; i++)
        {
            StartPlayback(i);
            yield return null;
        }
    }
}