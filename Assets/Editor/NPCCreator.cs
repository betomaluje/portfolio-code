using System.Collections.Generic;
using System.Linq;
using Base;
using BerserkPixel.Extensions;
using BerserkPixel.Extensions.Editor;
using BerserkPixel.StateMachine;
using NPCs;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UIElements;

public class NPCCreator : EditorWindow {
    private const string c_prefabFolder = "Assets/Prefabs/NPCs";
    private const string c_statesFolder = "Assets/Data/NPCs/States";
    private const string c_baseSpritesFolder = "Assets/Art/Sprites/Sunnyside_World_Assets/Characters/Human/IDLE";
    private const string c_spritesLirariesFolder = "Assets/Data/NPCs/Sprite Libraries";

    private const int c_minElementWidth = 100;
    private const int c_elementWidth = 160;
    private ObjectField _animationObject;
    private Image _previewImage;

    private List<State<NPCStateMachine>> _selectedStates;
    private SpriteLibraryAsset _spriteLibraryAsset;

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        var root = rootVisualElement;

        // Create a two-pane view with the left pane being fixed with
        var splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);

        // Add the view to the visual tree by adding it as a child to the root element
        root.Add(splitView);

        var selectedStatesPanel = new VisualElement
        {
            style =
            {
                position = Position.Absolute,
                top = 0,
                alignSelf = Align.FlexStart
            }
        };

        var statesMenu = CreateStatesPanel(selectedStatesPanel);
        // A TwoPaneSplitView always needs exactly two child elements
        splitView.Add(statesMenu);

        var rightPane = new VisualElement
        {
            style =
            {
                paddingLeft = 8,
                paddingRight = 8,
                paddingBottom = 8,
                paddingTop = 8
            }
        };
        splitView.Add(rightPane);

        // we add the selected states panel
        rightPane.Add(selectedStatesPanel);

        _previewImage = new Image();

        SetImageStyle(ref _previewImage);

        var baseSprite = AssetsExt.FindObjectOfType<Sprite>("base", c_baseSpritesFolder);

        var baseImage = new Image
        {
            sprite = baseSprite
        };

        SetImageStyle(ref baseImage);

        rightPane.Add(baseImage);
        baseImage.Add(_previewImage);
        rightPane.Add(AddHorizontalDivider());

        var spriteLabel = new Label("Sprite Assets");
        rightPane.Add(spriteLabel);

        var spriteLibrariesPane = LoadSpriteLibraries();
        rightPane.Add(spriteLibrariesPane);

        rightPane.Add(AddHorizontalDivider());

        var animationPane = new VisualElement();
        rightPane.Add(animationPane);

        var animationConfigLabel = new Label("Animation Config");
        animationPane.Add(animationConfigLabel);
        _animationObject = new ObjectField
        {
            objectType = typeof(AnimationConfig),
            allowSceneObjects = false,
            style =
            {
                marginTop = 2,
                minWidth = 250,
                width = 250,
                marginBottom = 8
            }
        };
        animationPane.Add(_animationObject);

        var createButton = AddCreateButton(_animationObject);
        rightPane.Add(createButton);
    }

    private VisualElement CreateStatesPanel(VisualElement selectedStatesPanel) {
        var leftMenu = new VisualElement
        {
            style =
            {
                paddingLeft = 8,
                paddingRight = 8,
                paddingBottom = 8,
                paddingTop = 8
            }
        };

        var statesLabel = new Label("States Assets");
        leftMenu.Add(statesLabel);

        leftMenu.Add(AddHorizontalDivider());

        var leftPane = LoadStates(selectedStatesPanel);
        leftMenu.Add(leftPane);

        return leftMenu;
    }

    [MenuItem(Shortcuts.ToolsNPCCreator, false, -100)]
    public static void ShowEditor() {
        var wnd = GetWindow<NPCCreator>();
        wnd.titleContent = new GUIContent("NPC Creator");
        wnd.minSize = new Vector2(470, 500);
        wnd.maxSize = new Vector2(470, 500);
    }

    private VisualElement AddHorizontalDivider() {
        var divider = new VisualElement
        {
            style =
            {
                position = Position.Relative,
                display = DisplayStyle.Flex,
                height = 1,
                maxHeight = 1,
                marginBottom = 4,
                marginTop = 4,
                color = Color.white,
                backgroundColor = Color.white,
                left = 0,
                right = 0
            }
        };

        return divider;
    }

    private void SetImageStyle(ref Image image) {
        var style = image.style;
        style.width = 200;
        style.height = 200;
        style.maxWidth = 200;
        style.maxHeight = 200;
        style.position = Position.Relative;
        style.alignSelf = Align.FlexEnd;

        image.scaleMode = ScaleMode.ScaleToFit;
    }

    private Button AddCreateButton(ObjectField animationObject) {
        var createButton = new Button
        {
            text = "Create NPC"
        };
        createButton.RegisterCallback<ClickEvent>(e => { CreateNPC(animationObject.value as AnimationConfig); });

        createButton.style.position = Position.Relative;
        createButton.style.height = 24;
        createButton.style.alignSelf = Align.FlexEnd;

        return createButton;
    }

    private void CreateNPC(AnimationConfig animationObject) {
        if (animationObject == null) {
            Debug.LogError("There's no Animation Config set. Please use one.");
            return;
        }

        if (_spriteLibraryAsset == null) {
            Debug.LogError($"Could not find any Sprite Library Asset at {c_spritesLirariesFolder}");
            return;
        }

        var prefab = AssetsExt.FindObjectOfType<GameObject>("NPC", c_prefabFolder);
        var container = Instantiate(prefab);
        container.name = "NPC";

        if (container.TryGetComponent(out NPCStateMachine script)) {
            script.SetStates(_selectedStates);
            script.animationConfig = animationObject.Clone();
        }

        var spriteLibrary = container.GetComponentInChildren<SpriteLibrary>();
        spriteLibrary.spriteLibraryAsset = _spriteLibraryAsset;

        // we move the script component to the top of the hierarchy
        // while (UnityEditorInternal.ComponentUtility.MoveComponentUp(script)) { }

        Selection.activeObject = container;
    }

    #region States

    private ListView LoadStates(VisualElement statesPane) {
        var availableStates =
            AssetsExt.FindAllScriptableObjectsOfType<State<NPCStateMachine>>("state", c_statesFolder);

        if (availableStates.Count <= 0) {
            return new ListView();
        }

        _selectedStates = new List<State<NPCStateMachine>>();

        var statesTitle = new Label("States to Add:");
        statesPane.Add(statesTitle);

        var leftPane = new ListView
        {
            style =
            {
                marginTop = 8,
                marginBottom = 8
            },
            selectionType = SelectionType.Single,
            makeItem = MakeStateItem,
            bindItem = BindItem,
            itemsSource = availableStates
        };

        return leftPane;

        void BindItem(VisualElement item, int index) {
            var b = (Button) item;
            b.text = availableStates[index].name;
            b.RegisterCallback<ClickEvent>(e => OnStateSelected(statesPane, availableStates[index]));
        }

        VisualElement MakeStateItem() {
            var button = new Button
            {
                style =
                {
                    position = Position.Relative,
                    height = 16,
                    flexGrow = 1,
                    alignSelf = Align.Center,
                    minWidth = c_minElementWidth,
                    width = c_elementWidth
                }
            };

            return button;
        }
    }

    private void OnStateSelected(VisualElement statesPane, State<NPCStateMachine> state) {
        var already = statesPane.Q<Label>(state.name);
        if (already == null) {
            var label = new Label
            {
                name = state.name,
                text = state.name
            };
            statesPane.Add(label);

            _selectedStates.Add(state);
        }
        else {
            statesPane.Remove(already);
            _selectedStates.Remove(state);
        }
    }

    #endregion

    #region Sprite Library

    private ListView LoadSpriteLibraries() {
        var spriteLibraries = AssetsExt.FindAllObjectsOfType<SpriteLibraryAsset>("NPC", c_spritesLirariesFolder);

        if (spriteLibraries.Count <= 0) {
            return new ListView();
        }

        OnSpriteLibrarySelected(spriteLibraries[0]);

        var spriteLibrariesPane = new ListView
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                flexShrink = 0
            },
            // Initialize the list view with all sprites' names
            selectionType = SelectionType.Single,
            makeItem = MakeSpriteLibraryItem,
            bindItem = BindItem,
            itemsSource = spriteLibraries
        };

        return spriteLibrariesPane;

        void BindItem(VisualElement item, int index) {
            var b = (Button) item;
            b.text = spriteLibraries[index].name;
            b.RegisterCallback<ClickEvent>(e => OnSpriteLibrarySelected(spriteLibraries[index]));
        }

        VisualElement MakeSpriteLibraryItem() {
            var button = new Button
            {
                style =
                {
                    position = Position.Relative,
                    height = 16,
                    flexGrow = 1,
                    alignSelf = Align.Center,
                    minWidth = c_minElementWidth,
                    width = c_elementWidth
                }
            };

            return button;
        }
    }

    private void OnSpriteLibrarySelected(SpriteLibraryAsset spriteLibrary) {
        _spriteLibraryAsset = spriteLibrary;

        var category = _spriteLibraryAsset.GetCategoryNames().FirstOrDefault();
        var label = _spriteLibraryAsset.GetCategoryLabelNames(category).FirstOrDefault();
        _previewImage.sprite = spriteLibrary.GetSprite(category, label);
    }

    #endregion
}