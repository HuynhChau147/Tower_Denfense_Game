using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMN : MonoBehaviour
{
    [SerializeField] private int mapWidth, mapHeight;
    [SerializeField] private float spacingX, spacingY;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Sprite emptyTile, downPath, leftRight, leftDown, rightDown, downLeft, downRight;
    [SerializeField] private Sprite spriteToUse;
    [SerializeField] private Transform cam;
    private bool forceDirectionChange = false;

    private bool continueLeft = false;
    private bool continueRight = false;
    private int currentCount = 0;
    [SerializeField] private int curX, curY;
    Tile[,] tileArray;
    private enum CurrentDirection
    {
        LEFT,
        RIGHT,
        DOWN,
        UP
    };
    private CurrentDirection curDirection = CurrentDirection.DOWN;

    private void Start()
    {
        Init();
        StartCoroutine(GenerationMap());
    }

    IEnumerator GenerationMap()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                float posX = i * spacingX + transform.position.x;
                float posY = j * spacingY + transform.position.y;

                var spawnTile = Instantiate(tilePrefab, new Vector3(posX, posY), Quaternion.identity);
                spawnTile.name = $"Tile {i},{j}";
                tileArray[i, j] = spawnTile;
                // bool offsetColor = (i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0);
                // spawnTile.InitColor(offsetColor);
                yield return new WaitForSeconds(0.05f);
            }
        }

        StartCoroutine(GeneratePath());
    }

    private void Init()
    {
        cam.transform.position = new Vector3((float)mapWidth * spacingX / 2 - 0.5f, (float)mapHeight * spacingY / 2 - 0.5f, -10);
        tileArray = new Tile[mapWidth, mapHeight];
    }

    IEnumerator GeneratePath()
    {
        curX = 0;
        curY = UnityEngine.Random.Range(0, mapHeight);

        spriteToUse = downPath;

        while (curX <= mapWidth - 1)
        {
            CheckCurrentDirections();
            ChooseDirection();

            if (curX <= mapWidth - 1)
            {
                UpdateMap(curX, curY, spriteToUse);
                // float posY = mapVisuals[curX, curY].gameObject.transform.position.y + pathOffset;
                // mapVisuals[curX, curY].gameObject.transform.position = new Vector3
                //     (mapVisuals[curX, curY].gameObject.transform.position.x, posY);

                // mapVisuals[curX, curY].tileType = MaptileArray.TileType.PATH;
            }

            if (curDirection == CurrentDirection.DOWN)
            {
                curX++;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    private void CheckCurrentDirections()
    {
        if (curDirection == CurrentDirection.LEFT && curY - 1 >= 0 && tileArray[curX, curY - 1].tileID == 0)
        {
            curY--;
        }
        else if (curDirection == CurrentDirection.RIGHT && curY + 1 <= mapHeight - 1 && tileArray[curX, curY + 1].tileID == 0)
        {
            curY++;
        }
        else if (curDirection == CurrentDirection.UP && curX - 1 >= 0 && tileArray[curX - 1, curY].tileID == 0)
        {
            if (continueLeft && tileArray[curX + 1, curY + 1].tileID == 0 ||
            continueRight && tileArray[curX - 1, curY + 1].tileID == 0)
            {
                curX--;
            }
            else
            {
                forceDirectionChange = true;
                tileArray[curX, curY].transform.position = new Vector2(tileArray[curX, curY].transform.position.x, tileArray[curX, curY].transform.position.y);
                // mapVisuals[curX, curY].transform.position = new Vector3(
                //             mapVisuals[curX, curY].transform.position.x,
                //             mapVisuals[curX, curY].gameObject.transform.position.y - pathOffset);
            }
        }
        else if (curDirection != CurrentDirection.DOWN)
        {
            forceDirectionChange = true;
            tileArray[curX, curY].transform.position = new Vector2(tileArray[curX, curY].transform.position.x, tileArray[curX, curY].transform.position.y);
            // mapVisuals[curX, curY].transform.position = new Vector3(
            //             mapVisuals[curX, curY].transform.position.x,
            //             mapVisuals[curX, curY].gameObject.transform.position.y - pathOffset);
        }
    }

    private void ChooseDirection()
    {
        if (currentCount < 3 && !forceDirectionChange)
        {
            currentCount++;
        }
        else
        {
            bool chanceToChange = Mathf.FloorToInt(UnityEngine.Random.value * 1.99f) == 0;

            if (chanceToChange || forceDirectionChange || currentCount > 7)
            {
                currentCount = 0;
                forceDirectionChange = false;
                ChangeDirection();
            }

            currentCount++;
        }
    }

    private void ChangeDirection()
    {
        int dirValue = Mathf.FloorToInt(UnityEngine.Random.value * 2.99f);

        if (dirValue == 0 && curDirection == CurrentDirection.LEFT && curY - 1 > 0
        || dirValue == 0 && curDirection == CurrentDirection.RIGHT && curY + 1 < mapHeight - 1)
        {
            if (curX - 1 >= 0)
            {
                if (tileArray[curX - 1, curY].tileID == 0 &&
                tileArray[curX + 1, curY + 1].tileID == 0 &&
                tileArray[curX - 1, curY + 1].tileID == 0)
                {
                    GoUp();
                    return;
                }
            }
        }

        if (curDirection == CurrentDirection.LEFT)
        {
            UpdateMap(curX, curY, leftDown);
        }
        else if (curDirection == CurrentDirection.RIGHT)
        {
            UpdateMap(curX, curY, rightDown);
        }

        if (curDirection == CurrentDirection.LEFT || curDirection == CurrentDirection.RIGHT)
        {
            curX++;
            spriteToUse = downPath;
            curDirection = CurrentDirection.DOWN;
            return;
        }

        if (curY - 1 > 0 && curY + 1 < mapHeight - 1 || continueLeft || continueRight)
        {
            if (dirValue == 1 && !continueRight || continueLeft)
            {
                if (tileArray[curX, curY - 1].tileID == 0)
                {
                    if (continueLeft)
                    {
                        spriteToUse = rightDown;
                        continueLeft = false;
                    }
                    else
                    {
                        spriteToUse = downLeft;
                    }
                    curDirection = CurrentDirection.LEFT;
                }
            }
            else
            {
                if (tileArray[curX, curY + 1].tileID == 0)
                {
                    if (continueRight)
                    {
                        continueRight = false;
                        spriteToUse = leftDown;
                    }
                    else
                    {
                        spriteToUse = downRight;
                    }
                    curDirection = CurrentDirection.RIGHT;
                }
            }
        }
        else if (curY - 1 > 0)
        {
            spriteToUse = downLeft;
            curDirection = CurrentDirection.LEFT;
        }
        else if (curY + 1 < mapHeight - 1)
        {
            spriteToUse = downRight;
            curDirection = CurrentDirection.RIGHT;
        }

        if (curDirection == CurrentDirection.LEFT)
        {
            GoLeft();
        }
        else if (curDirection == CurrentDirection.RIGHT)
        {
            GoRight();
        }
    }


    private void GoUp()
    {
        if (curDirection == CurrentDirection.LEFT)
        {
            UpdateMap(curX, curY, downRight);
            continueLeft = true;
        }
        else
        {
            UpdateMap(curX, curY, downLeft);
            continueRight = true;
        }
        curDirection = CurrentDirection.UP;
        curX--;
        spriteToUse = downPath;
    }

    private void GoLeft()
    {
        UpdateMap(curX, curY, spriteToUse);
        curY--;
        spriteToUse = leftRight;
    }

    private void GoRight()
    {
        UpdateMap(curX, curY, spriteToUse);
        curY++;
        spriteToUse = leftRight;
    }

    private void UpdateMap(int mapX, int mapY, Sprite spriteToUse)
    {
        tileArray[mapX, mapY].transform.position = new Vector2(tileArray[mapX, mapY].transform.position.x, tileArray[mapX, mapY].transform.position.y);
        tileArray[mapX, mapY].tileID = 1;
        tileArray[mapX, mapY].spriteRenderer.sprite = spriteToUse;
    }
}
