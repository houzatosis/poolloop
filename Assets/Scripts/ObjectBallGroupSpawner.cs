using UnityEngine;
using System.Collections;

public class ObjectBallGroupSpawner : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GameObject cueBallPrefab = (GameObject)Resources.Load("Spawned/TriangleBallLayout");
        GameObject.Instantiate(cueBallPrefab);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
