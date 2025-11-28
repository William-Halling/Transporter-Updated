using UnityEngine;

public class VoidKillZone : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        CharacterController cc = other.GetComponent<CharacterController>();


        if (cc != null && respawnPoint != null)
        {
            cc.enabled = false;
            other.transform.position = respawnPoint.position;
            cc.enabled = true;
        }
    }
}
