using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemSelectionUI : MonoBehaviour
{
    public Image selectionImage;
    public CanvasGroup canvasGroup;

    private Transform player;
    public Camera cam;


    public void Initialize(Transform player, Camera cam)
    {
        this.cam = cam;
        this.player = player;
        canvasGroup.alpha = 0f;
    }
    void Start()
    {
        canvasGroup.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = cam.transform.forward;
    }

    public void setIcon(Sprite newImage)
    {
        StopAllCoroutines();
        if (canvasGroup.alpha < 1f)
            canvasGroup.alpha = 1f;
        if (newImage != null)
            selectionImage.sprite = newImage;
        StartCoroutine(fadeIcon(3f));
    }

    IEnumerator fadeIcon(float delay)
    {
        yield return new WaitForSeconds(delay);
        canvasGroup.alpha = 0f;
    }
}
