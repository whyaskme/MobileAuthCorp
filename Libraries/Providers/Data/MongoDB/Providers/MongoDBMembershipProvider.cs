using System;
using System.Collections.Specialized;
using System.Web;
using System.Configuration.Provider;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using cs = MACServices.Constants.Strings;

namespace MongoDB.Web.Providers
{
    public class MongoDbMembershipProvider : MembershipProvider
    {
        private bool _enablePasswordReset;
        private bool _enablePasswordRetrieval;
        private int _maxInvalidPasswordAttempts;
        private int _minRequiredNonAlphanumericCharacters;
        private int _minRequiredPasswordLength;
        private readonly MongoCollection _mongoCollection;
        private int _passwordAttemptWindow;
        private MembershipPasswordFormat _passwordFormat;
        private string _passwordStrengthRegularExpression;
        private bool _requiresQuestionAndAnswer;
        private bool _requiresUniqueEmail;

        public string MembershipApplicationName = "MACOTPSystem";

        public MongoDbMembershipProvider()
        {
            ApplicationName = MembershipApplicationName;
            
            var mongoDBConnectionPool = (MongoDatabase)HttpContext.Current.Application[cs.MongoDB];
            _mongoCollection = mongoDBConnectionPool.GetCollection("Users");
        }

        public override sealed string ApplicationName { get; set; }

