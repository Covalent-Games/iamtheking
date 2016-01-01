using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private static CameraInputController _camInputController;

	private void Awake() {

		_camInputController = Camera.main.GetComponent<CameraInputController>();
	}

	public static IEnumerator ZoomOutAndCenterRoutine() {

		Vector3 targetPos = new Vector3(0f, 0f, Camera.main.transform.position.z);
		Vector3 zoomVelocity = Vector3.zero;
		float orthoVelocity = 0f;
		int frameDelayToAcceptTouch = 0;
		while (Camera.main.transform.position != targetPos) {
			frameDelayToAcceptTouch++;
			if (frameDelayToAcceptTouch > 20 && Input.touchCount > 0) {
				break;
			}
			Camera.main.transform.position = Vector3.SmoothDamp(
				Camera.main.transform.position,
				targetPos,
				ref zoomVelocity,
				0.25f);
			Camera.main.orthographicSize = Mathf.SmoothDamp(
				Camera.main.orthographicSize,
				15f,
				ref orthoVelocity,
				0.2f);
			yield return null;
		}
	}

	public static void CenterOnObject(Transform tform) {

		_camInputController.CenterOnObject(tform);
	}
}
