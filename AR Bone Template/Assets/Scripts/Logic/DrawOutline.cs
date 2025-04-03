using System.Threading.Tasks;
using UnityEngine;

public class DrawOutline : MonoBehaviour
{
    // Field assigned in inspector
    [SerializeField]
    private bool m_IsSphere;
    [SerializeField]
    private Color m_OutlineColor = Color.yellow;
    [SerializeField]
    private int m_SphereSegments = 64; // Number of segments for the sphere outline
    // Public

    // Private
    private MeshFilter meshFilter;


    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    private void OnRenderObject()
    {
        if (meshFilter == null) return;

        GL.PushMatrix();
        if (m_IsSphere) {
            GL.MultMatrix(Matrix4x4.identity);
        } else {
            GL.MultMatrix(transform.localToWorldMatrix);
        }

        Material lineMaterial = new(Shader.Find("Graphics Tools/Standard"));
        lineMaterial.color = m_OutlineColor;
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);

        if (m_IsSphere) {
            DrawSphereOutline(meshFilter.mesh.bounds);
        } else {
            DrawRectOutline(meshFilter.mesh.bounds);
        }

        GL.End();
        GL.PopMatrix();
    }

    private void DrawRectOutline(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        Vector3[] vertices = {
            center + new Vector3(-extents.x, -extents.y, -extents.z),
            center + new Vector3(extents.x, -extents.y, -extents.z),
            center + new Vector3(extents.x, -extents.y, extents.z),
            center + new Vector3(-extents.x, -extents.y, extents.z),
            center + new Vector3(-extents.x, extents.y, -extents.z),
            center + new Vector3(extents.x, extents.y, -extents.z),
            center + new Vector3(extents.x, extents.y, extents.z),
            center + new Vector3(-extents.x, extents.y, extents.z),
        };

        int[] edges = {
            0, 1, 1, 2, 2, 3, 3, 0, // Bottom face
            4, 5, 5, 6, 6, 7, 7, 4, // Top face
            0, 4, 1, 5, 2, 6, 3, 7  // Side edges
        };

        for (int i = 0; i < edges.Length; i += 2) {
            GL.Vertex(vertices[edges[i]]);
            GL.Vertex(vertices[edges[i + 1]]);
        }
    }

    private void DrawSphereOutline(Bounds bounds)
    {
        // Get the radius of the sphere based on the collider and scale
        float radius = GetComponent<SphereCollider>().radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
        Vector3 center = transform.position;  // The center of the sphere

        // Number of "layers" of the outline (to simulate thickness)
        int outlineLayers = 3;

        // Draw several circles slightly offset from each other
        for (int i = 0; i < outlineLayers; i++) {
            float offset = i * 0.00001f; // Offset between circles, adjust based on desired thickness
            DrawCircle(center, radius + offset);
            DrawCircle(center, radius - offset); // Draw on both sides to simulate thickness
        }
    }

    private void DrawCircle(Vector3 center, float radius)
    {
        // Get the camera's position
        Vector3 cameraPosition = Camera.main.transform.position;

        // Calculate the direction from the camera to the sphere's center
        Vector3 toSphere = center - cameraPosition;

        float angle = Vector3.Angle(Camera.main.transform.forward, toSphere);
        Vector3 axis = Vector3.Cross(Camera.main.transform.forward, toSphere);

        // Project the circle onto the camera's view plane
        Vector3 cameraRight = Quaternion.AngleAxis(angle, axis) * Camera.main.transform.right; // Right vector in world space
        Vector3 cameraUp = Quaternion.AngleAxis(angle, axis) * Camera.main.transform.up;       // Up vector in world space

        // Scale the camera's right and up vectors accordingly
        cameraRight *= radius;
        cameraUp *= radius;

        // Draw a circle by connecting points in a loop
        for (int i = 0; i <= m_SphereSegments; i++) {
            float angle1 = (i * 2 * Mathf.PI) / m_SphereSegments;
            float angle2 = ((i + 1) * 2 * Mathf.PI) / m_SphereSegments;

            // Calculate the positions of the points on the circle based on the angle
            Vector3 point1 = center + Mathf.Cos(angle1) * cameraRight + Mathf.Sin(angle1) * cameraUp;
            Vector3 point2 = center + Mathf.Cos(angle2) * cameraRight + Mathf.Sin(angle2) * cameraUp;

            GL.Vertex(point1);
            GL.Vertex(point2);
        }
    }


    public void SetColor(Color color) => m_OutlineColor = color;
}
