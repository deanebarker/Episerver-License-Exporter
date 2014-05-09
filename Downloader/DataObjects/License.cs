using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace InsightlyApi.InsightlyObjects
{
    [TableMapping(ObjectName = "License", TableName = "Licenses")]
    public class License : DataObject
    {
        public License(XmlElement xml) : base(xml)
        {
        }

        [ColumnMapping]
        public string Id
        {
            get
            {
                return GetValue(".//Id");
            }
        }

        [ColumnMapping]
        public string CompanyId
        {
            get
            {
                return GetValue(".//CustomerId");
            }
        }

        [ColumnMapping]
        public string ContactId
        {
            get
            {
                return GetValue(".//CustomerContactId");
            }
        }

        [ColumnMapping]
        public string Name
        {
            get
            {
                return GetValue(".//ProductName");
            }
        }

        [ColumnMapping]
        public string Created
        {
            get
            {
                return GetValue(".//Created");
            }
        }

        [ColumnMapping]
        public string Expires
        {
            get
            {
                return GetValue(".//ExpirationDate");
            }
        }

    }
}
