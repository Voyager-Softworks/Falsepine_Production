using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @deprecated
/// </summary>
public class MessageManager : MonoBehaviour
{
    public GameObject messagePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddMessage(string _messsage){
        GameObject message = Instantiate(messagePrefab, transform);
        message.GetComponent<MessageScript>().SetMessage(_messsage);
    }
}
