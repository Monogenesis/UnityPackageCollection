using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorAugmentation.SimpleUIDocumentParser.Editor
{
    [CustomEditor(typeof(SimpleUIInteractionMediator))]
    public class SimpleUIInteractionMediatorInspector : UnityEditor.Editor
    {
        private readonly HashSet<string> _missingEventReferenceTypeNames = new();

        private static readonly List<string> ListNames = new()
        {
            "buttonInteractions",
            "dropdownInteractions",
            "textFieldInteractions",
            "floatSliderInteractions",
            "intSliderInteractions",
            "minMaxSliderInteractions",
            "radioButtonInteractions",
            "radioButtonGroupInteractions",
            "toggleInteractions",
            "foldoutInteractions"
        };

        public override void OnInspectorGUI()
        {
            SerializedProperty property;
            _missingEventReferenceTypeNames.Clear();
            serializedObject.Update();
            SimpleUIInteractionMediator myScript = (SimpleUIInteractionMediator) target;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((SimpleUIInteractionMediator) target),
                typeof(SimpleUIInteractionMediator), false);


            EditorGUILayout.PropertyField(serializedObject.FindProperty("menuUIDocument"));

            if (GUILayout.Button("REFRESH"))
            {
                myScript.OnRefreshButtonPressed();
                Debug.Log("Finished Parsing The UIDocument!");
            }

            CheckForMissingReference<SimpleUIInteractionMediator.UIEventHandle<object>, object>(myScript.ButtonInteractions, ListNames[0]);

            CheckForMissingReference<SimpleUIInteractionMediator.UIEventHandle<ChangeEvent<string>>, ChangeEvent<string>>(
                myScript.DropdownInteractions, ListNames[1]);

            CheckForMissingReference<SimpleUIInteractionMediator.UIEventHandle<ChangeEvent<string>>, ChangeEvent<string>>(
                myScript.TextFieldInteractions, ListNames[2]);

            CheckForMissingReference<SimpleUIInteractionMediator.UIEventHandle<ChangeEvent<float>>, ChangeEvent<float>>(
                myScript.FloatSliderInteractions, ListNames[3]);

            CheckForMissingReference<SimpleUIInteractionMediator.UIEventHandle<ChangeEvent<int>>, ChangeEvent<int>>(
                myScript.INTSliderInteractions, ListNames[4]);

            CheckForMissingReference<SimpleUIInteractionMediator.UIEventHandle<ChangeEvent<Vector2>>, ChangeEvent<Vector2>>(
                myScript.MinMaxSliderInteractions, ListNames[5]);

            CheckForMissingReference<SimpleUIInteractionMediator.UIEventHandle<ChangeEvent<bool>>, ChangeEvent<bool>>(
                myScript.RadioButtonInteractions, ListNames[6]);

            CheckForMissingReference<SimpleUIInteractionMediator.UIEventHandle<ChangeEvent<int>>, ChangeEvent<int>>(
                myScript.RadioButtonGroupInteractions, ListNames[7]);

            CheckForMissingReference<SimpleUIInteractionMediator.UIEventHandle<ChangeEvent<bool>>, ChangeEvent<bool>>(
                myScript.ToggleInteractions, ListNames[8]);

            CheckForMissingReference<SimpleUIInteractionMediator.UIEventHandle<ChangeEvent<bool>>, ChangeEvent<bool>>(
                myScript.FoldoutInteractions, ListNames[9]);


            if (myScript.DuplicateNameReferenceTypeNames.Count > 0)
            {
                EditorGUILayout.HelpBox(
                    $"ACTION REQUIRED - DUPLICATE NAMES: {string.Join(", ", myScript.DuplicateNameReferenceTypeNames)}",
                    MessageType.Error);
            }

            if (myScript.EmptyNameReferenceTypeNames.Count > 0)
            {
                EditorGUILayout.HelpBox(
                    $"ACTION REQUIRED - EMPTY NAMES: {string.Join(", ", myScript.EmptyNameReferenceTypeNames)}",
                    MessageType.Error);
            }

            if (_missingEventReferenceTypeNames.Count > 0)
            {
                EditorGUILayout.HelpBox(
                    $"Missing Event References For: {string.Join(", ", _missingEventReferenceTypeNames)}",
                    MessageType.Warning);
            }


            foreach (var listName in ListNames)
            {
                property = serializedObject.FindProperty(listName);
                CheckForVisibility(property);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ForListDo<TListType, TDataType>(List<TListType> interactionList, string typeName, Action action)
            where TListType : SimpleUIInteractionMediator.UIEventHandle<TDataType>
        {
            action.Invoke();
        }

        private void CheckForMissingReference<TListType, TDataType>(List<TListType> interactionList, string typeName)
            where TListType : SimpleUIInteractionMediator.UIEventHandle<TDataType>
        {
            bool missingRefFound = false;
            for (int i = 0; i < interactionList.Count; i++)
            {
                var eventHandler = interactionList[i];
                if (eventHandler.InteractionEvent?.GetPersistentEventCount() < 1)
                {
                    missingRefFound = true;
                }
                else
                {
                    for (int j = 0; j < eventHandler.InteractionEvent?.GetPersistentEventCount(); j++)
                    {
                        if (eventHandler.InteractionEvent.GetPersistentMethodName(j).Equals(""))
                        {
                            missingRefFound = true;
                            break;
                        }
                    }
                }

                if (missingRefFound)
                {
                    _missingEventReferenceTypeNames.Add(typeName);
                    break;
                }
            }
        }

        private void CheckForVisibility(SerializedProperty property)
        {
            if (property.arraySize > 0)
            {
                if (EditorGUILayout.Foldout(true, property.name, true))
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(0).FindPropertyRelative("name"), true);
                    EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(0).FindPropertyRelative("eventParameterType"), true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(0).FindPropertyRelative("interactionEvent"), true);
                    EditorGUI.indentLevel--;
                }             

            }
        }
    }
}