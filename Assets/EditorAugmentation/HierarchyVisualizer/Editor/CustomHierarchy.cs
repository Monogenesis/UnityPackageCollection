using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorAugmentation.HierarchyVisualizer
{
    [InitializeOnLoad]
    public class CustomHierarchy : MonoBehaviour
    {
        // [SerializeField] private static Color standardBgColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        [SerializeField] private static Color standardBgColor = new Color(0, 0, 0, 1f);
        [SerializeField] private static Color selectedBgColor = new Color(0.93f, 0.68f, 0, 1);
        [SerializeField] private static Color disabledBgColor = new Color(0.6f, 0.6f, 0.6f, 1);
        [SerializeField] private static Color selectedDisabledBgColor = new Color(0.93f, 0.68f, 0, 0.7f);
        [SerializeField] private static Color standardFontColor = new Color(1, 1, 1, 1);

        // [SerializeField] private static Color standardFontColor = new Color(0.2f, 0.2f, 0.2f, 1);
        [SerializeField] private static Color disabledFontColor = new Color(1f, 1f, 1f, 0.7f);


        static CustomHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }


        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj != null)
            {
                var prefabType = PrefabUtility.GetPrefabAssetType(obj);
                if (prefabType == PrefabAssetType.NotAPrefab)
                {
                    Color bgColor = standardBgColor;
                    Color fontColor = standardFontColor;
                    GameObject gameObject = (GameObject) obj;
                    if (gameObject.name.StartsWith("#"))
                    {
                        bool isSelected = Selection.instanceIDs.Contains(instanceID);
                        bool isEnabled = gameObject.activeSelf;
                        bool selectedAndDisabled = isSelected && !isEnabled;
                        if (selectedAndDisabled)
                        {
                            bgColor = selectedDisabledBgColor;
                            fontColor = disabledFontColor;
                        }
                        else if (isSelected)
                        {
                            bgColor = selectedBgColor;
                        }
                        else if (!isEnabled)
                        {
                            bgColor = disabledBgColor;
                            fontColor = disabledFontColor;
                        }

                        Rect offsetRect = new Rect(selectionRect.position - new Vector2(-18, 0),
                            selectionRect.size * new Vector2(0.8f, 0.96f));
                        EditorGUI.DrawRect(offsetRect, bgColor);
                        EditorGUI.LabelField(offsetRect, obj.name, new GUIStyle()
                            {
                                normal = new GUIStyleState()
                                {
                                    textColor = fontColor,
                                },
                                fontStyle = FontStyle.Bold,
                                alignment = TextAnchor.MiddleCenter
                            }
                        );
                    }
                }
            }
        }
    }
}