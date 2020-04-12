using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;

namespace McAfee.Service.DataModel
{
    public class ConcreteMockMcAfeeServiceProcessB : MockMcAfeeServiceProcess
    {   
        #region Constructors

        public ConcreteMockMcAfeeServiceProcessB() { }
        public ConcreteMockMcAfeeServiceProcessB(ILog logger = null)
            : base(logger)
        {
        }

        #endregion

        #region Protected Methods

        override protected void InitializeWorkerProcess()
        {
            _workerList.Add(new ConcreteWorkerThreadC(_logger));
            _workerList.Add(new ConcreteWorkerThreadD(_logger));
        }

        #endregion
    }
}
