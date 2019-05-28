using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS || UNITY_TVOS
using UnityEngine.XR.iOS;
#endif

namespace BToolkit
{
    public class Scanning : MonoBehaviour
    {

        public enum FocusState
        {
            Initializing,
            Finding,
            Found
        }
        public GameObject findingSquare;
        public GameObject foundSquare;
        public float maxRayDistance = 30.0f;
        public LayerMask collisionLayerMask;
        public float findingSquareDist = 0.5f;
        private FocusState squareState;
        public FocusState SquareState
        {
            get
            {
                return squareState;
            }
            set
            {
                squareState = value;
                foundSquare.SetActive(squareState == FocusState.Found);
                findingSquare.SetActive(squareState != FocusState.Found);
            }
        }
        bool trackingInitialized;
        public delegate void FoundAction(Vector3 position, Quaternion rotation);
        public FoundAction OnFoundEvent = null;

        public void StartScan()
        {
            this.enabled = false;
        }

        void Start()
        {
            SquareState = FocusState.Initializing;
            trackingInitialized = true;
            this.enabled = false;
        }

        void Update()
        {
            Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, findingSquareDist);
#if UNITY_EDITOR
            Ray ray = Camera.main.ScreenPointToRay(center);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxRayDistance, collisionLayerMask))
            {
                foundSquare.transform.position = hit.point;
                foundSquare.transform.rotation = hit.transform.rotation;
                if (SquareState != FocusState.Found)
                {
                    SquareState = FocusState.Found;
                    if (OnFoundEvent != null)
                    {
                        OnFoundEvent(foundSquare.transform.position, foundSquare.transform.rotation);
                    }
                }
                return;
            }
#elif (UNITY_IOS || UNITY_TVOS)
		var screenPosition = Camera.main.ScreenToViewportPoint(center);
		ARPoint point = new ARPoint {
			x = screenPosition.x,
			y = screenPosition.y
		};
		ARHitTestResultType[] resultTypes = {
			ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
			// if you want to use infinite planes use this:
			//ARHitTestResultType.ARHitTestResultTypeExistingPlane,
			//ARHitTestResultType.ARHitTestResultTypeHorizontalPlane, 
			//ARHitTestResultType.ARHitTestResultTypeFeaturePoint
		}; 
		foreach (ARHitTestResultType resultType in resultTypes){
			if (HitTestWithResultType (point, resultType)){
                if (SquareState != FocusState.Found) {
                    SquareState = FocusState.Found;
                    if (OnFoundEvent != null) {
                        OnFoundEvent(foundSquare.transform.position, foundSquare.transform.rotation);
                    }
                }
				return;
			}
		}
#endif
            if (trackingInitialized)
            {
                SquareState = FocusState.Finding;
                if (Vector3.Dot(Camera.main.transform.forward, Vector3.down) > 0)
                {
                    findingSquare.transform.position = Camera.main.ScreenToWorldPoint(center);
                    Vector3 vecToCamera = findingSquare.transform.position - Camera.main.transform.position;
                    Vector3 vecOrthogonal = Vector3.Cross(vecToCamera, Vector3.up);
                    Vector3 vecForward = Vector3.Cross(vecOrthogonal, Vector3.up);
                    findingSquare.transform.rotation = Quaternion.LookRotation(vecForward, Vector3.up);
                }
                else
                {
                    findingSquare.SetActive(false);
                }
            }
        }

#if UNITY_IOS || UNITY_TVOS
    bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes) {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count > 0) {
            foreach (var hitResult in hitResults) {
                foundSquare.transform.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                foundSquare.transform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                return true;
            }
        }
        return false;
    }
#endif
    }
}