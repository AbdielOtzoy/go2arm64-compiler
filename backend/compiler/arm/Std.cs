using System.Collections.Generic;

public class StandardLibrary
{
    private readonly HashSet<string> UsedFunctions = new HashSet<string>();
    private readonly HashSet<string> UsedSymbols = new HashSet<string>();

    public void Use(string function)
    {
        UsedFunctions.Add(function);

        if (function == "print_integer")
        {
            UsedSymbols.Add("minus_sign");
        }
        else if (function == "print_double")
        {
            UsedSymbols.Add("dot_char");
            UsedSymbols.Add("zero_char");
            UsedSymbols.Add("minus_sign");
        }
        else if (function == "print_rune")
        {
        }
        else if (function == "print_string")
        {
        }
        else if (function == "print_bool")
        {
            UsedSymbols.Add("true_string");
            UsedSymbols.Add("false_string");
        }
        else if (function == "print_newline")
        {
            UsedSymbols.Add("newline");
        }
        else if (function == "print_array") 
        {
            UsedSymbols.Add("open_bracket");
            UsedSymbols.Add("close_bracket");
            UsedSymbols.Add("comma_space");
            UsedSymbols.Add("newline");
            UsedSymbols.Add("minus_sign");
        }
    }

    public string GetFunctionDefinitions()
    {
        var functions = new List<string>();

        foreach (var function in UsedFunctions)
        {
            if (FunctionDefinitions.TryGetValue(function, out var definition))
            {
                functions.Add(definition);
            }
        }

        var fnDefs = string.Join("\n", functions);

        var symbols = new List<string>();
        foreach (var symbol in UsedSymbols)
        {
            if (Symbols.TryGetValue(symbol, out var definition))
            {
                symbols.Add(definition);
            }
        }
        var symbolsDefs = string.Join("\n", symbols);

        return fnDefs + "\n" + symbolsDefs;
    }

