using UnityEngine;
using System.Collections;

public class CueBallSpawner : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GameObject cueBallPrefab = (GameObject)Resources.Load("Spawned/CueBall");
        GameObject.Instantiate(cueBallPrefab);        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
