using UnityEngine;

public class SJM_SelectorManager : MonoBehaviour
{
    public SJM_Selector[] selectors;
    public string playerID;
    public SJM_GameManager gameManager;

    private void Start()
    {
        selectors = GetComponentsInChildren<SJM_Selector>();
        playerID = selectors[0].currentValue + "" + selectors[1].currentValue + "" + selectors[2].currentValue + "";
        gameManager.UpdatePlayerID(playerID);
    }

    public void UpdateSelectorsValues()
    {
        playerID = selectors[0].currentValue + "" + selectors[1].currentValue + "" + selectors[2].currentValue + "";
        gameManager.UpdatePlayerID(playerID);

    }



}
