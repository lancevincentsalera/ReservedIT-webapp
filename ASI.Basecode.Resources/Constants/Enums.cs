namespace ASI.Basecode.Resources.Constants
{
    /// <summary>
    /// Class for enumerated values
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// API Result Status
        /// </summary>
        public enum Status
        {
            Success,
            Error,
            CustomErr,
        }

        /// <summary>
        /// Login Result
        /// </summary>
        public enum LoginResult
        {
            Success = 0,
            Failed = 1,
            Restricted = -1,
            Pending = 2,
        }

        /// <summary>
        /// ACCOUNT STATUS
        /// </summary>
        public enum UserAccountStatus
        {
            ACTIVE,
            RESTRICTED
        }

        /// <summary>
        /// ROLES
        /// </summary>
        public enum UserRoleManager
        {
            ROLE_SUPER = 0,
            ROLE_ADMIN = 1,
            ROLE_REGULAR = 2,
            ROLE_MANAGER = 3,
        }
    }
}
