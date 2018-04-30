using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableBounds {
    public SerializableVector3 center;
    public SerializableVector3 extents;
    public SerializableVector3 max;
    public SerializableVector3 min;
    public SerializableVector3 size;
    
    public SerializableBounds()
    {
        center = new SerializableVector3(0, 0, 0);
        extents = new SerializableVector3(0, 0, 0);
        max = new SerializableVector3(0, 0, 0);
        min = new SerializableVector3(0, 0, 0);
        size = new SerializableVector3(0, 0, 0);
    }

    public SerializableBounds(Bounds bounds)
    {
        center = new SerializableVector3(bounds.center);
        extents = new SerializableVector3(bounds.extents);
        max = new SerializableVector3(bounds.max);
        min = new SerializableVector3(bounds.min);
        size = new SerializableVector3(bounds.size);
    }

    public Bounds GetBounds()
    {

        return new Bounds(Serializable2Original(center), Serializable2Original(size));
    }

    public Vector3 Serializable2Original(SerializableVector3 v)
    {
        Vector3 original = new Vector3();
        original.x = v.x;
        original.y = v.y;
        original.z = v.z;
        return original;
    }
}
