using System;
using XRL.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XRL.Language;



namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_DismemberLogger : IPart
	{

		public acegiak_DismemberLogger()
		{
			base.Name = "acegiak_DismemberLogger";
		}

		public override bool SameAs(IPart p)
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "Dismember");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "Dismember")
			{
				acegiak_DismemberSearcher.dismeberees.Add(ParentObject);
				if(acegiak_DismemberSearcher.dismeberees.Count > 64){
					acegiak_DismemberSearcher.dismeberees.RemoveRange(0,32);
				}
				
			}

			return base.FireEvent(E);
		}

		

		

    }


}