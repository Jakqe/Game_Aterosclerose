using UnityEngine;

public class RandomSizeOnSpawn : MonoBehaviour
{
    [Min(0)]
    [SerializeField] private float _min;
    [Min(0)]
    [SerializeField] private float _max;

    private void Start()
    {
        this.transform.localScale = Vector3.one * (Random.value * (this._max - this._min) + this._min);
    }

    private void OnValidate()
    {
        if (this._min > this._max)
        {
            this._min = this._max;
        }
    }
}
