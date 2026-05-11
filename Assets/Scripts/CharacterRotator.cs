using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterRotator : MonoBehaviour
{
    public float sensitivity = 10f; // Hur snabbt han snurrar
    private float rotationY;

    public void Update()
    {
        // Vi lägger till: && !EventSystem.current.IsPointerOverGameObject()
        // Det betyder: "Om man håller ner musen OCH musen INTE är över en knapp/panel"
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, -mouseX * sensitivity);
        }
    }
}
