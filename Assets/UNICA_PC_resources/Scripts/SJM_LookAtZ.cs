using UnityEngine;

[ExecuteAlways]
public class SJM_LookAtZ : MonoBehaviour
{

    [Header("Target Options")]
    [Tooltip("Se attivo, l'oggetto ruoterà verso la camera invece che verso un Transform personalizzato.")]
    public bool useCamera = true;

    [Tooltip("Camera verso cui ruotare. Se non assegnata, verrà usata la Camera principale.")]
    public Camera targetCamera;

    [Tooltip("Transform alternativo da guardare se 'Use Camera' è disattivato.")]
    public Transform targetTransform;

    [Header("Rotation Settings")]
    [Tooltip("Se attivo, blocca la rotazione sull'asse Y locale (mantiene l'oggetto verticale).")]
    public bool lockY = false;

    [Tooltip("Soglia (in gradi) oltre la quale la rotazione viene aggiornata.")]
    [Range(0.01f, 5f)]
    public float rotationThreshold = 0.1f;

    private Quaternion lastRotation;

    void OnEnable()
    {
        if (useCamera && targetCamera == null)
            targetCamera = Camera.main;

        lastRotation = transform.rotation;
        UpdateRotation();
    }

    void Update()
    {
#if UNITY_EDITOR
        // In Editor, aggiorna anche quando non si è in Play Mode
        if (!Application.isPlaying)
        {
            if (useCamera && targetCamera == null)
                targetCamera = Camera.main;

            UpdateRotation();
            return;
        }
#endif
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        Vector3? targetPos = null;

        if (useCamera && targetCamera != null)
            targetPos = targetCamera.transform.position;
        else if (!useCamera && targetTransform != null)
            targetPos = targetTransform.position;

        if (!targetPos.HasValue)
            return;

        Vector3 direction = targetPos.Value - transform.position;

        if (lockY)
            direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        // Aggiorna solo se la differenza è significativa
        if (Quaternion.Angle(transform.rotation, targetRotation) > rotationThreshold)
        {
            transform.rotation = targetRotation;
            lastRotation = targetRotation;
        }
    }

#if UNITY_EDITOR
    // Mostra un gizmo che indica la direzione dell'asse Z in Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 forward = transform.forward * 0.5f;
        Gizmos.DrawLine(transform.position, transform.position + forward);
        Gizmos.DrawSphere(transform.position + forward, 0.02f);
    }
#endif
}









