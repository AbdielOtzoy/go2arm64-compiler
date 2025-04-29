.data
heap: .space 4096
.text
.global _start
_start:
    adr x10, heap
// Print statement
MOV x0, #16
SUB sp, sp, x0
// visiting args
// Constant: 5
MOV x0, #5
STR x0, [SP, #-8]!
// Constant: 6
MOV x0, #6
STR x0, [SP, #-8]!
MOV x0, #32
ADD sp, sp, x0
MOV x0, #8
SUB x0, sp, x0
ADR x1, L1
STR x1, [SP, #-8]!
STR x29, [SP, #-8]!
ADD x29, x0, xzr
MOV x0, #24
SUB sp, sp, x0
// Calling function: sumar
BL sumar
// Function call finished: sumar
L1:
MOV x4, #32
SUB x4, x29, x4
LDR x4, [x4, #0]
MOV x1, #8
SUB x1, x29, x1
LDR x29, [x1, #0]
MOV x0, #40
ADD sp, sp, x0
STR x4, [SP, #-8]!
// Returning from function: sumar
// Popping
LDR x0, [SP], #8
// Printing integer
MOV X0, x0
BL print_integer
MOV x0, #0
MOV x8, #93
SVC #0



 // Function Definitions
// Function: sumar
sumar:
// Function prologue
STR x30, [SP, #-8]!
// Return statement
MOV x0, #16
SUB x0, x29, x0
LDR x0, [x0, #0]
STR x0, [SP, #-8]!
MOV x0, #24
SUB x0, x29, x0
LDR x0, [x0, #0]
STR x0, [SP, #-8]!
// Adding
LDR x1, [SP], #8
LDR x0, [SP], #8
ADD x0, x0, x1
STR x0, [SP, #-8]!
LDR x0, [SP], #8
MOV x1, #32
SUB x1, x29, x1
STR x0, [x1, #0]
B L0
// End of return statement
L0:
// Epilogue
ADD x0, sp, xzr
LDR x30, [x0, #0]
BR x30
// End of function: sumar
// Popping object
// Popping object



 // Standard Library

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

    // Print newline
    mov x0, #1                 // fd = 1 (stdout)
    adr x1, newline            // Address of newline character
    mov x2, #1                 // Length = 1
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
    
minus_sign: .ascii "-"
newline: .ascii "\n"
