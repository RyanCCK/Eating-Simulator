using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TerrainDecay : MonoBehaviour
{
    private bool isDecaying;
    /////////////TODO/////////////TODO/////////////TOOD/////////////
    //TODO: Fix these states and the associated switch statement in the Decay loop
    /////////////TODO/////////////TODO/////////////TOOD/////////////
    private enum DecayStates
    {
        initial,
        warning,
        final,
        destroy,
        destroyed
    };
    private DecayStates state;
    [SerializeField] public float decayDelay = 1.5f;
    [SerializeField] public Material initialMaterial;
    [SerializeField] public Material warningMaterial;
    [SerializeField] public Material finalMaterial;
    
    // Start is called before the first frame update
    void Start()
    {
        isDecaying = false;
        state = DecayStates.initial;
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isDecaying && other.gameObject.tag == "Player")
        {
            isDecaying = true;
            StartCoroutine(Decay());
        }
    }
    /////////////TODO/////////////TODO/////////////TOOD/////////////
    //TODO: fix this loop to use fewer states and be cleaner
    /////////////TODO/////////////TODO/////////////TOOD/////////////
    IEnumerator Decay()
    {
        // Update the state of the tile, and modify its appearance
        while (state != DecayStates.destroyed)
        {
            switch (state)
            {
            case DecayStates.initial:
                {
                    state = DecayStates.warning;
                    GetComponent<MeshRenderer>().material = warningMaterial;
                    break;
                }
            case DecayStates.warning:
                {
                    state = DecayStates.final;
                    GetComponent<MeshRenderer>().material = finalMaterial;
                    break;
                }
            case DecayStates.final:
                {
                    state = DecayStates.destroy;
                    gameObject.AddComponent(typeof(Rigidbody));
                    Destroy(GetComponent<Collider>());
                    Destroy(transform.GetChild(0).gameObject);
                    break;
                }
            case DecayStates.destroy:
                {
                    Destroy(gameObject);
                    state = DecayStates.destroyed;
                    break;
                }
            }

            yield return new WaitForSeconds(decayDelay);
        }
    }
}
