using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class BlendShapeColliderUpdater : MonoBehaviour
{
    [SerializeField] private bool _autoUpdate = true;

    private SkinnedMeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;
    private float _lastBlendShapeWeight;

    private void Start()
    {
        this._meshRenderer = GetComponent<SkinnedMeshRenderer>();
        this._meshCollider = GetComponent<MeshCollider>();
        this._lastBlendShapeWeight = -1f; // Força uma atualização inicial
    }

    private void LateUpdate()
    {
        float currentWeight = this._meshRenderer.GetBlendShapeWeight(0);
        if (this._autoUpdate && currentWeight != this._lastBlendShapeWeight)
        {
            UpdateMeshCollider();
            this._lastBlendShapeWeight = currentWeight;
        }
    }

    public void UpdateMeshCollider()
    {
        Mesh bakedMesh = new Mesh();
        this._meshRenderer.BakeMesh(bakedMesh);
        this._meshCollider.sharedMesh = bakedMesh;
    }
}
