using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    // From: https://gist.github.com/eAi/c4c7fa9c35b8cb34d25f730bf70eaaf4#file-horizontalflowlayoutgroup-cs
    // And From: https://forum.unity.com/threads/flowlayoutgroup.296709/#post-7951756
    /// <summary>
    ///     Layout Group controller that arranges children in columns, fitting as many on a line until total height exceeds parent bounds
    /// </summary>
    public class HorizontalFlowLayoutGroup : LayoutGroup
    {
        public float Spacing;

        public bool ChildForceExpandWidth;
        public bool ChildForceExpandHeight;

        private float _layoutWidth;

        public override void CalculateLayoutInputVertical()
        {
            float minHeight = GetGreatestMinimumChildHeight() + padding.top + padding.bottom;

            SetLayoutInputForAxis(minHeight, -1, -1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            SetLayout(rectTransform.rect.height, 0, false);
        }

        public override void SetLayoutVertical()
        {
            SetLayout(rectTransform.rect.height, 1, false);
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            _layoutWidth = SetLayout(rectTransform.rect.height, 0, true);
        }

        protected bool IsCenterAlign =>
            childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.MiddleCenter ||
            childAlignment == TextAnchor.UpperCenter;

        protected bool IsRightAlign =>
            childAlignment == TextAnchor.LowerRight || childAlignment == TextAnchor.MiddleRight ||
            childAlignment == TextAnchor.UpperRight;

        protected bool IsMiddleAlign =>
            childAlignment == TextAnchor.MiddleLeft || childAlignment == TextAnchor.MiddleRight ||
            childAlignment == TextAnchor.MiddleCenter;

        protected bool IsLowerAlign =>
            childAlignment == TextAnchor.LowerLeft || childAlignment == TextAnchor.LowerRight ||
            childAlignment == TextAnchor.LowerCenter;

        /// <summary>
        ///     Holds the rects that will make up the current column being processed
        /// </summary>
        private readonly IList<RectTransform> _columnList = new List<RectTransform>();

        /// <summary>
        ///     Main layout method
        /// </summary>
        /// <param name="height">Height to calculate the layout with</param>
        /// <param name="axis">0 for horizontal axis, 1 for vertical</param>
        /// <param name="layoutInput">If true, sets the layout input for the axis. If false, sets child position for axis</param>
        public float SetLayout(float height, int axis, bool layoutInput)
        {
            float groupWidth = rectTransform.rect.width;

            // Height that is available after padding is subtracted
            float workingHeight = rectTransform.rect.height - padding.top - padding.bottom;

            // Accumulates the total width of the columns, including spacing and padding.
            float xOffset = IsRightAlign ? padding.right : (float)padding.left;

            float currentColumnWidth = 0f;
            float currentColumnHeight = 0f;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                // LowerAlign works from back to front
                int index = IsRightAlign ? rectChildren.Count - 1 - i : i;

                RectTransform child = rectChildren[index];

                float childWidth = LayoutUtility.GetPreferredSize(child, 0);
                float childHeight = LayoutUtility.GetPreferredSize(child, 1);

                // Max child width is layout group with - padding
                childHeight = Mathf.Min(childHeight, workingHeight);

                // Apply spacing if not the first element in a column
                if (_columnList.Count > 0)
                {
                    currentColumnHeight += Spacing;
                }

                // If adding this element would exceed the bounds of the column,
                // go to a new line after processing the current column
                if (currentColumnHeight + childHeight > workingHeight)
                {
                    // Undo spacing addition if we're moving to a new line (Spacing is not applied on edges)
                    currentColumnHeight -= Spacing;

                    // Process current column elements positioning
                    if (!layoutInput)
                    {
                        float w = CalculateColumnHorizontalOffset(groupWidth, xOffset, currentColumnWidth);
                        LayoutColumn(_columnList, currentColumnHeight, currentColumnWidth, workingHeight, padding.top,
                            w, axis);
                    }

                    // Clear existing column
                    _columnList.Clear();

                    // Add the current column width to total width accumulator, and reset to 0 for the next column
                    xOffset += currentColumnWidth;
                    xOffset += Spacing;

                    currentColumnWidth = 0;
                    currentColumnHeight = 0;
                }

                currentColumnHeight += childHeight;
                _columnList.Add(child);

                // We need the largest element height to determine the starting position of the next line
                if (childWidth > currentColumnWidth)
                {
                    currentColumnWidth = childWidth;
                }
            }

            if (!layoutInput)
            {
                float w = CalculateColumnHorizontalOffset(groupWidth, xOffset, currentColumnWidth);

                // Layout the final column
                LayoutColumn(_columnList, currentColumnHeight, currentColumnWidth, workingHeight, padding.top, w, axis);
            }

            _columnList.Clear();

            // Add the last column's width to the width accumulator
            xOffset += currentColumnWidth;
            xOffset += IsRightAlign ? padding.left : padding.right;

            if (layoutInput)
            {
                if (axis == 0)
                {
                    SetLayoutInputForAxis(xOffset, xOffset, -1, axis);
                }
            }

            return xOffset;
        }

        private float CalculateColumnHorizontalOffset(float groupWidth, float xOffset, float currentColumnWidth)
        {
            float h;

            if (IsLowerAlign)
            {
                h = groupWidth - xOffset - currentColumnWidth;
            }
            else if (IsMiddleAlign)
            {
                h = groupWidth * 0.5f - _layoutWidth * 0.5f + xOffset;
            }
            else
            {
                h = xOffset;
            }

            return h;
        }

        protected void LayoutColumn(IList<RectTransform> contents, float columnHeight, float columnWidth,
            float maxHeight, float yOffset, float xOffset, int axis)
        {
            float yPos = yOffset;

            if (!ChildForceExpandWidth && IsCenterAlign)
            {
                yPos += (maxHeight - columnHeight) * 0.5f;
            }
            else if (!ChildForceExpandWidth && IsRightAlign)
            {
                yPos += maxHeight - columnHeight;
            }

            float extraHeight = 0f;

            if (ChildForceExpandHeight)
            {
                int flexibleChildCount = 0;

                for (int i = 0; i < _columnList.Count; i++)
                {
                    if (LayoutUtility.GetFlexibleWidth(_columnList[i]) > 0f)
                    {
                        flexibleChildCount++;
                    }
                }

                if (flexibleChildCount > 0)
                {
                    extraHeight = (maxHeight - columnHeight) / flexibleChildCount;
                }
            }


            for (int j = 0; j < _columnList.Count; j++)
            {
                int index = IsRightAlign ? _columnList.Count - 1 - j : j;

                RectTransform columnChild = _columnList[index];

                float columnChildHeight = LayoutUtility.GetPreferredSize(columnChild, 1);

                if (LayoutUtility.GetFlexibleHeight(columnChild) > 0f)
                {
                    columnChildHeight += extraHeight;
                }

                float columnChildWidth = LayoutUtility.GetPreferredSize(columnChild, 0);

                if (ChildForceExpandWidth)
                {
                    columnChildWidth = columnWidth;
                }

                columnChildHeight = Mathf.Min(columnChildHeight, maxHeight);

                float xPos = xOffset;

                if (IsCenterAlign)
                {
                    xPos += (columnWidth - columnChildWidth) * 0.5f;
                }
                else if (IsLowerAlign)
                {
                    xPos += columnWidth - columnChildWidth;
                }

                if (axis == 1)
                {
                    SetChildAlongAxis(columnChild, 1, yPos, columnChildHeight);
                }
                else
                {
                    SetChildAlongAxis(columnChild, 0, xPos, columnChildWidth);
                }

                yPos += columnChildHeight + Spacing;
            }
        }

        public float GetGreatestMinimumChildHeight()
        {
            float max = 0f;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                float w = LayoutUtility.GetMinHeight(rectChildren[i]);

                max = Mathf.Max(w, max);
            }

            return max;
        }
    }
}