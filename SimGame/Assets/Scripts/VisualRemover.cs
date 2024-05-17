using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualRemover : MonoBehaviour
{
    void Start()
    {
        var checkpoints = GameObject.FindGameObjectsWithTag("checkpoint");
        System.Array.Reverse(checkpoints);
        foreach (var checkpoint in checkpoints)
        {
            Destroy(checkpoint.GetComponent<MeshFilter>());
        }
        Destroy(gameObject);
    }
}
