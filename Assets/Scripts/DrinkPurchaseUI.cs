using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DrinkPurchaseUI : MonoBehaviour
{
    [Header("References")]
    public Button m_button;
    public Image m_background;
    public Image m_icon;
    public TextMeshProUGUI m_primaryText;
    public TextMeshProUGUI m_secondaryText;
    public GameObject m_silverIcon;

    [Header("Journal Entry")]
    [ReadOnly] public Drink m_linkedDrink;
    [ReadOnly] public int m_price = 10;

    [Header("Assets")]
    public Sprite m_selectedSprite;
    public Sprite m_unselectedSprite;


    public System.Action OnClick;
    public bool m_isSelected = false;


    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable() {
        // bind button to the click function
        m_button.onClick.AddListener(() =>
        {
            OnClick?.Invoke();
        });
    }

    private void OnDisable() {
        // unbind button to the click function
        m_button.onClick.RemoveListener(() =>
        {
            OnClick?.Invoke();
        });
    }

    // Update is called once per frame
    void Update()
    {
        // check if mouse is over this grid item
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Camera uiCamera = Camera.main;

        // get corners
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        Vector2 bottomLeft = corners[0];
        Vector2 topLeft = corners[1];
        Vector2 topRight = corners[2];
        Vector2 bottomRight = corners[3];

        // check if mouse is within the grid item's screen space
        if (mouseScreenPos.x >= topLeft.x && mouseScreenPos.x <= topRight.x &&
            mouseScreenPos.y <= topLeft.y && mouseScreenPos.y >= bottomLeft.y)
        {
            // find mask in parent
            Mask mask = GetComponentInParent<Mask>();
            if (mask != null){
                // get bottom world corner of mask
                Vector3[] maskCorners = new Vector3[4];
                mask.GetComponent<RectTransform>().GetWorldCorners(maskCorners);
                Vector2 maskBottomLeft = maskCorners[0];
                // if mouse is below mask, skip
                if (mouseScreenPos.y < maskBottomLeft.y)
                {
                    return;
                }
            }
            

            InfoBox ib = FindObjectOfType<InfoBox>();
            // display info box
            if (ib) ib.Display(m_linkedDrink, true, 0.1f, 0.1f);
        }
    }

    public void UpdateUI()
    {
        // set background if m_isSelected
        m_background.sprite = m_isSelected ? m_selectedSprite : m_unselectedSprite;

        // set icon
        m_icon.sprite = m_linkedDrink.m_icon;

        // check if this drink is already active (aka purchased)
        bool allActive = IsAlreadyActive();

        // show correct text and silver icon (if needed)
        m_primaryText.text = m_linkedDrink.m_displayName.ToString();
        m_secondaryText.text = allActive ? "Active" : m_price.ToString();
        m_silverIcon.SetActive(!allActive);
    }

    public bool IsAlreadyActive()
    {
        if (StatsManager.activeDrinks.Contains(m_linkedDrink))
        {
            return true;
        }

        return false;
    }

    public void JournalButtonPressed(){
        // open the journal
        JournalManager.instance.OpenWindow();
        // select the entry
        //JournalManager.instance.
    }
}
