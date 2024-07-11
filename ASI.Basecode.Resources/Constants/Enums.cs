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
            Active,
            Restricted
        }

        /// <summary>
        /// ROLES
        /// </summary>
        public enum UserRoleManager
        {
            Admin = 1,
            Regular = 2,
            Manager = 3
        }
    }
}
