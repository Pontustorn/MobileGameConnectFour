using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipScript : MonoBehaviour
{
    public bool iHitSomething = false;
    public GameObject confettiPE;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Chip"))
        {
            if (!iHitSomething)
            {
                var confetti = Instantiate(confettiPE, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                Destroy(confetti, 2);
                iHitSomething = true;
            }
        }
    }
}
