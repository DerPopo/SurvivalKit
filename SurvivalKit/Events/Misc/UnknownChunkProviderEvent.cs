using SurvivalKit.Abstracts;
using SurvivalKit.Interfaces;
using System;

namespace SurvivalKit.Events.Misc
{
	/// <summary>
	/// Fired when an unknown chunk provider was detected. Can be used to implement custom chunk providers
	/// </summary>
	public class UnknownChunkProviderEvent : CancellableBaseEvent
	{
		private ChunkCluster cluster;
		private IChunkProvider chunkProvider;
		private int chunkProviderId;
		private bool cancelled;

		/// <summary>
		/// Initializes a new instance of the <see cref="SurvivalKit.Events.Misc.UnknownChunkProviderEvent"/> class.
		/// </summary>
		/// <param name="args">
		/// An object array of data to pass to the event.
		/// args[0] (ChunkCluster) the ChunkCluster that fired the event.
		/// args[1] (bool) indicates whether the event is cancelled (false by default).
		/// args[2] (int) the unknown chunkProviderId.
		/// </param>
		public UnknownChunkProviderEvent(Object[] args)
		{
			if (args == null || args.Length < 3)
				throw new ArgumentNullException();
			cluster = (ChunkCluster)args[0];
			cancelled = (bool)args[1];
			chunkProviderId = (int)args[2];
			chunkProvider = null;
		}

		/// <summary>
		/// An internally used function to get the name of this event.
		/// </summary>
		/// <returns>
		/// Returns the name of the event used for EventManager.fireEvent(String,Object[]).
		/// </returns>
		public static string getName()
		{
			return "OnUnknownChunkProvider";
		}

		/// <summary>
		/// Gets parameters used after firing an event.
		/// </summary>
		/// <returns>
		/// Returns an object array of parameters to pass to the caller of fireEvent.
		/// </returns>
		public override object[] getReturnParams ()
		{
			return new object[]{ this.cancelled, this.chunkProvider };
		}

		/// <summary>
		/// Gets or sets whether this event is cancelled.
		/// </summary>
		/// <value><c>true</c> if this instance cancelled, <c>false</c> otherwise.</value>
		public override bool IsCancelled {
			get { return this.cancelled; }
			set { this.cancelled = value; }
		}

		/// <summary>
		/// Gets the ChunkCluster that fired this event.
		/// </summary>
		/// <value>The ChunkCluster.</value>
		public ChunkCluster ChunkCluster {
			get { return this.cluster; }
		}

		/// <summary>
		/// Gets the unknown ChunkProvider id.
		/// </summary>
		/// <value>The id.</value>
		public int ChunkProviderId {
			get { return this.chunkProviderId; }
		}

		/// <summary>
		/// Gets or sets the chunk provider (null by default).
		/// If a non-null chunk provider is given, the event will be cancelled so the chunk provider won't be replaced by the game.
		/// </summary>
		/// <value>The chunk provider.</value>
		public IChunkProvider ChunkProvider {
			get { return this.chunkProvider; }
			set {
				this.chunkProvider = value; 
				if (value != null)
					IsCancelled = true;
			}
		}
	}
}

