using System.ComponentModel;

namespace Data.Enums
{
    public enum BookStatus : byte
    {
        None,
        /// <summary>
        /// Буду читать
        /// </summary>
        [Description("Буду читать")]
        WillRead,
        /// <summary>
        /// Читаю
        /// </summary>
        [Description("Читаю")]
        Reading,
        /// <summary>
        /// Прочитано
        /// </summary>
        [Description("Прочитано")]
        Read,
        /// <summary>
        /// Брошено
        /// </summary>
        [Description("Брошено")]
        Dropped,
    }
}
