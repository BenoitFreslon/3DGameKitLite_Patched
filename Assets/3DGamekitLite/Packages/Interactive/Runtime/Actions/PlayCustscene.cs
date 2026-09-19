using System.Collections;
using UnityEngine;

using Gamekit3D.GameCommands;

public class PlayCutscene : GameCommandHandler
{
    [Header("Cutscene Settings")]
    public Unity.Cinemachine.CinemachineVirtualCamera virtualCamera;
    public float cutsceneDuration = 3f;
    public int highPriority = 20;

    private int originalPriority;

    protected override void Awake()
    {
        base.Awake();

        // ⚠ EXACTEMENT comme SimpleTransformer : tout faire au Awake APRES base.Awake()
        if (virtualCamera == null)
            virtualCamera = GetComponent<Unity.Cinemachine.CinemachineVirtualCamera>();

        virtualCamera.enabled = false;
        Debug.Log("[PlayCutscene] Registered with interactionType = " + interactionType);
    }

    public override void PerformInteraction()
    {
        Debug.Log("[PlayCutscene] PerformInteraction RECEIVED !");

        if (virtualCamera == null)
        {
            Debug.LogWarning("[PlayCutscene] No virtual camera found.");
            return;
        }

        virtualCamera.enabled = true;


        // Sauvegarde et override de la priorité
        originalPriority = virtualCamera.Priority;
        virtualCamera.Priority = highPriority;

        StartCoroutine(CutsceneCoroutine());
    }

    private IEnumerator CutsceneCoroutine()
    {
        yield return new WaitForSeconds(cutsceneDuration);

        // Restaure la priorité originale
        virtualCamera.Priority = originalPriority;

        Debug.Log("[PlayCutscene] Cutscene finished, priority restored.");
    }
}
