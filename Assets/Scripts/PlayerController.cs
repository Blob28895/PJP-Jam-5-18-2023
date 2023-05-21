using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed = 5;
    private bool canMove = true;
    
    // variables for player movement
    private RunManager runManager;
    private Rigidbody2D rb;
    private ContactFilter2D movementFilter;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private Vector2 lastDirectionFacing = Vector2.zero;

    private AudioManager audioManager;

    // variables for victory display
    private bool isLevelComplete = false;
    private string nextLevelName;
    private GameObject levelComplete;
    private GameObject levelFailed;

    void Start()
    {  
        runManager = GetComponent<RunManager>();
        rb = GetComponent<Rigidbody2D>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        levelComplete = GameObject.FindGameObjectWithTag("Level Complete");
		levelComplete.SetActive(false);

        nextLevelName = getNextLevel(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        bool resetRun = Input.GetButtonDown("Reset Run"), resetAllRuns = Input.GetButtonDown("Reset All Runs");
        if(resetRun || resetAllRuns) {

            if(resetRun) { runManager.ResetSingleRun(); }
            runManager.ResetRun(resetAllRuns);
            canMove = true;
        }

        if(Input.GetButtonDown("Lock In Run") && !isLevelComplete) { 
            runManager.LockInRun(); 
        }

        // handle going to next scene
        if(Input.GetButtonDown("Lock In Run") && isLevelComplete) { 
            SceneManager.LoadScene(nextLevelName);
        }
    }

    private void FixedUpdate()
    {
        if(canMove) {
            Move();
        }
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        if(movement != Vector2.zero) { runManager.StartRun(); }

        // Check for potential collisions
		int count = rb.Cast (
			movement, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
			movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
			castCollisions, // List of collisions to store the found collisions into after the Cast is finished
			speed * Time.fixedDeltaTime); // The amount to cast equal to the movement

        

        bool success = movePlayer(movement);
        if(!success)
		{
            success = movePlayer(new Vector2(movement.x, 0));
            if(!success)
			{
                movePlayer(new Vector2(0, movement.y));
			}
		}

        // make player face the last direction they were facing
        if (movement == Vector2.zero) { movement = lastDirectionFacing; }
        else { lastDirectionFacing = movement; }

        // rotate player to direction facing
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f, 0f, angle + 180f);
    }

    private bool movePlayer(Vector2 vec)
	{
        int count = rb.Cast(
        vec, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
        movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
        castCollisions, // List of collisions to store the found collisions into after the Cast is finished
        speed * Time.fixedDeltaTime); // The amount to cast equal to the movement

        if (count == 0)
        {
            rb.MovePosition(rb.position + vec * speed * Time.fixedDeltaTime);
            return true;
        }
        return false;
	}

    public void setCanMove(bool b)
	{
        canMove = b;
	}

    public void displayVictory()
    {
        setCanMove(false);
        levelComplete.SetActive(true);
        isLevelComplete = true;
        (string elapsedTime, string usedEchoes) = runManager.getRunStats();

        levelComplete.transform.Find("Image").Find("Time").GetComponent<TMPro.TMP_Text>().text = elapsedTime;
        levelComplete.transform.Find("Image").Find("Echoes Used").GetComponent<TMPro.TMP_Text>().text = usedEchoes;
    }

    private string getNextLevel(string level)
    {
        string pattern = @"Level\s+(\d+)";
        Match match = Regex.Match(level, pattern);

        if(!match.Success) { throw new ArgumentException("Level name does not match pattern"); }

        if (int.TryParse(match.Groups[1].Value, out int levelNumber))
        {
            int nextLevelNumber = levelNumber + 1;
            return "Level " + nextLevelNumber.ToString();
        }
        throw new ArgumentException("Unable to parse level number");
    }
}