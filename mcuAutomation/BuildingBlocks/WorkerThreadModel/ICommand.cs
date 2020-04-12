﻿using System;

namespace McAfeeLabs.Engineering.Automation.WorkerThreadModel
{
    public interface ICommand
    {
        void Execute();
        bool Success { get; set; }
        event EventHandler StateComplete;
    }

    public interface CopyOfICommand
    {
        void Execute();
        bool Success { get; set; }
        event EventHandler StateComplete;
    }
}
