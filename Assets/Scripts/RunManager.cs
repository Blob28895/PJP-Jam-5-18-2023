using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RunManager : MonoBehaviour
{
    public int availableRuns = 3;
    public CountdownTimer countdownTimer;
    public TMP_Text remainingRunsText;

    private bool runStarted = false;
    private bool isPlaybackRunning = false;
    private int remainingRuns;
    private Vector3 startingPosition;

    private Recorder recorder;
    private PlayerController player;
    
    void Start()
    {
        recorder = GetComponent<Recorder>();
        player = GetComponent<PlayerController>();

        startingPosition = transform.position;
        remainingRuns = availableRuns;
    }

    void FixedUpdate()
    {
        if(runStarted) {
            if(countdownTimer.timerEnded) { ResetRun(); }
            recorder.RecordPosition(transform.position, remainingRuns); 
        }

        if(runStarted && !isPlaybackRunning && remainingRuns != availableRuns) { 
            Debug.Log("Starting multiple playbacks");
            StartMultiplePlaybacks(remainingRuns + 1, availableRuns, player.speed); 
        }
        remainingRunsText.text = "Remaining Echoes: " + remainingRuns;
    }

    public void StartRun()
    {
        runStarted = true;
        countdownTimer.StartTimer();
    }

    public void ResetRun(bool allRuns = false)
    {
        player.setCanMove(true);
        transform.position = startingPosition;

        countdownTimer.ResetTimer();
        recorder.StopAllPlaybacks();
        isPlaybackRunning = false;

        DestroyClones();
        resetEnemies();

        if(allRuns) { 
            remainingRuns = availableRuns;
            recorder.ClearAllRecordings();
        }
        runStarted = false;
    }

    public void LockInRun()
    {
        if(!runStarted) { return; }
        if(remainingRuns <= 0) { return; }

        ResetRun();
        remainingRuns--;

        isPlaybackRunning = false;
        
        if(remainingRuns == 0) 
        {
            StartMultiplePlaybacks(1, availableRuns, player.speed);
        }
    }

    private void StartMultiplePlaybacks(int beginAtRun, int endAtRun, int speed)
    {
        recorder.StopAllPlaybacks();
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
        playerClone.GetComponent<RunManager>().enabled = false;
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

    public void resetEnemies()
	{
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("enemy"))
		{
            enemy.GetComponent<EnemyInstructions>().resetEnemy();
		}
	}
}
