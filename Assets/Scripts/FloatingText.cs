using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float fadeSpeed = 1f;

    private TextMeshPro textMesh;
    private Color originalColor;

    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            originalColor = textMesh.color;
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Float upwards
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        if (textMesh != null)
        {
            Color color = textMesh.color;
            color.a = Mathf.Lerp(color.a, 0, fadeSpeed * Time.deltaTime);
            textMesh.color = color;
        }

        // Always face camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
    }

    public void SetText(string text)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
        }
    }

    public void SetColor(Color color)
    {
        if (textMesh != null)
        {
            originalColor = color;
            textMesh.color = color;
        }
    }
}
