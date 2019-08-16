using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using System.Linq;
using Duel.PlayerSystems;
using Sirenix.Serialization;
using Duel.Combat;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;

namespace Duel.Editor
{
    [EditorTool("Frame Data Tool", typeof(PlayerSystems.PlayerCombat))]
    public class FrameDataTool : EditorTool
    {
        public enum ToolMode
        {
            Select,
            Add,
            Remove,
            Position,
            Rotation,
            Scale,
            Vector
        }

        #region Toolbar UI Variables

        [SerializeField]
        private Texture2D m_ToolIcon;

        private SceneView sceneView;
        private GUIContent m_IconContent;

        public override GUIContent toolbarIcon
        {
            get => m_IconContent;
        }

        #endregion Toolbar UI Variables

        #region Color Variables

        private static Color? hurtboxColor;
        private static Color? hitboxColor;
        private static Color? selectedBoxColor;
        private static Color? superArmorColor;
        private static Color? highlightedHandleColor;

        public static Color? HurtboxColor
        {
            get
            {
                if (hurtboxColor == null)
                {
                    if (!Utilities.TryParseHtmlStringToNullableColor(EditorPrefs.GetString("HurtboxColor"), out hurtboxColor))
                    {
                        Debug.LogWarning("Failed to load HurtboxColor");
                    }
                }
                return hurtboxColor;
            }

            set
            {
                if (hurtboxColor != value)
                    EditorPrefs.SetString("HurtboxColor", $"#{ColorUtility.ToHtmlStringRGBA((Color)value)}");
                hurtboxColor = value;
            }
        }

        public static Color? HitboxColor
        {
            get
            {
                if (hitboxColor == null)
                {
                    if (!Utilities.TryParseHtmlStringToNullableColor(EditorPrefs.GetString("HitboxColor"), out hitboxColor))
                    {
                        Debug.LogWarning("Failed to load HitboxColor");
                    }
                }
                return hitboxColor;
            }
            set
            {
                if (hitboxColor != value)
                    EditorPrefs.SetString("HitboxColor", $"#{ColorUtility.ToHtmlStringRGBA((Color)value)}");
                hitboxColor = value;
            }
        }

        public static Color? SelectedBoxColor
        {
            get
            {
                if (selectedBoxColor == null)
                {
                    if (!Utilities.TryParseHtmlStringToNullableColor(EditorPrefs.GetString("SelectedBoxColor"), out selectedBoxColor))
                    {
                        Debug.LogWarning("Failed to load SelectedBoxColor");
                    }
                }
                return selectedBoxColor;
            }
            set
            {
                if (selectedBoxColor != value)
                    EditorPrefs.SetString("SelectedBoxColor", $"#{ColorUtility.ToHtmlStringRGBA((Color)value)}");
                selectedBoxColor = value;
            }
        }

        public static Color? SuperArmorColor
        {
            get
            {
                if (superArmorColor == null)
                {
                    if (!Utilities.TryParseHtmlStringToNullableColor(EditorPrefs.GetString("SuperArmorColor"), out superArmorColor))
                    {
                        Debug.LogWarning("Failed to load SuperArmorColor");
                    }
                }
                return superArmorColor;
            }
            set
            {
                if (superArmorColor != value)
                    EditorPrefs.SetString("SuperArmorColor", $"#{ColorUtility.ToHtmlStringRGBA((Color)value)}");
                superArmorColor = value;
            }
        }

        public static Color? HighlightedHandleColor
        {
            get
            {
                if (highlightedHandleColor == null)
                {
                    if (!Utilities.TryParseHtmlStringToNullableColor(EditorPrefs.GetString("HighlightedHandleColor"), out highlightedHandleColor))
                    {
                        Debug.LogWarning("Failed to load HighlightedHandleColor");
                    }
                }
                return highlightedHandleColor;
            }
            set
            {
                if (highlightedHandleColor != value)
                    EditorPrefs.SetString("HighlightedHandleColor", $"#{ColorUtility.ToHtmlStringRGBA((Color)value)}");
                highlightedHandleColor = value;
            }
        }

