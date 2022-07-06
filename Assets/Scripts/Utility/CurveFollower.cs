using UnityEngine;
using PathCreation;

// As dependencias devem ser injetadas neste componente
public class CurveFollower : MonoBehaviour
{
    public PathCreator Curve { get; set; }
    public float Speed { get; set; }
    public float DistanceOnCurve { get; set; }
    public bool IsFollowingCurve { get; set; }

    private void Update()
    {
        this.DistanceOnCurve += Time.deltaTime * this.Speed;

        if (this.Curve != null && this.IsFollowingCurve)
            this.transform.localPosition = this.Curve.path.GetPointAtDistance(this.DistanceOnCurve, EndOfPathInstruction.Loop);
    }
}
