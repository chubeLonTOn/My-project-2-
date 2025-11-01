using System;
using System.Collections.Generic;
using UnityEngine;

public static class GizmoHelper
{
    public static void DrawDisc(Vector3 position, Vector3 normal, float radius, int vertices = 8)
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>(PlotCircle3D(normal, radius, vertices));
        List<int> tris = new List<int>();
        verts.Insert(0, Vector3.zero);
        for (int i = 0; i < vertices; i++)
        {
            int a = i + 1;
            int b = i + 2;
            if (b > vertices)
                b = 1;
            
            tris.AddRange(new[] { 0, a, b });
        }
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        
        Gizmos.DrawMesh(mesh, 0, position);
    }

    public static void DrawCircle(Vector3 position, Vector3 normal, float radius, int vertices = 8)
    {
        Vector3[] points = PlotCircle3D(normal, radius, vertices);
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawLine(position + points[i], position + points[(i + 1) % points.Length]);
        }
    }
    
    public static void DrawWireCapsule(Vector3 position, Vector3 normal, float radius, float height, int vertices = 8)
    {
        Vector3 topCenter = position + (normal.normalized * ((height / 2) - radius));
        Vector3 bottomCenter = position + (-normal.normalized * ((height / 2) - radius));
        
        DrawCircle(topCenter, normal, radius, vertices);
        DrawCircle(bottomCenter, normal, radius, vertices);
        
        Vector3 perp = Vector3.Cross(normal, Vector3.up);
        if (perp.sqrMagnitude < 0.001f)
            perp = Vector3.Cross(normal, Vector3.right);
        perp.Normalize();

        var arc = PlotArc3D(perp, radius, 0, -180, vertices);
        DrawCurvedLine(arc, false, topCenter);
        arc =  PlotArc3D(Quaternion.AngleAxis(90, normal) * perp, radius, 0, -180, vertices);
        DrawCurvedLine(arc, false, topCenter);
        arc =  PlotArc3D(perp, radius, 0, 180, vertices);
        DrawCurvedLine(arc, false, bottomCenter);
        arc =  PlotArc3D(Quaternion.AngleAxis(90, normal) * perp, radius, 0, 180, vertices);
        DrawCurvedLine(arc, false, bottomCenter);
        
        Gizmos.DrawLine(topCenter + (perp * radius), bottomCenter + (perp * radius));
        Gizmos.DrawLine(topCenter - (perp * radius), bottomCenter - (perp * radius));
        perp = Quaternion.AngleAxis(90, normal) * perp;
        Gizmos.DrawLine(topCenter + (perp * radius), bottomCenter + (perp * radius));
        Gizmos.DrawLine(topCenter - (perp * radius), bottomCenter - (perp * radius));
    }

    public static void DrawCapsule(Vector3 position, Vector3 normal, float radius, float height, int sides = 16, int domeRings = 16)
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        
        Vector3 topCenter = position + (normal.normalized * ((height / 2) - radius));
        Vector3 bottomCenter = position + (-normal.normalized * ((height / 2) - radius));

        Vector3[] ring = PlotCircle3D(normal, radius, sides);
        
        //Center
        for (int i = 0; i < ring.Length; i++)
        {
            verts.AddRange(new []
            {
                topCenter + ring[i],
                bottomCenter + ring[i],
            });

            if (i > 0)
            {
                int x = (i * 2) + 1;
                tris.AddRange(new [] {x - 3, x - 2, x - 1});
                tris.AddRange(new [] {x - 2, x, x - 1});
            }
        }
        tris.AddRange(new [] { (sides * 2) - 2, (sides * 2) - 1, 0});
        tris.AddRange(new [] {(sides * 2) - 1, 1, 0});
        
        //DrawDome(topCenter, true);
        DrawDome(bottomCenter, false);
        
        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        Gizmos.DrawMesh(mesh);

        void DrawDome(Vector3 center, bool up)
        {
            Vector3 direction = up ? normal : -normal;
            for (int i = 1; i < domeRings; i++)
            {
                float n = (float)i / domeRings;
                float r = SphereSectionRadius(n * (up ? 1 : -1), radius);
                var points = PlotCircle3D(normal, r, sides);
                float offset = radius * n;
                int ringStartIndex = verts.Count;
                int previousRingStartIndex = i == 0 ? 0 : verts.Count - sides;
                for (int p = 0; p < points.Length; p++)
                {
                    verts.Add(center + (direction * offset) + points[p]);
                    if (p > 0)
                    {
                        int top = ringStartIndex + p;
                        if(i == 1)
                        {
                            if(up)
                            {
                                tris.AddRange(new[] { top - 1, (p - 1) * 2, top });
                                tris.AddRange(new[] { (p - 1) * 2, p * 2, top });
                            }
                            else
                            {
                                tris.AddRange(new[] { top - 1, (p) * 2, top });
                                tris.AddRange(new[] { (p) * 2, (p + 1) * 2, top });
                            }
                        }
                        else
                        {
                            int bottom = previousRingStartIndex + p;
                            tris.AddRange(new[] { top - 1, bottom - 1, top });
                            tris.AddRange(new[] { bottom - 1, bottom, top });
                        }
                    }
                }
                if(i == 1)
                {
                    tris.AddRange(new[] { ringStartIndex - 1 + sides, (sides - 1) * 2, ringStartIndex });
                    tris.AddRange(new[] { (sides - 1) * 2, 0, ringStartIndex });
                }
                else
                {
                    
                    tris.AddRange(new[] { ringStartIndex - 1 + sides, previousRingStartIndex - 1 + sides, ringStartIndex });
                    tris.AddRange(new[] { previousRingStartIndex - 1 + sides, previousRingStartIndex, ringStartIndex });
                }
            }
            int centerIndex = verts.Count;
            int topRing = verts.Count - sides;
            verts.Add(center + (direction * radius));
            for (int i = 1; i < sides; i++)
            {
                tris.AddRange(new[] { centerIndex, (topRing + i) - 1, topRing + i });
            }
            tris.AddRange(new[] { centerIndex, (topRing + sides) - 1, topRing });
            
        }
    }

    private static void DrawCurvedLine(Vector3[] points, bool closed, Vector3 offset = default)
    {
        for(int i = 1; i < points.Length; i++)
            Gizmos.DrawLine(offset + points[i - 1], offset + points[i]);
        if (closed)
            Gizmos.DrawLine(offset + points[0], offset + points[^1]);
    }

    private static Vector3[] PlotArc3D(Vector3 normal, float radius, float startAngle, float endAngle, int vertices)
    {
        float dif = Mathf.Abs(endAngle - startAngle);
        int verts = Mathf.CeilToInt((dif / 360f) * vertices);
        Vector3[] points = new Vector3[verts];
        Vector3 perp = Vector3.Cross(normal, Vector3.up);
        if (perp.sqrMagnitude < 0.001f)
            perp = Vector3.Cross(normal, Vector3.right);
        perp.Normalize();
        float angleStep = (endAngle - startAngle) / (verts - 1);
        for (int i = 0; i < verts; i++)
        {
            points[i] = Quaternion.AngleAxis(startAngle + (angleStep * i), normal) * (perp * radius);
        }
        return points;
    }
    
    private static Vector3[] PlotCircle3D(Vector3 normal, float radius, int vertices)
    {
        if(vertices < 3)
        {
            Debug.LogError("Unable to to draw circle with less than 3 vertices!");
            return Array.Empty<Vector3>();
        }    
        Vector3 perp = Vector3.Cross(normal, Vector3.up);
        if (perp.sqrMagnitude < 0.001f)
            perp = Vector3.Cross(normal, Vector3.right);
        perp.Normalize();
        float angleStep = 360f / (vertices);
        Vector3[] points = new Vector3[vertices];
        for (int i = 0; i < vertices; i++)
        {
            points[i] = Quaternion.AngleAxis(angleStep * i, normal) * (perp * radius);
        }
        return points;
    }
    
    private static float SphereSectionRadius(float normalizedDistance, float radius)
    {
        normalizedDistance = Mathf.Clamp(normalizedDistance, -1f, 1f);
        return radius * MathF.Sqrt(1f - normalizedDistance * normalizedDistance);
    }
    
    private static float WrapDegrees(float degrees)
    {
        degrees %= 360f;
        if (degrees < 0f)
            degrees += 360f;
        return degrees;
    }
}
