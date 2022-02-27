using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class PortraitFramePresenter : VisualElement
{
    private List<FrameRotator> frameRotators;
    private bool _stopped;
    private bool _endlessLoop;
    private bool _onlyAnimateWhenFrameRests;


    public bool EndlessLoop
    {
        get => _endlessLoop;
        set => _endlessLoop = value;
    }

    public bool OnlyAnimateWhenFrameRests
    {
        get => _onlyAnimateWhenFrameRests;
        set => _onlyAnimateWhenFrameRests = value;
    }

    private class FrameRotator
    {
        public VisualElement Frame;
        public readonly float RotationSpeed;
        public float RotationProgress;
        public bool IsRotating;
        public readonly bool TurnRight;

        public FrameRotator(VisualElement frame, float rotationSpeed, bool turnRight)
        {
            Frame = frame;
            RotationSpeed = rotationSpeed;
            TurnRight = turnRight;
        }

        public void Reset()
        {
            RotationProgress = 0f;
            IsRotating = false;
        }
    }

    public PortraitFramePresenter()
    {
        frameRotators = new List<FrameRotator>();
        RegisterCallback<PointerOverEvent>(OnPointerOver);
    }

    private void OnPointerOver(PointerOverEvent evt)
    {
        StartAnimation();
    }

    public void StartAnimation()
    {
        if (_onlyAnimateWhenFrameRests && !AnimationsHaveStopped())
            return;

        for (int i = 0; i < frameRotators.Count; i++)
        {
            FrameRotator frameRotator = frameRotators[i];

            if (!frameRotator.IsRotating)
            {
                StartFrameRotation(frameRotator);
            }
        }
    }

    public void StopAndResetAnimation()
    {
        _stopped = true;
        for (int i = 0; i < frameRotators.Count; i++)
        {
            frameRotators[i].Reset();
        }

        UnregisterCallback<PointerOverEvent>(OnPointerOver);
    }

    public void EnableAnimationTrigger()
    {
        _stopped = false;
        RegisterCallback<PointerOverEvent>(OnPointerOver);
    }

    public void DisableAnimationTrigger()
    {
        _stopped = true;
        UnregisterCallback<PointerOverEvent>(OnPointerOver);
    }
    
    public void CreatePortrait(List<Sprite> framePartsToDisplay, float rotationSpeed)
    {
        bool turnRight = true;

        Clear();
        frameRotators.Clear();

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
                    unityBackgroundScaleMode = new StyleEnum<ScaleMode>(ScaleMode.ScaleToFit),
                    width = Length.Percent(100),
                    height = Length.Percent(100),
                }
            };

            frameRotators.Add(new FrameRotator(frameContainer, (framePartsToDisplay.Count - i + 1) * rotationSpeed,
                turnRight));
            turnRight = !turnRight;

            Add(frameContainer);
        }
    }

    public bool AnimationsHaveStopped()
    {
        return frameRotators.All(rotator => !rotator.IsRotating);
    }

    private void StartFrameRotation(FrameRotator frameRotator)
    {
        frameRotator.IsRotating = true;
        frameRotator.Frame.schedule.Execute(() =>
        {
            if (frameRotator.TurnRight)
            {
                frameRotator.RotationProgress += frameRotator.RotationSpeed * Time.deltaTime;
            }
            else
            {
                frameRotator.RotationProgress -= frameRotator.RotationSpeed * Time.deltaTime;
            }

            frameRotator.Frame.style.rotate = new Rotate(frameRotator.RotationProgress);
        }).Until(() =>
        {
            if (_endlessLoop && !_stopped)
            {
                return false;
            }

            if (_stopped || frameRotator.RotationProgress is >= 360 or <= -360)
            {
                frameRotator.Reset();
                return true;
            }

            return false;
        });
    }

    #region UXML

    [Preserve]
    public new class UxmlFactory : UxmlFactory<PortraitFramePresenter, UxmlTraits>
    {
    }
    
    
    [Preserve]
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private UxmlBoolAttributeDescription m_endlessLoop =
            new UxmlBoolAttributeDescription
            {
                name = "endlessLoop", defaultValue = false
            };       
        
        private UxmlBoolAttributeDescription m_onlyAnimateWhenFrameRests =
            new UxmlBoolAttributeDescription
            {
                name = "only-animate-when-frame-rests", defaultValue = false
            };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            PortraitFramePresenter ate = ve as PortraitFramePresenter;

            ate.EndlessLoop = m_endlessLoop.GetValueFromBag(bag, cc);
            ate.OnlyAnimateWhenFrameRests = m_onlyAnimateWhenFrameRests.GetValueFromBag(bag, cc);
        }
    }

    #endregion


}