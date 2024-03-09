using UnityEngine;

namespace Zenvin.Stats {
	public class ClampedIntStatInstance : IntStatInstance {
		[SerializeField] private bool clampMin = true;
		[SerializeField] private int minValue = 0;
		[SerializeField] private bool clampMax = true;
		[SerializeField] private int maxValue = 100;

		protected override void ProcessValueChange (ref int value) {
			if (clampMin && value < minValue)
				value = minValue;
			if (clampMax && value > maxValue)
				value = maxValue;
		}

		public override string ToString () {
			var value = $"Default: {DefaultValue}";
			if (clampMin)
				value += $", Min: {minValue}";
			if (clampMax)
				value += $", Max: {maxValue}";

			return value;
		}
	}
}