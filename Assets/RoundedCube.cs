using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoundedCube : MonoBehaviour
{

    public int xSize, ySize, zSize;
    public int roundness;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
       

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Cube";

        CreateVertices();
        CreateTriangle();
        CreateColliders();
    }

    private void CreateVertices()
    {
        int cornerVertices = 8;
        int edgeVertices = 4 * (xSize + ySize + zSize - 3);
        int faceVertices = 2 * ((xSize - 1) * (ySize - 1) + (xSize - 1) * (zSize - 1) + (ySize - 1) * (zSize - 1));
        int v = 0;

        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        normals = new Vector3[vertices.Length];

        //WaitForSeconds wait = new WaitForSeconds(0.05f);

        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                SetVertices(v++, x, y, 0);
            }

            for (int z = 1; z <= zSize; z++)
            {
                SetVertices(v++, xSize, y, z);
            }

            for (int x = xSize - 1; x >= 0; x--)
            {
                SetVertices(v++, x, y, zSize);
            }

            for (int z = zSize - 1; z >= 1; z--)
            {
                SetVertices(v++, 0, y, z);
            }
        }

        for (int z = 1; z <= zSize - 1; z++)
        {
            for (int x = 1; x <= xSize - 1; x++)
            {
                SetVertices(v++, x, ySize, z);
            }
        }

        for (int z = 1; z <= zSize - 1; z++)
        {
            for (int x = 1; x <= xSize - 1; x++)
            {
                SetVertices(v++, x, 0, z);
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
    }

    private void SetVertices(int i, int x, int y, int z)
    {
        Vector3 inner = vertices[i] = new Vector3(x, y, z);

        if (inner.x < roundness)
            inner.x = roundness;
        else if (inner.x > xSize - roundness)
            inner.x = xSize - roundness;

        if (inner.y < roundness)
            inner.y = roundness;
        else if (inner.y > ySize - roundness)
            inner.y = ySize - roundness;

        if (z < roundness)
        {
            inner.z = roundness;
        }
        else if (z > zSize - roundness)
        {
            inner.z = zSize - roundness;
        }

        normals[i] = (vertices[i] - inner).normalized;
        vertices[i] = inner + normals[i] * roundness;
    }

    private void CreateTriangle()
    {
        int[] trianglesZ = new int[xSize * ySize * 12];
        int[] trianglesY = new int[xSize * zSize * 12];
        int[] trianglesX = new int[ySize * zSize * 12];
        int tZ = 0, tY = 0, tX = 0;


        int quad = 2 * (xSize * ySize + xSize * zSize + ySize * zSize);
        int[] triangles = new int[quad * 6];
        int ring = 2 * (xSize + zSize);
        int i = 0, v = 0;
        for (int y = 0; y < ySize; y++, v++) 
        {

            /*
            for (int q = 0; q < ring - 1; q++, v++)
            {
                i = SetQuad(triangles, i, v, v + 1, v + ring, v + ring + 1);
            }
            */

            for (int q = 0; q < xSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }

            for (int q = 0; q < zSize; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }

            for (int q = 0; q < xSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }

            for (int q = 0; q < zSize - 1; q++, v++) 
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
        int v = ring * ySize;



        for (int q = 0; q < xSize - 1; q++, v++)
        {
            i = SetQuad(triangles, i, v, v + 1, v + ring - 1, v + ring);
        }

        i = SetQuad(triangles, i, v, v + 1, v + ring - 1, v + 2);


        int vmin = ring * (ySize + 1) - 1;
        int vmid = vmin + 1;
        int vmax = v + 2;



        for (int z = 0; z < zSize - 2; z++)
        {
            i = SetQuad(triangles, i, vmin, vmid, vmin - 1, vmid + xSize - 1);

            for (int q = 0; q < xSize - 2; q++)
            {
                i = SetQuad(triangles, i, vmid, vmid + 1, vmid + xSize - 1, vmid + xSize);
                //i = i + 6;
                vmid++;
            }

            i = SetQuad(triangles, i, vmid, vmax, vmid + xSize - 1, vmax + 1);
            //i = i + 6;

            vmin--;
            vmid++;
            vmax++;
        }



        int vtop = vmin;

        i = SetQuad(triangles, i, vtop, vmid, --vtop, --vtop);

        for (int x = 0; x < xSize - 2; x++)
        {
            i = SetQuad(triangles, i, vmid, ++vmid, vtop, --vtop);
        }


        i = SetQuad(triangles, i, vmid, vtop - 2, vtop, --vtop);
        return i;
    }

    private int CreateBottomFace(int[] triangles,int i,int ring)
    {

        int vmid = vertices.Length - (xSize - 1) * (zSize - 1);
        int v = 1;

        i = SetQuad(triangles, i, ring - 1, vmid, 0, 1);

        for (int q = 0; q < xSize - 2; q++, v++)
        {
            i = SetQuad(triangles, i, vmid, ++vmid, v, v + 1);
        }

        i = SetQuad(triangles, i, vmid, v + 2, v, v + 1);


        int vmin = ring - 1;
        int vmax = v + 2;

        vmid++;

        for (int z = 0; z < zSize - 2; z++)
        {
            i = SetQuad(triangles, i, vmin - 1, vmid, vmin, vmid - xSize + 1);
            
            for (int q = 0; q < xSize - 2; q++)
            {
                i = SetQuad(triangles, i, vmid, vmid + 1, vmid - xSize + 1, vmid - xSize + 2);
                vmid++;
            }

            i = SetQuad(triangles, i, vmid, vmax + 1, vmid - xSize + 1, vmax);

            vmin--;
            vmid++;
            vmax++;
            
        }


        
        int vtop = vmin-1;

        vmid = vmid - xSize + 1;


        i = SetQuad(triangles, i, vtop, --vtop, vmin, vmid);

        for (int x = 0; x < xSize - 2; x++)
        {
            i = SetQuad(triangles, i, vtop, --vtop, vmid, ++vmid);
        }


        i = SetQuad(triangles, i, vtop, --vtop, vmid, vmax);
        

        return i;
    }

    private void CreateColliders()
    {
        AddBoxColliders(xSize, ySize - 2 * roundness, zSize - 2 * roundness);
        AddBoxColliders(xSize - 2 * roundness, ySize, zSize - 2 * roundness);
        AddBoxColliders(xSize - 2 * roundness, ySize - 2 * roundness, zSize);
    }

    private void AddBoxColliders(float x, float y, float z)
    {
        BoxCollider c = gameObject.AddComponent<BoxCollider>();
        c.size = new Vector3(x, y, z);
    }
    /*
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