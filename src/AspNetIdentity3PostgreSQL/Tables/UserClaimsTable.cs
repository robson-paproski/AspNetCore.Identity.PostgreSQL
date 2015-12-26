using AspNet.Identity.PostgreSQL.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AspNet.Identity.PostgreSQL.Tables
{
    /// <summary>
    /// Class that represents the AspNetUserClaims table in the PostgreSQL Database.
    /// </summary>
    public class UserClaimsTable
    {
        private PostgreSQLDatabase _database;

        internal const string tableName = "AspNetUserClaims";        
        internal const string fieldClaimType = "ClaimType";
        internal const string fieldClaimValue = "ClaimValue";
        internal const string fieldUserID = "UserId";
        internal static string fullTableName = Consts.Schema.Quoted() + "." + tableName.Quoted();

        /// <summary>
        /// Constructor that takes a PostgreSQLDatabase instance.
        /// </summary>
        /// <param name="database"></param>
        public UserClaimsTable(PostgreSQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public ClaimsIdentity FindByUserId(Guid userId)
        {
            ClaimsIdentity claims = new ClaimsIdentity();
            string commandText = "SELECT * FROM "+fullTableName+" WHERE "+fieldUserID.Quoted()+" = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "userId", userId } };

            var rows = _database.ExecuteQuery(commandText, parameters);
            foreach (var row in rows)
            {
                Claim claim = new Claim(row[fieldClaimType], row[fieldClaimValue]);
                claims.AddClaim(claim);
            }

            return claims;
        }

        /// <summary>
        /// Deletes all claims from a user given a userId.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string commandText = "DELETE FROM "+fullTableName+" WHERE "+fieldUserID.Quoted()+" = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", userId);

            return _database.ExecuteSQL(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new claim record in AspNetUserClaims table.
        /// </summary>
        /// <param name="userClaim">User's claim to be added.</param>
        /// <param name="userId">User's Id.</param>
        /// <returns></returns>
        public int Insert(Claim userClaim, Guid userId)
        {
            string commandText = "INSERT INTO "+fullTableName+" ("+fieldClaimValue.Quoted()+", "+fieldClaimType.Quoted()+", "+fieldUserID.Quoted()+") "+
                " VALUES (@value, @type, @userId)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("value", userClaim.Value);
            parameters.Add("type", userClaim.Type);
            parameters.Add("userId", userId);

            return _database.ExecuteSQL(commandText, parameters);
        }

        /// <summary>
        /// Deletes a claim record from a user.
        /// </summary>
        /// <param name="user">The user to have a claim deleted.</param>
        /// <param name="claim">A claim to be deleted from user.</param>
        /// <returns></returns>
        public int Delete(IdentityUser user, Claim claim)
        {
            string commandText = "DELETE FROM "+fullTableName+" WHERE "+fieldUserID.Quoted()+" = @userId "+
                " AND "+fieldClaimValue.Quoted()+" = @value AND "+fieldClaimType.Quoted()+" = @type";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", user.Id);
            parameters.Add("value", claim.Value);
            parameters.Add("type", claim.Type);

            return _database.ExecuteSQL(commandText, parameters);
        }

        public int Replace(Guid userId, Claim claim, Claim newClaim)
        {
            string commandText = "UPDATE " + fullTableName + " SET " + fieldClaimType.Quoted() + "  = @NewClaimType,  " +
                                 fieldClaimValue.Quoted() + " = @NewClaimValue WHERE " + fieldUserID.Quoted() +
                                 " = @userId " +
                                 " and " + fieldClaimType.Quoted() + "  = @OldClaimType and  " +
                                 fieldClaimValue.Quoted() + " = @OldClaimValue";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("NewClaimType", newClaim.Type);
            parameters.Add("NewClaimValue", newClaim.Value);
            parameters.Add("userId", userId.ToString());
            parameters.Add("OldClaimType", claim.Type);
            parameters.Add("OldClaimValue", claim.Value);

            return _database.ExecuteSQL(commandText, parameters);
        }

        public List<IdentityUser> FindUsersByClaim(Claim claim)
        {
            string commandText = "SELECT * FROM " + fullTableName + " WHERE "+
                fieldClaimType.Quoted() + "  = @ClaimType and  " + fieldClaimValue.Quoted() + " = @ClaimValue";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("ClaimType", claim.Type);
            parameters.Add("ClaimValue", claim.Value);

            var rows = _database.ExecuteQuery(commandText, parameters);
            var list = rows.Select(row => UserTable<IdentityUser>.loadUser(row)).ToList();

            return list;
        }
    }
}
