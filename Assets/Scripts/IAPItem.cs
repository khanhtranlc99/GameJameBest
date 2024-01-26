public class IAPItem
{
	public enum IAPItemType
	{
		Consumable,
		Durable
	}

	public string Id
	{
		get;
		set;
	}

	public string FormattedPrice
	{
		get;
		set;
	}

	public IAPController.BuyCallback Callback
	{
		get;
		set;
	}

	public IAPItemType IapItemType
	{
		get;
		set;
	}

	public IAPItem(string id)
		: this(id, IAPItemType.Consumable)
	{
	}

	public IAPItem(string id, IAPItemType type)
	{
		IapItemType = type;
		Id = id;
		Callback = delegate
		{
		};
	}
}
