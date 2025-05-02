public static class Register
{
    // General purpose registers
    public static string X0 => "x0";
    public static string X1 => "x1";
    public static string X2 => "x2";
    public static string X3 => "x3";
    public static string X4 => "x4";
    public static string X5 => "x5";
    public static string X6 => "x6";
    public static string X7 => "x7";
    public static string X8 => "x8";
    public static string X9 => "x9";
    public static string HP => "x10"; // Heap pointer
    public static string X11 => "x11";
    public static string X12 => "x12";
    public static string X13 => "x13";
    public static string X14 => "x14";
    public static string X15 => "x15";
    public static string X16 => "x16";
    public static string X17 => "x17";
    public static string X18 => "x18";
    public static string X19 => "x19";
    public static string X20 => "x20";
    public static string X21 => "x21";
    public static string X22 => "x22";
    public static string X23 => "x23";
    public static string X24 => "x24";
    public static string X25 => "x25";
    public static string X26 => "x26";
    public static string X27 => "x27";
    public static string X28 => "x28";
    public static string X29 => "x29";
    public static string X30 => "x30";

    // 32 bit registers
    public static string W0 => "w0";
    public static string W1 => "w1";
    public static string W2 => "w2";

    // Special registers
    public static string SP => "sp";
    public static string PC => "pc";
    public static string XZR => "xzr";

    // Aliases
    public static string FP => "x29";  // Frame pointer (alias for X29)
    public static string LR => "x30";  // Link register (alias for X30)

    // SIMD and floating-point registers
    public static string V0 => "v0";
    public static string V1 => "v1";
    public static string V2 => "v2";
    public static string V3 => "v3";
    public static string V4 => "v4";
    public static string V5 => "v5";
    public static string V6 => "v6";
    public static string V7 => "v7";
    public static string V8 => "v8";
    public static string V9 => "v9";
    public static string V10 => "v10";
    public static string V11 => "v11";
    public static string V12 => "v12";
    public static string V13 => "v13";
    public static string V14 => "v14";
    public static string V15 => "v15";
    public static string V16 => "v16";
    public static string V17 => "v17";
    public static string V18 => "v18";
    public static string V19 => "v19";
    public static string V20 => "v20";
    public static string V21 => "v21";
    public static string V22 => "v22";
    public static string V23 => "v23";
    public static string V24 => "v24";
    public static string V25 => "v25";
    public static string V26 => "v26";
    public static string V27 => "v27";
    public static string V28 => "v28";
    public static string V29 => "v29";
    public static string V30 => "v30";
    public static string V31 => "v31";

    // FPRs
    // S -> simple registers
    // D -> double registers
    // Q -> quad registers

    public static string S0 => "s0";
    public static string S1 => "s1";
    public static string S2 => "s2";
    public static string S3 => "s3";
    public static string S4 => "s4";
    public static string S5 => "s5";
    public static string S6 => "s6";
    public static string S7 => "s7";
    public static string S8 => "s8";
    public static string S9 => "s9";
    public static string S10 => "s10";
    public static string S11 => "s11";
    public static string S12 => "s12";
    public static string S13 => "s13";
    public static string S14 => "s14";
    public static string S15 => "s15";
    public static string S16 => "s16";
    public static string S17 => "s17";
    public static string S18 => "s18";
    public static string S19 => "s19";
    public static string S20 => "s20";
    public static string S21 => "s21";
    public static string S22 => "s22";
    public static string S23 => "s23";
    public static string S24 => "s24";
    public static string S25 => "s25";
    public static string S26 => "s26";
    public static string S27 => "s27";
    public static string S28 => "s28";
    public static string S29 => "s29";
    public static string S30 => "s30";
    public static string S31 => "s31";

    public static string D0 => "d0";
    public static string D1 => "d1";
    public static string D2 => "d2";
    public static string D3 => "d3";
    public static string D4 => "d4";
    public static string D5 => "d5";
    public static string D6 => "d6";
    public static string D7 => "d7";
    public static string D8 => "d8";
    public static string D9 => "d9";
    public static string D10 => "d10";
    public static string D11 => "d11";
    public static string D12 => "d12";
    public static string D13 => "d13";
    public static string D14 => "d14";
    public static string D15 => "d15";
    public static string D16 => "d16";
    public static string D17 => "d17";
    public static string D18 => "d18";
    public static string D19 => "d19";
    public static string D20 => "d20";
    public static string D21 => "d21";
    public static string D22 => "d22";
    public static string D23 => "d23";
    public static string D24 => "d24";
    public static string D25 => "d25";
    public static string D26 => "d26";
    public static string D27 => "d27";
    public static string D28 => "d28";
    public static string D29 => "d29";
    public static string D30 => "d30";
    public static string D31 => "d31";

    public static string Q0 => "q0";
    public static string Q1 => "q1";
    public static string Q2 => "q2";
    public static string Q3 => "q3";
    public static string Q4 => "q4";
    public static string Q5 => "q5";
    public static string Q6 => "q6";
    public static string Q7 => "q7";
    public static string Q8 => "q8";
    public static string Q9 => "q9";
    public static string Q10 => "q10";
    public static string Q11 => "q11";
    public static string Q12 => "q12";
    public static string Q13 => "q13";
    public static string Q14 => "q14";
    public static string Q15 => "q15";


}