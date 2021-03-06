using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class questEntityManagerSetAttachment_ToActor : questIEntityManagerSetAttachment_NodeSubType
	{
		private NodeRef _attachmentRef;
		private gameEntityReference _objectRef;
		private CBool _isPlayer;
		private CName _slot;
		private CEnum<questAttachmentOffsetMode> _offsetMode;
		private Vector3 _customOffsetPos;
		private Quaternion _customOffsetRot;

		[Ordinal(0)] 
		[RED("attachmentRef")] 
		public NodeRef AttachmentRef
		{
			get => GetProperty(ref _attachmentRef);
			set => SetProperty(ref _attachmentRef, value);
		}

		[Ordinal(1)] 
		[RED("objectRef")] 
		public gameEntityReference ObjectRef
		{
			get => GetProperty(ref _objectRef);
			set => SetProperty(ref _objectRef, value);
		}

		[Ordinal(2)] 
		[RED("isPlayer")] 
		public CBool IsPlayer
		{
			get => GetProperty(ref _isPlayer);
			set => SetProperty(ref _isPlayer, value);
		}

		[Ordinal(3)] 
		[RED("slot")] 
		public CName Slot
		{
			get => GetProperty(ref _slot);
			set => SetProperty(ref _slot, value);
		}

		[Ordinal(4)] 
		[RED("offsetMode")] 
		public CEnum<questAttachmentOffsetMode> OffsetMode
		{
			get => GetProperty(ref _offsetMode);
			set => SetProperty(ref _offsetMode, value);
		}

		[Ordinal(5)] 
		[RED("customOffsetPos")] 
		public Vector3 CustomOffsetPos
		{
			get => GetProperty(ref _customOffsetPos);
			set => SetProperty(ref _customOffsetPos, value);
		}

		[Ordinal(6)] 
		[RED("customOffsetRot")] 
		public Quaternion CustomOffsetRot
		{
			get => GetProperty(ref _customOffsetRot);
			set => SetProperty(ref _customOffsetRot, value);
		}

		public questEntityManagerSetAttachment_ToActor(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
