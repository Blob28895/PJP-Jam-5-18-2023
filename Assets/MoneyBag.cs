using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyBag : MonoBehaviour
{
	private GameObject levelComplete;

	private void Awake()
	{
		levelComplete = GameObject.FindGameObjectWithTag("Level Complete");
		levelComplete.SetActive(false);
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Player")) {
			Debug.Log("Entered");
			levelComplete.SetActive(true);	
		}

	}
}
