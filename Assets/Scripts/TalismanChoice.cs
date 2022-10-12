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
    public Button m_confirmButton;
    public GameObject m_choicePanel;

    [System.Serializable]
    public class ChoiceLink
    {
        public Button m_button;
        public StatsManager.Talisman m_talisman;
    }

    [SerializeField] public List<ChoiceLink> m_choices = new List<ChoiceLink>();

    [ReadOnly] public ChoiceLink m_selectedChoice;

    public Sprite m_selectedSprite;
    public Sprite m_unselectedSprite;

    public AudioClip m_confirmSound;
    public GameObject m_confirmEffect;

    private void Awake() {
        // disable panel
        m_choicePanel.SetActive(false);
    }
    

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
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        AddListeners();

        m_closeButton.onClick.AddListener(CloseWindow);

        m_confirmButton.onClick.AddListener(ConfirmChoice);
    }

    protected override void OnDisable() {
        base.OnDisable();
        
        RemoveAllListeners();

        m_closeButton.onClick.RemoveListener(CloseWindow);

        m_confirmButton.onClick.RemoveListener(ConfirmChoice);
    }

    private void AddListeners()
    {
        foreach (ChoiceLink link in m_choices)
        {
            link.m_button.onClick.AddListener(() =>
            {
                ChoiceClicked(link);
            });
        }
    }

    private void ChoiceClicked(ChoiceLink link)
    {
        // deselect all
        foreach (ChoiceLink choice in m_choices)
        {
            choice.m_button.interactable = true;

            // set background image
            choice.m_button.GetComponent<Image>().sprite = m_unselectedSprite;
        }

        // select this one
        link.m_button.interactable = false;
        m_selectedChoice = link;

        // set background image
        link.m_button.GetComponent<Image>().sprite = m_selectedSprite;
    }

    private void RemoveAllListeners()
    {
        foreach (ChoiceLink link in m_choices)
        {
            link.m_button.onClick.RemoveListener(() =>
            {
                ChoiceClicked(link);
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

        // disable all colliders
        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        // disable all mesh renderers
        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = false;
        }

        // stop all particle systems
        foreach (ParticleSystem system in GetComponentsInChildren<ParticleSystem>())
        {
            system.Stop();
        }

        // turn off all lights
        foreach (Light light in GetComponentsInChildren<Light>())
        {
            light.enabled = false;
        }

        // play sound
        if (m_confirmSound != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.PlayOneShot(m_confirmSound);
            }
        }

        // instantiate effect
        if (m_confirmEffect != null)
        {
            Instantiate(m_confirmEffect, transform.position, Quaternion.identity);
        }

        // message
        if (MessageManager.instance) {
            MessageManager.instance.AddMessage("New Talisman", "talisman", false);
            // notify
            NotificationManager.instance?.AddIcon("talisman", transform.position + Vector3.up * 2f);
        }

        CloseWindow();
    }

    private void ConfirmChoice(){
        if (m_selectedChoice == null) return;

        ChooseTalisman(m_selectedChoice.m_talisman);
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
