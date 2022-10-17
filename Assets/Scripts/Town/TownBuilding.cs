using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// A clickable building in the town. Used as a base class for specific buildings.
/// </summary>
public class TownBuilding : ClickableObject
{
    public enum BuildingType {
        BANK,
        HOME,
        INN,
        STORE
    }

    public GameObject UI;

    [Serializable]
    private class LightData {
        public Light m_lightObject;
        public float m_startIntensity;
    }
    private List<Material> m_lightMaterials = new List<Material>();
    private Color m_startMatColor = Color.white;
    private List<LightData> m_lights = new List<LightData>();
    public float m_currentIntensity = 0.0f;
    public float m_maxIntensity = 1.0f;
    public float m_minIntensity = 0.0f;
    public string m_windowInteractMatName = "windowInteract";

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        CloseUI();

        // find all children with the windowInteractRef material
        MeshRenderer[] allMeshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in allMeshes)
        {
            if (m_windowInteractMatName == null) continue;

            for (int i = 0; i < mesh.materials.Length; i++)
            {
                if (mesh.materials[i].name.Contains(m_windowInteractMatName))
                {
                    m_lightMaterials.Add(mesh.materials[i]);
                    m_startMatColor = mesh.materials[i].GetColor("_EmissiveColor");
                }
            }
        }

        // find all children with the light component
        Light[] allLights = GetComponentsInChildren<Light>();
        foreach (Light light in allLights)
        {
            LightData lightData = new LightData();
            lightData.m_lightObject = light;
            lightData.m_startIntensity = light.intensity;

            m_lights.Add(lightData);
        }

        // set the emission intensity to 0
        SetLight(0.0f);
    }

    private void SetLight(float _val)
    {
        foreach (Material mat in m_lightMaterials)
        {
            // update the emission color
            mat.SetColor("_EmissiveColor", m_startMatColor * _val);
        }

        foreach (LightData light in m_lights)
        {
            light.m_lightObject.intensity = _val * light.m_startIntensity;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // update light based on mouse over
        if (CheckMouseOver())
        {
            // ramp up to max
            m_currentIntensity = Mathf.Lerp(m_currentIntensity, m_maxIntensity, 10.0f * Time.deltaTime);
        }
        else
        {
            // ramp down to min
            m_currentIntensity = Mathf.Lerp(m_currentIntensity, m_minIntensity, 10.0f * Time.deltaTime);
        }

        SetLight(m_currentIntensity);

        //if escape is pressed, close the panel
        // if (Keyboard.current.escapeKey.wasPressedThisFrame)
        // {
        //     CloseUI();
        // }
    }

    /// <summary>
    /// When this building is clicked.
    /// </summary>
    public override void OnClick()
    {
        base.OnClick();

        ToggleableTownWindow ttw = GetComponent<ToggleableTownWindow>();
        if (ttw != null)
        {
            ttw.OpenWindow();
        }
    }

    /// <summary>
    /// Opens the attached UI.
    /// </summary>
    public virtual void OpenUI()
    {
        if (!UI) return;
        UI.SetActive(true);
    }

    /// <summary>
    /// Close the attached UI.
    /// </summary>
    public virtual void CloseUI()
    {
        if (!UI) return;
        UI.SetActive(false);
    }

    /// <summary>
    /// Toggle the attached UI.
    /// </summary>
    public virtual void ToggleUI()
    {
        if (!UI) return;
        if (UI.activeSelf)
        {
            CloseUI();
        }
        else
        {
            OpenUI();
        }
    }
}
