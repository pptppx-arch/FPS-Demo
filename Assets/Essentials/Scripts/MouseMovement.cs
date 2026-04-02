using UnityEngine;

public class MouseMovement : MonoBehaviour
{

    public float mouseSensitivty = 100f;

    float xRotation = 0f;
    float yRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;

    public float objectDef = 0f;

    void Start()
    {
       Cursor.lockState = CursorLockMode.Locked; 
    }

    // Update is called once per frame
    void Update()
    {
        if (objectDef == 0f)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivty * Time.deltaTime;
            yRotation += mouseX;
            transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        else
        {
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivty * Time.deltaTime;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
