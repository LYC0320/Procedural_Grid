using UnityEngine;
using System.Collections;


[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class Grid : MonoBehaviour {

    public int xSize, ySize;
    private Vector3[] vertices;
    private Vector2[] uv;
    private Vector4[] tangent;
    private Mesh mesh;

    private void Awake()
    {
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        uv = new Vector2[vertices.Length];
        tangent = new Vector4[vertices.Length];

        WaitForSeconds wait = new WaitForSeconds(0);

        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Procedural Grid";

        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++) 
            {
                
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangent[i] = new Vector4(1.0f, 0.0f, 0.0f, -1.0f);
                //vertices[i] = transform.TransformPoint(vertices[i]);
                yield return wait;
                //transform.TransformPoint(vertices[i]);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangent;

        int[] triangles = new int[xSize * ySize * 6];

        for (int j = 0, ti = 0; j < ySize; j++, ti += 11) 
        {
            for (int i = 0, vi = 0; i < xSize; i++, vi += 6)
            {
                triangles[vi+j*60] = i + ti;
                triangles[vi + 1+j*60] = triangles[vi + 4+j*60] = xSize + 1 + i + ti;
                triangles[vi + 2+j*60] = triangles[vi + 3+j*60] = i + 1 + ti;
                triangles[vi + 5+j*60] = xSize + 2 + i + ti;

                //mesh.triangles = triangles;
                //yield return wait;

            }
        }


        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

   

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 0.1f);
            //Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }

}
