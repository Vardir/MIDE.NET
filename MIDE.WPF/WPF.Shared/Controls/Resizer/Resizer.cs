﻿/****************************** Module Header ******************************\
Module Name:  Resizer.cs
Project:      RuntimeResizablePanel
Copyright (c) Microsoft Corporation.

Resizer is a class to represent the Thumb used for resizing.
This helps us to object for each Thumb direction and resize only sides where the change is made.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;


namespace MIDE.WPF.Controls
{
    /// <summary>
    /// Class to represent the Thumb used for resizing the panel.
    /// </summary>
    public class Resizer : Thumb
    {
        /// <summary>
        /// Direction to resize.
        /// </summary>
        public static DependencyProperty ThumbDirectionProperty = 
            DependencyProperty.Register("ThumbDirection", typeof(ResizeDirections), typeof(Resizer));

        public ResizeDirections ThumbDirection
        {
            get => (ResizeDirections)GetValue(ThumbDirectionProperty);
            set => SetValue(ThumbDirectionProperty, value);
        }

        static Resizer()
        {
            // This will allow us to create a Style with target type Resizer.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Resizer), new FrameworkPropertyMetadata(typeof(Resizer)));
        }

        public Resizer()
        {
            DragDelta += new DragDeltaEventHandler(Resizer_DragDelta);

        }

        void Resizer_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (DataContext is Control designerItem)
            {
                double deltaVertical, deltaHorizontal;

                switch (ThumbDirection)
                {

                    case ResizeDirections.TopLeft:
                        deltaVertical = ResizeTop(e, designerItem);
                        deltaHorizontal = ResizeLeft(e, designerItem);
                        break;
                    case ResizeDirections.Left:
                        deltaHorizontal = ResizeLeft(e, designerItem);
                        break;
                    case ResizeDirections.BottomLeft:
                        deltaVertical = ResizeBottom(e, designerItem);
                        deltaHorizontal = ResizeLeft(e, designerItem);
                        break;
                    case ResizeDirections.Bottom:
                        deltaVertical = ResizeBottom(e, designerItem);
                        break;
                    case ResizeDirections.BottomRight:
                        deltaVertical = ResizeBottom(e, designerItem);
                        deltaHorizontal = ResizeRight(e, designerItem);
                        break;
                    case ResizeDirections.Right:
                        deltaHorizontal = ResizeRight(e, designerItem);
                        break;
                    case ResizeDirections.TopRight:
                        deltaVertical = ResizeTop(e, designerItem);
                        deltaHorizontal = ResizeRight(e, designerItem);
                        break;
                    case ResizeDirections.Top:
                        deltaVertical = ResizeTop(e, designerItem);
                        break;
                    default:
                        break;
                }

            }

            e.Handled = true;
        }

        private static double ResizeRight(DragDeltaEventArgs e, Control designerItem)
        {
            double deltaHorizontal;
            deltaHorizontal = Math.Min(-e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);            
            designerItem.Width = designerItem.ActualWidth - deltaHorizontal;
            return deltaHorizontal;
        }

        private static double ResizeTop(DragDeltaEventArgs e, Control designerItem)
        {
            double deltaVertical;
            deltaVertical = Math.Min(e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
            designerItem.Height -= deltaVertical;
            return deltaVertical;
        }

        private static double ResizeLeft(DragDeltaEventArgs e, Control designerItem)
        {
            double deltaHorizontal;
            deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
            designerItem.Width -= deltaHorizontal;
            return deltaHorizontal;
        }

        private static double ResizeBottom(DragDeltaEventArgs e, Control designerItem)
        {
            double deltaVertical;
            deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
            designerItem.Height -= deltaVertical;
            return deltaVertical;
        }

    }

    /// <summary>
    /// Enum to maintain the direction user can resize.
    /// </summary>
    public enum ResizeDirections
    {
        TopLeft =0,
        Left,
        BottomLeft,
        Bottom,
        BottomRight,
        Right,
        TopRight,
        Top
    }
}