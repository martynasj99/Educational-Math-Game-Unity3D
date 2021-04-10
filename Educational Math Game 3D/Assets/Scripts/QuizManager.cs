using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuizManager : MonoBehaviour
{
    public class Question
    {
        public int num1;
        public int num2;
        public int answer;

        public Question(int num1, int num2, int answer)
        {
            this.num1 = num1;
            this.num2 = num2;
            this.answer = answer;
        }
    }

    public class Result
    {
        public int correct;
        public int incorrect;

        public Result()
        {

        }
        public void CorrectAnswer()
        {
            correct++;
        }

        public void IncorrectAnswer()
        {
            incorrect++;
        }
    }

    public static int level;
    public static int questions;
    public static int currentQuestionNum;

    public static List<Result> results = new List<Result>();

    public static string symbol;
    public static Result currentResult;
    public static QuizType.Type type;

    public static Question currentQuestion;

    public static int answer;

    public GameObject eol;
    private bool isAnswered = false;

    public GameObject speechManager;

    private SpeechManager speech;
    private int maxNum;
    void Start()
    {
        speech = speechManager.GetComponent<SpeechManager>();
        answer = 0;
        level = 0;
        currentQuestionNum = 0;

        switch (Difficulty.difficulty)
        {
            case Difficulty.GameDifficulty.EASY:
                questions = 5;
                maxNum = 10;
                break;
            case Difficulty.GameDifficulty.MEDIUM:
                questions = 10;
                maxNum = 20;
                break;
            case Difficulty.GameDifficulty.HARD:
                questions = 20;
                maxNum = 50;
                break;
            default:
                Debug.LogError("No Difficulty Found!");
                break;
        }

        NewLevel(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && isAnswered)
        {
            SubmitAnswer(answer);
        }
        if (Input.GetKey(KeyCode.H))
        {
            speech.Help(level);
        }
    }

    void NextQuestion()
    {
        isAnswered = false;
        if(level == 4)
        {
            type = (QuizType.Type) Random.Range(0, 4);
        }
        currentQuestion = GenerateQuestion(type);
        currentQuestionNum++;
    }

    public void SubmitAnswer(int ans)
    {
        if (ans == currentQuestion.answer)
        {
            currentResult.CorrectAnswer();
            speech.Encourage();
        }
        else 
        {
            currentResult.IncorrectAnswer();
            speech.Motivate();
        }
        
        if (currentQuestionNum == questions)
        {
            NewLevel(true);
        }
        else
        {
            NextQuestion();
        }
        answer = 0;
    }

    public void ProvideNumber(int num)
    {
        if (!isAnswered) answer = num;
        else
        {
            answer = Mathf.Abs(answer * 10) + Mathf.Abs(num);
            answer = num < 0 ? answer * -1 : answer;

        }
        isAnswered = true;
    }

    private  Question GenerateQuestion(QuizType.Type type)
    {
        int num1 = Random.Range(1, maxNum);
        int num2 = Random.Range(1, maxNum);
        int answer = 0;

        switch (type)
        {
            case QuizType.Type.ADDITION:
                symbol = "+";
                answer = num1 + num2;
                break;
            case QuizType.Type.SUBTRACTION:
                symbol = "-";
                answer = num1 - num2;
                break;
            case QuizType.Type.MULTIPLICATION:
                symbol = "x";
                answer = num1 * num2;
                break;
            case QuizType.Type.DIVISION:
                symbol = "/";
                answer = num1*num2 / num2;
                break;
            default:
                break;
        }
        return new Question(type == QuizType.Type.DIVISION ? num1*num2 : num1, num2, answer);
    }

    public void NewLevel(bool isEnd)
    {
        eol.SetActive(isEnd);
        Cursor.lockState = isEnd ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = isEnd ? 0 : 1;

        if (!isEnd)
        {
            currentResult = new Result();
            currentQuestionNum = 0;
            level++;
            type = (QuizType.Type) (level-1);
            results.Add(currentResult);
            NextQuestion();
            speech.NewLevel(level);
        }
        else if(isEnd)
        {
            eol.transform.GetChild(0).GetComponent<Text>().text = "Well Done! You completed level " + level;
            eol.transform.GetChild(1).GetComponent<Text>().text = "Correct " + currentResult.correct;
            eol.transform.GetChild(2).GetComponent<Text>().text = "Incorrect " + currentResult.incorrect;
        }

    }

}
