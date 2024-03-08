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
	}

	/// <summary>
	/// Generic component for holding the value of a stat on a given entity. <br></br>
	/// Instances should only be created through <see cref="StatContainer"/>.
	/// </summary>
	/// <typeparam name="TValue"> The type of value wrapped by the stat and the instance. </typeparam>
	public abstract class StatInstance<TValue> : StatInstance {
		private TValue value;

		[SerializeField] private Stat<TValue> stat;
		[SerializeField] private TValue defaultValue;

		/// <summary>
		/// A reference to the <see cref="Stat{T}"/> that the instance references.
		/// </summary>
		public Stat<TValue> Stat => stat;
		/// <summary>
		/// Gets or sets the current value of the instance.
		/// </summary>
		public TValue Value {
			get {
				return value;
			}
			set {
				this.value = value;
				if (Stat != null) {
					Stat.ProcessValueChange (this, ref this.value);
				}
				ProcessValueChange (ref this.value);
			}
		}

		/// <inheritdoc/>
		public sealed override bool IsValid => Stat != null;


		/// <inheritdoc/>
		public sealed override Stat GetStat () => Stat;
		/// <inheritdoc/>
		public sealed override object GetValue () => Value;

		/// <inheritdoc/>
		protected virtual void ProcessValueChange (ref TValue value) { }

		/// <summary>
		/// Implicitly converts a stat instance to the value it wraps.
		/// </summary>
		public static implicit operator TValue (StatInstance<TValue> instance) {
			return instance.Value;
		}
	}
}
