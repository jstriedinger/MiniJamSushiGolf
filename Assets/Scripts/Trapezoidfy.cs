using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Trapezoidify : MonoBehaviour
{
    [Range(0.1f, 2f)]
    public float topScaleFactor = 0.5f; // 1 = cube, 0.5 = strong taper

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        Vector3[] vertices = mesh.vertices;

        // Identify top vertices by y value
        float topY = float.MinValue;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y > topY)
                topY = vertices[i].y;
        }

        // Scale top vertices inward or outward
        for (int i = 0; i < vertices.Length; i++)
        {
            if (Mathf.Approximately(vertices[i].y, topY))
            {
                vertices[i].x *= topScaleFactor;
                vertices[i].z *= topScaleFactor;
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