        public override bool EnablePasswordReset
        {
            get { return _enablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
        }

        private IMongoQuery QueryUsernameIs(string userName )
        {
            return Query.And(Query.EQ("ApplicationName", ApplicationName),Query.EQ("UsernameLower", userName .ToLowerInvariant()));
        }

        private IMongoQuery QueryUsernameMatches(string userName )
        {
            return Query.And(Query.EQ("ApplicationName", ApplicationName),Query.Matches("UsernameLower", userName .ToLowerInvariant()));
        }

        private IMongoQuery QueryEmail(string email)
        {
            //return Query.And(Query.EQ("ApplicationName", ApplicationName),Query.Matches("EmailLower", email.ToLowerInvariant()));
            return Query.And(Query.EQ("ApplicationName", ApplicationName), Query.EQ("EmailLower", email.ToLowerInvariant()));
        }

        public override bool ChangePassword(string userName , string oldPassword, string newPassword)
        {
            var query = QueryUsernameIs(userName );

            var bsonDocument = _mongoCollection.FindOneAs<BsonDocument>(query);

            if (!VerifyPassword(bsonDocument, oldPassword))
                return false;

            var validatePasswordEventArgs = new ValidatePasswordEventArgs(userName , newPassword, false);
            OnValidatingPassword(validatePasswordEventArgs);

            if (validatePasswordEventArgs.Cancel)
                throw new MembershipPasswordException(validatePasswordEventArgs.FailureInformation.Message);

            var update = Update.Set("LastPasswordChangedDate", DateTime.UtcNow).Set("Password", EncodePassword(newPassword, PasswordFormat, bsonDocument["Salt"].AsString));
            _mongoCollection.Update(query, update);

            return true;
        }

        public override bool ChangePasswordQuestionAndAnswer(string userName , string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            var query = QueryUsernameIs(userName );
            var bsonDocument = _mongoCollection.FindOneAs<BsonDocument>(query);

            if (!VerifyPassword(bsonDocument, password))
                return false;

            var update = Update.Set("PasswordQuestion", newPasswordQuestion).Set("PasswordAnswer", EncodePassword(newPasswordAnswer, PasswordFormat, bsonDocument["Salt"].AsString));
            return _mongoCollection.Update(query, update).Ok;
        }

        public override MembershipUser CreateUser(string userName , string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (providerUserKey != null)
            {
                if (!(providerUserKey is Guid))
                {
                    status = MembershipCreateStatus.InvalidProviderUserKey;
                    return null;
                }
            }
            else
            {
                providerUserKey = ObjectId.GenerateNewId();
            }

            var validatePasswordEventArgs = new ValidatePasswordEventArgs(userName , password, true);
            OnValidatingPassword(validatePasswordEventArgs);

            if (validatePasswordEventArgs.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresQuestionAndAnswer && !String.IsNullOrWhiteSpace(passwordQuestion))
            {
                status = MembershipCreateStatus.InvalidQuestion;
                return null;
            }

            if (RequiresQuestionAndAnswer && !String.IsNullOrWhiteSpace(passwordAnswer))
            {
                status = MembershipCreateStatus.InvalidAnswer;
                return null;
            }

            if (GetUser(userName , false) != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            if (GetUser(providerUserKey, false) != null)
            {
                status = MembershipCreateStatus.DuplicateProviderUserKey;
                return null;
            }

            if (RequiresUniqueEmail && !String.IsNullOrWhiteSpace(GetUserNameByEmail(email)))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            var buffer = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buffer);
            var salt = Convert.ToBase64String(buffer);

            var creationDate = DateTime.UtcNow;

            var bsonDocument = new BsonDocument
            {
                { "ApplicationName", ApplicationName },
                { "CreationDate", creationDate },
                { "Email", email },
                { "EmailLower", email.ToLowerInvariant() },
                { "FailedPasswordAnswerAttemptCount", 0 },
                { "FailedPasswordAnswerAttemptWindowStart", creationDate },
                { "FailedPasswordAttemptCount", 0 },
                { "FailedPasswordAttemptWindowStart", creationDate },
                { "IsApproved", isApproved },
                { "IsLockedOut", false },
                { "LastActivityDate", creationDate },
                { "LastLockoutDate", new DateTime(1970, 1, 1) },
                { "LastLoginDate", creationDate },
                { "LastPasswordChangedDate", creationDate },
                { "Password", EncodePassword(password, PasswordFormat, salt) },
                { "PasswordAnswer", EncodePassword(passwordAnswer, PasswordFormat, salt) },
                { "PasswordQuestion", passwordQuestion },
                { "Salt", salt },
                { "Username", userName },
                { "UsernameLower", userName .ToLower() }
            };

            _mongoCollection.Insert(bsonDocument);
            status = MembershipCreateStatus.Success;
            return GetUser(userName , false);
        }

        public override bool DeleteUser(string userName , bool deleteAllRelatedData)
        {
            var query = QueryUsernameIs(userName );
            return _mongoCollection.Remove(query).Ok;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();

            var query = QueryEmail(emailToMatch);
            totalRecords = (int)_mongoCollection.FindAs<BsonDocument>(query).Count();

            foreach (var bsonDocument in _mongoCollection.FindAs<BsonDocument>(query).SetSkip(pageIndex * pageSize).SetLimit(pageSize))
            {
                membershipUsers.Add(ToMembershipUser(bsonDocument));
            }

            return membershipUsers;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();

            var query = QueryUsernameMatches(usernameToMatch);
            totalRecords = (int)_mongoCollection.FindAs<BsonDocument>(query).Count();

            foreach (var bsonDocument in _mongoCollection.FindAs<BsonDocument>(query).SetSkip(pageIndex * pageSize).SetLimit(pageSize))
            {
                membershipUsers.Add(ToMembershipUser(bsonDocument));
            }

            return membershipUsers;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var membershipUsers = new MembershipUserCollection();

            var query = Query.EQ("ApplicationName", ApplicationName);
            totalRecords = (int)_mongoCollection.FindAs<BsonDocument>(query).Count();

            foreach (var bsonDocument in _mongoCollection.FindAs<BsonDocument>(query).SetSkip(pageIndex * pageSize).SetLimit(pageSize))
            {
                membershipUsers.Add(ToMembershipUser(bsonDocument));
            }

            return membershipUsers;
        }

        public override int GetNumberOfUsersOnline()
        {
            var timeSpan = TimeSpan.FromMinutes(Membership.UserIsOnlineTimeWindow);
            return (int)_mongoCollection.Count(Query.And(Query.EQ("ApplicationName", ApplicationName), Query.GT("LastActivityDate", DateTime.UtcNow.Subtract(timeSpan))));
        }

        public override string GetPassword(string userName , string answer)
        {
            if (!EnablePasswordRetrieval)
                throw new NotSupportedException("This Membership Provider has not been configured to support password retrieval.");

            var query = QueryUsernameIs(userName);
            var bsonDocument = _mongoCollection.FindOneAs<BsonDocument>(query);

            if (RequiresQuestionAndAnswer && !VerifyPasswordAnswer(bsonDocument, answer))
                throw new MembershipPasswordException("The password-answer supplied is invalid.");

            return DecodePassword(bsonDocument["Password"].AsString);
        }

        public override MembershipUser GetUser(string userName , bool userIsOnline)
        {
            var query = QueryUsernameIs(userName );

            var bsonDocument = _mongoCollection.FindOneAs<BsonDocument>(query);

            if (bsonDocument == null)
                return null;

            if (!userIsOnline) return ToMembershipUser(bsonDocument);
            var update = Update.Set("LastActivityDate", DateTime.UtcNow);
            _mongoCollection.Update(query, update);

            return ToMembershipUser(bsonDocument);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            var query = Query.EQ("_id", (ObjectId)providerUserKey);
            var bsonDocument = _mongoCollection.FindOneAs<BsonDocument>(query);

            if (bsonDocument == null)
                return null;

            if (!userIsOnline) return ToMembershipUser(bsonDocument);
            var update = Update.Set("LastActivityDate", DateTime.UtcNow);
            _mongoCollection.Update(query, update);

            return ToMembershipUser(bsonDocument);
        }

        public override string GetUserNameByEmail(string email)
        {
            var query = QueryEmail(email);
            var bsonDocument = _mongoCollection.FindOneAs<BsonDocument>(query);
            return bsonDocument == null ? null : bsonDocument["Username"].AsString;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            try
            {
                ApplicationName = MembershipApplicationName;

                _enablePasswordReset = Boolean.Parse(config["enablePasswordReset"] ?? "true");
                _enablePasswordRetrieval = Boolean.Parse(config["enablePasswordRetrieval"] ?? "false");
                _maxInvalidPasswordAttempts = Int32.Parse(config["maxInvalidPasswordAttempts"] ?? "5");
                _minRequiredNonAlphanumericCharacters = Int32.Parse(config["minRequiredNonAlphanumericCharacters"] ?? "1");
                _minRequiredPasswordLength = Int32.Parse(config["minRequiredPasswordLength"] ?? "7");
                _passwordAttemptWindow = Int32.Parse(config["passwordAttemptWindow"] ?? "10");
                _passwordFormat = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), config["passwordFormat"] ?? "Hashed");
                _passwordStrengthRegularExpression = config["passwordStrengthRegularExpression"] ?? String.Empty;
                _requiresQuestionAndAnswer = Boolean.Parse(config["requiresQuestionAndAnswer"] ?? "false");
                _requiresUniqueEmail = Boolean.Parse(config["requiresUniqueEmail"] ?? "true");

                if (PasswordFormat == MembershipPasswordFormat.Hashed && EnablePasswordRetrieval)
                    throw new ProviderException("Configured settings are invalid: Hashed passwords cannot be retrieved. Either set the password format to different type, or set enablePasswordRetrieval to false.");

                //need try catch
                _mongoCollection.EnsureIndex("ApplicationName");
                _mongoCollection.EnsureIndex("EmailLower");
                _mongoCollection.EnsureIndex("UsernameLower");

                base.Initialize(name, config);
            }
            catch(Exception ex)
            {
                string errMsg = ex.ToString();
            }
        }

