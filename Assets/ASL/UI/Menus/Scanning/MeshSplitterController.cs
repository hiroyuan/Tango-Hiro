using ASL.UI.Menus.Scanning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshSplitterController : MonoBehaviour {
    public BoundingBoxSystem areaBound;
    public BoundingBoxSystem meshBound;
    public BoundingBoxSystem loadedBound;
    public List<GameObject> specificMeshes;
    public GameObject indicator;
    public Transform trans;

    public int x_area = 1;
    public int y_area = 1;
    public int z_area = 1;
    public int x_mesh = 2;
    public int y_mesh = 1;
    public int z_mesh = 1;

    Bounds loadedBBox;

    //private RoomSave roomSave = new RoomSave();
    private RoomSave rs;

    // Use this for initialization
    void Start ()
    {
        rs = (RoomSave)EditorWindow.GetWindow(typeof(RoomSave));
        specificMeshes = new List<GameObject>();
        indicator = GameObject.Find("IndicatorObject");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (rs.GetList() != null)
        {
            areaBound = new BoundingBoxSystem(rs.GetList(), x_area, y_area, z_area);
            areaBound.SplitBounds();
        }

        if (loadedBound != null)
        {
            loadedBound = new BoundingBoxSystem(loadedBBox, x_area, y_area, z_area);
            loadedBound.SplitBounds();
        }

    }

    void OnDrawGizmos()
    {
        //if (areaBound != null)
        //{
        //    for (int i = 0; i < areaBound.GetSubBounds().Length; i++)
        //    {
        //        Gizmos.DrawWireCube(areaBound.GetSubBounds()[i].subBound.center, areaBound.GetSubBounds()[i].subBound.size);
        //    }
        //}

        if (meshBound != null)
        {
            for (int i = 0; i < meshBound.GetSubBounds().Length; i++)
            {
                Gizmos.DrawWireCube(meshBound.GetSubBounds()[i].subBound.center, meshBound.GetSubBounds()[i].subBound.size);
            }
        }

        if (loadedBound != null)
        {
            Gizmos.DrawWireCube(loadedBound.GetBound().center, loadedBound.GetBound().size);
            for (int i = 0; i < loadedBound.GetSubBounds().Length; i++)
            {
                Gizmos.DrawWireCube(loadedBound.GetSubBounds()[i].subBound.center, loadedBound.GetSubBounds()[i].subBound.size);
            }
        }
    }

    public void Test()
    {
        //foreach (GameObject room in rs.GetList())
        //{
        //    List<GameObject> r = new List<GameObject>();
        //    r.Add(room);
        //    meshBound = new BoundingBoxSystem(r, x_mesh, y_mesh, z_mesh, room.transform, room.GetComponent<Mesh>());
        //    Debug.Log(room);
        //    //meshBound.SplitMesh();
        //}

        GameObject roomMesh = GameObject.Find("3_12_4_2017_11_23_11_AM");
        specificMeshes.Add(roomMesh);
        meshBound = new BoundingBoxSystem(specificMeshes, x_mesh, y_mesh, z_mesh, roomMesh.transform, roomMesh.GetComponent<MeshFilter>().mesh);
        meshBound.SplitBounds();
        meshBound.SplitMesh();
    }

    public void Load()
    {
        SerializableBounds b = rs.LoadBoundingBox();
        Vector3 center = convertSerializedVector3ToVector3(b.center);
        Vector3 size = convertSerializedVector3ToVector3(b.size);
        loadedBBox = new Bounds(center, size);
        loadedBound = new BoundingBoxSystem(loadedBBox, x_area, y_area, z_area);
    }

    private Bounds convertSerializedBoundsToBounds(SerializableBounds bounds)
    {
        Bounds b = new Bounds();
        b.center = convertSerializedVector3ToVector3(bounds.center);
        b.size = convertSerializedVector3ToVector3(bounds.size);
        return b;
    }

    private Vector3 convertSerializedVector3ToVector3(SerializableVector3 v)
    {
        Vector3 vector3 = new Vector3();
        vector3.x = v.x;
        vector3.y = v.y;
        vector3.z = v.z;
        return vector3;
    }
}
