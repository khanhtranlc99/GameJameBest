using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class FixError : MonoBehaviour
{
    [ContextMenu("FixNe")]
    void FixNe()
    {
        var allMeshFilter = GetComponentsInChildren<MeshFilter>();
        foreach (var meshChild in allMeshFilter)
        {
            Mesh mesh = meshChild.GetComponent<MeshFilter>().mesh;
            mesh.triangles = mesh.triangles.Reverse().ToArray();
        }

    }
}
