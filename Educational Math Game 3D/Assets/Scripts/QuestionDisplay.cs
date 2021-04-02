using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionDisplay : MonoBehaviour
{
    void Update()
    {
        GetComponent<Text>().text = QuizManager.currentQuestion.num1 + " " + QuizManager.symbol + " " + QuizManager.currentQuestion.num2 + " = \n" +
            "Level: " + QuizManager.level + " Question " + QuizManager.currentQuestionNum + "/" + QuizManager.questions; 
    }
}
