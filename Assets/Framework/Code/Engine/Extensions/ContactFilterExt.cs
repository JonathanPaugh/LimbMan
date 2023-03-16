using UnityEngine;

namespace Jape
{
	public static class ContactFilterExt
    {
        public static ContactFilter2D CollisionFilter(this ContactFilter2D filter)
        {
            filter.useTriggers = false;
            return filter;
        }

        public static ContactFilter2D TriggerFilter(this ContactFilter2D filter)
        {
            filter.useTriggers = true;
            return filter;
        }
    }
}