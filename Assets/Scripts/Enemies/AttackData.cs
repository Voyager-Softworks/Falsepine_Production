using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NodeAI/Actions/Attack")]
public class AttackData : ScriptableObject  /// @todo Comment
{
    public enum AttackType
    {
        Melee,
        Ranged,
        AOE
    }
    public string animationTrigger;
    public AudioClip attackSound;
    [SerializeField]
    public List<AttackPhase> attackPhases = new List<AttackPhase>();
    [System.Serializable]
    public class AttackPhase
    {
        [Header("General")]
        public AttackType attackType;
        public float attackRange;
        public float attackDelay;
        public float attackDuration;
        public float attackDamage;
        public float attackStunDuration;
        [Header("Projectile")]
        public float projectileSpeed;
        public bool projectileContinuousFire;
        public float projectileFireDelay;
        [SerializeReference]
        public GameObject projectile;
        [Header("AOE")]
        [SerializeReference]
        public GameObject AOEeffect;
        [Header("Rotation")]
        public float turnDuration;
        public float turnSpeed;
        public float turnDelay;
        [Header("Translation")]
        public float translationDuration;
        public float translationSpeed;
        public float translationDelay;
    }
}
