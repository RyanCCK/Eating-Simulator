using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TerrainDecay : MonoBehaviour
{
    [SerializeField] public float decayDelaySeconds = 1f;
    [SerializeField] public Material initialMaterial;
    [SerializeField] public Material warningMaterial;
    [SerializeField] public Material finalMaterial;

    private GameManager gameManager;
    private bool isDecaying;
    private enum DecayStates
    {
        initial,
        warning,
        final,
        breaking,
        destroyed
    };
    private DecayStates state;


    private void Awake()
    {
        state = DecayStates.initial;
        isDecaying = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }


    void OnCollisionEnter(Collision other)
    {
        if (!isDecaying && other.gameObject.tag == "Player")
        {
            isDecaying = true;
            StartCoroutine(Decay());
        }
    }


    // Update the state of the tile at a fixed time interval and apply state-based changes
    IEnumerator Decay()
    {
        while (state != DecayStates.destroyed)
        {
            switch (state)
            {
            case DecayStates.initial:
                {
                    //state = DecayStates.warning;
                    state = DecayStates.breaking;
                    //GetComponent<MeshRenderer>().material = warningMaterial;
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
                    state = DecayStates.breaking;
                    gameObject.AddComponent(typeof(Rigidbody));
                    Destroy(GetComponent<Collider>());
                    Destroy(transform.GetChild(0).gameObject);
                    break;
                }
            case DecayStates.breaking:
                {
                    Destroy(gameObject);
                    state = DecayStates.destroyed;
                    break;
                }
            }

            yield return new WaitForSeconds(decayDelaySeconds);
        }
    }
}
