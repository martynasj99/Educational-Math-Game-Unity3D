using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechManager : MonoBehaviour
{
    public Text speechText;

    public void Cangradualate()
    {
        string[] speech = { "That is correct!", "Wow!" };
        ChangeSpeech(speech[Random.Range(0, speech.Length)]);
    }
    public void Encourage()
    {
        string[] speech = { "You are doing great!", "Keep Going!" };
        ChangeSpeech(speech[Random.Range(0, speech.Length)]);
    }

    public void Motivate()
    {
        string[] speech = { "Try again!", "Don't give up!" };
        ChangeSpeech(speech[Random.Range(0, speech.Length)]);
    }
    public void NewLevel(int level)
    {
        switch (level)
        {
            case 1:
                ChangeSpeech("First Level is Addition!\n Good luck!");
                break;
            case 2:
                ChangeSpeech("Well done on completing the first level!\n Subtraction now");
                break;
            case 3:
                ChangeSpeech("Getting harder!\n Time for multiplication!");
                break;
            case 4:
                ChangeSpeech("Almost there!\n Final level division!");
                break;
            case 5:
                ChangeSpeech("Final level!\n Time to test your knowledge with a quiz!");
                break;
        }
    }

    public void Action()
    {
        string[] speech = { "I will help you be giving you the first digit!" };
        ChangeSpeech(speech[Random.Range(0, speech.Length)]);
    }
    public void Help(int level)
    {
        switch (level)
        {
            case 1:
                ChangeSpeech("2 + 6 = 8 -> Count Six times from Two!");
                break;
            case 2:
                ChangeSpeech("7 - 3 = 4");
                break;
            case 3:
                ChangeSpeech("4 x 3 = 12");
                break;
            case 4:
                ChangeSpeech("8 / 2 = 4");
                break;
            case 5:
                ChangeSpeech("I can't help you anymore!");
                break;
        }
    }

    private void ChangeSpeech(string speech)
    {
        speechText.text = speech;
    }


}
