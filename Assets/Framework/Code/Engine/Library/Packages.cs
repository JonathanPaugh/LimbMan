using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Jape
{
	public static class Packages
    {
        private static object Access(Assembly assembly, string instance, string target, Func<Type[], Type> classSolver = null, Func<MemberInfo[], MemberInfo> targetSolver = null, params object[] args)
        {
            return Member.Static(assembly, instance, target, classSolver, targetSolver).Get(args);
        }

        private static Type NonGenericSolver(Type[] types) { return types.FirstOrDefault(type => !type.IsGenericType); }

        public static class ProBuilder
        {
            private static Assembly Engine() { return Assembly.Load("Unity.Probuilder"); }
            private static Assembly Editor() { return Assembly.Load("Unity.Probuilder.Editor"); }

            public static void InitMesh(object mesh) { AccessEditorUtility("InitObject", null, mesh); }

            public static ProBuilderMesh CreateCube() { return (ProBuilderMesh)AccessShapeGenerator("GenerateCube", null, AccessEditorUtility("newShapePivotLocation"), Vector3.one); }

            public static object AccessMaterialEditor(string target, Func<MemberInfo[], MemberInfo> solver = null, params object[] args)
            {
                return Access(Editor(), "MaterialEditor", target, NonGenericSolver, solver, args);
            }

            public static object AccessVertexColor(string target, Func<MemberInfo[], MemberInfo> solver = null, params object[] args)
            {
                return Access(Editor(), "VertexColorPalette", target, null, solver, args);
            }

            public static object AccessShapeGenerator(string target, Func<MemberInfo[], MemberInfo> solver = null, params object[] args)
            {
                return Access(Engine(), "ShapeGenerator", target, null, solver, args);
            }

            public static object AccessEditorUtility(string target , Func<MemberInfo[], MemberInfo> solver = null, params object[] args)
            {
                return Access(Editor(), "EditorUtility", target, NonGenericSolver, solver, args);
            }

            public static object AccessMeshSelection(string target, Func<MemberInfo[], MemberInfo> solver = null, params object[] args)
            {
                return Access(Editor(), "MeshSelection", target, NonGenericSolver, solver, args);
            }

            public static Type MaterialPalette() { return Member.Class(Editor(), "MaterialPalette"); }
            public static Type ColorPalette() { return Member.Class(Engine(), "ColorPalette"); }
            public static Type TriggerBehaviour() { return Member.Class(Engine(), "TriggerBehaviour"); }
            public static Type ColliderBehaviour() { return Member.Class(Engine(), "ColliderBehaviour"); }
        }

        public static class SpriteShape
        {
            private static Assembly Engine() { return Assembly.Load("Unity.2D.SpriteShape.Runtime"); }
            private static Assembly Editor() { return Assembly.Load("Unity.2D.SpriteShape.Editor"); }
        }
    }
}