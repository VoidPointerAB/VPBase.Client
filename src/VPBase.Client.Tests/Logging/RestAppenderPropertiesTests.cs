using log4net.Core;
using NUnit.Framework;
using VPBase.Auth.Client.Models.Logging;
using VPBase.Client.Code.Log4Net;

namespace VPBase.Client.Tests.Logging
{
    [TestFixture]
    public class RestAppenderPropertiesTests
    {
        [Test]
        public void Where_get_property_value_using_log_definitions()
        {
            var restAppender = new RestAppender();

            var loggingEvent = new LoggingEvent(new LoggingEventData());

            loggingEvent.Properties[LoggingKeyDefinitions.UserId] = "userid";
            loggingEvent.Properties[LoggingKeyDefinitions.UserName] = "userName";

            loggingEvent.Properties[LoggingKeyDefinitions.EntityId] = "entityid";
            loggingEvent.Properties[LoggingKeyDefinitions.EntityValueName] = "entityValueName";
            loggingEvent.Properties[LoggingKeyDefinitions.EntityTypeName] = "entityTypeName";
            loggingEvent.Properties[LoggingKeyDefinitions.EntityType] = 1;

            loggingEvent.Properties[LoggingKeyDefinitions.EntityId2] = "entityid2";
            loggingEvent.Properties[LoggingKeyDefinitions.EntityValueName2] = "entityValueName2";
            loggingEvent.Properties[LoggingKeyDefinitions.EntityTypeName2] = "entityTypeName2";
            loggingEvent.Properties[LoggingKeyDefinitions.EntityType2] = 2;

            loggingEvent.Properties[LoggingKeyDefinitions.RawUrl] = "rawurl";
            loggingEvent.Properties[LoggingKeyDefinitions.AbsoluteUri] = "absoluturi";

            loggingEvent.Properties[LoggingKeyDefinitions.AdditionalInformation] = "additional";

            var logPropertyValues = restAppender.GetLogPropertyValuesFromEvent(loggingEvent);

            CheckLogStringProperty(LoggingKeyDefinitions.UserId, "userid", logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.UserName, "userName", logPropertyValues);

            CheckLogStringProperty(LoggingKeyDefinitions.EntityId, "entityid", logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.EntityValueName, "entityValueName", logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.EntityTypeName, "entityTypeName", logPropertyValues);
            CheckLogIntProperty(LoggingKeyDefinitions.EntityType, 1, logPropertyValues);

            CheckLogStringProperty(LoggingKeyDefinitions.EntityId2, "entityid2", logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.EntityValueName2, "entityValueName2", logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.EntityTypeName2, "entityTypeName2", logPropertyValues);
            CheckLogIntProperty(LoggingKeyDefinitions.EntityType2, 2, logPropertyValues);

            CheckLogStringProperty(LoggingKeyDefinitions.RawUrl, "rawurl", logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.AbsoluteUri, "absoluturi", logPropertyValues);

            CheckLogStringProperty(LoggingKeyDefinitions.AdditionalInformation, "additional", logPropertyValues);
        }

        [Test]
        public void Where_get_property_value_using_log_definitions_integer()
        {
            var restAppender = new RestAppender();

            var loggingEvent = new LoggingEvent(new LoggingEventData());

            loggingEvent.Properties[LoggingKeyDefinitions.UserId] = "1234";
            loggingEvent.Properties[LoggingKeyDefinitions.UserName] = "2345";

            loggingEvent.Properties[LoggingKeyDefinitions.EntityId] = "3456";
            loggingEvent.Properties[LoggingKeyDefinitions.EntityValueName] = "4567";
            loggingEvent.Properties[LoggingKeyDefinitions.EntityTypeName] = "5678";
            loggingEvent.Properties[LoggingKeyDefinitions.EntityType] = 123;

            loggingEvent.Properties[LoggingKeyDefinitions.EntityId2] = "7654";
            loggingEvent.Properties[LoggingKeyDefinitions.EntityValueName2] = "2222";
            loggingEvent.Properties[LoggingKeyDefinitions.EntityTypeName2] = "3333";
            loggingEvent.Properties[LoggingKeyDefinitions.EntityType2] = 456;

            loggingEvent.Properties[LoggingKeyDefinitions.RawUrl] = "999";
            loggingEvent.Properties[LoggingKeyDefinitions.AbsoluteUri] = "888";

            loggingEvent.Properties[LoggingKeyDefinitions.AdditionalInformation] = "additional";

            var logPropertyValues = restAppender.GetLogPropertyValuesFromEvent(loggingEvent);

            CheckLogStringProperty(LoggingKeyDefinitions.UserId, "1234", logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.UserName, "2345", logPropertyValues);

            CheckLogStringProperty(LoggingKeyDefinitions.EntityId, "3456", logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.EntityValueName, "4567", logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.EntityTypeName, "5678", logPropertyValues);
            CheckLogIntProperty(LoggingKeyDefinitions.EntityType, 123, logPropertyValues);

            CheckLogStringProperty(LoggingKeyDefinitions.EntityId2, "7654", logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.EntityValueName2, "2222", logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.EntityTypeName2, "3333", logPropertyValues);
            CheckLogIntProperty(LoggingKeyDefinitions.EntityType2, 456, logPropertyValues);

            CheckLogStringProperty(LoggingKeyDefinitions.RawUrl, "999", logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.AbsoluteUri, "888", logPropertyValues);

            CheckLogStringProperty(LoggingKeyDefinitions.AdditionalInformation, "additional", logPropertyValues);
        }

