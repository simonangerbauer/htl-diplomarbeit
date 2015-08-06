using UnityEngine;
using System;

public class ObstacleCollisionManager : MonoBehaviour {
	public string tagToDetectCollision;
	public bool isCollided = false;

	public void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Player" && !isCollided) 
		{
			col.gameObject.GetComponent<PlayerHealth>().ChangeHealth(-20);
			isCollided = true;
		}
	}
}
