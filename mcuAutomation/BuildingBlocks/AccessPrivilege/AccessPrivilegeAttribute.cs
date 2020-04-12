using System;

namespace AccessPrivilege
{
    public enum AccessPrivilegeGroup : int {ALL =          0x000001, 
                                            GUEST =        0x000010, 
                                            USER =         0x000100, 
                                            DEVELOPER =    0x001000, 
                                            SYSTEM_ADMIN = 0x010000 };
    
    [Serializable]
    [AttributeUsage(AttributeTargets.All,
        AllowMultiple = true,
        Inherited = true)]
    abstract public class AccessPrivilegeAttribute : Attribute
    {
        public AccessPrivilegeGroup AccessPrivilegeGroup { get; set; }

        public AccessPrivilegeAttribute()
        {
        }
    }
    public class AccessPrivilegeAllAttribute : AccessPrivilegeAttribute
    {
        public AccessPrivilegeAllAttribute() : base()
        {
            AccessPrivilegeGroup = AccessPrivilegeGroup.ALL;
        }
    }
    public class AccessPrivilegeGustAttribute : AccessPrivilegeAttribute
    {
        public AccessPrivilegeGustAttribute()
            : base()
        {
            AccessPrivilegeGroup = AccessPrivilegeGroup.GUEST;
        }
    }
    public class AccessPrivilegeUserAttribute : AccessPrivilegeAttribute
    {
        public AccessPrivilegeUserAttribute()
            : base()
        {
            AccessPrivilegeGroup = AccessPrivilegeGroup.USER;
        }
    }
    public class AccessPrivilegeDeveloperAttribute : AccessPrivilegeAttribute
    {
        public AccessPrivilegeDeveloperAttribute()
            : base()
        {
            AccessPrivilegeGroup = AccessPrivilegeGroup.DEVELOPER;
        }
    }
    public class AccessPrivilegeAdminAttribute : AccessPrivilegeAttribute
    {
        public AccessPrivilegeAdminAttribute()
            : base()
        {
            AccessPrivilegeGroup = AccessPrivilegeGroup.SYSTEM_ADMIN;
        }
    }
}
