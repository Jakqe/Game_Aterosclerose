using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuizDragQuestionOption : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private int _id;

    private Vector3 _fallbackPosition;
    private QuizDragQuestionSpot _currentSpot;

    public int OptionId => this._id;
    public QuizDragQuestion Question { get; set; }

    private void Start()
    {
        this._fallbackPosition = this.transform.position;
    }

    public void ResetPosition()
    {
        this.transform.position = this._fallbackPosition;
        this.Question.OptionsOnSpots[this._currentSpot.SpotId] = null;
        this._currentSpot = null;
        this.Question.OnOptionChanged();
    }

    public void ChangeColor(Color color)
    {
        GetComponent<Image>().color = color;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 position = this.transform.position;
        position.x = Input.mousePosition.x;
        position.y = Input.mousePosition.y;
        this.transform.position = position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (this._currentSpot != null)
        {
            this._currentSpot.enabled = true;
            this.Question.OptionsOnSpots[this._currentSpot.SpotId] = null;
            this.Question.OnOptionChanged();
            this._currentSpot = null;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        QuizDragQuestionSpot spot = this.Question.GetSpotUnder(this.transform.position);
        this._currentSpot = spot;

        if (spot != null)
        {
            if (this.Question.OptionsOnSpots[this._currentSpot.SpotId] != null)
            {
                this.Question.OptionsOnSpots[this._currentSpot.SpotId].ResetPosition();
            }

            this.transform.position = this._currentSpot.transform.position;
            this.Question.OptionsOnSpots[this._currentSpot.SpotId] = this;
            this.Question.OnOptionChanged();
        }
        else
        {
            this.transform.position = this._fallbackPosition;
        }
    }
}
