// Copyright (c) Adam Ralph. (adam@adamralph.com)
namespace DudeTests.Acceptance.Console
{
    using DudeTests.Acceptance.Console.Support;
    using FluentAssertions;
    using Xbehave;

    public class FileExecution
    {
        [Scenario]
        public void HelloWorld(ScenarioDirectory directory, string output)
        {
            "Given a hello world script"
                .f(c => directory = ScenarioDirectory.Create(c.Step.Scenario.DisplayName).WriteLine(
                    "hello.csx", @"Console.WriteLine(""Hello, "" + ""World!"");"));

            "When I execute the script"
                .f(() => output = DudeConsole.Execute("hello.csx", directory));

            "Then I see 'Hello, World!'"
                .f(() => output.Should().NotBeNull().And.Subject.Trim().Should().Be("Hello, World!"));
        }
    }
}
