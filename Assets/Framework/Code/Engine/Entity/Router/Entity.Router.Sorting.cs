using System;
using UnityEngine;

namespace Jape
{
    public partial class Entity
    {
        public partial class Router
        {
            [Serializable]
            internal class Sorting
            {
                internal enum Mode { Ascend, Descend }

                [SerializeField, HideInInspector] private Routing.Field? field;
                [SerializeField, HideInInspector] private Mode? sorting;

                private Cache<GUIContent> ascendingIcon = new(() =>
                {
                    return new GUIContent(Database.GetAsset<Texture2D>("IconUp").Load<Texture2D>());
                });

                private Cache<GUIContent> descendingIcon = new(() =>
                {
                    return new GUIContent(Database.GetAsset<Texture2D>("IconDown").Load<Texture2D>());
                });

                internal GUIContent GetContent(Routing.Field field)
                {
                    if (IsMatch(field)) { return GetIcon(); }
                    return GUIContent.none;
                }

                internal void Sort(Router router)
                {
                    if (field == null) { return; }
                    switch (sorting)
                    {
                        case Mode.Ascend: router.SortAscend((Routing.Field)field); break;
                        case Mode.Descend: router.SortDescend((Routing.Field)field); break;
                    }
                }

                internal void ToggleSorting(Routing.Field field, Router router)
                {
                    if (IsMatch(field)) { sorting = NextSorting(); }
                    else { SetField(field); }
                    Sort(router);
                }

                private void SetField(Routing.Field field)
                {
                    this.field = field;
                    sorting = Mode.Descend;
                }

                private Mode? NextSorting()
                {
                    switch (sorting)
                    {
                        case Mode.Ascend: return null;
                        case Mode.Descend: return Mode.Ascend;
                        default: return Mode.Descend;
                    }
                }

                private bool IsMatch(Routing.Field field)
                {
                    switch (field)
                    {
                        default: return false;
                        case Routing.Field.Output:
                            if (this.field == Routing.Field.Output) { return true; }
                            return false;

                        case Routing.Field.Target:
                            if (this.field == Routing.Field.Target) { return true; }
                            return false;

                        case Routing.Field.Action:
                            if (this.field == Routing.Field.Action) { return true; }
                            return false;

                        case Routing.Field.Parameters:
                            if (this.field == Routing.Field.Parameters) { return true; }
                            return false;

                        case Routing.Field.Delay:
                            if (this.field == Routing.Field.Delay) { return true; }
                            return false;
                    }
                }

                private GUIContent GetIcon()
                {
                    switch (sorting)
                    {
                        case Mode.Ascend: return ascendingIcon.Value;
                        case Mode.Descend: return descendingIcon.Value;
                        default: return GUIContent.none;
                    }
                }
            }
        }
    }
}