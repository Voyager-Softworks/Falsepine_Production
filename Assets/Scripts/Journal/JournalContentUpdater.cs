using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Base class for content pages in the journal to make sure that they are updated when the journal is opened.
/// </summary>
public class JournalContentUpdater : MonoBehaviour
{
    public GameObject contentParent;

    public GameObject textContentPrefab;
    public GameObject imageContentPrefab;

    List<GameObject> currentContentObjects = new List<GameObject>();

    public List<JournalContent> contentList = new List<JournalContent>();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        UpdateContent();
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    protected virtual void OnEnable()
    {
        UpdateContent();
    }

    protected virtual void OnDisable()
    {
        
    }

    /// <summary>
    /// Adds thext into this content page.
    /// </summary>
    /// <param name="text"></param>
    protected virtual void AddTextContentObject(string text, bool bold = false)
    {
        GameObject textContent = Instantiate(textContentPrefab, contentParent.transform);
        textContent.GetComponent<TextMeshProUGUI>().text = text;
        if (bold)
        {
            textContent.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        }

        currentContentObjects.Add(textContent);
    }

    /// <summary>
    /// Adds an image into this content page.
    /// </summary>
    /// <param name="sprite"></param>
    protected virtual void AddImageContentObject(Sprite sprite)
    {
        GameObject imageContent = Instantiate(imageContentPrefab, contentParent.transform);
        imageContent.GetComponent<Image>().sprite = sprite;

        currentContentObjects.Add(imageContent);
    }

    /// <summary>
    /// Updates the content with the current content list.
    /// </summary>
    public virtual void UpdateContent()
    {
        // remove current content
        RemoveContentObjects();

        // add new content
        foreach (JournalContent content in contentList)
        {
            if (content.text != string.Empty)
            {
                AddTextContentObject(content.text, content.bold);
            }
            if (content.image != null)
            {
                AddImageContentObject(content.image);
            }
        }
    }

    /// <summary>
    /// Removes old content
    /// </summary>
    protected void RemoveContentObjects()
    {
        foreach (GameObject content in currentContentObjects)
        {
            Destroy(content);
        }
        currentContentObjects.Clear();
    }
}
