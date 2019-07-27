using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using System.Linq;
using Duel.PlayerSystems;
using Sirenix.Serialization;
using Duel.Combat;
using Sirenix.OdinInspector.Editor;
using System;

namespace Duel.Editor
{
    [EditorTool("Frame Data Tool", typeof(PlayerSystems.PlayerCombat))]
    public class FrameDataTool : EditorTool
    {
        [SerializeField]
        private Texture2D m_ToolIcon;

        private GUIContent m_IconContent;
        private SerializedProperty attacks;
        private PlayerCombat targetedObject;

        private Color hurtboxColor = Color.green;
        private Color hitboxColor = Color.red;
        private Color superArmorColor = Color.cyan;

        private float sampleTime;

        private string[] attackNames = new string[0];

        private int selectedAttackIndex;
        private BoxInfo selectedBox;
        private bool isHitboxSelected;
        private int boxIndex;
        private Vector2 positionInBox;

        private Vector3 mousePosition;

        private CharacterAttackRegistryObject TargetRegistry
        {
            get => targetedObject?.attackRegistryObject;
        }

        private AttackInfo SelectedAttack
        {
            get => TargetRegistry?[SelectedAttackIndex];
        }

        private int SelectedAttackIndex
        {
            get => selectedAttackIndex;
            set
            {
                if (selectedAttackIndex != value)
                {
                    selectedAttackIndex = value;
                    OnSelectedAttackChange();
                }
                selectedAttackIndex = value;
            }
        }

        private int keyFrameIndex;

        private void OnSelectedAttackChange()
        {
            sampleTime = 0;
            KeyFrameIndex = -1;
            selectedBox = null;
            KeyFrameIndex = GetKeyFrameIndex();
        }

        public override GUIContent toolbarIcon
        {
            get => m_IconContent;
        }

        public int KeyFrameIndex
        {
            get => keyFrameIndex;
            set
            {
                if (value != keyFrameIndex)
                {
                    OnKeyFrameChange();
                }
                keyFrameIndex = value;
            }
        }

