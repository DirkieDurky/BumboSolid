namespace BumboSolid.HelperClasses
{
	public class OneBeforeOtherValidation
	{
		public bool Validate(Object start, Object end)
		{
			// Compare TimeOnly
			if (start is TimeOnly startTime && end is TimeOnly endTime)
			{
				if (startTime < endTime) return false;
				else return true;
			}

			return false;
		}
	}
}
