using UnityEngine;
using System.Collections;

public class CueBallSpawner : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GameObject cueBallPrefab = (GameObject)Resources.Load("Spawned/CueBall");
        GameObject clone = GameObject.Instantiate(cueBallPrefab);
        clone.transform.position = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
