using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthScript : MonoBehaviour
{
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    // public float healthRegen = 0.5f;
    // public float healthRegenDelay = 5.0f;
    // public float healthRegenDelayTimer = 0f;
    public bool isDead = false;
    public bool isInvincible = false;
    //public bool isRegenerating = false;

    public UnityEvent OnDeath;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if (isRegenerating)
        // {
        //     if (healthRegenDelayTimer < healthRegenDelay)
        //     {
        //         healthRegenDelayTimer += Time.deltaTime;
        //     }
        //     else
        //     {
        //         healthRegenDelayTimer = 0f;
        //         currentHealth += healthRegen;
        //         if (currentHealth > maxHealth)
        //         {
        //             currentHealth = maxHealth;
        //         }
        //     }
        // }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;
        if (isDead) return;

        currentHealth -= damage;
        GetComponent<NodeAI.NodeAI_Agent>().SetFloat("Health", currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die(){
        isDead = true;
        currentHealth = 0;

        OnDeath.Invoke();
        GetComponent<NodeAI.NodeAI_Agent>().SetState("Dead");
        GetComponentInChildren<Animator>().SetBool("Dead", true);

    }

    public void Heal(float heal)
    {
        if (isDead) return;

        currentHealth += heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void SetInvincible(bool _invincible)
    {
        isInvincible = _invincible;
    }

    // public void SetRegenerating(bool _regenerating)
    // {
    //     isRegenerating = _regenerating;
    // }


}
