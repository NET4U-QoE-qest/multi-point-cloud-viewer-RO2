using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SJM_GameManager : MonoBehaviour
{
    // cloud slots
    //  public Transform[] slots;


    // cloud names in stands (0:T1 , 1: T2, 2: T3)
    public string[] cloudNames;

    // random source
    public SJM_CloudRandomizer[] randomizer;

    // Data
    public string playerID;
    public int[] rateValues;

    public int remainTasks = 0;
    public int dataSent = 0;
    public int maxDataToCollect = 27;
    public TMP_Text remainTasksUI;
    // rate sent
    public bool rateStatus = false;

    // UI referance
    public TMP_Text playerIDTextUI;

    public bool isPause = true;

    public UnityEvent onPauseOn;
    public UnityEvent onPauseOff;

    public UnityEvent onSurveyStart;
    public UnityEvent onSurveyEnd;
    public UnityEvent onCleanData;


    private void Update()
    {
        if (!isPause)
        {
            if (remainTasks == 0)
            {
                if (dataSent == maxDataToCollect) SurveyEnd();
            }
            //else SurveyEnd();
        }

    }
    // whipe data
    [ContextMenu("Clear Saved Data")]
    public void ClearSaved()
    {
        onCleanData.Invoke();
        Debug.Log("Clean");
    }

    public void VoteCounting()
    {
        dataSent = dataSent + 1;
    }

    public void UpdatePlayerID(string value)
    {
        playerID = value;
        playerIDTextUI.text = "PLAYER " + value;
    }

    public void UpdateStoneName()
    {

        cloudNames[0] = randomizer[0].currentStone;
        cloudNames[1] = randomizer[1].currentStone;
        cloudNames[2] = randomizer[2].currentStone;
        Debug.Log("Updating Stone Names");
    }

    public void UpdateRemainTasks(int value)
    {
        remainTasks = value;
        remainTasksUI.text = "(" + remainTasks + ")";
    }


    public void RateingStatus(bool value)
    {
        rateStatus = value;
    }

    public void PauseMode(bool status)
    {
        isPause = status;

        if (status)
        {
            onPauseOn.Invoke();
        }
        else onPauseOff.Invoke();
    }

    public void SurveyStart()
    {
        onSurveyStart.Invoke();
    }
    public void SurveyEnd()
    {
        onSurveyEnd.Invoke();
        PauseMode(true);
    }


}

