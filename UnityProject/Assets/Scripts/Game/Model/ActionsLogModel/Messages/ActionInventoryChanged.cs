namespace Game.Models
{
	using CodeStage.AntiCheat.ObscuredTypes;
	using UltimateSurvival;

	public class ActionInventoryChanged
	{
		public enum ChangeType
		{
			None = 0,
			Gathered,
			Crafted,
			Dropped,
		}

		public ActionInventoryChanged(ItemData iItemData, int iValue, ChangeType iChangeType, bool iExaggerateIndication = false)
		{
			exaggerateIndication = iExaggerateIndication;
			value = iValue;
			itemData = iItemData;
			changeType = iChangeType;
		}

		public ActionInventoryChanged(SavableItem iItem, ChangeType iChangeType, bool iExaggerateIndication = false) : this(iItem.ItemData, iItem.Count, iChangeType, iExaggerateIndication)
		{	
		}

		// Note: Old implementation represented ID's for localized strings.
		// There was an assumption of exact data Views would use to represent messages, as well as forcing user to define that data inside unrelated classes.
		public int value { get; }

		public ItemData itemData { get; }

		public bool exaggerateIndication { get; }

		public ChangeType changeType { get; }
	}
}