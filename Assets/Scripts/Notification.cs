using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Notification : MonoBehaviour {

	public static float ScrollSpeed = 15f;

	private float _alpha = 1f;
	public Text NotificationText;
	private Vector3 _startPos;

	private void Awake() {

		NotificationText = GetComponent<Text>();
		_startPos = transform.position;
	}

	// Update is called once per frame
	void Update () {

		transform.position =
			new Vector3(transform.position.x, 
						transform.position.y - Time.deltaTime * ScrollSpeed,
						transform.position.z);

		if (transform.localPosition.y < -50) {
			_alpha -= Time.deltaTime;
			Color color = NotificationText.color;
			color.a = _alpha;
			NotificationText.color = color;
			if (_alpha <= 0f) {
				transform.position = _startPos;
				gameObject.SetActive(false);
				_alpha = 1f;
			}
		}
	}
}
