using System;
using XRL.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XRL.Language;



namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_DismemberSearcher : IPart
	{
		[NonSerialized]
		public BodyPart Part;

		[NonSerialized]
		public static List<GameObject> dismeberees = new List<GameObject>();

		public acegiak_DismemberSearcher()
		{
			base.Name = "acegiak_DismemberSearcher";
		}

		public override bool SameAs(IPart p)
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "EnteredCell");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "EnteredCell")
			{
				//IPart.AddPlayerMessage(ParentObject.DisplayName+" fell on the ground.");
                if(ParentObject.HasProperty("LimbSourceGameObjectID") && ParentObject.HasProperty("LimbSourceBodyPartID")){

					//IPart.AddPlayerMessage("it had properties!");
                    Cell cell = E.GetParameter("Cell") as Cell;
					GameObject parentO = GameObject.findById(ParentObject.GetStringProperty("LimbSourceGameObjectID"));
					if(parentO == null){
						//IPart.AddPlayerMessage("not in cell!");
					
						foreach(GameObject GO in dismeberees){
							//IPart.AddPlayerMessage("are you my mummy("+ParentObject.GetStringProperty("LimbSourceGameObjectID")+")?"+GO.DisplayName+":"+GO.id);

							if(GO.id == ParentObject.GetStringProperty("LimbSourceGameObjectID")){

								parentO = GO;
							}
						}
					}
					if(parentO != null){
						//IPart.AddPlayerMessage("it had a parent!");
						Body body = parentO.GetPart<Body>();
						if(body != null){
							//IPart.AddPlayerMessage("its parent had a body!");
							Part = DeepCopy(body.GetPartByID(ParentObject.GetIntProperty("LimbSourceBodyPartID"),true));
						}
					}
                }
				
			}

			return base.FireEvent(E);
		}

		
		public BodyPart DeepCopy(BodyPart original)
		{
			if(original == null){
				return null;
			}
			BodyPart bodyPart = new BodyPart(null);
			bodyPart.Type = original.Type;
			bodyPart.VariantType = original.VariantType;
			bodyPart.Description = original.Description;
			bodyPart.Name = original.Name;
			bodyPart.SupportsDependent = original.SupportsDependent;
			bodyPart.DependsOn = original.DependsOn;
			bodyPart.RequiresType = original.RequiresType;
			bodyPart.PreventRegenerationBecause = original.PreventRegenerationBecause;
			bodyPart.Category = original.Category;
			bodyPart.Laterality = original.Laterality;
			bodyPart.RequiresLaterality = original.RequiresLaterality;
			bodyPart.Mobility = original.Mobility;
			bodyPart.Primary = original.Primary;
			bodyPart.Native = original.Native;
			bodyPart.Integral = original.Integral;
			bodyPart.Mortal = original.Mortal;
			bodyPart.Abstract = original.Abstract;
			bodyPart.Extrinsic = original.Extrinsic;
			bodyPart.Plural = original.Plural;
			bodyPart.Position = original.Position;
			bodyPart._ID = original._ID;
			bodyPart.ParentBody = null;
			if (original.DefaultBehavior != null)
			{
				GameObject gameObject = original.DefaultBehavior.DeepCopy();
				if(ParentObject.DeepCopyInventoryObjectMap == null){
					ParentObject.DeepCopyInventoryObjectMap = new Dictionary<GameObject, GameObject>();
				}
				ParentObject.DeepCopyInventoryObjectMap.Add(original.DefaultBehavior, gameObject);
				gameObject.pPhysics._Equipped = ParentObject;
				bodyPart.DefaultBehavior = gameObject;
			}
			
			if (original.Parts != null)
			{
				foreach (BodyPart part in original.Parts)
				{
					if (!part.Extrinsic)
					{
						bodyPart.AddPart(DeepCopy(part));
					}
				}
				return bodyPart;
			}
			return bodyPart;
		}


		public override void SaveData(SerializationWriter Writer)
		{
			if(Part != null){
				Writer.Write(true);
				Part.Save(Writer);
			}else{
				Writer.Write(false);
			}
			base.SaveData(Writer);
		}

		public override void LoadData(SerializationReader Reader)
		{
			if(Reader.ReadBoolean()){
				Part = BodyPart.Load(Reader, null);
			}
			base.LoadData(Reader);
		}

		

    }


}