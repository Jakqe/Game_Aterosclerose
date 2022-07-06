using UnityEngine;
using UnityEngine.Events;

public class OnAnimEnd : MonoBehaviour
{
    [SerializeField] private UnityEvent _onAnimEnd;

    // Este evento deve ser chamado por um animation event
    public void AnimationEndEvent()
    {
        this._onAnimEnd?.Invoke();
    }
}
