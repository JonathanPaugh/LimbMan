using System.Collections.Generic;
using UnityEngine;

namespace Jape
{
	public interface ISaveable
    {
        string Key { get; }
        byte[] Data { get; }

        byte[] Serialize();
        int Deserialize(byte[] data);
    }
}