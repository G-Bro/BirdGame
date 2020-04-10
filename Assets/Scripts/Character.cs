using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private float sensitivity = 1f;
    private float speed = 3f;
    private float velocity = 10f;
    private float maxVelocity = 80f;

    private Rigidbody rb;
    private Quaternion targetAngle;
    private Quaternion birdAngle;

    private GameObject camera;
    private GameObject cameraOrigin;
    private GameObject birdBody;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        targetAngle = Quaternion.Euler(0, 0, 0);
        birdAngle   = targetAngle;

        camera       = GameObject.Find("Camera");
        cameraOrigin = GameObject.Find("Camera Origin");
        birdBody     = GameObject.Find("Bird Body");
    }

    void Update()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 vector = targetAngle.eulerAngles;

        vector.y += (Input.GetAxis("Mouse X") * sensitivity);
        vector.x -= (Input.GetAxis("Mouse Y") * sensitivity);

        // vector.x = Mathf.Clamp(vector.x, -85.0f, 85.0f);

        // Flap wings
        if (Input.GetMouseButton(0))
        {
            velocity += 1f;
        } 

        targetAngle = Quaternion.Euler(vector);

        // Where the fuck am I line.
        Debug.DrawLine(new Vector3(0, 0, 0), rb.position, Color.red);

        // Smooth rotation bit
        birdAngle = Quaternion.Lerp(
            birdAngle,
            targetAngle,
            Time.deltaTime * speed
        );

        // Yep. Does a thing.
        float test = 50;

        birdBody.transform.localRotation = birdAngle
            * Quaternion.Euler(0, 0, birdAngle.eulerAngles.y - targetAngle.eulerAngles.y)
            * Quaternion.Euler(
                Mathf.Max(
                    (90 / Mathf.Pow(test, 3)) * -Mathf.Pow(velocity - test, 3),
                    0
                ),
                0,
                0
            );

        cameraOrigin.transform.localRotation = Quaternion.RotateTowards(
           cameraOrigin.transform.localRotation,
           targetAngle,
           10f
        );

        Vector3 localGravity = Quaternion.Inverse(birdBody.transform.rotation) * Physics.gravity;

        velocity += (localGravity.z * 0.05f);
        // velocity *= 0.999f;
        velocity = Mathf.Clamp(velocity, 0, maxVelocity);

        rb.velocity = birdBody.transform.rotation * new Vector3(0, 0, velocity);
        rb.MovePosition(rb.position + (rb.velocity * Time.deltaTime));

        Debug.Log(velocity);

        // This is fine... Does the camera angle thing at high altitude.
        camera.transform.localRotation = Quaternion.Euler(Mathf.Clamp(4 * (rb.position.y - 20), 8, 40), 0, 0);
    }
}
