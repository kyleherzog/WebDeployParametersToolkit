using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDeployParametersToolkit.Utilities;

namespace WebDeployParametersToolkit.Tests.ParametersXmlReaderTests
{
    [TestClass]
    public class ReadShould
    {
        private const string defaultProjectName = "MyTestProject";

        [TestMethod]
        public void ReturnExpectedValuesWhenUsingEmptySettings()
        {
            var expected = GetBasicParameters(false);
            var reader = new ParametersXmlReader(Properties.Resources.BasicParameters, defaultProjectName, Properties.Resources.EmptySettings);
            var parameters = reader.Read();
            parameters.AssertHasSameItems(expected);
        }

        [TestMethod]
        public void ReturnExpectedValuesWhenUsingSimpleSettings()
        {
            var expected = GetBasicParameters(true);
            var reader = new ParametersXmlReader(Properties.Resources.BasicParameters, defaultProjectName, Properties.Resources.SimpleSettings);
            var parameters = reader.Read();
            parameters.AssertHasSameItems(expected);
        }

        [TestMethod]
        [ExpectedException(typeof(FileFormatException))]
        public void ThrowExceptionIfRootElementNotParameters()
        {
            var reader = new ParametersXmlReader("<?xml version=\"1.0\" encoding=\"utf-8\" ?><configuration></configuration>", defaultProjectName, null);
            reader.Read();
        }

        private ICollection<WebDeployParameter> GetAutoParameters()
        {
            var results = new List<WebDeployParameter>();

            results.Add(new WebDeployParameter()
            {
                Name = "FirstConnectionString-Web.config Connection String",
                DefaultValue = "server=localhost;database=FirstDb;uid=myUser;password=myPass;",
                Description = "FirstConnectionString Connection String used in web.config by the application to access the database.",
                Entries = new List<WebDeployParameterEntry>()
                {
                    new WebDeployParameterEntry()
                    {
                        Kind = "XmlFile",
                        Match = "/configuration/connectionStrings/add[@name='FirstConnectionString']/@connectionString",
                        Scope = @"\\web.config$"
                    }
                }
            });
            results.Add(new WebDeployParameter()
            {
                Name = "SecondConnectionString-Web.config Connection String",
                DefaultValue = "server=localhost;database=SecondDb;uid=myUser;password=myPass;",
                Description = "SecondConnectionString Connection String used in web.config by the application to access the database.",
                Entries = new List<WebDeployParameterEntry>()
                {
                    new WebDeployParameterEntry()
                    {
                        Kind = "XmlFile",
                        Match = "/configuration/connectionStrings/add[@name='SecondConnectionString']/@connectionString",
                        Scope = @"\\web.config$"
                    }
                }
            });

            return results;
        }

        private ICollection<WebDeployParameter> GetBasicParameters(bool includeAutoConnectionParameters)
        {
            var results = new List<WebDeployParameter>();
            results.Add(new WebDeployParameter()
            {
                Name = "FirstParameter",
                DefaultValue = "FirstValue",
                Description = "Description of FirstParameter",
                Entries = new List<WebDeployParameterEntry>()
                {
                    new WebDeployParameterEntry()
                    {
                        Kind = "XmlFile",
                        Match = "/configuration/appSettings/add[@key='FirstAppSetting']/@value",
                        Scope = @"\\web.config$"
                    }
                }
            });
            results.Add(new WebDeployParameter()
            {
                Name = "SecondParameter",
                DefaultValue = "SecondValue",
                Description = "Description of SecondParameter",
                Entries = new List<WebDeployParameterEntry>()
                {
                    new WebDeployParameterEntry()
                    {
                        Kind = "XmlFile",
                        Match = "/configuration/appSettings/add[@key='SecondAppSetting']/@value",
                        Scope = @"\\web.config$"
                    }
                }
            });

            results.Add(new WebDeployParameter()
            {
                Name = "IIS Web Application Name",
                DefaultValue = defaultProjectName
            });

            if (includeAutoConnectionParameters)
            {
                results.AddRange(GetAutoParameters());
            }

            return results;
        }
    }
}
