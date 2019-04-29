using System.Collections.Generic;
using UnityEngine;
#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
using UnityEngine.XR.iOS;
#endif

namespace BToolkit
{
    public abstract class ARKitTouch : MonoBehaviour
    {

        public enum TouchState { Down, Move }

        public void OnTouch(Vector3 screenPosition, TouchState state)
        {
#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
        var viewPosition = Camera.main.ScreenToViewportPoint(screenPosition);
        ARPoint point = new ARPoint { x = viewPosition.x, y = viewPosition.y };
        ARHitTestResultType[] resultTypes = {
                ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
				// if you want to use infinite planes use this:
				ARHitTestResultType.ARHitTestResultTypeExistingPlane,
                ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
                ARHitTestResultType.ARHitTestResultTypeFeaturePoint
            };
        foreach (ARHitTestResultType resultType in resultTypes) {
            if (HitTestWithResultType(point, resultType, state)) {
                return;
            }
        }
#endif
        }

#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
    bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes, TouchState state) {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count > 0) {
            foreach (var hitResult in hitResults) {
                //Debug.Log ("Got hit!");
                Vector3 position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                Quaternion rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                if (state == TouchState.Down) {
                    OnTouchDown(position, rotation);
                } else if (state == TouchState.Move) {
                    OnTouchMove(position, rotation);
                }
                return true;
            }
        }
        return false;
    }
#endif

        public abstract void OnTouchDown(Vector3 position, Quaternion rotation);
        public abstract void OnTouchMove(Vector3 position, Quaternion rotation);
    }
}