using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualUnit : MonoBehaviour {

    public List<Vector3> nodes = new List<Vector3>() { Vector3.zero, Vector3.zero};
    public bool isInitialized = false;

	// Use this for initialization
	void Start () {
	
	}

    void OnDrawGizmosSelected()
    {
        if (isInitialized && nodes.Count > 0)
        {
            VisualClass.DrawPath(nodes.ToArray(), Color.green);
        }
    }
}
