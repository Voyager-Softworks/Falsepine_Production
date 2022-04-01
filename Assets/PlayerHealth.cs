using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTES:
// Player has health from 0-100, Starting at 100.
// Player must be able to become invulnerable (toggle).
// Player must be able to be healed.
// Player must be able to take damage from enemies.
// Player must be able to die.

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public bool isInvulnerable = false;
    public bool isDead = false;

    [Header("UI")]
    [HideInInspector] public UIScript uiScript;



    // Start is called before the first frame update
    void Start()
    {
        if (uiScript == null) uiScript = FindObjectOfType<UIScript>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();

        //check for health and dead
        if (!isDead && currentHealth <= 0){
            Die();
        }
    }

    public void UpdateUI()
    {
        if (uiScript == null) return;

        uiScript._healthBar.rectTransform.sizeDelta = new Vector2(uiScript._healthBarMaxWidth * (currentHealth / maxHealth), uiScript._healthBar.rectTransform.sizeDelta.y);
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die(){
        isDead = true;
        currentHealth = 0;
    }

    public void Heal(float heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void ToggleInvulnerability()
    {
        isInvulnerable = !isInvulnerable;
    }
}
