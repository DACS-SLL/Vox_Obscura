using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float rangoInteraccion = 3f;
    public LayerMask capaInteractuable;
    public Camera camaraJugador;
    public bool tieneLlave = false;

    void Update()
    {

        Ray rayo = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, rangoInteraccion, capaInteractuable))
        {
            if (hit.collider.CompareTag("Key"))
            {
                Debug.Log("Presiona E para recoger la llave");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    tieneLlave = true;
                    Destroy(hit.collider.gameObject);
                    Debug.Log("Llave recogida!");
                }
            }
        }
    }
}

