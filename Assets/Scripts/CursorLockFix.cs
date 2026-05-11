using UnityEngine;

public class CursorLockFix : MonoBehaviour
{
    void Start()
    {
        // 1. Gör muspekaren osynlig
        Cursor.visible = false;

        // 2. Lås fast musen i mitten av skärmen så den inte åker utanför fönstret
        Cursor.lockState = CursorLockMode.Locked;
    }

}
