using System.Collections.Generic;

namespace Game.GlobalComponent.Qwest
{
	public class QwestProfile
	{
		public static Dictionary<string, bool> QwestStatus
		{
			get
			{
				return BaseProfile.ResolveValue(QwestProfileList.QwestStatus.ToString(), new Dictionary<string, bool>());
			}
			set
			{
				BaseProfile.StoreValue(value, QwestProfileList.QwestStatus.ToString());
			}
		}

		public static bool QwestArrow
		{
			get
			{
				return BaseProfile.ResolveValue(QwestProfileList.QwestArrow.ToString(), defaultValue: true);
			}
			set
			{
				BaseProfile.StoreValue(value, QwestProfileList.QwestArrow.ToString());
			}
		}
	}
}
