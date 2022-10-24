using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationEvents : MonoBehaviour
{
    private PlayerInventoryInterface pii;

    // Start is called before the first frame update
    void Start()
    {
        pii = GetComponentInParent<PlayerInventoryInterface>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // public void StartReload(){
    //     if (pii.selectedWeapon){
    //         RangedWeapon rw = pii.selectedWeapon as RangedWeapon;
    //         if (rw){
    //             rw.TryStartReload(gameObject);
    //         }
    //     }
    // }

    public void SingleReload(){
        Debug.Log("Reload");

        if (pii.selectedWeapon){
            RangedWeapon rw = pii.selectedWeapon as RangedWeapon;
            if (rw){
                rw.TryReload(gameObject);
            }
        }
    }

    public void EndReload(){
        if (pii.selectedWeapon){
            RangedWeapon rw = pii.selectedWeapon as RangedWeapon;
            if (rw){
                rw.TryEndReload(gameObject);
            }
        }
    }
}
