using UnityEngine;

namespace Zenvin.Stats {
	/// <summary>
	/// Non-generic base class for a stat instance. <br></br>
	/// See <see cref="StatInstance{T}"/>.
	/// </summary>
	public abstract class StatInstance : MonoBehaviour {
		/// <summary>
		/// Returns whether the stat instance is valid. This usually just means that a <see cref="Stat{T}"/> is assigned to it.
		/// </summary>
		public abstract bool IsValid { get; }

		/// <summary>
		/// If implemented, returns the <see cref="Stat"/> object assigned to a <see cref="StatInstance{T}"/>.
		/// </summary>
		public abstract Stat GetStat ();
		/// <summary>
		/// If implemented, returns the value of a <see cref="StatInstance{T}"/>.
		/// </summary>
		public abstract object GetValue ();

		internal abstract void SetStat (Stat stat);
	}

	/// <summary>
	/// Generic component for holding the value of a stat on a given entity. <br></br>
	/// Instances should only be created through <see cref="StatContainer"/>.
	/// </summary>
	/// <typeparam name="T"> The type of value wrapped by the stat and the instance. </typeparam>
	public abstract class StatInstance<T> : StatInstance {
		private bool valueSet = false;
		private T value;

		[SerializeField, HideInInspector] private Stat<T> stat;
		[SerializeField] private T defaultValue;

		/// <summary>
		/// A reference to the <see cref="Stat{T}"/> that the instance references.
		/// </summary>
		public Stat<T> Stat => stat;
		/// <summary>
		/// Gets or sets the current value of the instance.
		/// </summary>
		public T Value {
			get {
				return valueSet ? value : defaultValue;
			}
			set {
				this.value = value;
				valueSet = true;
				if (Stat != null) {
					Stat.ProcessValueChange (this, ref this.value);
				}
				ProcessValueChange (ref this.value);
			}
		}
		/// <summary>
		/// The default value of the instance. <br></br>
		/// Will be returned by <see cref="Value"/>, until that has been set at least once.
		/// </summary>
		public T DefaultValue => defaultValue;

		/// <inheritdoc/>
		public sealed override bool IsValid => Stat != null;


		/// <inheritdoc/>
		public sealed override Stat GetStat () => Stat;
		/// <inheritdoc/>
		public sealed override object GetValue () => Value;
		/// <inheritdoc/>
		public override string ToString () => $"Default: {defaultValue}";

		/// <inheritdoc/>
		protected virtual void ProcessValueChange (ref T value) { }


		internal sealed override void SetStat (Stat stat) {
			this.stat = stat as Stat<T>;
		}


		/// <summary>
		/// Implicitly converts a stat instance to the value it wraps.
		/// </summary>
		public static implicit operator T (StatInstance<T> instance) {
			return instance.Value;
		}
	}
}
