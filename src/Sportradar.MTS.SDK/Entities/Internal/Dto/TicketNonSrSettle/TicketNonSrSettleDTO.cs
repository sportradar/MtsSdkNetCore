﻿/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */

//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v8.6.6263.34621 (http://NJsonSchema.org)
// </auto-generated>
//----------------------

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.TicketNonSrSettle
{
    #pragma warning disable // Disable all warnings

    /// <summary>Non-Sportradar settle version 2.4 schema</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "8.6.6263.34621")]
    internal partial class TicketNonSrSettleDTO : System.ComponentModel.INotifyPropertyChanged
    {
        private long _timestampUtc;
        private string _ticketId;
        private Sender _sender = new Sender();
        private long? _nonSrSettleStake;
        private string _version;
    
        /// <summary>Timestamp of non-Sportradar settle placement (in UNIX time millis)</summary>
        [Newtonsoft.Json.JsonProperty("timestampUtc", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Range(1.0, 9223372036854775807.0)]
        public long TimestampUtc
        {
            get { return _timestampUtc; }
            set 
            {
                if (_timestampUtc != value)
                {
                    _timestampUtc = value; 
                    RaisePropertyChanged();
                }
            }
        }
    
        /// <summary>Ticket id to settle</summary>
        [Newtonsoft.Json.JsonProperty("ticketId", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(50, MinimumLength = 1)]
        public string TicketId
        {
            get { return _ticketId; }
            set 
            {
                if (_ticketId != value)
                {
                    _ticketId = value; 
                    RaisePropertyChanged();
                }
            }
        }
    
        /// <summary>Identification and settings of the settle sender</summary>
        [Newtonsoft.Json.JsonProperty("sender", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public Sender Sender
        {
            get { return _sender; }
            set 
            {
                if (_sender != value)
                {
                    _sender = value; 
                    RaisePropertyChanged();
                }
            }
        }
    
        /// <summary>Non-Sportradar settle stake in same currency as original ticket. Quantity multiplied by 10_000 and rounded to a long value. Applicable only if performing full cashout.</summary>
        [Newtonsoft.Json.JsonProperty("nonSrSettleStake", Required = Newtonsoft.Json.Required.AllowNull)]
        [System.ComponentModel.DataAnnotations.Range(0.0, 1000000000000000000.0)]
        public long? NonSrSettleStake
        {
            get { return _nonSrSettleStake; }
            set 
            {
                if (_nonSrSettleStake != value)
                {
                    _nonSrSettleStake = value; 
                    RaisePropertyChanged();
                }
            }
        }
    
        /// <summary>JSON format version (must be '2.4')</summary>
        [Newtonsoft.Json.JsonProperty("version", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(3, MinimumLength = 3)]
        [System.ComponentModel.DataAnnotations.RegularExpression(@"^(2\.4)$")]
        public string Version
        {
            get { return _version; }
            set 
            {
                if (_version != value)
                {
                    _version = value; 
                    RaisePropertyChanged();
                }
            }
        }
    
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static TicketNonSrSettleDTO FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TicketNonSrSettleDTO>(data);
        }
    
        protected virtual void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "8.6.6263.34621")]
    internal partial class Sender : System.ComponentModel.INotifyPropertyChanged
    {
        private int _bookmakerId;
    
        /// <summary>Client's id (provided by Sportradar to the client)</summary>
        [Newtonsoft.Json.JsonProperty("bookmakerId", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Range(1.0, 2147483647.0)]
        public int BookmakerId
        {
            get { return _bookmakerId; }
            set 
            {
                if (_bookmakerId != value)
                {
                    _bookmakerId = value; 
                    RaisePropertyChanged();
                }
            }
        }
    
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static Sender FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Sender>(data);
        }
    
        protected virtual void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}