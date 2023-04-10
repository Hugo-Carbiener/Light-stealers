using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourcePopUp : MonoBehaviour
{
    private static GameObject resourcePopUpPrefab;
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] SpriteRenderer spriteRdr;
    private Color textColor;
    private Color spriteColor;
    private float disappearTimer;
    private float moveYSpeed = 5;

    private void Awake()
    {
        if (!textMesh) textMesh = GetComponentInChildren<TextMeshPro>();
        if (!spriteRdr) spriteRdr = GetComponentInChildren<SpriteRenderer>();
        spriteColor = spriteRdr.color;
    }

    public static ResourcePopUp Create(Vector3 worldPosition, int amount, ResourceTypes resource)
    {
        resourcePopUpPrefab = Resources.Load("ResourcePopUp") as GameObject;
        GameObject gainPopUpObject = Instantiate(resourcePopUpPrefab, worldPosition, Quaternion.identity);
        ResourcePopUp gainPopUp = gainPopUpObject.GetComponent<ResourcePopUp>();
        gainPopUp.Setup(amount, resource);

        return gainPopUp;
    }

    public void Setup(int amount, ResourceTypes resource)
    {
        if(amount >= 0)
        {
            textMesh.text = "+" + amount;
            textColor = new Color(47, 147, 47);
        }
        else
        {
            textMesh.text = amount.ToString();
            textColor = new Color(190, 39, 39);
            moveYSpeed *= -0.5f;
            transform.localScale *= +0.75f;
            transform.position = transform.position + Vector3.down * 3;
        }
        disappearTimer = 1f;

        switch(resource)
        {
            case ResourceTypes.Food:
                spriteRdr.sprite = GameAssets.i.foodIcon;
                break;
            case ResourceTypes.Wood:
                spriteRdr.sprite = GameAssets.i.woodIcon;
                break;
            case ResourceTypes.Stone:
                spriteRdr.sprite = GameAssets.i.stoneIcon;
                break;
        }
    }

    private void Update()
    {
        disappearTimer -= Time.deltaTime;
        transform.position += new Vector3(0, moveYSpeed, 0) * Time.deltaTime;

        if (disappearTimer < 0)
        {
            float disappearSpeed = 5f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            spriteColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            spriteRdr.color = spriteColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
