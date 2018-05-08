using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedMeshHolder {
    public Mesh mesh;

    public LoadedMeshHolder(BoundHolder bh)
    {
        mesh = bh.mesh;
    }
}
