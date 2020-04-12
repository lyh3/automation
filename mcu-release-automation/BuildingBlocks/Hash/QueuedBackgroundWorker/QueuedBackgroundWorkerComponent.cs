using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace  McAfeeLabs.Engineering.Automation.Base
{
    public partial class QueuedBackgroundWorkeComponent : Component
    {
        QueuedBackgroundWorker _queuedBackgroundWorker = null;

        public QueuedBackgroundWorkeComponent()
        {
            InitializeComponent();
            _Init();
        }

        public QueuedBackgroundWorkeComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            _Init();
        }

        public QueuedBackgroundWorker _QueuedBackgroundWorker
        {
            get { return _queuedBackgroundWorker; }
        }

        public event RunWorkerCompletedEventHandler RunWorkerCompleted
        {
            add
            {
                lock (_queuedBackgroundWorker.OperationCompletedLock)//_operationCompletedLock)
                {
                    _queuedBackgroundWorker.OperationCompleted += value;// _operationCompleted += value;
                }
            }

            remove
            {
                lock (_queuedBackgroundWorker.OperationCompletedLock)//_operationCompletedLock)
                {
                    _queuedBackgroundWorker.OperationCompleted -= value;// _operationCompleted -= value;
                }
            }
        }

        public event DoWorkEventHandler DoWork
        {
            add
            {
                lock (_queuedBackgroundWorker.DoWorkLock)//_doWorkLock)
                {
                    _queuedBackgroundWorker.DoWork += value;// _doWork += value;
                }
            }

            remove
            {
                lock (_queuedBackgroundWorker.DoWorkLock)
                {
                    _queuedBackgroundWorker.DoWork -= value;
                }
            }
        }

        public event ProgressChangedEventHandler ProgressChanged
        {
            add
            {
                lock (_queuedBackgroundWorker.OperationProgressChangedLock)//_operationProgressChangedLock)
                {
                    _queuedBackgroundWorker.OperationProgressChanged += value;// _operationProgressChanged += value;
                }
            }

            remove
            {
                lock (_queuedBackgroundWorker.OperationProgressChangedLock)//_operationProgressChangedLock)
                {
                    _queuedBackgroundWorker.OperationProgressChanged -= value;// _operationProgressChanged -= value;
                }
            }
        }

        private void _Init()
        {
            _queuedBackgroundWorker = new QueuedBackgroundWorker();
        }

    }
}
