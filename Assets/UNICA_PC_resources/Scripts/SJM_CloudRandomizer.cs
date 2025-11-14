using System.Collections.Generic;
using UnityEngine;
public class SJM_CloudRandomizer : MonoBehaviour
{

    public List<GameObject> cloudsVariants = new List<GameObject>();    // the cloud prefab list
    public GameObject[] cloudReferences;                                // cloud reference in scene
    public GameObject[] shadows;                                        // fake stone shadow items...off by default
    public int index = 0;                                               // list index
    public Transform spawnPivot;                                        // spawning poin for cloud prefabs instantiated
    public string currentStone;                                         // current instantiated stone name
    public int itemsInList = 0;
    public GameObject currentInstantiatedStone;

    // reference
    public SJM_GameManager manager;

    public bool initialized = false;

    // save system
    public string saveFileName = "clouds_remaining.json";


    private void Start()
    {

        SaveLoadInitialization();


        itemsInList = cloudsVariants.Count;

        if (manager == null)
            manager = FindAnyObjectByType(typeof(SJM_GameManager)) as SJM_GameManager;

        manager.UpdateRemainTasks(itemsInList);

        foreach (GameObject item in shadows)
        {
            item.SetActive(false);
        }
    }

    public void SaveLoadInitialization()
    {
        // ripristina lista rimasta se esiste un salvataggio ---
        string path = SJM_RemainingListStore.GetPath(saveFileName);

        if (SJM_RemainingListStore.TryLoadIds(path, out var loadedIds) && loadedIds.Count > 0)
        {
            // Ricostruisci cloudsVariants dai nomi salvati, usando i prefab già assegnati in inspector
            cloudsVariants = SJM_RemainingListStore.RebuildFromIds(loadedIds, cloudsVariants);
        }
        else
        {
            // Prima esecuzione: deduplica per nome e salva lo stato iniziale
            cloudsVariants = SJM_RemainingListStore.RebuildFromIds(
                SJM_RemainingListStore.NamesOf(cloudsVariants),
                cloudsVariants
            );
            SJM_RemainingListStore.SaveIdsAtomic(path, SJM_RemainingListStore.NamesOf(cloudsVariants));
        }

        //-----------------------------------------------
    }

    // whipe data
    [ContextMenu("Clear Save For Next Run")]
    public void ClearSaveForNextRun()
    {
        string path = SJM_RemainingListStore.GetPath(saveFileName);
        SJM_RemainingListStore.WriteEmpty(path);
        Debug.Log("[CloudRandomizer] Salvataggio svuotato. Al prossimo avvio si riparte dai dati di build.");
    }

    // ------------------------------

    public void StartRandomize()
    {
        if (itemsInList > 0)
        {
            if (currentInstantiatedStone != null) Destroy(currentInstantiatedStone);

            index = Random.Range(0, cloudsVariants.Count);

            currentStone = cloudsVariants[index].name;
            currentInstantiatedStone = Instantiate(cloudsVariants[index]);
            InitializeShadows();
            currentInstantiatedStone.transform.position = spawnPivot.position;
            currentInstantiatedStone.transform.rotation = spawnPivot.rotation;
            currentInstantiatedStone.transform.parent = spawnPivot.transform;
            //resultContainer.text = cloudsVariants[index];
            cloudsVariants.Remove(cloudsVariants[index]);

            // Save----------------
            SJM_RemainingListStore.SaveIdsAtomic(SJM_RemainingListStore.GetPath(saveFileName), SJM_RemainingListStore.NamesOf(cloudsVariants));
            // Save ----------------

            manager.UpdateStoneName();

            UpdateListCount();
            UpdateReference();
        }

        //else Debug.Log("End of items");

    }

    public void UpdateListCount()
    {
        itemsInList = cloudsVariants.Count;
        manager.UpdateRemainTasks(itemsInList);
    }

    public void UpdateReference()
    {
        foreach (GameObject item in cloudReferences)
        {
            item.SetActive(false);
        }

        if (currentStone.StartsWith("Stone1"))
        {
            cloudReferences[0].SetActive(true);
        }
        else if (currentStone.StartsWith("Stone2"))
        {
            cloudReferences[1].SetActive(true);
        }
        else if (currentStone.StartsWith("Stone3"))
        {
            cloudReferences[2].SetActive(true);
        }
    }

    public void InitializeShadows()
    {
        if (!initialized)
        {
            foreach (GameObject item in shadows)
            {
                item.SetActive(true);
            }
        }
        initialized = true;
    }


}
