using UnityEngine;

public class TriggerAbrir : MonoBehaviour
{
    public Animator animator;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInteraction player = other.GetComponent<PlayerInteraction>();

            if (player != null && player.tieneLlave)
            {
                animator.Play("AbrirP");
                Debug.Log("Puerta abierta!");
            }
            else
            {
                Debug.Log("Necesitas una llave para abrir la puerta.");
            }
        }
    }
}