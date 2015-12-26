using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetIdentity3PostgreSQL.Tables
{
    /// <summary>
    /// Classe que representa a tabela AspNetUsers
    /// </summary>
    public class UserTable<TUser>
        where TUser : IdentityUser
    {
        private PostgreSQLDatabase _database;

        internal const string TableName = "AspNetUsers";
        internal const string FieldId = "Id";
        internal const string FieldUserName = "UserName";
        internal const string FieldPassword = "PasswordHash";
        internal const string FieldSecurityStamp = "SecurityStamp";
        internal const string FieldEmail = "Email";
        internal const string FieldEmailConfirmed = "EmailConfirmed";
        internal const string FieldPhoneNumber = "PhoneNumber";
        internal const string FieldPhoneNumberConfirmed = "PhoneNumberConfirmed";
        internal const string FieldTwoFactorEnabled = "TwoFactorEnabled";
        internal const string FieldLockoutEndDate = "LockoutEndDateUtc";
        internal const string FieldLockoutEnabled = "LockoutEnabled";
        internal const string FieldAccessFailedCount = "AccessFailedCount";
        internal const string FieldConcurrencyStamp = "ConcurrencyStamp";




        internal static string fullTableName = Consts.Schema.Quoted() + "." + TableName.Quoted();

        //TODO: Adicionar os campos faltantes para validação de lockout, phonenumber, e two steps authentication

        /// <summary>
        /// Construtor que Instancia a base postgresql
        /// </summary>
        /// <param name="database"></param>
        public UserTable(PostgreSQLDatabase database)
        {
            _database = database;
        }


        /// <summary>
        /// Function to load an user object
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>

        internal static TUser loadUser(Dictionary<string, string> row)
        {
            TUser user = null;
            user = (TUser)Activator.CreateInstance(typeof(TUser));
            user.Id = new Guid(row[FieldId]);
            user.UserName = row[FieldUserName];
            user.PasswordHash = string.IsNullOrEmpty(row[FieldPassword]) ? null : row[FieldPassword];
            user.SecurityStamp = string.IsNullOrEmpty(row[FieldSecurityStamp]) ? null : row[FieldSecurityStamp];
            user.Email = string.IsNullOrEmpty(row[FieldEmail]) ? null : row[FieldEmail];
            user.EmailConfirmed = row[FieldEmailConfirmed] == "True";
            user.PhoneNumber = string.IsNullOrEmpty(row[FieldPhoneNumber]) ? null : row[FieldPhoneNumber];
            user.PhoneNumberConfirmed = row[FieldPhoneNumberConfirmed] == "1" ? true : false;
            user.LockoutEnabled = row[FieldLockoutEnabled] == "1" ? true : false;
            user.LockoutEnd = string.IsNullOrEmpty(row[FieldLockoutEndDate]) ? DateTime.Now : DateTime.Parse(row[FieldLockoutEndDate]);
            user.AccessFailedCount = string.IsNullOrEmpty(row[FieldAccessFailedCount]) ? 0 : int.Parse(row[FieldAccessFailedCount]);
            user.TwoFactorEnabled = row[FieldTwoFactorEnabled] == "1" ? true : false;

            return user;

        }


        /// <summary>
        /// Gets the user's name, provided with an ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserName(string userId)
        {
            string commandText = "SELECT " + FieldUserName.Quoted() + " FROM " + fullTableName + " WHERE " + FieldId.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", userId } };

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Gets the user's ID, provided with a user name.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetUserId(string userName)
        {
            if (userName != null)
                userName = userName.ToLower();
            string commandText = "SELECT " + FieldId.Quoted() + " FROM " + fullTableName + " WHERE lower(" + FieldUserName.Quoted() + ") = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", userName } };

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Returns an TUser given the user's id.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public TUser GetUserById(string userId)
        {

            string commandText = "SELECT * FROM " + fullTableName + " WHERE " + FieldId.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", userId } };

            var row = _database.ExecuteQueryGetSingleRow(commandText, parameters);
            return loadUser(row);
        }


        /// <summary>
        /// Returns a list of TUser instances given a user name.
        /// </summary>
        /// <param name="userName">User's name.</param>
        /// <returns></returns>
        public TUser GetUserByUserName(string userName)
        {
            if (userName != null)
                userName = userName.ToLower();

            string commandText = "SELECT *  FROM " + fullTableName + " WHERE " + FieldUserName.Quoted() + " = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", userName } };

            var row = _database.ExecuteQueryGetSingleRow(commandText, parameters);
            return loadUser(row);
        }

        /// <summary>
        /// Returns a list of TUser instances given a user email.
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <returns></returns>
        public TUser GetUserByEmail(string email)
        {
            //Due to PostgreSQL's case sensitivity, we have another column for the user name in lowercase.
            if (email != null)
                email = email.ToLower();

            string commandText = "SELECT *  FROM " + fullTableName + " WHERE " + FieldEmail.Quoted() + " = @email";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@email", email } };

            var row = _database.ExecuteQueryGetSingleRow(commandText, parameters);

            return loadUser(row);
        }

        /// <summary>
        /// Return the user's password hash.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public string GetPasswordHash(string userId)
        {

            string commandText = "select " + FieldPassword.Quoted() + " from " + fullTableName + " where " + FieldId.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", userId);

            var passHash = _database.GetStrValue(commandText, parameters);
            if (string.IsNullOrEmpty(passHash))
            {
                return null;
            }

            return passHash;
        }

        /// <summary>
        /// Sets the user's password hash.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public int SetPasswordHash(string userId, string passwordHash)
        {

            string commandText = "update " + fullTableName + " set " + FieldPassword.Quoted() + " = @pwdHash where " + FieldId.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@pwdHash", passwordHash);
            parameters.Add("@id", userId);

            return _database.ExecuteSQL(commandText, parameters);
        }

        /// <summary>
        /// Returns the user's security stamp.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetSecurityStamp(string userId)
        {

            string commandText = "select " + FieldSecurityStamp.Quoted() + " from " + fullTableName + " where " + FieldId.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", userId } };
            var result = _database.GetStrValue(commandText, parameters);

            return result;
        }

        /// <summary>
        /// Inserts a new user in the AspNetUsers table.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Insert(TUser user)
        {
            var lowerCaseEmail = user.Email == null ? null : user.Email.ToLower();

            string commandText = "insert into " + fullTableName +
                "(" + FieldId.Quoted() + ", " +
                FieldUserName.Quoted() + ", " +
                FieldPassword.Quoted() + ", " +
                FieldSecurityStamp.Quoted() + ", " +
                FieldEmail.Quoted() + ", " +
                FieldEmailConfirmed.Quoted() + ", " +
                FieldPhoneNumber.Quoted() + ", " +
                FieldPhoneNumberConfirmed.Quoted() + ", " +
                FieldTwoFactorEnabled.Quoted() + ", " +
                FieldLockoutEndDate.Quoted() + ", " +
                FieldLockoutEnabled.Quoted() + ", " +
                FieldAccessFailedCount.Quoted() + ")" +
                " VALUES (@id, @name, @pwdHash, @SecStamp, @email, @emailconfirmed, @phoneNumber, @phoneNumberConfirmed, @twoFactorEnabled, @lockoutEndDate, @lockoutEnabled, @AccessFailedCount);";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", user.UserName);
            parameters.Add("@id", user.Id.ToString());
            parameters.Add("@pwdHash", user.PasswordHash);
            parameters.Add("@SecStamp", user.SecurityStamp);
            parameters.Add("@email", user.Email);
            parameters.Add("@emailconfirmed", user.EmailConfirmed);
            parameters.Add("@phoneNumber", user.PhoneNumber);
            parameters.Add("@phoneNumberConfirmed", user.PhoneNumberConfirmed);
            parameters.Add("@twoFactorEnabled", user.TwoFactorEnabled);
            parameters.Add("@lockoutEndDate", user.LockoutEnd);
            parameters.Add("@lockoutEnabled", user.LockoutEnabled);
            parameters.Add("@AccessFailedCount", user.AccessFailedCount);

            return _database.ExecuteSQL(commandText, parameters);
        }

        /// <summary>
        /// Deletes a user from the AspNetUsers table.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        private int Delete(Guid userId)
        {
            string commandText = "DELETE FROM " + fullTableName + " WHERE " + FieldId.Quoted() + " = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", userId.ToString());

            return _database.ExecuteSQL(commandText, parameters);
        }

        /// <summary>
        /// Deletes a user from the AspNetUsers table.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Delete(TUser user)
        {
            return Delete(user.Id);
        }

        /// <summary>
        /// Updates a user in the AspNetUsers table.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Update(TUser user)
        {
            string commandText = "UPDATE " + fullTableName + " set " +
                FieldUserName.Quoted() + " = @userName, " +
                FieldPassword.Quoted() + " = @pswHash, " +
                FieldSecurityStamp.Quoted() + " = @secStamp, " +
                FieldEmail.Quoted() + " = @email, " +
                FieldEmailConfirmed.Quoted() + " = @emailconfirmed, " +
                FieldPhoneNumber.Quoted() + " = @phoneNumber, " +
                FieldPhoneNumberConfirmed.Quoted() + " =  @phoneNumberConfirmed, " +
                FieldTwoFactorEnabled.Quoted() + " =  @twoFactorEnabled, " +
                FieldLockoutEndDate.Quoted() + " =  @lockoutEndDate, " +
                FieldLockoutEnabled.Quoted() + " =  @lockoutEnabled, " +
                FieldAccessFailedCount.Quoted() + " =  @AccessFailedCount " +
                " where " + FieldId.Quoted() + " = @userId";


            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userName", user.UserName);
            parameters.Add("@pswHash", user.PasswordHash);
            parameters.Add("@secStamp", user.SecurityStamp);
            parameters.Add("@userId", user.Id.ToString());
            parameters.Add("@email", user.Email);
            parameters.Add("@emailconfirmed", user.EmailConfirmed);
            parameters.Add("@phoneNumber", user.PhoneNumber);
            parameters.Add("@phoneNumberConfirmed", user.PhoneNumberConfirmed);
            parameters.Add("@twoFactorEnabled", user.TwoFactorEnabled);
            parameters.Add("@lockoutEndDate", user.LockoutEnd);
            parameters.Add("@lockoutEnabled", user.LockoutEnabled);
            parameters.Add("@AccessFailedCount", user.AccessFailedCount);
            return _database.ExecuteSQL(commandText, parameters);
        }

        public async Task<TUser> GetUserByNameAsync(string normalizedUserName)
        {
            TUser user = null;

            await Task.Run(() =>
            {
                user = GetUserByUserName(normalizedUserName);
            });
            return user;
        }

        public async Task<TUser> GetUserByIdAsync(string userId)
        {
            TUser user = null;

            await Task.Run(() =>
            {
                user = GetUserById(userId);
            });
            return user;
        }
    }
}