using UnityEngine;
using UnityEngine.UI;

public class SJM_StarRateing : MonoBehaviour
{
    [Header("Componenti UI")]
    public Button[] stars; // Array di 5 pulsanti/stelle
    public Sprite filledStar;
    public Sprite emptyStar;

    [Header("Impostazioni")]
    [Range(0, 5)]
    public int initialRating = 0; // Voto iniziale da 0 a 5

    public int currentRating = 0;

    public bool findRateManagerInParent = false;
    public SJM_RateManager rateManager;

    void Start()
    {
        if (findRateManagerInParent) rateManager = GetComponentInParent<SJM_RateManager>();
        currentRating = Mathf.Clamp(initialRating, 0, stars.Length);

        for (int i = 0; i < stars.Length; i++)
        {
            int index = i; // Cattura indice per closure
            stars[i].onClick.AddListener(() => SetRating(index + 1));
        }

        UpdateStars();
    }

    public void SetRating(int rating)
    {
        currentRating = Mathf.Clamp(rating, 0, stars.Length);
        UpdateStars();
        rateManager.UpdateRateingValues();
        Debug.Log("Voto assegnato: " + currentRating);
    }

    private void UpdateStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            Image img = stars[i].GetComponent<Image>();
            if (img != null)
            {
                img.sprite = (i < currentRating) ? filledStar : emptyStar;
            }
        }
    }

    public int GetRating()
    {
        return currentRating;
    }

    public void SetInitialRating(int rating)
    {
        initialRating = Mathf.Clamp(rating, 0, stars.Length);
        currentRating = initialRating;
        UpdateStars();
    }
}
