using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SJM_SendData : MonoBehaviour
{

    // google form fields iD 
    public string userIDField = "entry.1970875722";   // codice del campo relativo alla domanda in google form
    public string slot1NameField = "entry.2140967679";
    public string slot1Field = "entry.164424999";
    public string slot2NameField = "entry.2017213156";
    public string slot2Field = "entry.1388266961";
    public string slot3NameField = "entry.1652895108";
    public string slot3Field = "entry.881121697";

    // tmp variables
    private string ID;      // id utente
    private string S1N;     // nome pietra nel primo slot
    private string S1R;      // voto primo slot
    private string S2N;     // nome pietra nel secondo slot
    private string S2R;      // voto secondo slot
    private string S3N;     // nome pietra terzo slot
    private string S3R;      // voto terzo slot

    SJM_GameManager manager;

    [SerializeField]
    private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSe3lxcziBxR5jI-d9p1OByJGMOD7QJR5FP7FpH108YNkfOx6Q/formResponse";

    private void Start()
    {
        if (manager == null)
            manager = FindAnyObjectByType(typeof(SJM_GameManager)) as SJM_GameManager;
    }

    IEnumerator Post(string id, string s1n, string s1r, string s2n, string s2r, string s3n, string s3r)
    {
        WWWForm form = new WWWForm();
        form.AddField(userIDField, int.Parse(id));
        form.AddField(slot1NameField, s1n.ToString());
        form.AddField(slot1Field, int.Parse(s1r));
        form.AddField(slot2NameField, s2n.ToString());
        form.AddField(slot2Field, int.Parse(s2r));
        form.AddField(slot3NameField, s3n.ToString());
        form.AddField(slot3Field, int.Parse(s3r));

        UnityWebRequest www = UnityWebRequest.Post(BASE_URL, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }
    public void Send()
    {
        ID = manager.playerID;
        S1N = manager.cloudNames[0];
        S1R = manager.rateValues[0].ToString();
        S2N = manager.cloudNames[1];
        S2R = manager.rateValues[1].ToString();
        S3N = manager.cloudNames[2];
        S3R = manager.rateValues[2].ToString();
        Debug.Log(ID + ";" + S1N + ";" + S1R + ";" + S2N + ";" + S2R + ";" + S3N + ";" + S3R);
        StartCoroutine(Post(ID, S1N, S1R, S2N, S2R, S3N, S3R));
        //Debug.Log("data recieved");

    }
}
