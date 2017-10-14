using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.Identity.PostgreSQL.Context;


namespace AspNetCore.Identity.PostgreSQL.Tables
{
    /// <summary>
    /// Class that represents the AspNetRoles table in the PostgreSQL Database.
    /// </summary>
    public class RoleTable<TRole>  where TRole : IdentityRole
    {
        private PostgreSQLDatabase _database;
        private UserClaimsTable _userClaimsTable;

        internal const string tableName = "AspNetRoles";
        internal const string fieldId   = "Id";
        internal const string fieldName = "Name";
        internal static string fullTableName = Consts.Schema.Quoted() + "." + tableName.Quoted();

       

        /// <summary>
        /// Constructor that takes a PostgreSQLDatabase instance.
        /// </summary>
        /// <param name="database"></param>
        public RoleTable(PostgreSQLDatabase database)
        {
            _database = database;
            _userClaimsTable = new UserClaimsTable(database);
        }

        /// <summary>
        /// Deletes a role record from the AspNetRoles table.
        /// </summary>
        /// <param name="roleId">The role Id</param>
        /// <returns></returns>
        public int Delete(string roleId)
        {
            string commandText = "DELETE FROM "+fullTableName+" WHERE "+fieldId.Quoted()+" = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", roleId);

            return _database.ExecuteSQL(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new Role record in the AspNetRoles table.
        /// </summary>
        /// <param name="roleName">The role's name.</param>
        /// <returns></returns>
        public int Insert(IdentityRole role)
        {
            string commandText = "INSERT INTO "+fullTableName+" ("+fieldId.Quoted()+", "+fieldName.Quoted()+") VALUES (@id, @name)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", role.Name);
            parameters.Add("@id", role.Id);

            return _database.ExecuteSQL(commandText, parameters);
        }

        /// <summary>
        /// Returns a role name given the roleId.
        /// </summary>
        /// <param name="roleId">The role Id.</param>
        /// <returns>Role name.</returns>
        public string GetRoleName(string roleId)
        {
            string commandText = "SELECT "+fieldName.Quoted()+" FROM "+fullTableName+" WHERE "+fieldId.Quoted()+" = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", roleId);

            return (string)_database.ExecuteQueryGetSingleObject(commandText, parameters);
        }

        /// <summary>
        /// Returns the role Id given a role name.
        /// </summary>
        /// <param name="roleName">Role's name.</param>
        /// <returns>Role's Id.</returns>
        public string GetRoleId(string roleName)
        {
            string roleId = null;
            string commandText = "SELECT "+fieldId.Quoted()+" FROM "+fullTableName+" WHERE lower("+fieldName.Quoted()+") = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", roleName.ToLower() } };

            var result = _database.ExecuteQueryGetSingleObject(commandText, parameters);
            if (result != null)
            {
                return Convert.ToString(result);
            }

            return roleId;
        }

        /// <summary>
        /// Gets the IdentityRole given the role Id.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IdentityRole GetRoleById(string roleId)
        {
            var roleName = GetRoleName(roleId);
            IdentityRole role = null;

            if (roleName != null)
            {
                role = new IdentityRole(roleName, roleId);
                role.Id = new Guid(roleId);
            }

            return role;

        }

        /// <summary>
        /// Gets the IdentityRole given the role name.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public IdentityRole GetRoleByName(string roleName)
        {
            var roleId = GetRoleId(roleName);
            

            return GetRoleById(roleId); 
        }

        public int Update(IdentityRole role)
        {
            string commandText = "UPDATE "+fullTableName+" SET "+fieldName.Quoted()+" = @name WHERE "+fieldId.Quoted()+" = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", role.Id);

            return _database.ExecuteSQL(commandText, parameters);
        }

        public IList<IdentityUser> GetUsersInRole(string roleName)
        {
            string commandText = "SELECT * FROM " + fullTableName + " WHERE " +
            fieldName.Quoted() + "  = @Name";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Name", roleName);

            var rows = _database.ExecuteQuery(commandText, parameters);
            var list = rows.Select(UserTable<IdentityUser>.loadUser).ToList();

            return list;
        }

        public IList<TRole> GetRoles()
        {
            string commandText = "SELECT * FROM " + fullTableName;

            Dictionary<string, object> parameters = new Dictionary<string, object>();
          

            var rows = _database.ExecuteQuery(commandText, parameters);
            var list = rows.Select(loadRole).ToList();

            return list;
        }


        private TRole loadRole(Dictionary<string, string> arg)
        {
            if ((arg == null) || (arg.Count() == 0))
                return null;

            return new IdentityRole()
            {
                Id = new Guid(arg["Id"]),
                Name = arg["Name"],
                ConcurrencyStamp = arg["ConcurrencyStamp"]
            } as TRole;
        }
    }
}
