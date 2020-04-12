using System;

using McAfee.Service;
using McAfee.Service.DataModel;

namespace Windows.Service
{
    public partial class WindowsServiceMock : McAfeeServiceConfigThreadModel
    {
        #region Constructors

        public WindowsServiceMock(String serviceName, string serviceDescription)
            : base(serviceName, serviceDescription)
        {
        }

        #endregion

        public override void StopService()
        {
        }

        override protected void InitializeWorkerThreads()
        {
            _serviceThreadProcess.AddRange(new[] {(IMcAfeeServiceProcess)new ConcreteMockMcAfeeServiceProcessA(_logger), 
                                                    (IMcAfeeServiceProcess)new ConcreteMockMcAfeeServiceProcessB(_logger)});
            StartThreadProcess();
        }
    }
}
