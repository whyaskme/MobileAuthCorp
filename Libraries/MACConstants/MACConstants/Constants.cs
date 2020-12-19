using System;

using MongoDB.Bson;

namespace MACServices
{
    public static class Constants
    {
        #region Common
        public static class Common
        {
            public const string ItemSep = "|";          // Pipe(|) used as item seperator
            public const string KVSep = ":";
            public const string RequestId = "RequestId";
            public const string OTP = "OTP";
            public const string Ad = "Ad";
            public const string True = "True";
            public const string False = "False";
            public const string Disabled = "Disabled";

            public const string EmptyOwnerLogoUrl = "/Images/OwnerLogos/!Empty-Placeholder.png";
        }
        #endregion

        #region Event class range initialization

        public const Int32 EventLogAssignmentStartRange = 0;
            public const Int32 EventLogClientStartRange = 1000;
            public const Int32 EventLogExceptionStartRange = 2000;
            public const Int32 EventLogGroupStartRange = 3000;
            public const Int32 EventLogMessagingStartRange = 4000;
            private const Int32 EventLogBillingStartRange = 5000;
            //private const Int32 EventLogOtpStartRange = 6000;
            public const Int32 EventLogRegistrationStartRange = 7000;
            public const Int32 EventLogSecurityStartRange = 8000;
            public const Int32 EventLogSystemStartRange = 9000;
            //public const Int32 EventLogUserRolesStartRange = 10000;
            public const Int32 EventLogUserVerificationStartRange = 11000;

        #endregion

        #region String class range initialization

            //private const Int32 AppStringsOtpStartRange = 25000;
            private const Int32 AppStringsUserRolesStartRange = 26000;

        #endregion

        #region TokenKeys
        public static class TokenKeys
        {
            public const string ItemSep = "|";          // Pipe(|) used as item seperator
            public const string KVSep = ":";            // Colon(:) used as Key/value seperator

            public static string AccountName = ItemSep + "AccountName" + KVSep;
            public static string AccountNumber = ItemSep + "AccountNumber" + KVSep;
            public static string AdminFullName = ItemSep + "AdminFullName" + KVSep;

            public static string AllowedIPAddresses = ItemSep + "AllowedIPAddresses" + KVSep;

            public static string BillAmount = ItemSep + "BillAmount" + KVSep;
            public static string BillId = ItemSep + "BillId" + KVSep;
            public static string BillSentTo = ItemSep + "BillSentTo" + KVSep;

            public static string ChildGroupName = ItemSep + "ChildGroupName" + KVSep;
            public static string ClientApiAppDeleted = ItemSep + "ClientApiAppDeleted" + KVSep;
            public static string ClientName = ItemSep + "ClientName" + KVSep;
            public static string ClientGroupRestricted = ItemSep + "ClientGroupRestricted" + KVSep;
            public static string DatabaseDropped = ItemSep + "DatabaseDropped" + KVSep;
            public static string DatabaseCollectionName = ItemSep + "DatabaseCollectionName" + KVSep;
            public static string DatabaseCollectionObjectsCount = ItemSep + "DatabaseCollectionObjectsCount" + KVSep;
            public static string DatabaseRestoredFromBackup = ItemSep + "DatabaseRestoredFromBackup" + KVSep;
            public static string DatabaseSource = ItemSep + "DatabaseSource" + KVSep;
            public static string DatabaseTarget = ItemSep + "DatabaseTarget" + KVSep;
// ReSharper disable once MemberHidesStaticFromOuterClass
            public static string DeliveryMethod = ItemSep + "DeliveryMethod" + KVSep;
            public static string DeliveryMethodCurrent = ItemSep + "DeliveryMethodCurrent" + KVSep;
            public static string DeliveryMethodNext = ItemSep + "DeliveryMethodNext" + KVSep;
            public static string DuplicateEmail = ItemSep + "DuplicateEmail" + KVSep;
            public static string DuplicateUserName = ItemSep + "DuplicateUserName" + KVSep;
            public static string Email = ItemSep + "Email" + KVSep;
            public static string EndUserId = ItemSep + "EndUserId" + KVSep;
            public static string EventStatDate = ItemSep + "EventStatDate" + KVSep;
            public static string EventStatDetails = ItemSep + "EventStatDetails" + KVSep;
            public static string EventGeneratedByName = ItemSep + "EventGeneratedByName" + KVSep;
            public static string ExceptionDetails = "ExceptionDetails" + KVSep;
            public static string FailureDetails = ItemSep + "FailureDetails" + KVSep;
            public static string FromEmail = ItemSep + "FromEmail" + KVSep;
            public static string FromPhone = ItemSep + "FromPhone" + KVSep;
            public static string GroupName = ItemSep + "GroupName" + KVSep;
            public static string InvalidEmail = ItemSep + "InvalidEmail" + KVSep;
            public static string InvalidPassword = ItemSep + "InvalidPassword" + KVSep;
            public static string InvalidSecurityAnswer = ItemSep + "InvalidSecurityAnswer" + KVSep;
            public static string InvalidUserName = ItemSep + "InvalidUserName" + KVSep;
            public static string MessageCategory = ItemSep + "MessageCategory" + KVSep;
            public static string MessageProviderCurrent = ItemSep + "MessageProviderCurrent" + KVSep;
            public static string MessageProviderNext = ItemSep + "MessageProviderNext" + KVSep;
            public static string NewRole = ItemSep + "NewRole" + KVSep;
            public static string ObjectPropertiesUpdated = ItemSep + "ObjectPropertiesUpdated" + KVSep;
            public static string OTP = ItemSep + "OTP" + KVSep;
            public static string OTPCode = ItemSep + "OTPCode" + KVSep;
            public static string OTPExpired = ItemSep + "OTPExpired" + KVSep;
            public static string OTPExpiredTime = ItemSep + "OTPExpiredTime" + KVSep;
            public static string OTPInactive = ItemSep + "OTPInactive" + KVSep;
            public static string OTPInvalid = ItemSep + "OTPInvalid" + KVSep;
            public static string OTPRetriesCurrent = ItemSep + "OTPRetriesCurrent" + KVSep;
            public static string OTPRetriesMax = ItemSep + "OTPRetriesMax" + KVSep;
            public static string OTPValid = ItemSep + "OTPValid" + KVSep;
            public static string OwnerType = ItemSep + "OwnerType" + KVSep;
            public static string OwnerName = ItemSep + "OwnerName" + KVSep;
            public static string ParentGroupName = ItemSep + "ParentGroupName" + KVSep;
            public static string Password = ItemSep + "Password" + KVSep;
            public static string Phone = ItemSep + "Phone" + KVSep;
            public static string PreviousRole = ItemSep + "PreviousRole" + KVSep;
            public static string ProviderChangedValues = ItemSep + "ProviderChangedValues" + KVSep;
            public static string ProviderList = ItemSep + "ProviderList" + KVSep;
            public static string ProviderName = ItemSep + "ProviderName" + KVSep;
            public static string ProviderProperties = ItemSep + "ProviderProperties" + KVSep;
            public static string ProviderType = ItemSep + "ProviderType" + KVSep;
            public static string RequestId = ItemSep + "RequestId" + KVSep;
            public static string RequestType = ItemSep + "RequestType" + KVSep;
            public static string SecurityQuestion = ItemSep + "SecurityQuestion" + KVSep;
            public static string SecurityAnswer = ItemSep + "SecurityAnswer" + KVSep;
            public static string SentToAddress = ItemSep + "SentToAddress" + KVSep;
            public static string ServerIpAddress = ItemSep + "ServerIpAddress" + KVSep;
            public static string ServiceProviderName = ItemSep + "ServiceProviderName" + KVSep;
            public static string ServiceProviderUrl = ItemSep + "ServiceProviderUrl" + KVSep;
            public static string ServiceProviderRequestData = ItemSep + "ServiceProviderUrl" + KVSep;
            public static string ServiceProviderResponseData = ItemSep + "ServiceProviderUrl" + KVSep;
            public static string SubAccountName = ItemSep + "SubAccountName" + KVSep;
            public static string SubAccountNumber = ItemSep + "SubAccountNumber" + KVSep;
            public static string SubGroupName = ItemSep + "SubGroupName" + KVSep;
            public static string ToEmail = ItemSep + "ToEmail" + KVSep;
            public static string ToPhone = ItemSep + "ToPhone" + KVSep;
            public static string UnknownRegistrationError = ItemSep + "UnknownRegistrationError" + KVSep;
            public static string UpdatedByLoggedinAdminFullName = ItemSep + "UpdatedByLoggedinAdminFullName" + KVSep;
            public static string UpdatedValues = ItemSep + "UpdatedValues" + KVSep;
            public static string UserFullName = ItemSep + "UserFullName" + KVSep;
            public static string UserName = ItemSep + "UserName" + KVSep;
            public static string UserRole = ItemSep + "UserRole" + KVSep;
            public static string VerificationResponse = ItemSep + "VerificationResponse" + KVSep;

        }
        #endregion

        #region

        public static class Roles
        {
            public const string SystemAdministrator = "System Administrator";
            public const string GroupAdministrator = "Group Administrator";
            public const string ClientAdministrator = "Client Administrator";
            public const string AccountingUser = "Accounting User";
            public const string ClientUser = "Client User";
            public const string GroupUser = "Group User";
            public const string OperationsUser = "Operations User";
            public const string ViewOnlyUser = "View Only User";
        }

        #endregion

        #region EventLog
        public static class EventLog
        {
            public static class Assignments
            {
                public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogAssignmentStartRange, "Assignment Created (Base)", "Base Assignments Event Class");

