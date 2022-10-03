using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SectionButton : MonoBehaviour
{
    private Button button;
    public GameObject content;

    private void Awake() {
        button = GetComponent<Button>();

        // close
        CloseSection();
    }

    private void OnEnable() {
        button.onClick.AddListener(ToggleSection);
    }

    private void OnDisable() {
        button.onClick.RemoveListener(ToggleSection);
    }

    public void OpenSection()
    {
        content.SetActive(true);

        Rebuild();
    }

    public void CloseSection()
    {
        content.SetActive(false);

        Rebuild();
    }

    public void ToggleSection()
    {
        if (content.activeSelf)
        {
            CloseSection();
        }
        else
        {
            OpenSection();
        }
    }

    private void Rebuild()
    {
        // get all rect transform in parents
        List<RectTransform> parents = new List<RectTransform>();
        RectTransform parent = transform.parent.GetComponent<RectTransform>();
        while (parent != null)
        {
            parents.Add(parent);
            parent = parent.parent?.GetComponent<RectTransform>();
        }

        // rebuild all parents
        foreach (RectTransform p in parents)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(p);
        }
    }
}