        private void OnKeyFrameChange()
        {
            selectedBox = null;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnEnable()
        {
            KeyFrameIndex = -1;
            targetedObject = null;
            targetedObject = Selection.activeGameObject?.GetComponent<PlayerCombat>();
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            Initialize(targetedObject);
        }

        private void OnSceneGUI(SceneView obj)
        {
            switch (Event.current.type)
            {
                case EventType.MouseMove:
                    OnMouseMove(obj);
                    break;

                case EventType.MouseDown:
                    OnMouseDown();
                    break;

                case EventType.MouseDrag:
                    OnMouseDrag();
                    break;

                case EventType.MouseUp:
                    OnMouseUp();
                    break;

                //Sets Controls
                case EventType.Layout:
                    break;
                //Draws Handles
                case EventType.Repaint:
                    break;
            }
        }

        private void OnMouseMove(SceneView sceneView)
        {
            Vector3 mousePosition = Event.current.mousePosition;
            mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;
            mousePosition = sceneView.camera.ScreenToWorldPoint(mousePosition);
            this.mousePosition = mousePosition;
            boxIndex = -1;
            if (keyFrameIndex != -1)
            {
                for (int i = 0; i < SelectedAttack.keyFrames[keyFrameIndex].hitBoxes.Count; i++)
                {
                    if (IsPointInBox(SelectedAttack.keyFrames[keyFrameIndex].hitBoxes[i], this.mousePosition))
                    {
                        isHitboxSelected = true;
                        selectedBox = SelectedAttack.keyFrames[keyFrameIndex].hitBoxes[i];
                        boxIndex = i;
                    }
                }
            }
        }

        private void OnMouseDown()
        {
        }

        private void OnMouseDrag()
        {
        }

        private void OnMouseUp()
        {
        }

        private void Initialize(PlayerCombat targetedObject)
        {
            m_IconContent = new GUIContent()
            {
                image = m_ToolIcon,
                text = "Frame Data Tool",
                tooltip = "Sets hit and hurt boxes"
            };
            KeyFrameIndex = -1;

            selectedAttackIndex = 0;
            sampleTime = 0;
            selectedBox = null;
            boxIndex = -1;
            attackNames = TargetRegistry.GetAttackAnimationNames();
        }

        public override void OnToolGUI(EditorWindow window)
        {
            KeyFrameIndex = -1;
            bool attacksExist = attackNames.Length != 0;
            if (attacksExist)
            {
                KeyFrameIndex = GetKeyFrameIndex();
                DrawUI();
                SampleAnimation();
                bool keyFramesExist = KeyFrameIndex >= 0;

                if (keyFramesExist)
                {
                    DrawControls();
                    DrawBoxUI();
                }
            }
            else
            {
                DrawWarning();
            }
        }

        private void DrawBoxUI()
        {
            if (selectedBox == null)
                return;
            Handles.BeginGUI();
            GUIStyle boxStyle = new GUIStyle("box");
            GUILayout.BeginArea(new Rect(400, 10, 350, 80), boxStyle);
            string boxType = isHitboxSelected ? "Hitbox" : "Hurtbox";
            EditorGUILayout.HelpBox($"{boxType}[{boxIndex}]", MessageType.Info);
            GUILayout.EndArea();
            Handles.EndGUI();
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

            GUIStyle boxStyle = new GUIStyle("box");
            Rect area = new Rect(10, 10, 400, 270);

            EditorGUIUtility.AddCursorRect(area, MouseCursor.Arrow);
            GUILayout.BeginArea(area, boxStyle);
            GUILayout.Label(targetedObject.name);

            SelectedAttackIndex = EditorGUILayout.Popup("Attack", SelectedAttackIndex, attackNames);

            EditorGUILayout.BeginHorizontal();
            sampleTime = EditorGUILayout.Slider(sampleTime, 0, SelectedAttack.attackAnimation.length);
            EditorGUILayout.LabelField("KF:" + KeyFrameIndex, GUILayout.MaxWidth(30));
            EditorGUILayout.EndHorizontal();
            if (KeyFrameIndex >= 0 && SelectedAttack.keyFrames.Count > KeyFrameIndex)
            {
                EditorGUILayout.LabelField("Hitbox Count: " + SelectedAttack.keyFrames[KeyFrameIndex].hitBoxes.Count);
                EditorGUILayout.LabelField("Hurtbox Count: " + SelectedAttack.keyFrames[KeyFrameIndex].hurtBoxes.Count);
                EditorGUILayout.BeginHorizontal();
                FrameData currentFrameData = SelectedAttack.keyFrames[KeyFrameIndex];
                if (GUILayout.Button("Add Hitbox"))
                {
                    currentFrameData.hitBoxes.Add(new HitboxInfo());
                }
                if (GUILayout.Button("Add Hurtbox"))
                {
                    currentFrameData.hurtBoxes.Add(new HurtboxInfo());
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Reset Keyframes"))
            {
                ResetKeyFrames(SelectedAttack);
            }

            hurtboxColor = EditorGUILayout.ColorField("Hurtbox Color", hurtboxColor);
            hitboxColor = EditorGUILayout.ColorField("Hitbox Color", hitboxColor);
            superArmorColor = EditorGUILayout.ColorField("Super Armor Color", superArmorColor);
            EditorGUILayout.Vector2Field("Mouse Point", mousePosition);
            EditorGUILayout.Vector2Field("Box Point", positionInBox);

            GUILayout.EndArea();
            Handles.EndGUI();
            EditorUtility.SetDirty(TargetRegistry);
        }

        private void DrawControls()
        {
            if (SelectedAttack == null)
                return;
            for (int i = 0; i < SelectedAttack.keyFrames[KeyFrameIndex].hitBoxes.Count; i++)
            {
                DrawHitbox(i);
            }
        }

        private HitboxInfo GetHitboxInfo(int index)
        {
            return SelectedAttack.keyFrames[KeyFrameIndex].hitBoxes[index];
        }

        private HurtboxInfo GetHurtboxInfo(int index)
        {
            return SelectedAttack.keyFrames[KeyFrameIndex].hurtBoxes[index];
        }

        private void SampleAnimation()
        {
            SelectedAttack.attackAnimation.SampleAnimation(targetedObject.gameObject, sampleTime);
            if (SelectedAttack.keyFrames.Count > KeyFrameIndex && KeyFrameIndex >= 0)
                targetedObject.UpdateCollisionBoxes(SelectedAttack.keyFrames[KeyFrameIndex]);
        }

        private void DrawHitbox(int index)
        {
            HitboxInfo targetHitbox = GetHitboxInfo(index);

            Vector2 adjustedPosition = targetedObject.transform.InverseTransformPoint(targetHitbox.position);
            Quaternion rotation = Quaternion.Euler(0, 0, targetHitbox.rotation);

            Matrix4x4 m = Matrix4x4.Translate(adjustedPosition) * Matrix4x4.Rotate(rotation);

            Vector3[] hitboxVerts = new Vector3[4];

            hitboxVerts[0] = m.MultiplyPoint3x4(new Vector3(-targetHitbox.size.x, -targetHitbox.size.y, 0));
            hitboxVerts[1] = m.MultiplyPoint3x4(new Vector3(targetHitbox.size.x, -targetHitbox.size.y, 0));
            hitboxVerts[2] = m.MultiplyPoint3x4(new Vector3(targetHitbox.size.x, targetHitbox.size.y, 0));
            hitboxVerts[3] = m.MultiplyPoint3x4(new Vector3(-targetHitbox.size.x, targetHitbox.size.y, 0));

            Handles.DrawSolidRectangleWithOutline(hitboxVerts, hitboxColor, Color.yellow);

            targetHitbox.position = targetedObject.transform.TransformPoint(Handles.FreeMoveHandle(adjustedPosition, rotation, HandleUtility.GetHandleSize(adjustedPosition), Vector3.one * .1f, (a, b, c, d, e) => PositionHandle(a, b, c, d, e, targetHitbox, true, index)));
        }

        private bool IsPointInBox(BoxInfo box, Vector3 point)
        {
            bool pointInBox = false;
            Vector2 adjustedPosition = targetedObject.transform.InverseTransformPoint(box.position);
            Quaternion rotation = Quaternion.Euler(0, 0, box.rotation);
            Matrix4x4 m = Matrix4x4.Translate(adjustedPosition) * Matrix4x4.Rotate(rotation);

            Vector3 adjustedPoint = m.inverse.MultiplyPoint3x4(point);
            positionInBox = adjustedPoint;
            if (Mathf.Abs(adjustedPoint.x) <= Mathf.Abs(box.size.x) && Mathf.Abs(adjustedPoint.y) <= Mathf.Abs(box.size.y))
            {
                pointInBox = true;
            }

            return pointInBox;
        }

        private void PositionHandle(int controlId, Vector3 pos, Quaternion rot, float size, EventType ev, BoxInfo box, bool isHitbox, int index)
        {
            Handles.RectangleHandleCap(controlId, pos, rot, size, ev);

            /* if (HandleUtility.nearestControl == controlId)
             {
                 isHitboxSelected = isHitbox;
                 selectedBox = box;
                 boxIndex = index;
             }*/
        }

        private void DrawHurtbox(int index)
        {
            HurtboxInfo targetHurtbox = GetHurtboxInfo(index);

            Vector2 adjustedPosition = targetedObject.transform.InverseTransformPoint(targetHurtbox.position);
            Quaternion rotation = Quaternion.Euler(0, 0, targetHurtbox.rotation);

            Matrix4x4 initial = Matrix4x4.Translate(adjustedPosition);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);
            Matrix4x4 final = Matrix4x4.Translate(adjustedPosition * -1);

            Matrix4x4 transformation = initial * rotationMatrix * final;

            Vector3[] hitboxVerts = new Vector3[4];

            hitboxVerts[0] = transformation.MultiplyPoint3x4(new Vector3(targetHurtbox.position.x - targetHurtbox.size.x, targetHurtbox.position.y - targetHurtbox.size.y, 0));
            hitboxVerts[1] = transformation.MultiplyPoint3x4(new Vector3(targetHurtbox.position.x + targetHurtbox.size.x, targetHurtbox.position.y - targetHurtbox.size.y, 0));
            hitboxVerts[2] = transformation.MultiplyPoint3x4(new Vector3(targetHurtbox.position.x + targetHurtbox.size.x, targetHurtbox.position.y + targetHurtbox.size.y, 0));
            hitboxVerts[3] = transformation.MultiplyPoint3x4(new Vector3(targetHurtbox.position.x - targetHurtbox.size.x, targetHurtbox.position.y + targetHurtbox.size.y, 0));

            Handles.DrawSolidRectangleWithOutline(hitboxVerts, targetHurtbox.superArmor ? superArmorColor : hurtboxColor, Color.yellow);

            targetHurtbox.position = targetedObject.transform.TransformPoint(Handles.PositionHandle(adjustedPosition, rotation));
            targetHurtbox.rotation = Handles.RotationHandle(rotation, adjustedPosition).eulerAngles.z;
            targetHurtbox.size = Handles.ScaleHandle(targetHurtbox.size, adjustedPosition, rotation, HandleUtility.GetHandleSize(adjustedPosition));
        }

        private void ResetKeyFrames(AttackInfo attackInfo)
        {
            attackInfo.keyFrames.Clear();
            foreach (var item in attackInfo.attackAnimation.events)
            {
                if (item.stringParameter == "AttackKeyFrame")
                {
                    attackInfo.keyFrames.Add(new FrameData());
                }
            }
        }

        private int GetKeyFrameIndex()
        {
            int indexOfLastKeyFrame = -1;
            float timeProgression = 0;
            for (int i = 0; i < SelectedAttack.attackAnimation.events.Length; i++)
            {
                if (SelectedAttack.attackAnimation.events[i].stringParameter == "AttackKeyFrame" && SelectedAttack.attackAnimation.events[i].time > timeProgression && SelectedAttack.attackAnimation.events[i].time <= sampleTime)
                {
                    timeProgression = SelectedAttack.attackAnimation.events[i].time;
                    indexOfLastKeyFrame = i;
                }
            }

            return indexOfLastKeyFrame;
        }
    }
}