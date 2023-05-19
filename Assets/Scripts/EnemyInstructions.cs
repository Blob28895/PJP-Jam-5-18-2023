using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstructions: MonoBehaviour
{

	public enum Action { 
		walk,
		leftTurn,
		rightTurn
	}
	[System.Serializable]
	public class Instruction
	{
		public Action action;
		public int amount;

		public Instruction()
		{
			action = Action.walk;
			amount = 0;
		}
	};


	[SerializeField]
	public Instruction[] instructions;

	
}
