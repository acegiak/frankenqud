using XRL.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XRL.Language;
using System;


namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_frankenpopinit : IPart
	{
		public string Mods = string.Empty;

		public string Tiers = string.Empty;

		public acegiak_frankenpopinit()
		{
			base.Name = "acegiak_frankenpopinit";

            // AddToPopTable("RandomLiquid", new PopulationObject { Blueprint = "SomeLiquid" });
            // AddToPopTable("RandomFaction", new PopulationObject { Blueprint = "SomeFaction" });


            PopulationGroup group = new PopulationGroup{ Style="pickeach"};
            group.Items.Add(new PopulationObject{ Blueprint="OperatingTable"});
            group.Items.Add(new PopulationObject{ Blueprint="Bloodsplatter", Number="1-8" });

            AddToPopTable("SultanDungeons_Cubbies_*Default", group);


            PopulationGroup group2 = new PopulationGroup{ Style="pickeach"};
            group2.Items.Add(new PopulationObject{ Blueprint="OperatingTable" });
            group2.Items.Add(new PopulationObject{ Blueprint="Bloodsplatter", Number="1-8" });

            group2.Weight = 5;
            AddToPopTable("CommonOddEncounters",group2);

		}
    

        public static bool AddToPopTable(string table, params PopulationItem[] items) {
            PopulationInfo info;
            if (!PopulationManager.Populations.TryGetValue(table, out info))
                return false;
                
            // If this is a single group population, add to that group.
            if (info.Items.Count == 1 && info.Items[0] is PopulationGroup) { 
                var group = info.Items[0] as PopulationGroup;
                group.Items.AddRange(items);
                return true;
            }

            info.Items.AddRange(items);
            return true;
        }
    }
}