namespace MIDE.API.Services
{
    /// <summary>
    /// Provides the bridge between application and clipboard service (such as Windows system Clipboard) to store, share and retrieve data.
    /// </summary>
    public interface IClipboardProvider
    {
        /// <summary>
        /// Pushes the given data into the clipboard's storage
        /// </summary>
        /// <param name="obj">The data to store</param>
        /// <param name="copy">A flag to specify if data should be copied</param>
        void Push(object obj, bool copy = true);
        /// <summary>
        /// Pushes the given data into the buffer's storage using the given format
        /// </summary>
        /// <param name="obj">The data to store</param>
        /// <param name="dataFormat">The format used to store data</param>
        void Push(object obj, DataFormat dataFormat);

        /// <summary>
        /// Loads the value stored in the buffer
        /// </summary>
        /// <param name="dataFormat"></param>
        /// <returns></returns>
        object Pop(DataFormat dataFormat);        
    }

    public enum DataFormat
    {
        /// <summary>
        /// Specifies a bitmap format
        /// </summary>
        Bitmap,
        /// <summary>
        /// Specifies a comma-separated value (CSV) format, which is a common interchange format used by spreadsheets
        /// </summary>
        CommaSeparatedValue,
        /// <summary>
        /// Specifies the device-independent bitmap (DIB) format
        /// </summary>
        Dib,
        /// <summary>
        /// Specifies the Data Interchange Format (DIF)
        /// </summary>
        Dif,
        /// <summary>
        /// Specifies the file drop format
        /// </summary>
        FileDrop,
        /// <summary>
        /// Specifies text in the HTML format
        /// </summary>
        Html,
        /// <summary>
        /// Specifies the standard Unicode text format
        /// </summary>
        UnicodeText,
        /// <summary>
        /// Specifies the Tagged Image File Format (TIFF)
        /// </summary>
        Tiff,
        /// <summary>
        /// Specifies the standard ANSI text format
        /// </summary>
        Text,
        /// <summary>
        /// Specifies a format that encapsulates any type of object
        /// </summary>
        Serializable,
        /// <summary>
        /// Specifies text consisting of Rich Text Format (RTF) data
        /// </summary>
        Rtf,
        /// <summary>
        /// Specifies the Resource Interchange File Format (RIFF) audio format
        /// </summary>
        Riff
    }
}