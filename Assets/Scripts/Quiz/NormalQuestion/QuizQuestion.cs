using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class QuizQuestion : MonoBehaviour
{
    private enum QuizOptionState
    {
        DISABLED, NONE, SELECTED, CORRECT, INCORRECT
    }

    private class QuizOption
    {
        public Button button;
        public Image image;
        public QuizOptionState state;

        public QuizOption(Button button, Image image)
        {
            this.button = button;
            this.image = image;
            this.state = QuizOptionState.NONE;
        }
    }

    [Header("Setup")]
    [SerializeField] private Color _right;
    [SerializeField] private Color _wrong;
    [SerializeField] private Color _selected;
    [SerializeField] private int[] _correctAnswersIds;
    [Tooltip("Limita a seleção de opções para o número de alternativas")]
    [SerializeField] private bool _limitSelection = true;
    [SerializeField] private float _finishQuizDelay;

    [Header("References")]
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button[] _buttons;

    [Header("Events")]
    [SerializeField] private UnityEvent _onFinish;
    [SerializeField] private UnityEvent<bool, int> _onAnswerUpdate; // Chamado a cada resposta dada (terminou nessa tentativa, nmr de opções erradas nessa tentativa)
    [SerializeField] private UnityEvent<int, int> _onFinishWithStats; // Chamado ao final do quiz (nmr tentativas, nmr de erros)

    private int _attempts;
    private int _misses;
    private Color _startColor;
    private bool _isOver;
    private QuizOption[] _options;

    private void Start()
    {
        this._attempts = 1;
        this._confirmButton.interactable = false;

        this._options = new QuizOption[this._buttons.Length];
        for (int i = 0; i < this._buttons.Length; i++)
        {
            this._options[i] = new QuizOption(this._buttons[i], this._buttons[i].GetComponent<Image>());
            if (!this._buttons[i].gameObject.activeSelf)
            {
                this._options[i].state = QuizOptionState.DISABLED;
            }
        }

        this._startColor = this._options[0].image.color;
    }

    public void OptionClicked(int id)
    {
        if (this._isOver) return;

        bool isThisButtonSelected = this._options[id].state == QuizOptionState.SELECTED;

        if (!isThisButtonSelected && this._correctAnswersIds.Length == 1)
        {   // Caso tenha apenas uma resposta correta desseleciona a seleção anterior
            this._options.ForEach(option =>
            {
                if (option.state == QuizOptionState.SELECTED)
                {
                    SwitchOptionState(option, QuizOptionState.NONE);
                }
            });
        }

        // Troca o status de selecionado
        int numSelectedButtons = this._options.Count(option => option.state == QuizOptionState.SELECTED);
        int numCorrectButtons = this._options.Count(option => option.state == QuizOptionState.CORRECT);

        if (!this._limitSelection || isThisButtonSelected || this._correctAnswersIds.Length > numSelectedButtons + numCorrectButtons)
        {
            SwitchOptionState(this._options[id], isThisButtonSelected ? QuizOptionState.NONE : QuizOptionState.SELECTED);
        }

        this._confirmButton.interactable = this._options.Count(option => option.state == QuizOptionState.SELECTED) >= (this._limitSelection ? this._correctAnswersIds.Length - numCorrectButtons : 1);
    }

    public void ConfirmSelection()
    {
        if (this._isOver) return;

        int missesThisTime = 0;
        this._options.For((i, option) =>
        {
            if (option.state == QuizOptionState.SELECTED)
            {
                if (this._correctAnswersIds.Contains(i))
                {
                    SwitchOptionState(option, QuizOptionState.CORRECT);
                }
                else
                {
                    this._misses++;
                    missesThisTime++;
                    SwitchOptionState(option, QuizOptionState.INCORRECT);
                }
            }
        });

        int numCorrect = this._options.Count(option => option.state == QuizOptionState.CORRECT);
        int numUnselected = this._options.Count(option => option.state == QuizOptionState.NONE);

        if (numCorrect == this._correctAnswersIds.Length || numUnselected == 1)
        {   // Finalizou o quiz
            if (numUnselected == 1)
            {   // Utilizou uma tentativa automática (já que só tem uma opção sobrando)
                this._attempts++;
            }

            this._isOver = true;
            this._options.For((i, option) =>
            {   // Revela o resto das opções
                if (option.state == QuizOptionState.NONE)
                {
                    SwitchOptionState(option, this._correctAnswersIds.Contains(i) ? QuizOptionState.CORRECT : QuizOptionState.INCORRECT);
                }
            });
            this._onAnswerUpdate.Invoke(true, missesThisTime);
            StartCoroutine(OnFinishQuiz());
        }
        else
        {
            this._onAnswerUpdate.Invoke(false, missesThisTime);
            this._attempts++;
        }

        this._confirmButton.interactable = false;
    }

    private void SwitchOptionState(QuizOption option, QuizOptionState newState)
    {
        option.image.color = newState == QuizOptionState.NONE ? this._startColor :
                            (newState == QuizOptionState.SELECTED ? this._selected :
                            (newState == QuizOptionState.CORRECT ? this._right : this._wrong));
        option.state = newState;
        option.button.interactable = newState != QuizOptionState.CORRECT && newState != QuizOptionState.INCORRECT;
    }

    private IEnumerator OnFinishQuiz()
    {
        yield return new WaitForSeconds(this._finishQuizDelay);

        this.gameObject.SetActive(false);
        this._onFinish?.Invoke();
        this._onFinishWithStats?.Invoke(this._attempts, this._misses);
    }
}
