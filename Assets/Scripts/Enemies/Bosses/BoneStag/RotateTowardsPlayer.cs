using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
///  Script to handle rotation and translation towards the player as part of enemy attacks.
/// </summary>
/// <seealso cref="RotateTowards"/>
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
        direction.y = 0f;
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
            agent.Move((targetPos - startPos).normalized * Time.deltaTime * speed);
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

    public Vector3 GetPlayerDir()
    {
        return (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
    }

    /// <summary>
    ///  Method to rotate towards the player.
    /// </summary>
    /// <param name="duration">The length of time to rotate.</param>
    /// <param name="speed">The speed of the rotation.</param>
    /// <param name="delay">The delay before the rotation starts.</param>
    /// <example>
    /// <code>
    /// RotateToPlayer(1.0f, 6.0f, 0.2f); // Rotate towards the player for 1 second at a speed of 6, after a delay of 0.2 seconds.
    /// </code>
    /// </example>
    public void RotateToPlayer(float duration, float speed, float delay)
    {
        StartCoroutine(RotateToPlayerCoroutine(duration, speed, delay));
    }

}
