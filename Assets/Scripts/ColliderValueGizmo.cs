using UnityEngine;

public class ColliderValueGizmo : MonoBehaviour
{
	public Mesh mesh;

	public Collider collider;

	public Color Color = Color.white;

	private void OnDrawGizmosSelected()
	{
		if (collider == null || mesh == null)
		{
			return;
		}
		Gizmos.color = Color;
		BoxCollider boxCollider = collider as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.DrawMesh(mesh, collider.bounds.center, collider.transform.rotation, boxCollider.size);
			return;
		}
		MeshCollider x = collider as MeshCollider;
		if (x != null)
		{
			Gizmos.DrawMesh(mesh, collider.bounds.center, collider.transform.rotation);
		}
	}
}
