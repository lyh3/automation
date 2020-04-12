using System;
namespace WindowService.DataModel
{
    [Serializable]
    public class ModelPackage
    {
        public MicrocodeReleaseModel modelRequest { get; set; }
        public WorkflowConfigModel modelConfig { get; set; }
    }
}