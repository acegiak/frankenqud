using System;
using XRL.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XRL.Language;


namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_OperatingTable : IPart
	{
		public string Mods = string.Empty;

		public string Tiers = string.Empty;

		public acegiak_OperatingTable()
		{
			base.Name = "acegiak_OperatingTable";
		}

		public override bool SameAs(IPart p)
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "GetInventoryActions");
			Object.RegisterPartEvent(this, "InvCommandOperate");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "GetInventoryActions")
			{

				E.AddInventoryAction("Operate", 'u',  false, "a&Wu&ygment self", "InvCommandOperate", 100,  false,  false, false);
				
			}
            if(E.ID == "InvCommandOperate"){
                Operate(E.GetGameObjectParameter("Owner"));
            }
			return base.FireEvent(E);
		}

        public void Operate(GameObject who){
            Inventory part2 = who.GetPart<Inventory>();
            List<XRL.World.GameObject> ObjectChoices = new List<XRL.World.GameObject>();
            List<string> ChoiceList = new List<string>();
            List<char> HotkeyList = new List<char>();
            char ch = 'a';
            part2.ForeachObject(delegate(XRL.World.GameObject GO)
            {
                if(GO != null && PartWas(GO) != null){
                    ObjectChoices.Add(GO);
                    HotkeyList.Add(ch);
                    ChoiceList.Add(GO.DisplayName);
                    ch = (char)(ch + 1);
                }
            });
            if (ObjectChoices.Count == 0)
            {
                
                Popup.Show("You have nothing with which to augment yourself.");
                
                return;
            }
            int choicenum = Popup.ShowOptionList(string.Empty, ChoiceList.ToArray(), HotkeyList.ToArray(), 0, "With what shall you augment yourself?", 60,  false, true);
            if (choicenum >= 0)
            {
                Augment(who,ObjectChoices[choicenum]);
            }
        }

		public void Augment(GameObject who, GameObject what){
			BodyPartType type = PartWas(what);
			if(type == null){
				Popup.Show("You don't know how to attach that.");
				return;
			}




			List<BodyPart> ObjectChoices = new List<BodyPart>();
            List<string> ChoiceList = new List<string>();
            List<char> HotkeyList = new List<char>();
            char ch = 'a';
			foreach (BodyPart item in who.GetPart<Body>().GetParts())
			{	
				if(!item.Abstract){
                    ObjectChoices.Add(item);
                    HotkeyList.Add(ch);
                    ChoiceList.Add("attach to "+item.Name);
                    ch = (char)(ch + 1);

                    ObjectChoices.Add(item);
                    HotkeyList.Add(ch);
                    ChoiceList.Add("attach beside "+item.Name);
                    ch = (char)(ch + 1);
				}
			}
          
            if (ObjectChoices.Count == 0)
            {
                
                Popup.Show("You have no body.");
                
                return;
            }
			BodyPart part = null;
            int choicenum = Popup.ShowOptionList(string.Empty, ChoiceList.ToArray(), HotkeyList.ToArray(), 0, "Where to attach "+what.the+what.DisplayNameOnly+"?", 60,  false, true);
            if (choicenum %2== 1)
            {
				part = who.GetPart<Body>().GetBody().AddPartAt(ObjectChoices[choicenum],type);
            }else{
				part = ObjectChoices[choicenum].AddPart(type);
			}
			if(part == null){
				return;
			}
			part.Description = "Augmented "+part.Description ;

			what.Destroy(true);
			Popup.Show("You augment yourself with a new "+type.Name+".");

		}

		public BodyPartType PartWas(GameObject dismembered){
			string name = dismembered.GetPart<Description>().Short;
			if(!name.Contains("severed ")){
				return null;
			}
			name = Regex.Replace(name,"^.*?severed ","",RegexOptions.Singleline);
			name = Regex.Replace(name,"\\..*","",RegexOptions.Singleline);
			name = Regex.Replace(name,"fore","");
			name = Regex.Replace(name,"hind","");
			name = Regex.Replace(name,"mid","");
			//name = name.Substring(0,name.Length-1);
			name = Grammar.MakeTitleCase(name);
			//IPart.AddPlayerMessage("partname:"+name);
			if(BodyPartType.Get(name) != null){
				//IPart.AddPlayerMessage("So it turns out this aint null:"+BodyPartType.Get(name).ToString());
				return BodyPartType.Get(name);
			}
			foreach(string smallname in name.Split(' ')){
				//IPart.AddPlayerMessage("partname:"+smallname);
				if(BodyPartType.Get(smallname) != null){
					return BodyPartType.Get(smallname);
				}
			}
			return null;

		}
	}
}