        public override string ResetPassword(string userName , string answer)
        {
            if (!EnablePasswordReset)
                throw new NotSupportedException("This provider is not configured to allow password resets. To enable password reset, set _enablePasswordReset to \"true\" in the configuration file.");

            var query = QueryUsernameIs(userName );
            var bsonDocument = _mongoCollection.FindOneAs<BsonDocument>(query);

            if (RequiresQuestionAndAnswer && !VerifyPasswordAnswer(bsonDocument, answer))
                throw new MembershipPasswordException("The password-answer supplied is invalid.");

            var password = Membership.GeneratePassword(MinRequiredPasswordLength, MinRequiredNonAlphanumericCharacters);

            Update.Set("LastPasswordChangedDate", DateTime.UtcNow).Set("Password", EncodePassword(password, PasswordFormat, bsonDocument["Salt"].AsString));

            return password;
        }

        public override bool UnlockUser(string userName )
        {
            var query = QueryUsernameIs(userName );
            var update = Update.Set("FailedPasswordAttemptCount", 0).Set("FailedPasswordAttemptWindowStart", new DateTime(1970, 1, 1)).Set("FailedPasswordAnswerAttemptCount", 0).Set("FailedPasswordAnswerAttemptWindowStart", new DateTime(1970, 1, 1)).Set("IsLockedOut", false).Set("LastLockoutDate", new DateTime(1970, 1, 1));
            return _mongoCollection.Update(query, update).Ok;
        }

        public override void UpdateUser(MembershipUser user)
        {
            if (user.ProviderUserKey == null) return;
            var query = Query.EQ("_id", (ObjectId)user.ProviderUserKey);
            var bsonDocument = _mongoCollection.FindOneAs<BsonDocument>(query);

            if (bsonDocument == null)
                throw new ProviderException("The user was not found.");

            var update = Update.Set("ApplicationName", ApplicationName)
                .Set("Comment", user.Comment)
                .Set("Email", user.Email)
                .Set("EmailLower", user.Email.ToLowerInvariant())
                .Set("IsApproved", user.IsApproved)
                .Set("LastActivityDate", user.LastActivityDate.ToUniversalTime())
                .Set("LastLoginDate", user.LastLoginDate.ToUniversalTime());

            _mongoCollection.Update(query, update);
        }

