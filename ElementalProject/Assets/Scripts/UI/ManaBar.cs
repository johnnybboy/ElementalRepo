using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : MonoBehaviour
{
    public static ManaBar instance;
    [SerializeField] GameObject manaContainerPrefab;
    [SerializeField] List<GameObject> manaContainers;
    public int totalMana;
    public float currentMana;
    ManaContainer currentContainer;

    GameObject player;

    void Start()
    {
        instance = this;
        manaContainers = new List<GameObject>();

        player = GameObject.FindGameObjectWithTag("Player");

        SetupMana(player.GetComponent<PlayerController1>().maxMana);
    }

    public void SetupMana(int manaIn)
    {
        manaContainers.Clear();
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        totalMana = manaIn;
        currentMana = (float)totalMana;

        for (int i = 0; i < totalMana; i++)
        {
            GameObject newMana = Instantiate(manaContainerPrefab, transform);
            manaContainers.Add(newMana);
            if (currentContainer != null)
            {
                currentContainer.next = newMana.GetComponent<ManaContainer>();
            } 
            currentContainer = newMana.GetComponent<ManaContainer>();
        }

        currentContainer = manaContainers[0].GetComponent<ManaContainer>();
    }

    public void SetCurrentMana(float mana)
    {
        currentMana = mana;
        currentContainer.SetMana(currentMana);
    }

    //ManaBar.instance.AddMana(float manaUp)
    public void AddMana(float manaUp)
    {
        currentMana += manaUp;
        if (currentMana > totalMana)
            currentMana = (float)totalMana;
        currentContainer.SetMana(currentMana);
    }

    //ManaBar.instance.RemoveMana(float manaDown);
    public void RemoveMana(float manaDown)
    {
        currentMana -= manaDown;
        if (currentMana < 0)
            currentMana = 0f;
        currentContainer.SetMana(currentMana);
    }

    public void AddContainer()
    {
        GameObject newMana = Instantiate(manaContainerPrefab, transform);
        currentContainer = manaContainers[manaContainers.Count - 1].GetComponent<ManaContainer>();
        manaContainers.Add(newMana);

        if (currentContainer != null)
            currentContainer.next = newMana.GetComponent<ManaContainer>();

        currentContainer = manaContainers[0].GetComponent<ManaContainer>();

        totalMana++;
        currentMana = totalMana;
        SetCurrentMana(currentMana);
    }

}
