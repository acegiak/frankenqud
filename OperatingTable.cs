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
			Object.RegisterPartEvent(this, "CanSmartUse");
			Object.RegisterPartEvent(this, "CommandSmartUse");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "CanSmartUse")
			{
					return false;
				
			}
			else if (E.ID == "CommandSmartUse")
			{
					Operate(E.GetGameObjectParameter("User"));
				
			}
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
                if(GO != null &&(GO.GetPart<acegiak_DismemberSearcher>() != null || PartWas(GO) != null)){
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
			BodyPartType type = null;
			if(what.GetPart<acegiak_DismemberSearcher>().Part != null){
				type = Anatomies.GetBodyPartType(what.GetPart<acegiak_DismemberSearcher>().Part.Type);
			}else{
				type = PartWas(what);
			}

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
            
			if(what.GetPart<acegiak_DismemberSearcher>() != null && what.GetPart<acegiak_DismemberSearcher>().Part != null){
				if(!recurAdd(ObjectChoices[choicenum],what.GetPart<acegiak_DismemberSearcher>().Part,who, choicenum %2== 1)){
					Popup.Show("Something went awry.");

					return;
				}
			}else{
				Popup.Show("it had no dismemberedpart.");

			}
			

			what.Destroy(true);
			who.GetPart<Body>().UpdateBodyParts();
			who.GetPart<Body>().RecalculateArmor();
			Popup.Show("You augment yourself with a new "+type.Name+".");
		}

		public bool recurAdd(BodyPart destination, BodyPart add, GameObject who, bool after = false){
			if(add == null){
				return true;
			}
			

			add.Description = "Augmented "+Grammar.MakeTitleCase(add.Name);
			add.Description = Regex.Replace(add.Description,"Fore","");
			add.Description = Regex.Replace(add.Description,"Hind","");
			add.Description = Regex.Replace(add.Description,"Mid","");
			add.Description = Regex.Replace(add.Description,"Right *","");
			add.Description = Regex.Replace(add.Description,"Left *","");
			add.Description = Grammar.MakeTitleCase(add.Description);


			add.ParentBody = destination.ParentBody;


			try{
				BodyPart part = null;
				if(after){
					part = destination.ParentBody.GetBody().AddPart(add,destination.Position+1);
				}else{
					part = destination.AddPart(add);
				}



				int saveroll = XRL.Rules.Stat.RollSave(who,"Intelligence",22);
				string[] stats = new string[]{"Intelligence","Willpower","Ego","Agility","Toughness","Strength"};
				if(part.Type=="Face" || part.Type=="Head"){
					stats = new string[]{"Intelligence","Willpower","Ego"};
				}else{
					stats = new string[]{"Strength","Toughness","Agility"};
				}
				string thestat = stats[XRL.Rules.Stat.Rnd2.Next(stats.Length)];
				if (saveroll < 5){
					who.Statistics[thestat].BaseValue--;
					Popup.Show("Your "+thestat+" is damaged by the augmentation process.");
				}else if(saveroll < 15){
					part.Equip(GameObject.create("Scarring"));
					Popup.Show("Your new "+part.Name+" becomes horribly scarred by the augmentation process.");
				}else if(saveroll > 20){
					who.Statistics[thestat].BaseValue++;
					Popup.Show("Your "+thestat+" is enhanced by your new "+part.Name+".");
				}
			}catch(Exception e){
				IPart.AddPlayerMessage(e.Message);
				return false;
			}

			
			if(add.Parts != null){
				foreach(BodyPart p in add.Parts){
					if(!recurAdd(add,p,who,false)){
						return false;
					};
				}
			}
			return true;

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
			name = Grammar.MakeTitleCase(name);
			if(Anatomies.GetBodyPartType(name) != null){
				return Anatomies.GetBodyPartType(name);
			}
			foreach(string smallname in name.Split(' ')){
				if(Anatomies.GetBodyPartType(smallname) != null){
					return Anatomies.GetBodyPartType(smallname);
				}
			}
			return null;

		}
	}
}
