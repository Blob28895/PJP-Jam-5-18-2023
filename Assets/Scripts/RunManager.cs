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
    private float elapsedTime = 0f;

    private Recorder recorder;
    private PlayerController player;
    private AudioManager audioManager;

    void Start()
    {
        recorder = GetComponent<Recorder>();
        player = GetComponent<PlayerController>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        startingPosition = transform.position;
        remainingRuns = availableRuns;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if(runStarted) 
        {
            if(countdownTimer.timerEnded) 
            {
                recorder.ClearRecording(remainingRuns);
                ResetRun(); 
                audioManager.PlaySound("OutOfTime"); 
            }
            Debug.Log("Recording position");
            recorder.RecordPosition(transform.position, remainingRuns); 
        }

        if(runStarted && !isPlaybackRunning) { 
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

    public void ResetSingleRun()
    {
        recorder.ClearRecording(remainingRuns);
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

    public (string, string) getRunStats()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        
        string formattedElapsedTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        string usedEchoes = availableRuns - remainingRuns + "/" + availableRuns;
        
        return (formattedElapsedTime, usedEchoes);
    }
}
