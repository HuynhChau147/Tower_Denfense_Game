using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    public Transform transform;
    public SpriteRenderer spriteRenderer;
    public int tileID;
    // public void InitColor(bool isOffset)
    // {
    //     spriteRenderer.color = isOffset ? offsetColor : baseColor;
    // }
}
