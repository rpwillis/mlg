﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#pragma warning disable 1591

namespace MLG2007.Helper.CalendarStore {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Design.TypedDataSetGenerator", "2.0.0.0")]
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.ComponentModel.ToolboxItem(true)]
    [System.Xml.Serialization.XmlSchemaProviderAttribute("GetTypedDataSetSchema")]
    [System.Xml.Serialization.XmlRootAttribute("Calendars")]
    [System.ComponentModel.Design.HelpKeywordAttribute("vs.data.DataSet")]
    public partial class Calendars : System.Data.DataSet {
        
        private CalendarsDataTable tableCalendars;
        
        private System.Data.SchemaSerializationMode _schemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public Calendars() {
            this.BeginInit();
            this.InitClass();
            System.ComponentModel.CollectionChangeEventHandler schemaChangedHandler = new System.ComponentModel.CollectionChangeEventHandler(this.SchemaChanged);
            base.Tables.CollectionChanged += schemaChangedHandler;
            base.Relations.CollectionChanged += schemaChangedHandler;
            this.EndInit();
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        protected Calendars(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : 
                base(info, context, false) {
            if ((this.IsBinarySerialized(info, context) == true)) {
                this.InitVars(false);
                System.ComponentModel.CollectionChangeEventHandler schemaChangedHandler1 = new System.ComponentModel.CollectionChangeEventHandler(this.SchemaChanged);
                this.Tables.CollectionChanged += schemaChangedHandler1;
                this.Relations.CollectionChanged += schemaChangedHandler1;
                return;
            }
            string strSchema = ((string)(info.GetValue("XmlSchema", typeof(string))));
            if ((this.DetermineSchemaSerializationMode(info, context) == System.Data.SchemaSerializationMode.IncludeSchema)) {
                System.Data.DataSet ds = new System.Data.DataSet();
                ds.ReadXmlSchema(new System.Xml.XmlTextReader(new System.IO.StringReader(strSchema)));
                if ((ds.Tables["Calendars"] != null)) {
                    base.Tables.Add(new CalendarsDataTable(ds.Tables["Calendars"]));
                }
                this.DataSetName = ds.DataSetName;
                this.Prefix = ds.Prefix;
                this.Namespace = ds.Namespace;
                this.Locale = ds.Locale;
                this.CaseSensitive = ds.CaseSensitive;
                this.EnforceConstraints = ds.EnforceConstraints;
                this.Merge(ds, false, System.Data.MissingSchemaAction.Add);
                this.InitVars();
            }
            else {
                this.ReadXmlSchema(new System.Xml.XmlTextReader(new System.IO.StringReader(strSchema)));
            }
            this.GetSerializationData(info, context);
            System.ComponentModel.CollectionChangeEventHandler schemaChangedHandler = new System.ComponentModel.CollectionChangeEventHandler(this.SchemaChanged);
            base.Tables.CollectionChanged += schemaChangedHandler;
            this.Relations.CollectionChanged += schemaChangedHandler;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)]
        public CalendarsDataTable _Calendars {
            get {
                return this.tableCalendars;
            }
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.BrowsableAttribute(true)]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public override System.Data.SchemaSerializationMode SchemaSerializationMode {
            get {
                return this._schemaSerializationMode;
            }
            set {
                this._schemaSerializationMode = value;
            }
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public new System.Data.DataTableCollection Tables {
            get {
                return base.Tables;
            }
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public new System.Data.DataRelationCollection Relations {
            get {
                return base.Relations;
            }
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        protected override void InitializeDerivedDataSet() {
            this.BeginInit();
            this.InitClass();
            this.EndInit();
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public override System.Data.DataSet Clone() {
            Calendars cln = ((Calendars)(base.Clone()));
            cln.InitVars();
            cln.SchemaSerializationMode = this.SchemaSerializationMode;
            return cln;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        protected override bool ShouldSerializeTables() {
            return false;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        protected override bool ShouldSerializeRelations() {
            return false;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        protected override void ReadXmlSerializable(System.Xml.XmlReader reader) {
            if ((this.DetermineSchemaSerializationMode(reader) == System.Data.SchemaSerializationMode.IncludeSchema)) {
                this.Reset();
                System.Data.DataSet ds = new System.Data.DataSet();
                ds.ReadXml(reader);
                if ((ds.Tables["Calendars"] != null)) {
                    base.Tables.Add(new CalendarsDataTable(ds.Tables["Calendars"]));
                }
                this.DataSetName = ds.DataSetName;
                this.Prefix = ds.Prefix;
                this.Namespace = ds.Namespace;
                this.Locale = ds.Locale;
                this.CaseSensitive = ds.CaseSensitive;
                this.EnforceConstraints = ds.EnforceConstraints;
                this.Merge(ds, false, System.Data.MissingSchemaAction.Add);
                this.InitVars();
            }
            else {
                this.ReadXml(reader);
                this.InitVars();
            }
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        protected override System.Xml.Schema.XmlSchema GetSchemaSerializable() {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            this.WriteXmlSchema(new System.Xml.XmlTextWriter(stream, null));
            stream.Position = 0;
            return System.Xml.Schema.XmlSchema.Read(new System.Xml.XmlTextReader(stream), null);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        internal void InitVars() {
            this.InitVars(true);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        internal void InitVars(bool initTable) {
            this.tableCalendars = ((CalendarsDataTable)(base.Tables["Calendars"]));
            if ((initTable == true)) {
                if ((this.tableCalendars != null)) {
                    this.tableCalendars.InitVars();
                }
            }
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private void InitClass() {
            this.DataSetName = "Calendars";
            this.Prefix = "";
            this.Namespace = "http://tempuri.org/Calendars.xsd";
            this.EnforceConstraints = true;
            this.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            this.tableCalendars = new CalendarsDataTable();
            base.Tables.Add(this.tableCalendars);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private bool ShouldSerialize_Calendars() {
            return false;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private void SchemaChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e) {
            if ((e.Action == System.ComponentModel.CollectionChangeAction.Remove)) {
                this.InitVars();
            }
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static System.Xml.Schema.XmlSchemaComplexType GetTypedDataSetSchema(System.Xml.Schema.XmlSchemaSet xs) {
            Calendars ds = new Calendars();
            System.Xml.Schema.XmlSchemaComplexType type = new System.Xml.Schema.XmlSchemaComplexType();
            System.Xml.Schema.XmlSchemaSequence sequence = new System.Xml.Schema.XmlSchemaSequence();
            xs.Add(ds.GetSchemaSerializable());
            System.Xml.Schema.XmlSchemaAny any = new System.Xml.Schema.XmlSchemaAny();
            any.Namespace = ds.Namespace;
            sequence.Items.Add(any);
            type.Particle = sequence;
            return type;
        }
        
        public delegate void CalendarsRowChangeEventHandler(object sender, CalendarsRowChangeEvent e);
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Design.TypedDataSetGenerator", "2.0.0.0")]
        [System.Serializable()]
        [System.Xml.Serialization.XmlSchemaProviderAttribute("GetTypedTableSchema")]
        public partial class CalendarsDataTable : System.Data.DataTable, System.Collections.IEnumerable {
            
            private System.Data.DataColumn columnID;
            
            private System.Data.DataColumn columnURL;
            
            private System.Data.DataColumn columnName;
            
            private System.Data.DataColumn columnDescription;
            
            private System.Data.DataColumn columnIsParent;
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public CalendarsDataTable() {
                this.TableName = "Calendars";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            internal CalendarsDataTable(System.Data.DataTable table) {
                this.TableName = table.TableName;
                if ((table.CaseSensitive != table.DataSet.CaseSensitive)) {
                    this.CaseSensitive = table.CaseSensitive;
                }
                if ((table.Locale.ToString() != table.DataSet.Locale.ToString())) {
                    this.Locale = table.Locale;
                }
                if ((table.Namespace != table.DataSet.Namespace)) {
                    this.Namespace = table.Namespace;
                }
                this.Prefix = table.Prefix;
                this.MinimumCapacity = table.MinimumCapacity;
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            protected CalendarsDataTable(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : 
                    base(info, context) {
                this.InitVars();
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public System.Data.DataColumn IDColumn {
                get {
                    return this.columnID;
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public System.Data.DataColumn URLColumn {
                get {
                    return this.columnURL;
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public System.Data.DataColumn NameColumn {
                get {
                    return this.columnName;
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public System.Data.DataColumn DescriptionColumn {
                get {
                    return this.columnDescription;
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public System.Data.DataColumn IsParentColumn {
                get {
                    return this.columnIsParent;
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [System.ComponentModel.Browsable(false)]
            public int Count {
                get {
                    return this.Rows.Count;
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public CalendarsRow this[int index] {
                get {
                    return ((CalendarsRow)(this.Rows[index]));
                }
            }
            
            public event CalendarsRowChangeEventHandler CalendarsRowChanging;
            
            public event CalendarsRowChangeEventHandler CalendarsRowChanged;
            
            public event CalendarsRowChangeEventHandler CalendarsRowDeleting;
            
            public event CalendarsRowChangeEventHandler CalendarsRowDeleted;
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public void AddCalendarsRow(CalendarsRow row) {
                this.Rows.Add(row);
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public CalendarsRow AddCalendarsRow(string ID, string URL, string Name, string Description, bool IsParent) {
                CalendarsRow rowCalendarsRow = ((CalendarsRow)(this.NewRow()));
                rowCalendarsRow.ItemArray = new object[] {
                        ID,
                        URL,
                        Name,
                        Description,
                        IsParent};
                this.Rows.Add(rowCalendarsRow);
                return rowCalendarsRow;
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public virtual System.Collections.IEnumerator GetEnumerator() {
                return this.Rows.GetEnumerator();
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public override System.Data.DataTable Clone() {
                CalendarsDataTable cln = ((CalendarsDataTable)(base.Clone()));
                cln.InitVars();
                return cln;
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            protected override System.Data.DataTable CreateInstance() {
                return new CalendarsDataTable();
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            internal void InitVars() {
                this.columnID = base.Columns["ID"];
                this.columnURL = base.Columns["URL"];
                this.columnName = base.Columns["Name"];
                this.columnDescription = base.Columns["Description"];
                this.columnIsParent = base.Columns["IsParent"];
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            private void InitClass() {
                this.columnID = new System.Data.DataColumn("ID", typeof(string), null, System.Data.MappingType.Element);
                base.Columns.Add(this.columnID);
                this.columnURL = new System.Data.DataColumn("URL", typeof(string), null, System.Data.MappingType.Element);
                base.Columns.Add(this.columnURL);
                this.columnName = new System.Data.DataColumn("Name", typeof(string), null, System.Data.MappingType.Element);
                base.Columns.Add(this.columnName);
                this.columnDescription = new System.Data.DataColumn("Description", typeof(string), null, System.Data.MappingType.Element);
                base.Columns.Add(this.columnDescription);
                this.columnIsParent = new System.Data.DataColumn("IsParent", typeof(bool), null, System.Data.MappingType.Element);
                base.Columns.Add(this.columnIsParent);
                this.Constraints.Add(new System.Data.UniqueConstraint("CalendarsID", new System.Data.DataColumn[] {
                                this.columnID}, false));
                this.columnID.Unique = true;
                this.ExtendedProperties.Add("Generator_TablePropName", "_Calendars");
                this.ExtendedProperties.Add("Generator_UserTableName", "Calendars");
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public CalendarsRow NewCalendarsRow() {
                return ((CalendarsRow)(this.NewRow()));
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            protected override System.Data.DataRow NewRowFromBuilder(System.Data.DataRowBuilder builder) {
                return new CalendarsRow(builder);
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            protected override System.Type GetRowType() {
                return typeof(CalendarsRow);
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            protected override void OnRowChanged(System.Data.DataRowChangeEventArgs e) {
                base.OnRowChanged(e);
                if ((this.CalendarsRowChanged != null)) {
                    this.CalendarsRowChanged(this, new CalendarsRowChangeEvent(((CalendarsRow)(e.Row)), e.Action));
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            protected override void OnRowChanging(System.Data.DataRowChangeEventArgs e) {
                base.OnRowChanging(e);
                if ((this.CalendarsRowChanging != null)) {
                    this.CalendarsRowChanging(this, new CalendarsRowChangeEvent(((CalendarsRow)(e.Row)), e.Action));
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            protected override void OnRowDeleted(System.Data.DataRowChangeEventArgs e) {
                base.OnRowDeleted(e);
                if ((this.CalendarsRowDeleted != null)) {
                    this.CalendarsRowDeleted(this, new CalendarsRowChangeEvent(((CalendarsRow)(e.Row)), e.Action));
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            protected override void OnRowDeleting(System.Data.DataRowChangeEventArgs e) {
                base.OnRowDeleting(e);
                if ((this.CalendarsRowDeleting != null)) {
                    this.CalendarsRowDeleting(this, new CalendarsRowChangeEvent(((CalendarsRow)(e.Row)), e.Action));
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public void RemoveCalendarsRow(CalendarsRow row) {
                this.Rows.Remove(row);
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public static System.Xml.Schema.XmlSchemaComplexType GetTypedTableSchema(System.Xml.Schema.XmlSchemaSet xs) {
                System.Xml.Schema.XmlSchemaComplexType type = new System.Xml.Schema.XmlSchemaComplexType();
                System.Xml.Schema.XmlSchemaSequence sequence = new System.Xml.Schema.XmlSchemaSequence();
                Calendars ds = new Calendars();
                xs.Add(ds.GetSchemaSerializable());
                System.Xml.Schema.XmlSchemaAny any1 = new System.Xml.Schema.XmlSchemaAny();
                any1.Namespace = "http://www.w3.org/2001/XMLSchema";
                any1.MinOccurs = new decimal(0);
                any1.MaxOccurs = decimal.MaxValue;
                any1.ProcessContents = System.Xml.Schema.XmlSchemaContentProcessing.Lax;
                sequence.Items.Add(any1);
                System.Xml.Schema.XmlSchemaAny any2 = new System.Xml.Schema.XmlSchemaAny();
                any2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
                any2.MinOccurs = new decimal(1);
                any2.ProcessContents = System.Xml.Schema.XmlSchemaContentProcessing.Lax;
                sequence.Items.Add(any2);
                System.Xml.Schema.XmlSchemaAttribute attribute1 = new System.Xml.Schema.XmlSchemaAttribute();
                attribute1.Name = "namespace";
                attribute1.FixedValue = ds.Namespace;
                type.Attributes.Add(attribute1);
                System.Xml.Schema.XmlSchemaAttribute attribute2 = new System.Xml.Schema.XmlSchemaAttribute();
                attribute2.Name = "tableTypeName";
                attribute2.FixedValue = "CalendarsDataTable";
                type.Attributes.Add(attribute2);
                type.Particle = sequence;
                return type;
            }
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Design.TypedDataSetGenerator", "2.0.0.0")]
        public partial class CalendarsRow : System.Data.DataRow {
            
            private CalendarsDataTable tableCalendars;
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            internal CalendarsRow(System.Data.DataRowBuilder rb) : 
                    base(rb) {
                this.tableCalendars = ((CalendarsDataTable)(this.Table));
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public string ID {
                get {
                    try {
                        return ((string)(this[this.tableCalendars.IDColumn]));
                    }
                    catch (System.InvalidCastException e) {
                        throw new System.Data.StrongTypingException("The value for column \'ID\' in table \'Calendars\' is DBNull.", e);
                    }
                }
                set {
                    this[this.tableCalendars.IDColumn] = value;
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public string URL {
                get {
                    try {
                        return ((string)(this[this.tableCalendars.URLColumn]));
                    }
                    catch (System.InvalidCastException e) {
                        throw new System.Data.StrongTypingException("The value for column \'URL\' in table \'Calendars\' is DBNull.", e);
                    }
                }
                set {
                    this[this.tableCalendars.URLColumn] = value;
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public string Name {
                get {
                    try {
                        return ((string)(this[this.tableCalendars.NameColumn]));
                    }
                    catch (System.InvalidCastException e) {
                        throw new System.Data.StrongTypingException("The value for column \'Name\' in table \'Calendars\' is DBNull.", e);
                    }
                }
                set {
                    this[this.tableCalendars.NameColumn] = value;
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public string Description {
                get {
                    try {
                        return ((string)(this[this.tableCalendars.DescriptionColumn]));
                    }
                    catch (System.InvalidCastException e) {
                        throw new System.Data.StrongTypingException("The value for column \'Description\' in table \'Calendars\' is DBNull.", e);
                    }
                }
                set {
                    this[this.tableCalendars.DescriptionColumn] = value;
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public bool IsParent {
                get {
                    try {
                        return ((bool)(this[this.tableCalendars.IsParentColumn]));
                    }
                    catch (System.InvalidCastException e) {
                        throw new System.Data.StrongTypingException("The value for column \'IsParent\' in table \'Calendars\' is DBNull.", e);
                    }
                }
                set {
                    this[this.tableCalendars.IsParentColumn] = value;
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public bool IsIDNull() {
                return this.IsNull(this.tableCalendars.IDColumn);
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public void SetIDNull() {
                this[this.tableCalendars.IDColumn] = System.Convert.DBNull;
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public bool IsURLNull() {
                return this.IsNull(this.tableCalendars.URLColumn);
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public void SetURLNull() {
                this[this.tableCalendars.URLColumn] = System.Convert.DBNull;
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public bool IsNameNull() {
                return this.IsNull(this.tableCalendars.NameColumn);
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public void SetNameNull() {
                this[this.tableCalendars.NameColumn] = System.Convert.DBNull;
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public bool IsDescriptionNull() {
                return this.IsNull(this.tableCalendars.DescriptionColumn);
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public void SetDescriptionNull() {
                this[this.tableCalendars.DescriptionColumn] = System.Convert.DBNull;
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public bool IsIsParentNull() {
                return this.IsNull(this.tableCalendars.IsParentColumn);
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public void SetIsParentNull() {
                this[this.tableCalendars.IsParentColumn] = System.Convert.DBNull;
            }
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Data.Design.TypedDataSetGenerator", "2.0.0.0")]
        public class CalendarsRowChangeEvent : System.EventArgs {
            
            private CalendarsRow eventRow;
            
            private System.Data.DataRowAction eventAction;
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public CalendarsRowChangeEvent(CalendarsRow row, System.Data.DataRowAction action) {
                this.eventRow = row;
                this.eventAction = action;
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public CalendarsRow Row {
                get {
                    return this.eventRow;
                }
            }
            
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public System.Data.DataRowAction Action {
                get {
                    return this.eventAction;
                }
            }
        }
    }
}

#pragma warning restore 1591