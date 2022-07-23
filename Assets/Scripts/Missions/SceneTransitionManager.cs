using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
///  Script responsible for handling player entry and exit from mission scenes.
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    
    public Transform exitTransform; ///< The transform of the exit point.
    public Rect exitBounds; ///< The bounds of the exit point.
    
    Rect bounds;
    
    bool canMoveToNextScene = true; ///< Whether or not the player can move to the next scene.
    
    /// <summary>
    /// Allows the player to move to the next scene.
    /// </summary>
    public void UnlockNextScene()
    {
        canMoveToNextScene = true;
        GameObject.FindObjectOfType<MissionSequencer>().currentSequence.Dequeue(); // Remove the current scene from the sequence, as it has been completed.
    }

    /// <summary>
    /// Checks if the player can move to the next scene.
    /// </summary>
    /// @todo
    ///    <list type="bullet">
    ///      <item>
    ///       Add in a transition sound effect.
    ///     </item>
    ///   </list>
    /// @bug
    ///   <list type="bullet">
    ///    <item>
    ///      There may be some sort of issue with the bounds calculation not being correct, causing the trigger zone to not be where the gizmo shows.
    ///      this needs further testing to confirm.
    ///    </item>
    public void Update()
    {
        if (canMoveToNextScene)
        {
            // Check if the player is in the bounds of the exit point.
            Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            //get exit bounds scaled, transformed, and rotated to the exit transform
            bounds = new Rect(exitBounds.x + exitTransform.position.x, exitBounds.y + exitTransform.position.z, exitBounds.width, exitBounds.height);
            //rotate bounds to match exit transform rotation
            bounds = RotateRect(bounds, exitTransform.rotation.eulerAngles.y);
            //scale bounds to match exit transform scale
            bounds = ScaleRect(bounds, exitTransform.lossyScale);

            if (bounds.Contains(new Vector2(playerPosition.x, playerPosition.z)))
            {
                if(GameObject.FindObjectOfType<MissionSequencer>().currentSequence.Count > 0)
                {
                    Debug.Log("Moving to next scene.");
                    // Move to the next scene.
                    StartCoroutine(FadeToBlack(GameObject.FindObjectOfType<MissionSequencer>().currentSequence.Peek()));
                     // Load the next scene in the sequence.
                    canMoveToNextScene = false; // Prevent the player from moving to the next scene again.
                }
                else
                {
                    Debug.Log("Moving to Town scene.");
                    // Move to the next scene.
                    StartCoroutine(FadeToBlack("TownScene")); // Load the end scene.
                    canMoveToNextScene = false;
                }
            }
        }
    }

    /// <summary>
    ///  Coroutine to fade the screen to black and load the next scene.
    /// </summary>
    /// <param name="scene">The name of the Scene to load</param>
    /// @attention
    /// This uses the Fade object already present in the UI, but does not interact with the script which is meant to control this fade ui.
    /// This is a temporary solution, and a more robust solution should be discussed with Keane.
    public IEnumerator FadeToBlack(string scene)
    {
        //Fade to black.
        Image i = GameObject.Find("Fade").GetComponent<Image>();
        while(i.color.a < 1)
        {
            i.color = new Color(0, 0, 0, i.color.a + 0.01f);
            yield return new WaitForSeconds(0.01f);
        }
        //Load the next scene.
        SceneManager.LoadSceneAsync(scene);
    }


    /// <summary>
    ///  Scales a rectangle.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    Rect ScaleRect(Rect rect, Vector3 scale)
    {
        return new Rect(rect.x, rect.y, rect.width * scale.x, rect.height * scale.y);
    }

    /// <summary>
    ///  Rotates a rectangle.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    Rect RotateRect(Rect rect, float angle)
    {
        //rotate the rect around the center of the rect
        Vector2 center = new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
        Vector2 newCenter = RotatePoint(center, angle);
        //rotate the rect
        Rect rotatedRect = new Rect(newCenter.x - rect.width / 2, newCenter.y - rect.height / 2, rect.width, rect.height);
        return rotatedRect;
    }

    /// <summary>
    ///  Rotates a point around a center point.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    Vector2 RotatePoint(Vector2 point, float angle)
    {
        //rotate the point around the origin
        float radians = angle * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        float tx = point.x;
        float ty = point.y;
        point.x = (cos * tx) - (sin * ty);
        point.y = (sin * tx) + (cos * ty);
        return point;
    }

    /// <summary>
    ///  Draws the bounds of the exit point.
    /// </summary>
    void OnDrawGizmos()
    {
        bounds = new Rect(exitBounds.x + exitTransform.position.x, exitBounds.y + exitTransform.position.z, exitBounds.width, exitBounds.height);
        //rotate bounds to match exit transform rotation
        bounds = RotateRect(bounds, exitTransform.rotation.eulerAngles.y);
        //scale bounds to match exit transform scale
        bounds = ScaleRect(bounds, exitTransform.lossyScale);
        // Draw the exit point bounds.
        Gizmos.color = canMoveToNextScene ? Color.green : Color.red;
        Gizmos.matrix = exitTransform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(exitBounds.width, 0, exitBounds.height));
        Gizmos.matrix = Matrix4x4.identity;

    }
}