        [Test]
        public void Where_get_property_value_using_log_definitions_empty()
        {
            var restAppender = new RestAppender();

            var loggingEvent = new LoggingEvent(new LoggingEventData());

            loggingEvent.Properties[LoggingKeyDefinitions.UserId] = string.Empty;
            loggingEvent.Properties[LoggingKeyDefinitions.UserName] = string.Empty;

            loggingEvent.Properties[LoggingKeyDefinitions.EntityId] = string.Empty;
            loggingEvent.Properties[LoggingKeyDefinitions.EntityValueName] = string.Empty;
            loggingEvent.Properties[LoggingKeyDefinitions.EntityTypeName] = string.Empty;

            loggingEvent.Properties[LoggingKeyDefinitions.EntityId2] = string.Empty;
            loggingEvent.Properties[LoggingKeyDefinitions.EntityValueName2] = string.Empty;
            loggingEvent.Properties[LoggingKeyDefinitions.EntityTypeName2] = string.Empty;

            loggingEvent.Properties[LoggingKeyDefinitions.RawUrl] = string.Empty;
            loggingEvent.Properties[LoggingKeyDefinitions.AbsoluteUri] = string.Empty;

            loggingEvent.Properties[LoggingKeyDefinitions.AdditionalInformation] = string.Empty;

            var logPropertyValues = restAppender.GetLogPropertyValuesFromEvent(loggingEvent);

            CheckLogStringProperty(LoggingKeyDefinitions.UserId, string.Empty, logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.UserName, string.Empty, logPropertyValues);

            CheckLogStringProperty(LoggingKeyDefinitions.EntityId, string.Empty, logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.EntityValueName, string.Empty, logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.EntityTypeName, string.Empty, logPropertyValues);

            CheckLogStringProperty(LoggingKeyDefinitions.EntityId2, string.Empty, logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.EntityValueName2, string.Empty, logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.EntityTypeName2, string.Empty, logPropertyValues);

            CheckLogStringProperty(LoggingKeyDefinitions.RawUrl, string.Empty, logPropertyValues);
            CheckLogStringProperty(LoggingKeyDefinitions.AbsoluteUri, string.Empty, logPropertyValues);

            CheckLogStringProperty(LoggingKeyDefinitions.AdditionalInformation, string.Empty, logPropertyValues);
        }

