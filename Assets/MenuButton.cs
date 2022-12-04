using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// Select and hover changes
/// </summary>
public class MenuButton : MonoBehaviour
{
    Image m_arrowImage;

    // Start is called before the first frame update
    void Start()
    {
        // last image child is the arrow
        m_arrowImage = GetComponentsInChildren<Image>()[1];
    }

    // Update is called once per frame
    void Update()
    {
        // if mouse is hovering over this button, select it
        if (GetComponent<RectTransform>().rect.Contains(transform.InverseTransformPoint(Mouse.current.position.ReadValue())))
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        // toggle white/red and arrow on selected
        if (EventSystem.current.currentSelectedGameObject == gameObject) {
            GetComponentInChildren<TextMeshProUGUI>().color = Color.red;

            m_arrowImage.enabled = true;
        }
        else {
            GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

            m_arrowImage.enabled = false;
        }
    }
}
