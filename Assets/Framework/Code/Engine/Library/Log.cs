using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Jape
{
    public static class Log
    {

        public static void Write(params object[] inputs) { Process(inputs, s => Print(s, LogType.Log)); }
        public static void Warning(params object[] inputs) { Process(inputs, s => Print(s, LogType.Warning)); } 
        public static void Error(params object[] inputs) { Process(inputs, s => Print(s, LogType.Error)); } 

        private static string Format(object line1) { return LineStart() + line1.Filter(); }
        private static string Format(object line1, object line2) { return LineStart() + line1.Filter() + NewLine() + LineStart() + line2.Filter(); }

        private static string Filter(this object input) { return input == null ? "null" : input.ToString().RemoveSuffix(); }

        private static void Process(object[] inputs, Action<string> output)
        {
            object storedInput = null;
            bool used = true;

            for (int i = 0; i < inputs.Length; i++)
            {
                if (i.IsEven()) { storedInput = inputs[i]; used = false; }
                else { output(Format(storedInput, inputs[i])); storedInput = null; used = true; }
            }

            if (!used) { output(Format(storedInput)); }
        }

        private static void Print(object message, LogType logType)
        {
            StackTraceLogType storedType = Application.GetStackTraceLogType(logType);
            Application.SetStackTraceLogType(logType, StackTraceLogType.None);
            object output = Game.IsBuild ? message : message + NewLine() + GetStack();
            switch (logType)
            {
                case LogType.Log:
                    Debug.Log(output);
                    break;

                case LogType.Warning:
                    Debug.LogWarning(output);
                    break;
            }
            Application.SetStackTraceLogType(logType, storedType);
        }

        private static string GetStack()
        {
            string[] stack = StackTraceUtility
                             .ExtractStackTrace()
                             .Split(new [] { "\r\n", "\n" }, StringSplitOptions.None);

            List<string> richStack = new List<string>();

            int i = stack.Length - 1;
            while (i >= 0)
            {
                string stackLine = stack[i];

                if (stackLine.Contains("Jape.Log")) { break; }

                Regex pattern = new Regex(@"\(at (.*):(\d*)\)");
                Match match = pattern.Match(stackLine);

                if (match.Success)
                {
                    string path = match.Groups[1].Value;
                    string lineNumber = match.Groups[2].Value;
                    richStack.Insert(0, pattern.Replace(stackLine,"(" + $"<a href=\"{path}\" line=\"{lineNumber}\">{path}:{lineNumber}</a>" + ")"));
                } else {
                    richStack.Insert(0, stackLine);
                }

                i--;
            }

            return string.Join(NewLine(), richStack.ToArray());
        }
        
        public static string Timestamp()
        {
            if (Game.IsBuild) { return $"[{DateTime.Now:HH:mm} {DateTime.Now:ss:fff}]"; } 
            else { return $"[<size=8>{DateTime.Now:HH:mm}</size> <size=10>{DateTime.Now:ss:fff}</size>]"; }
        }

        public static string FrameStamp()
        {
            if (Game.IsBuild) { return $"[{Time.FrameCount()}]"; }
            else { return $"[<b>{Time.FrameCount()}</b>]"; }
        }

        public static string LineStart() { return $"{Timestamp() + FrameStamp()} "; }

        public static string NewLine() { return Environment.NewLine; }
    }
}