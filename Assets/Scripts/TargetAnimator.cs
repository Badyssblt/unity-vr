using UnityEngine;

/// <summary>
/// Adds visual animations to targets for more juice
/// </summary>
public class TargetAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private bool rotateConstantly = true;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 50, 0);

    [SerializeField] private bool bobUpDown = false;
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private float bobHeight = 0.5f;

    [SerializeField] private bool scaleAnimation = false;
    [SerializeField] private float scaleSpeed = 1f;
    [SerializeField] private float scaleAmount = 0.1f;

    private Vector3 startPosition;
    private Vector3 originalScale;
    private float bobTimer;

    void Start()
    {
        startPosition = transform.position;
        originalScale = transform.localScale;
        bobTimer = Random.Range(0f, Mathf.PI * 2); // Random start phase
    }

    void Update()
    {
        if (rotateConstantly)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }

        if (bobUpDown)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float yOffset = Mathf.Sin(bobTimer) * bobHeight;
            Vector3 newPos = transform.position;
            newPos.y = startPosition.y + yOffset;
            transform.position = newPos;
        }

        if (scaleAnimation)
        {
            float scaleMultiplier = 1 + Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;
            transform.localScale = originalScale * scaleMultiplier;
        }
    }
}