    private readonly static Dictionary<string, string> FunctionDefinitions = new Dictionary<string, string>
    {
        { "print_integer", @"
//--------------------------------------------------------------
// print_integer - Prints a signed integer to stdout
//
// Input:
//   x0 - The integer value to print
//--------------------------------------------------------------
print_integer:
    // Save registers
    stp x29, x30, [sp, #-16]!  // Save frame pointer and link register
    stp x19, x20, [sp, #-16]!  // Save callee-saved registers
    stp x21, x22, [sp, #-16]!
    stp x23, x24, [sp, #-16]!
    stp x25, x26, [sp, #-16]!
    stp x27, x28, [sp, #-16]!
    
    // Check if number is negative
    mov x19, x0                // Save original number
    cmp x19, #0                // Compare with zero
    bge positive_number        // Branch if greater or equal to zero
    
    // Handle negative number
    mov x0, #1                 // fd = 1 (stdout)
    adr x1, minus_sign         // Address of minus sign
    mov x2, #1                 // Length = 1
    mov w8, #64                // Syscall write
    svc #0
    
    neg x19, x19               // Make number positive
    
positive_number:
    // Prepare buffer for converting result to ASCII
    sub sp, sp, #32            // Reserve space on stack
    mov x22, sp                // x22 points to buffer
    
    // Initialize digit counter
    mov x23, #0                // Digit counter
    
    // Handle special case for zero
    cmp x19, #0
    bne convert_loop
    
    // If number is zero, just write '0'
    mov w24, #48               // ASCII '0'
    strb w24, [x22, x23]       // Store in buffer
    add x23, x23, #1           // Increment counter
    b print_result             // Skip conversion loop
    
convert_loop:
    // Divide the number by 10
    mov x24, #10
    udiv x25, x19, x24         // x25 = x19 / 10 (quotient)
    msub x26, x25, x24, x19    // x26 = x19 - (x25 * 10) (remainder)
    
    // Convert remainder to ASCII and store in buffer
    add x26, x26, #48          // Convert to ASCII ('0' = 48)
    strb w26, [x22, x23]       // Store digit in buffer
    add x23, x23, #1           // Increment digit counter
    
    // Prepare for next iteration
    mov x19, x25               // Quotient becomes the new number
    cbnz x19, convert_loop     // If number is not zero, continue
    
    // Reverse the buffer since digits are in reverse order
    mov x27, #0                // Start index
reverse_loop:
    sub x28, x23, x27          // x28 = length - current index
    sub x28, x28, #1           // x28 = length - current index - 1
    
    cmp x27, x28               // Compare indices
    bge print_result           // If crossed, finish reversing
    
    // Swap characters
    ldrb w24, [x22, x27]       // Load character from start
    ldrb w25, [x22, x28]       // Load character from end
    strb w25, [x22, x27]       // Store end character at start
    strb w24, [x22, x28]       // Store start character at end
    
    add x27, x27, #1           // Increment start index
    b reverse_loop             // Continue reversing
    
print_result:
    // Print the result
    mov x0, #1                 // fd = 1 (stdout)
    mov x1, x22                // Buffer address
    mov x2, x23                // Buffer length
    mov w8, #64                // Syscall write
    svc #0
    
    // Clean up and restore registers
    add sp, sp, #32            // Free buffer space
    ldp x27, x28, [sp], #16    // Restore callee-saved registers
    ldp x25, x26, [sp], #16
    ldp x23, x24, [sp], #16
    ldp x21, x22, [sp], #16
    ldp x19, x20, [sp], #16
    ldp x29, x30, [sp], #16    // Restore frame pointer and link register
    ret                        // Return to caller
    "
    },

    {
        "print_string", @"
//--------------------------------------------------------------
// print_string - Prints a null-terminated string to stdout
//
// Input:
//   x0 - The address of the null-terminated string to print
//--------------------------------------------------------------
print_string:
    // Save link register and other registers we'll use
    stp     x29, x30, [sp, #-16]!
    stp     x19, x20, [sp, #-16]!
    
    // x19 will hold the string address
    mov     x19, x0
    
print_loop:
    // Load a byte from the string
    ldrb    w20, [x19]
    
    // Check if it's the null terminator (0)
    cbz     w20, print_done
    
    // Prepare for write syscall
    mov     x0, #1              // File descriptor: 1 for stdout
    mov     x1, x19             // Address of the character to print
    mov     x2, #1              // Length: 1 byte
    mov     x8, #64             // syscall: write (64 on ARM64)
    svc     #0                  // Make the syscall
    
    // Move to the next character
    add     x19, x19, #1
    
    // Continue the loop
    b       print_loop
    
print_done:
    // Restore saved registers
    ldp     x19, x20, [sp], #16
    ldp     x29, x30, [sp], #16
    ret
    // Return to the caller
    "},
    { "print_bool", @"
//--------------------------------------------------------------
// print_bool - Prints a boolean value to stdout as true or false
//
// Input:
//   x0 - The boolean value to print (0 = false, non-zero = true)
//--------------------------------------------------------------
print_bool:
    // Save registers
    stp x29, x30, [sp, #-16]!  // Save frame pointer and link register
    stp x19, x20, [sp, #-16]!  // Save callee-saved registers
    stp x21, x22, [sp, #-16]!  // Save additional registers
    
    // Save the boolean value
    mov x19, x0
    
    // Check if value is 0 (false) or non-zero (true)
    cmp x19, #0
    beq print_bool_false
    
    // Print true if non-zero
    adr x21, true_string       // Load address of true string
    mov x22, #4                // Length of ""true"" (4 chars)
    b print_bool_string
    
print_bool_false:
    // Print false if zero
    adr x21, false_string      // Load address of false string
    mov x22, #5                // Length of ""false"" (5 chars)
    
print_bool_string:
    // Print the string (true or false)
    mov x0, #1                 // File descriptor: 1 for stdout
    mov x1, x21                // Address of the string to print
    mov x2, x22                // Length of the string
    mov x8, #64                // syscall: write (64 on ARM64)
    svc #0                     // Make the syscall
    
    // Restore registers
    ldp x21, x22, [sp], #16    // Restore additional registers
    ldp x19, x20, [sp], #16    // Restore callee-saved registers
    ldp x29, x30, [sp], #16    // Restore frame pointer and link register
    ret

" },
    {
        "print_double", @"
print_double:
    // Save context
    stp x29, x30, [sp, #-16]!    
    stp x19, x20, [sp, #-16]!
    stp x21, x22, [sp, #-16]!
    stp x23, x24, [sp, #-16]!
    stp x25, x26, [sp, #-16]!
    stp x27, x28, [sp, #-16]!
    
    // Check if number is negative
    fmov x19, d0
    tst x19, #(1 << 63)       // Check sign bit
    beq skip_minus

    // Print minus sign
    mov x0, #1
    adr x1, minus_sign
    mov x2, #1
    mov x8, #64
    svc #0

    // Make value positive
    fneg d0, d0

skip_minus:
    // Convert integer part
    fcvtzs x19, d0             // x19 = int(d0)
    
    // --- START of embedded integer printing logic ---
    // Prepare buffer for converting integer to ASCII
    sub sp, sp, #32            // Reserve space on stack
    mov x22, sp                // x22 points to buffer
    
    // Initialize digit counter
    mov x23, #0                // Digit counter
    
    // Handle special case for zero
    cmp x19, #0
    bne int_convert_loop
    
    // If number is zero, just write '0'
    mov w24, #48               // ASCII '0'
    strb w24, [x22, x23]       // Store in buffer
    add x23, x23, #1           // Increment counter
    b int_print_result         // Skip conversion loop
    
int_convert_loop:
    // Divide the number by 10
    mov x24, #10
    udiv x25, x19, x24         // x25 = x19 / 10 (quotient)
    msub x26, x25, x24, x19    // x26 = x19 - (x25 * 10) (remainder)
    
    // Convert remainder to ASCII and store in buffer
    add w26, w26, #48          // Convert to ASCII ('0' = 48)
    strb w26, [x22, x23]       // Store digit in buffer
    add x23, x23, #1           // Increment digit counter
    
    // Prepare for next iteration
    mov x19, x25               // Quotient becomes the new number
    cbnz x19, int_convert_loop // If number is not zero, continue
    
    // Reverse the buffer since digits are in reverse order
    mov x27, #0                // Start index
int_reverse_loop:
    sub x28, x23, x27          // x28 = length - current index
    sub x28, x28, #1           // x28 = length - current index - 1
    
    cmp x27, x28               // Compare indices
    bge int_print_result       // If crossed, finish reversing
    
    // Swap characters
    ldrb w24, [x22, x27]       // Load character from start
    ldrb w25, [x22, x28]       // Load character from end
    strb w25, [x22, x27]       // Store end character at start
    strb w24, [x22, x28]       // Store start character at end
    
    add x27, x27, #1           // Increment start index
    b int_reverse_loop         // Continue reversing
    
int_print_result:
    // Print the integer part
    mov x0, #1                 // fd = 1 (stdout)
    mov x1, x22                // Buffer address
    mov x2, x23                // Buffer length
    mov w8, #64                // Syscall write
    svc #0
    
    // Free buffer space
    add sp, sp, #32
    // --- END of embedded integer printing logic ---

    // Print dot '.'
    mov x0, #1
    adr x1, dot_char
    mov x2, #1
    mov x8, #64
    svc #0

    // Get fractional part: frac = d0 - float(int(d0))
    frintm d4, d0             // d4 = floor(d0)
    fsub d2, d0, d4           // d2 = d0 - floor(d0) (exact fraction)

    // Multiply by 1_000_000 (6 decimals)
    movz x1, #0x000F, lsl #16
    movk x1, #0x4240, lsl #0   // x1 = 1000000
    scvtf d3, x1              // d3 = 1000000.0
    fmul d2, d2, d3           // d2 = frac * 1_000_000
    
    // Round to nearest integer to avoid precision errors
    frintn d2, d2             // d2 = round(d2)
    fcvtzs x20, d2             // x20 = int(d2) (fractional part)

    // Print leading zeros if necessary
    movz x21, #0x0001, lsl #16
    movk x21, #0x86A0, lsl #0  // x21 = 100000
    mov x22, #0               // initialize zero counter
    mov x23, #10              // constant for division

leading_zero_loop:
    udiv x24, x20, x21        // x24 = x20 / x21
    cbnz x24, done_leading_zeros  // If there's a non-zero digit, exit loop

    // Print '0'
    mov x0, #1
    adr x1, zero_char
    mov x2, #1
    mov x8, #64
    svc #0

    udiv x21, x21, x23        // x21 /= 10
    add x22, x22, #1          // increment zero counter
    cmp x21, #0               // check if we reached the end
    beq print_remaining       // if divisor is 0, jump to print the rest
    b leading_zero_loop

done_leading_zeros:
    // --- START of embedded integer printing logic for fractional part ---
    // Save the fractional part
    mov x19, x20
    
    // Prepare buffer for converting integer to ASCII
    sub sp, sp, #32            // Reserve space on stack
    mov x22, sp                // x22 points to buffer
    
    // Initialize digit counter
    mov x23, #0                // Digit counter
    
    // Handle special case for zero
    cmp x19, #0
    bne frac_convert_loop
    
    // If number is zero, just write '0'
    mov w24, #48               // ASCII '0'
    strb w24, [x22, x23]       // Store in buffer
    add x23, x23, #1           // Increment counter
    b frac_print_result        // Skip conversion loop
    
frac_convert_loop:
    // Divide the number by 10
    mov x24, #10
    udiv x25, x19, x24         // x25 = x19 / 10 (quotient)
    msub x26, x25, x24, x19    // x26 = x19 - (x25 * 10) (remainder)
    
    // Convert remainder to ASCII and store in buffer
    add w26, w26, #48          // Convert to ASCII ('0' = 48)
    strb w26, [x22, x23]       // Store digit in buffer
    add x23, x23, #1           // Increment digit counter
    
    // Prepare for next iteration
    mov x19, x25               // Quotient becomes the new number
    cbnz x19, frac_convert_loop // If number is not zero, continue
    
    // Reverse the buffer since digits are in reverse order
    mov x27, #0                // Start index
frac_reverse_loop:
    sub x28, x23, x27          // x28 = length - current index
    sub x28, x28, #1           // x28 = length - current index - 1
    
    cmp x27, x28               // Compare indices
    bge frac_print_result      // If crossed, finish reversing
    
    // Swap characters
    ldrb w24, [x22, x27]       // Load character from start
    ldrb w25, [x22, x28]       // Load character from end
    strb w25, [x22, x27]       // Store end character at start
    strb w24, [x22, x28]       // Store start character at end
    
    add x27, x27, #1           // Increment start index
    b frac_reverse_loop        // Continue reversing
    
frac_print_result:
    // Print the fractional part
    mov x0, #1                 // fd = 1 (stdout)
    mov x1, x22                // Buffer address
    mov x2, x23                // Buffer length
    mov w8, #64                // Syscall write
    svc #0
    
    // Free buffer space
    add sp, sp, #32
    // --- END of embedded integer printing logic for fractional part ---
    
    b exit_function

print_remaining:
    // Special case when fractional part is 0 after printing zeros
    cmp x20, #0
    bne exit_function
    
    // We've already printed all necessary zeros
    // No need to print anything else

exit_function:
    // Restore context
    ldp x27, x28, [sp], #16
    ldp x25, x26, [sp], #16
    ldp x23, x24, [sp], #16
    ldp x21, x22, [sp], #16
    ldp x19, x20, [sp], #16
    ldp x29, x30, [sp], #16
    ret
    "},
    {
        "print_rune", @"
        //--------------------------------------------------------------
// print_rune - Imprime un carácter en stdout
//
// Input:
//   x0 - El valor del carácter a imprimir
//--------------------------------------------------------------
print_rune:
    // Guardar los registros
    stp     x29, x30, [sp, #-16]!  // Guardar frame pointer y link register
    stp     x19, x20, [sp, #-16]!  // Guardar registros preservados
    
    // Guardar el carácter en un registro preservado
    mov     x19, x0
    
    // Reservar espacio en la pila para el carácter
    sub     sp, sp, #16
    
    // Guardar el carácter en el buffer de la pila
    strb    w19, [sp]
    
    // Llamada al sistema write
    mov     x0, #1                 // Descriptor de archivo: 1 para stdout
    mov     x1, sp                 // Dirección del carácter a imprimir
    mov     x2, #1                 // Longitud: 1 byte
    mov     x8, #64                // Syscall: write (64 en ARM64)
    svc     #0                     // Realizar la llamada al sistema
    
    // Liberar el espacio en la pila
    add     sp, sp, #16

    // Restaurar los registros
    ldp     x19, x20, [sp], #16    // Restaurar registros preservados
    ldp     x29, x30, [sp], #16    // Restaurar frame pointer y link register
    ret                            // Retornar al llamador
        "
    },
    {
        "print_newline", @"
        //--------------------------------------------------------------
// print_newline - Imprime un salto de línea en stdout
//
// Input:
//   Ninguno
//--------------------------------------------------------------
print_newline:
    // Guardar los registros
    stp     x29, x30, [sp, #-16]!  // Guardar frame pointer y link register
    stp     x19, x20, [sp, #-16]!  // Guardar registros preservados
    stp     x21, x22, [sp, #-16]!  // Guardar registros adicionales
    stp     x23, x24, [sp, #-16]!  // Guardar registros adicionales
    stp     x25, x26, [sp, #-16]!  // Guardar registros adicionales
    stp     x27, x28, [sp, #-16]!  // Guardar registros adicionales
    // Imprimir salto de línea
    mov     x0, #1                 // Descriptor de archivo: 1 para stdout
    adr     x1, newline            // Dirección del carácter de salto de línea
    mov     x2, #1                 // Longitud: 1 byte
    mov     x8, #64                // Syscall: write (64 en ARM64)
    svc     #0                     // Realizar la llamada al sistema
    // Restaurar los registros
    ldp     x27, x28, [sp], #16    // Restaurar registros adicionales
    ldp     x25, x26, [sp], #16    // Restaurar registros adicionales
    ldp     x23, x24, [sp], #16    // Restaurar registros adicionales
    ldp     x21, x22, [sp], #16    // Restaurar registros adicionales
    ldp     x19, x20, [sp], #16    // Restaurar registros preservados
    ldp     x29, x30, [sp], #16    // Restaurar frame pointer y link register
    ret                            // Retornar al llamador
        "},
        {
            "print_array", @"
            //--------------------------------------------------------------
// print_array - Imprime todos los elementos de un arreglo
//
// Input:
//   x0 - Dirección base del arreglo (debe ser un arreglo de enteros .word)
//   w1 - Número de elementos en el arreglo
//
// Ejemplo de uso:
//   adr x0, numeros       // Cargar dirección del arreglo
//   mov w1, #5            // Indicar que tiene 5 elementos
//   bl print_array        // Llamar a la función
//--------------------------------------------------------------
print_array:
    // Guardar registros
    stp x29, x30, [sp, #-16]!  // Guardar frame pointer y link register
    stp x19, x20, [sp, #-16]!  // Guardar registros que usaremos
    stp x21, x22, [sp, #-16]!
    stp x23, x24, [sp, #-16]!
    
    // Inicializar variables
    mov x19, x0                // x19 = dirección base del arreglo
    mov w20, w1                // w20 = número de elementos
    mov w21, #0                // w21 = índice actual (inicia en 0)
    mov w22, #4                // w22 = tamaño de cada elemento (.word = 4 bytes)
    
    // Imprimir corchete de apertura
    mov x0, #1                 // fd = 1 (stdout)
    adr x1, open_bracket       // Dirección del corchete de apertura
    mov x2, #1                 // Longitud = 1
    mov w8, #64                // Syscall write
    svc #0
    
    // Verificar si el arreglo está vacío
    cbz w20, empty_array
    
array_loop:
    // Calcular dirección del elemento actual
    umull x23, w21, w22        // x23 = índice * tamaño_elemento
    add x23, x19, x23          // x23 = dirección_base + desplazamiento
    
    // Cargar el valor del elemento actual
    ldr w0, [x23]              // w0 = valor del elemento
    
    // Guardar registros importantes antes de llamar a función
    stp x19, x20, [sp, #-16]!
    stp x21, x22, [sp, #-16]!
    
    // Imprimir el valor del elemento
    bl print_integer
    
    // Restaurar registros después de la llamada
    ldp x21, x22, [sp], #16
    ldp x19, x20, [sp], #16
    
    // Incrementar índice
    add w21, w21, #1
    
    // Verificar si hay más elementos para imprimir
    cmp w21, w20
    bge array_end              // Si índice >= tamaño, terminar
    
    // Imprimir separador (coma y espacio)
    mov x0, #1                 // fd = 1 (stdout)
    adr x1, comma_space        // Dirección de "", ""
    mov x2, #2                 // Longitud = 2
    mov w8, #64                // Syscall write
    svc #0
    
    // Continuar el bucle
    b array_loop
    
array_end:
    // Imprimir corchete de cierre
    mov x0, #1                 // fd = 1 (stdout)
    adr x1, close_bracket      // Dirección del corchete de cierre
    mov x2, #1                 // Longitud = 1
    mov w8, #64                // Syscall write
    svc #0
    
    // Imprimir salto de línea
    mov x0, #1                 // fd = 1 (stdout)
    adr x1, newline            // Dirección del salto de línea
    mov x2, #1                 // Longitud = 1
    mov w8, #64                // Syscall write
    svc #0
    
    // Restaurar registros y retornar
    ldp x23, x24, [sp], #16
    ldp x21, x22, [sp], #16
    ldp x19, x20, [sp], #16
    ldp x29, x30, [sp], #16
    ret
    
empty_array:
    // Si el arreglo está vacío, imprimir el corchete de cierre directamente
    mov x0, #1                 // fd = 1 (stdout)
    adr x1, close_bracket      // Dirección del corchete de cierre
    mov x2, #1                 // Longitud = 1
    mov w8, #64                // Syscall write
    svc #0
    
    // Imprimir salto de línea
    mov x0, #1                 // fd = 1 (stdout)
    adr x1, newline            // Dirección del salto de línea
    mov x2, #1                 // Longitud = 1
    mov w8, #64                // Syscall write
    svc #0
    
    // Restaurar registros y retornar
    ldp x23, x24, [sp], #16
    ldp x21, x22, [sp], #16
    ldp x19, x20, [sp], #16
    ldp x29, x30, [sp], #16
    ret
"
        }
        
        };

    private readonly static Dictionary<string, string> Symbols = new Dictionary<string, string>
    {
        { "minus_sign", @"minus_sign: .ascii ""-""" },
        { "dot_char", @"dot_char: .ascii "".""" },
        { "zero_char", @"zero_char: .ascii ""0""" },
        { "newline", @"newline: .ascii ""\n""" },
        { "true_string", @"true_string: .ascii ""true""" },
        { "false_string", @"false_string: .ascii ""false""" },
        { "open_bracket", @"open_bracket: .ascii ""[""" },
        { "close_bracket", @"close_bracket: .ascii ""]""" },
        { "comma_space", @"comma_space: .ascii "", """ }
    };
}