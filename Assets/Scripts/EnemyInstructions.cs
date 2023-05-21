using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstructions: MonoBehaviour
{
	public LayerMask includedLayers;
	public GameObject detectionBubble;
	public float detectionTime = 0.5f;
	public GameObject enemyLight;
	public float walkingSpeed = 1f;
	public float lightDetectionRange = 4.5f;
	[SerializeField]
	public Instruction[] instructions;
	
	private Transform enemyTransform;
	private IEnumerator wait;
	private bool waiting = false;
	private int instructionIndex = 0;
	private Instruction currInstruction;
	private Vector3 instructionEndPosition;
	private float instructionEndOrientation;

	private bool rotating = false;
	private float degreesPerTick = 3f;
	private Quaternion initialOrientation;
	private Vector3 initialPosition;
	private int rotations;
	private float finishDetectTime = 0f;

	private GameObject hitPlayer = null;
	private bool playerSpotted = false;
	private Vector3 originVector = new Vector3(-4.5f, 0, 0);
	private Dictionary<float, Vector3> rays = new Dictionary<float, Vector3>()
	{
		{-20f, new Vector3() },
		{-10f, new Vector3() },
		{0f, new Vector3() },
		{10f, new Vector3() },
		{20f, new Vector3() }
	};

	public enum Action { 
		walk,
		leftTurn,
		rightTurn
	}
	[System.Serializable]
	public class Instruction
	{
		public Action action;
		public float amount;
		public float waitAfterCompletion;

		public Instruction()
		{
			action = Action.walk;
			amount = 0;
		}
	};
	private void Awake()
	{
		enemyTransform = GetComponent<Transform>();
		
		updateRays();
		//drawFunc();
		if (instructions.Length > 0)
		{
			currInstruction = instructions[0];
			setEndState();
		}
		
		
		initialOrientation = enemyTransform.rotation;
		initialPosition = enemyTransform.position;
		
	}

	private void drawFunc()
	{
		for(float i = -20f; i < 30f; i += 10f)
		{
			Debug.DrawLine(enemyTransform.position, enemyTransform.position + rays[i], Color.red, 2.5f);
		}
	}
	
	private void FixedUpdate()
	{
		if (!playerSpotted)
		{

			if (instructions.Length == 0)
			{
				return;
			}
			if (enemyTransform.position != instructionEndPosition)
			{
				//Debug.Log("Moving towards instruction");
				enemyTransform.position = Vector3.MoveTowards(enemyTransform.position, instructionEndPosition, walkingSpeed * Time.deltaTime);
			}
			else if (currInstruction.action == Action.walk && !waiting)
			{
				wait = waiter(currInstruction.waitAfterCompletion);
				StartCoroutine(wait);
			}

			if (rotations > 0)
			{
				//Debug.Log("Rotating");
				rotating = true;
				--rotations;
				turn();
			}
			else if (rotating)
			{
				rotating = false;
				wait = waiter(currInstruction.waitAfterCompletion);
				StartCoroutine(wait);
			}
			updateRays();
			//drawFunc();
			hitPlayer = fireRaycasts();	
		}
		else if(Time.time >= finishDetectTime)
		{ // if the player is already spotted
			//Debug.Log("Moving Towards Player");
			if(enemyTransform.position != hitPlayer.transform.position)
			{
				walk(hitPlayer.transform.position);
			}
			else
			{
				enemyLight.SetActive(false);
			}
		}

		
		if(hitPlayer != null && !playerSpotted)
		{ // will only be entered when first seeing a player
		  //Debug.Log("Found him!");
			GetComponent<AudioSource>().Play();
			playerSpotted = true;
			GameObject bubble = Instantiate(detectionBubble);
			bubble.transform.position = enemyTransform.position + new Vector3(0, GetComponent<SpriteRenderer>().bounds.size.x / 2, 0);
			Destroy(bubble, detectionTime);
			finishDetectTime = Time.time + detectionTime;
			hitPlayer.GetComponent<Collider2D>().isTrigger = true;
			hitPlayer.GetComponent<PlayerController>().setCanMove(false);
			//hitPlayer.GetComponent<PlayerController>().enabled = false;
			if(waiting)
			{
				StopCoroutine(wait);
			}
		}

	}
	private GameObject fireRaycasts()
	{
		RaycastHit2D hit;
		for(float i = -20f; i < 30; i +=10)
		{
			if(i == 0)
			{
				Debug.DrawRay(enemyTransform.position, /*enemyTransform.position +*/ rays[i], Color.green, 2f);

			}
			//Debug.Log(rays[0]);
			hit = Physics2D.Raycast(enemyTransform.position, /*enemyTransform.position +*/ rays[i], lightDetectionRange, includedLayers);
			if(hit.collider != null && (hit.collider.gameObject.CompareTag("Player") || hit.collider.gameObject.CompareTag("Clone")))
			{
				enemyTransform.Rotate(0, 0, i);
				Debug.Log(i);
				return hit.collider.gameObject;
			}
		}

		return null;
	}
	private void setEndState()
	{
		if(currInstruction.action == Action.walk)
		{
			setEndPosition(currInstruction.amount);
			instructionEndOrientation = enemyTransform.rotation.z;
		}
		else
		{
			setEndOrientation(currInstruction.action, currInstruction.amount);
			instructionEndPosition = enemyTransform.position;
		}
	}
	private void setEndPosition(float distance)
	{
		instructionEndPosition = enemyTransform.position + (rays[0] / lightDetectionRange) * distance;
	}
	private void setEndOrientation(Action action, float degrees)
	{
		if (action == Action.rightTurn)
		{
			//Debug.Log("right turn");
			degrees *= -1;
		}
		instructionEndOrientation = enemyTransform.rotation.z + degrees;
		rotations = (int)(Mathf.Abs(instructionEndOrientation - enemyTransform.rotation.z) / degreesPerTick);
		//Debug.Log(instructionEndOrientation);
	}

	private IEnumerator waiter(float seconds)
	{
		//Debug.Log("Starting Waiting");
		waiting = true;
		yield return new WaitForSeconds(seconds);
		nextInstruction();
		waiting = false;
		//Debug.Log("Done Waiting");
	}

	private void nextInstruction()
	{

		instructionIndex++;
		if(instructionIndex >= instructions.Length || instructionIndex < 0)
		{
			instructionIndex = 0;
		}
		//Debug.Log(instructionIndex);
		currInstruction = instructions[instructionIndex];
		setEndState();
	}
	private void walk(Vector3 pos)
	{
		enemyTransform.position = Vector3.MoveTowards(enemyTransform.position, pos, walkingSpeed * Time.deltaTime);
		
	}

	private void turn()
	{
		if(currInstruction.action == Action.leftTurn)
		{
			enemyTransform.Rotate(0, 0, degreesPerTick);
		}
		else
		{
			enemyTransform.Rotate(0, 0, -degreesPerTick);
		}
	}

	private void updateRays()
	{
		//Debug.Log(enemyTransform.rotation.z);
		rays[0] = Quaternion.AngleAxis(enemyTransform.eulerAngles.z, Vector3.forward) * originVector;
		rays[-20f] = Quaternion.AngleAxis(-20f + enemyTransform.eulerAngles.z, Vector3.forward) * originVector;
		rays[-10f] = Quaternion.AngleAxis(-10f + enemyTransform.eulerAngles.z, Vector3.forward) * originVector;
		rays[10f] = Quaternion.AngleAxis(10f + enemyTransform.eulerAngles.z, Vector3.forward) * originVector;
		rays[20f] = Quaternion.AngleAxis(20f + enemyTransform.eulerAngles.z, Vector3.forward) * originVector;
		//drawFunc();
		
	}

	public void resetEnemy()
	{
		enemyTransform.position = initialPosition;
		enemyTransform.rotation = initialOrientation;
		rotations = 0;
		instructionEndOrientation = enemyTransform.rotation.z;
		instructionEndPosition = enemyTransform.position;
		hitPlayer = null;
		playerSpotted = false;
		instructionIndex = -1;
		updateRays();
		nextInstruction();
		if(waiting)
		{
			//Debug.Log("Stop Wait Early");
			waiting = false;
			StopCoroutine(wait);
		}
		
		rotating = false;
		enemyLight.SetActive(true);
		
	}



}
