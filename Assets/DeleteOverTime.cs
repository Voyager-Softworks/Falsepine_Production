using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOverTime : MonoBehaviour
{
    public float m_timeToLive = 4.5f;
    private float m_timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= m_timeToLive){
            Destroy(gameObject);
        }
    }
}
