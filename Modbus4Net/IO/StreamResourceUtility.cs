using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modbus4Net.IO
{
    internal static class StreamResourceUtility
    {
        internal static string ReadLine(IStreamResource stream)
        {
            var result = new StringBuilder();
            byte[] singleByteBuffer = new byte[1];

            do
            {
                if (stream.Read(singleByteBuffer, 0, 1) == 0)
                    continue;

                result.Append(Encoding.UTF8.GetChars(singleByteBuffer).First());
            } while (!result.ToString().EndsWith(Modbus.NewLine));

            return result.ToString().Substring(0, result.Length - Modbus.NewLine.Length);
        }

        internal static async Task<string> ReadLineAsync(IStreamResource stream)
        {
            var result = new StringBuilder();
            byte[] singleByteBuffer = new byte[1];

            do
            {
                if (await stream.ReadAsync(singleByteBuffer, 0, 1) == 0)
                    continue;

                result.Append(Encoding.UTF8.GetChars(singleByteBuffer).First());
            } while (!result.ToString().EndsWith(Modbus.NewLine));

            return result.ToString().Substring(0, result.Length - Modbus.NewLine.Length);
        }
    }
}
