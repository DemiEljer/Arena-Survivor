using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public float Speed = 1.0f;

    public float ExtendedSpeed = 3.0f;

    private Transform _PlayerTransform { get; set; }

    private Rigidbody _PlayerRigidbody { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _PlayerTransform = GetComponent<Transform>();
        _PlayerRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");
        var currentSpeed = Input.GetButton("Sprint") ? ExtendedSpeed : Speed;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * currentSpeed;
        Vector3.ClampMagnitude(movement, currentSpeed);
        movement *= Time.deltaTime;

        _PlayerTransform.Translate(movement);
    }
}
