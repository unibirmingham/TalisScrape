﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.0.30319.33440.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(TypeName = "OAI-PMHtype", Namespace = "http://www.openarchives.org/OAI/2.0/")]
[System.Xml.Serialization.XmlRootAttribute("OAI-PMH", Namespace = "http://www.openarchives.org/OAI/2.0/", IsNullable = false)]
public partial class OAIPMHtype
{

    private System.DateTime responseDateField;

    private requestType requestField;

    private object[] itemsField;

    /// <remarks/>
    public System.DateTime responseDate
    {
        get
        {
            return this.responseDateField;
        }
        set
        {
            this.responseDateField = value;
        }
    }

    /// <remarks/>
    public requestType request
    {
        get
        {
            return this.requestField;
        }
        set
        {
            this.requestField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("GetRecord", typeof(GetRecordType))]
    [System.Xml.Serialization.XmlElementAttribute("Identify", typeof(IdentifyType))]
    [System.Xml.Serialization.XmlElementAttribute("ListIdentifiers", typeof(ListIdentifiersType))]
    [System.Xml.Serialization.XmlElementAttribute("ListMetadataFormats", typeof(ListMetadataFormatsType))]
    [System.Xml.Serialization.XmlElementAttribute("ListRecords", typeof(ListRecords))]
    [System.Xml.Serialization.XmlElementAttribute("ListSets", typeof(ListSetsType))]
    [System.Xml.Serialization.XmlElementAttribute("error", typeof(OAIPMHerrorType))]
    public object[] Items
    {
        get
        {
            return this.itemsField;
        }
        set
        {
            this.itemsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class requestType
{

    private verbType verbField;

    private bool verbFieldSpecified;

    private string identifierField;

    private string metadataPrefixField;

    private string fromField;

    private string untilField;

    private string setField;

    private string resumptionTokenField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public verbType verb
    {
        get
        {
            return this.verbField;
        }
        set
        {
            this.verbField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool verbSpecified
    {
        get
        {
            return this.verbFieldSpecified;
        }
        set
        {
            this.verbFieldSpecified = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]
    public string identifier
    {
        get
        {
            return this.identifierField;
        }
        set
        {
            this.identifierField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string metadataPrefix
    {
        get
        {
            return this.metadataPrefixField;
        }
        set
        {
            this.metadataPrefixField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string from
    {
        get
        {
            return this.fromField;
        }
        set
        {
            this.fromField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string until
    {
        get
        {
            return this.untilField;
        }
        set
        {
            this.untilField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string set
    {
        get
        {
            return this.setField;
        }
        set
        {
            this.setField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string resumptionToken
    {
        get
        {
            return this.resumptionTokenField;
        }
        set
        {
            this.resumptionTokenField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute(DataType = "anyURI")]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public enum verbType
{

    /// <remarks/>
    Identify,

    /// <remarks/>
    ListMetadataFormats,

    /// <remarks/>
    ListSets,

    /// <remarks/>
    GetRecord,

    /// <remarks/>
    ListIdentifiers,

    /// <remarks/>
    ListRecords,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class ListRecords
{

    private recordType[] recordField;

    private resumptionTokenType resumptionTokenField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("record")]
    public recordType[] record
    {
        get
        {
            return this.recordField;
        }
        set
        {
            this.recordField = value;
        }
    }

    /// <remarks/>
    public resumptionTokenType resumptionToken
    {
        get
        {
            return this.resumptionTokenField;
        }
        set
        {
            this.resumptionTokenField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class recordType
{

    private headerType headerField;

    private System.Xml.XmlElement metadataField;

    private System.Xml.XmlElement[] aboutField;

    /// <remarks/>
    public headerType header
    {
        get
        {
            return this.headerField;
        }
        set
        {
            this.headerField = value;
        }
    }

    /// <remarks/>
    public System.Xml.XmlElement metadata
    {
        get
        {
            return this.metadataField;
        }
        set
        {
            this.metadataField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("about")]
    public System.Xml.XmlElement[] about
    {
        get
        {
            return this.aboutField;
        }
        set
        {
            this.aboutField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class headerType
{

    private string identifierField;

    private string datestampField;

    private string[] setSpecField;

    private statusType statusField;

    private bool statusFieldSpecified;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "anyURI")]
    public string identifier
    {
        get
        {
            return this.identifierField;
        }
        set
        {
            this.identifierField = value;
        }
    }

    /// <remarks/>
    public string datestamp
    {
        get
        {
            return this.datestampField;
        }
        set
        {
            this.datestampField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("setSpec")]
    public string[] setSpec
    {
        get
        {
            return this.setSpecField;
        }
        set
        {
            this.setSpecField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public statusType status
    {
        get
        {
            return this.statusField;
        }
        set
        {
            this.statusField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool statusSpecified
    {
        get
        {
            return this.statusFieldSpecified;
        }
        set
        {
            this.statusFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public enum statusType
{

    /// <remarks/>
    deleted,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class resumptionTokenType
{

    private System.DateTime expirationDateField;

    private bool expirationDateFieldSpecified;

    private string completeListSizeField;

    private string cursorField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime expirationDate
    {
        get
        {
            return this.expirationDateField;
        }
        set
        {
            this.expirationDateField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool expirationDateSpecified
    {
        get
        {
            return this.expirationDateFieldSpecified;
        }
        set
        {
            this.expirationDateFieldSpecified = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "positiveInteger")]
    public string completeListSize
    {
        get
        {
            return this.completeListSizeField;
        }
        set
        {
            this.completeListSizeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "nonNegativeInteger")]
    public string cursor
    {
        get
        {
            return this.cursorField;
        }
        set
        {
            this.cursorField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class ListIdentifiersType
{

    private headerType[] headerField;

    private resumptionTokenType resumptionTokenField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("header")]
    public headerType[] header
    {
        get
        {
            return this.headerField;
        }
        set
        {
            this.headerField = value;
        }
    }

    /// <remarks/>
    public resumptionTokenType resumptionToken
    {
        get
        {
            return this.resumptionTokenField;
        }
        set
        {
            this.resumptionTokenField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class GetRecordType
{

    private recordType recordField;

    /// <remarks/>
    public recordType record
    {
        get
        {
            return this.recordField;
        }
        set
        {
            this.recordField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class setType
{

    private string setSpecField;

    private string setNameField;

    private System.Xml.XmlElement[] setDescriptionField;

    /// <remarks/>
    public string setSpec
    {
        get
        {
            return this.setSpecField;
        }
        set
        {
            this.setSpecField = value;
        }
    }

    /// <remarks/>
    public string setName
    {
        get
        {
            return this.setNameField;
        }
        set
        {
            this.setNameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("setDescription")]
    public System.Xml.XmlElement[] setDescription
    {
        get
        {
            return this.setDescriptionField;
        }
        set
        {
            this.setDescriptionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class ListSetsType
{

    private setType[] setField;

    private resumptionTokenType resumptionTokenField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("set")]
    public setType[] set
    {
        get
        {
            return this.setField;
        }
        set
        {
            this.setField = value;
        }
    }

    /// <remarks/>
    public resumptionTokenType resumptionToken
    {
        get
        {
            return this.resumptionTokenField;
        }
        set
        {
            this.resumptionTokenField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class ListMetadataFormatsType
{

    private metadataFormatType[] metadataFormatField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("metadataFormat")]
    public metadataFormatType[] metadataFormat
    {
        get
        {
            return this.metadataFormatField;
        }
        set
        {
            this.metadataFormatField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class metadataFormatType
{

    private string metadataPrefixField;

    private string schemaField;

    private string metadataNamespaceField;

    /// <remarks/>
    public string metadataPrefix
    {
        get
        {
            return this.metadataPrefixField;
        }
        set
        {
            this.metadataPrefixField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "anyURI")]
    public string schema
    {
        get
        {
            return this.schemaField;
        }
        set
        {
            this.schemaField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "anyURI")]
    public string metadataNamespace
    {
        get
        {
            return this.metadataNamespaceField;
        }
        set
        {
            this.metadataNamespaceField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class IdentifyType
{

    private string repositoryNameField;

    private string baseURLField;

    private protocolVersionType protocolVersionField;

    private string[] adminEmailField;

    private string earliestDatestampField;

    private deletedRecordType deletedRecordField;

    private granularityType granularityField;

    private string[] compressionField;

    private System.Xml.XmlElement[] descriptionField;

    /// <remarks/>
    public string repositoryName
    {
        get
        {
            return this.repositoryNameField;
        }
        set
        {
            this.repositoryNameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "anyURI")]
    public string baseURL
    {
        get
        {
            return this.baseURLField;
        }
        set
        {
            this.baseURLField = value;
        }
    }

    /// <remarks/>
    public protocolVersionType protocolVersion
    {
        get
        {
            return this.protocolVersionField;
        }
        set
        {
            this.protocolVersionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("adminEmail")]
    public string[] adminEmail
    {
        get
        {
            return this.adminEmailField;
        }
        set
        {
            this.adminEmailField = value;
        }
    }

    /// <remarks/>
    public string earliestDatestamp
    {
        get
        {
            return this.earliestDatestampField;
        }
        set
        {
            this.earliestDatestampField = value;
        }
    }

    /// <remarks/>
    public deletedRecordType deletedRecord
    {
        get
        {
            return this.deletedRecordField;
        }
        set
        {
            this.deletedRecordField = value;
        }
    }

    /// <remarks/>
    public granularityType granularity
    {
        get
        {
            return this.granularityField;
        }
        set
        {
            this.granularityField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("compression")]
    public string[] compression
    {
        get
        {
            return this.compressionField;
        }
        set
        {
            this.compressionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("description")]
    public System.Xml.XmlElement[] description
    {
        get
        {
            return this.descriptionField;
        }
        set
        {
            this.descriptionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public enum protocolVersionType
{

    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("2.0")]
    Item20,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public enum deletedRecordType
{

    /// <remarks/>
    no,

    /// <remarks/>
    persistent,

    /// <remarks/>
    transient,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.openarchives.org/OAI/2.0/")]
public enum granularityType
{

    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("YYYY-MM-DD")]
    YYYYMMDD,

    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("YYYY-MM-DDThh:mm:ssZ")]
    YYYYMMDDThhmmssZ,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(TypeName = "OAI-PMHerrorType", Namespace = "http://www.openarchives.org/OAI/2.0/")]
public partial class OAIPMHerrorType
{

    private OAIPMHerrorcodeType codeField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public OAIPMHerrorcodeType code
    {
        get
        {
            return this.codeField;
        }
        set
        {
            this.codeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(TypeName = "OAI-PMHerrorcodeType", Namespace = "http://www.openarchives.org/OAI/2.0/")]
public enum OAIPMHerrorcodeType
{

    /// <remarks/>
    cannotDisseminateFormat,

    /// <remarks/>
    idDoesNotExist,

    /// <remarks/>
    badArgument,

    /// <remarks/>
    badVerb,

    /// <remarks/>
    noMetadataFormats,

    /// <remarks/>
    noRecordsMatch,

    /// <remarks/>
    badResumptionToken,

    /// <remarks/>
    noSetHierarchy,
}
