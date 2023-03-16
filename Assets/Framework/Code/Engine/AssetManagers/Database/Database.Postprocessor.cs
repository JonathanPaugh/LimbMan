using System.Collections.Generic;
using System.Linq;

namespace Jape
{
    public partial class Database
    {   
        #if UNITY_EDITOR

        internal class PostprocessorEditor : UnityEditor.AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                Database database = Instance;

                if (database == null) { return; }
                if (!database.UpdateAutomatically) { return; }

                IEnumerable<string> paths = importedAssets.Concat(deletedAssets).Concat(movedAssets).Concat(movedFromAssetPaths);
                bool update = paths.Select(path => path.ToLower()).Any(path => !path.Contains("database.asset") && path.Contains("/resources/"));

                if (update)
                {
                    database.UpdateDatabase();
                    database.OnAfterDeserialize();
                }
            }
        }

        #endif
    }
}