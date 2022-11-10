using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aim zones are used to calculate damage, area, and range, of weapons.
/// </summary>
public class AimZone : MonoBehaviour
{
    private Mesh m_mesh;
    private MeshRenderer m_meshRenderer;
    private MeshFilter m_meshFilter;

    public Color m_ZoneColor = Color.white;

    public Color m_leftStartColor = new Color(1.0f, 0.0f, 0.0f, 0.0f);
    public Color m_leftEndColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

    public Color m_midStartColor = new Color(1.0f, 0.0f, 0.0f, 0.0f);
    public Color m_midEndColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

    public Color m_rightStartColor = new Color(0.5f, 0.5f, 0.5f, 0.0f);
    public Color m_rightEndColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);

    public LineRenderer m_leftLine = null;
    public LineRenderer m_midLine = null;
    public LineRenderer m_rightLine = null;

    /// <summary>
    /// Corners are used by the aim zone to keep track of the corners of the aim zone.
    /// </summary>
    public struct Corners
    {
        public Vector3 frontLeft;
        public Vector3 frontRight;
        public Vector3 backRight;
        public Vector3 backLeft;

        // constructor
        public Corners(Vector3 frontLeft, Vector3 frontRight, Vector3 backRight, Vector3 backLeft)
        {
            this.frontLeft = frontLeft;
            this.frontRight = frontRight;
            this.backRight = backRight;
            this.backLeft = backLeft;
        }

        public List<Vector3> ToList()
        {
            List<Vector3> corners = new List<Vector3>();
            corners.Add(frontLeft);
            corners.Add(frontRight);
            corners.Add(backRight);
            corners.Add(backLeft);
            return corners;
        }

        // zero constructor
        static public Corners Zero = new Corners(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero);

        // equality operator
        public static bool operator ==(Corners a, Corners b)
        {
            return a.frontLeft == b.frontLeft && a.frontRight == b.frontRight && a.backRight == b.backRight && a.backLeft == b.backLeft;
        }
        // inequality operator
        public static bool operator !=(Corners a, Corners b)
        {
            return !(a == b);
        }
    }

    private Corners m_corners;
    private Corners m_fallOffCorners;

    private Vector3 m_fl { get { return m_corners.frontLeft; } set { m_corners.frontLeft = value; } }
    private Vector3 m_fr { get { return m_corners.frontRight; } set { m_corners.frontRight = value; } }
    private Vector3 m_br { get { return m_corners.backRight; } set { m_corners.backRight = value; } }
    private Vector3 m_bl { get { return m_corners.backLeft; } set { m_corners.backLeft = value; } }

    public Vector3 fl { get { return m_corners.frontLeft; } }
    public Vector3 fr { get { return m_corners.frontRight; } }
    public Vector3 br { get { return m_corners.backRight; } }
    public Vector3 bl { get { return m_corners.backLeft; } }

    //equality operator (check for null)
    public static bool operator ==(AimZone a, AimZone b)
    {
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        return a.m_corners == b.m_corners;
    }
    //inequality operator
    public static bool operator !=(AimZone a, AimZone b)
    {
        return !(a == b);
    }

    // Start is called before the first frame update
    private void Awake()
    {
        // create a new mesh with 6 vertices (4 corners + 2 for the falloff)
        // m_mesh = new Mesh();
        // m_mesh.vertices = new Vector3[6];
        // m_mesh.triangles = new int[4];
        // m_mesh.RecalculateBounds();

        // // set the mesh filter to use the mesh
        // m_meshFilter = GetComponent<MeshFilter>();
        // m_meshFilter.mesh = m_mesh;

        //get mesh renderer
        m_meshRenderer = GetComponent<MeshRenderer>();

        //get mesh filter
        m_meshFilter = GetComponent<MeshFilter>();

        //get mesh
        m_mesh = m_meshFilter.mesh;

        UpdateZoneColors(m_ZoneColor);

        //hide aim zone
        Hide();
        //MatchSnowHeight();

        // load colors
        LoadColors();
    }

    private void OnEnable()
    {
        Application.onBeforeRender += UpdateLine;
    }

    private void OnDisable()
    {
        Application.onBeforeRender -= UpdateLine;
    }


    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Get corners of the aim zone in world space
    /// </summary>
    /// <returns>Corners struct</returns>
    public Corners GetWorldCorners()
    {
        Corners corners = new Corners();
        corners.frontLeft = m_fl;
        corners.frontRight = m_fr;
        corners.backRight = m_br;
        corners.backLeft = m_bl;
        return corners;
    }

    /// <summary>
    /// Get corners of the aim zone in local space
    /// </summary>
    /// <returns>Corners Struct</returns>
    public Corners GetLocalCorners()
    {
        Corners corners = new Corners();
        corners.frontLeft = transform.InverseTransformPoint(m_fl);
        corners.frontRight = transform.InverseTransformPoint(m_fr);
        corners.backRight = transform.InverseTransformPoint(m_br);
        corners.backLeft = transform.InverseTransformPoint(m_bl);
        return corners;
    }

    /// <summary>
    /// Sets the corners of the aim zone using world space coordinates
    /// </summary>
    /// <param name="_corners">A corners struct containing the points</param>
    public void SetCorners(Corners _corners)
    {
        this.m_fl = _corners.frontLeft;
        this.m_fr = _corners.frontRight;
        this.m_br = _corners.backRight;
        this.m_bl = _corners.backLeft;
    }

    /// <summary>
    /// Sets the corners of the aim zone using world space coordinates
    /// </summary>
    /// <param name="_frontLeft"></param>
    /// <param name="_frontRight"></param>
    /// <param name="_backRight"></param>
    /// <param name="_backLeft"></param>
    public void SetCorners(Vector3 _frontLeft, Vector3 _frontRight, Vector3 _backRight, Vector3 _backLeft)
    {
        SetCorners(new Corners(_frontLeft, _frontRight, _backRight, _backLeft));
    }

    /// <summary>
    /// Sets the colour of the aim zone for each corner
    /// </summary>
    /// <param name="_flColor"></param>
    /// <param name="_frColor"></param>
    /// <param name="_brColor"></param>
    /// <param name="_blColor"></param>
    public void UpdateZoneColors(Color _flColor, Color _frColor, Color _brColor, Color _blColor)
    {
        Color[] colors = new Color[4];
        colors[0] = _flColor;
        colors[1] = _frColor;
        colors[2] = _brColor;
        colors[3] = _blColor;
        m_mesh.colors = colors;
    }

    /// <summary>
    /// Sets the colour of the aim zone overall
    /// </summary>
    /// <param name="_color"></param>
    public void UpdateZoneColors(Color _color)
    {
        UpdateZoneColors(_color, _color, _color, _color);
    }

    /// <summary>
    /// Sets the colour of the aim lines
    /// </summary>
    /// <param name="_leftStart"></param>
    /// <param name="_leftEnd"></param>
    /// <param name="_midStart"></param>
    /// <param name="_midEnd"></param>
    /// <param name="_rightStart"></param>
    /// <param name="_rightEnd"></param>
    public void UpdateLineColors(Color _leftStart, Color _leftEnd, Color _midStart, Color _midEnd, Color _rightStart, Color _rightEnd)
    {
        m_midLine.startColor = _midStart;
        m_midLine.endColor = _midEnd;

        m_leftLine.startColor = _leftStart;
        m_leftLine.endColor = _leftEnd;

        m_rightLine.startColor = _rightStart;
        m_rightLine.endColor = _rightEnd;
    }

    /// <summary>
    /// Sets all of the colours of the lines
    /// </summary>
    /// <param name="_color"></param>
    public void UpdateLineColors(Color _color, bool _endAlpha = false)
    {
        // make end colours
        Color endColor = _color;
        if (_endAlpha)
        {
            endColor.a = 0;
        }

        UpdateLineColors(_color, endColor, _color, endColor, _color, endColor);
    }

    /// <summary>
    /// Update the actual mesh of the aim zone (the quad)
    /// </summary>
    public void UpdateVisuals(float _falloffMult)
    {
        // update the shader
        m_meshRenderer.material.SetFloat("_falloffMult", _falloffMult);

        UpdateMesh();
        UpdateLine();
    }

    /// <summary>
    /// Updates the mesh to represent the aim zone
    /// </summary>
    private void UpdateMesh()
    {
        // set the vertices of the aimQuad
        m_mesh.SetVertices(GetLocalCorners().ToList());

        //set tris to match the aimLines
        m_mesh.SetTriangles(new List<int>() { 0, 1, 2, 0, 2, 3, }, 0);
        Vector2[] uvs = CalculateUVs();
        m_mesh.uv = uvs;

        m_mesh.RecalculateBounds();
        m_mesh.RecalculateNormals();
        m_mesh.RecalculateTangents();
        m_mesh.RecalculateUVDistributionMetrics();
    }

    /// <summary>
    /// Calculate the UVs of the aim quad corners
    /// </summary>
    /// <returns></returns>
    private Vector2[] CalculateUVs()
    {
        //custom UVs
        Vector3[] vertices = m_mesh.vertices;
        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2(0, 1);
        uvs[1] = new Vector2(1, 1);
        uvs[2] = CalculateUVFromWorld(m_br);
        uvs[3] = CalculateUVFromWorld(m_bl);
        return uvs;
    }

    /// <summary>
    /// Takes a world point and calulates the effective UV coordinates of the point in the aim zone <br/>
    /// Widest points have UV.x between 0 and 1. 0 is left.<br/>
    /// Tallest points have UV.y between 0 and 1. 0 is closer to player.<br/>
    /// </summary>
    /// <param name="_worldPoint"></param>
    /// <returns></returns>
    public Vector2 CalculateUVFromWorld(Vector3 _worldPoint)
    {
        // convert to local space
        Vector3 localPoint = transform.InverseTransformPoint(_worldPoint);

        // get local corners
        Corners localCorners = GetLocalCorners();

        // get x value from 0 to 1 between front left and front right
        float x = Mathf.InverseLerp(localCorners.frontLeft.x, localCorners.frontRight.x, localPoint.x);

        // get z value from 0 to 1 between back left and front left
        float z = Mathf.InverseLerp(localCorners.backLeft.z, localCorners.frontLeft.z, localPoint.z);

        // return the UV
        return new Vector2(x, z);
    }

    /// <summary>
    /// Updates the lines to represent the aim zone
    /// </summary>
    private void UpdateLine()
    {
        m_leftLine.SetPositions(new Vector3[] { m_bl, m_fl });
        m_midLine.SetPositions(new Vector3[] { (m_bl + m_br) / 2.0f, (m_fl + m_fr) / 2.0f });
        m_rightLine.SetPositions(new Vector3[] { m_br, m_fr });

        UpdateZoneColors(m_ZoneColor);
        UpdateLineColors(m_leftStartColor, m_leftEndColor, m_midStartColor, m_midEndColor, m_rightStartColor, m_rightEndColor);
    }

    /// <summary>
    /// Calculates the damage multiplier based on position within the aim zone. <br/>
    /// Performs the same action as the shader, where damage falls off along length as well as width.
    /// </summary>
    /// <param name="uv"></param>
    /// <param name="_falloffMult"></param>
    /// <returns></returns>
    private float CalcDmgMult_float(Vector2 uv, float _falloffMult)
    {
        float lengthVal = 1.0f;

        // calc far dmg falloff
        if (uv.y >= 0.75f)
        {
            lengthVal *= (1.0f - ((uv.y - 0.75f) / 0.25f));
        }

        // calc close dmg falloff
        if (uv.y <= 0.1)
        {
            lengthVal *= (1.0f - ((0.1f - uv.y) / 0.1f));
        }

        // if behind or too far, no dmg
        if (uv.y > 1.0 || uv.y < 0.0)
        {
            return 0.0f;
        }

        // make width var be 1 in the middle, and reach 0 at sides
        float widthVal = 1.0f;
        float currentWidth = uv.x + 0.5f;
        if (currentWidth > 1.0f)
        {
            currentWidth = 1.0f - (currentWidth - 1.0f);
        }

        // calc width dmg falloff
        if (currentWidth > 0.5)
        {
            widthVal = currentWidth * 2.0f - 1.0f;
        }

        // if too wide, no dmg
        if (currentWidth <= 0.5f)
        {
            return 0.0f;
        }

        // use falloff to calculate horiz dmg
        widthVal = Mathf.Lerp(1.0f - _falloffMult, 1.0f, widthVal);
        widthVal = Mathf.Clamp(widthVal, 0.0f, 1.0f);


        return (lengthVal * widthVal);
    }

    /// <summary>
    /// Calculates the damage multiplier based on position within the aim zone.
    /// </summary>
    /// <param name="_worldPoint"></param>
    /// <param name="_falloffMult"></param>
    /// <returns></returns>
    public float CalcDmgMult_float(Vector3 _worldPoint, float _falloffMult)
    {
        Vector2 uv = CalculateUVFromWorld(_worldPoint);
        return CalcDmgMult_float(uv, _falloffMult);
    }

    /// <summary>
    /// Hide the zone
    /// </summary>
    public void Hide()
    {
        // hide the mesh renderer
        m_meshRenderer.enabled = false;

        // hide lines
        m_leftLine.enabled = false;
        m_midLine.enabled = false;
        m_rightLine.enabled = false;
    }
    /// <summary>
    /// Show the zone
    /// </summary>
    public void Show()
    {
        // show the mesh renderer
        m_meshRenderer.enabled = true;

        // show the lines
        m_leftLine.enabled = true;
        m_midLine.enabled = true;
        m_rightLine.enabled = true;
    }

    

    public void LoadRed()
    {
        m_ZoneColor.r = PlayerPrefs.GetFloat("AimZoneRed", 1.0f);
    }
    public void LoadGreen()
    {
        m_ZoneColor.g = PlayerPrefs.GetFloat("AimZoneGreen", 0.0f);
    }
    public void LoadBlue()
    {
        m_ZoneColor.b = PlayerPrefs.GetFloat("AimZoneBlue", 0.0f);
    }
    public void LoadAlpha()
    {
        m_ZoneColor.a = PlayerPrefs.GetFloat("AimZoneAlpha", 0.5f);
    }
    public void LoadColors()
    {
        LoadRed();
        LoadGreen();
        LoadBlue();
        LoadAlpha();

        AimZoneSettings settings = GameObject.FindObjectOfType<AimZoneSettings>(true);
        if (settings != null)
        {
            settings.SetSliders(m_ZoneColor);
        }
    }

    public void SaveRed()
    {
        PlayerPrefs.SetFloat("AimZoneRed", m_ZoneColor.r);
    }
    public void SaveGreen()
    {
        PlayerPrefs.SetFloat("AimZoneGreen", m_ZoneColor.g);
    }
    public void SaveBlue()
    {
        PlayerPrefs.SetFloat("AimZoneBlue", m_ZoneColor.b);
    }
    public void SaveAlpha()
    {
        PlayerPrefs.SetFloat("AimZoneAlpha", m_ZoneColor.a);
    }
    public void SaveColors()
    {
        SaveRed();
        SaveGreen();
        SaveBlue();
        SaveAlpha();
    }

    public void SetColorFromSettings(Color _color)
    {
        Color endColor = new Color(_color.r, _color.g, _color.b, 0.0f);

        m_ZoneColor = _color;
        m_leftStartColor = _color;
        m_midStartColor = _color;
        m_rightStartColor = _color;
        m_leftEndColor = endColor;
        m_midEndColor = endColor;
        m_rightEndColor = endColor;

        SaveColors();
    }
}