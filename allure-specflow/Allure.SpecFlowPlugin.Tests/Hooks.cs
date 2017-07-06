﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Allure.SpecFlowPlugin.Tests
{
    public enum TestOutcome { passed, failed, broken, hang }

    [Binding]
    public class Hooks
    {
        FeatureContext featureContext;
        ScenarioContext scenarioContext;
        public Hooks(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;

        }

        [StepDefinition(@"Step is '(.*)'")]
        public void StepResultIs(TestOutcome outcome)
        {
            Thread.Sleep(50);
            switch (outcome)
            {
                case TestOutcome.passed:
                    break;
                case TestOutcome.failed:
                    throw new AssertionException("This test is failed");
                case TestOutcome.broken:
                    throw new Exception("This test has error");
                case TestOutcome.hang:
                    Thread.Sleep(3000);
                    break;
                default:
                    throw new ArgumentException("value is not supported");
            }
        }

        [StepDefinition("Step with attachment")]
        public void StepWithAttach()
        {
            var path = Guid.NewGuid().ToString();
            File.WriteAllText(path, "hi there");
            featureContext.Get<Allure>().Attach(path);
        }

        [StepDefinition("Step with table")]
        public void StepWithTable(Table table)
        {
        }

        [StepDefinition("Step with params: (.*), (.*), (.*)")]
        public void StepWithArgs(int number, string text, DateTime date)
        {
        }

        [BeforeTestRun]
        public static void SetTestFolderForNUnit()
        {
            var dir = Path.GetDirectoryName(typeof(Hooks).Assembly.Location);
            Environment.CurrentDirectory = dir;
        }


        [BeforeScenario(Order = 1)]
        public void AllwaysPassingBeforeScenario()
        {
        }

        [AfterScenario(Order = 1)]
        public void AllwaysPassingAfterScenario()
        {
        }

        [BeforeFeature(tags: "BeforeFeature")]
        [AfterFeature(tags: "AfterFeature")]
        public static void HandleFeature(FeatureContext featureContext)
        {
            Handle(featureContext, featureContext.FeatureInfo.Tags);
        }


        [BeforeScenario(tags: "BeforeScenario")]
        [AfterScenario(tags: "AfterScenario")]
        [BeforeStep(tags: "BeforeStep")]
        [AfterStep(tags: "AfterStep")]
        public void HandleIt()
        {
            Handle(featureContext, scenarioContext.ScenarioInfo.Tags);
        }
        private static void Handle(FeatureContext featurecontext, string[] tags)
        {
            if (tags.Contains("attachment"))
            {
                var path = $"{Guid.NewGuid().ToString()}.txt";
                File.WriteAllText(path, "hi there");
                featurecontext.Get<Allure>()
                    .Attach(path)
                    .Attach(path, "text file");
            }
            if (tags.Any(x => x.StartsWith("fail")))
                throw new Exception("Wasted");
        }

    }
}