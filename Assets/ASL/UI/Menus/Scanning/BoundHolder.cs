using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class BoundHolder {
    public Bounds subBound;
    //private List<Vector3> partialVertices;
    //private List<Vector3> partialNormals;
    //private List<Color> partialColor;
    //private List<Vector2> partialUVs;
    //private List<int> newTriangles;
    //private VertexLookUp map;
    private bool isActive;

    public Mesh[] meshInBBox;
    public Dictionary<int, MeshInBBox> meshes = new Dictionary<int, MeshInBBox>();
    public int id;

    public BoundHolder()
    {
        isActive = false;
    }

    public BoundHolder(Vector3 center, Vector3 size)
    {
        subBound = new Bounds(center, size);
        isActive = false;
    }

    public bool GetStatus()
    {
        return isActive;
    }

    public void SetStatus(bool status)
    {
        isActive = status;
    }

    public bool CheckIntersects(Vector3 point)
    {
        if (subBound.Contains(point))
        {
            return true;
        }
        return false;
    }

    public byte[] Serialize()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                writeBounds(bw, subBound);
                writeMesh(bw, meshes);
            }

            return ms.ToArray();
        }
    }

    public byte[] SerializeBounds()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                writeBounds(bw, subBound);
            }

            return ms.ToArray();
        }
    }

    public BoundHolder Desirialize(byte[] data)
    {
        BoundHolder result = new BoundHolder();
        using (MemoryStream ms = new MemoryStream(data))
        {
            using (BinaryReader br = new BinaryReader(ms))
            {
                result.subBound = readBounds(br);
                result.meshInBBox = readMesh(br);
            }
            return result;
        }
    }

    public BoundHolder DesirializeBounds(byte[] data)
    {
        BoundHolder result = new BoundHolder();
        using (MemoryStream ms = new MemoryStream(data))
        {
            using (BinaryReader br = new BinaryReader(ms))
            {
                result.subBound = readBounds(br);
            }
            return result;
        }
    }

    public BoundHolder DesirializeMeshes(byte[] data)
    {
        BoundHolder result = new BoundHolder();
        using (MemoryStream ms = new MemoryStream(data))
        {
            using (BinaryReader br = new BinaryReader(ms))
            {
                result.meshInBBox = readMesh(br);
            }
            return result;
        }
    }

    private void writeBounds(BinaryWriter bw, Bounds b)
    {
        bw.Write(b.center.x);
        bw.Write(b.center.y);
        bw.Write(b.center.z);
        bw.Write(b.size.x);
        bw.Write(b.size.y);
        bw.Write(b.size.z);
    }

    private Bounds readBounds(BinaryReader br)
    {
        Bounds result = new Bounds();

        Vector3 center = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        Vector3 size = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

        result.center = center;
        result.size = size;

        return result;
    }

    private void writeMesh(BinaryWriter bw, Dictionary<int, MeshInBBox> m)
    {
        bw.Write(m.Count); // to write how many meshee are there
        foreach (KeyValuePair<int, MeshInBBox> entry in m) {
            // make copy of mesh arrays
            Vector3[] copyVertices = entry.Value.partialVertices.ToArray();
            Vector3[] copyNormals = entry.Value.partialNormals.ToArray();
            Color[] copyColors = entry.Value.partialColor.ToArray();
            int[] copyTriangles = entry.Value.newTriangles.ToArray();

            // write the length of arrays first so that I can generate proper array length when I read back
            bw.Write(copyVertices.Length);
            bw.Write(copyNormals.Length);
            bw.Write(copyColors.Length);
            bw.Write(copyTriangles.Length);

            int index = 0;
            foreach (Vector3 vertex in copyVertices)
            {
                bw.Write(copyVertices[index].x);
                bw.Write(copyVertices[index].y);
                bw.Write(copyVertices[index].z);
                bw.Write(copyNormals[index].x);
                bw.Write(copyNormals[index].y);
                bw.Write(copyNormals[index].z);
                bw.Write(copyColors[index].r);
                bw.Write(copyColors[index].g);
                bw.Write(copyColors[index].b);
                bw.Write(copyColors[index].a);
                index++;
            }

            index = 0;
            foreach (int triangleIndex in copyTriangles)
            {
                bw.Write(copyTriangles[index]);
                index++;
            }
        }
    }

    private Mesh[] readMesh(BinaryReader br)
    {
        int count = br.ReadInt32();
        Mesh[] results = new Mesh[count];
        // instantiate
        for (int i = 0; i < results.Length; i++)
        {
            results[i] = new Mesh();
        }

        for (int i = 0; i < results.Length; i++)
        {
            Vector3[] vertices = new Vector3[br.ReadInt32()];
            Vector3[] normals = new Vector3[br.ReadInt32()];
            Color[] colors = new Color[br.ReadInt32()];
            int[] triangles = new int[br.ReadInt32()];

            for (int index = 0; index < vertices.Length; index++)
            {
                vertices[index] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                normals[index] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                colors[index] = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            }
            for (int index = 0; index < triangles.Length; index++)
            {
                triangles[index] = br.ReadInt32();
            }

            results[i].vertices = vertices;
            results[i].normals = normals;
            results[i].colors = colors;
            results[i].triangles = triangles;
        }

        return results;
    }
}
