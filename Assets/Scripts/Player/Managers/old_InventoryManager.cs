using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class old_InventoryManager : MonoBehaviour
{
    public InputAction openBagAction;
    public InputAction openJournalAction;
    public InputAction closeAction;

    public InputAction hotkey1Action;
    public InputAction hotkey2Action;
    public InputAction hotkey3Action;


    [Header("UI Sounds")]
    public AudioClip openBagSound;
    public AudioClip closeBagSound;
    public AudioClip openJournalSound;
    public AudioClip closeJournalSound;

    private UIScript _uiScript;
    private AudioSource _audioSource;
    private Animator _animator;

    //this should be replaced with more complicated system if doing full prod
    [Serializable]
    public class InventoryItem{
        public string name;
        public int hotkey;
        public int amount;
        public int maxAmount;
        public int currentSlot;
        public Sprite icon;
        public GameObject prefab;
        public float placeDistance;
        public float placeDelay;
        public Transform transOnPlayer;
    }

    public InventoryItem bearTraps = new InventoryItem();
    public InventoryItem baitMeat = new InventoryItem();
    public InventoryItem baitBird = new InventoryItem();



    // Start is called before the first frame update
    void Start()
    {
        openBagAction.Enable();
        openJournalAction.Enable();
        closeAction.Enable();
        
        hotkey1Action.Enable();
        hotkey2Action.Enable();
        hotkey3Action.Enable();

        openBagAction.performed += ctx => ToggleBag();
        openJournalAction.performed += ctx => ToggleJournal();
        closeAction.performed += ctx => CloseAll(true);

        hotkey1Action.performed += HotkeyPressed;
        hotkey2Action.performed += HotkeyPressed;
        hotkey3Action.performed += HotkeyPressed;

        if (_uiScript == null) _uiScript = FindObjectOfType<UIScript>();
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
    }

    private void OnDestroy()
    {
        DisableInput();
    }

    private void OnDisable() {
        DisableInput();
    }

    public void DisableInput()
    {
        openBagAction.Disable();
        openJournalAction.Disable();
        closeAction.Disable();

        hotkey1Action.Disable();
        hotkey2Action.Disable();
        hotkey3Action.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdatePlayerPrefab();
        UpdateBagUI();
        UpdateHotbarUI();
    }

    private void UpdatePlayerPrefab()
    {
        if (bearTraps.amount <= 0)
        {
            if (bearTraps.transOnPlayer) bearTraps.transOnPlayer.localScale = Vector3.zero;
        }
        else
        {
            if (bearTraps.transOnPlayer) bearTraps.transOnPlayer.localScale = Vector3.one;
        }
    }

    private void UpdateBagUI(){
        if (_uiScript == null || _uiScript.bagUIList == null || _uiScript.bagUIList.bagPanel == null || !_uiScript.bagUIList.bagPanel.activeSelf) return;

        //clear slots
        _uiScript.bagUIList.ClearSlots();

        int currentSlot = 0;

        //bear traps
        bearTraps.currentSlot = -1;
        if (bearTraps.amount > 0){
            _uiScript.bagUIList.SwitchBagSlotBackground(currentSlot, true);
            _uiScript.bagUIList.GetBagSlotIcon(currentSlot).enabled = true;
            _uiScript.bagUIList.GetBagSlotIcon(currentSlot).sprite = bearTraps.icon;
            _uiScript.bagUIList.GetBagSlotText(currentSlot).text = bearTraps.amount.ToString();
            currentSlot++;
            bearTraps.currentSlot = currentSlot;
        }

        //bait meat
        baitMeat.currentSlot = -1;
        if (baitMeat.amount > 0){
            _uiScript.bagUIList.SwitchBagSlotBackground(currentSlot, true);
            _uiScript.bagUIList.GetBagSlotIcon(currentSlot).enabled = true;
            _uiScript.bagUIList.GetBagSlotIcon(currentSlot).sprite = baitMeat.icon;
            _uiScript.bagUIList.GetBagSlotText(currentSlot).text = baitMeat.amount.ToString();
            currentSlot++;
            baitMeat.currentSlot = currentSlot;
        }

        //bait bird
        baitBird.currentSlot = -1;
        if (baitBird.amount > 0){
            _uiScript.bagUIList.SwitchBagSlotBackground(currentSlot, true);
            _uiScript.bagUIList.GetBagSlotIcon(currentSlot).enabled = true;
            _uiScript.bagUIList.GetBagSlotIcon(currentSlot).sprite = baitBird.icon;
            _uiScript.bagUIList.GetBagSlotText(currentSlot).text = baitBird.amount.ToString();
            currentSlot++;
            baitBird.currentSlot = currentSlot;
        }

        //loop through inventory slots and check if mouse is over any
        int hoveredSlot = -1;
        for (int i = 0; i < _uiScript.bagUIList.bagSlots.Count; i++){
            RectTransform itemRect = _uiScript.bagUIList.bagSlots[i].GetComponent<RectTransform>();
            Vector2 localMousePosition = itemRect.InverseTransformPoint(Mouse.current.position.ReadValue());
            if (itemRect.rect.Contains(localMousePosition)){
                hoveredSlot = i+1;
                break;
            }
        }
        UpdateInfoBox(hoveredSlot);
    }

    private void UpdateInfoBox(int slot){
        bool display = false;
        _uiScript.infoPannel.SetActive(false);
        if (slot == -1) return;

        if (slot == bearTraps.currentSlot){
            _uiScript.infoText.text = bearTraps.name + ": " + bearTraps.amount + "/" + bearTraps.maxAmount;
            display = true;
        }
        else if (slot == baitMeat.currentSlot){
            _uiScript.infoText.text = baitMeat.name + ": " + baitMeat.amount + "/" + baitMeat.maxAmount;
            display = true;
        }
        else if (slot == baitBird.currentSlot){
            _uiScript.infoText.text = baitBird.name + ": " + baitBird.amount + "/" + baitBird.maxAmount;
            display = true;
        }

        if (display){
            _uiScript.infoPannel.SetActive(true);
        }
    }

    private void UpdateHotbarUI()
    {
        if (_uiScript == null || _uiScript.hotbarUIList == null || _uiScript.hotbarUIList.hotbarPanel == null || !_uiScript.hotbarUIList.hotbarPanel.activeSelf) return;

        //clear slots
        _uiScript.hotbarUIList.ClearSlots();

        int currentSlot = 0;

        //bear traps
        if (bearTraps.amount > 0){
            _uiScript.hotbarUIList.GetHotbarSlotIcon(currentSlot).enabled = true;
            _uiScript.hotbarUIList.GetHotbarSlotIcon(currentSlot).sprite = bearTraps.icon;
            _uiScript.hotbarUIList.GetHotbarSlotCountText(currentSlot).text = bearTraps.amount.ToString();
        }
        currentSlot++;

        //bait meat
        if (baitMeat.amount > 0){
            _uiScript.hotbarUIList.GetHotbarSlotIcon(currentSlot).enabled = true;
            _uiScript.hotbarUIList.GetHotbarSlotIcon(currentSlot).sprite = baitMeat.icon;
            _uiScript.hotbarUIList.GetHotbarSlotCountText(currentSlot).text = baitMeat.amount.ToString();
        }
        currentSlot++;

        //bait bird
        if (baitBird.amount > 0){
            _uiScript.hotbarUIList.GetHotbarSlotIcon(currentSlot).enabled = true;
            _uiScript.hotbarUIList.GetHotbarSlotIcon(currentSlot).sprite = baitBird.icon;
            _uiScript.hotbarUIList.GetHotbarSlotCountText(currentSlot).text = baitBird.amount.ToString();
        }
        currentSlot++;
    }

    public void HotkeyPressed(InputAction.CallbackContext ctx){
        //check if placing bear trap anim is playing or dodge anim is playing
        if (_animator.GetCurrentAnimatorStateInfo(3).IsName("Player|PLACE TRAP (ALL)") || 
            _animator.GetCurrentAnimatorStateInfo(3).IsName("Player|DODGE (ALL)") ||
            _animator.GetCurrentAnimatorStateInfo(3).IsName("Player|PAIN (ALL)"))
        {
            return;
        }

        if (ctx.performed)
        {
            if (ctx.action == hotkey1Action)
            {
                UseHotkey(1);
            }
            else if (ctx.action == hotkey2Action)
            {
                UseHotkey(2);
            }
            else if (ctx.action == hotkey3Action)
            {
                UseHotkey(3);
            }
        }
    }

    public void UseHotkey(int hotkey){
        InventoryItem item = GetHotkeyItem(hotkey);

        if (item.amount > 0){
            item.amount--;
            if (item.prefab != null){
                Vector3 forwardPos = transform.position + transform.forward * item.placeDistance;
                Vector3 groundPos = forwardPos;
                //raycast to find ground
                RaycastHit hit;
                if (Physics.Raycast(forwardPos, Vector3.down, out hit, 100f))
                {
                    groundPos = hit.point;
                }

                //get lowest point on mesh
                MeshFilter meshFilter = item.prefab.GetComponentInChildren<MeshFilter>();
                if (meshFilter != null)
                {
                    Vector3[] vertices = meshFilter.sharedMesh.vertices;
                    float lowestY = float.MaxValue;
                    foreach (Vector3 vertex in vertices)
                    {
                        if (vertex.y < lowestY)
                        {
                            lowestY = vertex.y;
                        }
                    }
                    groundPos.y += Mathf.Abs(lowestY) * item.prefab.transform.localScale.y;
                }

                StartCoroutine(SpawnItemTrapWithDelay(item, groundPos, Quaternion.identity, item.placeDelay));

                //if beartrap, do animation
                if (item.name == "Bear Trap"){
                    _animator.SetTrigger("PlaceTrap");
                }
                //if (obj.GetComponent<Rigidbody>()) obj.GetComponent<Rigidbody>().AddForce(transform.forward * 10, ForceMode.Impulse);
            }
        }
    }

    private IEnumerator SpawnItemTrapWithDelay(InventoryItem item, Vector3 pos, Quaternion rot, float delay){
        yield return new WaitForSeconds(delay);
        Instantiate(item.prefab, pos, Quaternion.identity);
    }

    private InventoryItem GetHotkeyItem(int hotkey)
    {
        if (hotkey == bearTraps.hotkey) return bearTraps;
        else if (hotkey == baitMeat.hotkey) return baitMeat;
        else if (hotkey == baitBird.hotkey) return baitBird;
        else return new InventoryItem();
    }

    public void SetHotkey(int slot, int hotkey){
        RemoveBind(hotkey);

        if (slot == 0){
            bearTraps.hotkey = hotkey;
        }
        else if (slot == 1){
            baitMeat.hotkey = hotkey;
        }
        else if (slot == 2){
            baitBird.hotkey = hotkey;
        }
    }

    public void RemoveBind(int hotkey){
        if (bearTraps.hotkey == hotkey){
            bearTraps.hotkey = -1;
        }
        else if (baitMeat.hotkey == hotkey){
            baitMeat.hotkey = -1;
        }
        else if (baitBird.hotkey == hotkey){
            baitBird.hotkey = -1;
        }
    }

    internal void AddItem(string itemName, int amount)
    {
        if (itemName.ToLower() == bearTraps.name.ToLower()){
            AddBearTrap(amount);
        }
        else if (itemName.ToLower() == baitMeat.name.ToLower()){
            AddBaitMeat(amount);
        }
        else if (itemName.ToLower() == baitBird.name.ToLower()){
            AddBaitBird(amount);
        }
    }

    public void AddBearTrap(int amount){
        bearTraps.amount += amount;
        bearTraps.amount = Mathf.Clamp(bearTraps.amount, 0, bearTraps.maxAmount);
        UpdateBagUI();
    }

    public void AddBaitMeat(int amount){
        baitMeat.amount += amount;
        baitMeat.amount = Mathf.Clamp(baitMeat.amount, 0, baitMeat.maxAmount);
        UpdateBagUI();
    }

    public void AddBaitBird(int amount){
        baitBird.amount += amount;
        baitBird.amount = Mathf.Clamp(baitBird.amount, 0, baitBird.maxAmount);
        UpdateBagUI();
    }

#region OpenCloseScripts

    public void OpenJournal()
    {
        if (_uiScript == null || _uiScript.journalUIList == null || _uiScript.journalUIList.journalPanel == null) return;

        CloseAll();

        _uiScript.journalUIList.journalPanel.SetActive(true);

        if (_audioSource) _audioSource.PlayOneShot(openJournalSound);
    }
    public void CloseJournal()
    {
        if (_uiScript == null || _uiScript.journalUIList == null || _uiScript.journalUIList.journalPanel == null) return;

        _uiScript.journalUIList.journalPanel.SetActive(false);

        if (_audioSource) _audioSource.PlayOneShot(closeJournalSound);
    }
    public void ToggleJournal()
    {
        if (_uiScript == null || _uiScript.journalUIList == null || _uiScript.journalUIList.journalPanel == null) return;

        if (_uiScript.journalUIList.journalPanel.activeSelf) CloseJournal();
        else OpenJournal();
    }

    public void OpenBag()
    {
        if (_uiScript == null || _uiScript.bagUIList == null || _uiScript.bagUIList.bagPanel == null) return;

        CloseAll();

        _uiScript.bagUIList.bagPanel.SetActive(true);

        if (_audioSource) _audioSource.PlayOneShot(openBagSound);
    }
    public void CloseBag()
    {
        if (_uiScript == null || _uiScript.bagUIList == null || _uiScript.bagUIList.bagPanel == null) return;

        _uiScript.bagUIList.bagPanel.SetActive(false);

        if (_audioSource) _audioSource.PlayOneShot(closeBagSound);
    }
    public void ToggleBag()
    {
        if (_uiScript == null || _uiScript.bagUIList == null || _uiScript.bagUIList.bagPanel == null) return;

        if (_uiScript.bagUIList.bagPanel.activeSelf) CloseBag();
        else OpenBag();
    }

    public void CloseAll(bool openPause = false)
    {
        if (_uiScript == null) return;

        //if no UI is open, toggle pause menu
        if (openPause &&
            !_uiScript.journalUIList.journalPanel.activeSelf &&
            !_uiScript.bagUIList.bagPanel.activeSelf)
        {
            TogglePauseMenu();
        }
        else
        {
            CloseJournal();
            CloseBag();
            ClosePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        if (_uiScript == null || _uiScript.pauseUI == null || _uiScript.pauseUI == null) return;

        if (_uiScript.pauseUI.activeSelf)
        {
            ClosePauseMenu();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    private void OpenPauseMenu(){
        if (_uiScript == null || _uiScript.pauseUI == null || _uiScript.pauseUI == null) return;

        CloseAll();

        _uiScript.pauseUI.SetActive(true);
    }

    private void ClosePauseMenu(){
        if (_uiScript == null || _uiScript.pauseUI == null || _uiScript.pauseUI == null) return;

        _uiScript.pauseUI.SetActive(false);
    }

    #endregion
}
