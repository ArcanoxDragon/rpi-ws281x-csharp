namespace rpi_ws281x
{
	/// <summary>
	/// Represents the channel which holds the LEDs
	/// </summary>
	public class Channel
	{
		public Channel() : this(0, 0) { }

		public Channel(int ledCount, int gpioPin, byte brightness = 255, bool invert = false, StripType stripType = StripType.Unknown)
		{
			GPIOPin = gpioPin;
			Invert = invert;
			Brightness = brightness;
			StripType = stripType;
			LEDs = new LED[ledCount];

			for (int i = 0; i <= ledCount - 1; i++)
				LEDs[i] = new LED(i);
		}

		/// <summary>
		/// Returns the GPIO pin which is connected to the LED strip
		/// </summary>
		public int GPIOPin { get; private set; }

		/// <summary>
		/// Returns a value which indicates if the signal needs to be inverted.
		/// Set to true to invert the signal (when using NPN transistor level shift).
		/// </summary>
		public bool Invert { get; private set; }

		/// <summary>
		/// Gets or sets the brightness of the LEDs
		/// 0 = darkes, 255 = brightest
		/// </summary>
		public byte Brightness { get; set; }

		/// <summary>
		/// Returns the type of the channel.
		/// The type defines the ordering of the colors.
		/// </summary>
		public StripType StripType { get; private set; }

		/// <summary>
		/// Returns all LEDs on this channel
		/// </summary>
		public LED[] LEDs { get; }

		public int LEDCount => LEDs.Length;
	}
}