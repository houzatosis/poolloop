using UnityEngine;
using System.Collections;

public class PoolLoopBall : MonoBehaviour
{
    public string ballName = "Ball"; // e.g. cue, stripe, solid, needs to match name of a renderable prefab in the resources folder

	// Use this for initialization
	void Start ()
    {
        GameObject prefabResource = (GameObject)Resources.Load(ballName);        
        if (prefabResource != null)
        {
            GameObject ballInstance = GameObject.Instantiate(prefabResource);
            ballInstance.transform.parent = transform;
            // reset local position and scale so they match the layout of the spawner collision
            ballInstance.transform.localPosition = new Vector3(0, 0, 0);
            ballInstance.transform.localScale = new Vector3(1, 1, 1);

            // remove default mesh filter and mesh renderer
            Destroy(gameObject.GetComponent<MeshFilter>());
            Destroy(gameObject.GetComponent<MeshRenderer>());
        }
        else
        {
            Debug.LogError("Couldn't load resource " + ballName);
        }

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.mass = 0.17f;
        body.drag = 0.5f;

        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        PoolLoopGame game = GameObject.Find("PoolLoopGame").GetComponent<PoolLoopGame>();
        game.OnPoolLoopBallStart(this);
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
