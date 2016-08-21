// Copyright (c) Adam Ralph. (adam@adamralph.com)
namespace DudeTests.Acceptance.Console.Support
{
    using System.IO;

    public sealed class ScenarioDirectory
    {
        private static readonly string RootDirectory = "scenarios";

        private readonly string name;

        private ScenarioDirectory(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return this.name; }
        }

        public static ScenarioDirectory Create(string scenario)
        {
            var name = Path.Combine(RootDirectory, scenario);
            FileSystem.EnsureDirectoryDeleted(name);
            FileSystem.EnsureDirectoryCreated(name);
            return new ScenarioDirectory(name);
        }

        public ScenarioDirectory WriteLine(string fileName, string text)
        {
            fileName = Path.Combine(this.name, fileName);
            FileSystem.EnsureDirectoryCreated(Path.GetDirectoryName(fileName));
            using (var writer = new StreamWriter(fileName, true))
            {
                writer.WriteLine(text);
                writer.Flush();
            }

            return this;
        }
    }
}
