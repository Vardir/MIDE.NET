﻿using System;

namespace NodeGraphs.DataModels
{
    [Flags]
    public enum GridSide
    {
        None = 0,
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8
    }
}