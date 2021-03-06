using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
///  Script to handle rotation and translation towards the player as part of enemy attacks.
/// </summary>
public class RotateTowardsPlayer : MonoBehaviour
{
    /// <summary>
    ///  Coroutine to rotate towards the player.
    /// </summary>
    /// <param name="duration">The length of time to rotate.</param>
    /// <param name="speed">The speed of the rotation.</param>
    /// <param name="delay">The delay before the rotation starts.</param>
    /// <returns></returns>
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
    /// <summary>
    ///  Coroutine to move towards the player.
    /// </summary>
    /// <param name="duration">The length of time to move.</param>
    /// <param name="speed">The speed of the movement.</param>
    /// <param name="delay">The delay before the movement starts.</param>
    /// <returns></returns>
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
    /// <summary>
    ///  Method to move towards the player.
    /// </summary>
    /// <param name="duration">The length of time to move.</param>
    /// <param name="speed">The speed of the movement.</param>
    /// <param name="delay">The delay before the movement starts.</param>
    public void MoveToPlayer(float duration, float speed, float delay)
    {
        StartCoroutine(MoveToPlayerCoroutine(duration, speed, delay));
    }

    /// <summary>
    ///  Method to rotate towards the player.
    /// </summary>
    /// <param name="duration">The length of time to rotate.</param>
    /// <param name="speed">The speed of the rotation.</param>
    /// <param name="delay">The delay before the rotation starts.</param>
    public void RotateToPlayer(float duration, float speed, float delay)
    {
        StartCoroutine(RotateToPlayerCoroutine(duration, speed, delay));
    }
    
}
