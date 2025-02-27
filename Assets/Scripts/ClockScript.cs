using UnityEngine;

public class ClockScript : MonoBehaviour {
    [SerializeField] private GameObject secondHand;
    [SerializeField] private GameObject minuteHand;
    [SerializeField] private GameObject hourHand;
    [SerializeField] public float speed = 1f;
    private const float SECOND_ROT = 360f / 60f;
    private const float MINUTE_ROT = 360f / 60f / 60f;
    private const float HOUR_ROT = 360f / 60f / 60f / 12f;

    void Update() {
        secondHand.transform.Rotate(0, 0, speed * SECOND_ROT * Time.deltaTime);
        minuteHand.transform.Rotate(0, 0, speed * MINUTE_ROT * Time.deltaTime);
        hourHand.transform.Rotate(0, 0, speed * HOUR_ROT * Time.deltaTime);
    }
}
