﻿using UnityEngine;
using UnityEngine.Extensions;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class CameraInputController : MonoBehaviour {

	public static ISelectable CurrentSelectedObject;

	public float ScrollSpeed = 0.75f;
	public float PinchSpeed = 1.5f;

	private Camera _camera;
	private float _previousPinchDistance;
	private bool _centering = false;
	

	void Awake() {

		_camera = GetComponent<Camera>();
	}

	void Update () {

		switch (Input.touchCount) {
			case 1:
				HandleSingleTouch();
				break;
			case 2:
				PinchZoom();
				if (Input.GetTouch(1).phase == TouchPhase.Ended) {
					_previousPinchDistance = 0f;
				}
				break;
		}
		
		if (CurrentSelectedObject != null && !_centering) {
			//CenterCamOnSelected();
		}
	}

	private void CenterCamOnSelected() {

		if (((MonoBehaviour)CurrentSelectedObject).CompareTag("Hero")) {
			Vector3 targetPos = GetCameraCenteringPos(((MonoBehaviour)CurrentSelectedObject).transform);
			_camera.transform.position = targetPos; 
		}

	}

	private void SelectObject(RaycastHit2D hitinfo, Touch touch) {

		if (touch.phase == TouchPhase.Began) {
			ISelectable selected = hitinfo.transform.gameObject.GetComponent<ISelectable>();

			// If we touched a valied selectable object
			if (selected != null) {
				// If the object we touched was already selected.
				if (CurrentSelectedObject == selected) {
					DeselectObject(selected);
					GameManager.Instance.PauseForMenu(false);
				} else {
					if (CurrentSelectedObject != null) {
						DeselectObject(CurrentSelectedObject);
						GameManager.Instance.PauseForMenu(false);
					}
					selected.Select();
					StartCoroutine(CenterSelectableUIRoutine(hitinfo.transform));
					CurrentSelectedObject = selected;
					GameManager.Instance.PauseForMenu(true);
				}
			} 
		}			
	}

	private IEnumerator CenterSelectableUIRoutine(Transform tform) {

		_centering = true;
		Vector3 targetCamPos = GetCameraCenteringPos(tform);

		Vector3 camVelocity = Vector3.zero;
		float orthVelocity = 0f;
		// Smoothly position the camera over the UI element at the correct zoom level.
		while (_camera.transform.position != targetCamPos) {
			_camera.transform.position =
				Vector3.SmoothDamp(_camera.transform.position,
								   targetCamPos,
								   ref camVelocity,
								   .05f);
			_camera.orthographicSize =
				Mathf.SmoothDamp(_camera.orthographicSize,
								 5f,
								 ref orthVelocity,
								 .05f);
			yield return null;
		}
		_centering = false;
	}

	public void CenterOnObject (Transform tform) {

		StartCoroutine(CenterSelectableUIRoutine(tform));
		ISelectable iSelectable = tform.GetComponent<ISelectable>();
		if (iSelectable != null) {
			iSelectable.Select();
		}
		CurrentSelectedObject = iSelectable;

	}

	/// <summary>
	/// Calculates a position correctly aligned with the provided transform's position.
	/// </summary>
	/// <param name="tform">The transform to align to.</param>
	/// <returns>The calculated aligned position.</returns>
	private Vector3 GetCameraCenteringPos(Transform tform) {

		// We need to calculate screen position based on a fully zoomed in camera or the math gets complicated.
		float previousSize = _camera.orthographicSize;
		_camera.orthographicSize = 5f;

		Vector3 targetScreenPos =
			new Vector3(Screen.width / 12f,
						Screen.height - Screen.height / 5.5f,
						0f);
		Vector3 offset = _camera.ScreenToWorldPoint(targetScreenPos) - tform.position;
		Vector3 targetCamPos = _camera.transform.position - offset;
		targetCamPos.z = _camera.transform.position.z;
		// Reassign the camera zoom so it doesnt "snap" and look yucky.
		_camera.orthographicSize = previousSize;

		return targetCamPos;
	}

	private void DeselectObject(ISelectable selected) {

		selected.Deselect();
		CurrentSelectedObject = null;
	}

	private void FingerDrag(Touch touch) {

		Vector3 pos = _camera.transform.position;
		// Subtract the delta to get it appears the map is scrolling and not the camera. 
		// The math foo is to scale scrolling speed based on zoom level, otherwise its REALLY slow when zoomed all the way out.
		pos = pos - (Vector3)touch.deltaPosition * Time.deltaTime * (ScrollSpeed + ((_camera.orthographicSize - 5f) / 10f));
		pos.z = _camera.transform.position.z;
		_camera.transform.position = pos;

		// Deselect any selected object.
		if (CurrentSelectedObject != null) {
			DeselectObject(CurrentSelectedObject);
			GameManager.Instance.PauseForMenu(false);
		}
	}

	private void HandleSingleTouch() {
		
		Touch touch = Input.GetTouch(0);
#if UNITY_EDITOR
		if (!EventSystem.current.IsPointerOverGameObject()) {
			RaycastHit2D hitInfo = Physics2D.Raycast(_camera.ScreenToWorldPoint(touch.position), Vector2.zero);
			if (hitInfo) {
				if (touch.phase == TouchPhase.Began) {
					SelectObject(hitInfo, touch);
					Debug.Log(hitInfo.transform.name);
				} else if (touch.phase == TouchPhase.Moved) {
					FingerDrag(touch);
				}
			} 
		}
#else
		if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
			RaycastHit2D hitInfo = Physics2D.Raycast(_camera.ScreenToWorldPoint(touch.position), Vector2.zero);
			if (hitInfo) {
				if (touch.phase == TouchPhase.Began) {
					SelectObject(hitInfo, touch);
					Debug.Log(hitInfo.transform.name);
				} else if (touch.phase == TouchPhase.Moved) {
					FingerDrag(touch);
				}
			}
		}
#endif
	}


	private void PinchZoom() {
		
		float currentPinchDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
		// Pinch distance is set to zero whenever there are not exactly 2 fingers on the screen.
		if (_previousPinchDistance != 0f) {
			float delta = _previousPinchDistance - currentPinchDistance;
			_camera.orthographicSize =
				Mathf.Clamp(_camera.orthographicSize + delta * Time.deltaTime * PinchSpeed, 5f, 15f);
		}
		_previousPinchDistance = currentPinchDistance;
	}
}
