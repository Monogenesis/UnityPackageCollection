using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Test : MonoBehaviour
{
    public UIDocument document;

    private VisualElement _root;
    void Awake()
    {
        _root = document.rootVisualElement;

        // Create a list of data. In this case, numbers from 1 to 1000.
        const int itemCount = 10;
        var items = new List<string>(itemCount);
        for (int i = 1; i <= itemCount; i++)
            items.Add(i.ToString());

        // The "makeItem" function is called when the
        // ListView needs more items to render.
        Func<VisualElement> makeItem = () => new Label();

        // As the user scrolls through the list, the ListView object
        // recycles elements created by the "makeItem" function,
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list).
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];

        // Provide the list view with an explict height for every row
        // so it can calculate how many items to actually display
        const int itemHeight = 20;

        var listView = new ListView(items, itemHeight, makeItem, bindItem);

        listView.reorderMode = ListViewReorderMode.Animated;
        listView.selectionType = SelectionType.Multiple;

        listView.onItemsChosen += objects => Debug.Log(objects);
        listView.onSelectionChange += objects => Debug.Log(objects);

        listView.style.flexGrow = 1.0f;

        _root.Add(listView);



        _root.Q<Button>().clicked += () =>
        {

            items.Add("New Entry");
            listView.itemsSource = new List<string>(items);
            listView.hierarchy.Add(new Label());
            Debug.Log("Adding new Entry");
        };
    }


}