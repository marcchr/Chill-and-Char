using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] GameObject winText;
    void Start()
    {
        winText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        winText.SetActive(true);
    }
}