                #region Admin and User relationship events

                    // Assignments
                    public static Tuple<int, string, string> AdminAssignedToClient = new Tuple<int, string, string>(Base.Item1 + 1, "Assignment Created (Admin To Client)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been assigned to Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> AdminAssignedToGroup = new Tuple<int, string, string>(AdminAssignedToClient.Item1 + 1, "Assignment Created (Admin To Group)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been assigned to Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> UserAssignedToClient = new Tuple<int, string, string>(AdminAssignedToGroup.Item1 + 1, "Assignment Created (User To Client)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been assigned to Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> UserAssignedToGroup = new Tuple<int, string, string>(UserAssignedToClient.Item1 + 1, "Assignment Created (User To Group)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been assigned to Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");


                    // Removals
                    public static Tuple<int, string, string> AdminRemovedFromClient = new Tuple<int, string, string>(UserAssignedToGroup.Item1 + 1, "Assignment Removed (Admin From Client)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> AdminRemovedFromGroup = new Tuple<int, string, string>(AdminRemovedFromClient.Item1 + 1, "Assignment Removed (Admin From Group)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> UserRemovedFromClient = new Tuple<int, string, string>(AdminRemovedFromGroup.Item1 + 1, "Assignment Removed (User From Client)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> UserRemovedFromGroup = new Tuple<int, string, string>(UserRemovedFromClient.Item1 + 1, "Assignment Removed (User From Group)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");


                    public static Tuple<int, string, string> AdminRelationshipsRemoved = new Tuple<int, string, string>(UserRemovedFromGroup.Item1 + 1, "Assignment Removed (Admin From Clients and Groups)", "All Administrators have been removed from Clients and Groups. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> AllRelationshipsRemoved = new Tuple<int, string, string>(AdminRelationshipsRemoved.Item1 + 1, "Assignment Removed (Admin, Clients and Groups)", "All relationships have been removed from Administrators, Clients and Groups. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                #endregion

                #region Client relationship events

                    // Assignments
                    public static Tuple<int, string, string> ClientAssignedToAdmin = new Tuple<int, string, string>(AllRelationshipsRemoved.Item1 + 1, "Assignment Created (Client To Admin)", "Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been assigned to Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> ClientAssignedToGroup = new Tuple<int, string, string>(ClientAssignedToAdmin.Item1 + 1, "Assignment Created (Client To Group)", "Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been assigned to Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> ClientAssignedToUser = new Tuple<int, string, string>(ClientAssignedToGroup.Item1 + 1, "Assignment Created (Client To User)", "Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been assigned to User ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                    // Removals
                    public static Tuple<int, string, string> ClientRemovedFromUser = new Tuple<int, string, string>(ClientAssignedToUser.Item1 + 1, "Assignment Removed (Client From User)", "Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from User ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> ClientRemovedFromAdmin = new Tuple<int, string, string>(ClientRemovedFromUser.Item1 + 1, "Assignment Removed (Client From Admin)", "Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> ClientRemovedFromGroup = new Tuple<int, string, string>(ClientRemovedFromAdmin.Item1 + 1, "Assignment Removed (Client From Group)", "Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                #endregion

                #region Group relationship events

                    // Assignments
                    public static Tuple<int, string, string> GroupAssignedToAdmin = new Tuple<int, string, string>(ClientRemovedFromGroup.Item1 + 1, "Assignment Created (Group To Admin)", "Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been assigned to Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> GroupAssignedToClient = new Tuple<int, string, string>(GroupAssignedToAdmin.Item1 + 1, "Assignment Created (Group To Client)", "Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been assigned to Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> GroupAssignedToGroup = new Tuple<int, string, string>(GroupAssignedToClient.Item1 + 1, "Assignment Created (Group To Group)", "Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been assigned to Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> ParentGroupAssignedToSubGroup = new Tuple<int, string, string>(GroupAssignedToGroup.Item1 + 1, "Assignment Created (Parent-Group To Sub-Group)", "Parent-Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been assigned to Sub-Group ([" + TokenKeys.ChildGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> SubGroupAssignedToParentGroup = new Tuple<int, string, string>(ParentGroupAssignedToSubGroup.Item1 + 1, "Assignment Created (Sub-Group To Parent-Group)", "Sub-Group ([" + TokenKeys.ChildGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been assigned to Parent-Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                    // Removals
                    public static Tuple<int, string, string> GroupRemovedFromAdmin = new Tuple<int, string, string>(SubGroupAssignedToParentGroup.Item1 + 1, "Assignment Removed (Group From Admin)", "Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> GroupRemovedFromClient = new Tuple<int, string, string>(GroupRemovedFromAdmin.Item1 + 1, "Assignment Removed (Group From Client)", "Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> GroupRemovedFromGroup = new Tuple<int, string, string>(GroupRemovedFromClient.Item1 + 1, "Assignment Removed (Group From Group)", "Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                public static Tuple<int, string, string> ParentGroupRemovedFromSubGroup = new Tuple<int, string, string>(GroupRemovedFromGroup.Item1 + 1, "Assignment Removed (Parent-Group From Sub-Group)", "Parent-Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from Sub-Group ([" + TokenKeys.ChildGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> SubGroupRemovedFromParentGroup = new Tuple<int, string, string>(ParentGroupRemovedFromSubGroup.Item1 + 1, "Assignment Removed (Sub-Group From Parent-Group)", "Sub-Group ([" + TokenKeys.ChildGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from Parent-Group ([" + TokenKeys.GroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                #endregion

                #region Account relationship events

                    // Assignments
                    public static Tuple<int, string, string> UserAssignedToAccount = new Tuple<int, string, string>(SubGroupRemovedFromParentGroup.Item1 + 1, "Assignment Created (User To Account)", "([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) added to primary account ([" + TokenKeys.AccountName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) account #([" + TokenKeys.AccountNumber.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> SubAccountAssignedToAccount = new Tuple<int, string, string>(UserAssignedToAccount.Item1 + 1, "Assignment Created (Sub-Account To Account)", "Sub-account ([" + TokenKeys.SubAccountName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + " - " + TokenKeys.SubAccountNumber.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been added to primary account ([" + TokenKeys.AccountName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + " - " + TokenKeys.AccountNumber.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                    // Removals
                    public static Tuple<int, string, string> UserRemovedFromAccount = new Tuple<int, string, string>(SubAccountAssignedToAccount.Item1 + 1, "Assignment Removed (User From Account)", "([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) removed from primary account ([" + TokenKeys.AccountName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) account #([" + TokenKeys.AccountNumber.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> SubAccountRemovedFromAccount = new Tuple<int, string, string>(UserRemovedFromAccount.Item1 + 1, "Assignment Removed (Sub-Account From Account)", "Sub-account ([" + TokenKeys.SubAccountName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + " - " + TokenKeys.SubAccountNumber.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been removed from primary account ([" + TokenKeys.AccountName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + " - " + TokenKeys.AccountNumber.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                #endregion

                public static class RoleAssignment
                {
                    // ReSharper disable once MemberHidesStaticFromOuterClass
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(SubAccountRemovedFromAccount.Item1 + 1, "Role Assignment (Base)", "Base Assignments (Roles) Event Class");
                    public static Tuple<int, string, string> Promoted = new Tuple<int, string, string>(Base.Item1 + 1, "Role Assignment (Promoted)", "([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been Promoted from [" + TokenKeys.PreviousRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] to [" + TokenKeys.NewRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Demoted = new Tuple<int, string, string>(Promoted.Item1 + 1, "Role Assignment (Demoted)", "([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been Demoted from [" + TokenKeys.PreviousRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] to [" + TokenKeys.NewRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }
            }

            #region Event Types
            public static class Billing
            {
                public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogBillingStartRange, "Billing (Base)", "Base Billing Event Class");
                public static Tuple<int, string, string> Created = new Tuple<int, string, string>(Base.Item1 + 1, "Billing (Created)", "[" + TokenKeys.BillAmount.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] Bill for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Created. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Sent = new Tuple<int, string, string>(Created.Item1 + 1, "Billing (Sent)", "[" + TokenKeys.BillAmount.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] Bill for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Sent to ([" + TokenKeys.BillSentTo.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Paid = new Tuple<int, string, string>(Sent.Item1 + 1, "Billing (Paid)", "[" + TokenKeys.BillAmount.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] Bill for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Paid. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Voided = new Tuple<int, string, string>(Paid.Item1 + 1, "Billing (Voided)", "[" + TokenKeys.BillAmount.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] Bill for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Voided. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
            }

            public static class Client
            {
                public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogClientStartRange, "Client (Base)", "Base Client Event Class");
                public static Tuple<int, string, string> Created = new Tuple<int, string, string>(Base.Item1 + 1, "Client (Created)", "Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Created. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Updated = new Tuple<int, string, string>(Created.Item1 + 1, "Client (Updated)", "Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Updated. [" + TokenKeys.ObjectPropertiesUpdated.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "].  Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Disabled = new Tuple<int, string, string>(Updated.Item1 + 1, "Client (Disabled)", "Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Disabled. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Enabled = new Tuple<int, string, string>(Disabled.Item1 + 1, "Client (Enabled)", "Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Enabled. Event generated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
            }

            public static class Exceptions
            {
                public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogExceptionStartRange, "Exceptions (Base)", "Base Exception Event Class");
                public static Tuple<int, string, string> General = new Tuple<int, string, string>(Base.Item1 + 1, "Exceptions (General)", "[" + TokenKeys.ExceptionDetails.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> LogSendOtpError = new Tuple<int, string, string>(General.Item1 + 1, "Exceptions (OTP Send Failure)", "[" + TokenKeys.ExceptionDetails.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] - Client: " + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + ", List: " + TokenKeys.ProviderList.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + " for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
            }

            public static class Failures
            {
                public static Tuple<int, string, string> Base = new Tuple<int, string, string>(Exceptions.LogSendOtpError.Item1 + 1, "Failures (Base)", "Base Failure Event Class");
                public static Tuple<int, string, string> General = new Tuple<int, string, string>(Base.Item1 + 1, "Failures (General)", "[" + TokenKeys.FailureDetails.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
            }

            public static class Group
            {
                public static class ParentGroup
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogGroupStartRange, "Group (Base)", "Base Group Event Class");
                    public static Tuple<int, string, string> Created = new Tuple<int, string, string>(Base.Item1 + 1, "Group (Created)", "Group ([" + TokenKeys.ParentGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Created by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Updated = new Tuple<int, string, string>(Created.Item1 + 1, "Group (Updated)", "Group ([" + TokenKeys.ParentGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Updated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Changes: [" + TokenKeys.ObjectPropertiesUpdated.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "].");
                    public static Tuple<int, string, string> Disabled = new Tuple<int, string, string>(Updated.Item1 + 1, "Group (Disabled)", "Group ([" + TokenKeys.ParentGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Disabled by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Enabled = new Tuple<int, string, string>(Disabled.Item1 + 1, "Group (Enabled)", "Group ([" + TokenKeys.ParentGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Enabled by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }
                public static class ChildGroup
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(ParentGroup.Enabled.Item1 + 1, "Child Group (Base)", "Base Group Event Class");
                    public static Tuple<int, string, string> Created = new Tuple<int, string, string>(Base.Item1 + 1, "Group (Sub-group Created)", "Sub-group ([" + TokenKeys.SubGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) of parent group ([" + TokenKeys.ParentGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Created by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Updated = new Tuple<int, string, string>(Created.Item1 + 1, "Group (Sub-group Updated)", "Sub-group ([" + TokenKeys.SubGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) of parent group ([" + TokenKeys.ParentGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Updated by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Changes: [" + TokenKeys.ObjectPropertiesUpdated.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "].");
                    public static Tuple<int, string, string> Disabled = new Tuple<int, string, string>(Updated.Item1 + 1, "Group (Sub-group Disabled)", "Sub-group ([" + TokenKeys.SubGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) of parent group ([" + TokenKeys.ParentGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Disabled by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Enabled = new Tuple<int, string, string>(Disabled.Item1 + 1, "Group (Sub-group Enabled)", "Sub-group ([" + TokenKeys.SubGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) of parent group ([" + TokenKeys.ParentGroupName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Enabled by ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }
            }

            public static class Messaging
            {
                public static class Email
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogMessagingStartRange, "Messaging (Email Base)", "Base Email Messaging Event Class");
                    public static Tuple<int, string, string> Sent = new Tuple<int, string, string>(Base.Item1 + 1, "Messaging (Email Sent)", "Email message ([" + TokenKeys.MessageCategory.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) sent to ([" + TokenKeys.ToEmail.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Recieved = new Tuple<int, string, string>(Sent.Item1 + 1, "Messaging (Email Recieved)", "Email message ([" + TokenKeys.MessageCategory.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) recieved from ([" + TokenKeys.FromEmail.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }

                public static class Sms
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(Email.Recieved.Item1 + 1, "Messaging (SMS Base)", "Base Sms Messaging Event Class");
                    public static Tuple<int, string, string> Sent = new Tuple<int, string, string>(Base.Item1 + 1, "Messaging (SMS Sent)", "SMS message ([" + TokenKeys.MessageCategory.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) sent to ([" + TokenKeys.ToPhone.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Recieved = new Tuple<int, string, string>(Sent.Item1 + 1, "Messaging (SMS Recieved)", "SMS message ([" + TokenKeys.MessageCategory.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) recieved from ([" + TokenKeys.FromPhone.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }

                public static class Voice
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(Sms.Recieved.Item1 + 1, "Messaging (Voice Base)", "Base Voice Messaging Event Class");
                    public static Tuple<int, string, string> Sent = new Tuple<int, string, string>(Base.Item1 + 1, "Messaging (Voice Sent)", "Voice message ([" + TokenKeys.MessageCategory.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) sent to ([" + TokenKeys.ToPhone.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Recieved = new Tuple<int, string, string>(Sent.Item1 + 1, "Messaging (Voice Recieved)", "Voice message ([" + TokenKeys.MessageCategory.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) recieved from ([" + TokenKeys.FromPhone.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }
            }

            public static class Oas
            {
                public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogGroupStartRange, "OAS (Base)", "Base Oas Event Class");
                public static Tuple<int, string, string> DeletedEndUser = new Tuple<int, string, string>(Base.Item1 + 1, "OAS (Deleted End User)", "OAS Successfully deleted End-User for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> FailedToDeleteEndUser = new Tuple<int, string, string>(DeletedEndUser.Item1 + 1, "OAS (Failed to Delete End User)", "OAS Failed to delete End-User for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
            }

            public static class Otp
            {
                public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogGroupStartRange, "OTP (Base)", "Base OTP Event Class");
                public static Tuple<int, string, string> RequestAdmin = new Tuple<int, string, string>(Base.Item1 + 1, "OTP Request (Admin Console)", "OTP Request (Admin Console) - Console OTP requested for User ([" + TokenKeys.EndUserId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) via ([" + TokenKeys.DeliveryMethod.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) at: ([" + TokenKeys.SentToAddress.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) with RequestId ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                public static Tuple<int, string, string> RequestClientTest = new Tuple<int, string, string>(RequestAdmin.Item1 + 1, "OTP Request (Client Test)", "OTP Request (Client Test) - ([" + TokenKeys.EndUserId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) requested an OTP test for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) via ([" + TokenKeys.DeliveryMethod.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) at: ([" + TokenKeys.SentToAddress.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) with RequestId ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                public static Tuple<int, string, string> Sent = new Tuple<int, string, string>(RequestClientTest.Item1 + 1, "OTP (Sent)", "OTP (Sent) ([" + TokenKeys.RequestType.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) via ([" + TokenKeys.DeliveryMethod.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) RequestId ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                public static Tuple<int, string, string> ReSent = new Tuple<int, string, string>(Sent.Item1 + 1, "OTP (Re-Sent)", "([" + TokenKeys.ClientGroupRestricted.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) - OTP (Re)sent to User ([" + TokenKeys.EndUserId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) via ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) at: ([" + TokenKeys.SentToAddress.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) with RequestId ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                public static Tuple<int, string, string> Validated = new Tuple<int, string, string>(ReSent.Item1 + 1, "OTP Validation (Validated)", "OTP validation request ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Success for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> ValidatedClientTest = new Tuple<int, string, string>(Validated.Item1 + 1, "OTP Validation (Client Test Validated)", "Client Test OTP validation request ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Success for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                public static Tuple<int, string, string> Invalidated = new Tuple<int, string, string>(ValidatedClientTest.Item1 + 1, "OTP Validation (Invalidated)", "The OTP ([" + TokenKeys.OTPCode.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) submitted for RequestId ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) hass been Disabled. Max retries([" + TokenKeys.OTPRetriesMax.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Current retries ([" + TokenKeys.OTPRetriesCurrent.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                public static Tuple<int, string, string> Inactive = new Tuple<int, string, string>(Invalidated.Item1 + 1, "OTP Validation (Inactive)", "The OTP ([" + TokenKeys.OTPCode.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) submitted for RequestId ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) is Inactive. Max retries([" + TokenKeys.OTPRetriesMax.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Current retries ([" + TokenKeys.OTPRetriesCurrent.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                public static Tuple<int, string, string> Expired = new Tuple<int, string, string>(Inactive.Item1 + 1, "OTP Validation (Expired)", "The OTP ([" + TokenKeys.OTPCode.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) submitted for RequestId ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Expired on ([" + TokenKeys.OTPExpiredTime.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Max retries([" + TokenKeys.OTPRetriesMax.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Current retries ([" + TokenKeys.OTPRetriesCurrent.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> TooManyRetries = new Tuple<int, string, string>(Expired.Item1 + 1, "OTP Validation (Too Many Retries)", "The OTP ([" + TokenKeys.OTPCode.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) submitted for RequestId ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) has been Disabled. Too many retries. Max retries([" + TokenKeys.OTPRetriesMax.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Current retries ([" + TokenKeys.OTPRetriesCurrent.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Failure details ([" + TokenKeys.FailureDetails.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                public static Tuple<int, string, string> BadOtpCode = new Tuple<int, string, string>(TooManyRetries.Item1 + 1, "OTP Validation (Bad Code)", "The OTP ([" + TokenKeys.OTPCode.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) submitted for RequestId ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) is Bad. Max retries([" + TokenKeys.OTPRetriesMax.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Current retries ([" + TokenKeys.OTPRetriesCurrent.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");

                public static Tuple<int, string, string> Created = new Tuple<int, string, string>(TooManyRetries.Item1 + 1, "OTP (Created)", "([" + TokenKeys.ClientGroupRestricted.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) - OTP Created ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Updated = new Tuple<int, string, string>(Created.Item1 + 1, "OTP (Updated)", "([" + TokenKeys.ClientGroupRestricted.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) - OTP Updated ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Deleted = new Tuple<int, string, string>(Updated.Item1 + 1, "OTP (Deleted)", "([" + TokenKeys.ClientGroupRestricted.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) - OTP Deleted ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Disabled = new Tuple<int, string, string>(Deleted.Item1 + 1, "OTP (Disabled)", "([" + TokenKeys.ClientGroupRestricted.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) - OTP Disabled ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Enabled = new Tuple<int, string, string>(Disabled.Item1 + 1, "OTP (Enabled)", "([" + TokenKeys.ClientGroupRestricted.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) - OTP Disabled ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Cancelled = new Tuple<int, string, string>(Enabled.Item1 + 1, "OTP (Cancelled)", "([" + TokenKeys.ClientGroupRestricted.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) - OTP Cancelled ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> AdminConsoleOtp = new Tuple<int, string, string>(Cancelled.Item1 + 1, "OTP (Admin Console Request)", "Admin Console Login Request - OTP sent to Admin-User ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) via ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) at: ([" + TokenKeys.SentToAddress.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) with RequestId ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Queued = new Tuple<int, string, string>(AdminConsoleOtp.Item1 + 1, "OTP (Queued)", "OTP Queued for User ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) via ([" + TokenKeys.DeliveryMethod.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) at: ([" + TokenKeys.SentToAddress.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) with RequestId ([" + TokenKeys.RequestId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> ProviderFailedTryNextProvider = new Tuple<int, string, string>(Queued.Item1 + 1, "OTP Service (Provider Failed)", "The OTP message provider ([" + TokenKeys.MessageProviderCurrent.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) could not be contacted via ([" + TokenKeys.DeliveryMethodCurrent.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Will retry sending OTP through ([" + TokenKeys.MessageProviderNext.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) via ([" + TokenKeys.DeliveryMethodNext.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
            }

            public static class Providers
            {
                public static Tuple<int, string, string> Base = new Tuple<int, string, string>(Otp.ProviderFailedTryNextProvider.Item1 + 1, "Provider (Base)", "Base Provider Event Class");
                public static Tuple<int, string, string> Created = new Tuple<int, string, string>(Base.Item1 + 1, "Provider (Created)", "[" + TokenKeys.ProviderType.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] Provider ([" + TokenKeys.ProviderName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Created for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Updated = new Tuple<int, string, string>(Created.Item1 + 1, "Provider (Updated)", "[" + TokenKeys.ProviderType.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] Provider ([" + TokenKeys.ProviderName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Updated [" + TokenKeys.ProviderChangedValues.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Deleted = new Tuple<int, string, string>(Updated.Item1 + 1, "Provider (Deleted)", "[" + TokenKeys.ProviderType.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] Provider ([" + TokenKeys.ProviderName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Deleted for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> AllReset = new Tuple<int, string, string>(Deleted.Item1 + 1, "Provider (Properties Reset)", "[" + TokenKeys.ProviderType.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] provider ([" + TokenKeys.ProviderName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) properties have been reset for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Properties changed ([" + TokenKeys.ProviderChangedValues.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) by [" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "].");
            }

            public static class Registration
            {
                public static class AdminUser
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogRegistrationStartRange, "Admin User (Base)", "Base Admin User Event Class");
                    public static Tuple<int, string, string> Created = new Tuple<int, string, string>(Base.Item1 + 1, "Admin User (Created)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Created by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Updated = new Tuple<int, string, string>(Created.Item1 + 1, "Admin User (Updated)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Updated by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Updated values ([" + TokenKeys.UpdatedValues.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Deleted = new Tuple<int, string, string>(Updated.Item1 + 1, "Admin User (Deleted)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Deleted by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Disabled = new Tuple<int, string, string>(Deleted.Item1 + 1, "Admin User (Disabled)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Disabled by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Enabled = new Tuple<int, string, string>(Disabled.Item1 + 1, "Admin User (Enabled)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Enabled by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Activated = new Tuple<int, string, string>(Enabled.Item1 + 1, "Admin User (Activated)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Activated by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Deactivated = new Tuple<int, string, string>(Activated.Item1 + 1, "Admin User (Deactivated)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Deactivated by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> DuplicateUserName = new Tuple<int, string, string>(Deactivated.Item1 + 1, "Admin User (Duplicate Username)", "A user with that User Name ([" + TokenKeys.DuplicateUserName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) already exists.");
                    public static Tuple<int, string, string> DuplicateEmail = new Tuple<int, string, string>(DuplicateUserName.Item1 + 1, "Admin User (Duplicate Email)", "A user with that Email ([" + TokenKeys.DuplicateEmail.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) already exists.");
                    public static Tuple<int, string, string> InvalidEmail = new Tuple<int, string, string>(DuplicateEmail.Item1 + 1, "Admin User (Invalid Email)", "Invalid Email address ([" + TokenKeys.Email.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> InvalidSecurityAnswer = new Tuple<int, string, string>(InvalidEmail.Item1 + 1, "Admin User (Invalid Security Answer)", "Invalid Security Answer ([" + TokenKeys.SecurityAnswer.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> InvalidPassword = new Tuple<int, string, string>(InvalidSecurityAnswer.Item1 + 1, "Admin User (Invalid Password)", "Invalid Password ([" + TokenKeys.Password.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). It must be seven characters long and have at least one non-alphanumeric character..");
                    public static Tuple<int, string, string> UnknownRegistrationError = new Tuple<int, string, string>(InvalidPassword.Item1 + 1, "Admin User (Unknown Registration Error)", "Unknown registration error for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }

                public static class ClientUser
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(AdminUser.UnknownRegistrationError.Item1+1, "Client User (Base)", "Base Admin User Event Class");
                    public static Tuple<int, string, string> Created = new Tuple<int, string, string>(Base.Item1 + 1, "Client User (Created)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Created by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Added = new Tuple<int, string, string>(Created.Item1 + 1, "Client User (Added)", "([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) account Added to Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Updated = new Tuple<int, string, string>(Added.Item1 + 1, "Client User (Updated)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Updated by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Updated values ([" + TokenKeys.UpdatedValues.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Deleted = new Tuple<int, string, string>(Updated.Item1 + 1, "Client User (Deleted)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Deleted by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Disabled = new Tuple<int, string, string>(Deleted.Item1 + 1, "Client User (Disabled)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Disabled by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Enabled = new Tuple<int, string, string>(Disabled.Item1 + 1, "Client User (Enabled)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Enabled by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Activated = new Tuple<int, string, string>(Enabled.Item1 + 1, "Client User (Activated)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Activated by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Deactivated = new Tuple<int, string, string>(Activated.Item1 + 1, "Client User (Deactivated)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account for ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Deactivated by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> DuplicateUserName = new Tuple<int, string, string>(Deactivated.Item1 + 1, "Client User (Duplicate Username)", "A user with that User Name [" + TokenKeys.UserName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) already exists for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> DuplicateEmail = new Tuple<int, string, string>(DuplicateUserName.Item1 + 1, "Client User (Duplicate Email)", "A user with that Email [" + TokenKeys.Email.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) already exists for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> InvalidEmail = new Tuple<int, string, string>(DuplicateEmail.Item1 + 1, "Client User (Invalid Email)", "Invalid Email address [" + TokenKeys.Email.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> InvalidSecurityAnswer = new Tuple<int, string, string>(InvalidEmail.Item1 + 1, "Client User (Invalid Security Answer)", "Invalid Security Answer [" + TokenKeys.SecurityAnswer.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> InvalidPassword = new Tuple<int, string, string>(InvalidSecurityAnswer.Item1 + 1, "Client User (Invalid Password)", "Invalid Password [" + TokenKeys.Password.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). It must be seven characters long and have at least one non-alphanumeric character for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> UnknownRegistrationError = new Tuple<int, string, string>(InvalidPassword.Item1 + 1, "Client User (Unknown Registration Error)", "Unknown registration error for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }

                public static class EndUser
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(ClientUser.UnknownRegistrationError.Item1 + 1, "End User (Base)", "Base Admin User Event Class");
                    public static Tuple<int, string, string> Created = new Tuple<int, string, string>(Base.Item1 + 1, "End User (Created)", "End User ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Registered for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Updated = new Tuple<int, string, string>(Created.Item1 + 1, "End User (Updated)", "End User ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Updated by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Deleted = new Tuple<int, string, string>(Updated.Item1 + 1, "End User (Deleted)", "End User ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Deleted by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Disabled = new Tuple<int, string, string>(Deleted.Item1 + 1, "End User (Disabled)", "End User ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Disabled by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Enabled = new Tuple<int, string, string>(Disabled.Item1 + 1, "End User (Enabled)", "End User ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Enabled by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Activated = new Tuple<int, string, string>(Enabled.Item1 + 1, "End User (Activated)", "End User ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Activated by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> Deactivated = new Tuple<int, string, string>(Activated.Item1 + 1, "End User (Deactivated)", "End User ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Deactivated by ([" + TokenKeys.UpdatedByLoggedinAdminFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> DuplicateUserName = new Tuple<int, string, string>(Deactivated.Item1 + 1, "End User (Duplicate Username)", "A user with that User Name ([" + TokenKeys.UserName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) already exists for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> DuplicateEmail = new Tuple<int, string, string>(DuplicateUserName.Item1 + 1, "End User (Duplicate Email)", "A user with that Email ([" + TokenKeys.Email.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) already exists for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> InvalidEmail = new Tuple<int, string, string>(DuplicateEmail.Item1 + 1, "End User (Invalid Email)", "Invalid Email address ([" + TokenKeys.Email.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> InvalidSecurityAnswer = new Tuple<int, string, string>(InvalidEmail.Item1 + 1, "End User (Invalid Security Answer)", "Invalid Security Answer ([" + TokenKeys.SecurityAnswer.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> InvalidPassword = new Tuple<int, string, string>(InvalidSecurityAnswer.Item1 + 1, "End User (Invalid Password)", "Invalid Password ([" + TokenKeys.Password.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). It must be seven characters long and have at least one non-alphanumeric character for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> UnknownRegistrationError = new Tuple<int, string, string>(InvalidPassword.Item1 + 1, "End User (Unknown Registration Error)", "Unknown registration error for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }
            }

            public static class Security
            {
                public static class Login
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogSecurityStartRange, "Login (Base)", "Base Security Login Event Class");
                    public static Tuple<int, string, string> Succeeded = new Tuple<int, string, string>(Base.Item1 + 1, "Login (Succeeded)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) successfully Logged In to the Admin console.");
                    public static Tuple<int, string, string> Failed = new Tuple<int, string, string>(Succeeded.Item1 + 1, "Login (Failed)", "Invalid Username ([" + TokenKeys.UserName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) or Password ([" + TokenKeys.Password.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Attempted to log In to the Admin console.");
                }

                public static class Logout
                {
                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(Login.Failed.Item1 + 1, "Logout (Base)", "Base Security Logout Event Class");
                    public static Tuple<int, string, string> Succeeded = new Tuple<int, string, string>(Base.Item1 + 1, "Logout (Succeeded)", "[" + TokenKeys.UserRole.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] account ([" + TokenKeys.UserFullName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) successfully Logged Out from the Admin console.");
                    public static Tuple<int, string, string> SessionExpired = new Tuple<int, string, string>(Succeeded.Item1 + 1, "Logout (Session Expired)", "Session expired. User logged out for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                    public static Tuple<int, string, string> UnknownError = new Tuple<int, string, string>(SessionExpired.Item1 + 1, "Logout (Unknown Error)", "Unspecified error. User logged out for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                }
            }

            public static class Services
            {
                public static class ExternalApi
                {
                    //ServiceProviderName
                    //ServiceProviderUrl
                    //ServiceProviderRequestData
                    //ServiceProviderResponseData

                    public static Tuple<int, string, string> Base = new Tuple<int, string, string>(Security.Logout.UnknownError.Item1, "External Api (Base)", "Base Services External Api Event Class");
                    public static Tuple<int, string, string> Request = new Tuple<int, string, string>(Base.Item1 + 1, "External Api (Request)", "Service request to [" + TokenKeys.ServiceProviderName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] at ([" + TokenKeys.ServiceProviderUrl.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) sent. Request data ([" + TokenKeys.ServiceProviderRequestData.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "])");
                    public static Tuple<int, string, string> Response = new Tuple<int, string, string>(Request.Item1 + 1, "External Api (Response)", "Service response from [" + TokenKeys.ServiceProviderName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] at ([" + TokenKeys.ServiceProviderUrl.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) received. Response data ([" + TokenKeys.ServiceProviderResponseData.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "])");
                }
            }

            public static class System
            {
                public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogSystemStartRange, "System (Base)", "Base System Event Class");
                public static Tuple<int, string, string> SystemReset = new Tuple<int, string, string>(Base.Item1 + 1, "System (System Reset)", "Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) committed a full system data reset. The following Database Collections were dropped ([" + TokenKeys.DatabaseDropped.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> ClientApiAppDeleted = new Tuple<int, string, string>(SystemReset.Item1 + 1, "System Reset (Client API App Deleted)", "Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) deleted the ([" + TokenKeys.ClientApiAppDeleted.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) Client API App for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> DatabaseCollectionDropped = new Tuple<int, string, string>(ClientApiAppDeleted.Item1 + 1, "System Reset (Collection Dropped)", "Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) dropped the Database Collection ([" + TokenKeys.DatabaseCollectionName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) during a System Reset event.");
                public static Tuple<int, string, string> DatabaseBackupStarted = new Tuple<int, string, string>(DatabaseCollectionDropped.Item1 + 1, "System Backup (Database Started)", "Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) database Backup completed.");
                public static Tuple<int, string, string> DatabaseCollectionBackedup = new Tuple<int, string, string>(DatabaseBackupStarted.Item1 + 1, "System Backup (Collection Objects)", "Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) backedup ([" + TokenKeys.DatabaseCollectionObjectsCount.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] objects) in the ([" + TokenKeys.DatabaseCollectionName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) database collection.");
                public static Tuple<int, string, string> DatabaseBackupCompleted = new Tuple<int, string, string>(DatabaseCollectionBackedup.Item1 + 1, "System Backup (Database Completed)", "Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) database Backup completed.");
                public static Tuple<int, string, string> DatabaseBackupCopyCompleted = new Tuple<int, string, string>(DatabaseBackupCompleted.Item1 + 1, "System Backup (Database Copied)", "Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) backed up database from ([" + TokenKeys.DatabaseSource.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) to ([" + TokenKeys.DatabaseTarget.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> DatabaseBackupDropped = new Tuple<int, string, string>(DatabaseBackupCopyCompleted.Item1 + 1, "System Maintainence (Backups Pruned)", "Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) dropped the backup database ([" + TokenKeys.DatabaseTarget.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) as a result of a backup request (Pruning Operation).");
                public static Tuple<int, string, string> DatabaseRestored = new Tuple<int, string, string>(DatabaseBackupDropped.Item1 + 1, "System Maintainence (Database Restored)", "Admin ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) restored database from ([" + TokenKeys.DatabaseSource.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) to ([" + TokenKeys.DatabaseTarget.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> AwsHealthCheck = new Tuple<int, string, string>(DatabaseRestored.Item1 + 1, "AWS (Health Check)", "AWS (Health Check) server: ([" + TokenKeys.ServerIpAddress.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "])");
                public static Tuple<int, string, string> AllowedIPsUpdated = new Tuple<int, string, string>(AwsHealthCheck.Item1 + 1, "Security (IPs Reset)", "Allowed IP addresses reset for [" + TokenKeys.OwnerType.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "] ([" + TokenKeys.OwnerName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). New allowed IP addresses are ([" + TokenKeys.AllowedIPAddresses.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Updated by [" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "].");
                public static Tuple<int, string, string> EventStatsReset = new Tuple<int, string, string>(AllowedIPsUpdated.Item1 + 1, "System Stats (Event Stats Reset)", "Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) event stats have been reset for the date ([" + TokenKeys.EventStatDate.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) by [" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]. Stat details ([" + TokenKeys.EventStatDetails.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "])");
                public static Tuple<int, string, string> OperationalTest = new Tuple<int, string, string>(EventStatsReset.Item1 + 1, "System Operations (Connectivity Successful)", "The Operational test user ([" + TokenKeys.EventGeneratedByName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) sucessfully completed a System Connectivity test.");
            }

            public static class UserVerification
            {
                public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogUserVerificationStartRange, "User Verification (Base)", "Base User Verification Event Class");
                public static Tuple<int, string, string> Succeeded = new Tuple<int, string, string>(Base.Item1 + 1, "User Verification (Succeeded)", "Successfully Verified user ([" + TokenKeys.EndUserId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) via ([" + TokenKeys.ProviderName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Verification Result: ([" + TokenKeys.VerificationResponse.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
                public static Tuple<int, string, string> Failed = new Tuple<int, string, string>(Succeeded.Item1 + 1, "User Verification (Failed)", "Failed user verification ([" + TokenKeys.EndUserId.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) via ([" + TokenKeys.ProviderName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]). Verification Result: ([" + TokenKeys.VerificationResponse.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]) for Client ([" + TokenKeys.ClientName.Replace(TokenKeys.ItemSep, "").Replace(TokenKeys.KVSep, "") + "]).");
            }
            #endregion
        }
        #endregion
        
        #region EventStats
        public static class EventStats
        {
            //OTP Stats
            public const string OtpSent = "OtpSent";
            public const string OtpInvalid = "OtpInvalid";
            public const string OtpExpired = "OtpExpired";
            public const string OtpValid = "OtpValid";

            //End User Registration Stats
            public const string EndUserRegister = "EndUserRegister";

            // Ads send
            public const string AdMessageSent = "AdMessageSent";
            public const string AdEnterOtpScreenSent = "AdEnterOtpScreenSent";
            public const string AdVerificationScreenSent = "AdVerificationScreenSent";

            public const string Exceptions = "Exceptions";
        }
        #endregion

        #region Strings
        public static class Strings
        {
            // Mongo Constants
            public const string MongoDB = "MongoDB";   //mongoDBConnectionPool App Name
            public const string MongoDBDocServer = "MongoDBDocServer";   //mongoDBConnectionPool Documentation db
            public const string ConnectionMode = "ConnectionMode";
            public const string ReadPreference = "ReadPreference";
            public const string WriteConcern = "WriteConcern";
            public const string ConnectionTimeoutSeconds = "ConnectionTimeoutSeconds";
            public const string MinDBConnections = "MinDBConnections";
            public const string MaxDBConnections = "MaxDBConnections";
            public const string BackgroundIndex = "BackgroundIndex";
            public const string SparseIndex = "SparseIndex";
            public const string Automatic = "Automatic";
            public const string Direct = "Direct";
            public const string ReplicaSet = "ReplicaSet";
            public const string ShardRouter = "ShardRouter";
            public const string Nearest = "Nearest";
            public const string Primary = "Primary";
            public const string PrimaryPreferred = "PrimaryPreferred";
            public const string Secondary = "Secondary";
            public const string SecondaryPreferred = "SecondaryPreferred";
            public const string Acknowledged = "Acknowledged";
            public const string Unacknowledged = "Unacknowledged";
            public const string ReplicaSetFlag = "localhost:27019";
            public const string ReplicaSetName = "ReplicaSetName";
            public const string MongoDbSvr1 = "mongolan1.mobileauthcorp.net";
            public const string MongoDbSvr2 = "mongolan2.mobileauthcorp.net";
            public const string MongoDbSvr3 = "mongolan3.mobileauthcorp.net";
            // Mongo Constants

            public const string Email = "Email";
            public const string Html = "Html";
            public const string Sms = "Sms";
            public const string Voice = "Voice";
            public const string NotSpecified = "Not Specified";
            public const string ServiceLog = "Service Log";

            public const string DefaultAccountingEmail = "accounting@mobileauthcorp.com";
            public const string DefaultFromEmail = "info@services.mobileauthcorp.com";
            public const string DefaultEmptyObjectId = "000000000000000000000000";
            public const string DefaultStaticObjectId = "111111111111111111111111";

            public const string DefaultGroupId = "536a77011c863311244dab70";
            public const string DefaultGroupName = "!MAC Default Group";

            public const string DefaultClientId = "530f6e8e675c9b1854a6970b";
            public const string DefaultClientName = "!MAC Default Client";
            public const string DefaultClientAppUrl = "/!Defaults/ClientApps/";

            public const string DefaultAdminId = "5387d81e1c863364a8292fc5";
            public const string DefaultAdminName = "!System Administrator";
            public const string DefaultAdminUserName = "system@mobileauthcorp.com";
            public const string DefaultAdminEmail = "system@mobileauthcorp.com";
            public const string DefaultAdminMobilePhone = "480-268-4076";
            public const string DefaultStateId = "52ade08f2b0b6d13f88c4e62"; //AZ

            public const string DefaultSystemAdminId = "5285be322b0b6d1ac4a542ca";
            public const string DefaultGroupAdminId = "5285be642b0b6d1ac4a542ce";
            public const string DefaultClientAdminId = "5285be8d2b0b6d1ac4a542d2";
            public const string DefaultClientUserId = "5285be8d2b0b6d1ac4a542d6";
            public const string DefaultEndUserId = DefaultEmptyObjectId;
            public const string DefaultEmailProvider = "52a9ff62675c9b04c077107d";
            public const string DefaultSmsProvider = "52a9ff62675c9b04c077107f";
            public const string DefaultVoiceProvider = "52a9ff62675c9b04c077107e";
            public const string DefaultAdvertisingProvider = "53e16c34ead636177850bcba";
            public const int MaxNumberOfBackupsToKeep = 10;

            public static string[] ArrCollectionsToDelete = { "Account", "Client", "Elmah", "EndUser", "Event", "Group", "OasClientList", "Otp", "Users", "UsersInRoles", "UserProfile" };
            
            public const string DisplayDesktop = "Desktop";
            public const string DisplayMobile = "Mobile";

            // Ad returned from Secure Ads Server
            public const string AdType = "AdType";
            public const string AdMessage = "AdMessage";
            public const string AdEnterOtp = "AdEnterOtp";
            public const string AdVerification = "AdVerification";

            public const string QAUserFirstName = "QAUser";
            public const string QAUserLastName = "QAUser";
            public const string AdminQAUserOTP = "QA1234";
            public const string AdminQAUserDeliveryMethod = "ModeQA";

            public static class Otp
            {
                public static Tuple<int, string, string> Base = new Tuple<int, string, string>(EventLogGroupStartRange, "", "Otp Strings Class");
                public static Tuple<int, string, string> Request = new Tuple<int, string, string>(Base.Item1 + 1, "", "");
                public static Tuple<int, string, string> Sent = new Tuple<int, string, string>(Request.Item1 + 1, "", "");
                public static Tuple<int, string, string> Valiudated = new Tuple<int, string, string>(Sent.Item1 + 1, "", "");

                public const string RequestTypeClientManaged = "Client Managed";
                public const string RequestTypeClientGroupRestricted = "Client Restricted";
                public const string RequestTypeGroupRestricted = "Group Restricted";
                public const string RequestTypeOpen = "Open";
                // Required carrier information sent on first first text message sent to user
                //public static string CarrierFirstTimeInfo =
                //    "|Msg freq depends on user. HELP for help or STOP to opt-out. Msg and Data rates may apply.";
            }

            public static class UserRoles
            {
                public static Tuple<int, string, string> SystemAdmin = new Tuple<int, string, string>(AppStringsUserRolesStartRange, "System Administrator", DefaultSystemAdminId);
                public static Tuple<int, string, string> GroupAdmin = new Tuple<int, string, string>(SystemAdmin.Item1 + 1, "Group Administrator", DefaultGroupAdminId);
                public static Tuple<int, string, string> ClientAdmin = new Tuple<int, string, string>(GroupAdmin.Item1 + 1, "Client Administrator", DefaultClientAdminId);
                public static Tuple<int, string, string> AccountingUser = new Tuple<int, string, string>(ClientAdmin.Item1 + 1, "Accounting User", DefaultClientUserId);
                public static Tuple<int, string, string> ClientUser = new Tuple<int, string, string>(AccountingUser.Item1 + 1, "Client User", DefaultClientUserId);
                public static Tuple<int, string, string> GroupUser = new Tuple<int, string, string>(ClientUser.Item1 + 1, "Group User", DefaultClientUserId);
                public static Tuple<int, string, string> OperationsUser = new Tuple<int, string, string>(GroupUser.Item1 + 1, "Operations User", DefaultClientUserId);
                public static Tuple<int, string, string> ViewOnlyUser = new Tuple<int, string, string>(OperationsUser.Item1 + 1, "View Only User", DefaultClientUserId);
                public static Tuple<int, string, string> EndUser = new Tuple<int, string, string>(ViewOnlyUser.Item1 + 1, "End User", DefaultEndUserId);
            }
        }
        #endregion

        #region DocumentTemplateReplacementTokens
        public static class DocumentTemplateReplacementTokens
        {
            public const string NL = "|";
            public const string AD = "[AD]";
            public const string ClientName = "[ClientName]";
            public const string FirstName = "[FirstName]";
            public const string OTP = "[OTP]";
            public const string DETAILS = "[DETAILS]";

            public const int Generic = 0;
            public const int Authentication = 1;
            public const int TransactionVerification = 2;
            public const int RegistrationOTP = 3;
            public const int Notification = 4;
            public const int AdminLogin = 5;
            public const int TransactionVerificationReply = 6;
        }
        #endregion

        #region Application
        public static class Application
        {
            public static class Startup
            {
                public const string UserClientList = "UserClientList";
                public const int UserClientCount = 2500;
            }
        }
        #endregion

        #region RegexStrings
        public static class RegexStrings
        {
            public const string EmailAddress = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            public const string PhoneNumber = @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$";
            //public const string PhoneNumber = @"^\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";
        }
        #endregion

        #region Dictionary
        public static class Dictionary
        {
            // constants used in formatting delimited strings
            public static class Keys
            {
                public const string ItemSep = Common.ItemSep;       // Pipe(|) used as item seperator
                public const string KVSep = Common.KVSep;           // Colon(:) used as Key/value seperator
                public const string Key = "Key";
                public const string Debug = "Debug";                //Used to set debug conditions in services
                public const string ServiceName = "ServiceName";
                public const string Request = "Request";
                public const string Name = "Name";
                public const string UserId = "UserId";
                public const string FullyQualifiedDomainName = "FQDN";
                public const string NotificationOption = "NotificationOption";
                public const string CID = "CID";                    //Client Id
                public const string ClientName = "ClientName";
                public const string GroupId = "GroupId";
                public const string GroupName = "GroupName";
                public const string RequestId = Common.RequestId;   //OTP Request Id
                public const string OTP = Common.OTP;               //OTP One-Time Password
                public const string LoopBackTest = WebConfig.AppSettingsKeys.LoopBackTest; // OTP Message delivery loop back test
                public const string Message = "Message";
                public const string Comment = "Comment";
                public const string ReplyCompletion = "ReplyCompletion";
                public const string ReplyError = "ReplyError";
                public const string ReplyStatus = "ReplyStatus";
                public const string ReplyUri = "ReplyUri";
                public const string EnableReply = "EnableReply";
                public const string TrxDetails = "TrxDetails";
                public const string TrxType = "TrxType";            //Used to select the document template
                public const string LoggedInAdminId = "LoggedInAdminId";
                public const string LoggedInAdminIpAddress = "LoggedInAdminIpAddress";
                public const string Description = "Description";
                public const string Abbreviation = "Abbreviation";

                public const string Protocol = "Protocol";
                public const string EmailLandingPage = "EmailLandingPage";
                public const string API = "API";

                public const string RegistrationType = "RegType";

                public const string TypeDef = "TypeDef";
                public const string TypeDefName = "TypeDefName";
                public const string VerificationProviderName = "VerificationProviderName";

                public const string FileType = "FileType";
                public const string FileName = "FileName";
                public const string UploadFolder = "UploadFolder";
                public const string Register = "Register";
                public const string Login = "Login";
                public const string User = "User";

// ReSharper disable once MemberHidesStaticFromOuterClass
                public const string DeliveryMethod = "DeliveryMethod";
                public const string EnterOTPAd = "EnterOTPAd";
                public const string VerificationAd = "VerificationAd";
                public const string TLM = "TLM"; // time to live minutes
                public const string OTPRetriesMax = "OTPRetriesMax";
                public const string OTPExpiredTime = "OTPExpiredTime";
                public const string BillDate = "BillDate";
                public const string DummyBilling = "DummyBilling";      // Debug only
                public const string SendCarrierInfo = "SendCarrierInfo"; // For client managed users, false=send carrier info on first text message, true=don't send
                public const string AdPassOption = "AdPassOption";
                public const string SetFirstTimeCarrierInfoSent = "SetFirstTimeCarrierInfoSent";
                public const string AllowedIpList = "AllowedIpList";

                public static class Ads
                {
                    public const string AdNumber = "AdNumber";
                    public const string Age = "AdAge";
                    public const string City = "AdCity";
                    public const string Ethnicity = "AdEthnicity";
                    public const string Gender = "AdGender";
                    public const string Homeowner = "AdHomeowner";
                    public const string HouseholdIncome = "AdHouseholdIncome";
                    public const string MaritalStatus = "AdMaritalStatus";
                    public const string SpecificKeywords = "AdSpecificKeywords";
                    public const string State = "AdState";
                    public const string Type = "AdType";
                    public const string AdServerDomain = "AdServerDomain";
                }
            }

            public static class Values
            {
                public const string True = Common.True;
                public const string False = Common.False;
                public const string Ping = "Ping";
                public const string ResendOtp = "ResendOtp";
                public const string CancelOtp = "CancelOtp";
                public const string SendOtp = "SendOtp";
                public const string SendOtpAdmin = "SendOtpAdmin";
                public const string SendMessage = "SendMessage";
                public const string VerifyOtp = "VerifyOtp";
                public const string EndUserRegister = "EndUserRegister";
                public const string FileRegister = "FileRegister";
                public const string Admin = "Admin";
                // oas requests
                public const string GetEndUserInfo = "GetEndUserInfo";
                public const string ActivateEndUser = "ActivateEndUser";
                public const string DeactivateEndUser = "DeactivateEndUser";
                public const string ADSTOP = "ADSTOP";
                public const string ADENABLE = "ADENABLE";

                //public const string ResetSMSStop = "ResetSMSStop";
                public const string DeleteEndUser = "DeleteEndUser";
                public const string UpdateEndUser = "UpdateEndUser";
                public const string CheckEndUserRegistration = "CkEndUserReg";
                public const string CancelRegistration = "CancelReg";
                public const string RequestRegistrationOtp = "ReqRegOtp";
                public const string OpenRegister = "OpenRegister";
                public const string ClientRegister = "ClientRegister";
                public const string GroupRegister = "GroupRegister";
                public const string MACDemoEndUserManagement = "MACDemoEndUserMgmt";
                public const string GetUsageBillingForMonth = "GetUsageBillingForMonth";

                public const string Enable = "Enable";
                public const string Disable = "Disable";
                public const string Register = "Register";
                public const string Delete = "Delete";
                public const string Info = "Info";
                public const string Exists = "Exists";
                public const string Update = "Update";
                public const string Create = "Create";
                public const string GetListOfTypes = "GetListOfTypes";
                public const string GetListByName = "GetListByName";
                public const string UpdateTypeByName = "UpdateTypeByName";
                public const string NewTypeByName = "NewTypeByName";
                public const string Test = "Test";
                // Notification
                public const string Email = "Email";
                public const string Text = "Text";
                public const string Any = "Any";

                public const string GetClient = "GetClient";
                public const string UpdateClient = "UpdateClient";
                public const string GetAvailableMessageProviders = "GetAvailableMessageProviders";
                public const string GetClientId = "GetClientId";
                public const string GetClientName = "GetClientName";
                public const string GetAvaliableIPList = "GetAvaliableIPList";
                public const string SetAvaliableIPList = "SetAvaliableIPList";

                public const string Open = "Open";
                public const string Client = "Client";
                public const string Group = "Group";

                public const string JSAPI = "JSAPI";
                public const string MSAPI = "MSAPI";

                public const string SetAdPassOption = "SetAdPassOption";
                public const string AdEnable = "AdEnable";
                public const string AdDisable = "AdDisable";

                //public const string ResponseXMLDS = "XMLDS";        //Default, XML containing delimited strings
                //public const string ResponseXML = "XML";
                //public const string ResponseJSON = "JSON";

                // OTP Message delivery loop back test conditions
                public const string NoSend = WebConfig.AppSettingsKeys.NoSend; // return from message delivery as OTP message was sent successfully
                public const string StrtThread = WebConfig.AppSettingsKeys.StartThread; // Start thread to call ValicateOTP service
            }

            public static class Userinfo
            {
                public const string Prefix = "Prefix";
                public const string FirstName = "FirstName";
                public const string LastName = "LastName";
                public const string MiddleName = "MiddleName";
                public const string Suffix = "Suffix";
                public const string PhoneNumber = "PhoneNumber";
                public const string EmailAddress = "EmailAddress";
                public const string UID = "UID";
                public const string CompanyName = "CoName";

                public const string DOB = "DOB";
                public const string SSN4 = "SSN4";
                public const string Street = "Street";
                public const string Street2 = "Street2";
                public const string Unit = "Unit";
                public const string City = "City";
                public const string State = "State";
                public const string ZipCode = "ZipCode";
                public const string DriverLic = "DriverLic";
                public const string DriverLicSt = "DriverLicSt";

                public const string Country = "Country";

                public const string EndUserIpAddress = "EndUserIpAddress";
            }
        }
        #endregion

        #region EndUserStates
        public static class EndUserStates
        {
            public const string Registered = "Registered";
            public const string WaitingOtp = "WaitingOtp";
            public const string WaitingCompletion = "WaitingCompletion";
            public const string WaitingEmailSent = "WaitingEmailSent";
            public const string FailedReg = "Failed Registeration";

        }
        #endregion

        #region DeliveryMethod
        public static class DeliveryMethod
        {
            public static Tuple<string, ObjectId> Email = new Tuple<string, ObjectId>("Email", ObjectId.Parse("52a9ff62675c9b04c077107f"));
            public static Tuple<string, ObjectId> Sms = new Tuple<string, ObjectId>("Sms", ObjectId.Parse("52ade07f2b0b6d13f88c4d99"));
            public static Tuple<string, ObjectId> Voice = new Tuple<string, ObjectId>("Voice", ObjectId.Parse("52ade0802b0b6d13f88c4da0"));
        }
        #endregion

        #region Web Config
        public static class WebConfig
        {
            public static class ConnectionStringKeys
            {
                public const string MongoServer = "MongoServer";
                public const string DocumentationServer = "DocumentationServer";
                public const string OperationalTestServer = "OperationalTestServer";
            }

            public static class HostInfo
            { // used in SetWebConfig() method in utils
                public class RequestVariables
                {
                    public const string ServerName = "SERVER_NAME";
                }

                public class Host
                {
                    public const string Localhost = "localhost";

                    public const string LocalhostGeneric = "LocalHost - Generic";
                    public const string LocalhostChris = "LocalHost - Chris";
                    public const string LocalhostJoe = "LocalHost - Joe";
                    public const string LocalhostTerry = "LocalHost - Terry";
                    public const string Corp = "corp.mobileauthcorp.com";

                    // This is AWS environment
                    public const string Demo = "demo.mobileauthcorp.net";
                    public const string ProductionStaging = "production-staging.mobileauthcorp.net";
                    public const string Production = "www.mobileauthcorp.net";
                    public const string QA = "qa.mobileauthcorp.net";
                    public const string Test = "test.mobileauthcorp.net";
                    public const string TestIntegration = "test-integration.mobileauthcorp.net";
                }

                public class DbLocation
                {
                    //public const string Localhost = "mongodb://macservices:!macservices@corp.mobileauthcorp.com:27017";
                    //public const string Corp = "mongodb://macservices:!macservices@corp.mobileauthcorp.com:27017";

                    // This is AWS environment
                    //public const string Demo = "mongodb://macservices:!macservices@172.31.37.171:27017";
                    //public const string ProductionStaging = "mongodb://macservices:!macservices@172.31.37.171:27017"; // AWS server
                    //public const string Production = "mongodb://macservices:!macservices@172.31.37.171:27017"; // AWS server
                    //public const string QA = "mongodb://macservices:!macservices@172.31.37.171:27017";
                    //public const string Test = "mongodb://macservices:!macservices@172.31.37.171:27017"; // AWS server

                    public const string Localhost = "mongodb://admin:admin@ds147069.mlab.com:47069";
                    public const string Corp = "mongodb://admin:admin@ds147069.mlab.com:47069";

                    public const string Demo = "mongodb://admin:admin@ds147069.mlab.com:47069";
                    public const string ProductionStaging = "mongodb://admin:admin@ds147069.mlab.com:47069"; // AWS server
                    public const string Production = "mongodb://admin:admin@ds147069.mlab.com:47069"; // AWS server
                    public const string QA = "mongodb://admin:admin@ds147069.mlab.com:47069";
                    public const string Test = "mongodb://admin:admin@ds147069.mlab.com:47069"; // AWS server
                }

                public class DbName
                {
                    //public const string Localhost = "MAC_R1_Test";
                    //public const string Corp = "MAC_R1_Corp";
                    //public const string Demo = "MAC_R1_Demo";
                    //public const string ProductionStaging = "MAC_R1_Production_Staging";
                    //public const string Production = "MAC_R1_Production";
                    //public const string QA = "MAC_R1_QA";
                    //public const string Test = "MAC_R1_Test";
                    //public const string OperationalTestDbName = "MAC_R1_OperationalTest";

                    public const string Localhost = "mac_r1";
                    public const string Corp = "mac_r1";
                    public const string Demo = "mac_r1";
                    public const string ProductionStaging = "mac_r1";
                    public const string Production = "mac_r1";
                    public const string QA = "mac_r1";
                    public const string Test = "mac_r1";
                    public const string OperationalTestDbName = "mac_r1";
                }

                public class AdLocation
                {
                    public const string Localhost = "http://localhost:8080/Demos";

                    //public const string Demo = "http://www.otp-ap.com";
                    // This is AWS environment
                    public const string Demo = "http://www.otp-ap.us";

                    public const string Polaris = "http://www.otp-ap.us";
                    public const string ProductionStaging = "http://www.otp-ap.us";
                    public const string Production = "http://www.otp-ap.us";
                    public const string QA = "http://www.otp-ap.us";
                    public const string Test = "http://www.otp-ap.us";
                }
            }

            public static class AppSettingsKeys
            {
                public const string MessageBroadcastAPIService = "MessageBroadcastAPIService";
                public const string ConfigName = "ConfigName"; // string
                public const string MongoDbName = "MongoDbName"; // string
                public const string MongoDbDocumentDBName = "MongoDbDocumentDBName"; // string
                public const string MongoDbOperationalTestDBName = "MongoDbOperationalTestDBName"; // string
                public const string Port = "Port"; // string
                public const string Host = "Host"; // string
                public const string EnableSsl = "EnableSsl"; // bool
                public const string UseDefaultCredentials = "UseDefaultCredentials"; // bool
                public const string LoginUserName = "LoginUserName"; // string
                public const string LoginPassword = "LoginPassword"; // string
                public const string FromAddress = "FromAddress"; // string

                public const string InitUserClientIdList = "InitUserClientIdList"; // bool
                public const string ReturnErrorDetails = "ReturnErrorDetails";  // bool
                public const string DebugLogRequests = "DebugLogRequests";      // bool
                public const string DebugLogResponses = "DebugLogResponses";    // bool
                public const string AdServerDomain = "AdServerDomain";          // string
                public const string NoAds = "NoAds";
                public const string SecureAds = "SecureAds";
                public const string LoopBackTest = "LoopBackTest";              // appsetting name
                  public const string Disabled = Common.Disabled;               // send message, No loopback
                  public const string NoSend = "NoSend";                        // don't send messags, no loopback
                  public const string LoopBackReplyAtGateway = "LoopBackReplyAtGateway"; // MessageBroadcase only, loopback as Reply message
                  public const string StartThread = "StartThread";              // Loopback by starting thread that calls VerifyOTP service
                  public const string LoopBackReplyAtAPI = "LoopBackReplyAtAPI";// Loopback as reply message in Message Broadcast Api service
                public const string Debug = "Debug";                            // bool
                public const string EmailServiceLog = "EmailServiceLog";        // bool
                public const string UseClientService = "UseClientService";      // string

                public const string ConnectionStrings = "connectionStrings";      // string
                public const string AppSettings = "appSettings";      // string
                
                // Base Url where MAC's Open Access Services are running
                public const string MacOpenServicesUrl = "MacOpenServicesUrl";
                // Base Url where MAC's OTP and Registration Services are running
                public const string MacServicesUrl = "MacServicesUrl";
                public const string MessageBroadcastReplyDomain = "MessageBroadcastReplyDomain";
                // Registration Upload Folder, Note: must exists on Server where MAC's registration service is running
                public const string RegistrationFileUploadSubFolder = "RegistrationFileUploadSubFolder";

            }
        }
        #endregion

        #region Services
        public static class ServiceUrls
        {
        // OTP Services -->
            public const string RequestOtpWebService = "/Otp/RequestOtp.asmx/WsRequestOtp";
            public const string VerifyOtpWebService = "/Otp/ValidateOtp.asmx/WsValidateOtp";
            public const string MessageBroadcastReplyService = "/Otp/MessageBroadcastReplyService.asmx/WsMessageBroadcastReplyService";
        // Registration services -->
            public const string SecureTraidingRegisterUserWebService = "/User/StsEndUserRegistration.asmx/WsStsEndUserRegistration";
            public const string RegisteredUserWebService = "/User/EndUserRegistration.asmx/WsEndUserRegistration";
            public const string EndUserManagementWebService = "/User/EndUserManagement.asmx/WsEndUserManagement";
            public const string EndUserFileRegistrationWebService = "/User/EndUserFileRegistration.asmx/WsEndUserFileRegistration";
            public const string EndUserCompleteRegistrationWebService = "/User/EndUserCompleteRegistration.asmx/WsEndUserCompleteRegistration";
        // Open Access Services -->
            public const string MacOpenEndUserServices = "/OAS/OpenEndUserServices.asmx/WsOpenEndUserServices";
            public const string MacOpenClientServices = "/OAS/OpenClientServices.asmx/WsOpenClientServices";
        // Admin Services -->
            public const string MacUsageBilling = "/AdminServices/UsageBilling.asmx/WsUsageBilling";
            public const string MacSystemStats = "/AdminServices/SystemStats.asmx/WsSystemStats";
            public const string MacManageTypeDefsService = "/AdminServices/ManageTypeDefsService.asmx/WsManageTypeDefsService";
            public const string MacGroupInfo = "/AdminServices/GroupInfo.asmx/WsGroupInfo";
            public const string MacClientServices = "/AdminServices/ClientServices.asmx/WsClientServices"; 
        // Test Services
            public const string MacTestAdService = "/Test/TestAdService.asmx/WsTestAdService";
        }

        public static class ReplyServiceRequest
        {
            public const string Request = Dictionary.Keys.Request;
            public const string cid = Dictionary.Keys.CID;
            public const string ReplyState = "ReplyState";
            public const string RequestId = Dictionary.Keys.RequestId;
            public const string ErrorMsg = "ErrorMsg";
            public const string SetResponseOTP = "SetResponseOTP";
            public const string GetReplyCompletion = "GetReplyCompletion";
            public const string inpId = "inpId";
            public const string text = "text";
            public const string inpContactString = "inpContactString";

        }

        public static class ServiceResponse
        {
            public const string calledMethod = "calledMethod";
            public const string Reply = "Reply";
            public const string Action = "Action";
            public const string Request = "Request";
            public const string Error = "Error";
            public const string Debug = Dictionary.Keys.Debug;
            public const string QA = "QA";
            public const string Response = "Resp";
            public const string Details = "Details";
            public const string Name = "Name";
            public const string BadLine = "BadLine";            //File Register
            public const string BadNode = "BadNode";            //File Register
            public const string Result = "Result";              //File Register
// ReSharper disable once MemberHidesStaticFromOuterClass
            public const string DeliveryMethod = Dictionary.Keys.DeliveryMethod;
            public const string OTP = Common.OTP;
            public const string RequestId = Common.RequestId;
            public const string EnterOTPAd = Dictionary.Keys.EnterOTPAd;
            public const string VerificationAd = Dictionary.Keys.VerificationAd;
            public const string UserId = Dictionary.Keys.UserId;
            public const string TLM = "TLM"; // time to live minutes
            public const string ClientName = Dictionary.Keys.ClientName;
            public const string OTPRetriesMax = Dictionary.Keys.OTPRetriesMax;
            public const string OTPExpiredTime = Dictionary.Keys.OTPExpiredTime;

            public const string Validated = "Validated";
            public const string Invalid = "Invalid";
            public const string Timeout = "Timeout";
            public const string Enabled = "Enabled";
            public const string Disabled = "Disabled";
            public const string Deleted = "Deleted";
            public const string Resent = "Resent";
            public const string Inactive = "Inactive";
            public const string Deactivated = "Deactivated";
            public const string Activated = "Activated";
            public const string Registered = "Registered";
            public const string NotRegistered = "Not registered";
            public const string Success = "Success";
            public const string Sent = "Sent";
            public const string Canceled = "Canceled";
            public const string STOP = "STOP";
            public const string ShortCode = "ShortCode";
            public const string FromNumber = "FromNumber";
            public const string ResponseOTP = "ResponseOTP";
        }

        public static class MessageBroadcast
        {
            public static class RequestKeys
            {
                public static string CarrierFirstTimeInfo =
    " Msg freq depends on user. HELP for help or STOP to opt-out. Msg and Data rates may apply.";
            }

            public static class ResponseKeys //XML
            {
                public const string RXRESULTCODE = "RXRESULTCODE";
                public const string INPBATCHID = "INPBATCHID";
                public const string CURRTZVOICE = "CURRTZVOICE";
                public const string COUNTQUEUEDUPVOICE = "COUNTQUEUEDUPVOICE";
                public const string COUNTQUEUEDUPEMAIL = "COUNTQUEUEDUPEMAIL";
                public const string COUNTQUEUEDUPSMS = "COUNTQUEUEDUPSMS";
                public const string CURRTZ = "CURRTZ";
                public const string DISTRIBUTIONID = "DISTRIBUTIONID";
                public const string BLOCKEDBYDNC = "BLOCKEDBYDNC";
                public const string SMSINIT = "SMSINIT";
                public const string SMSDEST = "SMSDEST";            //Short Code
                public const string TYPE = "TYPE";
                public const string MESSAGE = "MESSAGE";
                public const string ERRMESSAGE = "ERRMESSAGE";
                public const string END = "END";
            }
        }

        public static class WhitePagesPro
        {
            public const string UserVerificationWhitePagesService = "/User/UserVerificationWhitePagesPro.asmx/WsUserVerificationWhitePagesPro";      
        }
        public static class LexisNexis
        {
            // Verification Providers -->
            public const string UserVerificationLexisNexisService = "/User/UserVerificationLexisNexis.asmx/WsUserVerificationLexisNexis";
            public const string GLBPurpose = "GLBPurpose";
            public const string DLPurpose = "DLPurpose";
            public const string UseDOBFilter = "UseDOBFilter";
            public const string DOBRadius = "DOBRadius";
            public const string StartingRecord = "StartingRecord";
            public const string ReturReturnCoun = "ReturReturnCoun";
            public const string LogFile = "LogFile";
            public const string LogReq = "LogReq";
            public const string LogResp = "LogResp";
        }
        #endregion
    }
}
