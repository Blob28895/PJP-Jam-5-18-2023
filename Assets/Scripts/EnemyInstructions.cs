using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstructions: MonoBehaviour
{
	public float walkingSpeed = 1f;
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

	private Vector3 rayDirection = new Vector2(-1, 0);
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
		if (instructions.Length > 0)
		{
			currInstruction = instructions[0];
			setEndState();
		}
		
		Debug.DrawLine(transform.position, rayDirection, Color.red, 5f);
		initialOrientation = transform.rotation.z;
		initialPosition = transform.position;
		
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
			--rotations;
			transform.Rotate(0, 0, degreesPerTick);
		}

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
		instructionEndPosition = transform.position + Vector3.left * distance;
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

	}




	
}
