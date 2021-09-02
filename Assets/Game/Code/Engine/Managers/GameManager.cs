using System;
using System.Collections.Generic;
using System.Linq;
using Jape;
using JapeNet;
using JapeNet.Client;
using JapeNet.Server;
using UnityEngine;

namespace Game
{
    public sealed class GameManager : Manager<GameManager>
    {
        private new static bool InitOnLoad => true;
    }
}