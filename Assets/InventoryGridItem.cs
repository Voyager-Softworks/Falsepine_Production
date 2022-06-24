using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryGridItem : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI text;
    public Button button;

    // reference to the item this grid item is representing
    public Item item;
}
