using UnityEngine;
using UnityEngine.UI;

public class CropProgressUI : MonoBehaviour
{
    public Image fillImage;
    public CanvasGroup canvasGroup;

    public float fadeStartDistance = 3f;
    public float fadeEndDistance = 6f;

    private Transform target;
    private Camera cam;
    public Transform player;
    private float currentProgress;
    private float dist;

    public void Initialize(Transform target, Camera cam, Transform player)
    {
        this.target = target;
        this.cam = cam;
        this.player = player;

        currentProgress = 0f;
        fillImage.fillAmount = 0f;
        canvasGroup.alpha = 1f;
    }

    

    public void SetProgress(float t)
    {
        currentProgress = t;
        fillImage.fillAmount = t;
    }
    void Update()
    {
        if (target == null || cam == null || player == null) return;

        Vector3 worldPos = target.position + Vector3.up * 1.5f;
        transform.position = worldPos;
        transform.forward = cam.transform.forward;

        float dist = Vector3.Distance(player.position, target.position);
        float alpha = Mathf.InverseLerp(fadeEndDistance, fadeStartDistance, dist);

        if (currentProgress >= 1f)
            alpha = 0f;

        canvasGroup.alpha = alpha;
    }
}