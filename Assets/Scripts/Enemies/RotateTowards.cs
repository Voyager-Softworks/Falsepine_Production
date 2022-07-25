using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  A class that handles coroutine rotation towards generic objects.
/// </summary>
/// <seealso cref="RotateTowardsPlayer"/>
public class RotateTowards : MonoBehaviour
{
     /// <summary>
    ///  Coroutine to rotate towards an object.
    /// </summary>
    /// <param name="obj">The object to rotate towards.</param>
    /// <param name="duration">The length of time to rotate.</param>
    /// <param name="speed">The speed of the rotation.</param>
    /// <param name="delay">The delay before the rotation starts.</param>
    /// <returns></returns>
    private IEnumerator RotateToObjectCoroutine(GameObject obj, float duration, float speed, float delay)
    {
        yield return new WaitForSeconds(delay);
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Vector3 targetPos = obj.transform.position;
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
    ///  Rotate the object towards another object.
    /// </summary>
    /// <param name="obj">The object to rotate towards.</param>
    /// <param name="duration">The duration for which to rotate.</param>
    /// <param name="speed">The rotation speed</param>
    /// <param name="delay">The delay before the rotation begins.</param>
    public void RotateToObject(GameObject obj, float duration, float speed, float delay)
    {
        StartCoroutine(RotateToObjectCoroutine(obj, duration, speed, delay));
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
