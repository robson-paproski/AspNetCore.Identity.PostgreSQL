using System;
using System.Collections.Generic;
using AspNetCore.Identity.PostgreSQL.Context;
using Microsoft.AspNetCore.Identity;


namespace AspNetCore.Identity.PostgreSQL.Tables
{
    /// <summary>
    /// Class that represents the AspNetUserLogins table in the PostgreSQL Database.
    /// </summary>
    public class UserLoginsTable
    {
        private PostgreSQLDatabase _database;

        internal const string tableName = "AspNetUserLogins";
        internal const string fielduserID = "UserId";
        internal const string fieldLoginProvider = "LoginProvider";
        internal const string fieldProviderKey = "ProviderKey";
        internal const string fieldProviderDisplayName = "ProviderDisplayName";
        internal static string fullTableName = Consts.Schema.Quoted() + "." + tableName.Quoted();        

        /// <summary>
        /// Constructor that takes a PostgreSQLDatabase instance.
        /// </summary>
        /// <param name="database"></param>
        public UserLoginsTable(PostgreSQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Deletes a login record from a user in the UserLogins table.
        /// </summary>
        /// <param name="user">User to have login deleted.</param>
        /// <param name="login">Login to be deleted from user.</param>
        /// <returns></returns>
        public int Delete(IdentityUser user, UserLoginInfo login)
        {           
            string commandText = "DELETE FROM "+fullTableName+" WHERE "+fielduserID.Quoted()+" = @userId AND "+fieldLoginProvider.Quoted()+" = @loginProvider "+
                " AND "+fieldProviderKey.Quoted() + " = @providerKey";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", user.Id);
            parameters.Add("loginProvider", login.LoginProvider);
            parameters.Add("providerKey", login.ProviderKey);

            return _database.ExecuteSQL(commandText, parameters);
        }

        /// <summary>
        /// Deletes all logins from a user in the UserLogins table.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string commandText = "DELETE FROM "+fullTableName+" WHERE "+fielduserID.Quoted()+" = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", userId);

            return _database.ExecuteSQL(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new login record in the AspNetUserLogins table.
        /// </summary>
        /// <param name="user">User to have new login added.</param>
        /// <param name="login">Login to be added.</param>
        /// <returns></returns>
        public int Insert(IdentityUser user, UserLoginInfo login)
        {
            string commandText = "INSERT INTO "+fullTableName+"  ("+fieldLoginProvider.Quoted()+", "+fieldProviderKey.Quoted()+", "+fielduserID.Quoted()+", "+fieldProviderDisplayName.Quoted()+") "+
                                                                "VALUES (@loginProvider, @providerKey, @userId, @ProviderDisplayName)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("loginProvider", login.LoginProvider);
            parameters.Add("providerKey", login.ProviderKey);
            parameters.Add("ProviderDisplayName", login.ProviderDisplayName);
            parameters.Add("userId", user.Id);

            return _database.ExecuteSQL(commandText, parameters);
        }

        /// <summary>
        /// Return a user ID given a user's login.
        /// </summary>
        /// <param name="userLogin">The user's login info.</param>
        /// <returns></returns>
        public Guid FindUserIdByLogin(UserLoginInfo userLogin)
        {
            string commandText = "SELECT "+fielduserID.Quoted()+" FROM "+fullTableName+" WHERE "+fieldLoginProvider.Quoted()+" = @loginProvider AND "+fieldProviderKey.Quoted()+" = @providerKey";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("loginProvider", userLogin.LoginProvider);
            parameters.Add("providerKey", userLogin.ProviderKey);

            return new Guid((string)_database.ExecuteQueryGetSingleObject(commandText, parameters));
        }

        /// <summary>
        /// Returns a list of user's logins.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public List<UserLoginInfo> FindByUserId(Guid userId)
        {
            List<UserLoginInfo> logins = new List<UserLoginInfo>();
            string commandText = "SELECT * FROM "+fullTableName+" WHERE "+fielduserID.Quoted()+" = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@userId", userId } };

            var rows = _database.ExecuteQuery(commandText, parameters);
            foreach (var row in rows)
            {
                var login = new UserLoginInfo(row[fieldLoginProvider], row[fieldProviderKey], row[fieldProviderDisplayName]);
                logins.Add(login);
            }

            return logins;
        }
    }
}
