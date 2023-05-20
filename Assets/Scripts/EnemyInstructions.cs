using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstructions: MonoBehaviour
{
	public LayerMask includedLayers;
	public float walkingSpeed = 1f;
	public float lightDetectionRange = 4.5f;
	[SerializeField]
	public Instruction[] instructions;
	
	
	private Transform transform;
	private IEnumerator wait;
	private bool waiting = false;
	private int instructionIndex = 0;
	private Instruction currInstruction;
	private Vector3 instructionEndPosition;
	private float instructionEndOrientation;

	private bool rotating = false;
	private float degreesPerTick = 3f;
	private float initialOrientation;
	private Vector3 initialPosition;
	private int rotations;

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
		transform = GetComponent<Transform>();
		rays[0f] = originVector;
		initializeRays();
		//drawFunc();
		if (instructions.Length > 0)
		{
			currInstruction = instructions[0];
			setEndState();
		}
		
		
		initialOrientation = transform.rotation.z;
		initialPosition = transform.position;
		
	}

	public void initializeRays()
	{
		rays[-20f] = Quaternion.AngleAxis(-20f, Vector3.forward) * originVector;
		rays[-10f] = Quaternion.AngleAxis(-10f, Vector3.forward) * originVector;
		rays[10f] = Quaternion.AngleAxis(10f, Vector3.forward) * originVector;
		rays[20f] = Quaternion.AngleAxis(20f, Vector3.forward) * originVector;
	}
	private void drawFunc()
	{
		for(float i = -20f; i < 30f; i += 10f)
		{
			Debug.DrawLine(transform.position, transform.position + rays[i], Color.red, 2.5f);
		}
	}

	private void FixedUpdate()
	{
		if(instructions.Length == 0)
		{
			return;
		}
		if(transform.position != instructionEndPosition)
		{
			transform.position = Vector3.MoveTowards(transform.position, instructionEndPosition, walkingSpeed * Time.deltaTime);
		}
		else if (currInstruction.action == Action.walk && !waiting)
		{
			wait = waiter(currInstruction.waitAfterCompletion);
			StartCoroutine(wait);
		}

		if(rotations > 0)
		{
			rotating = true;
			--rotations;
			turn();
		}
		else if(rotating)
		{
			rotating = false;
			wait = waiter(currInstruction.waitAfterCompletion);
			StartCoroutine(wait);
		}


		updateRays();
		fireRaycasts();

		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.position + rays[0], lightDetectionRange, includedLayers);
		if(Physics2D.Raycast(transform.position, transform.position + rays[0], lightDetectionRange, includedLayers))
		{
			Debug.Log("Hit!");
		}
	}
	private GameObject fireRaycasts()
	{
		RaycastHit2D hit;
		for(float i = -20f; i < 30; i +=10)
		{
			hit = Physics2D.Raycast(transform.position, transform.position + rays[i], lightDetectionRange, includedLayers);
			if(hit.collider != null)
			{
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
			instructionEndOrientation = transform.rotation.z;
		}
		else
		{
			setEndOrientation(currInstruction.action, currInstruction.amount);
			instructionEndPosition = transform.position;
		}
	}
	private void setEndPosition(float distance)
	{
		instructionEndPosition = transform.position + (rays[0] / lightDetectionRange) * distance;
	}
	private void setEndOrientation(Action action, float degrees)
	{
		if (action == Action.rightTurn)
		{
			degrees *= -1;
		}
		instructionEndOrientation = transform.rotation.z + degrees;
		rotations = (int)(Mathf.Abs(instructionEndOrientation - transform.rotation.z) / degreesPerTick);
		Debug.Log(instructionEndOrientation);
	}

	private IEnumerator waiter(float seconds)
	{
		waiting = true;
		yield return new WaitForSeconds(seconds);
		nextInstruction();
		waiting = false;
	}

	private void nextInstruction()
	{

		instructionIndex++;
		if(instructionIndex >= instructions.Length)
		{
			instructionIndex = 0;
		}
		Debug.Log(instructionIndex);
		currInstruction = instructions[instructionIndex];
		setEndState();
	}
	private void walk()
	{
		transform.Translate(Vector3.left * walkingSpeed * Time.deltaTime);
		
	}

	private void turn()
	{
		transform.Rotate(0, 0, degreesPerTick);
		
	}

	private void updateRays()
	{
		//Debug.Log(transform.rotation.z);
		rays[0] = Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * originVector;
		rays[-20f] = Quaternion.AngleAxis(-20f + transform.eulerAngles.z, Vector3.forward) * originVector;
		rays[-10f] = Quaternion.AngleAxis(-10f + transform.eulerAngles.z, Vector3.forward) * originVector;
		rays[10f] = Quaternion.AngleAxis(10f + transform.eulerAngles.z, Vector3.forward) * originVector;
		rays[20f] = Quaternion.AngleAxis(20f + transform.eulerAngles.z, Vector3.forward) * originVector;
		
	}





}
