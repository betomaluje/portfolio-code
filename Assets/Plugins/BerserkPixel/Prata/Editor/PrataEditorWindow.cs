using BerserkPixel.Prata.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BerserkPixel.Prata
{
    public class PrataEditorWindow : EditorWindow
    {
        private PrataGraphView graphView;
        private string _filename = "New Conversation";

        private TextField filenameTextField;
        private ObjectField _loadFileField;

        public void SetLoadedField(DialogContainer dialogue)
        {
            _loadFileField.value = dialogue;
        }

        [MenuItem(PrataConstants.MenuRoot + "/Graph View #p")]
        public static void Open()
        {
            GetWindow<PrataEditorWindow>("Dialog Graph");
        }

        public static PrataEditorWindow ExternalOpen()
        {
            return GetWindow<PrataEditorWindow>("Dialog Graph");
        }

        private void CreateGUI()
        {
            GraphSaveUtilities.GenerateFolders();
            GraphSaveUtilities.CreateFirstCharacter("Player");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddMiniMap();
            AddStyles();
            AddToolbar();
            AddSecondaryToolbar();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        private void OnDestroy()
        {
            AssetDatabase.SaveAssets();
        }

        #region Elements Addition

        private void AddGraphView()
        {
            graphView = new PrataGraphView();
            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }

        private void AddMiniMap()
        {
            var miniMap = new MiniMap { anchored = false, windowed = false };
            miniMap.SetPosition(new Rect(10, 55, 200, 140));
            graphView.Add(miniMap);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("PrataVariables");
        }

        private void AddToolbar()
        {
            var toolbar = new Toolbar();

            filenameTextField = PrataElementsUtilities.CreateTextField(
                "Filename:",
                "Filename:",
                evt => { _filename = evt.newValue; });
            filenameTextField.AddClasses(
                "prata-node_textfield",
                "prata-node_quote-textfield"
            );
            filenameTextField.SetValueWithoutNotify(_filename);
            filenameTextField.MarkDirtyRepaint();
            toolbar.Add(filenameTextField);

            var saveButton = PrataElementsUtilities.CreateButton("Save Data", SaveData);
            var clearButton = PrataElementsUtilities.CreateButton("Clear All", ClearData);

            toolbar.Add(saveButton);
            toolbar.Add(clearButton);

            rootVisualElement.Insert(1, toolbar);
        }

        private void AddSecondaryToolbar()
        {
            var toolbar = new Toolbar();

            _loadFileField = PrataElementsUtilities.CreateObjectField<DialogContainer>("Load Graph");
            _loadFileField.RegisterCallback<ChangeEvent<Object>>((evt) =>
            {
                if (evt.newValue == null)
                {
                    ResetTextfields();
                    return;
                }

                _filename = evt.newValue.name;
                filenameTextField.SetValueWithoutNotify(_filename);
                filenameTextField.MarkDirtyRepaint();

                if (_loadFileField.value is DialogContainer dialogContainer)
                {
                    LoadData(dialogContainer);
                }
            });

            toolbar.Add(_loadFileField);

            rootVisualElement.Insert(2, toolbar);
        }

        #endregion

        private void SaveData()
        {
            if (string.IsNullOrEmpty(_filename))
            {
                EditorUtility.DisplayDialog("Invalid filename", "Please enter a valid filename", "Ok");
                return;
            }

            var saveUtility = new GraphSaveUtilities(graphView);
            saveUtility.SaveGraph(_filename);
        }

        private void LoadData(DialogContainer dialogContainer)
        {
            if (string.IsNullOrEmpty(_filename))
            {
                EditorUtility.DisplayDialog("Invalid filename", "Please enter a valid filename", "Ok");
                return;
            }

            var loadUtility = new GraphSaveUtilities(graphView);
            loadUtility.LoadGraph(dialogContainer);
        }

        private void ClearData()
        {
            var choice = EditorUtility.DisplayDialogComplex(
                "Are you sure?",
                "This will clear everything. There's no turning back",
                "Yes",
                "Cancel",
                "");

            if (choice == 0)
            {
                var clearUtility = new GraphSaveUtilities(graphView);
                clearUtility.ClearAll();
                ResetTextfields();
            }
        }

        private void ResetTextfields()
        {
            _loadFileField.value = null;
            _filename = "New Conversation";
            filenameTextField.SetValueWithoutNotify(_filename);
            filenameTextField.MarkDirtyRepaint();
        }
    }
}