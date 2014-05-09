using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace InsightlyApi.InsightlyObjects
{
    [TableMapping(ObjectName="Company", TableName="Companies")]
    class Company : DataObject
    {
        public Company(XmlElement xml)
            : base(xml)
        {

        }

        [ColumnMapping]
        public string Name
        {
            get
            {
                return GetValue(".//Name");
            }
        }

        [ColumnMapping]
        public string Address
        {
            get
            {
                return GetValue(".//Address");
            }
        }

        [ColumnMapping]
        public string City
        {
            get
            {
                return GetValue(".//City");
            }
        }

        [ColumnMapping]
        public string PostalCode
        {
            get
            {
                return GetValue(".//ZipCode");
            }
        }

        [ColumnMapping]
        public string Country
        {
            get
            {
                return GetValue(".//Country");
            }
        }
    }
}
