using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string transitionTo;
    [SerializeField] private string entryID;   // UNIQUE per trigger
    [SerializeField] private Transform startPoint;

    private void Start()
    {
        if (GameManager.Instance.lastEntryID == entryID)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = startPoint.position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.lastEntryID = entryID;
        SceneManager.LoadScene(transitionTo);
    }
}
