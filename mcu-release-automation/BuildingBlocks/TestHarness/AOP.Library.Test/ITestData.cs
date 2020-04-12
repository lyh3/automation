using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

//using AOP.Library.Authorization;
//using BuildingBlocks.Shared;

namespace McAfeeLabs.Engineering.Automation.AOP
{
    public interface ITestData : INotifyPropertyChanged
    {
        [SynchronizeAttribute(FromMilliseconds = 10)]
        long Hits { get; set; }

        void Visit();

        [IntegerRangeValidator(MinVal = 0, MaxVal = 20)]
        [SynchronizeAttribute()]
        int IntVal { get; set; }

        [RegExValidator(RegularExpression = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$")]
        [SynchronizeAttribute()]
        [PropertyChangedNotification]
        string Email { get; set; }

        [StringLengthValidator(MinVal = 1, MaxVal = 30)]
        string String { get; set; }

        [FloatRangeValidator(MinVal = 0f, MaxVal = 20f)]
        [PropertyChangedNotification]
        float Float { get; set; }

        [LongRangeValidator(MinVal = 0L, MaxVal = 100000L)]
        long Long { get; set; }

        int TimeCosummingAdd(int a, int b);
        //[AuthorizationAttribute(Tag = AccessPrivilegeGroup.SYSTEM_ADMIN)]
        void AccessSecess();
        //[AuthorizationAttribute(Tag = AccessPrivilegeGroup.USER)]
        void AccessDenail();
    }
}
