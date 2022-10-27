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
        if (pii == null)
        {
            pii = GetComponentInParent<PlayerInventoryInterface>();
        }
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

        if (pii != null && pii.selectedWeapon){
            RangedWeapon rw = pii.selectedWeapon as RangedWeapon;
            if (rw){
                rw.TryReload(gameObject);
            }
        }
    }

    public void EndReload(){
        if (pii != null && pii.selectedWeapon){
            RangedWeapon rw = pii.selectedWeapon as RangedWeapon;
            if (rw){
                rw.TryEndReload(gameObject);
            }
        }
    }

    public void ThrowLetGo(){
        if (pii != null){
            pii.ThrowLetGo();
        }
    }
}