        #endregion Color Variables

        #region Combat Data Variables

        private PlayerCombat targetedObject;
        private SerializedProperty attacks;

        private string[] animationNames = new string[0];

        private int selectedAnimationIndex;
        private float sampleTime;
        private int frameDataIndex;

        private CharacterAnimationRegistryObject TargetRegistry
        {
            get => targetedObject?.animationRegistryObject;
        }

        private Combat.AnimationInfo SelectedAnimation
        {
            get => TargetRegistry?[SelectedAnimationIndex];
        }

        private FrameData SelectedFrameData
        {
            get => FrameDataIndex >= 0 && FrameDataIndex < SelectedAnimation.frameData.Count ? SelectedAnimation?.frameData[FrameDataIndex] : null;
        }

        private int SelectedAnimationIndex
        {
            get => selectedAnimationIndex;
            set
            {
                if (selectedAnimationIndex != value)
                {
                    selectedAnimationIndex = value;
                    OnSelectedAnimationChange();
                }
                selectedAnimationIndex = value;
            }
        }

        public int FrameDataIndex
        {
            get => frameDataIndex;
            set
            {
                if (value != frameDataIndex)
                {
                    frameDataIndex = value;

                    OnFrameDataChange();
                }
                frameDataIndex = value;
            }
        }

        public float SampleTime
        {
            get => sampleTime;
            set
            {
                if (sampleTime != value)
                {
                    OnSampleTimeChange(value);
                }
                sampleTime = value;
            }
        }

        #endregion Combat Data Variables

        #region UI Variables

        private Rect uiArea;
        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private bool colorGroupOpen;
        private bool boxInfoOpen;
        private Vector2 boxInfoScrollPosition;
        private static bool enabled;

        private string[] BoxTypeNames = new string[2] { "Hit", "Hurt" };

        #endregion UI Variables

        #region Tool Functionality Variables

        private List<EditorBoxInfo> editorBoxes = new List<EditorBoxInfo>();

        private EditorBoxInfo selectedBox;

        private Vector2 curMouseWorldPosition;

        private ToolMode currentToolMode;
        private int scaleControlIndex = -1;

        private ToolMode CurrentToolMode
        {
            get => currentToolMode;
            set
            {
                if (currentToolMode != value)
                {
                    OnToolModeChange(value);
                }
                currentToolMode = value;
            }
        }

        #endregion Tool Functionality Variables

        #region On Value Change Functions

        private void OnSelectedAnimationChange()
        {
            SampleTime = 0;
            //Tools.current = Tool.View;
            CurrentToolMode = ToolMode.Select;
            editorBoxes.Clear();
            CurrentToolMode = ToolMode.Select;

            if (SelectedFrameData != null)
            {
                for (int i = 0; i < SelectedFrameData.BoxCount; i++)
                {
                    editorBoxes.Add(new EditorBoxInfo(SelectedFrameData[i], targetedObject.transform));
                }
            }
        }

        private void OnToolModeChange(ToolMode newMode)
        {
            if (selectedBox == null && newMode != ToolMode.Select)
            {
                CurrentToolMode = ToolMode.Select;
            }

            if (selectedBox != null)
            {
                switch (newMode)
                {
                    case ToolMode.Position:
                        GUIUtility.hotControl = selectedBox.positionControlID;
                        break;

                    case ToolMode.Rotation:
                        GUIUtility.hotControl = selectedBox.rotationControlID;
                        break;

                    case ToolMode.Scale:
                        //GUIUtility.hotControl = selectedBox.sca;
                        break;

                    case ToolMode.Vector:
                        GUIUtility.hotControl = selectedBox.vectorControlID;
                        break;
                }
            }
        }

        private void OnFrameDataChange()
        {
            SelectBox(null);
            editorBoxes.Clear();
            CurrentToolMode = ToolMode.Select;

            if (SelectedFrameData != null)
            {
                for (int i = 0; i < SelectedFrameData.BoxCount; i++)
                {
                    editorBoxes.Add(new EditorBoxInfo(SelectedFrameData[i], targetedObject.transform));
                }
            }
        }

