using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EditorAugmentation.CustomAttributes;
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

        [SerializeField] private List<UIEventHandle<ChangeEvent<float>>> sliderInteractions =
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
        public List<UIEventHandle<ChangeEvent<float>>> SliderInteractions => sliderInteractions;
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

                sliderInteractions =
                    LoadElements<Slider, ChangeEvent<float>, float>(_menuRoot, sliderInteractions);

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

            buttonInteractions.ForEach(eventHandle =>
            {
                _menuRoot.Query<Button>(eventHandle.Name).ForEach(ele =>
                    ele.clicked += () => eventHandle.InteractionEvent.Invoke(null));
            });            
            
            toggleInteractions.ForEach(eventHandle =>
            {
                _menuRoot.Query<Toggle>(eventHandle.Name).ForEach(ele =>
                    ele.RegisterValueChangedCallback(eventHandle.InteractionEvent.Invoke));
            });

            dropdownInteractions.ForEach(eventHandle =>
            {
                _menuRoot.Query<DropdownField>(eventHandle.Name).ForEach(ele =>
                    ele.RegisterValueChangedCallback(eventHandle.InteractionEvent.Invoke));
            });

            textFieldInteractions.ForEach(eventHandle =>
            {
                _menuRoot.Query<TextField>(eventHandle.Name).ForEach(ele =>
                    ele.RegisterValueChangedCallback(eventHandle.InteractionEvent.Invoke));
            });

            sliderInteractions.ForEach(eventHandle =>
            {
                _menuRoot.Query<Slider>(eventHandle.Name).ForEach(ele =>
                    ele.RegisterValueChangedCallback(eventHandle.InteractionEvent.Invoke));
            });

            intSliderInteractions.ForEach(eventHandle =>
            {
                _menuRoot.Query<SliderInt>(eventHandle.Name).ForEach(ele =>
                    ele.RegisterValueChangedCallback(eventHandle.InteractionEvent.Invoke));
            });

            minMaxSliderInteractions.ForEach(eventHandle =>
            {
                _menuRoot.Query<MinMaxSlider>(eventHandle.Name).ForEach(ele =>
                    ele.RegisterValueChangedCallback(eventHandle.InteractionEvent.Invoke));
            });

            radioButtonInteractions.ForEach(eventHandle =>
            {
                _menuRoot.Query<RadioButton>(eventHandle.Name).ForEach(ele =>
                    ele.RegisterValueChangedCallback(eventHandle.InteractionEvent.Invoke));
            });

            radioButtonGroupInteractions.ForEach(eventHandle =>
            {
                _menuRoot.Query<RadioButtonGroup>(eventHandle.Name).ForEach(ele =>
                    ele.RegisterValueChangedCallback(eventHandle.InteractionEvent.Invoke));
            });

            foldoutInteractions.ForEach(eventHandle =>
            {
                _menuRoot.Query<Foldout>(eventHandle.Name).ForEach(ele =>
                    ele.RegisterValueChangedCallback(eventHandle.InteractionEvent.Invoke));
            });
        }

        private List<UIEventHandle<TEventReturnType>> LoadElements<TElementType, TEventReturnType, TEventType>(
            VisualElement root,
            List<UIEventHandle<TEventReturnType>> interactionList)
            where TElementType : VisualElement, INotifyValueChanged<TEventType>
            where TEventReturnType : ChangeEvent<TEventType>
        {
            List<UIEventHandle<TEventReturnType>> filteredList = new();
            TElementType element;
            int counter = 0;
            bool emptyName;

            var elementList = root.Query<TElementType>().ToList();
            if (elementList.GroupBy(x => x.name).Any(g => g.Count() > 1))
            {
                _duplicateNameReferenceTypeNames.Add(typeof(TElementType).Name);
            }

            if (elementList.Any(x => x.name == ""))
            {
                _emptyNameReferenceTypeNames.Add(typeof(TElementType).Name);
            }

            for (int i = 0; i < elementList.Count; i++)
            {
                element = elementList[i];
                counter = 0;
                emptyName = element.name == "";

                for (int j = 0; j < interactionList.Count; j++)
                {
                    if (element.name == interactionList[j].Name)
                    {
                        counter++;
                    }
                }

                if (counter == 0 && !emptyName)
                {
                    UIEventHandle<TEventReturnType> handle = new UIEventHandle<TEventReturnType>(element.name,
                        $"ChangeEvent<{typeof(TEventReturnType).GetGenericArguments()[0].Name}>");
                    interactionList.Add(handle);
                    element.RegisterValueChangedCallback(evt =>
                    {
                        var result = interactionList.First(ele => ele.Name == element.name);
                        result.InteractionEvent.Invoke((TEventReturnType) evt);
                    });
                }
            }

            for (int i = 0; i < interactionList.Count; i++)
            {
                var interaction = interactionList[i];
                counter = 0;

                for (int j = 0; j < elementList.Count; j++)
                {
                    if (elementList[j].name != interactionList[i].Name)
                    {
                        counter++;
                    }
                }

                if (counter < elementList.Count)
                {
                    filteredList.Add(interaction);
                }
            }

            if (filteredList.Count > 0)
            {
                filteredList = filteredList.GroupBy(x => x.Name).Select(y => y.FirstOrDefault()).ToList();
            }

            return filteredList;
        }

        private List<UIEventHandle<object>> LoadElements<TElementType>(VisualElement root,
            List<UIEventHandle<object>> interactionList) where TElementType : Button
        {
            List<UIEventHandle<object>> filteredList = new();
            TElementType element;
            int counter = 0;
            bool emptyName;

            var elementList = root.Query<TElementType>().ToList();
            if (elementList.GroupBy(x => x.name).Any(g => g.Count() > 1))
            {
                _duplicateNameReferenceTypeNames.Add(typeof(TElementType).Name);
            }

            if (elementList.Any(x => x.name == ""))
            {
                _emptyNameReferenceTypeNames.Add(typeof(TElementType).Name);
            }

            for (int i = 0; i < elementList.Count; i++)
            {
                element = elementList[i];
                counter = 0;
                emptyName = element.name == "";

                for (int j = 0; j < interactionList.Count; j++)
                {
                    if (element.name == interactionList[j].Name)
                    {
                        counter++;
                    }
                }

                if (counter == 0 && !emptyName)
                {
                    UIEventHandle<object> handle = new UIEventHandle<object>(element.name, "None");
                    interactionList.Add(handle);
                    element.clicked += () =>
                    {
                        interactionList.First(ele => ele.Name == element.name).InteractionEvent.Invoke(null);
                    };
                }
            }

            for (int i = 0; i < interactionList.Count; i++)
            {
                var interaction = interactionList[i];
                counter = 0;

                for (int j = 0; j < elementList.Count; j++)
                {
                    if (elementList[j].name != interactionList[i].Name)
                    {
                        counter++;
                    }
                }

                if (counter < elementList.Count)
                {
                    filteredList.Add(interaction);
                }
            }

            if (filteredList.Count > 0)
            {
                filteredList = filteredList.GroupBy(x => x.Name).Select(y => y.FirstOrDefault()).ToList();
            }

            return filteredList;
        }

        [Serializable]
        public class UIEventHandle<T>
        {
            [SerializeField, ReadOnly] private string name;

            [SerializeField, ReadOnly] private string eventParameterType;

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