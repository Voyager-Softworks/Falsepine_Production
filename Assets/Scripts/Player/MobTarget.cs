using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using System.Linq;
public class MobTarget : MonoBehaviour  /// @todo Comment
{
    public int maxConcurrentAttackers = 3;
    public float influenceRadius = 10;
    public List<NodeAI_Agent> attackers = new List<NodeAI_Agent>();

    public bool RegisterAttacker(NodeAI_Agent attacker)
    {
        if(attackers.Contains(attacker))
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
            a => a == null || a.GetComponent<HealthScript>().isDead || Vector3.Distance(a.transform.position, transform.position) > influenceRadius).ToList().ForEach(
                a => attackers.Remove(a));
    }
}
