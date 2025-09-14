using UnityEngine;

public class Attack2_trigger : MonoBehaviour
{
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !player.GetComponent<Player>().attack1CanHit)
        {
            player.GetComponent<Player>().Attack_2();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && !player.GetComponent<Player>().attack1CanHit)
        {
            player.GetComponent<Player>().Attack_2();
        }
    }
}