        public override bool ValidateUser(string userName , string password)
        {
            var query = QueryUsernameIs(userName);
            var bsonDocument = _mongoCollection.FindOneAs<BsonDocument>(query);

            if (bsonDocument == null || !bsonDocument["IsApproved"].AsBoolean || bsonDocument["IsLockedOut"].AsBoolean)
                return false;

            if (VerifyPassword(bsonDocument, password))
            {
                _mongoCollection.Update(query, Update.Set("LastLoginDate", DateTime.UtcNow));
                return true;
            }

            _mongoCollection.Update(query, Update.Inc("FailedPasswordAttemptCount", 1).Set("FailedPasswordAttemptWindowStart", DateTime.UtcNow));
            return false;
        }

        #region Private Methods

        private string DecodePassword(string password)
        {
            switch (_passwordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    return password;

                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Hashed passwords cannot be decoded.");

                default:
                    var passwordBytes = Convert.FromBase64String(password);
                    var decryptedBytes = DecryptPassword(passwordBytes);
                    return decryptedBytes == null ? null : Encoding.Unicode.GetString(decryptedBytes, 16, decryptedBytes.Length - 16);
            }
        }

        private string EncodePassword(string password, MembershipPasswordFormat membershipPasswordFormat, string salt)
        {
            if (password == null)
                return null;

            if (membershipPasswordFormat == MembershipPasswordFormat.Clear)
                return password;

            var passwordBytes = Encoding.Unicode.GetBytes(password);
            var saltBytes = Convert.FromBase64String(salt);
            var allBytes = new byte[saltBytes.Length + passwordBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, allBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, allBytes, saltBytes.Length, passwordBytes.Length);

            if (membershipPasswordFormat != MembershipPasswordFormat.Hashed)
                return Convert.ToBase64String(EncryptPassword(allBytes));
            var hashAlgorithm = HashAlgorithm.Create("SHA1");
            if (hashAlgorithm != null)
                return Convert.ToBase64String(hashAlgorithm.ComputeHash(allBytes));

            return Convert.ToBase64String(EncryptPassword(allBytes));
        }

        private MembershipUser ToMembershipUser(BsonDocument bsonDocument)
        {
            if (bsonDocument == null)
                return null;

            var comment = (bsonDocument.Contains("Comment") ? bsonDocument["Comment"].AsString : null) ??
                          "Why do we need comments?";

            var email = bsonDocument.Contains("Email") ? bsonDocument["Email"].AsString : null;
            var passwordQuestion = bsonDocument.Contains("PasswordQuestion") ? bsonDocument["PasswordQuestion"].AsString : null;

            if (String.IsNullOrEmpty(Name))
            {
                var newUser = new MembershipUser(ApplicationName, bsonDocument["Username"].AsString, bsonDocument["_id"].AsObjectId, email, passwordQuestion, comment, bsonDocument["IsApproved"].AsBoolean, bsonDocument["IsLockedOut"].AsBoolean, bsonDocument["CreationDate"].ToUniversalTime(), bsonDocument["LastLoginDate"].ToUniversalTime(), bsonDocument["LastActivityDate"].ToUniversalTime(), bsonDocument["LastPasswordChangedDate"].ToUniversalTime(), bsonDocument["LastLockoutDate"].ToUniversalTime());
                return newUser;
            }
            else
            {
                var newUser = new MembershipUser(Name, bsonDocument["Username"].AsString, bsonDocument["_id"].AsObjectId, email, passwordQuestion, comment, bsonDocument["IsApproved"].AsBoolean, bsonDocument["IsLockedOut"].AsBoolean, bsonDocument["CreationDate"].ToUniversalTime(), bsonDocument["LastLoginDate"].ToUniversalTime(), bsonDocument["LastActivityDate"].ToUniversalTime(), bsonDocument["LastPasswordChangedDate"].ToUniversalTime(), bsonDocument["LastLockoutDate"].ToUniversalTime());
                return newUser;
            }
        }

        private bool VerifyPassword(BsonDocument user, string password)
        {
            return user["Password"].AsString == EncodePassword(password, PasswordFormat, user["Salt"].AsString);
        }

        private bool VerifyPasswordAnswer(BsonDocument user, string passwordAnswer)
        {
            return user["PasswordAnswer"].AsString == EncodePassword(passwordAnswer, PasswordFormat, user["Salt"].AsString);
        }

        #endregion
    }
}
