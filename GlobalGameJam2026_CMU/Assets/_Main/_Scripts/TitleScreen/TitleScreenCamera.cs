using UnityEngine;

public class TitleScreenCamera : MonoBehaviour
{
    public float positionAmount = 0.05f;
    public float rotationAmount = 0.5f;
    public float speed = 0.5f;

    Vector3 startPos;
    Vector3 startRot;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localEulerAngles;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * speed) * positionAmount;
        float y = Mathf.Cos(Time.time * speed * 0.8f) * positionAmount;

        transform.localPosition = startPos + new Vector3(x, y, 0);

        float rotZ = Mathf.Sin(Time.time * speed) * rotationAmount;
        transform.localEulerAngles = startRot + new Vector3(0, 0, rotZ);
    }
}
