using UnityEngine;
using System;

public class CollisionManager : MonoBehaviour {
	public bool isCollided = false;

	public void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Player" && !isCollided) 
		{
			col.gameObject.GetComponent<PlayerHealth>().ChangeHealth(-20);
			isCollided = true;
		}

	}
	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Remover" || other.tag == "Enemy" || other.tag == "Ground")
		{
			if(other.tag == "Enemy")
			{
				other.gameObject.SetActive(false);
				GameController.instance.ChangeCurrency(10);
			}
			gameObject.SetActive(false);
			isCollided = false;
		}

	}
}
