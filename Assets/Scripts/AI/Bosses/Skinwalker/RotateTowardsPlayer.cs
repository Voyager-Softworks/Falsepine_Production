using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RotateTowardsPlayer : MonoBehaviour
{
    private IEnumerator RotateToPlayerCoroutine(float duration, float speed, float delay)
    {
        yield return new WaitForSeconds(delay);
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Vector3 targetPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 direction = targetPos - startPos;
        float angle = Vector3.Angle(direction, transform.forward);
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MoveToPlayerCoroutine(float duration, float speed, float delay)
    {
        yield return new WaitForSeconds(delay);
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Vector3 targetPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 direction = targetPos - startPos;
        while (elapsedTime < duration)
        {
            agent.Move((targetPos - startPos) * Time.deltaTime * speed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
    }

    public void MoveToPlayer(float duration, float speed, float delay)
    {
        StartCoroutine(MoveToPlayerCoroutine(duration, speed, delay));
    }

    public void RotateToPlayer(float duration, float speed, float delay)
    {
        StartCoroutine(RotateToPlayerCoroutine(duration, speed, delay));
    }
    
}
