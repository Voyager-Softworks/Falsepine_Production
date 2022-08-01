using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;

/// <summary>
/// Tag manager for the ItemDatabase.
/// </summary>
[Serializable]
public class TagManager
{
    [Serializable]
    public class Tag
    {
        // constructor
        public Tag(string _name)
        {
            // needs unique id
            m_id = this.GetType().Name + "_" + Guid.NewGuid().ToString();
            this.name = _name;
            m_payload = "";
        }
        public Tag(string _name, string _payload) : this(_name){
            this.m_payload = _payload;
        }

        [SerializeField] readonly public string m_id = "";
        [SerializeField] private string m_name = "";
        /// <summary>
        /// ONLY letters, numbers, and underscores are allowed (setter auto corrects! be mindful).
        /// </summary>
        [SerializeField] public string name 
        {
            get { 
                return m_name; 
            }
            set
            {
                m_name = value;
                ValidateName();
            }
        }

        [SerializeField] private string m_payload = "";
        [SerializeField] public string payload
        {
            get { 
                return m_payload; 
            }
            set { 
                m_payload = value; 
            }
        }

        public void ValidateName()
        {
            //correct name
            //to lower
            m_name = m_name.ToLower();
            //replace non letters and numbers with ""
            m_name = Regex.Replace(m_name, @"[^a-zA-Z0-9_]", "");
            //replace space with _
            m_name = m_name.Replace(" ", "_");
        }

        // SIMPLE PAYLOAD CONVERTERS ++++++++++

        /// <summary>
        /// Converts payload to int.
        /// </summary>
        public int PayloadToInt()
        {
            try
            {
                return int.Parse(m_payload);
            }
            catch (Exception e)
            {
                Debug.LogError("TagManager.Tag.ToInt() failed: " + e.Message);
                return 0;
            }
        }

        /// <summary>
        /// Converts payload to float.
        /// </summary>
        public float PayloadToFloat()
        {
            try
            {
                return float.Parse(m_payload);
            }
            catch (Exception e)
            {
                Debug.LogError("TagManager.Tag.ToFloat() failed: " + e.Message);
                return 0;
            }
        }

        /// <summary>
        /// Converts payload to bool.
        /// </summary>
        public bool PayloadToBool()
        {
            try
            {
                return bool.Parse(m_payload);
            }
            catch (Exception e)
            {
                Debug.LogError("TagManager.Tag.ToBool() failed: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Converts payload to string. (no change)
        /// </summary>
        public string PayloadToString()
        {
            return m_payload;
        }

        /// <summary>
        /// Converts payload to list of strings. Expected String: "a,b,c"
        /// </summary>
        public List<string> PayloadToList()
        {
            try
            {
                return m_payload.Split(',').ToList();
            }
            catch (Exception e)
            {
                Debug.LogError("TagManager.Tag.ToList() failed: " + e.Message);
                return new List<string>();
            }
        }



        /// <summary>
        /// Converts string into new tag.
        /// </summary>
        /// <param name="_payload">Expected String: "tagName,tagPayload"</param>
        public static implicit operator Tag(string _payload)
        {
            // split into tag name and payload and remove brackets
            string[] split = _payload.Substring(1, _payload.Length - 2).Split(',');
            // create new tag
            Tag tag = new Tag(split[0], split[1]);
            return tag;
        }
    }
}