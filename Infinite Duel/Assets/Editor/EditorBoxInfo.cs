using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Duel.Combat;
using UnityEditor;
using System;

namespace Duel.Editor
{
    public class EditorBoxInfo
    {
        public BoxInfo boxInfo;
        private Transform parent;
        public bool selected;

        public int positionControlID;
        public int rotationControlID;
        public int[] scaleControlIDs = new int[4];
        public int vectorControlID;

        public BoxType BoxType
        {
            get => boxInfo.Type;
        }

        public Vector2 WorldPosition
        {
            get => parent.TransformPoint(boxInfo.position);
        }

        public Quaternion Rotation
        {
            get => Quaternion.Euler(0, 0, boxInfo.rotation);
        }

        public Matrix4x4 LocalToWorldMatrix
        {
            get => Matrix4x4.Translate(WorldPosition) * Matrix4x4.Rotate(Rotation);
        }

        public Matrix4x4 WorldToLocalMatrix
        {
            get => Matrix4x4.Inverse(LocalToWorldMatrix);
        }

        public EditorBoxInfo(BoxInfo boxInfo, Transform parent)
        {
            this.boxInfo = boxInfo;
            this.parent = parent;
        }

        public void Draw()
        {
            positionControlID = GUIUtility.GetControlID(FocusType.Passive);

            Vector3[] hitboxVerts = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                hitboxVerts[i] = GetWorldCorner(i);
            }

            Handles.DrawSolidRectangleWithOutline(hitboxVerts, GetColor(), GetColor());
        }

        public void SetControls(FrameDataTool.ToolMode toolMode)
        {
            HandleUtility.AddDefaultControl(positionControlID);

            if (toolMode == FrameDataTool.ToolMode.Rotation)
            {
                HandleUtility.AddDefaultControl(rotationControlID);
            }
            else if (toolMode == FrameDataTool.ToolMode.Scale)
            {
                for (int i = 0; i < 4; i++)
                {
                    HandleUtility.AddDefaultControl(scaleControlIDs[i]);
                }
            }
            else if (toolMode == FrameDataTool.ToolMode.Vector)
            {
                HandleUtility.AddDefaultControl(vectorControlID);
            }
        }

        public void Move(Vector2 delta)
        {
            boxInfo.position += (Vector2)parent.InverseTransformVector(delta);
        }

        public void PlaceCorner(Vector2 cornerWorldPos, int cornerIndex)
        {
            int opposingCornerIndex = (cornerIndex + 2) % 4;

            Vector2 oldCornerPos = GetLocalCorner(cornerIndex);
            Vector2 newCornerPos = WorldToLocalMatrix.MultiplyPoint(cornerWorldPos);

            Vector2 opposingCornerPos = GetLocalCorner(opposingCornerIndex);

            Vector2 newSize = new Vector2(newCornerPos.x - opposingCornerPos.x, newCornerPos.y - opposingCornerPos.y);
            newSize = new Vector2(Mathf.Abs(newSize.x), Mathf.Abs(newSize.y));
            Vector2 newPosition = LocalToWorldMatrix.MultiplyPoint3x4(new Vector2((newCornerPos.x + opposingCornerPos.x) / 2f, (newCornerPos.y + opposingCornerPos.y) / 2f));

            if ((oldCornerPos.x > opposingCornerPos.x && newCornerPos.x < opposingCornerPos.x) ||
                (oldCornerPos.x < opposingCornerPos.x && newCornerPos.x > opposingCornerPos.x) ||
                (oldCornerPos.y > opposingCornerPos.y && newCornerPos.y < opposingCornerPos.y) ||
                (oldCornerPos.y < opposingCornerPos.y && newCornerPos.y > opposingCornerPos.y))
            {
                return;
            }

            boxInfo.size = newSize;
            boxInfo.position = newPosition;
        }

        public void Rotate(Vector2 startPoint, Vector2 endPoint)
        {
            Vector2 dirToStart = WorldToLocalMatrix.MultiplyPoint(startPoint).normalized;
            Vector2 dirToEnd = WorldToLocalMatrix.MultiplyPoint(endPoint).normalized;

            float angleToStart = Mathf.Atan2(dirToStart.y, dirToStart.x); // In Radians
            float angleToEnd = Mathf.Atan2(dirToEnd.y, dirToEnd.x);// In Radians

            float angleDelta = angleToEnd - angleToStart;

            float degreeDelta = angleDelta * Mathf.Rad2Deg;
            boxInfo.rotation += degreeDelta;
        }

        public bool ContainsPoint(Vector2 point)
        {
            Vector3 adjustedPoint = WorldToLocalMatrix.MultiplyPoint3x4(point);
            bool inBox = Mathf.Abs(adjustedPoint.x) <= Mathf.Abs(boxInfo.size.x / 2f) && Mathf.Abs(adjustedPoint.y) <= Mathf.Abs(boxInfo.size.y / 2f);
            return inBox;
        }

        public Color GetColor()
        {
            if (GUIUtility.hotControl == positionControlID)
            {
                return new Color32(255, 255, 0, 64);
            }
            else if (selected)
            {
                return (Color)FrameDataTool.SelectedBoxColor;
            }
            else if (BoxType == BoxType.Hit)
            {
                return (Color)FrameDataTool.HitboxColor;
            }
            else if (BoxType == BoxType.Hurt)
            {
                return (Color)FrameDataTool.HurtboxColor;
            }
            else
            {
                return (Color)FrameDataTool.SuperArmorColor;
            }
        }

        internal void DrawRotation()
        {
            rotationControlID = GUIUtility.GetControlID(FocusType.Passive);
            Handles.color = GUIUtility.hotControl == rotationControlID ? Color.yellow : Color.white;

            float hypotenuse = Mathf.Sqrt((boxInfo.size.x / 2f) * (boxInfo.size.x / 2f) + (boxInfo.size.y / 2f) * (boxInfo.size.y / 2f));
            Handles.DrawWireDisc(WorldPosition, Vector3.forward, hypotenuse);
        }

