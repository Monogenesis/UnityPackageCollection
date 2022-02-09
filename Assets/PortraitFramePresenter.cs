using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class PortraitFramePresenter : VisualElement
{
    private PortraitFrameController _portraitController;
    private int _currentLevel;
    private List<FrameRotator> frames = new();

    public PortraitFrameController PortraitController
    {
        get => _portraitController;
        set => _portraitController = value;
    }

    public int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            _currentLevel = value;
            CreatePortrait();
        }
    }

    private class FrameRotator
    {
        public VisualElement _frame;
        public float _rotationSpeed;

        public FrameRotator(VisualElement frame, float rotationSpeed)
        {
            _frame = frame;
            _rotationSpeed = rotationSpeed;
        }
    }


    private void CreatePortrait()
    {
        if (_portraitController == null)
            return;

        Clear();
        List<Sprite> framePartsToDisplay = _portraitController.GetFrameSprites(_currentLevel);

        for (int i = 0; i < framePartsToDisplay.Count; i++)
        {
            Sprite framePart = framePartsToDisplay[i];
            VisualElement frameContainer = new VisualElement
            {
                name = $"{name}-{GetType().Name.ToLower()}-frame-{i}",
                style =
                {
                    position = new StyleEnum<Position>(Position.Absolute),
                    backgroundImage = new StyleBackground(Background.FromSprite(framePart)),
                    width = Length.Percent(100),
                    height = Length.Percent(100),
                }
            };

            frames.Add(new FrameRotator(frameContainer, i * PortraitFrameController.RotationSpeed));

            Add(frameContainer);
        }
    }

    [Preserve]
    public new class UxmlFactory : UxmlFactory<PortraitFramePresenter, UxmlTraits>
    {
    }
}