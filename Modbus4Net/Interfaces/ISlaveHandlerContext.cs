namespace Modbus4Net
{
    public interface ISlaveHandlerContext
    {
        IModbusFunctionService GetHandler(byte functionCode);
    }
}