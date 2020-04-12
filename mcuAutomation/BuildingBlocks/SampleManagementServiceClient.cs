﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.237
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://www.avertlabs.com/", ConfigurationName = "McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.SampleManagementServiceSoap")]
    public interface SampleManagementServiceSoap
    {

        // CODEGEN: Generating message contract since element name request from namespace http://www.avertlabs.com/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action = "http://www.avertlabs.com/ArchiveSamples", ReplyAction = "*")]
        McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.ArchiveSamplesResponse ArchiveSamples(McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.ArchiveSamplesRequest request);

        // CODEGEN: Generating message contract since element name request from namespace http://www.avertlabs.com/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action = "http://www.avertlabs.com/RetrieveSamples", ReplyAction = "*")]
        McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesResponse RetrieveSamples(McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesRequest request);
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class ArchiveSamplesRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "ArchiveSamples", Namespace = "http://www.avertlabs.com/", Order = 0)]
        public McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.ArchiveSamplesRequestBody Body;

        public ArchiveSamplesRequest()
        {
        }

        public ArchiveSamplesRequest(McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.ArchiveSamplesRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://www.avertlabs.com/")]
    public partial class ArchiveSamplesRequestBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string request;

        public ArchiveSamplesRequestBody()
        {
        }

        public ArchiveSamplesRequestBody(string request)
        {
            this.request = request;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class ArchiveSamplesResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "ArchiveSamplesResponse", Namespace = "http://www.avertlabs.com/", Order = 0)]
        public McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.ArchiveSamplesResponseBody Body;

        public ArchiveSamplesResponse()
        {
        }

        public ArchiveSamplesResponse(McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.ArchiveSamplesResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://www.avertlabs.com/")]
    public partial class ArchiveSamplesResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string ArchiveSamplesResult;

        public ArchiveSamplesResponseBody()
        {
        }

        public ArchiveSamplesResponseBody(string ArchiveSamplesResult)
        {
            this.ArchiveSamplesResult = ArchiveSamplesResult;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class RetrieveSamplesRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "RetrieveSamples", Namespace = "http://www.avertlabs.com/", Order = 0)]
        public McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesRequestBody Body;

        public RetrieveSamplesRequest()
        {
        }

        public RetrieveSamplesRequest(McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://www.avertlabs.com/")]
    public partial class RetrieveSamplesRequestBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string request;

        public RetrieveSamplesRequestBody()
        {
        }

        public RetrieveSamplesRequestBody(string request)
        {
            this.request = request;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class RetrieveSamplesResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "RetrieveSamplesResponse", Namespace = "http://www.avertlabs.com/", Order = 0)]
        public McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesResponseBody Body;

        public RetrieveSamplesResponse()
        {
        }

        public RetrieveSamplesResponse(McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://www.avertlabs.com/")]
    public partial class RetrieveSamplesResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string RetrieveSamplesResult;

        public RetrieveSamplesResponseBody()
        {
        }

        public RetrieveSamplesResponseBody(string RetrieveSamplesResult)
        {
            this.RetrieveSamplesResult = RetrieveSamplesResult;
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface SampleManagementServiceSoapChannel : McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.SampleManagementServiceSoap, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SampleManagementServiceSoapClient : System.ServiceModel.ClientBase<McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.SampleManagementServiceSoap>, McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.SampleManagementServiceSoap
    {

        public SampleManagementServiceSoapClient()
        {
        }

        public SampleManagementServiceSoapClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public SampleManagementServiceSoapClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public SampleManagementServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public SampleManagementServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.ArchiveSamplesResponse McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.SampleManagementServiceSoap.ArchiveSamples(McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.ArchiveSamplesRequest request)
        {
            return base.Channel.ArchiveSamples(request);
        }

        public string ArchiveSamples(string request)
        {
            McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.ArchiveSamplesRequest inValue = new McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.ArchiveSamplesRequest();
            inValue.Body = new McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.ArchiveSamplesRequestBody();
            inValue.Body.request = request;
            McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.ArchiveSamplesResponse retVal = ((McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.SampleManagementServiceSoap)(this)).ArchiveSamples(inValue);
            return retVal.Body.ArchiveSamplesResult;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesResponse McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.SampleManagementServiceSoap.RetrieveSamples(McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesRequest request)
        {
            return base.Channel.RetrieveSamples(request);
        }

        public string RetrieveSamples(string request)
        {
            McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesRequest inValue = new McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesRequest();
            inValue.Body = new McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesRequestBody();
            inValue.Body.request = request;
            McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.RetrieveSamplesResponse retVal = ((McAfeeLabs.Engineering.Automation.Base.SampleManagementServiceClient.SampleManagementServiceSoap)(this)).RetrieveSamples(inValue);
            return retVal.Body.RetrieveSamplesResult;
        }
    }
}
