using UnityEngine;
using UnityEngine.EventSystems;

public class TitleScreen : MonoBehaviour
{
    private void Start()
    {
        EventSystem.current = GetComponentInChildren<EventSystem>();
    }

    void Update()
    {
        if (Input.anyKey)
        {
            Destroy(this.gameObject);
        }
    }
}