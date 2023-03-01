using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Item
{
    public class CatalogObject : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 4f;
        
        private ARRaycastManager aRRaycastManager;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();

        private void Start()
        {
            aRRaycastManager = FindObjectOfType<ARRaycastManager>();
            
            var screenPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            aRRaycastManager.Raycast(screenPosition, hits, TrackableType.Planes);
            
            if (hits.Count > 0)
            {
                transform.position = hits[0].pose.position;
                transform.rotation = hits[0].pose.rotation;
            }
            
            hits.Clear();
        }

        private void Update()
        {
            if (Input.touchCount == 0) return;

            if (Input.touchCount == 1) 
                MoveObject();

            if (Input.touchCount == 2) 
                RotateObject();
        }

        private void MoveObject()
        {
            Touch touch = Input.GetTouch(0);
            aRRaycastManager.Raycast(touch.position, hits, TrackableType.Planes);

            if (hits.Count > 0)
            {
                Pose hitPose = hits[0].pose;
                transform.position = hitPose.position;
                hits.Clear();
            }
        }

        private void RotateObject()
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Moved)
            {
                var y = -touch.deltaPosition.x * rotationSpeed * Time.deltaTime;
                transform.Rotate(0f, y, 0f);
            }
        }
    }
}