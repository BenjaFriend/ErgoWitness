using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

    public GameObject laserPrefab;
    private GameObject _laser;
    private Transform _laserTransform;
    private Vector3 _hitPoint;

    
    public Transform cameraRigTransform;        // The transform of the camera rig
        
    public GameObject teleportReticlePrefab;    // The teleporting reticle that will be put on the ground 
                    
    private GameObject reticle;                 // Reference to the instance of the teleport reticle
    
    private Transform teleportReticleTransform; // Where the reticle will be put on the gorund
        
    public Transform headTransform;             // The location of the camera for the player
            
    public Vector3 teleportReticleOffset;       // Any offset that we need for the teleporter
    
    public LayerMask teleportMask;              //                
                        
    private bool shouldTeleport;                // Should we teleport or not

    private SteamVR_TrackedObject trackedObj;       // The tracked object that is the controller


    private SteamVR_Controller.Device Controller    // The device property that is the controller, so that we can tell what index we are on
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake()
    {
        // Get the tracked object componenet
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void Start()
    {
        // Instantiate the laser object
        _laser = Instantiate(laserPrefab);
        // Set the transform of the laser
        _laserTransform = _laser.transform;

        // Instantiate a reticle object
        reticle = Instantiate(teleportReticlePrefab);
        // Move that object to where it should be
        teleportReticleTransform = reticle.transform;

    }

    // Update is called once per frame
    void Update ()
    {
        // When we release the touch pad button, we want to teleport...
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                _hitPoint = hit.point;
                ShowLaser(hit);
                // Show the reticle
                reticle.SetActive(true);
                // Move the reticle to where we are htiting with the raycast
                teleportReticleTransform.position = _hitPoint + teleportReticleOffset;
                // Determine that we should teleport
                shouldTeleport = true;

            }
            else
            {
                // Disable the laster because we don't want to show it
                laserPrefab.SetActive(false);
                // Set the reticle as inactive as well
                reticle.SetActive(false);

            }
        }

        // When we release the touchpad, then we want to move
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
        {
            TeleportPlayer();
        }
    }


    private void ShowLaser(RaycastHit hit)
    {
        _laser.SetActive(true);

        _laserTransform.position = Vector3.Lerp(trackedObj.transform.position, _hitPoint, .5f);
        _laserTransform.LookAt(_hitPoint);
        _laserTransform.localScale = new Vector3(_laserTransform.localScale.x, _laserTransform.localScale.y, hit.distance);
    }

    private void TeleportPlayer()
    {
        // We should not teleport while we are teleporting
        shouldTeleport = false;
        // Set the reticle as inactive because we have now teleported
        reticle.SetActive(false);
        // Calculate the difference between where we want to be and where we are
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        // We do not want want a Y difference
        difference.y = 0;
        // Move The player's head(the camera) as well
        cameraRigTransform.position = _hitPoint + difference;
    }
}
