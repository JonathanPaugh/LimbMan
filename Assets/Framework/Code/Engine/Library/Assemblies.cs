using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UnityEngine;

namespace Jape
{
	public static class Assemblies
    {
        public static Assembly Core => Assembly.Load("mscorlib");
        public static Assembly UnityCore => Assembly.Load("UnityEngine.CoreModule");

        public static Assembly FrameworkEngine => Assembly.Load(Framework.Settings.frameworkEngine.name);
        public static Assembly FrameworkEditor => Assembly.Load(Framework.Settings.frameworkEditor.name);
        public static IEnumerable<Assembly> FrameworkAssemblies => Framework.Settings.frameworkAssemblies.Select(a => Assembly.Load(a.name));

        public static Assembly GameEngine => Assembly.Load(Framework.Settings.gameEngine.name);
        public static Assembly GameEditor => Assembly.Load(Framework.Settings.gameEditor.name);
        public static IEnumerable<Assembly> GameAssemblies => Framework.Settings.gameAssemblies.Select(a => Assembly.Load(a.name));

        public static IEnumerable<Assembly> GetAll() { return AppDomain.CurrentDomain.GetAssemblies(); }
        public static IEnumerable<Assembly> GetFull() { return GetJape().Prepend(UnityCore); }

        public static IEnumerable<Assembly> GetJape()
        {
            return new [] { FrameworkEngine, FrameworkEditor, GameEngine, GameEditor }
                          .Concat(FrameworkAssemblies)
                          .Concat(GameAssemblies);
        }

        public static IEnumerable<Assembly> GetJapeRuntime()
        {
            return new [] { FrameworkEngine, GameEngine }
                          .Concat(FrameworkAssemblies)
                          .Concat(GameAssemblies);
        }

        public static IEnumerable<Assembly> GetJapeEngine() { return new [] { FrameworkEngine, GameEngine }; }
        public static IEnumerable<Assembly> GetJapeEditor() { return new [] { FrameworkEditor, GameEditor }; }
    }
}