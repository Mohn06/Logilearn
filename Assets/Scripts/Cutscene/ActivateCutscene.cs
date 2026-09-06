using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ActivateCutscene : MonoBehaviour
{
     [SerializeField] private PlayableDirector cutsceneDirector;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cutsceneDirector.Play();
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
