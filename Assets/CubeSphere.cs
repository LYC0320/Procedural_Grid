using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeSphere : MonoBehaviour
{

    public int gridSize;
    //private float roundness;
    //public int roundness;
    public float radius = 1;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Color32[] cubeUV;

    private void Awake()
    {
        Generate();
        
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Sphere";

        CreateVertices();
        CreateTriangle();
        CreateColliders();
    }

    private void CreateVertices()
    {
        int cornerVertices = 8;
        int edgeVertices = 4 * (gridSize + gridSize + gridSize - 3);
        int faceVertices = 2 * ((gridSize - 1) * (gridSize - 1) + (gridSize - 1) * (gridSize - 1) + (gridSize - 1) * (gridSize - 1));
        int v = 0;

        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        normals = new Vector3[vertices.Length];
        cubeUV = new Color32[vertices.Length];

        //WaitForSeconds wait = new WaitForSeconds(0.05f);

        for (int y = 0; y <= gridSize; y++)
        {
            for (int x = 0; x <= gridSize; x++)
            {
                SetVertices(v++, x, y, 0);
            }

            for (int z = 1; z <= gridSize; z++)
            {
                SetVertices(v++, gridSize, y, z);
            }

            for (int x = gridSize - 1; x >= 0; x--)
            {
                SetVertices(v++, x, y, gridSize);
            }

            for (int z = gridSize - 1; z >= 1; z--)
            {
                SetVertices(v++, 0, y, z);
            }
        }

        for (int z = 1; z <= gridSize - 1; z++)
        {
            for (int x = 1; x <= gridSize - 1; x++)
            {
                SetVertices(v++, x, gridSize, z);
            }
        }

        for (int z = 1; z <= gridSize - 1; z++)
        {
            for (int x = 1; x <= gridSize - 1; x++)
            {
                SetVertices(v++, x, 0, z);
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.colors32 = cubeUV;
    }

    private void SetVertices(int i, int x, int y, int z)
    {
        /*
        Vector3 inner = vertices[i] = new Vector3(x, y, z);

        if (inner.x < roundness)
            inner.x = roundness;
        else if (inner.x > gridSize - roundness)
            inner.x = gridSize - roundness;

        if (inner.y < roundness)
            inner.y = roundness;
        else if (inner.y > gridSize - roundness)
            inner.y = gridSize - roundness;

        if (z < roundness)
        {
            inner.z = roundness;
        }
        else if (z > gridSize - roundness)
        {
            inner.z = gridSize - roundness;
        }

        normals[i] = (vertices[i] - inner).normalized;
        vertices[i] = inner + normals[i] * roundness;
        cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
        */

        /* // First approch
        Vector3 v = new Vector3(x, y, z) * 2f / gridSize - Vector3.one;
        normals[i] = v.normalized;
        vertices[i] = normals[i] * radius;
        cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
        */

        Vector3 v = new Vector3(x, y, z) * 2f / gridSize - Vector3.one;

        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;

        Vector3 s;

        s.x = v.x * Mathf.Sqrt(1 - y2 * 0.5f - z2 * 0.5f + y2 * z2 / 3f);
        s.y = v.y * Mathf.Sqrt(1 - x2 * 0.5f - z2 * 0.5f + x2 * z2 / 3f);
        s.z = v.z * Mathf.Sqrt(1 - x2 * 0.5f - y2 * 0.5f + x2 * y2 / 3f);

        normals[i] = s;
        vertices[i] = normals[i] * radius;
        cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
    }

    private void CreateTriangle()
    {
        int[] trianglesZ = new int[gridSize * gridSize * 12];
        int[] trianglesY = new int[gridSize * gridSize * 12];
        int[] trianglesX = new int[gridSize * gridSize * 12];
        int tZ = 0, tY = 0, tX = 0;


        int quad = 2 * (gridSize * gridSize + gridSize * gridSize + gridSize * gridSize);
        int[] triangles = new int[quad * 6];
        int ring = 2 * (gridSize + gridSize);
        int i = 0, v = 0;
        for (int y = 0; y < gridSize; y++, v++)
        {

            /*
            for (int q = 0; q < ring - 1; q++, v++)
            {
                i = SetQuad(triangles, i, v, v + 1, v + ring, v + ring + 1);
            }
            */

            for (int q = 0; q < gridSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }

            for (int q = 0; q < gridSize; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }

            for (int q = 0; q < gridSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }

            for (int q = 0; q < gridSize - 1; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }

            tX = SetQuad(trianglesX, tX, v, v + 1 - ring, v + ring, v + 1);
        }

        tY = CreateTopFace(trianglesY, tY, ring);
        tY = CreateBottomFace(trianglesY, tY, ring);

        //mesh.triangles = trianglesZ;

        mesh.subMeshCount = 3;
        mesh.SetTriangles(trianglesZ, 0);
        mesh.SetTriangles(trianglesX, 1);
        mesh.SetTriangles(trianglesY, 2);
    }

    private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;

        return i + 6;
    }

    private int CreateTopFace(int[] triangles, int i, int ring)
    {
        int v = ring * gridSize;



        for (int q = 0; q < gridSize - 1; q++, v++)
        {
            i = SetQuad(triangles, i, v, v + 1, v + ring - 1, v + ring);
        }

        i = SetQuad(triangles, i, v, v + 1, v + ring - 1, v + 2);


        int vmin = ring * (gridSize + 1) - 1;
        int vmid = vmin + 1;
        int vmax = v + 2;



        for (int z = 0; z < gridSize - 2; z++)
        {
            i = SetQuad(triangles, i, vmin, vmid, vmin - 1, vmid + gridSize - 1);

            for (int q = 0; q < gridSize - 2; q++)
            {
                i = SetQuad(triangles, i, vmid, vmid + 1, vmid + gridSize - 1, vmid + gridSize);
                //i = i + 6;
                vmid++;
            }

            i = SetQuad(triangles, i, vmid, vmax, vmid + gridSize - 1, vmax + 1);
            //i = i + 6;

            vmin--;
            vmid++;
            vmax++;
        }



        int vtop = vmin;

        i = SetQuad(triangles, i, vtop, vmid, --vtop, --vtop);

        for (int x = 0; x < gridSize - 2; x++)
        {
            i = SetQuad(triangles, i, vmid, ++vmid, vtop, --vtop);
        }


        i = SetQuad(triangles, i, vmid, vtop - 2, vtop, --vtop);
        return i;
    }

    private int CreateBottomFace(int[] triangles, int i, int ring)
    {

        int vmid = vertices.Length - (gridSize - 1) * (gridSize - 1);
        int v = 1;

        i = SetQuad(triangles, i, ring - 1, vmid, 0, 1);

        for (int q = 0; q < gridSize - 2; q++, v++)
        {
            i = SetQuad(triangles, i, vmid, ++vmid, v, v + 1);
        }

        i = SetQuad(triangles, i, vmid, v + 2, v, v + 1);


        int vmin = ring - 1;
        int vmax = v + 2;

        vmid++;

        for (int z = 0; z < gridSize - 2; z++)
        {
            i = SetQuad(triangles, i, vmin - 1, vmid, vmin, vmid - gridSize + 1);

            for (int q = 0; q < gridSize - 2; q++)
            {
                i = SetQuad(triangles, i, vmid, vmid + 1, vmid - gridSize + 1, vmid - gridSize + 2);
                vmid++;
            }

            i = SetQuad(triangles, i, vmid, vmax + 1, vmid - gridSize + 1, vmax);

            vmin--;
            vmid++;
            vmax++;

        }



        int vtop = vmin - 1;

        vmid = vmid - gridSize + 1;


        i = SetQuad(triangles, i, vtop, --vtop, vmin, vmid);

        for (int x = 0; x < gridSize - 2; x++)
        {
            i = SetQuad(triangles, i, vtop, --vtop, vmid, ++vmid);
        }


        i = SetQuad(triangles, i, vtop, --vtop, vmid, vmax);


        return i;
    }

    private void CreateColliders()
    {
        /*
        AddBoxColliders(gridSize, gridSize - 2 * roundness, gridSize - 2 * roundness);
        AddBoxColliders(gridSize - 2 * roundness, gridSize, gridSize - 2 * roundness);
        AddBoxColliders(gridSize - 2 * roundness, gridSize - 2 * roundness, gridSize);

        Vector3 min = Vector3.one * roundness;
        Vector3 half = new Vector3(gridSize, gridSize, gridSize) * 0.5f;
        Vector3 max = new Vector3(gridSize, gridSize, gridSize) - min;

        AddCapsuleCollider(0, half.x, min.y, min.z);
        AddCapsuleCollider(0, half.x, max.y, min.z);
        AddCapsuleCollider(0, half.x, min.y, max.z);
        AddCapsuleCollider(0, half.x, max.y, max.z);

        AddCapsuleCollider(1, min.x, half.y, min.z);
        AddCapsuleCollider(1, max.x, half.y, min.z);
        AddCapsuleCollider(1, min.x, half.y, max.z);
        AddCapsuleCollider(1, max.x, half.y, max.z);

        AddCapsuleCollider(2, min.x, min.y, half.z);
        AddCapsuleCollider(2, min.x, max.y, half.z);
        AddCapsuleCollider(2, max.x, min.y, half.z);
        AddCapsuleCollider(2, max.x, max.y, half.z);
        */

        SphereCollider c = gameObject.AddComponent<SphereCollider>();
    }

    private void AddBoxColliders(float x, float y, float z)
    {
        BoxCollider c = gameObject.AddComponent<BoxCollider>();
        c.size = new Vector3(x, y, z);
    }

    /*
    private void AddCapsuleCollider(int direction, float x, float y, float z)
    {
        CapsuleCollider c = gameObject.AddComponent<CapsuleCollider>();
        c.center = new Vector3(x, y, z);
        c.direction = direction;
        c.radius = roundness;
        c.height = c.center[direction] * 2f;
    }
    
    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.TransformPoint(vertices[i]), normals[i]);
        }
    }
    */
}