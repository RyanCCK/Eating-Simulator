using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationZone : MonoBehaviour
{
    [Tooltip("The rotation in world coordinates that the player's rotation will be set to.")]
    [SerializeField] public Quaternion rotation;
    [SerializeField] public float rotationSpeed;
}
