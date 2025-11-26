using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    public bool tieneLlave = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            tieneLlave = true;
            Debug.Log("Llave recogida!");
            Destroy(other.gameObject);
        }
    }
}
