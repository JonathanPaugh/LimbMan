using System;
using System.IO;
using System.Net;
using System.Text;
using Jape;
using UnityEngine;

namespace JapeNet
{
    public partial class Response
    {
        public class Body
        {
            public string ToJson() => JsonUtility.ToJson(this);
        }
    }
}
