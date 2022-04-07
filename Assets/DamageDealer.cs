using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{

    [System.Serializable]
    public struct HurtBox
    {
        [SerializeField]
        public Transform position;
        public float radius;
        public float damage;
        public string name;
        public float delay;
        public float duration;
        public ParticleSystem[] particle;
        public GameObject spawnedParticle;
    }
    [SerializeField]
    public List<HurtBox> hurtBoxes = new List<HurtBox>();

    public GameObject hurtPlayerEffect;

    public void EnableHurtBox(string name)
    {
        StartCoroutine(EnableHurtBoxRoutine(name));
    }

    public float GetAttackDuration(string name)
    {
        foreach (HurtBox hb in hurtBoxes)
        {
            if (hb.name == name)
            {
                return hb.duration;
            }
        }
        return 0;
    }
    
    

    IEnumerator EnableHurtBoxRoutine(string name)
    {
        HurtBox hb = hurtBoxes.Find(x => x.name == name);
        yield return new WaitForSeconds(hb.delay);
        
        RaycastHit[] hits = Physics.SphereCastAll(hb.position.position, hb.radius, Vector3.forward, hb.radius,LayerMask.GetMask("Player"));
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                hit.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(hb.damage);
                Debug.Log("Hit player");
                Destroy(Instantiate(hurtPlayerEffect, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up)), 5.0f);
                break;
            }
        }

        if(hb.particle != null)
        {
            foreach (ParticleSystem ps in hb.particle)
            {
                ps.Play();
            }
        }
        if(hb.spawnedParticle != null)
        {
            Destroy(Instantiate(hb.spawnedParticle, hb.position.position, Quaternion.identity), 5.0f);
        }
        
    }

    private void OnDrawGizmosSelected() {
        
        foreach (HurtBox hb in hurtBoxes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hb.position.position, hb.radius);
        }
    }


}
