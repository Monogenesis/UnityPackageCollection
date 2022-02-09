using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PortraitFrameController : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private List<Texture2D> spriteSheets;
    [SerializeField] private int levelsPerSpriteSheet = 16;
    private List<Sprite> _sprites = new();
    public const float RotationSpeed = 0.25f;


    private void Awake()
    {
        for (int i = 0; i < spriteSheets.Count; i++)
        {
            _sprites.AddRange(Resources.LoadAll<Sprite>(spriteSheets[i].name));
        }

        _uiDocument.rootVisualElement.Query<PortraitFramePresenter>().ForEach(frame =>
        {
            frame.PortraitController = this;
            frame.CurrentLevel = 999;
        });


    }

    public List<Sprite> GetFrameSprites(int level)
    {
        List<Sprite> result = new();
        level = Mathf.Clamp(level, 1, levelsPerSpriteSheet * spriteSheets.Count);
        int levelIndex = level - 1;
        int spriteSheetIndex = Mathf.FloorToInt(levelIndex / levelsPerSpriteSheet);

        for (int i = 0; i < spriteSheetIndex; i++)
        {
            result.Add(_sprites[(i * levelsPerSpriteSheet) + levelsPerSpriteSheet - 1]);
        }

        result.Add(_sprites[levelIndex]);

        return result;
    }


    public List<Sprite> GetSprites(int level)
    {
        Sprite sprite;
        Rect rect;
        List<Sprite> result = new();
        level = Mathf.Clamp(level, 1, levelsPerSpriteSheet * spriteSheets.Count);
        int levelIndex = level - 1;
        int spriteIndex = levelIndex % levelsPerSpriteSheet;
        int spriteSheetIndex = Mathf.FloorToInt(levelIndex / levelsPerSpriteSheet);


        for (int i = 0; i < spriteSheetIndex; i++)
        {
            rect = IndexToRectangle(levelsPerSpriteSheet - 1, 4, 4, 2048, 2048);
            sprite = Sprite.Create(spriteSheets[i], rect, new Vector2(.5f, .5f));
            result.Add(sprite);
        }

        rect = IndexToRectangle(spriteIndex, 4, 4, 2048, 2048);
        sprite = Sprite.Create(spriteSheets[spriteSheetIndex], rect, new Vector2(.5f, .5f));

        result.Add(sprite);

        return result;
    }

    private Rect IndexToRectangle(int index, int gridX, int gridY, int dimX, int dimY)
    {
        Rect result = new();
        int column = index % gridX;
        int row = Mathf.FloorToInt(index / (float) gridX);

        bool outOfBounce = row >= gridY;
        bool dimensionMismatch = ((dimX / (float) gridX) % 1f) != 0;
        dimensionMismatch |= ((dimY / (float) gridY) % 1f) != 0;

        if (outOfBounce || dimensionMismatch)
        {
            Debug.LogError("SpriteSheet errors!");
        }
        row = gridY - row - 1;

        int tileWidth = dimX / gridX;
        int tileHeight = dimY / gridY;
        result.x = column * tileWidth;
        result.y = row * tileHeight;
        result.width = tileWidth;
        result.height = tileHeight;

        return result;
    }
}