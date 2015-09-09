using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Spawner : MonoBehaviour {

	public float spawnMin = 1.0f;
	public float spawnMax = 2.0f;
	public GameObject[] possibleOjects;
	public bool spawnObjects = true;
	public int Seed;

	List<GameObject> objects;
	// Use this for initialization
	void Start () 
	{
		objects = new List<GameObject> ();
		Invoke ("Spawn", 1.0f);
	}

	void Spawn()
	{
		for (int i = 0; i < objects.Count; i++) 
		{
			GameObject temp = objects [i];
			if(Seed != null)
			{
				Random.seed = Seed;
			}
			else
			{
				Random.seed = System.Environment.TickCount; 
			}
			int randomIndex = Random.Range (i, objects.Count);
			objects [i] = objects [randomIndex];
			objects [randomIndex] = temp;
		}
		GameObject obj = objects.FirstOrDefault (x => !x.activeInHierarchy);
		if (obj == null) 
		{
			for (int i = 0; i < possibleOjects.GetLength(0); i++) {
				GameObject tmp = (GameObject)Instantiate (possibleOjects [i]);
				tmp.SetActive (false);
				objects.Add (tmp);
				obj = tmp;
			}
		}
		obj.transform.position = new Vector3 (gameObject.transform.position.x, obj.transform.position.y, 0);
		obj.SetActive (true);
		Invoke ("Spawn", Random.Range(spawnMin, spawnMax));
	}
}
