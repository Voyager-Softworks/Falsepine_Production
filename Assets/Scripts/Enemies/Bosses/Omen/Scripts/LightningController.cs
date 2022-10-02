using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

public class LightningController : MonoBehaviour
{
    public BossEnemyHealth bossEnemyHealth;
    public GameObject lightningPrefab;

    public float spawnRadius = 10.0f;

    public float spawnDelay = 1.0f;
    float spawnTimer = 0.0f;

    public float spawnDelayJitter = 0.5f;

    [Range(0.0f, 1.0f)]
    public float stormIntensityMultiplier = 1.0f;
    float stormIntensity = 0.0f;

    public bool isStormActive = false;

    public AudioController stormAmbienceController;
    public Volume stormVolume;

    public VisualEffect rainEffect;
    public Light stormLight;
    public float clearLightIntensity = 77585f;
    public float stormLightIntensity = 50000f;

    public DecalProjector wetnessDecalProjector;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (bossEnemyHealth.m_currentHealth > 1.0f)
            stormIntensityMultiplier = 1f - (bossEnemyHealth.m_currentHealth / bossEnemyHealth.m_maxHealth);
        else
            stormIntensityMultiplier = 0f;

        stormIntensity = Mathf.Lerp(stormIntensity, stormIntensityMultiplier, Time.deltaTime / 2.0f);
        stormLight.intensity = Mathf.Lerp(clearLightIntensity, stormLightIntensity, stormIntensity * 2.0f);
        stormVolume.weight = Mathf.Clamp(stormIntensity * 1.5f, 0.0f, 1.0f);

        stormAmbienceController.audioChannels[0].volume = Mathf.Clamp(stormIntensity, 0f, 0.33f) * 3.0f;
        stormAmbienceController.audioChannels[1].volume = (Mathf.Clamp(stormIntensity, 0.33f, 0.66f) - 0.33f) * 3.0f;
        stormAmbienceController.audioChannels[2].volume = (Mathf.Clamp(stormIntensity, 0.66f, 1.0f) - 0.66f) * 3.0f;

        rainEffect.SetFloat("AmountMult", (Mathf.Clamp(stormIntensity, 0.33f, 0.66f) - 0.33f) * 3.0f);

        if (stormIntensity > 0.33f)
        {
            wetnessDecalProjector.fadeFactor = Mathf.Max(wetnessDecalProjector.fadeFactor, (Mathf.Clamp(stormIntensity, 0.33f, 0.66f) - 0.33f) * 3.0f);
        }
        else
        {
            if (wetnessDecalProjector.fadeFactor > 0.0f)
            {
                wetnessDecalProjector.fadeFactor = Mathf.Max(0.0f, wetnessDecalProjector.fadeFactor - (Time.deltaTime * 0.05f));
            }
        }


        if (stormIntensity > 0.66f)
        {
            if (spawnTimer <= 0.0f)
            {
                Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
                spawnPosition.y = transform.position.y;
                GameObject lightningStrike = Instantiate(lightningPrefab, spawnPosition, Quaternion.identity);
                spawnTimer = (spawnDelay + Random.Range(-spawnDelayJitter, spawnDelayJitter)) * (1.0f - ((Mathf.Clamp(stormIntensity, 0.66f, 1.0f) - 0.66f) * 2f));
            }
            else
            {
                spawnTimer -= Time.deltaTime;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
