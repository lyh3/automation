﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace McAfee.Automation.WCFAuthenticationProxy.AuthenticationService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AuthorizationRequestData", Namespace="http://schemas.datacontract.org/2004/07/WCFAuthenticationService")]
    [System.SerializableAttribute()]
    public partial class AuthorizationRequestData : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DomainField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LoginNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private AccessPrivilege.AccessPrivilegeAttribute[] PrivilegeListField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TokenField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Domain {
            get {
                return this.DomainField;
            }
            set {
                if ((object.ReferenceEquals(this.DomainField, value) != true)) {
                    this.DomainField = value;
                    this.RaisePropertyChanged("Domain");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LoginName {
            get {
                return this.LoginNameField;
            }
            set {
                if ((object.ReferenceEquals(this.LoginNameField, value) != true)) {
                    this.LoginNameField = value;
                    this.RaisePropertyChanged("LoginName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public AccessPrivilege.AccessPrivilegeAttribute[] PrivilegeList {
            get {
                return this.PrivilegeListField;
            }
            set {
                if ((object.ReferenceEquals(this.PrivilegeListField, value) != true)) {
                    this.PrivilegeListField = value;
                    this.RaisePropertyChanged("PrivilegeList");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Token {
            get {
                return this.TokenField;
            }
            set {
                if ((object.ReferenceEquals(this.TokenField, value) != true)) {
                    this.TokenField = value;
                    this.RaisePropertyChanged("Token");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AuthorizationResults", Namespace="http://schemas.datacontract.org/2004/07/WCFAuthenticationService")]
    [System.SerializableAttribute()]
    public partial class AuthorizationResults : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<bool> AcknowledgedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrormessageField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<bool> Acknowledged {
            get {
                return this.AcknowledgedField;
            }
            set {
                if ((this.AcknowledgedField.Equals(value) != true)) {
                    this.AcknowledgedField = value;
                    this.RaisePropertyChanged("Acknowledged");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Errormessage {
            get {
                return this.ErrormessageField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrormessageField, value) != true)) {
                    this.ErrormessageField = value;
                    this.RaisePropertyChanged("Errormessage");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AuthenticationService.IAuthentication")]
    public interface IAuthentication {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/Ping", ReplyAction="http://tempuri.org/IAuthentication/PingResponse")]
        string Ping();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/AuthorizationRequest", ReplyAction="http://tempuri.org/IAuthentication/AuthorizationRequestResponse")]
        McAfee.Automation.WCFAuthenticationProxy.AuthenticationService.AuthorizationResults AuthorizationRequest(McAfee.Automation.WCFAuthenticationProxy.AuthenticationService.AuthorizationRequestData requestData);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAuthenticationChannel : McAfee.Automation.WCFAuthenticationProxy.AuthenticationService.IAuthentication, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AuthenticationClient : System.ServiceModel.ClientBase<McAfee.Automation.WCFAuthenticationProxy.AuthenticationService.IAuthentication>, McAfee.Automation.WCFAuthenticationProxy.AuthenticationService.IAuthentication {
        
        public AuthenticationClient() {
        }
        
        public AuthenticationClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AuthenticationClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AuthenticationClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AuthenticationClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string Ping() {
            return base.Channel.Ping();
        }
        
        public McAfee.Automation.WCFAuthenticationProxy.AuthenticationService.AuthorizationResults AuthorizationRequest(McAfee.Automation.WCFAuthenticationProxy.AuthenticationService.AuthorizationRequestData requestData) {
            return base.Channel.AuthorizationRequest(requestData);
        }
    }
}
