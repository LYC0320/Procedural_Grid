using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cube : MonoBehaviour
{

    public int xSize, ySize, zSize;

    private Mesh mesh;
    private Vector3[] vertices;

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
    }

    private void CreateVertices()
    {
        int cornerVertices = 8;
        int edgeVertices = 4 * (xSize + ySize + zSize - 3);
        int faceVertices = 2 * ((xSize - 1) * (ySize - 1) + (xSize - 1) * (zSize - 1) + (ySize - 1) * (zSize - 1));
        int v = 0;

        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        //WaitForSeconds wait = new WaitForSeconds(0.05f);

        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[v++] = new Vector3(x, y, 0);
                //yield return wait;
            }

            for (int z = 1; z <= zSize; z++)
            {
                vertices[v++] = new Vector3(xSize, y, z);
                //yield return wait;
            }

            for (int x = xSize - 1; x >= 0; x--)
            {
                vertices[v++] = new Vector3(x, y, zSize);
                //yield return wait;
            }

            for (int z = zSize - 1; z >= 1; z--)
            {
                vertices[v++] = new Vector3(0, y, z);
                //yield return wait;
            }
        }

        for (int z = 1; z <= zSize - 1; z++)
        {
            for (int x = 1; x <= xSize - 1; x++)
            {
                vertices[v++] = new Vector3(x, ySize, z);
                //yield return wait;
            }
        }

        for (int z = 1; z <= zSize - 1; z++)
        {
            for (int x = 1; x <= xSize - 1; x++)
            {
                vertices[v++] = new Vector3(x, 0, z);
                //yield return wait;
            }
        }

       

        mesh.vertices = vertices;
    }

    private void CreateTriangle()
    {
        int quad = 2 * (xSize * ySize + xSize * zSize + ySize + zSize);
        int[] triangles = new int[quad * 6];
        int ring = 2 * (xSize + zSize);
        int i = 0, v = 0;
        for (int y = 0; y < ySize; y++, v++) 
        {
            for (int q = 0; q < ring - 1; q++, v++)
            {
                i = SetQuad(triangles, i, v, v + 1, v + ring, v + ring + 1);
            }

            i = SetQuad(triangles, i, v, v + 1 - ring, v + ring, v + 1);
        }

        i = CreateTopFace(triangles, i, ring);

        mesh.triangles = triangles;
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

        return i;
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}