        [Test]
        public void Where_get_property_value_using_log_definitions_null()
        {
            var restAppender = new RestAppender();

            var loggingEvent = new LoggingEvent(new LoggingEventData());

            loggingEvent.Properties[LoggingKeyDefinitions.UserId] = null;
            loggingEvent.Properties[LoggingKeyDefinitions.UserName] = null;

            loggingEvent.Properties[LoggingKeyDefinitions.EntityId] = null;
            loggingEvent.Properties[LoggingKeyDefinitions.EntityValueName] = null;
            loggingEvent.Properties[LoggingKeyDefinitions.EntityTypeName] = null;

            loggingEvent.Properties[LoggingKeyDefinitions.EntityId2] = null;
            loggingEvent.Properties[LoggingKeyDefinitions.EntityValueName2] = null;
            loggingEvent.Properties[LoggingKeyDefinitions.EntityTypeName2] = null;

            loggingEvent.Properties[LoggingKeyDefinitions.RawUrl] = null;
            loggingEvent.Properties[LoggingKeyDefinitions.AbsoluteUri] = null;

            loggingEvent.Properties[LoggingKeyDefinitions.AdditionalInformation] = null;

            var logPropertyValues = restAppender.GetLogPropertyValuesFromEvent(loggingEvent);

            CheckLogPropertyIsNull(LoggingKeyDefinitions.UserId, logPropertyValues);
            CheckLogPropertyIsNull(LoggingKeyDefinitions.UserName, logPropertyValues);

            CheckLogPropertyIsNull(LoggingKeyDefinitions.EntityId, logPropertyValues);
            CheckLogPropertyIsNull(LoggingKeyDefinitions.EntityValueName, logPropertyValues);
            CheckLogPropertyIsNull(LoggingKeyDefinitions.EntityTypeName, logPropertyValues);

            CheckLogPropertyIsNull(LoggingKeyDefinitions.EntityId2, logPropertyValues);
            CheckLogPropertyIsNull(LoggingKeyDefinitions.EntityValueName2, logPropertyValues);
            CheckLogPropertyIsNull(LoggingKeyDefinitions.EntityTypeName2, logPropertyValues);

            CheckLogPropertyIsNull(LoggingKeyDefinitions.RawUrl, logPropertyValues);
            CheckLogPropertyIsNull(LoggingKeyDefinitions.AbsoluteUri, logPropertyValues);

            CheckLogPropertyIsNull(LoggingKeyDefinitions.AdditionalInformation, logPropertyValues);
        }

        [Test]
        public void Where_get_property_value_using_custom_data_string()
        {
            var restAppender = new RestAppender();

            var loggingEvent = new LoggingEvent(new LoggingEventData());

            loggingEvent.Properties["CustomKey"] = "data";

            var logPropertyValues = restAppender.GetLogPropertyValuesFromEvent(loggingEvent);

            var customProperty = logPropertyValues.FirstOrDefault(x => x.Key == "CustomKey");

            Assert.That(customProperty != null);

            Assert.That("data", Is.EqualTo(customProperty.StringValue));
            Assert.That("data", Is.EqualTo(customProperty.ObjectValue));
            Assert.That(typeof(string).FullName, Is.EqualTo(customProperty.DataTypeFullName));
            Assert.That(LoggingPropertyType.String, Is.EqualTo(customProperty.LoggingPropertyType));
        }

        [Test]
        public void Where_get_log_value_using_custom_data_string()
        {
            var restAppender = new RestAppender();

            var loggingEvent = new LoggingEvent(new LoggingEventData());

            loggingEvent.Properties["CustomKey"] = "data";

            var logValueItem = restAppender.GetLogValueItem(loggingEvent);

            var keyValue = logValueItem.KeyValues.FirstOrDefault(x => x.Key == "CustomKey");

            Assert.That(keyValue != null);

            Assert.That("data", Is.EqualTo(keyValue.Value));
            Assert.That(typeof(string).FullName, Is.EqualTo(keyValue.DataTypeFullName));
        }

        [Test]
        public void Where_get_property_value_using_custom_data_integer()
        {
            var restAppender = new RestAppender();

            var loggingEvent = new LoggingEvent(new LoggingEventData());

            loggingEvent.Properties["CustomKey"] = 10;

            var logPropertyValues = restAppender.GetLogPropertyValuesFromEvent(loggingEvent);

            var customProperty = logPropertyValues.FirstOrDefault(x => x.Key == "CustomKey");

            Assert.That(customProperty != null);

            Assert.That(10, Is.EqualTo(customProperty.IntValue));
            Assert.That(10, Is.EqualTo(customProperty.ObjectValue));
            Assert.That(typeof(int).FullName, Is.EqualTo(customProperty.DataTypeFullName));
            Assert.That(LoggingPropertyType.Int, Is.EqualTo(customProperty.LoggingPropertyType));
        }

        [Test]
        public void Where_get_property_value_using_custom_data_datetime()
        {
            var restAppender = new RestAppender();

            var loggingEvent = new LoggingEvent(new LoggingEventData());

            loggingEvent.Properties["CustomKey"] = new DateTime(2023, 4, 28, 19, 33, 12);

            var logPropertyValues = restAppender.GetLogPropertyValuesFromEvent(loggingEvent);

            var customProperty = logPropertyValues.FirstOrDefault(x => x.Key == "CustomKey");

            Assert.That(customProperty != null);

            Assert.That(new DateTime(2023, 4, 28, 19, 33, 12), Is.EqualTo(customProperty.DateTimeValue));
            Assert.That(new DateTime(2023, 4, 28, 19, 33, 12), Is.EqualTo(customProperty.ObjectValue));
            Assert.That(typeof(DateTime).FullName, Is.EqualTo(customProperty.DataTypeFullName));
            Assert.That(LoggingPropertyType.DateTime, Is.EqualTo(customProperty.LoggingPropertyType));
        }

