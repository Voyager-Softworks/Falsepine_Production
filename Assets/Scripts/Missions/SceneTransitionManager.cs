using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///  Script responsible for handling player entry and exit from mission scenes.
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    public Transform exitTransform; ///< The transform of the exit point.
    public Rect exitBounds; ///< The bounds of the exit point.
    
    bool canMoveToNextScene = false; ///< Whether or not the player can move to the next scene.
    
    /// <summary>
    /// Allows the player to move to the next scene.
    /// </summary>
    public void UnlockNextScene()
    {
        canMoveToNextScene = true;
    }

    /// <summary>
    /// Checks if the player can move to the next scene.
    /// </summary>
    /// @todo
    ///    <list type="bullet">
    ///      <item>
    ///       Test this to make sure it works.
    ///     </item>
    ///   </list>
    public void Update()
    {
        if (canMoveToNextScene)
        {
            // Check if the player is in the bounds of the exit point.
            Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            if (playerPosition.x > exitBounds.xMin && playerPosition.x < exitBounds.xMax && playerPosition.y > exitBounds.yMin && playerPosition.y < exitBounds.yMax)
            {
                // Move to the next scene.
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
