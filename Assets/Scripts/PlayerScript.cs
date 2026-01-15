using UnityEngine;
public class PlayerScript : MonoBehaviour
{
    public float speed = 5.0f; 
    private Vector3 targetPosition;
    private Camera mainCamera;
    //LayerMask layermask;
    void Start()
    {
        RaycastHit hit;
        targetPosition = transform.position;
        mainCamera = Camera.main;
    }

    void Update()
    {
        Debug.DrawRay(transform.position,transform.TransformDirection(Vector3.down));
        if (Input.GetMouseButtonDown(0))
        {
            SetTargetPosition();
            Debug.Log("Mouse");
            Debug.Log(targetPosition);
        }
        MoveObject();
    }


    void SetTargetPosition()
    {

        targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = transform.position.z;
        targetPosition.y = 0.8f;
    }

    void MoveObject()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }
}
