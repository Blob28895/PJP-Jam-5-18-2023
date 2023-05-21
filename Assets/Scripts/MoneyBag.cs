using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyBag : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Player")) 
		{
			PlayerController player = collision.GetComponent<PlayerController>();
			player.displayVictory();
			GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().PlaySound("Coins");
		}
	}
}
