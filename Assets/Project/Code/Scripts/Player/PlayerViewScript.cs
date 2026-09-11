using UnityEngine;

public class PlayerViewScript : MonoBehaviour
{
    public float MouseSensitivity = 100f;

    public GameObject PlayerBody;

    public GameObject PlayerHead;

    private float _XRotation = 0.0f;

    private Transform _PlayerBodyTransform;

    private Transform _PlayerHeadTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerBody is not null)
        {
            _PlayerBodyTransform = PlayerBody.GetComponent<Transform>();
        }

        if (PlayerHead is not null)
        {
            _PlayerHeadTransform = PlayerHead.GetComponent<Transform>();
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (_PlayerBodyTransform is not null
            && _PlayerHeadTransform is not null)
        {
            var mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

            _XRotation -= mouseY;
            _XRotation = Mathf.Clamp(_XRotation, -90f, 60f);

            _PlayerHeadTransform.localRotation = Quaternion.Euler(_XRotation, 0f, 0f);

            _PlayerBodyTransform.Rotate(new Vector3(0, 1.0f, 0) * mouseX);
        }
    }
}
