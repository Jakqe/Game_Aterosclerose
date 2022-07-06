using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class ArteryController : MonoBehaviour
{
    private PathCreator[] _curves;
    private SkinnedMeshRenderer _meshRenderer;
    private float[][] _curvesStartYPos;
    private float _visibleDamage;
    public CurveFollower transformationfollower;

    public float ArteryDamage { get; set; }

    private void Start()
    {
        this.ArteryDamage = 0f;
        this._visibleDamage = 0f;
        this._meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        this._curves = GetComponentsInChildren<PathCreator>();
        this._curvesStartYPos = new float[this._curves.Length][];
        this._curves.For((i, curve) =>
        {
            this._curvesStartYPos[i] = new float[] { curve.bezierPath[2].y, curve.bezierPath[3].y, curve.bezierPath[4].y };
        });
    }

    private void LateUpdate()
    {
        if (this.ArteryDamage != this._visibleDamage)
        {
            UpdateArteryClosing();
        }
    }

    public void OnQuestionUpdate(bool finished, int misses)
    {
        const float attemptFactor = 0.01f;
        const float missFactor = 0.02f;
        this.ArteryDamage += misses * missFactor;
        if (!finished)
        {
            this.ArteryDamage += attemptFactor;
        }
        this.ArteryDamage = Mathf.Clamp(this.ArteryDamage, 0f, .9f);
    }

    private void UpdateArteryClosing()
    {
        this._meshRenderer.SetBlendShapeWeight(0, this._visibleDamage * 100f);

        this._curves.For((i, curve) =>
        {
            for (int p = 0; p < 3; p++)
            {
                Vector3 middlePoint = curve.bezierPath[2 + p];
                middlePoint.y = Mathf.Lerp(this._curvesStartYPos[i][p], -.22f, this._visibleDamage);

                curve.bezierPath.SetPoint(2 + p, middlePoint);
            }
            curve.EditorData.VertexPathSettingsChanged();
        });

        this._visibleDamage = Mathf.Lerp(this._visibleDamage, this.ArteryDamage, Time.deltaTime);
        if (Mathf.Abs(this._visibleDamage - this.ArteryDamage) < .01f)
        {
            this._visibleDamage = this.ArteryDamage;
        }
    }
    public void SpawnTrasnformation()
    {
        CurveFollower follower = Instantiate(transformationfollower, Vector3.zero,Quaternion.identity).GetComponent<CurveFollower>();

        follower.Curve = _curves[2];
        follower.Speed = 0.5f;
        follower.IsFollowingCurve = true;
    }
}
