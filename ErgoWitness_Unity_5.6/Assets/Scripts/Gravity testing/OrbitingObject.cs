using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will allow for objects to obit each other
/// 
/// Author: Ben Hoffman
[RequireComponent(typeof(Rigidbody))]
public class OrbitingObject : MonoBehaviour {


    public Transform orbitTarget;

    private float mass = 1f;    // 1
    private Rigidbody rb;

    private float mass2 = 100f;  // 100

    [SerializeField]
    private float GRAVITATION_CONST = -9.8f;        // -1000
    private Vector3 differenceVector;


    //private Vector3 _currentPosition;

	public Vector3 Position { get { return transform.position;  } }
    public float Mass { get { return rb.mass; } }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        // Use our rigidbody to determine out mass
        mass = rb.mass;
        // Get the other objects mass
        mass2 = GetComponent<Rigidbody>().mass;

    }

    private void FixedUpdate()
    {
        //_currentPosition = transform.position;

        rb.AddForce(Orbit());

    }

    /// <summary>
    ///  Use Newtons universal law of gravity to calculate the force that should be applied
    ///  towards a given object
    ///  
    /// Author: Ben Hoffman
    /// </summary>
    /// <returns></returns>
    private Vector3 Orbit()
    {
        Vector3 F_orbitForce = Vector3.zero;

        Vector3 R_DifferenceVector = (orbitTarget.position - transform.position);

        Vector3 U_differenceVector = R_DifferenceVector.normalized;

        F_orbitForce = -GRAVITATION_CONST * ( (rb.mass * mass2) / U_differenceVector.sqrMagnitude ) * U_differenceVector;

        return F_orbitForce;
    }

    /// <summary>
    /// Allow this object's mass to be changed
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    /// <param name="additionalMass"></param>
    private void ChangeMass(float additionalMass)
    {
        mass += additionalMass;
    }

}
