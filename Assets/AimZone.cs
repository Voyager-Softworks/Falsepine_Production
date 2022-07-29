using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimZone : MonoBehaviour
{
    private Mesh m_mesh;
    private MeshRenderer m_meshRenderer;

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

    private Vector3 m_fl { get{ return m_corners.frontLeft; } set { m_corners.frontLeft = value; } }
    private Vector3 m_fr { get { return m_corners.frontRight; } set { m_corners.frontRight = value; } }
    private Vector3 m_br { get { return m_corners.backRight; } set { m_corners.backRight = value; } }
    private Vector3 m_bl { get { return m_corners.backLeft; } set { m_corners.backLeft = value; } }

    public Vector3 fl { get{ return m_corners.frontLeft; } }
    public Vector3 fr { get { return m_corners.frontRight; } }
    public Vector3 br { get { return m_corners.backRight; } }
    public Vector3 bl { get { return m_corners.backLeft; } }

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
    private void Awake() {
        //get mesh
        m_mesh = GetComponent<MeshFilter>().mesh;
        //get mesh renderer
        m_meshRenderer = GetComponent<MeshRenderer>();

        //hide aim zone
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        // // if any of the local points are not the same as the mesh points, update the mesh
        // if (m_mesh.vertices[0] != fl || m_mesh.vertices[1] != fr || m_mesh.vertices[2] != br || m_mesh.vertices[3] != bl)
        // {
        //     UpdateMesh();
        // }
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

        UpdateMesh();
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
    /// Update the actual mesh of the aim zone (the quad)
    /// </summary>
    public void UpdateMesh(){
        // set the vertices of the aimQuad
        m_mesh.SetVertices(GetLocalCorners().ToList());

        //set tris to match the aimLines
        m_mesh.SetTriangles(new List<int>() { 0, 1, 2, 0, 2, 3 }, 0);

        m_mesh.RecalculateBounds();
        m_mesh.RecalculateNormals();
    }

    public void Hide()
    {
        // hide the mesh renderer
        m_meshRenderer.enabled = false;
    }
    public void Show()
    {
        // show the mesh renderer
        m_meshRenderer.enabled = true;
    }
}