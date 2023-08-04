/* FlexibleGridLayout.cs
 * From: Game Dev Guide - Fixing Grid Layouts in Unity With a Flexible Grid Component
 * Created: June 2020, NowWeWake
 */

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            Uniform,
            Width,
            Height,
            FixedRows,
            FixedColumns
        }

        [Header("Flexible Grid")]
        public FitType fitType = FitType.Uniform;

        public int rows;
        public int columns;
        public Vector2 cellSize;
        public Vector2 spacing;

        public bool fitX;
        public bool fitY;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
            {
                float squareRoot = Mathf.Sqrt(transform.childCount);
                rows = columns = Mathf.CeilToInt(squareRoot);
                switch (fitType)
                {
                    case FitType.Width:
                        fitX = true;
                        fitY = false;
                        break;
                    case FitType.Height:
                        fitX = false;
                        fitY = true;
                        break;
                    case FitType.Uniform:
                        fitX = fitY = true;
                        break;
                }
            }

            int childrenCount = rectChildren.Count(child => child.gameObject.activeSelf);
            if (fitType == FitType.Width || fitType == FitType.FixedColumns)
            {
                rows = Mathf.CeilToInt(childrenCount / (float)columns);
            }

            if (fitType == FitType.Height || fitType == FitType.FixedRows)
            {
                columns = Mathf.CeilToInt(childrenCount / (float)rows);
            }


            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            float cellWidth = parentWidth / columns - spacing.x / (columns / ((float)columns - 1))
                                                    - padding.left / (float)columns - padding.right / (float)columns;
            float cellHeight = parentHeight / rows - spacing.y / (rows / ((float)rows - 1))
                                                   - padding.top / (float)rows - padding.bottom / (float)rows;

            cellSize.x = fitX ? cellWidth : cellSize.x;
            cellSize.y = fitY ? cellHeight : cellSize.y;

            for (int i = 0; i < childrenCount; i++)
            {
                int rowCount = i / columns;
                int columnCount = i % columns;

                if (fitType == FitType.Width || fitType == FitType.Height)
                {
                    int rest = rows * columns - i;
                    if (i == childrenCount - 1 && rest > 0)
                    {
                        cellSize.x *= rest;
                        columnCount /= rest;
                    }
                }

                RectTransform item = rectChildren[i];

                float xPos = cellSize.x * columnCount + spacing.x * columnCount + padding.left;
                float yPos = cellSize.y * rowCount + spacing.y * rowCount + padding.top;

                Vector2 sizeDelta = rectTransform.sizeDelta;
                switch (m_ChildAlignment)
                {
                    case TextAnchor.UpperLeft:
                        break;
                    case TextAnchor.UpperCenter:
                        xPos += 0.5f * (sizeDelta.x +
                                        (spacing.x + padding.left + padding.left) -
                                        columns * (cellSize.x + spacing.x + padding.left)); //Center xPos
                        break;
                    case TextAnchor.UpperRight:
                        xPos = -xPos + sizeDelta.x -
                               cellSize.x; //Flip xPos to go bottom-up
                        break;
                    case TextAnchor.MiddleLeft:
                        yPos += 0.5f * (sizeDelta.y +
                                        (spacing.y + padding.top + padding.top) -
                                        rows * (cellSize.y + spacing.y + padding.top)); //Center yPos
                        break;
                    case TextAnchor.MiddleCenter:
                        xPos += 0.5f * (sizeDelta.x +
                                        (spacing.x + padding.left + padding.left) -
                                        columns * (cellSize.x + spacing.x + padding.left)); //Center xPos
                        yPos += 0.5f * (sizeDelta.y +
                                        (spacing.y + padding.top + padding.top) -
                                        rows * (cellSize.y + spacing.y + padding.top)); //Center yPos
                        break;
                    case TextAnchor.MiddleRight:
                        xPos = -xPos + sizeDelta.x -
                               cellSize.x; //Flip xPos to go bottom-up
                        yPos += 0.5f * (sizeDelta.y +
                                        (spacing.y + padding.top + padding.top) -
                                        rows * (cellSize.y + spacing.y + padding.top)); //Center yPos
                        break;
                    case TextAnchor.LowerLeft:
                        yPos = -yPos + sizeDelta.y -
                               cellSize.y; //Flip yPos to go Right to Left
                        break;
                    case TextAnchor.LowerCenter:
                        xPos += 0.5f * (sizeDelta.x +
                                        (spacing.x + padding.left + padding.left) -
                                        columns * (cellSize.x + spacing.x + padding.left)); //Center xPos
                        yPos = -yPos + sizeDelta.y -
                               cellSize.y; //Flip yPos to go Right to Left
                        break;
                    case TextAnchor.LowerRight:
                        xPos = -xPos + sizeDelta.x -
                               cellSize.x; //Flip xPos to go bottom-up
                        yPos = -yPos + sizeDelta.y -
                               cellSize.y; //Flip yPos to go Right to Left
                        break;
                }

                if (item.GetComponent<ContentSizeFitter>() != null)
                {
                    // float totalMinWidth = childrenCount * (cellSize.x + spacing.x) + padding.left + padding.right;
                    // float totalMinHeight = childrenCount * (cellSize.y + spacing.y) + padding.top + padding.bottom;
                    //
                    // SetLayoutInputForAxis(totalMinWidth, -1, -1, 0);
                    // SetLayoutInputForAxis(totalMinHeight, -1, -1, 1);

                    SetChildAlongAxis(item, 0, xPos, cellSize.x);
                    SetChildAlongAxis(item, 1, yPos, cellSize.y);
                }
                else
                {
                    SetChildAlongAxis(item, 0, xPos, cellSize.x);
                    SetChildAlongAxis(item, 1, yPos, cellSize.y);
                }
            }
        }

        public override void CalculateLayoutInputVertical()
        {
            //throw new System.NotImplementedException();
        }

        public override void SetLayoutHorizontal()
        {
            //throw new System.NotImplementedException();
        }

        public override void SetLayoutVertical()
        {
            //throw new System.NotImplementedException();
        }
    }
}