using UnityEngine;
public class Attack1_trigger : MonoBehaviour
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
        if (other.CompareTag("Enemy"))
        {
            player.GetComponent<Player>().attack1CanHit = true;
            if (!player.GetComponent<Player>().targets.Contains(other.gameObject))
            {
                player.GetComponent<Player>().targets.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && player.GetComponent<Player>().targets.Contains(other.gameObject))
        {
            player.GetComponent<Player>().targets.Remove(other.gameObject);
        }
        if(other.CompareTag("Enemy") && player.GetComponent<Player>().targets.Count == 0)
        {
            player.GetComponent<Player>().attack1CanHit = false; //herhangi bir düşman alandan çıktıysa ve alanda düşman kalmadıysa saldırı yapmayı durdur
        }
    }
}
