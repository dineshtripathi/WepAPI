﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.1.0.0
//      SpecFlow Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Flex.Entity.Api.Integration.Tests.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.1.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("TagTypes")]
    public partial class TagTypesFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "TagTypes.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "TagTypes", "\tIn order to manage in Tag Type in Flex\r\n\tAs a OE user\r\n\tI want to be able to cre" +
                    "ate tag type and delete tag type", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("TT01 Create a Tag Type")]
        [NUnit.Framework.CategoryAttribute("PAT")]
        public virtual void TT01CreateATagType()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TT01 Create a Tag Type", new string[] {
                        "PAT"});
#line 12
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Name",
                        "Value"});
            table1.AddRow(new string[] {
                        "RelativeUrl",
                        "/entities/tags/"});
            table1.AddRow(new string[] {
                        "Accept",
                        "application/json;"});
#line 13
 testRunner.Given("a request to create a new tag type with tag type name \'ipaddress\'", ((string)(null)), table1, "Given ");
#line 17
 testRunner.When("the client makes a POST request with the body \'{ key : \"ipaddress\"}\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 18
 testRunner.Then("the Api returns with response code \'201\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 19
 testRunner.And("the response contains the newly created tags type details", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("TT02 Create Tag Type which is already exist in Flex")]
        [NUnit.Framework.CategoryAttribute("NAT")]
        public virtual void TT02CreateTagTypeWhichIsAlreadyExistInFlex()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TT02 Create Tag Type which is already exist in Flex", new string[] {
                        "NAT"});
#line 22
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Name",
                        "Value"});
            table2.AddRow(new string[] {
                        "RelativeUrl",
                        "/entities/tags/"});
            table2.AddRow(new string[] {
                        "Accept",
                        "application/json;"});
#line 23
 testRunner.Given("a request to create a new tag type with tag type name \'ipaddress\'", ((string)(null)), table2, "Given ");
#line 27
 testRunner.When("the client makes a POST request with the body \'{ key : \"ipaddress\"}\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 28
 testRunner.Then("the Api returns with response code \'400\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 29
 testRunner.And("the response contains sucess \'false\' with error message \'key = ipaddress already " +
                    "exisits\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("TT03 Delete Tag Type which exist without attached with entities")]
        [NUnit.Framework.CategoryAttribute("PAT")]
        public virtual void TT03DeleteTagTypeWhichExistWithoutAttachedWithEntities()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TT03 Delete Tag Type which exist without attached with entities", new string[] {
                        "PAT"});
#line 32
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Name",
                        "Value"});
            table3.AddRow(new string[] {
                        "RelativeUrl",
                        "/entities/tags/"});
            table3.AddRow(new string[] {
                        "Accept",
                        "application/json;"});
#line 33
 testRunner.Given("a a request to delete a tag type with name \'ipaddress\'", ((string)(null)), table3, "Given ");
#line 37
 testRunner.When("the client makes a DELETE request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 38
 testRunner.Then("the Api returns with response code \'200\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 39
 testRunner.And("the response contains sucess \'true\' with error message \'\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("TT04 Delete Tag Type without exist")]
        [NUnit.Framework.CategoryAttribute("NAT")]
        public virtual void TT04DeleteTagTypeWithoutExist()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TT04 Delete Tag Type without exist", new string[] {
                        "NAT"});
#line 43
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "Name",
                        "Value"});
            table4.AddRow(new string[] {
                        "RelativeUrl",
                        "/entities/tags/"});
            table4.AddRow(new string[] {
                        "Accept",
                        "application/json;"});
#line 44
 testRunner.Given("a a request to delete a tag type with name \'X\'", ((string)(null)), table4, "Given ");
#line 48
 testRunner.When("the client makes a DELETE request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 49
 testRunner.Then("the Api returns with response code \'404\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 50
 testRunner.And("the response contains sucess \'false\' with error message \'tag type with key = X no" +
                    "t found.\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("TT05 Delete Tag Type which uses in entities")]
        [NUnit.Framework.IgnoreAttribute("Ignored scenario")]
        [NUnit.Framework.CategoryAttribute("NAT")]
        [NUnit.Framework.CategoryAttribute("/*")]
        [NUnit.Framework.CategoryAttribute("Not")]
        [NUnit.Framework.CategoryAttribute("implemented")]
        [NUnit.Framework.CategoryAttribute("*/")]
        public virtual void TT05DeleteTagTypeWhichUsesInEntities()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TT05 Delete Tag Type which uses in entities", new string[] {
                        "NAT",
                        "ignore",
                        "/*",
                        "Not",
                        "implemented",
                        "*/"});
#line 54
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Name",
                        "Value"});
            table5.AddRow(new string[] {
                        "RelativeUrl",
                        "/entities/tags/"});
            table5.AddRow(new string[] {
                        "Accept",
                        "application/json;"});
#line 55
 testRunner.Given("a a request to delete a tag type with name \'hostid\'", ((string)(null)), table5, "Given ");
#line 59
 testRunner.When("the client makes a DELETE request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 60
 testRunner.Then("the Api returns with response code \'404\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 61
 testRunner.And("the response contains sucess \'false\' with error message \'Not found\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("TT06 GET Tag Types - Get tag type collections")]
        [NUnit.Framework.CategoryAttribute("PAT")]
        public virtual void TT06GETTagTypes_GetTagTypeCollections()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TT06 GET Tag Types - Get tag type collections", new string[] {
                        "PAT"});
#line 65
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "Name",
                        "Value"});
            table6.AddRow(new string[] {
                        "RelativeUrl",
                        "/entities/tags"});
            table6.AddRow(new string[] {
                        "Accept",
                        "application/json;"});
#line 66
 testRunner.Given("a request for all tag types", ((string)(null)), table6, "Given ");
#line 70
 testRunner.When("the client makes a GET request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 71
 testRunner.Then("the Api returns with response code \'200\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 72
 testRunner.And("the response contains a collection of tag type details", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("TT07 GET Tag Types - Get tag type")]
        [NUnit.Framework.CategoryAttribute("PAT")]
        public virtual void TT07GETTagTypes_GetTagType()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TT07 GET Tag Types - Get tag type", new string[] {
                        "PAT"});
#line 76
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "Name",
                        "Value"});
            table7.AddRow(new string[] {
                        "RelativeUrl",
                        "/entities/tags"});
            table7.AddRow(new string[] {
                        "Accept",
                        "application/json;"});
#line 77
 testRunner.Given("a request for an tag types with prefix \'temp\'", ((string)(null)), table7, "Given ");
#line 81
 testRunner.When("the client makes a GET request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 82
 testRunner.Then("the Api returns with response code \'200\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 83
 testRunner.And("the response contains tag type details", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
