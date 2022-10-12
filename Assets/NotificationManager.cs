using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class NotificationManager : MonoBehaviour
{
    private static NotificationManager _instance;
    public static NotificationManager instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<NotificationManager>();
            }
            return _instance;
        }
    }

    public class Notif {
        public string icon = "journal";
        public int count = 1;
        public FloatingTextPopup popup;
        public Vector3 position;
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

    public void AddIcon(string _icon = "journal", Vector3 _position = new Vector3()) {
        Vector3 targetPos = _position;
        // check if any notifs are near this position, if so, move upwards
        while (m_notifs.Any<Notif>(x => Vector3.Distance(x.position, targetPos) < 1f)) {
            targetPos.y += 1f;
        }

        GameObject notif = Instantiate(m_notifPrefab, targetPos, Quaternion.identity);
        
        Notif newNotif = new Notif();
        newNotif.icon = _icon;
        newNotif.popup = notif.GetComponent<FloatingTextPopup>();
        newNotif.popup.SetIcon(_icon);
        newNotif.position = targetPos;

        m_notifs.Add(newNotif);
    }

    public void AddIconAtPlayer(string _icon = "journal", float _height = 1.0f) {
        Vector3 position = Vector3.up * _height;
        PlayerMovement playerMovement = GameObject.FindObjectOfType<PlayerMovement>();
        if (playerMovement != null){
            position = playerMovement.transform.position + new Vector3(0f, _height, 0f);
        }

        AddIcon(_icon, position);
    }
}
