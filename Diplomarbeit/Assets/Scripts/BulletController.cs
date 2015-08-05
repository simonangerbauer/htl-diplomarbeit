using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Enemy") 
		{
			other.gameObject.SetActive(false);
			gameObject.SetActive(false);
		}
	}
}
