using UnityEngine;
using System.Collections;

public class PoolLoopGame : MonoBehaviour
{
    enum State
    {
        BEFORE_PREPARING_SHOT,
        PREPARING_SHOT,
        BALLS_ACTIVE
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

	void Start ()
    {
        state = State.BEFORE_PREPARING_SHOT;
        cueBall = null;

        shotCamera = GameObject.Find("ShotCamera").GetComponent<Camera>();
        poolStick = GameObject.Find("PoolStick");
    }
	
	void Update ()
    {
        switch (state)
        {
        case State.BEFORE_PREPARING_SHOT:
            BeforePreparingShotUpdate();
            break;
        case State.PREPARING_SHOT:
            PreparingShotUpdate();
            break;
        }
	}

    void BeforePreparingShotUpdate()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray testRay = shotCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(testRay, out hit))
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
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(shotCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                shotDistance = (cueBall.transform.position - hit.point).magnitude;
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
        float zOffset = offsetDistance * Mathf.Sin(shotAngle);

        poolStick.transform.position = cueBall.transform.position + new Vector3(xOffset, 0, zOffset);
        poolStick.transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg*(Mathf.PI - shotAngle), 90);
    }

    void EnterBallsActiveState()
    {
        Rigidbody body = cueBall.GetComponent<Rigidbody>();
        Vector3 forceDirection = new Vector3(-Mathf.Cos(shotAngle), 0, -Mathf.Sin(shotAngle));

        Vector3 forcePosition = cueBall.transform.position - forceDirection * cueBall.transform.localScale.x;

        body.AddForceAtPosition(forceDirection*Mathf.Pow(10, 1 + shotDistance), forcePosition);
    }
}
