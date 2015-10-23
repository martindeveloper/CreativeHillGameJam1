using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SpriteTiler : MonoBehaviour {

    [System.Serializable]
    public enum TilerDirection
    {
        Right,
        Left,
        Up,
        Down
    }

    public Sprite TargetSprite;
    public TilerDirection Direction;
    public int TileCount;

    private Texture2D TileTexture;
    private List<GameObject> Tiles = new List<GameObject>();
    private Transform TilerTransform;
    private Vector3 TilerOrigin;
    private GameObject TileProtype;

    public void OnValidate()
    {
        if(TargetSprite == null)
        {
            return;
        }

        if (Tiles.Count != 0)
        {
            RemoveTiles();
        }

        Initialize();

        SpawnTiles();
    }

    private void Initialize()
    {
        TileTexture = TargetSprite.texture;
        TilerTransform = GetComponent<Transform>();
        TilerOrigin = TilerTransform.position;

        TileProtype = new GameObject();
        TileProtype.name = "Tile";
        TileProtype.AddComponent<SpriteRenderer>();
        TileProtype.GetComponent<Transform>().parent = TilerTransform;
        TileProtype.SetActive(false);
        TileProtype.hideFlags = HideFlags.HideInHierarchy;

        gameObject.isStatic = true;
    }

    private void SpawnTiles()
    {
        float textureWidth = TileTexture.width / TargetSprite.pixelsPerUnit;

        for (int i = 0; i < TileCount; i++)
        {
            float padding = i * textureWidth;

            Vector3 paddingVector = Vector3.zero;

            switch(Direction)
            {
                case TilerDirection.Right:
                    paddingVector = new Vector3(padding, 0.0f);
                    break;
                case TilerDirection.Left:
                    paddingVector = new Vector3(-padding, 0.0f);
                    break;
                case TilerDirection.Up:
                    paddingVector = new Vector3(0.0f, padding);
                    break;
                case TilerDirection.Down:
                    paddingVector = new Vector3(0.0f, -padding);
                    break;
            }

            Vector3 position = TilerOrigin + paddingVector;

            GameObject tile = GameObject.Instantiate(TileProtype, position, Quaternion.identity) as GameObject;

            if (tile != null)
            {
                tile.GetComponent<Transform>().parent = TilerTransform;
                tile.GetComponent<SpriteRenderer>().sprite = TargetSprite;
                tile.SetActive(true);
                tile.isStatic = true;

                Tiles.Add(tile);
            }
        }
    }

    private void RemoveTiles()
    {
        foreach (GameObject tile in Tiles)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyImmediate(tile);
            };
        }
    }

    public void OnDestroy()
    {
        RemoveTiles();
    }
}
