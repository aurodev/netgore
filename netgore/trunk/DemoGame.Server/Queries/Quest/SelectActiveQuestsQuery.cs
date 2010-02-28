using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DemoGame.Server.DbObjs;
using NetGore.Db;
using NetGore.Features.Quests;

namespace DemoGame.Server.Queries
{
    [DbControllerQuery]
    public class SelectActiveQuestsQuery : DbQueryReader<CharacterID>
    {
        static readonly string _queryStr =
            string.Format("SELECT `quest_id` FROM `{0}` WHERE `character_id`=@id AND `completed_on` IS NULL",
                          CharacterQuestStatusTable.TableName);

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectActiveQuestsQuery"/> class.
        /// </summary>
        /// <param name="connectionPool"><see cref="DbConnectionPool"/> to use for creating connections to
        /// execute the query on.</param>
        public SelectActiveQuestsQuery(DbConnectionPool connectionPool) : base(connectionPool, _queryStr)
        {
            QueryAsserts.ContainsColumns(CharacterQuestStatusTable.DbColumns, "quest_id", "completed_on", "character_id");
        }

        public IEnumerable<QuestID> Execute(CharacterID id)
        {
            List<QuestID> ret = new List<QuestID>();

            using (var r = ExecuteReader(id))
            {
                while (r.Read())
                {
                    ret.Add(r.GetQuestID(0));
                }
            }

            return ret;
        }

        /// <summary>
        /// When overridden in the derived class, creates the parameters this class uses for creating database queries.
        /// </summary>
        /// <returns>IEnumerable of all the <see cref="DbParameter"/>s needed for this class to perform database queries.
        /// If null, no parameters will be used.</returns>
        protected override IEnumerable<DbParameter> InitializeParameters()
        {
            return CreateParameters("@id");
        }

        /// <summary>
        /// When overridden in the derived class, sets the database parameters values <paramref name="p"/>
        /// based on the values specified in the given <paramref name="item"/> parameter.
        /// </summary>
        /// <param name="p">Collection of database parameters to set the values for.</param>
        /// <param name="item">The value or object/struct containing the values used to execute the query.</param>
        protected override void SetParameters(DbParameterValues p, CharacterID item)
        {
            p["@id"] = (int)item;
        }
    }
}