        internal void DrawScale()
        {
            float hypotenuse = Mathf.Sqrt((boxInfo.size.x / 2f) * (boxInfo.size.x / 2f) + (boxInfo.size.y / 2f) * (boxInfo.size.y / 2f)) / 20f;
            Vector2 cornerDistance = boxInfo.size / 2f;
            for (int i = 0; i < 4; i++)
            {
                scaleControlIDs[i] = GUIUtility.GetControlID(FocusType.Passive);
                Vector2 cornerLocalPosition = new Vector2();

                switch (i)
                {
                    case 0:
                        cornerLocalPosition = new Vector2(cornerDistance.x, cornerDistance.y);
                        break;

                    case 1:
                        cornerLocalPosition = new Vector2(cornerDistance.x, -cornerDistance.y);
                        break;

                    case 2:
                        cornerLocalPosition = new Vector2(-cornerDistance.x, -cornerDistance.y);
                        break;

                    case 3:
                        cornerLocalPosition = new Vector2(-cornerDistance.x, cornerDistance.y);
                        break;
                }

                Vector2 cornerWorldPosition = LocalToWorldMatrix.MultiplyPoint3x4(cornerLocalPosition);
                Handles.color = GUIUtility.hotControl == scaleControlIDs[i] ? Color.yellow : Color.white;
                Handles.DrawWireDisc(cornerWorldPosition, Vector3.forward, hypotenuse);
            }
        }

        internal void DrawVector()
        {
            if (boxInfo.Type == BoxType.Hit)
            {
                float hypotenuse = Mathf.Sqrt((boxInfo.size.x / 2f) * (boxInfo.size.x / 2f) + (boxInfo.size.y / 2f) * (boxInfo.size.y / 2f));
                Handles.color = GUIUtility.hotControl == vectorControlID ? Color.yellow : Color.white;

                Vector2 directionPoint = WorldPosition + ((HitboxInfo)boxInfo).knockbackDirection.normalized * hypotenuse;
                Handles.DrawLine(WorldPosition, directionPoint);

                vectorControlID = GUIUtility.GetControlID(FocusType.Passive);
                Handles.DrawWireDisc(directionPoint, Vector3.forward, hypotenuse / 20f);
            }
        }

        internal void AdjustVector(Vector2 endPoint)
        {
            if (BoxType == BoxType.Hit)
            {
                ((HitboxInfo)boxInfo).knockbackDirection = (endPoint - WorldPosition).normalized;
            }
        }

        public bool IsPointInVectorControl(Vector2 point)
        {
            if (BoxType != BoxType.Hit)
                return false;
            float hypotenuse = Mathf.Sqrt((boxInfo.size.x / 2f) * (boxInfo.size.x / 2f) + (boxInfo.size.y / 2f) * (boxInfo.size.y / 2f));
            Vector2 directionPoint = WorldPosition + ((HitboxInfo)boxInfo).knockbackDirection.normalized * hypotenuse;

            return Vector2.Distance(directionPoint, point) <= (hypotenuse / 20f);
        }

        /// <summary>
        /// Is Point In Any Scale Control
        /// </summary>

        public bool IsPointInScaleControl(Vector2 point)
        {
            float hypotenuse = Mathf.Sqrt((boxInfo.size.x / 2f) * (boxInfo.size.x / 2f) + (boxInfo.size.y / 2f) * (boxInfo.size.y / 2f)) / 20f;
            Vector2 cornerWorldPosition = LocalToWorldMatrix.MultiplyPoint3x4(new Vector2(boxInfo.size.x, boxInfo.size.y));

            Vector2 absolutePoint = new Vector2(Mathf.Abs(point.x), Mathf.Abs(point.y));

            return Vector2.Distance(absolutePoint, cornerWorldPosition) <= hypotenuse;
        }

        public bool IsPointInRotationControl(Vector2 point)
        {
            float hypotenuse = Mathf.Sqrt((boxInfo.size.x / 2f) * (boxInfo.size.x / 2f) + (boxInfo.size.y / 2f) * (boxInfo.size.y / 2f));

            return WorldToLocalMatrix.MultiplyPoint3x4(point).magnitude <= hypotenuse;
        }

        /// <summary>
        /// Is Point In Specific Scale Control
        /// </summary>
        /// <param name="point"></param>
        /// <param name="cornerIndex"></param>
        /// <returns></returns>
        public bool IsPointInScaleControl(Vector2 point, int cornerIndex)
        {
            float hypotenuse = Mathf.Sqrt((boxInfo.size.x / 2f) * (boxInfo.size.x / 2f) + (boxInfo.size.y / 2f) * (boxInfo.size.y / 2f)) / 20f;
            Vector2 cornerWorldPosition = GetWorldCorner(cornerIndex);

            Vector2 absolutePoint = new Vector2(point.x, point.y);

            return Vector2.Distance(absolutePoint, cornerWorldPosition) <= hypotenuse;
        }

        private Vector2 GetLocalCorner(int cornerIndex)
        {
            Vector2 point = boxInfo.size / 2f;
            if (cornerIndex == 1)
            {
                point.y = -point.y;
            }
            else if (cornerIndex == 2)
            {
                point.x = -point.x;
                point.y = -point.y;
            }
            else if (cornerIndex == 3)
            {
                point.x = -point.x;
            }
            return point;
        }

        private Vector2 GetWorldCorner(int cornerIndex)
        {
            return LocalToWorldMatrix.MultiplyPoint3x4(GetLocalCorner(cornerIndex));
        }
    }
}