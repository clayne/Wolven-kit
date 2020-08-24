using System.IO;
using System.Runtime.Serialization;
using WolvenKit.CR2W.Reflection;
using static WolvenKit.CR2W.Types.Enums;


namespace WolvenKit.CR2W.Types
{
	[DataContract(Namespace = "")]
	[REDMeta]
	public class CR4PlayerStateSailingPassive : CR4PlayerStateUseGenericVehicle
	{
		[RED("boatLogic")] 		public CHandle<CBoatComponent> BoatLogic { get; set;}

		[RED("dismountRequest")] 		public CBool DismountRequest { get; set;}

		[RED("vehicleCombatMgr")] 		public CHandle<W3VehicleCombatManager> VehicleCombatMgr { get; set;}

		[RED("rudderDamper")] 		public CFloat RudderDamper { get; set;}

		public CR4PlayerStateSailingPassive(CR2WFile cr2w, CVariable parent, string name) : base(cr2w, parent, name){ }

		public static new CVariable Create(CR2WFile cr2w, CVariable parent, string name) => new CR4PlayerStateSailingPassive(cr2w, parent, name);

		public override void Read(BinaryReader file, uint size) => base.Read(file, size);

		public override void Write(BinaryWriter file) => base.Write(file);

	}
}