using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class QuizDragQuestion : MonoBehaviour
{
    [SerializeField] private QuizDragQuestionOption[] _options;
    [SerializeField] private QuizDragQuestionSpot[] _spots;
    [Tooltip("A distância máxima para achar um spot")]
    [Min(0f)]
    [SerializeField] private float _spotFindDistance;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Color _correctColor;
    [SerializeField] private float _finishQuizDelay;
    [SerializeField] private UnityEvent _onFinishQuiz;

    public QuizDragQuestionOption[] OptionsOnSpots { get; private set; }

    private void Start()
    {
        foreach (QuizDragQuestionOption option in this._options)
        {
            option.Question = this;
        }

        this.OptionsOnSpots = new QuizDragQuestionOption[this._options.Length];
        this._continueButton.interactable = false;
    }

    public void OnConfirmOptions()
    {
        bool allCorrect = true;
        for (int i = 0; i < this.OptionsOnSpots.Length; i++)
        {
            QuizDragQuestionOption option = this.OptionsOnSpots[i];
            if (option.OptionId == i)
            {
                option.ChangeColor(this._correctColor);
                option.enabled = false; // Desabilita drag
                this._spots[i].enabled = false; // Desabilita o spot
            }
            else
            {
                option.ResetPosition();
                allCorrect = false;
            }
        }

        if (allCorrect)
        {
            this._continueButton.interactable = false;
            StartCoroutine(OnFinishQuiz());
        }
    }

    public void OnOptionChanged()
    {
        bool placedAllOptions = true;
        foreach (QuizDragQuestionOption option in this.OptionsOnSpots)
        {
            if (option == null)
            {
                placedAllOptions = false;
                break;
            }
        }

        this._continueButton.interactable = placedAllOptions;
    }

    public QuizDragQuestionSpot GetSpotUnder(Vector2 position)
    {
        foreach (QuizDragQuestionSpot spot in this._spots)
        {
            if (spot.enabled && (((Vector2)spot.transform.position) - position).magnitude <= this._spotFindDistance)
            {
                return spot;
            }
        }

        return null;
    }

    private IEnumerator OnFinishQuiz()
    {
        yield return new WaitForSeconds(this._finishQuizDelay);

        this._onFinishQuiz?.Invoke();
    }
}
