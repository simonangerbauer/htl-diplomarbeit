using UnityEngine;
using System.Collections;

public class ObstacleRemover : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Obstacle") 
		{
			other.gameObject.SetActive(false);
			other.gameObject.GetComponent<ObstacleCollisionManager>().isCollided = false;
		}
	}
}
