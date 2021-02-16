using CP77.CR2W.Reflection;
using FastMember;
using static CP77.CR2W.Types.Enums;

namespace CP77.CR2W.Types
{
	[REDMeta]
	public class AIbehaviorFindNavigablePointTaskDefinition : AIbehaviorTaskDefinition
	{
		[Ordinal(1)] [RED("destination")] public CHandle<AIArgumentMapping> Destination { get; set; }
		[Ordinal(2)] [RED("outAdjustedDestination")] public CHandle<AIArgumentMapping> OutAdjustedDestination { get; set; }
		[Ordinal(3)] [RED("outWasAdjusted")] public CHandle<AIArgumentMapping> OutWasAdjusted { get; set; }

		public AIbehaviorFindNavigablePointTaskDefinition(CR2WFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