        [Test]
        public void Where_get_property_value_using_custom_data_bool()
        {
            var restAppender = new RestAppender();

            var loggingEvent = new LoggingEvent(new LoggingEventData());

            loggingEvent.Properties["CustomKey"] = true;

            var logPropertyValues = restAppender.GetLogPropertyValuesFromEvent(loggingEvent);

            var customProperty = logPropertyValues.FirstOrDefault(x => x.Key == "CustomKey");

            Assert.That(customProperty != null);

            Assert.That(true, Is.EqualTo(customProperty.BoolValue));
            Assert.That(true, Is.EqualTo(customProperty.ObjectValue));
            Assert.That(typeof(bool).FullName, Is.EqualTo(customProperty.DataTypeFullName));
            Assert.That(LoggingPropertyType.Bool, Is.EqualTo(customProperty.LoggingPropertyType));
        }

        [Test]
        public void Where_get_property_value_using_custom_data_decimal()
        {
            var restAppender = new RestAppender();

            var loggingEvent = new LoggingEvent(new LoggingEventData());

            loggingEvent.Properties["CustomKey"] = 2.1m;

            var logPropertyValues = restAppender.GetLogPropertyValuesFromEvent(loggingEvent);

            var customProperty = logPropertyValues.FirstOrDefault(x => x.Key == "CustomKey");

            Assert.That(customProperty != null);

            Assert.That(2.1m, Is.EqualTo(customProperty.DecimalValue));
            Assert.That(2.1m, Is.EqualTo(customProperty.ObjectValue));
            Assert.That(typeof(decimal).FullName, Is.EqualTo(customProperty.DataTypeFullName));
            Assert.That(LoggingPropertyType.Decimal, Is.EqualTo(customProperty.LoggingPropertyType));
        }

        private void CheckLogStringProperty(string key, string expectedValue, IEnumerable<LoggingPropertyValue> logPropertyValues)
        {
            var logPropertyValue = logPropertyValues.FirstOrDefault(x => x.Key == key);

            Assert.That(logPropertyValue != null);

            Assert.That(expectedValue, Is.EqualTo(logPropertyValue.StringValue));
            Assert.That(expectedValue, Is.EqualTo(logPropertyValue.ObjectValue));
            Assert.That(typeof(string).FullName, Is.EqualTo(logPropertyValue.DataTypeFullName));
            Assert.That(LoggingPropertyType.String, Is.EqualTo(logPropertyValue.LoggingPropertyType));
        }

        private void CheckLogIntProperty(string key, int expectedValue, IEnumerable<LoggingPropertyValue> logPropertyValues)
        {
            var logPropertyValue = logPropertyValues.FirstOrDefault(x => x.Key == key);

            Assert.That(logPropertyValue != null);

            Assert.That(expectedValue, Is.EqualTo(logPropertyValue.IntValue));
            Assert.That(expectedValue, Is.EqualTo(logPropertyValue.ObjectValue));
            Assert.That(typeof(int).FullName, Is.EqualTo(logPropertyValue.DataTypeFullName));
            Assert.That(LoggingPropertyType.Int, Is.EqualTo(logPropertyValue.LoggingPropertyType));
        }

        /// Some cases string fields will be used/handled as integers
        private void CheckLogStringPropertySpecial(string key, string expectedValue, IEnumerable<LoggingPropertyValue> logPropertyValues)
        {
            var logPropertyValue = logPropertyValues.FirstOrDefault(x => x.Key == key);

            Assert.That(logPropertyValue != null);

            Assert.That(expectedValue, Is.EqualTo(logPropertyValue.StringValue));

            var expectedIntValue = int.Parse(expectedValue);
            Assert.That(expectedIntValue, Is.EqualTo(logPropertyValue.IntValue));
            Assert.That(expectedIntValue, Is.EqualTo(logPropertyValue.ObjectValue));
            Assert.That(typeof(int).FullName, Is.EqualTo(logPropertyValue.DataTypeFullName));
            Assert.That(LoggingPropertyType.Int, Is.EqualTo(logPropertyValue.LoggingPropertyType));
        }

        private void CheckLogPropertyIsNull(string key, IEnumerable<LoggingPropertyValue> logPropertyValues)
        {
            var logPropertyValue = logPropertyValues.FirstOrDefault(x => x.Key == key);
            Assert.That(logPropertyValue == null);
        }
    }
}
