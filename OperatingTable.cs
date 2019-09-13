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
            
			if(what.GetPart<acegiak_DismemberSearcher>() != null && what.GetPart<acegiak_DismemberSearcher>().Part != null){
				recurAdd(ObjectChoices[choicenum],what.GetPart<acegiak_DismemberSearcher>().Part, choicenum %2== 1);

				//RegenerateLimb(what,what.GetPart<acegiak_DismemberSearcher>().DismemberedParts[0],ObjectChoices[choicenum],choicenum %2== 1);
			}
			// if (choicenum %2== 1)
            // {
			// 	if(what.GetPart<acegiak_DismemberSearcher>() != null && what.GetPart<acegiak_DismemberSearcher>().Part != null){
			// 		part = who.GetPart<Body>().GetBody().AddPart(what.GetPart<acegiak_DismemberSearcher>().Part,ObjectChoices[choicenum].Position+1);
			// 	}else{
			// 		part = who.GetPart<Body>().GetBody().AddPartAt(ObjectChoices[choicenum],type);
			// 	}
            // }else{
			// 	if(what.GetPart<acegiak_DismemberSearcher>() != null && what.GetPart<acegiak_DismemberSearcher>().Part != null){
			// 		part = recurAdd(what.GetPart<acegiak_DismemberSearcher>().Part, ObjectChoices[choicenum]);
			// 	}else{
			// 		part = ObjectChoices[choicenum].AddPart(type);
			// 	}
				
			// }
			// if(part == null){
			// 	return;
			// }
			// part.Description = "Augmented "+part.Description ;

			what.Destroy(true);
			Popup.Show("You augment yourself with a new "+type.Name+".");

		}

		public void recurAdd(BodyPart destination, BodyPart add, bool after = false){
			if(add == null){
				return;
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
			}catch(Exception e){

			}
			if(add.Parts != null){
				foreach(BodyPart p in add.Parts){
					recurAdd(add,p,false);
				}
			}

		}

		// public void RegenerateLimb(GameObject what,Body.DismemberedPart Part,BodyPart destination,bool after = false)
		// {
		// 	IPart.AddPlayerMessage("PARTT:"+Part.Part.Name);
		// 	BodyPart part = null;
		// 	if(after){
		// 		part = destination.ParentBody.GetBody().AddPart(Part.Part,destination.Position+1);
		// 	}else{
		// 		part = destination.AddPart(Part.Part);
		// 	}

		// 	//Part.Reattach(this);
		// 	what.GetPart<acegiak_DismemberSearcher>().DismemberedParts.Remove(Part);
		// 	if (ParentObject.IsPlayer())
		// 	{
		// 		// IPart.AddPlayerMessage("&GYou regenerate your " + Part.Part.GetOrdinalName() + "&G!");
		// 		// AchievementManager.SetAchievement("ACH_REGENERATE_LIMB");
		// 	}
		// 	else if (Visible())
		// 	{
		// 		//MessageQueue.AddPlayerMessage("&R" + ParentObject.The + ParentObject.DisplayName + ParentObject.GetVerb("regenerate") + " " + ParentObject.its + " " + Part.Part.GetOrdinalName() + "&R!");
		// 	}
		// 	if (Part.Part.HasID())
		// 	{
		// 		Body.DismemberedPart dismemberedPart = null;
		// 		while ((dismemberedPart = what.GetPart<acegiak_DismemberSearcher>().FindRegenerablePart(Part.Part.ID)) != null)
		// 		{
		// 			//RegenerateLimb(what,dismemberedPart,part);
		// 		}
		// 	}
		// 	// if (DoUpdate)
		// 	// {
		// 	// 	UpdateBodyParts();
		// 	// }
		// }




		// public string recurAdd(BodyPart destination, string partdescription, bool after = false){
		// 	IPart.AddPlayerMessage("processing: "+partdescription);
		// 	if(partdescription.Substring(0,1) == "}"){
		// 		return partdescription.Substring(1);
		// 	}

		// 	string PartType = partdescription.Substring(1,partdescription.IndexOf('|')-1);

		// 	string PartName = partdescription.Substring(partdescription.IndexOf('|')+1,partdescription.IndexOf('{')-(partdescription.IndexOf('|')+1));
		// 	IPart.AddPlayerMessage("Part Type:"+PartType);
		// 	IPart.AddPlayerMessage("Part Name:"+PartName);

		// 	BodyPart part = null;






		// 	//    @hand|sucker{@finger|finger{}}@hand|sucker2{}}

		// 	while(partdescription.Substring(0,1) != "}"){
		// 		partdescription = partdescription.Substring(partdescription.IndexOf("{")+1);
		// 		partdescription = recurAdd(part,partdescription,false);
		// 	}
		// 	return partdescription;
		// }

		public BodyPartType PartWas(GameObject dismembered){

			// acegiak_DismemberSearcher searcher = dismembered.GetPart<acegiak_DismemberSearcher>();
			// if(searcher != null && searcher.SavedParts != null){
			// 	string PartType = searcher.SavedParts.Substring(1,searcher.SavedParts.IndexOf('|')-1);

			// 	IPart.AddPlayerMessage("A Fancy part:"+PartType);
			// 	return BodyPartType.Get(PartType);
			// }

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
