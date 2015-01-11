using SurvivalKit.Interfaces;
using System;

namespace SurvivalKit.Events.Misc
{
	/// <summary>
	/// Fired when a torch/light flicker Update() method is called.
	/// </summary>
	public class TorchFlickerUpdateEvent : Event, ICancellable
	{
		private object flickerclass;
		private bool cancelled;
		private float intensity;
		private float maxIntensity;
		private float minIntensity;

		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Misc.TorchFlickerUpdateEvent"/> class.
		/// </summary>
		/// <param name="args">
		/// An object array of data to pass to the event.
		/// args[0] (object) reserved
		/// args[1] (bool) indicates whether the event is cancelled
		/// args[2] (float) the current light intensity
		/// args[3] (float) the minimum light intensity
		/// args[4] (float) the maximum light intensity
		/// </param>
		public TorchFlickerUpdateEvent(Object[] args)
		{
			if (args == null || args.Length < 5)
				throw new ArgumentNullException();
			flickerclass = args[0];
			cancelled = (bool)args[1];
			intensity = (float)args[2];
			maxIntensity = (float)args[3];
			minIntensity = (float)args[4];
		}

		/// <summary>
		/// An internally used function to get the name of this event.
		/// </summary>
		/// <returns>
		/// Returns the name of the event used for EventManager.fireEvent(String,Object[]).
		/// </returns>
		public static string getName()
		{
			return "TorchFlickerUpdate";
		}
		/// <summary>
		/// Gets parameters used after firing an event.
		/// </summary>
		/// <returns>
		/// Returns an object array of parameters to pass to the caller of fireEvent.
		/// </returns>
		public override object[] getReturnParams ()
		{
			return new object[]{ this.cancelled, this.intensity, this.maxIntensity, this.minIntensity };
		}
		/// <summary>
		/// Gets whether this event supports clients.
		/// </summary>
		/// <returns><c>true</c>, if clients are supported, <c>false</c> otherwise.</returns>
		public override bool supportsClient ()
		{
			return true;
		}

		/// <summary>
		/// Gets or sets whether this event is cancelled.
		/// </summary>
		/// <value><c>true</c> if this instance cancelled, <c>false</c> otherwise.</value>
		public bool IsCancelled {
			get { return this.cancelled; }
			set { this.cancelled = value; }
		}
		/// <summary>
		/// Gets or sets the current light intensity.
		/// </summary>
		public float Intensity {
			get { return this.intensity; }
			set {
				if (value <= MaxIntensity && value >= MinIntensity)
					this.intensity = value;
			}
		}
		/// <summary>
		/// Gets the minimum light intensity.
		/// </summary>
		public float MaxIntensity {
			get { return this.maxIntensity; }
		}
		/// <summary>
		/// Gets the maximum light intensity.
		/// </summary>
		public float MinIntensity {
			get { return this.minIntensity; }
		}
	}
}

