using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionFlight : MonoBehaviour
{
    public Animator animator; ///< The animator.
    public string speedParam = "Speed"; ///< The speed parameter.
    public string angularSpeedParam = "AngularSpeed"; ///< The angular speed parameter.

    public float speed, angularSpeed, acceleration, pitchSpeed;

    Vector3 targetPos;

    Vector3 desiredVelocity;
    float angularVelocity;
    float desiredAngularVelocity;
    public float stoppingDistance = 0.1f;
    Vector3 currentVelocity;
    Vector3 currentAcceleration;
    float currentPitch;
    float desiredPitch;


    bool isMoving;

    public void SetDestination(Vector3 _targetPos)
    {
        targetPos = _targetPos;
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            ComputePhysics();
            float requiredDelta = targetPos.y - transform.position.y;
            float deltaDot = Vector3.Dot(Vector3.up, targetPos - transform.position);
            desiredPitch = Mathf.Clamp(deltaDot * 45, -45, 45);
            desiredPitch *= 1f - Mathf.Clamp(Mathf.Abs(angularVelocity), 0, 1);
            if (Vector3.Distance(transform.position, targetPos) < stoppingDistance)
            {
                isMoving = false;
            }
        }
        else
        {
            ComputePhysics();
            desiredVelocity = Vector3.zero;
            desiredAngularVelocity = 0;
            desiredPitch = 0;
            angularVelocity = Mathf.MoveTowards(angularVelocity, desiredAngularVelocity, angularSpeed * Time.deltaTime);
            currentVelocity = transform.forward * speed;
            currentAcceleration = (currentVelocity - desiredVelocity).normalized * acceleration;
        }
        animator.SetFloat(speedParam, currentVelocity.magnitude);
        animator.SetFloat(angularSpeedParam, angularVelocity);
        //Lerp the X axis rotation based on desired vertical velocity

        currentPitch = Mathf.MoveTowards(currentPitch, -desiredPitch, pitchSpeed * Time.deltaTime);
        transform.localEulerAngles = new Vector3(currentPitch, transform.localEulerAngles.y, 0f);
    }

    float GetTurnDirection(Vector3 targetVelocity)
    {
        Vector3 forward = transform.right;
        Vector3 withoutY = targetVelocity;
        withoutY.y = 0;
        Vector3 targetForward = withoutY.normalized;
        float dot = Vector3.Dot(forward, targetForward);
        return dot;
    }

    void ComputePhysics()
    {
        desiredVelocity = targetPos - transform.position;
        desiredAngularVelocity = GetTurnDirection(desiredVelocity);
        currentAcceleration = (desiredVelocity - currentVelocity) * acceleration;
        currentVelocity += currentAcceleration * Time.deltaTime;
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, speed);
        angularVelocity = Mathf.Lerp(angularVelocity, desiredAngularVelocity, angularSpeed * Time.deltaTime);
    }






}
