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
        public void correctAnswer()
        {
            correct++;
        }

        public void incorrectAnswer()
        {
            incorrect++;
        }
    }

    public static int level = 0;
    public static int questions = 10;
    public static int currentQuestionNum = 0;

    public static List<Result> results = new List<Result>();

    public static Result currentResult;
    public static string symbol;
    public static QuizType.Type type;

    public static Question currentQuestion;

    public GameObject eol;

    void Start()
    {
        type = (QuizType.Type) level;
        currentResult = new Result();
        NextQuestion();
    }

    void NextQuestion()
    {
        currentQuestion = GenerateQuestion(type);
        currentQuestionNum++;
    }

    public void ProvideAnswer(int answer)
    {
        if (answer == currentQuestion.answer) currentResult.correctAnswer();
        else currentResult.incorrectAnswer();
        
        if (currentQuestionNum == questions)
        {
            NewLevel(true);

        }
        else
        {
            NextQuestion();
        }
    }

    private  Question GenerateQuestion(QuizType.Type type)
    {
        int num1 = Random.Range(1, 10);
        int num2 = Random.Range(1, 10);
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
                answer = num1 / num2;
                break;
            default:
                break;
        }
        return new Question(num1, num2, answer);
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
            type = (QuizType.Type)level;
            results.Add(currentResult);
            NextQuestion();
        }
        else if(isEnd)
        {
            eol.transform.GetChild(0).GetComponent<Text>().text = "Well Done! You completed level " + level;
            eol.transform.GetChild(1).GetComponent<Text>().text = "Correct " + currentResult.correct;
            eol.transform.GetChild(2).GetComponent<Text>().text = "Incorrect " + currentResult.incorrect;
        }

    }

}
