using UnityEngine;

public class SJM_RateManager : MonoBehaviour
{
    // rateing components
    public SJM_StarRateing[] ratingSlots;

    // find rating component in children

    public bool findSlotsOnCildren = true;

    // rateing values storage
    public int[] ratingValues;


    SJM_GameManager manager;

    private void Start()
    {
        if (manager == null)
            manager = FindAnyObjectByType(typeof(SJM_GameManager)) as SJM_GameManager;

        if (findSlotsOnCildren) ratingSlots = GetComponentsInChildren<SJM_StarRateing>();

        ratingValues = new int[ratingSlots.Length];
        UpdateRateingValues();

    }

    public void UpdateRateingValues()
    {
        Debug.Log("update rateing");

        ratingValues[0] = ratingSlots[0].currentRating;
        ratingValues[1] = ratingSlots[1].currentRating;
        ratingValues[2] = ratingSlots[2].currentRating;

        manager.rateValues = ratingValues;

    }

    public void ResetRatingValues()
    {
        ratingSlots[0].SetRating(1);
        ratingSlots[1].SetRating(1);
        ratingSlots[2].SetRating(1);

        UpdateRateingValues();
    }
}