        private void OnSampleTimeChange(float newSampleTime)
        {
            if (targetedObject != null && Selection.activeGameObject == targetedObject.gameObject)
                FrameDataIndex = GetFrameDataIndexAtTime(newSampleTime);
        }

        #endregion On Value Change Functions

        #region Initialization and Termination

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            editorBoxes.Clear();
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;

            Initialize();
        }

        private void Initialize()
        {
            m_IconContent = new GUIContent()
            {
                image = m_ToolIcon,
                text = "Frame Data Tool",
                tooltip = "Sets hit and hurt boxes"
            };
            enabled = true;
            editorBoxes.Clear();
            uiArea = new Rect(10, 10, 400, 270);
            targetedObject = null;
            targetedObject = Selection.activeGameObject?.GetComponent<PlayerCombat>();
            if (targetedObject == null)
            {
                enabled = false;
                return;
            }
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            FrameDataIndex = -1;
            SelectedAnimationIndex = 0;
            SampleTime = 0.1f;

            SampleTime = 0;
            selectedBox = null;
            animationNames = TargetRegistry.GetAnimationClipNames();
        }

        #endregion Initialization and Termination

        #region Scene Functions

        private void OnSceneGUI(SceneView obj)
        {
            if (!enabled)
            {
                return;
            }
            sceneView = obj;
            curMouseWorldPosition = GetWorldPosition(Event.current.mousePosition);

            switch (Event.current.type)
            {
                case EventType.MouseMove:
                    break;

                case EventType.MouseDown:
                    if (Event.current.button == 0)
                        OnMouseDown();
                    break;

                case EventType.MouseDrag:
                    if (Event.current.button == 0)
                        OnMouseDrag();
                    break;

                case EventType.MouseUp:
                    if (Event.current.button == 0)
                        OnMouseUp();
                    break;

                //Sets Controls
                case EventType.Layout:
                    OnLayout();
                    break;
                //Draws Handles
                case EventType.Repaint:
                    bool keyFramesExist = FrameDataIndex >= 0;

                    if (keyFramesExist)
                    {
                        PaintBoxes();
                    }
                    break;
            }
            if (selectedBox != null && Event.current.button == 0)
                DistributeBoxSceneFunctions();

            enabled = false;
        }

        private void OnLayout()
        {
            for (int i = 0; i < editorBoxes.Count; i++)
            {
                editorBoxes[i].SetControls(CurrentToolMode);
            }
        }

        private void OnMouseDown()
        {
            if (FrameDataIndex != -1)
            {
                if (uiArea.Contains(Event.current.mousePosition))
                {
                    return;
                }

                if (CurrentToolMode == ToolMode.Select)
                {
                    if (!uiArea.Contains(Event.current.mousePosition))
                        SelectBox(null);
                    SelectBox(GetBoxContainingCursor());
                }
                else if (CurrentToolMode == ToolMode.Remove)
                {
                    EditorBoxInfo boxToRemove = GetBoxContainingCursor(true);
                    if (boxToRemove != null)
                        RemoveBox(boxToRemove);
                }
                else if (CurrentToolMode == ToolMode.Add)
                {
                    EditorBoxInfo boxToAlter = GetBoxContainingCursor(true);
                    if (boxToAlter != null)
                        AlterBox(boxToAlter);
                    else
                        AddBox(curMouseWorldPosition, BoxType.Hit);
                }
                else if (CurrentToolMode == ToolMode.Vector)
                {
                    if (selectedBox.IsPointInVectorControl(curMouseWorldPosition))
                    {
                        GUIUtility.hotControl = selectedBox.vectorControlID;
                    }
                }
            }
        }

        private void OnMouseUp()
        {
        }

        private void OnMouseDrag()
        {
        }

        private void DistributeBoxSceneFunctions()
        {
            DeterminePositionFunctions();
            DetermineVectorFunctions();
            DetermineScaleFunctions();
            DetermineRotationFunctions();
        }

