using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public class Notif {
        public string icon = "journal";
        public int count = 1;
        public TextMeshProUGUI text;
    }
    public List<Notif> m_notifs = new List<Notif>();
    public GameObject m_notifPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddIcon(string _icon = "journal", int _amount = 1){
        // check if message is same as last message, if so, increment count and update text
        // if not, create new message

        if (m_notifs.Count > 0 && m_notifs[m_notifs.Count - 1].icon == _icon && m_notifs[m_notifs.Count - 1].text != null && m_notifs[m_notifs.Count - 1].text.GetComponent<FloatingTextPopup>().m_fadeTimer <= 0.0f)
        {
            m_notifs[m_notifs.Count - 1].count++;
            // m_notifs[m_notifs.Count - 1].text.text = "<sprite name=\"" + _icon + "\"> " + m_notifs[m_notifs.Count - 1].count;
            // reset fade timer
            m_notifs[m_notifs.Count - 1].text.GetComponent<FloatingTextPopup>().m_fadeTimer = 0.0f;
        }
        else
        {
            GameObject notif = Instantiate(m_notifPrefab, transform);
            Notif newNotif = new Notif();
            newNotif.icon = _icon;
            newNotif.text = notif.GetComponentInChildren<TextMeshProUGUI>();
            newNotif.text.text = "<sprite name=\"" + _icon + "\" color=#808080> ";
            m_notifs.Add(newNotif);
        }

        // if there is are any notifs before this one, set fade delay to 0
        for (int i = 0; i < m_notifs.Count - 2; i++)
        {
            if (m_notifs[i].text != null) m_notifs[i].text.GetComponent<FloatingTextPopup>().m_startFadeDelay = 0;
        }
    }
}
