using BeauData;

namespace ProtoAqua
{
    public class ServiceIds
    {
        // prevent instantiation
        private ServiceIds() { }

        #region Shared Interface

        /// <summary>
        /// Registers an id as a service id.
        /// </summary>
        static public FourCC Register(string inId, string inName = null, string inDescription = null)
        {
            return FourCC.Register(typeof(IService), inId, inName, inDescription);
        }

        #endregion // Shared Interface

        static public readonly FourCC Audio = Register("AUD", "Audio system");
        static public readonly FourCC Data = Register("DATA", "Data system");
        static public readonly FourCC Effects = Register("EFX", "Effects system");
        static public readonly FourCC Network = Register("NET", "Network system");
        static public readonly FourCC State = Register("STAT", "State system");
        static public readonly FourCC Tweaks = Register("TWCK", "Tweak system");
    }
}