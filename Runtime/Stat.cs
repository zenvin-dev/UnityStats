using UnityEngine;

namespace Zenvin.Stats {
	/// <summary>
	/// Non-generic base class representing a stat.
	/// </summary>
	public abstract class Stat : ScriptableObject {
		[SerializeField] private string identifier;
		[SerializeField] private Color accentColor;

		/// <summary>
		/// An identifier for the stat. May be used to look up instances through
		/// <see cref="StatContainer.Get{TStatInstance}(string)"/> or <see cref="StatContainer.TryGet{TStatInstance}(string, out TStatInstance)"/>
		/// </summary>
		public string Identifier { get => identifier; private set => identifier = value; }
		internal Color AccentColor { get => accentColor; private set => accentColor = value; }
	}

	/// <summary>
	/// Generic object representing a stat.
	/// </summary>
	/// <typeparam name="TValue"> The type of value represented by the stat. </typeparam>
	public abstract class Stat<TValue> : Stat {
		/// <summary>
		/// Can be overridden to process a stat's value being changed.
		/// </summary>
		/// <param name="value"> The value of the stat. </param>
		/// <param name="instance"> The <see cref="StatInstance{TValue}"/> holding the value. </param>
		internal protected virtual void ProcessValueChange (StatInstance<TValue> instance, ref TValue value) { }
	}
}
