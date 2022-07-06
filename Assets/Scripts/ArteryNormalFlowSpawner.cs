using UnityEngine;
using PathCreation;

public class ArteryNormalFlowSpawner : MonoBehaviour
{
    [Tooltip("Prefabs que devem aparecer no fluxo normal da artéria")]
    [SerializeField] private GameObject[] _prefabs;
    [Tooltip("Chance de cada prefab ser escolhido")]
    [SerializeField] private float[] _prefabWeights;
    [Range(0.05f, 2f)]
    [SerializeField] private float _spacing;
    [SerializeField] private float _variability;
    [SerializeField] private float _speed;
    [SerializeField] private PathCreator[] _curves;

    private float[] _weightsSumCache;

    private void Start()
    {
        CacheWeightSum();
        SpawnPrefabs();
    }

    private void SpawnPrefabs()
    {
        foreach (PathCreator curve in this._curves)
        {
            float _startOffset = Random.value * this._spacing;
            int _numPrefabs = (int)(curve.path.length / this._spacing);

            for (int i = 0; i < _numPrefabs; i++)
            {
                CurveFollower follower = Instantiate(ChoosePrefabBasedOnWeights(), Vector3.zero, Quaternion.identity).GetComponent<CurveFollower>();
                follower.Curve = curve;
                follower.Speed = this._speed;
                follower.DistanceOnCurve = (_startOffset + i * this._spacing) + (Random.value - .5f) * this._variability;
                follower.IsFollowingCurve = true;
            }
        }
    }

    private void CacheWeightSum()
    {
        this._weightsSumCache = new float[this._prefabWeights.Length];
        this._weightsSumCache[0] = this._prefabWeights[0];
        for (int i = 1; i < this._weightsSumCache.Length; i++)
        {
            this._weightsSumCache[i] = this._prefabWeights[i] + this._weightsSumCache[i - 1];
        }
    }

    private GameObject ChoosePrefabBasedOnWeights()
    {
        float totalWeight = this._weightsSumCache[this._weightsSumCache.Length - 1];
        float randomValue = Random.value * totalWeight;

        for (int i = 0; i < this._weightsSumCache.Length; i++)
            if (randomValue <= this._weightsSumCache[i] && this._weightsSumCache[i] != 0.0f)
                return this._prefabs[i];

        return this._prefabs[Random.Range(0, this._prefabs.Length)]; // não deve cair aqui
    }
}
