using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerInventoryInterface : MonoBehaviour  /// @todo Comment
{
    [Header("Input")]
    // Input actions:
    // swap weapon
    public InputAction swapWeaponAction;
    // reload
    public InputAction reloadAction;
    // fire weapon
    public InputAction fireWeaponAction;
    // aim weapon
    public InputAction aimWeaponAction;
    public InputAction useEquipmentAction;

    [Header("Inventory")]
    public string playerInventoryName = "player";
    public Inventory playerInventory;

    public enum SelectedWeaponType{ Primary, Secondary, None }
    [SerializeField] public SelectedWeaponType selectedWeaponType = SelectedWeaponType.None;
    public Item selectedWeapon;
    public Item selectedEquipment;

    [Serializable]
    public class WeaponModelLink
    {
        public Item weapon;
        public GameObject weaponFirepoint;
        public string animatorBoolName = "";
    }


    [Header("References")]  
    public List<WeaponModelLink> weaponFirepointLinks = new List<WeaponModelLink>();

    public LineRenderer aimLines;
    public Color aimLineStartColor = Color.white;
    public Color aimLineEndColor = Color.white;

    public GameObject aimQuad;

    public Animator playerAnimator;

    public struct AimZone
    {
        public Vector3 forwardLeft;
        public Vector3 forwardRight;
        public Vector3 backwardRight;
        public Vector3 backwardLeft;

        public Vector3 fl { get{ return forwardLeft; } }
        public Vector3 fr { get{ return forwardRight; } }
        public Vector3 br { get{ return backwardRight; } }
        public Vector3 bl { get{ return backwardLeft; } }


        // constructor
        public AimZone(Vector3 forwardLeft, Vector3 forwardRight, Vector3 backwardRight, Vector3 backwardLeft)
        {
            this.forwardLeft = forwardLeft;
            this.forwardRight = forwardRight;
            this.backwardRight = backwardRight;
            this.backwardLeft = backwardLeft;
        }

        // zero constructor
        static public AimZone Zero = new AimZone(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero);

        // get list
        public List<Vector3> GetPoints()
        {
            List<Vector3> points = new List<Vector3>();
            points.Add(forwardLeft);
            points.Add(forwardRight);
            points.Add(backwardRight);
            points.Add(backwardLeft);
            return points;
        }

        //equality operator
        public static bool operator ==(AimZone a, AimZone b)
        {
            return a.forwardLeft == b.forwardLeft && a.forwardRight == b.forwardRight && a.backwardRight == b.backwardRight && a.backwardLeft == b.backwardLeft;
        }
        public static bool operator !=(AimZone a, AimZone b)
        {
            return !(a == b);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        playerInventory = InventoryManager.instance?.GetInventory(playerInventoryName);

        SwapWeapon();
        // select weapon
        SelectWeapon(selectedWeaponType);

        // select equipment
        SelectEquipment();

        aimLines.startColor = aimLineStartColor;
        aimLines.endColor = aimLineEndColor;
    }

    // Update is called once per frame
    void Update()
    {
        // if swap weapon action is pressed, swap weapon
        if (swapWeaponAction.triggered)
        {
            SwapWeapon();
        }

        if (selectedWeapon){
            selectedWeapon.ManualUpdate(gameObject);

            GameObject weaponFirepoint = GetWeaponFirepoint(selectedWeapon);
            Vector3 weaponFirepointPosition = weaponFirepoint.transform.position;
            Vector3 fireDirection = GetComponent<PlayerMovement>().GetMouseAimPlanePoint() - weaponFirepointPosition;

            RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
            if (rangedWeapon)
            {
                // if fire weapon action is pressed and is not auto and is not shooting, shoot weapon || if fire weapon action is current down and is auto, shoot weapon
                if ((fireWeaponAction.triggered && !rangedWeapon.m_isAutomnatic) || (fireWeaponAction.ReadValue<float>() > 0 && rangedWeapon.m_isAutomnatic))
                {
                    if (rangedWeapon.TryShoot(weaponFirepointPosition, fireDirection, gameObject)){
                        // play shoot animation
                        playerAnimator.SetTrigger("Shoot");
                    }
                }

                // if reload action is pressed, reload weapon
                if (reloadAction.triggered)
                {
                    if (rangedWeapon.TryReload(gameObject)){
                        playerAnimator.SetTrigger("Reload");
                    }
                }

                // if aim weapon action is down, aim weapon
                if (aimWeaponAction.ReadValue<float>() > 0)
                {
                    rangedWeapon.TrySetAim(true, gameObject);
                }
                else
                {
                    rangedWeapon.TrySetAim(false, gameObject);
                }

                // stop aiming
                if (swapWeaponAction.triggered)
                {
                    rangedWeapon.TrySetAim(false, gameObject);
                }

            }

            UpdateAimZone();
        }

        if (useEquipmentAction.triggered && selectedEquipment == null)
        {
            SelectEquipment();
        }
        if (selectedEquipment){
            selectedEquipment.ManualUpdate(gameObject);

            if (useEquipmentAction.triggered)
            {
                Equipment equipment = selectedEquipment as Equipment;
                if (equipment)
                {
                    Vector3 spawnDirection = (GetComponent<PlayerMovement>().GetMouseAimPlanePoint() - transform.position).normalized;
                    Vector3 spawnPostion = transform.position + spawnDirection * 1.5f;

                    equipment.TossPrefab(spawnPostion, spawnDirection * 0.5f, gameObject);

                    playerAnimator.SetTrigger("PlaceBeartrap");
                }
            }
        }
    }

    private void UpdateAimZone()
    {
        if (aimLines)
        {
            if (!selectedWeapon) return;

            RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
            if (!rangedWeapon) return;

            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (!playerMovement) return;

            if (!rangedWeapon.m_isAiming || rangedWeapon.m_reloadTimer > 0 || playerMovement.isRolling){
                aimQuad.SetActive(false);
            }
            else{
                aimQuad.SetActive(true);
            }

            // update actual aim zone mesh
            AimZone aimZone = GetAimZone(aimQuad.transform);
            if (aimZone != AimZone.Zero){
                // set the vertices of the aimQuad
                Mesh mesh = aimQuad.GetComponent<MeshFilter>().mesh;
                mesh.SetVertices(aimZone.GetPoints());

                //set tris to match the aimLines
                mesh.SetTriangles(new List<int>() { 0, 1, 2, 0, 2, 3 }, 0);

                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
            }
        }
    }

    public AimZone GetAimZone(Transform _t = null)
    {
        AimZone aimZone = AimZone.Zero;

        if (aimQuad == null) return aimZone;

        RangedWeapon rangedWeapon = selectedWeapon as RangedWeapon;
        if (!rangedWeapon) return aimZone;

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (!playerMovement) return aimZone;

        GameObject firepointObj = GetWeaponFirepoint(selectedWeapon);
        if (!firepointObj) return aimZone;
        
        Vector3 aimPoint = playerMovement.GetMouseAimPlanePoint();

        float currentAimAngle = rangedWeapon.CalcCurrentAimAngle();
        // if nan or inf, set to 0
        if (float.IsNaN(currentAimAngle) || float.IsInfinity(currentAimAngle)) currentAimAngle = 0;

        aimZone.backwardLeft = aimQuad.transform.position + aimQuad.transform.right * -0.1f + aimQuad.transform.forward * 0.1f;
        aimZone.backwardRight = aimQuad.transform.position + aimQuad.transform.right * 0.1f + aimQuad.transform.forward * 0.1f;

        float distFromBackLeftToAimPoint = Vector3.Distance(aimZone.backwardLeft, new Vector3(aimPoint.x, aimZone.backwardLeft.y, aimPoint.z));
        float distFromBackRightToAimPoint = Vector3.Distance(aimZone.backwardRight, new Vector3(aimPoint.x, aimZone.backwardRight.y, aimPoint.z));

        aimZone.forwardLeft = Quaternion.Euler(0, -currentAimAngle, 0) * (aimZone.backwardLeft + (aimQuad.transform.forward * distFromBackLeftToAimPoint) - aimZone.backwardLeft) + aimZone.backwardLeft;
        aimZone.forwardRight = Quaternion.Euler(0, currentAimAngle, 0) * (aimZone.backwardRight + (aimQuad.transform.forward * distFromBackRightToAimPoint) - aimZone.backwardRight) + aimZone.backwardRight;
        aimZone.forwardLeft.y = aimQuad.transform.position.y;
        aimZone.forwardRight.y = aimQuad.transform.position.y;

        if (_t != null)
        {
            //convert coordinates to local space of _t
            aimZone.backwardLeft = _t.InverseTransformPoint(aimZone.backwardLeft);
            aimZone.backwardRight = _t.InverseTransformPoint(aimZone.backwardRight);
            aimZone.forwardLeft = _t.InverseTransformPoint(aimZone.forwardLeft);
            aimZone.forwardRight = _t.InverseTransformPoint(aimZone.forwardRight);
        }

        return aimZone;
    }

    public GameObject GetWeaponFirepoint(Item weapon)
    {
        if (!weapon) return null;
        foreach (WeaponModelLink link in weaponFirepointLinks)
        {
            if (link.weapon.id == weapon.id)
            {
                return link.weaponFirepoint;
            }
        }
        return null;
    }

    public string GetWeaponAnimatorBoolName(Item weapon)
    {
        foreach (WeaponModelLink link in weaponFirepointLinks)
        {
            if (link.weapon.id == weapon.id)
            {
                return link.animatorBoolName;
            }
        }
        return "";
    }

    public void SwapWeapon(){
        // if inventory is null, return
        if (playerInventory == null) return;

        // toggle between primary and secondary weapon
        if (selectedWeaponType == SelectedWeaponType.Primary)
        {
            SelectWeapon(SelectedWeaponType.Secondary);
        } 
        else if (selectedWeaponType == SelectedWeaponType.Secondary)
        {
            SelectWeapon(SelectedWeaponType.Primary);
        }

        // if selected weapon is null, try to select primary, secondary, then none
        if (selectedWeapon == null){
            SelectWeapon(SelectedWeaponType.Primary);
            if (selectedWeapon == null){
                SelectWeapon(SelectedWeaponType.Secondary);
                if (selectedWeapon == null){
                    SelectWeapon(SelectedWeaponType.None);
                }
            }
        }
    }

    public void SelectWeapon(SelectedWeaponType _type){
        selectedWeaponType = _type;

        // if inventory is null, return
        if (playerInventory == null) return;

        // get selected weapon
        selectedWeapon = playerInventory.slots.ElementAtOrDefault((int)selectedWeaponType)?.item;

        // if selected weapon is null, return
        if (selectedWeapon == null) return;

        // set animator
        string animatorBoolName = GetWeaponAnimatorBoolName(selectedWeapon);
        if (playerAnimator && animatorBoolName != "")
        {
            DisableAllAnimatorWeapons();
            playerAnimator.SetBool(animatorBoolName, true);
        }
    }

    public void SelectEquipment(){
        // if inventory is null, return
        if (playerInventory == null) return;

        // get selected equipment
        selectedEquipment = playerInventory.slots.ElementAtOrDefault(2)?.item;

        // if selected equipment is null, return
        if (selectedEquipment == null) return;
    }

    public void DisableAllAnimatorWeapons(){
        if (playerAnimator == null) return;

        foreach (WeaponModelLink link in weaponFirepointLinks)
        {
            string animatorBoolName = link.animatorBoolName;
            if (animatorBoolName != "")
            {
                playerAnimator.SetBool(animatorBoolName, false);
            }
        }
    }

    private void OnEnable() {
        EnableInput();
    }

    private void OnDisable() {
        DisableInput();
    }

    public void EnableInput()
    {
        swapWeaponAction.Enable();
        reloadAction.Enable();
        fireWeaponAction.Enable();
        aimWeaponAction.Enable();
        useEquipmentAction.Enable();
    }

    public void DisableInput()
    {
        swapWeaponAction.Disable();
        reloadAction.Disable();
        fireWeaponAction.Disable();
        aimWeaponAction.Disable();
        useEquipmentAction.Disable();
    }
}