        #region Box Functions

        private void DeterminePositionFunctions()
        {
            if (CurrentToolMode != ToolMode.Position)
                return;
            if (Event.current.type == EventType.MouseDown && selectedBox.ContainsPoint(curMouseWorldPosition))
            {
                GUIUtility.hotControl = selectedBox.positionControlID;
            }
            else if (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl == selectedBox.positionControlID)
            {
                selectedBox.Move(GetWorldDelta());
            }
        }

        private void DetermineVectorFunctions()
        {
            if (CurrentToolMode != ToolMode.Vector)
                return;
            if (Event.current.type == EventType.MouseDown && selectedBox.IsPointInVectorControl(curMouseWorldPosition))
            {
                GUIUtility.hotControl = selectedBox.vectorControlID;
            }
            else if (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl == selectedBox.vectorControlID)
            {
                selectedBox.AdjustVector(curMouseWorldPosition);
            }
        }

        private void DetermineScaleFunctions()
        {
            if (CurrentToolMode != ToolMode.Scale)
                return;

            if (Event.current.type == EventType.MouseDown)
            {
                scaleControlIndex = -1;
                for (int i = 0; i < 4; i++)
                {
                    if (selectedBox.IsPointInScaleControl(curMouseWorldPosition, i))
                    {
                        scaleControlIndex = i;
                        GUIUtility.hotControl = selectedBox.scaleControlIDs[i];
                        break;
                    }
                }
            }
            else if (Event.current.type == EventType.MouseDrag && scaleControlIndex > -1 && GUIUtility.hotControl == selectedBox.scaleControlIDs[scaleControlIndex])
            {
                selectedBox.PlaceCorner(curMouseWorldPosition, scaleControlIndex);
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                scaleControlIndex = -1;
            }
        }

