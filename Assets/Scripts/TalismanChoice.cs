using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[RequireComponent(typeof(Interactable))]
public class TalismanChoice : ToggleableWindow
{
    public Button m_closeButton;
    public GameObject m_choicePanel;

    [System.Serializable]
    public class ChoiceLink
    {
        public Button m_button;
        public StatsManager.Talisman m_talisman;
    }

    [SerializeField] public List<ChoiceLink> m_choices = new List<ChoiceLink>();
    

    // Start is called before the first frame update
    void Start()
    {
        // make random talismans
        foreach (ChoiceLink link in m_choices)
        {
            int tries = 100;
            // ensure talisman is unique
            link.m_talisman = StatsManager.instance.GetRandomTalisman();
            while (m_choices.Any<ChoiceLink>(x => x.m_talisman.m_statMod.statType == link.m_talisman.m_statMod.statType) && tries > 0)
            {
                link.m_talisman = StatsManager.instance.GetRandomTalisman();
                tries--;
            }

            link.m_button.GetComponentInChildren<TextMeshProUGUI>().text = link.m_talisman.m_statMod.ToText();
        }

        // get the interactable component
        Interactable interactable = GetComponent<Interactable>();
        if (interactable == null)
        {
            Debug.LogError("Interactable component not found!");
            return;
        }
        
        // add the interactable event
        interactable.onInteract += OpenWindow;

        AddListeners();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        m_closeButton.onClick.AddListener(CloseWindow);
    }

    protected override void OnDisable() {
        base.OnDisable();

        m_closeButton.onClick.RemoveListener(CloseWindow);
        
        RemoveAllListeners();
    }

    private void AddListeners()
    {
        foreach (ChoiceLink link in m_choices)
        {
            link.m_button.onClick.AddListener(() =>
            {
                ChooseTalisman(link.m_talisman);
            });
        }
    }

    private void RemoveAllListeners()
    {
        foreach (ChoiceLink link in m_choices)
        {
            link.m_button.onClick.RemoveListener(() =>
            {
                ChooseTalisman(link.m_talisman);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChooseTalisman(StatsManager.Talisman talisman) {
        if (StatsManager.instance == null) return;
        if (talisman == null) return;

        StatsManager.instance.m_activeTalismans.Add(talisman);

        // disable interactable
        GetComponent<Interactable>().DisableInteract();

        CloseWindow();
    }

    // ToggleableWindow overrides
    public override bool IsOpen()
    {
        return m_choicePanel.activeSelf;
    }
    public override void OpenWindow()
    {
        base.OpenWindow();
        m_choicePanel.SetActive(true);
    }
    public override void CloseWindow()
    {
        base.CloseWindow();
        m_choicePanel.SetActive(false);
    }
}
