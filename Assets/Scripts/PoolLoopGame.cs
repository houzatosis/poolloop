using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PoolLoopGame : MonoBehaviour
{
    // Recorded Shot
    class Shot
    {

    }

    enum State
    {
        BEFORE_PREPARING_SHOT,
        PREPARING_SHOT,
        BALLS_ACTIVE,
        BALLS_REWINDING
    }

    State state;

    // when we're preparing a shot, this is the cueBall we're trying to hit
    PoolLoopBall cueBall;

    Vector3 cueBallScreenPosition;
    // from which angle do we hit the ball (this angle is measured on the XZ plane relative to the positive world x-axis)
    float shotAngle;
    // shot distance is an analog for the shot power, the larger the distance, the more powerful the shot
    float shotDistance;

    Camera shotCamera;
    GameObject poolStick;
    Text debugText;

    float clockTime;

    ArrayList balls;

    void Awake()
    {
        balls = new ArrayList();
    }

	void Start ()
    {
        state = State.BEFORE_PREPARING_SHOT;
        cueBall = null;

        shotCamera = GameObject.Find("ShotCamera").GetComponent<Camera>();
        poolStick = GameObject.Find("PoolStick");

        clockTime = 0.0f;

        debugText = gameObject.GetComponentInChildren<Text>();
    }

    void Update()
    {
        switch (state)
        {
            case State.BEFORE_PREPARING_SHOT:
                BeforePreparingShotUpdate();
                break;
            case State.PREPARING_SHOT:
                PreparingShotUpdate();
                break;
            case State.BALLS_ACTIVE:
                BallsActiveUpdate();
                break;
        }

        RefreshDebugText();
    }

    void BeforePreparingShotUpdate()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray testRay = shotCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(testRay);
            if (hit)
            {
                PoolLoopBall ball = hit.collider.gameObject.GetComponent<PoolLoopBall>();
                if (ball != null)
                {
                    if (ball.ballName == "CueBall")
                        EnterPreparingShotState(ball);
                }
            }
        }
    }

    void EnterPreparingShotState(PoolLoopBall cueBallToHit)
    {
        cueBall = cueBallToHit;
        state = State.PREPARING_SHOT;

        MeshRenderer poolStickRenderer = poolStick.GetComponent<MeshRenderer>();
        poolStickRenderer.enabled = true;

        cueBallScreenPosition = shotCamera.WorldToScreenPoint(cueBall.transform.position);

        shotAngle = 0;
        shotDistance = 0.01f;
    }

    void PreparingShotUpdate()
    {
        Vector3 mouseToCueball = Input.mousePosition - cueBallScreenPosition;
        if (mouseToCueball.sqrMagnitude > 0.01f)
        {
            // update shotAngle and shotDistance
            shotAngle = Mathf.Atan2(mouseToCueball.y, mouseToCueball.x);
            RaycastHit2D hit = Physics2D.GetRayIntersection(shotCamera.ScreenPointToRay(Input.mousePosition));
            if (hit)
            {
                shotDistance = (cueBall.transform.position - new Vector3(hit.point.x, hit.point.y, cueBall.transform.position.z)).magnitude;
                if (shotDistance < cueBall.transform.localScale.x)
                    shotDistance = cueBall.transform.localScale.x + 0.01f;
            }            
        }

        OrientStick();

        if (Input.GetButtonDown("Fire1"))
        {
            EnterBallsActiveState();
        }
    }

    void OrientStick()
    {
        CapsuleCollider collider = poolStick.GetComponent<CapsuleCollider>();
        float stickLength = collider.height * poolStick.transform.localScale.y;
        float offsetDistance = stickLength * 0.5f + shotDistance;

        float xOffset = offsetDistance * Mathf.Cos(shotAngle);
        float yOffset = offsetDistance * Mathf.Sin(shotAngle);

        poolStick.transform.position = cueBall.transform.position + new Vector3(xOffset, yOffset, 0);
        poolStick.transform.rotation = Quaternion.Euler(0, 0, 90 - Mathf.Rad2Deg * (Mathf.PI - shotAngle));
    }

    void EnterBallsActiveState()
    {
        state = State.BALLS_ACTIVE;
        MeshRenderer poolStickRenderer = poolStick.GetComponent<MeshRenderer>();
        poolStickRenderer.enabled = false;

        Rigidbody2D body = cueBall.GetComponent<Rigidbody2D>();
        Vector2 forceDirection = new Vector3(-Mathf.Cos(shotAngle), -Mathf.Sin(shotAngle), 0);

        Vector2 cueBallPos2d = new Vector2(cueBall.transform.position.x, cueBall.transform.position.y);
        Vector2 forcePosition = cueBallPos2d - forceDirection * cueBall.transform.localScale.x;

        body.AddForceAtPosition(forceDirection*Mathf.Pow(25, 1 + shotDistance), forcePosition);
    }

    void BallsActiveUpdate()
    {
        clockTime += Time.deltaTime;

        bool allSleeping = true;
        for (int i = 0; i < balls.Count; ++i)
        {
            PoolLoopBall ball = (PoolLoopBall)balls[i];
            Rigidbody2D ballBody = ball.gameObject.GetComponent<Rigidbody2D>();
            if (!ballBody.IsSleeping())
            {
                allSleeping = false;
                break;
            }
        }

        if (allSleeping)
        {
            state = State.BEFORE_PREPARING_SHOT;
        }
    }

    void RefreshDebugText()
    {
        debugText.text = "State = " + state.ToString() + " Shot Clock = " + clockTime;
    }

    public void OnPoolLoopBallStart(PoolLoopBall ball)
    {
        balls.Add(ball);
    }    
}
