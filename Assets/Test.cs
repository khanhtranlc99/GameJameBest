using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public List<MeshCollider> allMissingMeshCollider = new List<MeshCollider>();
    public List<Material> allMaterial = new List<Material>();
    public Material errorShader,resultShader;
[ContextMenu("FindAll")]
    void FindAll()
    {
        allMissingMeshCollider = new List<MeshCollider>();
        var allMeshCollider = FindObjectsOfType<MeshCollider>(true);
        foreach (var mesh in allMeshCollider)
        {
            if(mesh.sharedMesh != null)
                 continue;
            allMissingMeshCollider.Add(mesh);
        }
    }
    [ContextMenu("FindAllNullMaterial")]
    void FindAllNullMaterial()
    {
        var cachedShader = errorShader.shader;
        foreach (var material in allMaterial)
        {
            if (material.shader == cachedShader)
            {
                material.shader = resultShader.shader;
            }
        }
    }
}
