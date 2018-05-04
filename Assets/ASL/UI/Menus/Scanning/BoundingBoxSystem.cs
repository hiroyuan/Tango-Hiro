﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BoundingBoxSystem {
    private Bounds bounds;
    //private SerializableBounds bounds;
    private BoundHolder[] subBounds;
    private int xDirCount;
    private int yDirCount;
    private int zDirCount;
    private Transform trans;
    private Mesh mesh;

    public BoundingBoxSystem()
    {
        
    }

    public BoundingBoxSystem(BoundHolder[] b)
    {
        subBounds = b;
    }

    public BoundingBoxSystem(Bounds b, int x, int y, int z)
    {
        bounds = b;
        //bounds = new SerializableBounds(DrawBoundingBox(list));
        xDirCount = x;
        yDirCount = y;
        zDirCount = z;
        subBounds = new BoundHolder[xDirCount * yDirCount * zDirCount];
    }

    public BoundingBoxSystem(List<GameObject> list, int x, int y, int z)
    {
        bounds = DrawBoundingBox(list);
        //bounds = new SerializableBounds(DrawBoundingBox(list));
        xDirCount = x;
        yDirCount = y;
        zDirCount = z;
        subBounds = new BoundHolder[xDirCount * yDirCount * zDirCount];
    }

    public BoundingBoxSystem(List<GameObject> list, int x, int y, int z, Transform t, Mesh m)
    {
        bounds = DrawBoundingBox(list);
        //bounds = new SerializableBounds(DrawBoundingBox(list));
        xDirCount = x;
        yDirCount = y;
        zDirCount = z;
        subBounds = new BoundHolder[xDirCount * yDirCount * zDirCount];
        trans = t;
        mesh = m;
    }

    public BoundingBoxSystem(int x, int y, int z)
    {
        xDirCount = x;
        yDirCount = y;
        zDirCount = z;
        subBounds = new BoundHolder[xDirCount * yDirCount * zDirCount];
    }

    public void SetBound(Bounds b)
    {
        bounds = b;
    }

    public void SetSplitter(int x, int y, int z)
    {
        xDirCount = x;
        yDirCount = y;
        zDirCount = z;
        subBounds = new BoundHolder[xDirCount * yDirCount * zDirCount];
    }

    public void SetSplitter(List<GameObject> list, int x, int y, int z)
    {
        bounds = DrawBoundingBox(list);
        xDirCount = x;
        yDirCount = y;
        zDirCount = z;
        subBounds = new BoundHolder[xDirCount * yDirCount * zDirCount];
    }

    /// <summary>
    /// This function finds the bounding box for all game objects in the List
    /// passed into the parameter.
    /// Referenced from Unity community and API.
    /// https://answers.unity.com/questions/777855/bounds-finding-box.html
    /// https://docs.unity3d.com/ScriptReference/Bounds.html
    /// </summary>
    /// <param name="l"></param>
    /// <returns>bounds</returns>
    public static Bounds DrawBoundingBox(List<GameObject> l)
    {
        if (l.Count == 0)
        {
            return new Bounds(Vector3.zero, Vector3.one);
        }

        float minX = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxY = -Mathf.Infinity;
        float minZ = Mathf.Infinity;
        float maxZ = -Mathf.Infinity;

        Vector3[] points = new Vector3[8];

        foreach (GameObject go in l)
        {
            getBoundsPointsNoAlloc(go, points);
            foreach (Vector3 v in points)
            {
                if (v.x < minX) minX = v.x;
                if (v.x > maxX) maxX = v.x;
                if (v.y < minY) minY = v.y;
                if (v.y > maxY) maxY = v.y;
                if (v.z < minZ) minZ = v.z;
                if (v.z > maxZ) maxZ = v.z;
            }
        }

        float sizeX = maxX - minX;
        float sizeY = maxY - minY;
        float sizeZ = maxZ - minZ;

        Vector3 center = new Vector3(minX + sizeX / 2.0f, minY + sizeY / 2.0f, minZ + sizeZ / 2.0f);

        return new Bounds(center, new Vector3(sizeX, sizeY, sizeZ));
    }

    public void SplitBounds()
    {
        float sizeX = (bounds.max.x - bounds.min.x) / xDirCount;
        float sizeY = (bounds.max.y - bounds.min.y) / yDirCount;
        float sizeZ = (bounds.max.z - bounds.min.z) / zDirCount;
        Vector3 subSize = new Vector3(sizeX, sizeY, sizeZ);

        Vector3 subCenter;
        int index = 0;
        float xStartPoint = bounds.min.x;
        float yStartPoint = bounds.min.y;
        float zStartPoint = bounds.min.z;
        for (int i = 0; i < zDirCount; i++)
        {
            for (int j = 0; j < yDirCount; j++)
            {
                for (int k = 0; k < xDirCount; k++)
                {
                    subCenter = new Vector3(xStartPoint + sizeX / 2, yStartPoint + sizeY / 2, zStartPoint + sizeZ / 2);
                    subBounds[index++] = new BoundHolder(subCenter, subSize, mesh, xDirCount * yDirCount * zDirCount);
                    xStartPoint += sizeX;
                }
                xStartPoint = bounds.min.x;
                yStartPoint += sizeY;
            }
            xStartPoint = bounds.min.x;
            yStartPoint = bounds.min.y;
            zStartPoint += sizeZ;
        }
    }

    public void SplitMesh()
    {
        putTrianglesIntoBoundingBox(mesh);
        drawNewMesh();
    }

    public void ActivateSubBoundsByIndicator(GameObject indicator)
    {
        for (int i = 0; i < subBounds.Length; i++)
        {
            if (subBounds[i].CheckIntersects(indicator.transform.position))
            {
                subBounds[i].SetStatus(true);
            }
            else
            {
                subBounds[i].SetStatus(false);
            }
        }
    }

    public Bounds GetBound()
    {
        return bounds;
        //return bounds.GetBounds();
    }

    public BoundHolder[] GetSubBounds()
    {
        return subBounds;
    }

    /// <summary>
    /// This function is support function of drawBoundingBox.
    /// Referenced from Unity community.
    /// https://answers.unity.com/questions/777855/bounds-finding-box.html
    /// https://docs.unity3d.com/ScriptReference/Bounds.html
    /// </summary>
    /// <param name="go"></param>
    /// <param name="points"></param>
    private static void getBoundsPointsNoAlloc(GameObject go, Vector3[] points)
    {
        if (points == null || points.Length < 8)
        {
            Debug.Log("Bad Array");
            return;
        }
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (mf == null)
        {
            Debug.Log("No MeshFilter on object");
            for (int i = 0; i < points.Length; i++)
                points[i] = go.transform.position;
            return;
        }

        Transform tr = go.transform;

        Vector3 v3Center = mf.mesh.bounds.center;
        Vector3 v3ext = mf.mesh.bounds.extents;

        points[0] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y + v3ext.y, v3Center.z - v3ext.z));  // Front top left corner
        points[1] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y + v3ext.y, v3Center.z - v3ext.z));  // Front top right corner
        points[2] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y - v3ext.y, v3Center.z - v3ext.z));  // Front bottom left corner
        points[3] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y - v3ext.y, v3Center.z - v3ext.z));  // Front bottom right corner
        points[4] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y + v3ext.y, v3Center.z + v3ext.z));  // Back top left corner
        points[5] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y + v3ext.y, v3Center.z + v3ext.z));  // Back top right corner
        points[6] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y - v3ext.y, v3Center.z + v3ext.z));  // Back bottom left corner
        points[7] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y - v3ext.y, v3Center.z + v3ext.z));  // Back bottom right corner
    }

    private BoundHolder FindBoundingBox(Vector3 vertex)
    {
        //Vector3 worldPoint = trans.TransformPoint(vertex);

        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float minY = bounds.min.y;
        float maxY = bounds.max.y;
        float minZ = bounds.min.z;
        float maxZ = bounds.max.z;

        float xDirSizeOfSubBounds = (maxX - minX) / xDirCount;
        float yDirSizeOfSubBounds = (maxY - minY) / yDirCount;
        float zDirSizeOfSubBounds = (maxZ - minZ) / zDirCount;

        float floatNthSubBoundsForXDir = (vertex.x - minX) / xDirSizeOfSubBounds;
        float floatNthSubBoundsForYDir = (vertex.y - minY) / yDirSizeOfSubBounds;
        float floatNthSubBoundsForZDir = (vertex.z - minZ) / zDirSizeOfSubBounds;

        int nthSubBoundsForXDir = 0;
        int nthSubBoundsForYDir = 0;
        int nthSubBoundsForZDir = 0;

        if (floatNthSubBoundsForXDir == System.Math.Floor(floatNthSubBoundsForXDir) && floatNthSubBoundsForXDir != 0)
            nthSubBoundsForXDir = (int)floatNthSubBoundsForXDir - 1;
        else if (floatNthSubBoundsForXDir == 0)
            nthSubBoundsForXDir = 0;
        else
            nthSubBoundsForXDir = (int)floatNthSubBoundsForXDir;

        if (floatNthSubBoundsForYDir == System.Math.Floor(floatNthSubBoundsForYDir) && floatNthSubBoundsForYDir != 0)
            nthSubBoundsForYDir = (int)floatNthSubBoundsForYDir - 1;
        else if (floatNthSubBoundsForYDir == 0)
            nthSubBoundsForYDir = 0;
        else
            nthSubBoundsForYDir = (int)floatNthSubBoundsForYDir;

        if (floatNthSubBoundsForZDir == System.Math.Floor(floatNthSubBoundsForZDir) && floatNthSubBoundsForZDir != 0)
            nthSubBoundsForZDir = (int)floatNthSubBoundsForZDir - 1;
        else if (floatNthSubBoundsForZDir == 0)
            nthSubBoundsForZDir = 0;
        else
            nthSubBoundsForZDir = (int)floatNthSubBoundsForZDir;


        int indexOfSubBounds = nthSubBoundsForXDir + nthSubBoundsForYDir * xDirCount + nthSubBoundsForZDir * xDirCount * yDirCount;
        //Debug.Log("Index of SubBound: " + indexOfSubBounds);

        return subBounds[indexOfSubBounds];
    }

    private void putTrianglesIntoBoundingBox(Mesh m)
    {
        int vertexIndex = 0;
        BoundHolder b = null;
        int[] triangle = new int[3];
        for (int index = 0; index < m.triangles.Length/*256965*/; index += 3)
        {
            vertexIndex = m.triangles[0 + index];
            Vector3 worldPoint = trans.TransformPoint(m.vertices[vertexIndex]);
            b = FindBoundingBox(/*m.vertices[vertexIndex]*/worldPoint);
            triangle[0] = m.triangles[0 + index];
            triangle[1] = m.triangles[1 + index];
            triangle[2] = m.triangles[2 + index];
            b.AddTriangle(triangle, m);
        }
    }

    private void drawNewMesh()
    {
        int index = 0;
        foreach (BoundHolder b in subBounds)
        {
            GameObject newGameObject = new GameObject();
            MeshFilter mf = newGameObject.AddComponent<MeshFilter>();
            mf.name = "NewMesh" + index;
            MeshRenderer mr = newGameObject.AddComponent<MeshRenderer>();

            mf.mesh = b.SetThenGetMesh();
            mr.material = new Material(Shader.Find("Custom/UnlitVertexColor"));
            index++;
        }
    }
}
