using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class JournalContentUpdater : MonoBehaviour  /// @todo comment
{
    public GameObject contentParent;

    public GameObject textContentPrefab;
    public GameObject imageContentPrefab;

    List<GameObject> currentContentObjects = new List<GameObject>();

    public List<JournalContent> contentList = new List<JournalContent>();

    // Start is called before the first frame update
    protected void Start()
    {

    }

    // Update is called once per frame
    protected void Update()
    {

    }

    protected virtual void OnEnable()
    {
        UpdateContent();
    }

    protected virtual void AddTextContentObject(string text)
    {
        GameObject textContent = Instantiate(textContentPrefab, contentParent.transform);
        textContent.GetComponent<TextMeshProUGUI>().text = text;

        currentContentObjects.Add(textContent);
    }

    protected virtual void AddImageContentObject(Sprite sprite)
    {
        GameObject imageContent = Instantiate(imageContentPrefab, contentParent.transform);
        imageContent.GetComponent<Image>().sprite = sprite;

        currentContentObjects.Add(imageContent);
    }

    public virtual void UpdateContent()
    {
        // remove current content
        RemoveContentObjects();

        // add new content
        foreach (JournalContent content in contentList)
        {
            if (content.text != string.Empty)
            {
                AddTextContentObject(content.text);
            }
            if (content.image != null)
            {
                AddImageContentObject(content.image);
            }
        }
    }

    protected void RemoveContentObjects()
    {
        foreach (GameObject content in currentContentObjects)
        {
            Destroy(content);
        }
        currentContentObjects.Clear();
    }
}
