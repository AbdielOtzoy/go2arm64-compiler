public static class Utils {
    public static List<byte> StringTo1ByteArray(string str) {
        var resultado = new List<byte>();
        int elementIndex = 0;

        while(elementIndex < str.Length) {
            resultado.Add((byte)str[elementIndex]);
            elementIndex++;
        }

        resultado.Add(0); // null terminator
        return resultado;
    }

    public static byte StringToByte(string str) {

        return (byte)str[1];
    }

   public static (string, string) GetFloatBitParts(float number)
{
    // Convertimos el float a su representación binaria (IEEE 754) como uint
    uint bits = BitConverter.ToUInt32(BitConverter.GetBytes(number), 0);

    // Extraemos los bits inferiores (lower 16) y superiores (upper 16)
    ushort lower = (ushort)(bits & 0xFFFF);          // Máscara para los 16 bits bajos
    ushort upper = (ushort)((bits >> 16) & 0xFFFF);  // Shift para los 16 bits altos

    // Formateamos el resultado en hexadecimal
    return ($"0x{lower:X4}", $"0x{upper:X4}");
}
    public static (string bits0_15, string bits16_31, string bits32_47, string bits48_63) 
            GetDoubleComponents(double value)
        {
            // Get the raw 64-bit representation
            byte[] bytes = BitConverter.GetBytes(value);
            ulong longValue = BitConverter.ToUInt64(bytes, 0);

            // Extract each 16-bit segment
            ushort bits0_15 = (ushort)(longValue & 0xFFFF);
            ushort bits16_31 = (ushort)((longValue >> 16) & 0xFFFF);
            ushort bits32_47 = (ushort)((longValue >> 32) & 0xFFFF);
            ushort bits48_63 = (ushort)((longValue >> 48) & 0xFFFF);

            // Format as 4-character hex strings with 0x prefix
            return (
                $"0x{bits0_15:X4}",
                $"0x{bits16_31:X4}",
                $"0x{bits32_47:X4}",
                $"0x{bits48_63:X4}"
            );
        }

}