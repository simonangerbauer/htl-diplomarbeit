using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObstacleSpawner : MonoBehaviour {

	public float spawnMin = 1.0f;
	public float spawnMax = 2.0f;
	public GameObject[] possibleObstacles;
	public int pooledAmount = 9;

	List<GameObject> obstacles;
	// Use this for initialization
	void Start () 
	{
		obstacles = new List<GameObject> ();
		Invoke ("Spawn", 1.0f);
	}
	
	void Spawn()
	{
		for (int i = 0; i < obstacles.Count; i++) {
			GameObject temp = obstacles[i];
			int randomIndex = Random.Range(i, obstacles.Count);
			obstacles[i] = obstacles[randomIndex];
			obstacles[randomIndex] = temp;
		}
		GameObject obstacle = obstacles.FirstOrDefault(x => !x.activeInHierarchy);
		if (obstacle == null) 
		{
			for(int i = 0; i < possibleObstacles.GetLength(0); i++)
			{
				GameObject obj = (GameObject)Instantiate(possibleObstacles[i]);
				obj.SetActive(false);
				obstacles.Add(obj);
				obstacle = obj;
			}
		}
		obstacle.transform.position = new Vector3 (gameObject.transform.position.x, obstacle.transform.position.y, 0);
		obstacle.SetActive (true);
		Invoke ("Spawn", Random.Range(spawnMin, spawnMax));
	}
}
