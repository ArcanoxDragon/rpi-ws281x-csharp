using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Native;

namespace rpi_ws281x
{
	/// <summary>
	/// Wrapper class to controll WS281x LEDs
	/// </summary>
	public class WS281x : IDisposable
	{
		private readonly int[] dataBuffer;

		private ws2811_t _ws2811;
		private GCHandle _ws2811Handle;
		private bool     _isDisposingAllowed;

		/// <summary>
		/// Initialize the wrapper
		/// </summary>
		/// <param name="settings">Settings used for initialization</param>
		public WS281x(Settings settings)
		{
			this._ws2811 = new ws2811_t();
			//Pin the object in memory. Otherwies GC will probably move the object to another memory location.
			//This would cause errors because the native library has a pointer on the memory location of the object.
			this._ws2811Handle = GCHandle.Alloc(this._ws2811, GCHandleType.Pinned);

			this._ws2811.dmanum = settings.DMAChannel;
			this._ws2811.freq = settings.Frequency;

			var maxChannelLength = 0;

			for (int i = 0; i < PInvoke.RPI_PWM_CHANNELS; i++)
			{
				if (settings.Channels[i] == null)
					continue;

				InitChannel(i, settings.Channels[i]);
				maxChannelLength = Math.Max(maxChannelLength, settings.Channels[i].LEDCount);
			}

			this.dataBuffer = new int[maxChannelLength];

			Settings = settings;

			var initResult = PInvoke.ws2811_init(ref this._ws2811);
			if (initResult != ws2811_return_t.WS2811_SUCCESS)
			{
				var returnMessage = GetMessageForStatusCode(initResult);
				throw new Exception($"Error while initializing.\nError code: {initResult}\nMessage: {returnMessage}");
			}

			//Disposing is only allowed if the init was successfull.
			//Otherwise the native cleanup function throws an error.
			this._isDisposingAllowed = true;
		}

		/// <summary>
		/// Returns the settings which are used to initialize the component
		/// </summary>
		public Settings Settings { get; }

		/// <summary>
		/// Renders the content of the channels
		/// </summary>
		public void Render()
		{
			for (int i = 0; i <= Settings.Channels.Length - 1; i++)
			{
				var channel = Settings.Channels[i];

				if (channel == null)
					continue;

				for (int j = 0; j < channel.LEDCount; j++)
					this.dataBuffer[j] = channel.LEDs[j].RGBValue;

				Marshal.Copy(this.dataBuffer, 0, this._ws2811.channels[i].leds, channel.LEDCount);
			}

			var result = PInvoke.ws2811_render(ref this._ws2811);
			if (result != ws2811_return_t.WS2811_SUCCESS)
			{
				var returnMessage = GetMessageForStatusCode(result);
				throw new Exception(string.Format("Error while rendering.{0}Error code: {1}{0}Message: {2}", Environment.NewLine, result.ToString(), returnMessage));
			}
		}

		/// <summary>
		/// Sets the color of a given LED
		/// </summary>
		/// <param name="channelIndex">Channel which controls the LED</param>
		/// <param name="ledID">ID/Index of the LED</param>
		/// <param name="color">New color</param>
		public void SetLEDColor(int channelIndex, int ledID, Color color)
		{
			Settings.Channels[channelIndex].LEDs[ledID].Color = color;
		}

		/// <summary>
		/// Initialize the channel propierties
		/// </summary>
		/// <param name="channelIndex">Index of the channel tu initialize</param>
		/// <param name="channelSettings">Settings for the channel</param>
		private void InitChannel(int channelIndex, Channel channelSettings)
		{
			this._ws2811.channels[channelIndex].count = channelSettings.LEDs.Length;
			this._ws2811.channels[channelIndex].gpionum = channelSettings.GPIOPin;
			this._ws2811.channels[channelIndex].brightness = channelSettings.Brightness;
			this._ws2811.channels[channelIndex].invert = Convert.ToInt32(channelSettings.Invert);

			if (channelSettings.StripType != StripType.Unknown)
			{
				//Strip type is set by the native assembly if not explicitly set.
				//This type defines the ordering of the colors e. g. RGB or GRB, ...
				this._ws2811.channels[channelIndex].strip_type = (int) channelSettings.StripType;
			}
		}

		/// <summary>
		/// Returns the error message for the given status code
		/// </summary>
		/// <param name="statusCode">Status code to resolve</param>
		/// <returns></returns>
		private string GetMessageForStatusCode(ws2811_return_t statusCode)
		{
			var strPointer = PInvoke.ws2811_get_return_t_str((int) statusCode);
			return Marshal.PtrToStringAuto(strPointer);
		}

		#region IDisposable Support

		private bool disposedValue; // To detect redundant calls

		// ReSharper disable once UnusedParameter.Global
		protected virtual void Dispose(bool disposing)
		{
			if (this.disposedValue)
				return;

			if (this._isDisposingAllowed)
			{
				PInvoke.ws2811_fini(ref this._ws2811);
				this._ws2811Handle.Free();

				this._isDisposingAllowed = false;
			}

			this.disposedValue = true;
		}

		~WS281x() => Dispose(false);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}