using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace InsightlyApi.InsightlyObjects
{
    [TableMapping(ObjectName="Contact", TableName="Contacts")]
    public class Contact : DataObject
    {
        public Contact(XmlElement xml) : base(xml)
        {

        }

        [ColumnMapping]
        public string Name
        {
            get
            {
                return GetValue(".//Id");
            }
        }

        [ColumnMapping]
        public string FirstName
        {
            get
            {
                return GetValue(".//FirstName");
            }
        }

        [ColumnMapping]
        public string LastName
        {
            get
            {
                return GetValue(".//LastName");
            }
        }

        [ColumnMapping]
        public string Email
        {
            get
            {
                return GetValue(".//Email");
            }
        }
    }
}
