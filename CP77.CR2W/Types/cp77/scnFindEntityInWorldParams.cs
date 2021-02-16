using CP77.CR2W.Reflection;
using FastMember;
using static CP77.CR2W.Types.Enums;

namespace CP77.CR2W.Types
{
	[REDMeta]
	public class scnFindEntityInWorldParams : CVariable
	{
		[Ordinal(0)] [RED("actorRef")] public gameEntityReference ActorRef { get; set; }
		[Ordinal(1)] [RED("forceMaxVisibility")] public CBool ForceMaxVisibility { get; set; }

		public scnFindEntityInWorldParams(CR2WFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
