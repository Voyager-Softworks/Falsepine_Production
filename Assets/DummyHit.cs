using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyHit : MonoBehaviour
{
    EnemyHealth m_enemyHealth;
    Animator m_animation;

    // Start is called before the first frame update
    void Start()
    {
        m_enemyHealth = GetComponent<EnemyHealth>();
        m_animation = GetComponent<Animator>();

        // bind DoHit to the Damage event
        m_enemyHealth.Damage += DoHit;
    }

    private void OnDisable() {
        // unbind DoHit from the Damage event
        m_enemyHealth.Damage -= DoHit;
    }

    public void DoHit(Health_Base.DamageStat _stat)
    {
        // play the hit animation
        m_animation.SetTrigger("Hit");
    }
}
