using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
        // set the opacity of this image to opacity of most recent notif
        if (m_notifs.Count > 0 && m_notifs[m_notifs.Count - 1].text != null) {
            Notif mostRecentNotif = m_notifs[m_notifs.Count - 1];
            float opacity = mostRecentNotif.text.color.a;
            Image image = GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, opacity);
        }
        else{
            Image image = GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
    }

    public void AddIcon(string _icon = "journal", int _amount = 1){
        // check if message is same as last message, if so, increment count and update text
        // if not, create new message

        // todo: make it so that it checks all messages, not just the last one
        bool found = false;
        foreach (Notif notif in m_notifs) {
            if (notif.icon == _icon && notif.text != null) {
                notif.count++;
                // m_notifs[m_notifs.Count - 1].text.text = "<sprite name=\"" + _icon + "\"> " + m_notifs[m_notifs.Count - 1].count;
                // reset fade timer
                notif.text.GetComponent<FloatingTextPopup>().ResetTimers();
                notif.text.GetComponent<FloatingTextPopup>().SetOpacity(1.0f);
                found = true;
                return;
            }
        }
        if (!found)
        {
            GameObject notif = Instantiate(m_notifPrefab, transform);
            Notif newNotif = new Notif();
            newNotif.icon = _icon;
            newNotif.text = notif.GetComponentInChildren<TextMeshProUGUI>();
            newNotif.text.text = "<sprite name=\"" + _icon + "\"> ";
            m_notifs.Add(newNotif);
        }

        // if there is are any notifs before this one, set fade delay to 0
        for (int i = 0; i < m_notifs.Count - 2; i++)
        {
            if (m_notifs[i].text != null) m_notifs[i].text.GetComponent<FloatingTextPopup>().m_startFadeDelay = 0;
        }
    }
}
