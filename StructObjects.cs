using UnityEngine;

/// <summary>
/// Object Cell
/// </summary>
public struct ObjCell
{
    public PreInstance cell;
    public Vector2Int xy;
    /// <summary>
    /// if door: true,
    /// if wall: false,
    /// if nothing: null
    /// </summary>
    public bool? e, n, s, w;
    public bool r, br;

    public ObjCell(PreInstance cell, Vector2Int xy, bool? e, bool? n, bool? s, bool? w, bool r, bool br)
    {
        this.cell = cell; this.xy = xy;
        this.e = e; this.n = n;
        this.s = s; this.w = w;
        this.r = r; this.br = br;
    }

    public void InstantiateRoom()
    {
        Object.Instantiate(cell.prefab,(Vector2)cell.xy,cell.rotation,cell.parent);
    }
}

public struct CellChild
{
    public PreInstance cellChild;
    public Item item;
	
    public CellChild(PreInstance cellChild,Item item)
    {
        this.cellChild = cellChild;
        this.item = item;
    }

    public CellChild(PreInstance cellChild)
    {
        this.cellChild = cellChild;
        this.item = null;
    }

    public Object InstantiateCellChild()
    {
        return Object.Instantiate(cellChild.prefab, (Vector2)cellChild.xy, cellChild.rotation, cellChild.parent);
    }

    public CellChild DesInstantiateCellChild(GameObject child, GameObject prefab)
    {
        CellChild output = new CellChild(new PreInstance(prefab, child.transform.parent, child.transform.position));
        Object.Destroy(child);
        return output;
    }

    public CellChild DesInstantiateCellChild(GameObject child, GameObject prefab,Item item)
    {
        CellChild output = new CellChild(new PreInstance(prefab, child.transform.parent, child.transform.position),item);
        Object.Destroy(child);
        return output;
    }
}

public struct PreInstance
{
    public GameObject prefab;
    public Transform parent;
    public Vector2 xy;
    public Quaternion rotation;

    public PreInstance(GameObject prefab, Transform parent, Vector2 xy)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.xy = xy;
        this.rotation = Quaternion.identity;
    }
}

