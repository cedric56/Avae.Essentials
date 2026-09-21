//#nullable enable
//using Microsoft.Maui.ApplicationModel;
//using Microsoft.Maui.Devices.Sensors;
//using System.Runtime.Versioning;
//using Windows.Devices.Geolocation;
//using PositionChangedEventArgs = Windows.Devices.Geolocation.PositionChangedEventArgs;

//namespace Avae.Essentials
//{
//	partial class GeolocationImplementation
//	{
//		public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;

//		public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;

//		internal void OnLocationChanged(Location location) =>
//			OnLocationChanged(new GeolocationLocationChangedEventArgs(location));

//		internal void OnLocationChanged(GeolocationLocationChangedEventArgs e) =>
//			LocationChanged?.Invoke(null, e);

//		internal void OnLocationError(GeolocationError geolocationError) =>
//			ListeningFailed?.Invoke(null, new GeolocationListeningFailedEventArgs(geolocationError));
//	}

//	[SupportedOSPlatform("windows10.0.10240")]
//	partial class GeolocationImplementation : IGeolocation
//	{
//		Geolocator? listeningGeolocator;

//		/// <summary>
//		/// Indicates if currently listening to location updates while the app is in foreground.
//		/// </summary>
//		public bool IsListeningForeground { get => listeningGeolocator != null; }

//		public bool IsEnabled => throw new NotImplementedException();

//		public async Task<Location?> GetLastKnownLocationAsync()
//		{
//			// no need for permissions as AllowFallbackToConsentlessPositions
//			// will allow the device to return a location regardless

//			var geolocator = new Geolocator
//			{
//				DesiredAccuracy = PositionAccuracy.Default,
//			};
//			geolocator.AllowFallbackToConsentlessPositions();

//			var location = await geolocator.GetGeopositionAsync().AsTask();

//			return location?.Coordinate?.ToLocation();
//		}

//		public async Task<Location?> GetLocationAsync(GeolocationRequest request, CancellationToken cancellationToken)
//		{
//			ArgumentNullException.ThrowIfNull(request);

//			await Permissions.EnsureGrantedAsync<Permissions.LocationWhenInUse>();

//			var geolocator = new Geolocator
//			{
//				DesiredAccuracyInMeters = request.PlatformDesiredAccuracy
//			};

//			CheckStatus(geolocator.LocationStatus);

//			cancellationToken = Utils.TimeoutToken(cancellationToken, request.Timeout);

//			var location = await geolocator.GetGeopositionAsync().AsTask(cancellationToken);

//			return location?.Coordinate?.ToLocation();
//		}

//		static void CheckStatus(PositionStatus status)
//		{
//			switch (status)
//			{
//				case PositionStatus.Disabled:
//				case PositionStatus.NotAvailable:
//					throw new FeatureNotEnabledException("Location services are not enabled on device.");
//			}
//		}

//		/// <summary>
//		/// Starts listening to location updates using the <see cref="Geolocation.LocationChanged"/>
//		/// event or the <see cref="Geolocation.ListeningFailed"/> event. Events may only sent when
//		/// the app is in the foreground. Requests <see cref="Permissions.LocationWhenInUse"/>
//		/// from the user.
//		/// </summary>
//		/// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
//		/// <exception cref="FeatureNotSupportedException">Thrown if listening is not supported on this platform.</exception>
//		/// <exception cref="InvalidOperationException">Thrown if already listening and <see cref="IsListeningForeground"/> returns <see langword="true"/>.</exception>
//		/// <param name="request">The listening request parameters to use.</param>
//		/// <returns><see langword="true"/> when listening was started, or <see langword="false"/> when listening couldn't be started.</returns>
//		public async Task<bool> StartListeningForegroundAsync(GeolocationListeningRequest request)
//		{
//			ArgumentNullException.ThrowIfNull(request);

//			if (request.MinimumTime.TotalMilliseconds < 0)
//				throw new ArgumentOutOfRangeException(nameof(request), "MinimumTime must be positive.");

//			if (IsListeningForeground)
//				throw new InvalidOperationException("Already listening to location updates.");

//			await Permissions.EnsureGrantedAsync<Permissions.LocationWhenInUse>();

//			listeningGeolocator = new Geolocator
//			{
//				DesiredAccuracyInMeters = request.PlatformDesiredAccuracy,
//				ReportInterval = (uint)request.MinimumTime.TotalMilliseconds,
//				MovementThreshold = request.PlatformDesiredAccuracy,
//			};

//			CheckStatus(listeningGeolocator.LocationStatus);

//			listeningGeolocator.PositionChanged += OnLocatorPositionChanged;
//			listeningGeolocator.StatusChanged += OnLocatorStatusChanged;

//			return true;
//		}

//		/// <summary>
//		/// Stop listening for location updates when the app is in the foreground.
//		/// Has no effect when not listening and <see cref="Geolocation.IsListeningForeground"/>
//		/// is currently <see langword="false"/>.
//		/// </summary>
//		public void StopListeningForeground()
//		{
//			if (!IsListeningForeground || listeningGeolocator == null)
//				return;

//			listeningGeolocator.PositionChanged -= OnLocatorPositionChanged;
//			listeningGeolocator.StatusChanged -= OnLocatorStatusChanged;

//			listeningGeolocator = null;
//		}

//		void OnLocatorPositionChanged(Geolocator sender, PositionChangedEventArgs e) =>
//			OnLocationChanged(e.Position.ToLocation());

//		void OnLocatorStatusChanged(Geolocator sender, StatusChangedEventArgs e)
//		{
//			if (!IsListeningForeground)
//				return;

//			StopListeningForeground();

//			GeolocationError error;
//			switch (e.Status)
//			{
//				case PositionStatus.Disabled:
//					error = GeolocationError.Unauthorized;
//					break;

//				case PositionStatus.NoData:
//					error = GeolocationError.PositionUnavailable;
//					break;

//				default:
//					return;
//			}

//			OnLocationError(error);
//		}

//		public Task<Location?> GetLocationAsync(Microsoft.Maui.Devices.Sensors.GeolocationRequest request, CancellationToken cancelToken)
//		{
//			throw new NotImplementedException();
//		}

//		public Task<bool> StartListeningForegroundAsync(Microsoft.Maui.Devices.Sensors.GeolocationListeningRequest request)
//		{
//			throw new NotImplementedException();
//		}
//	}
//}
