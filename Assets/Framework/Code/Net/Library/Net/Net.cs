using System;
using System.Collections.Generic;
using System.Linq;
using Jape;
using UnityEngine;

namespace JapeNet
{
	public static partial class Net
    {
        private static void OfflineAccessError()
        {
            Log.Write("Cannot call this command in offline mode");
        }
    }
}