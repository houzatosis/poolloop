using UnityEngine;
using System.Collections;

public class PoolLoopBall : MonoBehaviour
{
    public string ballName = "Ball"; // e.g. cue, stripe, solid, needs to match name of a renderable prefab in the resources folder
    const int MAX_NUM_SCRUB_FRAMES = 4096;

    enum State
    {
        IDLE,
        ACTIVE,
        SCRUBBING
    }

    struct PositionKey
    {
        public float clockTime;
        public Vector3 position;
    }

    PositionKey[] scrubTrack;
    int numScrubFrames;
    int lastScrubFrame;

    State state;

    PoolLoopGame game;
    Rigidbody2D body;

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

        body = GetComponent<Rigidbody2D>();
        body.mass = 0.17f;
        body.drag = 0.5f;

        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        game = GameObject.Find("PoolLoopGame").GetComponent<PoolLoopGame>();
        game.OnPoolLoopBallStart(this);

        scrubTrack = new PositionKey[MAX_NUM_SCRUB_FRAMES];
        numScrubFrames = 0;

        state = State.IDLE;
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void FixedUpdate()
    {
        if (state == State.ACTIVE)
        {
            if (numScrubFrames < MAX_NUM_SCRUB_FRAMES)
            {
                scrubTrack[numScrubFrames].clockTime = Time.fixedTime - game.GetLastClockStartTime();
                scrubTrack[numScrubFrames].position = gameObject.transform.position;
                ++numScrubFrames;                
            }
            else
            {
                Debug.Log("Ran out of scrub frames!");
            }
        }
    }

    public void EnterActiveState()
    {
        state = State.ACTIVE;
        body.isKinematic = false;
        numScrubFrames = 0;
    }

    public void EnterScrubbingState()
    {
        state = State.SCRUBBING;
        body.isKinematic = true;

        Debug.Log("Used " + numScrubFrames.ToString() + " scrub frames");
        if (numScrubFrames > 0)
            lastScrubFrame = numScrubFrames - 1;
        else
            lastScrubFrame = 0;        
    }

    public void UpdateScrubTime(float scrubTime)
    {
        while (lastScrubFrame > 0 && scrubTrack[lastScrubFrame - 1].clockTime > scrubTime)
            --lastScrubFrame;
        
        if (lastScrubFrame == 0 || lastScrubFrame == (numScrubFrames - 1))
        {
            transform.position = scrubTrack[lastScrubFrame].position;
        }
        else
        {
            PositionKey k0 = scrubTrack[lastScrubFrame];
            PositionKey k1 = scrubTrack[lastScrubFrame + 1];
            float tSpan = k1.clockTime - k0.clockTime;
            float t = (scrubTime - k0.clockTime) / tSpan;
            
            transform.position = Vector3.Lerp(k0.position, k1.position, t);
        }
    }
}
