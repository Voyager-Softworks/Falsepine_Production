using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using System.Linq;
/// <summary>
///  Script which handles directed attacking. 
/// </summary>
/// <remarks>
/// This script is attached to the player.
/// It is responsible for making sure that only a certain number of enemies attempt to attack the player at once, even when many enemies are pursuing them.
/// </remarks>
public class MobTarget : MonoBehaviour
{
    public int maxConcurrentAttackers = 3; //< The maximum number of enemies that can attack the player at once.
    public float influenceRadius = 10; //< The radius of the influence sphere.
    public List<NodeAI_Agent> attackers = new List<NodeAI_Agent>(); //< The list of attackers.


    /// <summary>
    /// If the attacker is already registered, return true. If the attacker is not already registered
    /// and the number of attackers is less than the maximum allowed, add the attacker to the list and
    /// return true. Otherwise, return false
    /// </summary>
    /// <param name="NodeAI_Agent">The attacker that is trying to register.</param>
    /// <returns>
    /// A boolean value.
    /// </returns>
    public bool RegisterAttacker(NodeAI_Agent attacker)
    {
        if (attackers.Contains(attacker))
        {
            return true;
        }
        if (attackers.Count < maxConcurrentAttackers)
        {
            attackers.Add(attacker);
            return true;
        }
        return false;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        attackers.Where(
            a => a == null || a.GetComponent<EnemyHealth>().hasDied || Vector3.Distance(a.transform.position, transform.position) > influenceRadius).ToList().ForEach(
                a => attackers.Remove(a));
    }
}
