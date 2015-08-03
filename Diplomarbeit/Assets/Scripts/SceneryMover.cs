using UnityEngine;
using System.Collections;

public class SceneryMover : MonoBehaviour {

	public float amount = 109.2f; // Width 2x of Scenery Prefab

	// Use this for initialization
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") 
		{
			Vector3 pos = gameObject.transform.position;
			pos.x += amount;
			gameObject.transform.position = pos;
		}
	}
}