        private void DetermineRotationFunctions()
        {
            if (CurrentToolMode != ToolMode.Rotation)
                return;

            if (Event.current.type == EventType.MouseDown)
            {
                if (selectedBox.IsPointInRotationControl(curMouseWorldPosition))
                {
                    GUIUtility.hotControl = selectedBox.rotationControlID;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl == selectedBox.rotationControlID)
            {
                selectedBox.Rotate(curMouseWorldPosition - GetWorldDelta(), curMouseWorldPosition);
            }
        }

        #endregion Box Functions

        #endregion Scene Functions

        #region World Position Functions

        private Vector2 GetWorldDelta()
        {
            Vector2 oldScreenPosition = Event.current.mousePosition - Event.current.delta;

            Vector2 oldWorldPosition = GetWorldPosition(oldScreenPosition);

            Vector2 worldDelta = GetWorldPosition(Event.current.mousePosition) - oldWorldPosition;

            return worldDelta;
        }

        private Vector2 GetWorldPosition(Vector2 mousePos)
        {
            Vector3 mousePosition = mousePos;
            mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;
            mousePosition = sceneView.camera.ScreenToWorldPoint(mousePosition);

            return mousePosition;
        }

        #endregion World Position Functions

        #region UI Functions

        public override void OnToolGUI(EditorWindow window)
        {
            enabled = true;
            if (boxStyle == null)
                boxStyle = new GUIStyle("box");
            if (buttonStyle == null)
                buttonStyle = new GUIStyle("Button");

            bool attacksExist = animationNames.Length != 0;
            if (attacksExist)
            {
                DrawUI();
                SampleAnimation();
            }
            else
            {
                DrawWarning();
            }
        }

        private void DrawWarning()
        {
            Handles.BeginGUI();
            GUIStyle boxStyle = new GUIStyle("box");
            GUILayout.BeginArea(new Rect(10, 10, 350, 160), boxStyle);
            EditorGUILayout.HelpBox("Add Attacks", MessageType.Warning);
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private void DrawUI()
        {
            Handles.BeginGUI();

            EditorGUIUtility.AddCursorRect(uiArea, MouseCursor.Arrow);
            GUILayout.BeginArea(uiArea, boxStyle);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(targetedObject.name);

            EditorGUILayout.EndHorizontal();

            SelectedAnimationIndex = EditorGUILayout.Popup("Animation", SelectedAnimationIndex, animationNames);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Keyframes"))
            {
                ResetKeyFrames(SelectedAnimation);
            }
            if (GUILayout.Button("Fill Keyframes"))
            {
                FillKeyFrames();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("ST:" + (int)(SampleTime * SelectedAnimation.clip.frameRate), GUILayout.MaxWidth(30));
            SampleTime = EditorGUILayout.Slider(SampleTime, 0, SelectedAnimation.clip.length * .999f);
            EditorGUILayout.LabelField("KF:" + FrameDataIndex, GUILayout.MaxWidth(30));
            EditorGUILayout.EndHorizontal();

            DrawKeyframeButtons();

            if (SelectedFrameData != null)
            {
                DrawFrameDataInfo();
                EditorGUI.BeginChangeCheck();
                DrawSelectedBoxInfo();
                if (EditorGUI.EndChangeCheck())
                {
                    sceneView.Repaint();
                }
                DrawToolModeSelector();
            }
            DrawColors();

            GUILayout.EndArea();
            Handles.EndGUI();
            EditorUtility.SetDirty(TargetRegistry);
        }

        private void DrawKeyframeButtons()
        {
            bool frameDataExistsAtSampleTime = SelectedAnimation.clip.events.Any(e =>
                Mathf.Approximately(e.time, (int)(SampleTime * SelectedAnimation.clip.frameRate) / SelectedAnimation.clip.frameRate));
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(SelectedFrameData != null && frameDataExistsAtSampleTime);

            if (GUILayout.Button("Add Keyframe"))
            {
                AddKeyframeAtSampleTime();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(SelectedFrameData == null || !frameDataExistsAtSampleTime);
            if (GUILayout.Button("Remove Keyframe"))
            {
                RemoveKeyframeAtSampleTime();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSelectedBoxInfo()
        {
            EditorGUI.BeginDisabledGroup(selectedBox == null);
            boxInfoOpen = EditorGUILayout.Foldout(boxInfoOpen, "Selected Box Info") && selectedBox != null;
            boxInfoScrollPosition = EditorGUILayout.BeginScrollView(boxInfoScrollPosition);
            if (boxInfoOpen)
            {
                selectedBox.boxInfo.position = EditorGUILayout.Vector2Field("Position", selectedBox.boxInfo.position);
                selectedBox.boxInfo.rotation = EditorGUILayout.FloatField("Rotation", selectedBox.boxInfo.rotation);
                selectedBox.boxInfo.size = EditorGUILayout.Vector2Field("Scale", selectedBox.boxInfo.size);
                DrawBoxTypeSelector();
                EditorGUI.BeginDisabledGroup(selectedBox.BoxType != BoxType.Hit);

                if (selectedBox.boxInfo is HitboxInfo hitboxInfo)
                {
                    hitboxInfo.knockbackDirection = EditorGUILayout.Vector2Field("Knockback Direction", hitboxInfo.knockbackDirection);
                    hitboxInfo.knockbackDirection = hitboxInfo.knockbackDirection.normalized;

                    hitboxInfo.knockbackForce = EditorGUILayout.FloatField("Force", hitboxInfo.knockbackForce);
                    hitboxInfo.damage = EditorGUILayout.FloatField("Damage Dealt", hitboxInfo.damage);
                    hitboxInfo.hitStun = EditorGUILayout.FloatField("Hitstun Duration", hitboxInfo.hitStun);
                }

                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(selectedBox.BoxType == BoxType.Hit);
                if (selectedBox.boxInfo is HurtboxInfo hurtboxInfo)
                {
                    hurtboxInfo.superArmor = EditorGUILayout.Toggle("Superarmor", hurtboxInfo.superArmor);
                    EditorGUI.BeginDisabledGroup(hurtboxInfo.superArmor);
                    hurtboxInfo.damageMitigation = EditorGUILayout.Slider("Damage Mitigation", hurtboxInfo.damageMitigation, 0, 100);
                    hurtboxInfo.hitstunMitigation = EditorGUILayout.Slider("Hitstun Mitigation", hurtboxInfo.hitstunMitigation, 0, 100);
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndScrollView();

            EditorGUI.EndDisabledGroup();
        }

        private void DrawBoxTypeSelector()
        {
            int boxType = selectedBox.BoxType == 0 ? 0 : 1;
            int boxTypeSelection = GUILayout.Toolbar(boxType, BoxTypeNames);
            if (boxTypeSelection != boxType)
            {
                AlterBox(selectedBox);
            }
        }

        private void DrawFrameDataInfo()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Hitbox Count: " + SelectedFrameData.HitboxCount);
            EditorGUILayout.LabelField("Hurtbox Count: " + SelectedFrameData.HurtboxCount);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawToolModeSelector()
        {
            EditorGUILayout.BeginHorizontal();
            CurrentToolMode = GUILayout.Toggle(CurrentToolMode == ToolMode.Select, "Select", buttonStyle) ? ToolMode.Select : CurrentToolMode;
            CurrentToolMode = GUILayout.Toggle(CurrentToolMode == ToolMode.Add, "Add", buttonStyle) ? ToolMode.Add : CurrentToolMode;
            CurrentToolMode = GUILayout.Toggle(CurrentToolMode == ToolMode.Remove, "Remove", buttonStyle) ? ToolMode.Remove : CurrentToolMode;
            EditorGUI.BeginDisabledGroup(selectedBox == null);
            CurrentToolMode = GUILayout.Toggle(CurrentToolMode == ToolMode.Position, "Position", buttonStyle) ? ToolMode.Position : CurrentToolMode;
            CurrentToolMode = GUILayout.Toggle(CurrentToolMode == ToolMode.Rotation, "Rotation", buttonStyle) ? ToolMode.Rotation : CurrentToolMode;
            CurrentToolMode = GUILayout.Toggle(CurrentToolMode == ToolMode.Scale, "Scale", buttonStyle) ? ToolMode.Scale : CurrentToolMode;
            CurrentToolMode = GUILayout.Toggle(CurrentToolMode == ToolMode.Vector, "Vector", buttonStyle) ? ToolMode.Vector : CurrentToolMode;
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawColors()
        {
            colorGroupOpen = EditorGUILayout.Foldout(colorGroupOpen, "Colors");
            if (colorGroupOpen)
            {
                HurtboxColor = EditorGUILayout.ColorField("Hurtbox Color", (Color)HurtboxColor);
                HitboxColor = EditorGUILayout.ColorField("Hitbox Color", (Color)HitboxColor);
                SuperArmorColor = EditorGUILayout.ColorField("Super Armor Color", (Color)SuperArmorColor);
                SelectedBoxColor = EditorGUILayout.ColorField("Selected Box Color", (Color)SelectedBoxColor);
                HighlightedHandleColor = EditorGUILayout.ColorField("Hightlighted Handle Color", (Color)HighlightedHandleColor);
            }
        }

        #endregion UI Functions

        #region Box Handling Functions

        private void SelectBox(EditorBoxInfo boxToSelect)

        {
            if (selectedBox != null)
                selectedBox.selected = false;

            selectedBox = boxToSelect;

            if (selectedBox != null)
            {
                GUIUtility.hotControl = selectedBox.positionControlID;
                selectedBox.selected = true;
            }
        }

        private void PaintBoxes()
        {
            for (int i = 0; i < editorBoxes.Count; i++)
            {
                editorBoxes[i].Draw();
            }

            if (selectedBox != null)
            {
                if (currentToolMode == ToolMode.Rotation)
                {
                    selectedBox.DrawRotation();
                }
                else if (currentToolMode == ToolMode.Scale)
                {
                    selectedBox.DrawScale();
                }
                else if (currentToolMode == ToolMode.Vector)
                {
                    selectedBox.DrawVector();
                }
            }
        }

        private void AlterBox(EditorBoxInfo boxToAlter)
        {
            if (boxToAlter.BoxType == BoxType.Hurt)
            {
                int boxIndex = SelectedFrameData.boxes.IndexOf(boxToAlter.boxInfo);

                BoxInfo oldBox = SelectedFrameData[boxIndex];
                SelectedFrameData.Remove(oldBox);

                BoxInfo newBox = new HitboxInfo() { position = oldBox.position, rotation = oldBox.rotation, size = oldBox.size };
                boxToAlter.boxInfo = newBox;
                SelectedFrameData.AddBox(newBox);
            }
            else
            {
                int boxIndex = SelectedFrameData.boxes.IndexOf(boxToAlter.boxInfo);

                BoxInfo oldBox = SelectedFrameData[boxIndex];
                SelectedFrameData.Remove(oldBox);

                BoxInfo newBox = new HurtboxInfo() { position = oldBox.position, rotation = oldBox.rotation, size = oldBox.size };
                boxToAlter.boxInfo = newBox;
                SelectedFrameData.AddBox(newBox);
            }
        }

        #endregion Box Handling Functions

        #region Helper Functions

        private void SampleAnimation()
        {
            SelectedAnimation.clip.SampleAnimation(targetedObject.gameObject, SampleTime);
            if (SelectedAnimation.frameData.Count > FrameDataIndex && FrameDataIndex >= 0)
                targetedObject.UpdateCollisionBoxes(SelectedAnimation.frameData[FrameDataIndex]);
        }

        private void ResetKeyFrames(Combat.AnimationInfo attackInfo)
        {
            attackInfo.frameData.Clear();
            foreach (var item in attackInfo.clip.events)
            {
                if ((PlayerAnimationEventType)Utilities.GetByte(item.intParameter, 0) == (int)PlayerAnimationEventType.AttackKeyFrame)
                {
                    attackInfo.frameData.Add(new FrameData(item.time));
                }
            }

            SelectedAnimation.SortFrameData();
        }

        private void FillKeyFrames()
        {
            float length = SelectedAnimation.clip.length;
            float fps = SelectedAnimation.clip.frameRate;
            float time = 0;
            List<AnimationEvent> events = new List<AnimationEvent>();
            events.AddRange(AnimationUtility.GetAnimationEvents(SelectedAnimation.clip));

            while (time < length)
            {
                if (!SelectedAnimation.clip.events.Any(e => Mathf.Approximately(e.time, (int)(time * SelectedAnimation.clip.frameRate) / SelectedAnimation.clip.frameRate)))
                {
                    AnimationEvent newEvent = new AnimationEvent();

                    float newTime = time * SelectedAnimation.clip.frameRate;
                    int closestSample = (int)(newTime);

                    newEvent.time = closestSample / SelectedAnimation.clip.frameRate;
                    newEvent.functionName = "InvokeAnimationEvent";

                    byte eventType = (byte)PlayerAnimationEventType.AttackKeyFrame;
                    byte animationIndex = (byte)SelectedAnimationIndex;
                    byte frameDataIndex = (byte)closestSample;

                    int value = System.BitConverter.ToInt32(new byte[] { eventType, animationIndex, frameDataIndex, 0 }, 0);
                    newEvent.intParameter = value;
                    Debug.Log(closestSample);
                    SelectedAnimation.frameData.Insert(frameDataIndex, new FrameData(newEvent.time));
                    events.Add(newEvent);
                }
                time += 1 / fps;
            }

            AnimationUtility.SetAnimationEvents(SelectedAnimation.clip, events.ToArray());
            SelectedAnimation.SortFrameData();

            OnSampleTimeChange(SampleTime);
        }

        private int GetFrameDataIndexAtTime(float time)
        {
            int indexOfLastKeyFrame = -1;

            for (int i = SelectedAnimation.frameData.Count - 1; i >= 0; i--)
            {
                bool timeIsBeforeSample = SelectedAnimation.frameData[i].Time <= time;
                if (timeIsBeforeSample)
                {
                    indexOfLastKeyFrame = i;
                    break;
                }
            }

            return indexOfLastKeyFrame;
        }

        private void AddBox(Vector2 worldPosition, BoxType boxType)
        {
            BoxInfo newBox = null;
            switch (boxType)
            {
                case BoxType.Hit:
                    newBox = new HitboxInfo();
                    break;

                case BoxType.Hurt:
                    newBox = new HurtboxInfo();
                    break;

                case BoxType.Super:
                    newBox = new HurtboxInfo() { superArmor = true };
                    break;
            }
            newBox.position = targetedObject.transform.InverseTransformPoint(worldPosition);

            SelectedFrameData.AddBox(newBox);
            EditorBoxInfo newBoxInfo = new EditorBoxInfo(newBox, targetedObject.transform);
            editorBoxes.Add(newBoxInfo);
        }

        private void RemoveBox(EditorBoxInfo boxToRemove)
        {
            SelectedFrameData.Remove(boxToRemove.boxInfo);
            editorBoxes.Remove(boxToRemove);
        }

        private EditorBoxInfo GetBoxContainingCursor(bool topDown = false)
        {
            EditorBoxInfo boxContainingCursor = null;

            if (topDown)
            {
                for (int i = editorBoxes.Count - 1; i >= 0; i--)
                {
                    if (editorBoxes[i].ContainsPoint(curMouseWorldPosition))
                    {
                        boxContainingCursor = editorBoxes[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < editorBoxes.Count; i++)
                {
                    if (editorBoxes[i].ContainsPoint(curMouseWorldPosition))
                    {
                        boxContainingCursor = editorBoxes[i];
                    }
                }
            }

            return boxContainingCursor;
        }

        private void AddKeyframeAtSampleTime()
        {
            AnimationEvent newEvent = new AnimationEvent();

            float newTime = SampleTime * SelectedAnimation.clip.frameRate;
            int closestSample = (int)(newTime);

            newEvent.time = closestSample / SelectedAnimation.clip.frameRate;
            newEvent.functionName = "InvokeAnimationEvent";

            byte eventType = (byte)PlayerAnimationEventType.AttackKeyFrame;
            byte animationIndex = (byte)SelectedAnimationIndex;
            byte frameDataIndex = (byte)(FrameDataIndex + 1);

            int value = System.BitConverter.ToInt32(new byte[] { eventType, animationIndex, frameDataIndex, 0 }, 0);
            newEvent.intParameter = value;

            List<AnimationEvent> currentAnimationEvents = new List<AnimationEvent>();
            currentAnimationEvents.AddRange(AnimationUtility.GetAnimationEvents(SelectedAnimation.clip));
            currentAnimationEvents.Add(newEvent);

            FrameData newFrameData = new FrameData(newEvent.time);
            SelectedAnimation.frameData.Add(newFrameData);

            SelectedAnimation.SortFrameData();

            AnimationUtility.SetAnimationEvents(SelectedAnimation.clip, currentAnimationEvents.OrderBy(s => s.time).ToArray());
            OnSampleTimeChange(SampleTime);
        }

        private void RemoveKeyframeAtSampleTime()
        {
            List<AnimationEvent> currentAnimationEvents = new List<AnimationEvent>();
            currentAnimationEvents.AddRange(AnimationUtility.GetAnimationEvents(SelectedAnimation.clip));
            currentAnimationEvents.RemoveAll(e => Mathf.Approximately(e.time, (int)(SampleTime * SelectedAnimation.clip.frameRate) / SelectedAnimation.clip.frameRate));
            SelectedAnimation.frameData.Remove(SelectedFrameData);
            SelectedAnimation.SortFrameData();

            AnimationUtility.SetAnimationEvents(SelectedAnimation.clip, currentAnimationEvents.OrderBy(s => s.time).ToArray());
            OnSampleTimeChange(SampleTime);
        }

        #endregion Helper Functions
    }
}