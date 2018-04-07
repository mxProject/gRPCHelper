using System;
using System.Collections.Generic;
using System.Text;

using Examples.GrpcModels;

namespace Examples.GrpcServer
{

    /// <summary>
    /// 
    /// </summary>
    internal static class SampleDataRepository
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static IDictionary<string, Team> GetTeams(int count)
        {

            Dictionary<string, Team> teams = new Dictionary<string, Team>();

            foreach ( Team team in GenerateTeams(count))
            {
                teams.Add(team.Code, team);
            }

            return teams;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static IEnumerable<Team> GenerateTeams(int count)
        {

            string[] countries = new string[] { "Japan", "Spain", "Germany", "Italy", "England", "France" };

            for (int i = 1; i <= count; ++i)
            {

                Team team = new Team();

                team.Code = i.ToString("d" + count.ToString().Length);
                team.Name = string.Format("Team{0}", team.Code);

                int r;
                Math.DivRem(i, countries.Length, out r);

                team.Country = countries[r];

                yield return team;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static IEnumerable<Player> GeneratePlayers(int initial, int count, IDictionary<string, Team> teams)
        {

            Random r = new Random();

            string[] teamCodes = new string[teams.Count];

            teams.Keys.CopyTo(teamCodes, 0);

            for (int i = 1; i <= count; ++i)
            {

                Player player = new Player();

                player.Name = string.Format("Player{0}", initial + i - 1);
                player.Age = r.Next(16, 40);
                player.TeamCode = teamCodes[r.Next(0, teams.Count - 1)];
                player.Positions.AddRange(GeneratePositions(r));

                yield return player;

            }

        }

        private static IList<string> GeneratePositions(Random r)
        {

            List<string> positions = new List<string>(3);

            if (r.NextDouble() > 0.5) { positions.Add("FW"); }
            if (r.NextDouble() > 0.5) { positions.Add("MF"); }
            if (r.NextDouble() > 0.5) { positions.Add("DF"); }
            if (positions.Count == 0) { positions.Add("GK"); }

            return positions;

        }

    }

}
