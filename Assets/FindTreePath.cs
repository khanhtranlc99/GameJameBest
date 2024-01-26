using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTreePath : MonoBehaviour
{
    public Transform[] tree;
    public Mesh meshFilter;
    [ContextMenu("tryFind")]
    void TryFind()
    {
        foreach (var t in tree)
        {
            t.GetChild(0).GetComponent<MeshFilter>().mesh = meshFilter;
        }
    }
}
