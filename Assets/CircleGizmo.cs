using UnityEngine;
using System.Collections;

public class CircleGizmo : MonoBehaviour {

    public int resolution = 10;

	private void OnDrawGizmosSelected()
    {
        float step = 2f / resolution;

        for (int i = 0; i <= resolution; i++)
        {
            ShowPoints(i * step - 1f, -1f);
            ShowPoints(i * step - 1f, 1f);
        }

        for (int i = 0; i <= resolution; i++)
        {
            ShowPoints(-1f, i * step - 1f);
            ShowPoints(1f, i * step - 1f);
        }

    }

    private void ShowPoints(float x, float y)
    {
        Vector2 square = new Vector2(x, y);

        //Vector2 circle = square.normalized
        Vector2 circle = new Vector2(square.x * Mathf.Sqrt(1 - square.y * square.y * 0.5f), square.y * Mathf.Sqrt(1 - square.x * square.x * 0.5f)); //the distance between adjacent points is more uniform than what we got with our first approach
        Vector2 origin = Vector2.zero;

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(square, 0.025f);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(circle, 0.025f);
        Gizmos.DrawLine(origin, circle);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(circle, square);
    }
}
