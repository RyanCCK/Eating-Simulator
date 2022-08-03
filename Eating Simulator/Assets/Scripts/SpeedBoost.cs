using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SpeedBoost : MonoBehaviour
{
    [Tooltip("Initial force applied once on contact with the speed boost.")]
    [SerializeField] public float impulseForce = 100f;
    [Tooltip("Force applied every physics frame throughout the Force Duration.")]
    [SerializeField] public float continuouslyAppliedForce = 50f;
    [Tooltip("Maximum allowed speed for the player during the Total Duration.")]
    [SerializeField] public float maxSpeed = 100f;
    [Tooltip("Duration during which the Continuously Applied Force is being applied every physics frame.")]
    [SerializeField] public float forceDuration = 2f;
    [Tooltip("Duration during which velocity is clamed to maxVelocity of speed boost, instead of default player max velocity.")]
    [SerializeField] public float totalDuration = 5f;
    [Tooltip("Normalized and added to normalized player movement vector to calculate true direction.")]
    [SerializeField] public Vector3 forceDirection;
    [Tooltip("How strongly the force direction is weighted agaisnt the player direction. 1 weighs both equivalently, while greater than 1 " +
             "favors force direction, and less than 1 favors player direction.")]
    [SerializeField] public float forceDirectionPower = 1f;
    [Tooltip("Prevent any force from being applied in the positive X direction.")]
    [SerializeField] public bool restrictPosX = false;
    [Tooltip("Prevent any force from being applied in the negative X direction.")]
    [SerializeField] public bool restrictNegX = false;
    [Tooltip("Prevent any force from being applied in the positive Y direction.")]
    [SerializeField] public bool restrictPosY = false;
    [Tooltip("Prevent any force from being applied in the negative Y direction.")]
    [SerializeField] public bool restrictNegY = false;
    [Tooltip("Prevent any force from being applied in the positive Z direction.")]
    [SerializeField] public bool restrictPosZ = false;
    [Tooltip("Prevent any force from being applied in the negative Z direction.")]
    [SerializeField] public bool restrictNegZ = false;
}
