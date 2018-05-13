using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedMeshHolder {
    public Mesh[] meshes;

    public LoadedMeshHolder(BoundHolder bh)
    {
        meshes = bh.meshInBBox;
    }
}
