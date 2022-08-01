using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] public Vector3 direction = new Vector3(0f, 0f, 1f);
    [SerializeField] public float acceleration = 50f;
    [SerializeField] public float duration = 1f;
}
