using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed = 5;
    public int availableRuns = 3;
    public CountdownTimer countdownTimer;

    private bool canMove = true;
    private bool runStarted = false;
    private bool isPlaybackRunning = false;
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
        if(canMove){
            Move();
        }

        if(runStarted) {
            if(countdownTimer.timerEnded) { ResetRun(); }
            recorder.RecordPosition(transform.position, remainingRuns); 
        }

        if(runStarted && !isPlaybackRunning && remainingRuns != availableRuns) { 
            Debug.Log("Starting multiple playbacks");
            StartMultiplePlaybacks(remainingRuns + 1, availableRuns); 
        }
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0);
        if(movement != Vector3.zero) { StartRun(); }

        transform.Translate(movement * speed * Time.deltaTime);
    }

    private void StartRun()
    {
        runStarted = true;
        countdownTimer.StartTimer();
    }

    private void ResetRun(bool allRuns = false)
    {
        Debug.Log("Remaining runs: " + remainingRuns);
        transform.position = startingPosition;

        countdownTimer.ResetTimer();
        if(allRuns) { remainingRuns = availableRuns; }
    }

    private void LockInRun()
    {
        if(!runStarted) { return; }
        if(remainingRuns <= 0) { return; }

        ResetRun();
        remainingRuns--;

        isPlaybackRunning = false;
        
        if(remainingRuns == 0) 
        {
            canMove = false;
            StartMultiplePlaybacks(1, availableRuns);
            canMove = true;
        }
    }

    private void StartMultiplePlaybacks(int beginAtRun, int endAtRun)
    {
        DestroyClones();
        isPlaybackRunning = true;

        for(int i = beginAtRun; i <= endAtRun; i++)
        {
            recorder.StartPlaybackRun(i, CreateClone(), speed);
        }
    }

    private GameObject CreateClone()
    {
        GameObject playerClone = Instantiate(gameObject, startingPosition, Quaternion.identity);
        playerClone.tag = "Clone";
        playerClone.GetComponent<PlayerController>().enabled = false;

        return playerClone;
    }

    private void DestroyClones()
    {
        GameObject[] cloneObjects = GameObject.FindGameObjectsWithTag("Clone"); // Assuming the game objects have a common tag

        foreach (GameObject cloneObject in cloneObjects)
        {
            Destroy(cloneObject);
        }
    }
}