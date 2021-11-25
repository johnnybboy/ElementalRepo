using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaContainer : MonoBehaviour
{
    public ManaContainer next;

    [Range(0, 1)] float fill;
    [SerializeField] Image fillImage;

   public void SetMana(float count)
    {
        fill = count;
        fillImage.fillAmount = fill;
        count--;
        if (next != null)
        {
            next.SetMana(count);
        }
    }
}
