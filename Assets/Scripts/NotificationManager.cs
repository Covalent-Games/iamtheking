using UnityEngine;
using UnityEngine.Extensions;
using System.Collections.Generic;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour {

	public static NotificationManager Instance;

	private RectTransform _notificationView;
	private List<GameObject> _notificationList = new List<GameObject>();
	private RectTransform _blankNotification;

	void Awake () {

		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(this);
		}
		_notificationView = transform.FindChildRecursive("NotificationView_Panel").GetComponent<RectTransform>();
		_blankNotification = transform.FindChildRecursive("Notification_Text").GetComponent<RectTransform>();
	}

	private void Start() {

		//_notificationView.gameObject.SetActive(false);
	}

	public static void DisplayNotification(string message) {

		DisplayNotification(message, new Color(1, 1, 1, 1));
	}

	public static void DisplayNotification(string message, Color color) {

		Notification notification = Instance.GetNextNotification();
		notification.NotificationText.text = message;
		notification.NotificationText.color = color;
		notification.transform.SetParent(Instance._notificationView);
	}

	private Notification GetNextNotification() {

		GameObject notification = null;

		for (int i = 0; i < _notificationList.Count; i++) {
			if (!_notificationList[i].activeSelf) {
				notification = _notificationList[i];
				break;
			}
		}

		if (notification == null) {
			notification = (GameObject)Instantiate(
				_blankNotification.gameObject, 
				_blankNotification.transform.position, 
				_blankNotification.transform.rotation);
			// The false is to keep the world positioning. Otherwise it is 0:0 for width and height.
			notification.transform.SetParent(_notificationView, false);
			notification.GetComponent<RectTransform>().anchoredPosition = _blankNotification.anchoredPosition;
			_notificationList.Add(notification);
		}
		notification.SetActive(true);

		return notification.GetComponent<Notification>();
	}
}
