using System.Collections.Generic;

namespace Jape
{
    public interface IRestrictor<in T>
    {
        bool IsRestricted();
        void Restrict(T restrictor);
        void Unrestrict(T restrictor);
    }

    public interface ISelfRestrictor<out TSelf, in TRestrictor>
    {
        bool IsRestricted();
        TSelf Restrict(TRestrictor restrictor);
        TSelf Unrestrict(TRestrictor restrictor);
    }
}