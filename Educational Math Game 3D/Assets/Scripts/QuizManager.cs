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

    private const int FINAL_LEVEL = 6;

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
    public GameObject eog;
    private bool isAnswered = false;

    public GameObject speechManager;
    public GameObject AICoLearner;

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

        StartLevel();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && isAnswered)
        {
            SubmitAnswer();
        }
        if (Input.GetKey(KeyCode.H))
        {
            speech.Help(level);
        }

    }

    void NextQuestion()
    {
        isAnswered = false;
        if(level == 5)
        {
            type = (QuizType.Type) Random.Range(0, 4);
        }
        currentQuestion = GenerateQuestion(type);
        currentQuestionNum++;
        if(currentQuestionNum == questions/2)
        {
           
            StartCoroutine(AICoLearner.GetComponent<AICoLearner>().ExecuteAction(Mathf.Abs(FirstDigit(currentQuestion.answer))));
            speech.Action();
        }
    }

    public void SubmitAnswer()
    {
        if (answer == currentQuestion.answer)
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
            EndLevel();
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

    public void EndLevel()
    {
        eol.SetActive(true);
        results.Add(currentResult);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        eol.transform.GetChild(0).GetComponent<Text>().text = "Well Done! You completed level " + level;
        eol.transform.GetChild(1).GetComponent<Text>().text = "Correct " + currentResult.correct;
        eol.transform.GetChild(2).GetComponent<Text>().text = "Incorrect " + currentResult.incorrect;

    }

    public void StartLevel()
    {
        eol.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        level++;
        currentResult = new Result();
        currentQuestionNum = 0;
        type = (QuizType.Type)(level - 1);
        
        NextQuestion();
        speech.NewLevel(level);
        if (level == FINAL_LEVEL)
        {
            EndGame();
        }
        GameObject.Find("GameManager").GetComponent<WindSystem>().GenerateWind();
    }

    public void EndGame()
    {
        eog.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
       
        for (int i = 0; i < FINAL_LEVEL-1; i++)
        {
            if(i == FINAL_LEVEL - 2)
            {
                eog.transform.GetChild(i + 1).GetComponent<Text>().text = "Quiz: " + results[i].correct + " out of " + questions;
            }
            else
            {
                eog.transform.GetChild(i + 1).GetComponent<Text>().text = "Level " + (i + 1) + ": " + results[i].correct + " out of " + questions;
            }
        }

    }

    private int FirstDigit(int n)
    {
        while (n >= 10)
            n /= 10;

        return n;
    }

}
