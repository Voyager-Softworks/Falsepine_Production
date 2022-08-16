using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "NodeAI/Actions/Attack")]
/// <summary>
/// A data format for creating attacks for enemies.
/// </summary>
public class AttackData : ScriptableObject
{
    /// <summary>
    /// The type of attack.
    /// </summary>
    public enum AttackType
    {
        Melee, ///< A Melee (Close range, physical) attack.
        Ranged, ///< A Ranged (Long range, projectile) attack.
        AOE ///< An Area of Effect (Radius) attack.
    }
    public string animationTrigger; ///< The name of the animation trigger to play.
    public AudioClip attackSound; ///< The sound to play when the attack is performed.
    [SerializeField]
    public List<AttackPhase> attackPhases = new List<AttackPhase>(); ///< The attack phases.
    [System.Serializable]
    /// <summary>
    ///  A class representing one phase of an attack.
    /// </summary>
    /// <remarks>
    /// An attack may be made up of multiple phases.
    /// </remarks>
    public class AttackPhase
    {
        [Header("General")]
        public AttackType attackType; ///< The type of attack.
        public float attackRange; ///< The range of the attack.
        public float attackDelay; ///< The delay between the start of the attack and the actual attack hitting.
        public float attackDuration; ///< The duration for which to check for a hit.
        public float attackDamage; ///< The amount of damage to deal.
        public float attackStunDuration; ///< The duration for which to stun the target.
        [Header("Projectile")]
        public float projectileSpeed; ///< The speed of the projectile.
        public bool projectileContinuousFire; ///< Whether projectiles should be fired continuously throughout the duration of the attack.
        public float projectileFireDelay; ///< The delay between firing projectiles, if continuous fire is enabled.
        [SerializeReference]
        public GameObject projectile; ///< The projectile to fire.
        [Header("AOE")]
        [SerializeReference]
        public GameObject AOEeffect; ///< The effect to play when the attack is performed.
        public Vector2 AOEspawnOffset; ///< The offset from the agent's position to spawn the effect.
        public Color AOEindicatorColor; ///< The color of the indicator.
        public float AOEindicatorDuration; ///< The amount of time for which the indicator should be visible.

        [Header("Rotation")]
        public float turnDuration; ///< The duration for which to turn the agent towards the target.
        public float turnSpeed; ///< The speed at which to turn the agent towards the target.
        public float turnDelay; ///< The delay between the start of the attack the start of the turn.
        [Header("Translation")]
        public float translationDuration; ///< The duration for which to move the agent towards the target.
        public float translationSpeed; ///< The speed at which to move the agent towards the target.
        public float translationDelay; ///< The delay between the start of the attack and the start of the translation.
    }
}
