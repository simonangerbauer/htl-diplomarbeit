using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
	public Slider health;

	public void Awake()
	{
		health.value = 100;
	}

	public void ChangeHealth(int value)
	{
		health.value += value;
		if (health.value <= 0) 
		{
			playerDead();
		}
	}
	private void playerDead()
	{
		GameObject.Find("GameController").GetComponent<GameController>().GameOver ();
	}
}
