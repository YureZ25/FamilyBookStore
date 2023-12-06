using System.ComponentModel;

namespace Data.Enums
{
    public enum BookStatus
    {
        None,
        [Description("Буду читать")]
        WillRead,
        [Description("Читаю")]
        Reading,
        [Description("Прочитано")]
        Read,
        [Description("Брошено")]
        Dropped,
    }
}
