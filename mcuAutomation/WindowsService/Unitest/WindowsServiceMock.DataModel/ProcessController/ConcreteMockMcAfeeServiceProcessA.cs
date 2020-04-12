using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;

namespace McAfee.Service.DataModel
{
    public class ConcreteMockMcAfeeServiceProcessA : MockMcAfeeServiceProcess
    {
        #region Constructors

        public ConcreteMockMcAfeeServiceProcessA() { }
        public ConcreteMockMcAfeeServiceProcessA(ILog logger = null)
            : base(logger)
        {
        }

        #endregion

        #region Protected Methods

        override protected void InitializeWorkerProcess()
        {
            _workerList.Add(new ConcreteWorkerThreadA(_logger));
            _workerList.Add(new ConcreteWorkerThreadB(_logger));
        }

        #endregion
    }
}
