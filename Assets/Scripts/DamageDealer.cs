using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using System;
using UnityEngine.SceneManagement;

/// <summary>
///  Script that handles dealing damage to the player through melee, ranged, and AOE attacks.
/// </summary>
public class DamageDealer : MonoBehaviour
{

    public List<Collider> m_hurtBoxes = new List<Collider>(); ///< Colliders used to detect when the player is hit by an attack

    public GameObject m_hurtPlayerEffect; ///< Particle _effect spawned when the player is hurt

    public GameObject m_indicatorPrefab; ///< Indicator that shows where the attack will hit
    public struct IndicatorMaterial
    {
        public Material m_material;
        public string m_keyword;
    }

    public List<IndicatorMaterial> m_indicatorMaterials = new List<IndicatorMaterial>(); ///< Materials that can be used for the indicator

    public float m_damage = 10f; ///< Damage done by the attack
    public int m_attkNum = 1; ///< Number of Attacks

    void Start()
    {
        GetComponent<EnemyHealth>().Death += (Health_Base.DeathContext context) =>
        {
            foreach (var hurtBox in m_hurtBoxes)
            {
                hurtBox.enabled = false;
            }
        };
    }


    /// <summary>
    ///  Coroutine to execute a melee attack on the player.
    /// </summary>
    /// <param name="_dmg">The damage of the attack.</param>
    /// <param name="_delay">The amount of time before the damage is dealt.</param>
    /// <param name="_duration">The length of time during which the hurtboxes are checked for collision with the player.</param>
    /// <param name="_stunDuration">The length of time to stun the player for if they are hit.</param>
    /// <returns></returns>
    IEnumerator MeleeAttackCoroutine(float _dmg, float _delay, float _duration, float _stunDuration)
    {
        yield return new WaitForSeconds(_delay); // Waitfor the duration of the delay.

        //Enable hurtboxes
        foreach (Collider hurtBox in m_hurtBoxes)
        {
            hurtBox.enabled = true;
        }
        float timer = 0f;
        bool playerHit = false;

        //For the duration of the attack, check for the player in hurtboxes, and hurt them once.
        while (timer < _duration && !playerHit)
        {
            timer += Time.deltaTime;
            foreach (Collider hurtBox in m_hurtBoxes)
            {
                if (hurtBox.enabled)
                {
                    RaycastHit[] hits = Physics.BoxCastAll(hurtBox.bounds.center, hurtBox.bounds.extents, hurtBox.transform.forward, hurtBox.transform.rotation, 0.5f);
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            hit.collider.GetComponent<PlayerHealth>().TakeDamage(_dmg);
                            hit.collider.GetComponent<PlayerHealth>().Stun(_stunDuration);
                            Instantiate(m_hurtPlayerEffect, hit.point, Quaternion.identity);
                            playerHit = true;
                            break;
                        }
                    }
                }
            }
            yield return null;
        }
        foreach (Collider hurtBox in m_hurtBoxes)
        {
            hurtBox.enabled = false;
        }
    }

    /// <summary>
    ///  Coroutine to execute an AOE attack on the player.
    /// </summary>
    /// <param name="_dmg">The damage of the attack.</param>
    /// <param name="_delay">The amount of time before the damage is dealt.</param>
    /// <param name="_radius">The _radius of the AOE attack.</param>
    /// <param name="_effect">The _effect to spawn when the player is hit.</param>
    /// <param name="_stunDuration">The length of time to stun the player for if they are hit.</param>
    /// <returns></returns>
    IEnumerator AOEAttackCoroutine(float _dmg, float _delay, float _radius, GameObject _effect, Vector2 _offset, float _stunDuration)
    {
        yield return new WaitForSeconds(_delay);
        Vector3 offsetVector = transform.forward * _offset.y + transform.right * _offset.x;
        Destroy(Instantiate(_effect, transform.position + offsetVector, Quaternion.identity), 20.0f);
        RaycastHit[] hits = Physics.SphereCastAll(transform.position + offsetVector, _radius, transform.forward, 0.5f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<PlayerHealth>().TakeDamage(_dmg);
                hit.collider.GetComponent<PlayerHealth>().Stun(_stunDuration);
                Instantiate(m_hurtPlayerEffect, hit.point, Quaternion.identity);
                break;
            }
        }
    }
    /// <summary>
    ///  Coroutine to display an attack indicator for AOE attacks.
    /// </summary>
    /// <param name="_delay">The amount of time before the attack hits. (Seconds)</param>
    /// <param name="_radius">The damage _radius of the attack.</param>
    /// <param name="_offset">The amount to _offset the position of the attack indicator by in respect to the origin of the enemy.</param>
    /// <param name="_playerDirectionFunction">A delegate used to find the direction of the player relative to the enemy</param>
    /// <param name="_translationSpeed">The _speed at which the enemy is moving towards the player.</param>
    /// <param name="_translationDuration">The _duration for which the enemy will move towards the player during this attack phase.</param>
    /// <param name="_attackColor">The color to make the attack indicator circle.</param>
    /// <param name="_indicatorDuration">The _duration to display the indicator for prior to the attack hitting</param>
    /// <returns></returns>
    public IEnumerator IndicatorCoroutine(float _delay, float _radius, Vector2 _offset, Func<Vector3> _playerDirectionFunction, float _translationSpeed, float _translationDuration, Color _attackColor, float _indicatorDuration)
    {
        yield return new WaitForSeconds(_delay - _indicatorDuration); //Wait until it is time to begin displaying the indicator

        Vector3 offsetVector = transform.forward * _offset.y + transform.right * _offset.x; //Get the _offset position
        m_indicatorPrefab = GameObject.FindGameObjectWithTag("Indicator"); //Get the indicator prefab
        GameObject indicator = Instantiate(m_indicatorPrefab, transform.position + offsetVector + (_playerDirectionFunction() * (_translationSpeed * _translationDuration)) - Vector3.up, Quaternion.Euler(90, 0, 0)); //Instantiate the indicator
        indicator.tag = "Untagged"; //Remove the tag from the indicator
        float t = 0.0f; //Create the timer


        //Set the properties of the decal projector
        DecalProjector decalProjector = indicator.GetComponent<DecalProjector>();
        Material oldMat = decalProjector.material;
        decalProjector.material = new Material(oldMat); //Make a new instance of the material
        decalProjector.material.SetColor("_BaseColor", _attackColor);

        //Maths to get around Unity's strange storing of HDR colors
        Color emissiveColor = oldMat.GetColor("_EmissiveColorHDR");
        var maxColComponent = emissiveColor.maxColorComponent;
        byte maxOverExposedColor = 191;
        var factor = maxOverExposedColor / maxColComponent;
        float intensity = Mathf.Log(255f / factor) / Mathf.Log(2f);
        Color newEmissiveColor = new Color(_attackColor.r * intensity, _attackColor.g * intensity, _attackColor.b * intensity, _attackColor.a);
        decalProjector.material.SetColor("_EmissiveColor", newEmissiveColor);
        decalProjector.material.SetInt("_UseEmissiveIntensity", 1);
        decalProjector.material.SetFloat("_EmissiveIntensity", oldMat.GetFloat("_EmissiveIntensity"));


        decalProjector.size = Vector3.zero;

        //Animate the size of the indicator
        Vector3 startSize = new Vector3(0.0f, 0.0f, 2.0f);
        Vector3 endSize = new Vector3(_radius * 2f, _radius * 2f, 2.0f);
        while (t < _indicatorDuration)
        {
            Vector3 groundPos = transform.position;
            groundPos.y = 0.0f;
            offsetVector = transform.forward * _offset.y + transform.right * _offset.x;
            indicator.transform.position = (groundPos + offsetVector) + (transform.forward * _translationSpeed * _translationDuration * (1 - ((t + (_delay - _indicatorDuration)) / _delay))) + (Vector3.up);

            t += Time.deltaTime;
            decalProjector.size = Vector3.Lerp(startSize, endSize, t / _indicatorDuration);
            yield return null;
        }
        Destroy(indicator);

    }


    /// <summary>
    ///  Coroutine to execute a ranged attack on the player.
    /// </summary>
    /// <param name="_projectile">The _projectile to spawn.</param>
    /// <param name="_delay">The amount of time before the damage is dealt.</param>
    /// <param name="_speed">The _speed of the _projectile.</param>
    /// <param name="_spawnPoint">The _spawnPoint of the _projectile.</param>
    /// <param name="_aimAtPlayer">Whether or not the _projectile should aim at the player.</param>
    /// <returns></returns>
    IEnumerator RangedAttackCoroutine(GameObject _projectile, float _delay, float _speed, Transform _spawnpoint, bool _aimAtPlayer = false)
    {
        yield return new WaitForSeconds(_delay);
        GameObject proj = Instantiate(_projectile, _spawnpoint.position, _spawnpoint.rotation);
        if (!_aimAtPlayer)
            proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * _speed;
        else
        {
            Vector3 dir = (GameObject.FindGameObjectWithTag("Player").transform.position - proj.transform.position).normalized;
            proj.transform.rotation = Quaternion.LookRotation(dir);
            proj.GetComponent<Rigidbody>().velocity = dir * _speed;
        }
        Destroy(proj, 20.0f);
    }

    /// <summary>
    ///  Coroutine to execute a ranged attack on the player.
    /// </summary>
    /// <param name="_projectile">The _projectile to spawn.</param>
    /// <param name="_delay">The amount of time before the damage is dealt.</param>
    /// <param name="_speed">The _speed of the _projectile.</param>
    /// <param name="_spawnPoint">The _spawnPoint of the _projectile.</param>
    /// <param name="_duration">The _duration for which projectiles should be spawned.</param>
    /// <param name="_waitBetweenSpawns">The amount of time between each _projectile spawn.</param>
    /// <returns></returns>
    IEnumerator RangedAttackCoroutine(GameObject _projectile, float _delay, float _speed, Transform _spawnPoint, float _duration, float _waitBetweenSpawns)
    {
        yield return new WaitForSeconds(_delay);
        for (int i = 0; i < _duration / _waitBetweenSpawns; i++)
        {
            GameObject proj = Instantiate(_projectile, _spawnPoint.position, _spawnPoint.rotation);
            proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * -_speed;
            Destroy(proj, 20.0f);
            yield return new WaitForSeconds(_waitBetweenSpawns);
        }
    }

    /// <summary>
    ///  Do a melee attack on the player.
    /// </summary>
    /// <param name="_dmg">The Damage</param>
    /// <param name="_delay">The amount of time before the damage is dealt.</param>
    /// <param name="_duration">The length of time during which the hurtboxes are checked for collision with the player.</param>
    /// <param name="_stunDuration">The length of time to stun the player for if they are hit.</param>
    public void MeleeAttack(float _dmg, float _delay, float _duration, float _stunDuration)
    {
        StartCoroutine(MeleeAttackCoroutine(_dmg, _delay, _duration, _stunDuration));
    }

    /// <summary>
    ///  Do an AOE attack on the player.
    /// </summary>
    /// <param name="_dmg">The Damage</param>
    /// <param name="_delay">The amount of time before the damage is dealt.</param>
    /// <param name="_radius">The _radius of the AOE attack.</param>
    /// <param name="_effect">The _effect to spawn when the player is hit.</param>
    /// <param name="_stunDuration">The length of time to stun the player for if they are hit.</param>
    public void AOEAttack(float _dmg, float _delay, float _radius, GameObject _effect, Vector2 _offset, float _stunDuration)
    {
        StartCoroutine(AOEAttackCoroutine(_dmg, _delay, _radius, _effect, _offset, _stunDuration));
    }

    /// <summary>
    ///  Display the AOE indicator using the coroutine.
    /// </summary>
    /// <param name="_delay">The amount of time before the attack hits. (Seconds)</param>
    /// <param name="_radius">The damage _radius of the attack.</param>
    /// <param name="_offset">The amount to _offset the position of the attack indicator by in respect to the origin of the enemy.</param>
    /// <param name="_playerDirectionFunction">A delegate used to find the direction of the player relative to the enemy</param>
    /// <param name="_translationSpeed">The _speed at which the enemy is moving towards the player.</param>
    /// <param name="_translationDuration">The _duration for which the enemy will move towards the player during this attack phase.</param>
    /// <param name="_attackColor">The color to make the attack indicator circle.</param>
    /// <param name="_indicatorDuration">The _duration to display the indicator for prior to the attack hitting</param>
    public void DisplayIndicator(float _delay, float _radius, Vector2 _offset, Func<Vector3> _playerDirectionFunction, float _translationSpeed, float _translationDuration, Color _color, float _indicatorDuration)
    {
        StartCoroutine(IndicatorCoroutine(_delay, _radius, _offset, _playerDirectionFunction, _translationSpeed, _translationDuration, _color, _indicatorDuration));
    }


    /// <summary>
    ///  Do a ranged attack on the player.
    /// </summary>
    /// <param name="_projectile">The _projectile to spawn.</param>
    /// <param name="_delay">The amount of time before the damage is dealt.</param>
    /// <param name="_speed">The _speed of the _projectile.</param>
    /// <param name="_spawnPoint">The _spawnPoint of the _projectile.</param>
    /// <param name="_aimAtPlayer">Whether or not the _projectile should aim at the player.</param>
    public void RangedAttack(GameObject _projectile, float _delay, float _speed, Transform _spawnPoint, bool _aimAtPlayer = false)
    {
        StartCoroutine(RangedAttackCoroutine(_projectile, _delay, _speed, _spawnPoint, _aimAtPlayer));
    }

    /// <summary>
    ///  Do a ranged attack on the player.
    /// </summary>
    /// <param name="_projectile">The _projectile to spawn.</param>
    /// <param name="_delay">The amount of time before the damage is dealt.</param>
    /// <param name="_speed">The _speed of the _projectile.</param>
    /// <param name="_spawnPoint">The _spawnPoint of the _projectile.</param>
    /// <param name="_duration">The _duration for which projectiles should be spawned.</param>
    /// <param name="_waitBetweenSpawns">The amount of time between each _projectile spawn.</param>
    public void RangedAttack(GameObject _projectile, float _delay, float _speed, Transform _spawnPoint, float _duration, float _waitBetweenSpawns)
    {
        StartCoroutine(RangedAttackCoroutine(_projectile, _delay, _speed, _spawnPoint, _duration, _waitBetweenSpawns));
    }

    /// <summary>
    /// Cancels all currently running attack coroutines.
    /// </summary>
    /// <remarks>
    /// This is used to stop attacks from continuing to execute after the enemy has been defeated, or otherwise should not be executing attacks it has queued.</remarks>
    public void CancelAttack()
    {
        StopAllCoroutines();
    }






}
