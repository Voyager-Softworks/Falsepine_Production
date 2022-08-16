using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a trail behind a bullet.
/// </summary>
public class BulletTrail : MonoBehaviour
{
    public bool fadeOverTime = true;
    public float fadeRate = 1.0f;

    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOverTime)
        {
            Color color = lineRenderer.startColor;
            color.a -= Time.deltaTime * fadeRate;
            color.a = Mathf.Clamp01(color.a);
            lineRenderer.startColor = color;

            // once start color is 0, fade end color to 0
            if (color.a <= 0)
            {
                color = lineRenderer.endColor;
                color.a -= Time.deltaTime * fadeRate;
                color.a = Mathf.Clamp01(color.a);
                lineRenderer.endColor = color;
            }
        }

        // if end color is 0, destroy this object
        if (lineRenderer.endColor.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sets positions of the line renderer.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void SetPositions(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}
