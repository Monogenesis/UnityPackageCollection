using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace EditorAugmentation.SimpleUIDocumentParser
{
    [Serializable]
    public class SimpleUIInteractionMediator : MonoBehaviour
    {
        [SerializeField] private UIDocument menuUIDocument;

        #region InteractionLists

        [SerializeField] private List<UIEventHandle<object>> buttonInteractions = new List<UIEventHandle<object>>();

        [SerializeField] private List<UIEventHandle<ChangeEvent<string>>> dropdownInteractions =
            new List<UIEventHandle<ChangeEvent<string>>>();

        [SerializeField] private List<UIEventHandle<ChangeEvent<string>>> textFieldInteractions =
            new List<UIEventHandle<ChangeEvent<string>>>();

        [SerializeField] private List<UIEventHandle<ChangeEvent<float>>> floatSliderInteractions =
            new List<UIEventHandle<ChangeEvent<float>>>();

        [SerializeField] private List<UIEventHandle<ChangeEvent<int>>> intSliderInteractions =
            new List<UIEventHandle<ChangeEvent<int>>>();

        [SerializeField] private List<UIEventHandle<ChangeEvent<Vector2>>> minMaxSliderInteractions =
            new List<UIEventHandle<ChangeEvent<Vector2>>>();

        [SerializeField] private List<UIEventHandle<ChangeEvent<bool>>> radioButtonInteractions =
            new List<UIEventHandle<ChangeEvent<bool>>>();

        [SerializeField] private List<UIEventHandle<ChangeEvent<int>>> radioButtonGroupInteractions =
            new List<UIEventHandle<ChangeEvent<int>>>();

        [SerializeField] private List<UIEventHandle<ChangeEvent<bool>>>
            toggleInteractions = new List<UIEventHandle<ChangeEvent<bool>>>();

        [SerializeField] private List<UIEventHandle<ChangeEvent<bool>>>
            foldoutInteractions = new List<UIEventHandle<ChangeEvent<bool>>>();

        #endregion

        #region Properties

        public List<UIEventHandle<object>> ButtonInteractions => buttonInteractions;
        public List<UIEventHandle<ChangeEvent<bool>>> ToggleInteractions => toggleInteractions;
        public List<UIEventHandle<ChangeEvent<string>>> DropdownInteractions => dropdownInteractions;
        public List<UIEventHandle<ChangeEvent<string>>> TextFieldInteractions => textFieldInteractions;
        public List<UIEventHandle<ChangeEvent<float>>> FloatSliderInteractions => floatSliderInteractions;
        public List<UIEventHandle<ChangeEvent<int>>> INTSliderInteractions => intSliderInteractions;
        public List<UIEventHandle<ChangeEvent<Vector2>>> MinMaxSliderInteractions => minMaxSliderInteractions;
        public List<UIEventHandle<ChangeEvent<bool>>> RadioButtonInteractions => radioButtonInteractions;
        public List<UIEventHandle<ChangeEvent<int>>> RadioButtonGroupInteractions => radioButtonGroupInteractions;
        public List<UIEventHandle<ChangeEvent<bool>>> FoldoutInteractions => foldoutInteractions;

        #endregion

        private VisualElement _menuRoot;

        // private  List<string> _duplicateNameReferenceTypeNames = new List<string>();
        private readonly HashSet<string> _duplicateNameReferenceTypeNames = new();
        public HashSet<string> DuplicateNameReferenceTypeNames => _duplicateNameReferenceTypeNames;

        private readonly HashSet<string> _missingEventReferenceTypeNames = new();
        public HashSet<string> MissingEventReferenceTypeNames => _missingEventReferenceTypeNames;

        private readonly HashSet<string> _emptyNameReferenceTypeNames = new();
        public HashSet<string> EmptyNameReferenceTypeNames => _emptyNameReferenceTypeNames;

        public Dictionary<string, UIEventHandle<object>> listOfEvents = new();
    

        private void OnValidate()
        {
            HookUnityEvents();
        }

        private void HookUnityEvents()
        {
            _duplicateNameReferenceTypeNames.Clear();
            _emptyNameReferenceTypeNames.Clear();
            menuUIDocument ??= GetComponent<UIDocument>();
            _menuRoot = menuUIDocument != null ? menuUIDocument.rootVisualElement : null;

            if (_menuRoot is not null)
            {
                buttonInteractions = LoadElements<Button>(_menuRoot, buttonInteractions);

                dropdownInteractions =
                    LoadElements<DropdownField, ChangeEvent<string>, string>(_menuRoot, dropdownInteractions);

                textFieldInteractions =
                    LoadElements<TextField, ChangeEvent<string>, string>(_menuRoot, textFieldInteractions);

                floatSliderInteractions =
                    LoadElements<Slider, ChangeEvent<float>, float>(_menuRoot, floatSliderInteractions);

                intSliderInteractions =
                    LoadElements<SliderInt, ChangeEvent<int>, int>(_menuRoot, intSliderInteractions);

                radioButtonInteractions =
                    LoadElements<RadioButton, ChangeEvent<bool>, bool>(_menuRoot, radioButtonInteractions);

                radioButtonGroupInteractions =
                    LoadElements<RadioButtonGroup, ChangeEvent<int>, int>(_menuRoot, radioButtonGroupInteractions);

                toggleInteractions = LoadElements<Toggle, ChangeEvent<bool>, bool>(_menuRoot, toggleInteractions);

                minMaxSliderInteractions =
                    LoadElements<MinMaxSlider, ChangeEvent<Vector2>, Vector2>(_menuRoot, minMaxSliderInteractions);

                foldoutInteractions = LoadElements<Foldout, ChangeEvent<bool>, bool>(_menuRoot, foldoutInteractions);
            }
        }
    
        public void OnRefreshButtonPressed()
        {
            HookUnityEvents();
        }

        private void Awake()
        {
            HookUnityEvents();
        }

        private List<UIEventHandle<TEventReturnType>> LoadElements<TElementType, TEventReturnType, TEventType>(
            VisualElement root,
            List<UIEventHandle<TEventReturnType>> interactionList)
            where TElementType : VisualElement, INotifyValueChanged<TEventType>
            where TEventReturnType : ChangeEvent<TEventType>
        {
            var elementList = root.Query<TElementType>().ToList();
            elementList.ForEach(
                type =>
                {
                    bool hasDuplicate = elementList.GroupBy(x => x.name).Any(g => g.Count() > 1);
                    if (hasDuplicate)
                    {
                        _duplicateNameReferenceTypeNames.Add(typeof(TElementType).Name);
                    }

                    bool nameEmpty = elementList.Any(x => x.name == "");
                    if (nameEmpty)
                    {
                        _emptyNameReferenceTypeNames.Add(typeof(TElementType).Name);
                    }

                    if (!hasDuplicate || nameEmpty)
                    {
                        UIEventHandle<TEventReturnType> handle = new UIEventHandle<TEventReturnType>(type.name,
                            $"ChangeEvent<{typeof(TEventReturnType).GetGenericArguments()[0].Name}>");
                        interactionList.Add(handle);
                        type.RegisterValueChangedCallback(evt =>
                        {
                            var result = interactionList.First(ele => ele.Name == type.name);
                            result.InteractionEvent.Invoke((TEventReturnType) evt);
                        });
                    }
                });

            return FilterObsoleteElements<TElementType, TEventReturnType>(interactionList,
                new List<VisualElement>(elementList.ToList()));
        }

        private List<UIEventHandle<object>> LoadElements<TElementType>(VisualElement root,
            List<UIEventHandle<object>> interactionList) where TElementType : Button
        {
            var elementList = root.Query<TElementType>().ToList();
            elementList.ForEach(
                type =>
                {
                    bool hasDuplicate = elementList.GroupBy(x => x.name).Any(g => g.Count() > 1);
                    hasDuplicate |= interactionList.GroupBy(x => x.Name).Any(g => g.Count() > 1);
                    if (hasDuplicate)
                    {
                        _duplicateNameReferenceTypeNames.Add(typeof(TElementType).Name);
                    }

                    bool nameEmpty = elementList.Any(x => x.name == "");
                    if (nameEmpty)
                    {
                        _emptyNameReferenceTypeNames.Add(typeof(TElementType).Name);
                    }

                    if (!hasDuplicate || nameEmpty)
                    {
                        UIEventHandle<object> handle = new UIEventHandle<object>(type.name, "None");
                        interactionList.Add(handle);
                        type.clicked += () =>
                        {
                            interactionList.First(ele => ele.Name == type.name).InteractionEvent.Invoke(null);
                        };
                    }
                });

            return FilterObsoleteElements<Button, object>(interactionList,
                new List<VisualElement>(elementList.ToList()));
        }


        private List<UIEventHandle<TEventType>> FilterObsoleteElements<TElementType, TEventType>(
            List<UIEventHandle<TEventType>> list,
            List<VisualElement> elements)
        {
            // Remove old elements
            var uiEventHandles = list.GroupBy(handle => handle.Name).Select(handles => handles.First()).ToList();
            uiEventHandles.RemoveAll(handle => elements.ToList().All(type => type.name != handle.Name));

            return uiEventHandles.ToList();
        }

        [Serializable]
        public class UIEventHandle<T>
        {
            [SerializeField] private string name;

            [SerializeField] private string eventParameterType;

            [SerializeField] private UnityEvent<T> interactionEvent;

            public string Name
            {
                get => name;
                set => name = value;
            }

            public UnityEvent<T> InteractionEvent
            {
                get => interactionEvent;
                set => interactionEvent = value;
            }
        

            public UIEventHandle(string name, string eventTypeName)
            {
                this.name = name;
                eventParameterType = eventTypeName;
                interactionEvent = null;
            }
        }
    }
}