using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    //this is an idea for later
    //will make spells more interesting


    public enum SPELL { straight, burst, spread };

    [Header("Spell Type")]

    public SPELL spellType;

    [Header("Projectiles")]

    public GameObject magicProjectile;
    public int numberOfProjectiles = 1;
    public float burstInterval = .1f;

    private AudioSource magicSound;

    private void Start()
    {
        //magicSound is GetChild(0)
        magicSound = this.gameObject.transform.GetChild(0).GetComponent<AudioSource>();
    }

    public IEnumerator CastSpell(Transform firePoint)
    {
        //play sound
        if (magicSound != null)  //make sure there's something to play
        {
            magicSound.Play();
        }

        if (spellType == SPELL.straight)
        {
            Instantiate(magicProjectile, firePoint.position, transform.rotation);
        }
        else if (spellType == SPELL.burst)
        {
            for (int i = 0; i < numberOfProjectiles; i++)
            {
                Instantiate(magicProjectile, firePoint.position, transform.rotation);
                yield return new WaitForSeconds(burstInterval);
            }
        }
        else if (spellType == SPELL.spread)
        {
            Quaternion currentRotation = transform.rotation;

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                //TODO
                //a bit more difficult
                Debug.Log("No spread spell yet!");
            }
        }

        yield return null;
    }
}
