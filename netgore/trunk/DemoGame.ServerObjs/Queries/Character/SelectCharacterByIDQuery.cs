using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NetGore.Db;

namespace DemoGame.Server.Queries
{
    [DBControllerQuery]
    public class SelectCharacterByIDQuery : DbQueryReader<CharacterID>
    {
        static readonly string _queryString = string.Format("SELECT * FROM `{0}` WHERE `id`=@characterID", DBTables.Character);

        public SelectCharacterByIDQuery(DbConnectionPool connectionPool) : base(connectionPool, _queryString)
        {
        }

        public SelectCharacterQueryValues Execute(CharacterID characterID)
        {
            SelectCharacterQueryValues ret;

            using (IDataReader r = ExecuteReader(characterID))
            {
                if (!r.Read())
                    throw new ArgumentException(string.Format("Could not find character with ID `{0}`.", characterID),
                                                characterID.ToString());

                ret = CharacterQueryHelper.ReadCharacterQueryValues(r);
            }

            return ret;
        }

        protected override IEnumerable<DbParameter> InitializeParameters()
        {
            return CreateParameters("@characterID");
        }

        protected override void SetParameters(DbParameterValues p, CharacterID characterID)
        {
            p["@characterID"] = characterID;
        }
    }
}