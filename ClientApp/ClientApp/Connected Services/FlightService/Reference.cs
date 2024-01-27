﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClientApp.FlightService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Flight", Namespace="http://schemas.datacontract.org/2004/07/_00010358_Service")]
    [System.SerializableAttribute()]
    public partial class Flight : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime DepartureTimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DestinationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FlightCodeField;
        
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
        public System.DateTime DepartureTime {
            get {
                return this.DepartureTimeField;
            }
            set {
                if ((this.DepartureTimeField.Equals(value) != true)) {
                    this.DepartureTimeField = value;
                    this.RaisePropertyChanged("DepartureTime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Destination {
            get {
                return this.DestinationField;
            }
            set {
                if ((object.ReferenceEquals(this.DestinationField, value) != true)) {
                    this.DestinationField = value;
                    this.RaisePropertyChanged("Destination");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FlightCode {
            get {
                return this.FlightCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.FlightCodeField, value) != true)) {
                    this.FlightCodeField = value;
                    this.RaisePropertyChanged("FlightCode");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="FlightService.IAirportService")]
    public interface IAirportService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAirportService/InsertFlight", ReplyAction="http://tempuri.org/IAirportService/InsertFlightResponse")]
        bool InsertFlight(ClientApp.FlightService.Flight flight);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAirportService/InsertFlight", ReplyAction="http://tempuri.org/IAirportService/InsertFlightResponse")]
        System.Threading.Tasks.Task<bool> InsertFlightAsync(ClientApp.FlightService.Flight flight);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAirportServiceChannel : ClientApp.FlightService.IAirportService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AirportServiceClient : System.ServiceModel.ClientBase<ClientApp.FlightService.IAirportService>, ClientApp.FlightService.IAirportService {
        
        public AirportServiceClient() {
        }
        
        public AirportServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AirportServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AirportServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AirportServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool InsertFlight(ClientApp.FlightService.Flight flight) {
            return base.Channel.InsertFlight(flight);
        }
        
        public System.Threading.Tasks.Task<bool> InsertFlightAsync(ClientApp.FlightService.Flight flight) {
            return base.Channel.InsertFlightAsync(flight);
        }
    }
}
